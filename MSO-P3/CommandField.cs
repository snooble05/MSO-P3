using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using System.Diagnostics;

namespace MSO_P3
{
	public class CommandField : Panel
	{
		private List<ICommand> _commands;
		private RichTextBox _commandInput;
		private Button _runButton;
		private Button _clearButton;
		private Label _output;
		private Character _character;
		private Grid _grid;
		public List<ICommand> Commands
		{
			get { return _commands; }
		}
		public Label Output
		{
			get { return _output; }
		}
		public Character Character 
		{ 
			get { return _character; }
		}
		public Grid Grid
		{
			get { return _grid; }
		}

		public CommandField(Grid grid)
		{
			_commands = new List<ICommand>();

			_commandInput = new RichTextBox();
			_commandInput.Text = "";
			_commandInput.Location = new Point(this.Location.X, this.Location.Y);
			_commandInput.AcceptsTab = true;
			_commandInput.Multiline = true;
			_commandInput.ShortcutsEnabled = true;
			this.Controls.Add(_commandInput);

			_runButton = new Button();
			_runButton.Text = "Run";
			_runButton.BackColor = Color.Lavender;
			_runButton.FlatStyle = FlatStyle.Flat;
			_runButton.FlatAppearance.BorderColor = Color.Black;
			_runButton.Size = new Size(100, 40);
			_runButton.Click += runInput;
			this.Controls.Add(_runButton);

			_clearButton = new Button();
			_clearButton.Text = "Clear";
			_clearButton.BackColor = Color.Lavender;
			_clearButton.FlatStyle = FlatStyle.Flat;
			_clearButton.FlatAppearance.BorderColor = Color.Black;
			_clearButton.Size = new Size(100, 40);
			_clearButton.Click += clearField;
			this.Controls.Add(_clearButton);

			_output = new Label();
			_output.BackColor = Color.PaleTurquoise;
			_output.ForeColor = Color.Black;
			_output.Text = "Output:\n";
			_output.Font = new Font("Lucida Console", 12f);
			_output.Padding = new Padding(10);
			this.Controls.Add(_output);

			_grid = grid;
			_character = Grid.Character;

			this.Resize += resize;
			this.resize(null!, null!);
		}

		public void runInput(object o, EventArgs ea)
		{
			_commands.Clear();
			_output.Text = "Output:\n";
			string[] lines = _commandInput.Text.TrimEnd().Split('\n');
			for (int i = 0; i < lines.Length; i++)
			{
				string[] commandString = lines[i].Split([" ", "\t"], StringSplitOptions.None);

				if (!(commandString[0] == String.Empty))
				{
					commandString = commandString.Where(str => !string.IsNullOrEmpty(str)).ToArray();
					string command = commandString[0].ToLower();
					string addOn = commandString[1];
					switch (command)
					{
						case "move":
							Commands.Add(new MoveCommand(Convert.ToInt32(addOn)));
							break;
						case "turn":
							Commands.Add(new TurnCommand(addOn));
							break;
						case "repeat":
							Commands.Add(new RepeatCommand(ParseRepeatCommands(lines.Skip(i + 1).ToArray(), 1), Convert.ToInt32(addOn)));
							break;
						case "repeatuntil":
							Commands.Add(new RepeatUntilCommand(ParseRepeatCommands(lines.Skip(i + 1).ToArray(), 1), Conditions.GetCondition(addOn), Grid));
							break;
						default:
							throw new ArgumentException("Unknown command given");
					}
				}
			}
			foreach (ICommand command in Commands)
			{
				command.Execute(Character);
				if (Character.position.X > Grid.GridSize || Character.position.X < 0 || Character.position.Y > Grid.GridSize || Character.position.Y < 0)
				{
					throw new IndexOutOfRangeException("Character cannot move outside of the grid");
				}
				setOutput(command);
			}
			_output.Text += $"\nEnd State {Character.position} facing ";
			switch (Character.direction)
			{
				case Direction.ViewDir.North:
					_output.Text += "north";
					break;
				case Direction.ViewDir.East:
					_output.Text += "east";
					break;
				case Direction.ViewDir.South:
					_output.Text += "south";
					break;
				case Direction.ViewDir.West:
					_output.Text += "west";
					break;
			}
			Grid.Invalidate();
			hasCleared(Character, Grid);
		}

