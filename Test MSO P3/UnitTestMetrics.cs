using MSO_P3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_MSO_P3
{
	public class UnitTestMetrics
	{
		#region CalculateNumberOfCommands() Tests
		[Fact]
		public void TestNumberCommands_Empty()
		{
			int commandNumber = Metric.CalculateNumberOfCommands([]);

			Assert.Equal(0, commandNumber);
		}

		[Fact]
		public void TestNumberCommands_NotNested()
		{
			List<ICommand> commands = [new TurnCommand("left"), new MoveCommand(3), new TurnCommand("left"), new MoveCommand(2),
									   new MoveCommand(3), new TurnCommand("right"), new MoveCommand(1), new TurnCommand("left")];

			int commandNumber = Metric.CalculateNumberOfCommands(commands);

			Assert.Equal(8, commandNumber);
		}

		[Fact]
		public void TestNumberCommands_SingleNested()
		{
			List<ICommand> commands = [new RepeatCommand([new MoveCommand(2), new TurnCommand("left")], 3), new MoveCommand(4)];

			int commandNumber = Metric.CalculateNumberOfCommands(commands);

			Assert.Equal(4, commandNumber);
		}

		[Fact]
		public void TestNumberCommands_DoubleNested()
		{
			List<ICommand> commands = [new MoveCommand(2), new RepeatCommand([new RepeatCommand([new TurnCommand("right")], 2), new MoveCommand(2)], 3)];

			int commandNumber = Metric.CalculateNumberOfCommands(commands);

			Assert.Equal(5, commandNumber);
		}
		#endregion

		#region CalculateNumberOfRepeats() Tests
		[Fact]
		public void TestNumberRepeats_Empty()
		{
			int repeatNumber = Metric.CalculateNumberOfRepeats([]);

			Assert.Equal(0, repeatNumber);
		}

		[Fact]
		public void TestNumberRepeats_NoRepeat()
		{
			List<ICommand> commands = [new MoveCommand(2), new TurnCommand("right"), new TurnCommand("right"), new MoveCommand(5)];

			int repeatNumber = Metric.CalculateNumberOfRepeats(commands);

			Assert.Equal(0, repeatNumber);
		}

		[Fact]
		public void TestNumberRepeats_SingleRepeat()
		{
			List<ICommand> commands = [new MoveCommand(2), new RepeatCommand([new TurnCommand("right")], 2), new MoveCommand(5)];

			int repeatNumber = Metric.CalculateNumberOfRepeats(commands);

			Assert.Equal(1, repeatNumber);
		}

		[Fact]
		public void TestNumberRepeats_MultipleRepeat()
		{
			List<ICommand> commands = [new RepeatCommand([new MoveCommand(2), new TurnCommand("left")], 3),
									   new RepeatCommand([new TurnCommand("right"), new MoveCommand(2)], 3)];

			int repeatNumber = Metric.CalculateNumberOfRepeats(commands);

			Assert.Equal(2, repeatNumber);
		}

		[Fact]
		public void TestNumberRepeats_NestedRepeat()
		{
			List<ICommand> commands = [new RepeatCommand([new RepeatCommand([new MoveCommand(2), new RepeatCommand([new TurnCommand("left")], 2)], 3),
									   new MoveCommand(2)], 4)];

			int repeatNumber = Metric.CalculateNumberOfRepeats(commands);

			Assert.Equal(3, repeatNumber);
		}
		#endregion

		#region CalculateNestingLevel() Tests
		[Fact]
		public void TestNestingLevel_LevelOf0()
		{
			List<ICommand> commands = [new TurnCommand("right"), new MoveCommand(2), new TurnCommand("left")];

			int nestingLevel = Metric.CalculateNestingLevel(commands);

			Assert.Equal(0, nestingLevel);
		}

		[Fact]
		public void TestNestingLevel_LevelOf1()
		{
			List<ICommand> commands = [new RepeatCommand([new TurnCommand("right"), new MoveCommand(2), new TurnCommand("left")], 2)];

			int nestingLevel = Metric.CalculateNestingLevel(commands);

			Assert.Equal(1, nestingLevel);
		}

		[Fact]
		public void TestNestingLevel_LevelOf2()
		{
			List<ICommand> commands = [new RepeatCommand([new TurnCommand("right"),
									   new RepeatCommand([new MoveCommand(2), new TurnCommand("left")], 3)], 2)];

			int nestingLevel = Metric.CalculateNestingLevel(commands);

			Assert.Equal(2, nestingLevel);
		}

		[Fact]
		public void TestNestingLevel_SameLevel1Repeats()
		{
			List<ICommand> commands = [new RepeatCommand([new TurnCommand("right"), new MoveCommand(2)], 2),
									   new RepeatCommand([new TurnCommand("left")], 3)];

			int nestingLevel = Metric.CalculateNestingLevel(commands);

			Assert.Equal(1, nestingLevel);
		}

		[Fact]
		public void TestNestingLevel_SameLevel2Repeats()
		{
			List<ICommand> commands = [new RepeatCommand([new RepeatCommand([new TurnCommand("right")], 1)], 2),
									   new RepeatCommand([new RepeatCommand([new TurnCommand("left")], 1)], 3)];

			int nestingLevel = Metric.CalculateNestingLevel(commands);

			Assert.Equal(2, nestingLevel);
		}
		#endregion
	}
}
