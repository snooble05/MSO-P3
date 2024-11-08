using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using MSO_P3;

namespace Test_MSO_P3
{
	public class InputTestParser
	{
		public CommandField commandField;

		public InputTestParser()
		{
			commandField = new CommandField(new Grid(new Character(new Point(0,0), Direction.ViewDir.East), 8, new List<Point>() { }));
		}

		public void setInputText(string text)
		{
			commandField.setInputText(text);
		}
	}
	public class UnitTestInput
	{
		private readonly InputTestParser _parser;

		public UnitTestInput()
		{
			_parser = new InputTestParser();
		}
		[Fact]
		public void Input_ValidCommands()
		{
			string inputText = "move 7\nturn left\n";

			_parser.setInputText(inputText);

			_parser.commandField.runInput(null, EventArgs.Empty);

			Assert.Collection(_parser.commandField.Commands,
				item => { var moveCommand = Assert.IsType<MoveCommand>(item); Assert.Equal(7, moveCommand.Steps); },
				item => { var turnCommand = Assert.IsType<TurnCommand>(item); Assert.Equal("left", turnCommand.TurningDirection); }
				);
		}

		[Fact]
		public void Input_InvalidCommands()
		{
			string inputText = "fly 10";
			
			_parser.setInputText(inputText);

			Action a = () => _parser.commandField.runInput(null, EventArgs.Empty);

			Assert.Throws<ArgumentException>(a);
		}

		[Fact]
		public void Input_EmptyLines()
		{
			string inputText = "move 7\n\nturn left\n\n";

			_parser.setInputText(inputText);

			_parser.commandField.runInput(null, EventArgs.Empty);

			Assert.Collection(_parser.commandField.Commands,
				item => { var moveCommand = Assert.IsType<MoveCommand>(item); Assert.Equal(7, moveCommand.Steps); },
				item => { var turnCommand = Assert.IsType<TurnCommand>(item); Assert.Equal("left", turnCommand.TurningDirection); }
				);
		}

		[Fact]
		public void Input_RepeatNesting()
		{
			string inputText = "move 3\nrepeat 2 times\n Turn right\n move 1";

			_parser.setInputText(inputText);

			_parser.commandField.runInput(null, EventArgs.Empty);

			Assert.Collection(_parser.commandField.Commands,
				item => { var moveCommand = Assert.IsType<MoveCommand>(item); Assert.Equal(3, moveCommand.Steps); },
				item => { var repeatCommand = Assert.IsType<RepeatCommand>(item); Assert.Equal(2, repeatCommand.RepeatAmount);
					Assert.Collection(repeatCommand.Commands,
						item => { var turnCommand = Assert.IsType<TurnCommand>(item); Assert.Equal("right", turnCommand.TurningDirection); },
						item => { var moveCommand = Assert.IsType<MoveCommand>(item); Assert.Equal(1, moveCommand.Steps); }
					);
				}
			);
		}
	}
}
