﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSO_P3
{
	public class Character
	{
		public Point position { get; set; }
		public Direction.ViewDir direction { get; set; }
		public List<(Point, Point)> path { get; set; }

		public Character(Point position, Direction.ViewDir direction)
		{
			this.position = position;
			this.direction = direction;
			this.path = new List<(Point, Point)> ();
		}

		public Character(Character c)
		{
			this.position = c.position;
			this.direction = c.direction;
			this.path = c.path;
		}

		public void ExecuteCommands(List<ICommand> commands)
		{
			foreach (ICommand command in commands)
			{
				command.Execute(this);
			}
		}
	}

	public struct Direction
	{
		public enum ViewDir
		{
			North = 0, East = 1, South = 2, West = 3
		}
	}
}
