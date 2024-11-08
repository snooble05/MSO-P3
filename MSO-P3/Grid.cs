﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MSO_P3
{
	public class Grid : UserControl
	{
		public GridUI gridUI;
		private Character _character;
		private int _gridSize;
		private List<Point> _blockedCells;
		private Point? _endPoint;

		public Character Character
		{
			get { return _character; }
			set
			{
				if (value.position.X >= _gridSize || value.position.Y >= _gridSize)
				{
					throw new ArgumentOutOfRangeException("Character is outside of the grid");
				} else if (_blockedCells.Contains(value.position))
				{
					throw new ArgumentException("Character cannot start on a blocked cell");
				} else if (value == null)
				{
					throw new ArgumentNullException("Character cannot be null");
				}
				_character = value;
			}
		}
		public int GridSize
		{
			get { return _gridSize; }
		}
		public List<Point> BlockedCells
		{
			get { return _blockedCells; }
		}
		public Point? EndPoint
		{
			get { return _endPoint; }
		}

		public Grid(Character character, int gridSize, List<Point> blockedCells, Point? endPoint = null)
		{
			this._gridSize = gridSize;
			this._blockedCells = blockedCells;
			Character = character;
			this._endPoint = endPoint;
			this.gridUI = new GridUI(this._character, this._gridSize, this._blockedCells, this._endPoint);

			this.Paint += this.draw;
		}

		public void setGrid(string input)
		{
			string[] gridString = input.Split("\r\n");
			_gridSize = gridString.Length;
			for (int i = 0; i < _gridSize; i++)
			{
				for (int j = 0; j < _gridSize; j++)
				{
					switch (gridString[i][j])
					{
						case '+':
							_blockedCells.Add(new Point(j, i));
							break;
						case 'x':
							_endPoint = new Point(j, i);
							break;
					}
				}
			}
			Character = new Character(new Point(0, 0), Direction.ViewDir.East);
			gridUI = new GridUI(Character, GridSize, BlockedCells, EndPoint);
		}

		private void draw(object o, PaintEventArgs pea)
		{
			this.gridUI.draw(pea);
		}
	}

	public class GridUI
	{
		private int _gridSize;
		private Character _character;
		private List<Point> _blockedCells;
		private Point? _endPoint;

		public GridUI(Character character, int gridSize, List<Point> blockedCells, Point? endPoint)
		{
			_character = character;
			_blockedCells = blockedCells;
			_endPoint = endPoint;
			_gridSize = gridSize;
		}

		public void draw(PaintEventArgs pea)
		{
			Graphics g = pea.Graphics;
			int drawSize = Math.Min(pea.ClipRectangle.Width, pea.ClipRectangle.Height);
			drawSize = (drawSize / _gridSize) * _gridSize;
			int cellSize = drawSize / _gridSize;

			//Drawing the grid
			g.FillRectangle(Brushes.White, 0, 0, drawSize, drawSize);
			for(int i = 0; i < this._gridSize; i++)
			{
				for (int j = 0; j < this._gridSize; j++)
				{
					g.DrawRectangle(Pens.Black, cellSize * i, cellSize * j, cellSize, cellSize);
				}
			}

			foreach (Point cell in _blockedCells)
			{
				g.FillRectangle(Brushes.Orange, (cell.X * cellSize) + 1, (cell.Y * cellSize) + 1, cellSize - 1, cellSize - 1);
			}

			if (_endPoint != null)
			{
				g.FillRectangle(Brushes.SpringGreen, (((Point)_endPoint).X * cellSize) + 1, (((Point)_endPoint).Y * cellSize) + 1, cellSize - 1, cellSize - 1);
			}

			foreach ((Point begin, Point end) in _character.path)
			{
				g.DrawLine(new Pen(Color.Magenta, 4f), (begin.X * cellSize) + (cellSize / 2), (begin.Y * cellSize) + (cellSize / 2) - 2, (end.X * cellSize) + (cellSize / 2), (end.Y * cellSize) + (cellSize / 2) - 2);
			}

			//Drawing the player
			g.FillEllipse(Brushes.Blue, (_character.position.X * cellSize) + 1, (_character.position.Y * cellSize) + 1, cellSize - 2, cellSize - 2);

			//Drawing the direction arrow inside the player
			Point[] trianglePoints = new Point[3];
			switch (_character.direction)
			{
				case Direction.ViewDir.North:
					trianglePoints = [new Point((_character.position.X * cellSize) + (cellSize / 2) - (cellSize / 5), (_character.position.Y * cellSize) + (cellSize / 2)),
									  new Point((_character.position.X * cellSize) + (cellSize / 2) + (cellSize / 5), (_character.position.Y * cellSize) + (cellSize / 2)),
									  new Point((_character.position.X * cellSize) + (cellSize / 2), (_character.position.Y * cellSize) + (cellSize / 2) - (cellSize / 4))];
					break;
				case Direction.ViewDir.East:
					trianglePoints = [new Point((_character.position.X * cellSize) + (cellSize / 2), (_character.position.Y * cellSize) + (cellSize / 2) - (cellSize / 5)),
									  new Point((_character.position.X * cellSize) + (cellSize / 2), (_character.position.Y * cellSize) + (cellSize / 2) + (cellSize / 5)),
									  new Point((_character.position.X * cellSize) + (cellSize / 2) + (cellSize / 4), (_character.position.Y * cellSize) + (cellSize / 2))];
					break;
				case Direction.ViewDir.South:
					trianglePoints = [new Point((_character.position.X * cellSize) + (cellSize / 2) - (cellSize / 5), (_character.position.Y * cellSize) + (cellSize / 2)),
									  new Point((_character.position.X * cellSize) + (cellSize / 2) + (cellSize / 5), (_character.position.Y * cellSize) + (cellSize / 2)),
									  new Point((_character.position.X * cellSize) + (cellSize / 2), (_character.position.Y * cellSize) + (cellSize / 2) + (cellSize / 4))];
					break;
				case Direction.ViewDir.West:
					trianglePoints = [new Point((_character.position.X * cellSize) + (cellSize / 2), (_character.position.Y * cellSize) + (cellSize / 2) - (cellSize / 5)),
									  new Point((_character.position.X * cellSize) + (cellSize / 2), (_character.position.Y * cellSize) + (cellSize / 2) + (cellSize / 5)),
									  new Point((_character.position.X * cellSize) + (cellSize / 2) - (cellSize / 4), (_character.position.Y * cellSize) + (cellSize / 2))];
					break;
			}
			g.DrawPolygon(new Pen(Color.Black, 2f), trianglePoints);
		}
	}
}
