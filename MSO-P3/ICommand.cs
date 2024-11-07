using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel.Design;

namespace MSO_P3;

public interface ICommand
{
	public void Execute(Character c);
}

public class TurnCommand : ICommand
{
	private string _turningDirection { get; set; }

	public string TurningDirection
	{
		get { return _turningDirection; }
	}

	public TurnCommand(string turningDirection)
	{
		this._turningDirection = turningDirection;
	}

	public void Execute(Character c)
	{
		switch (_turningDirection)
		{
			case "right":
				c.direction = (Direction.ViewDir)((int)(c.direction + 1) % 4);
				break;
			case "left":
				c.direction = (Direction.ViewDir)((int)(c.direction + 3) % 4);
				break;
			default:
				throw new ArgumentException("Invalid turning direction");
		}
	}
}

public class MoveCommand : ICommand
{
	private int _steps { get; set; }

	public int Steps
	{
		get { return _steps; }
	}

	public MoveCommand(int steps)
	{
		_steps = steps;
	}

	public void Execute(Character c)
	{
		switch (c.direction)
		{
			case Direction.ViewDir.North:
				c.position = new Point(c.position.X, c.position.Y - _steps);
				break;
			case Direction.ViewDir.East:
				c.position = new Point(c.position.X + _steps, c.position.Y);
				break;
			case Direction.ViewDir.South:
				c.position = new Point(c.position.X, c.position.Y + _steps);
				break;
			case Direction.ViewDir.West:
				c.position = new Point(c.position.X - _steps, c.position.Y);
				break;
		}
	}
}

public class RepeatCommand : ICommand
{
	private List<ICommand> _commands { get; set; }
	private int _repeatAmount { get; set; }

	public List<ICommand> Commands
	{
		get { return _commands; }
	}
	public int RepeatAmount
	{
		get { return _repeatAmount; }
	}

	public RepeatCommand(List<ICommand> commands, int repeatAmount)
	{
		_commands = commands;
		_repeatAmount = repeatAmount;
	}

	public void Execute(Character c)
	{
		for (int i = 0; i < _repeatAmount; i++)
		{
			foreach (ICommand command in _commands)
			{
				command.Execute(c);
			}
		}
	}
}

public class RepeatUntilCommand : ICommand
{
	private List<ICommand> _commands;
	private Func<Character, Grid, bool> _condition;
	private Grid _grid;
	public List<ICommand> Commands
	{
		get { return _commands; }
	}
	public Func<Character, Grid, bool> Condition
	{
		get { return _condition; }
	}
		
	public Grid Grid
	{
		get { return _grid; }
	}

	public RepeatUntilCommand(List<ICommand> commands, Func<Character, Grid, bool> condition, Grid g)
	{
		this._commands = commands;
		this._condition = condition;
		this._grid = g;
	}
	public void Execute(Character c)
	{
		bool condition = Condition(c, Grid);

        while (!condition)
        {
            foreach (ICommand command in _commands)
            {
                command.Execute(c);
				condition = Condition(c, Grid);

				//if (condition)
				//{
				//	throw new Exception("You have hit a " + _conditionString);
				//}
            }
        }
    }
}

public static class Condition
{
    public static Func<Character, Grid, bool> GetCondition(string condition)
    {
		switch (condition)
		{
			case "WallAhead":
				return wallAhead;
			case "GridEdge":
				return gridEdge;
			default:
				throw new ArgumentException("Unkown condition given");
		}
    }


    public static bool wallAhead(Character c, Grid g)
	{
		switch (c.direction)
		{
			case Direction.ViewDir.North:
				return c.position.Y == 0 || g.BlockedCells.Contains(new Point(c.position.X, c.position.Y - 1));
			case Direction.ViewDir.East:
				return c.position.X == (g.GridSize - 1) || g.BlockedCells.Contains(new Point(c.position.X + 1, c.position.Y));
			case Direction.ViewDir.South:
				return c.position.Y == (g.GridSize - 1) || g.BlockedCells.Contains(new Point(c.position.X, c.position.Y + 1));
			case Direction.ViewDir.West:
				return c.position.X == 0 || g.BlockedCells.Contains(new Point(c.position.X - 1, c.position.Y));
			default:
				throw new ArgumentException("Character has an invalid direction");
		}
	}
	public static bool gridEdge(Character c, Grid g)
	{
		switch (c.direction)
		{
			case Direction.ViewDir.North:
				return c.position.Y == 0;
			case Direction.ViewDir.East:
				return c.position.X == (g.GridSize - 1);
			case Direction.ViewDir.South:
				return c.position.Y == (g.GridSize - 1);
			case Direction.ViewDir.West:
				return c.position.X == 0;
			default:
				throw new ArgumentException("Character has an invalid direction");
		}
	}

	public static bool IsCleared(Character c, Grid g)
	{
		if (c.position == g.EndPoint)
		{
			//add clear logic: display message of cleared or not
			return true;
		} else
		{
			return false;
		}
	}
}

public struct Preset
{
	private List<ICommand> _commands;

	public List<ICommand> Commands
	{
		get { return _commands; }
	}

	public Preset(List<ICommand> commands)
	{
		_commands = commands;
	}
}

