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
		private Label _output;
		public List<ICommand> Commands
		{
			get { return _commands; }
		}

		public CommandField()
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

			_output = new Label();
			_output.BackColor = Color.PaleTurquoise;
			_output.ForeColor = Color.Black;
			_output.Text = "Output: \n";
			_output.Font = new Font("Lucida Console", 12f);
			_output.Padding = new Padding(10);
			this.Controls.Add(_output);

			this.Resize += resize;
			this.resize(null!, null!);
		}

		private void runInput(object o, EventArgs ea)
		{
			_commands.Clear();
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
						default:
							throw new ArgumentException("Unknown command given");
					}
				}
			}
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
						default:
							throw new ArgumentException("Unkown command given");
					}

					if (i + 1 == remainingLines.Length) break;
					//Debug.WriteLine(remainingLines[i + 1]);
					if (!(remainingLines[i + 1].StartsWith(' ') || remainingLines[i + 1].StartsWith('\t')))
					{
						break;
					}
				}
			}
			return commands;
		}

		private void resize(object o, EventArgs ea)
		{
			_commandInput.Size = new Size(this.Size.Width - 10, (this.Size.Height / 2) - 80);
			_runButton.Location = new Point(30, (this.Size.Height / 2) - 70);
			_output.Location = new Point(0, (this.ClientSize.Height / 2) + 20);
			_output.Size = new Size(this.Size.Width - 10, (this.ClientSize.Height / 2) - 63);
		}
	}
}
