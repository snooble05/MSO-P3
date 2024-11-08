using MSO_P3;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_MSO_P3
{
	public class UnitTestCommand
	{
		#region TurnCommand Tests
		[Fact]
		public void TestTurnCommand()
		{
			Character p1 = new Character(new Point(0, 0), Direction.ViewDir.North);
			TurnCommand test = new TurnCommand("right");

			test.Execute(p1);

			Assert.Equal(Direction.ViewDir.East, p1.direction);
		}

		[Fact]
		public void TestTurnCommand_CyclicEnum_NorthToWest()
		{
			Character p1 = new Character(new Point(0, 0), Direction.ViewDir.North);
			TurnCommand test = new TurnCommand("left");

			test.Execute(p1);

			Assert.Equal(Direction.ViewDir.West, p1.direction);
		}

		[Fact]
		public void TestTurnCommand_CyclicEnum_WestToNorth()
		{
			Character p1 = new Character(new Point(0, 0), Direction.ViewDir.West);
			TurnCommand test = new TurnCommand("right");

			test.Execute(p1);

			Assert.Equal(Direction.ViewDir.North, p1.direction);
		}
		#endregion

		#region MoveCommand Tests
		[Fact]
		public void TestMoveCommand_Upwards()
		{
			Character p1 = new Character(new Point(0, 0), Direction.ViewDir.North);
			MoveCommand move = new MoveCommand(10);

			move.Execute(p1);

			Assert.Equal(-10, p1.position.Y);

		}

		[Fact]
		public void TestMoveCommand_Downwards()
		{
			Character p1 = new Character(new Point(0, 0), Direction.ViewDir.South);
			MoveCommand move = new MoveCommand(10);

			move.Execute(p1);

			Assert.Equal(10, p1.position.Y);

		}

		[Fact]
		public void TestMoveCommand_Right()
		{
			Character p1 = new Character(new Point(0, 0), Direction.ViewDir.East);
			MoveCommand move = new MoveCommand(10);

			move.Execute(p1);

			Assert.Equal(10, p1.position.X);

		}

		[Fact]
		public void TestMoveCommand_Left()
		{
			Character p1 = new Character(new Point(0, 0), Direction.ViewDir.West);
			MoveCommand move = new MoveCommand(10);

			move.Execute(p1);

			Assert.Equal(-10, p1.position.X);
		}
		#endregion

		#region RepeatCommand Tests
		[Fact]
		public void TestRepeatCommand_SimpleTurn()
		{
			Character p1 = new Character(new Point(0, 0), Direction.ViewDir.North);
			List<ICommand> commands = [new MoveCommand(10), new TurnCommand("left")];

			RepeatCommand repeat = new RepeatCommand(commands, 2);
			repeat.Execute(p1);

			Assert.Equal(new Point(-10, -10), p1.position);

		}

		[Fact]
		public void TestRepeatCommand_NestedRepeat()
		{
			Character p1 = new Character(new Point(0, 0), Direction.ViewDir.North);
			List<ICommand> commands = [new MoveCommand(10), new TurnCommand("left"), new RepeatCommand([new MoveCommand(2)], 3)];

			RepeatCommand repeat = new RepeatCommand(commands, 2);
			repeat.Execute(p1);

			Assert.Equal(new Point(-16, -4), p1.position);
			Assert.Equal(Direction.ViewDir.South, p1.direction);
		}
		#endregion

		#region Condition Tests
		[Fact]
		public void WallAhead_PlayerInTopLeftCornerNorth()
		{
			Character c = new Character(new Point(0, 0), Direction.ViewDir.North);
			Grid g = new Grid(c, 4, new List<Point>(), null);

			bool wallInFront = Conditions.wallAhead(c, g);

			Assert.True(wallInFront);
		}

		[Fact]
		public void WallAhead_PlayerInTopLeftCornerEast()
		{
			Character c = new Character(new Point(0, 0), Direction.ViewDir.East);
			Grid g = new Grid(c, 4, new List<Point>(), null);

			bool wallInFront = Conditions.wallAhead(c, g);

			Assert.False(wallInFront);
		}

		[Fact]
		public void WallAhead_PlayerInFrontOfBlockedCell()
		{
			Character c = new Character(new Point(1, 1), Direction.ViewDir.East);
			List<Point> blockedCells = new List<Point>() { new Point(2, 1) };
			Grid g = new Grid(c, 4, blockedCells, null);

			bool wallInFront = Conditions.wallAhead(c, g);

			Assert.True(wallInFront);
		}

		[Fact]
		public void WallAhead_PlayerInBottomRightCornerSouth()
		{
			Character c = new Character(new Point(3, 3), Direction.ViewDir.South);
			Grid g = new Grid(c, 4, new List<Point>(), null);

			bool wallInFront = Conditions.wallAhead(c, g);

			Assert.True(wallInFront);
		}

		[Fact]
		public void WallAhead_PlayerInBottomRightCornerWest()
		{
			Character c = new Character(new Point(3, 3), Direction.ViewDir.West);
			Grid g = new Grid(c, 4, new List<Point>(), null);

			bool wallInFront = Conditions.wallAhead(c, g);

			Assert.False(wallInFront);
		}
		#endregion
	}
}