		private List<ICommand> ParseRepeatCommands(string[] remainingLines, int nestingLevel)
		{
			List<ICommand> commands = new List<ICommand>();
			for(int i = 0; i < remainingLines.Length; i++)
			{
				string[] commandString = remainingLines[i].Split([" ", "\t"], StringSplitOptions.None);
				if (!(commandString[nestingLevel] == String.Empty))
				{
					commandString = commandString.Skip(nestingLevel).ToArray();
					string command = commandString[0].ToLower();
					string addOn = commandString[1];

					switch (command)
					{
						case "move":
							commands.Add(new MoveCommand(Convert.ToInt32(addOn)));
							break;
						case "turn":
							commands.Add(new TurnCommand(addOn));
							break;
						case "repeat":
							commands.Add(new RepeatCommand(ParseRepeatCommands(remainingLines.Skip(i + 1).ToArray(), nestingLevel + 1), Convert.ToInt32(addOn)));
							break;
						case "repeatuntil":
							commands.Add(new RepeatUntilCommand(ParseRepeatCommands(remainingLines.Skip(i + 1).ToArray(), nestingLevel + 1), Conditions.GetCondition(addOn), Grid));
							break;
						default:
							throw new ArgumentException("Unkown command given");
					}

					if (i + 1 == remainingLines.Length) break;
					if (!(remainingLines[i + 1].StartsWith(' ') || remainingLines[i + 1].StartsWith('\t')))
					{
						break;
					}
				}
			}
			return commands;
		}

		private void setOutput(ICommand command)
		{
			if (command is MoveCommand)
			{
				_output.Text += $"Move {((MoveCommand)command).Steps} ";
			}
			else if (command is TurnCommand)
			{
				_output.Text += $"Turn {((TurnCommand)command).TurningDirection} ";
			}
			else if (command is RepeatCommand)
			{
				for (int i = 0; i < ((RepeatCommand)command).RepeatAmount; i++)
				{
					foreach (ICommand repeatedCommand in ((RepeatCommand)command).Commands)
					{
						setOutput(repeatedCommand);
					}
				}
			}
			else if (command is RepeatUntilCommand)
			{
				bool condition = ((RepeatUntilCommand)command).Condition(Character, Grid);
				while (!condition)
				{
					foreach(ICommand repeatedCommand in ((RepeatUntilCommand)command).Commands)
					{
						setOutput(repeatedCommand);
					}
				}
			}
		}

		public void clearField(object o, EventArgs ea)
		{
			_commandInput.Clear();
			Output.Text = "Output:\n";
			Grid.Character.position = new Point(0, 0);
			Grid.Character.direction = Direction.ViewDir.East;
			Grid.Invalidate();
		}

		private void hasCleared(Character c, Grid g)
		{
			if(c.position == g.EndPoint)
			{
				Form clearedPopUp = new Form();
				clearedPopUp.Size = new Size(300, 150);
				clearedPopUp.Text = "Exercise clear!";
				clearedPopUp.BackColor = Color.Azure;
				Label clearedLabel = new Label();
				clearedLabel.Size = clearedPopUp.Size;
				clearedLabel.Text = "You cleared the exercise!";
				clearedLabel.Font = new Font("Lucida Console", 16f);
				clearedPopUp.Controls.Add(clearedLabel);
				clearedPopUp.ShowDialog();
			}
		}

		private void resize(object o, EventArgs ea)
		{
			_commandInput.Size = new Size(this.Size.Width - 10, (this.Size.Height / 2) - 80);
			_runButton.Location = new Point(30, (this.Size.Height / 2) - 70);
			_clearButton.Location = new Point(160, (this.Size.Height / 2) - 70);
			_output.Location = new Point(0, (this.ClientSize.Height / 2) + 20);
			_output.Size = new Size(this.Size.Width - 10, (this.ClientSize.Height / 2) - 63);
		}

		public void setInputText(string text)
		{
			_commandInput.Text = text;
		}
	}
}
