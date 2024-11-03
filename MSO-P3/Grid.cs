using System;
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
			string[] gridString = input.Split("\n");
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
		}

		public void loadGrid(string filePath)
		{
			StreamReader reader = new StreamReader(filePath);
			string gridString = reader.ReadToEnd();
			setGrid(gridString);
		}

		private void draw(object o, PaintEventArgs pea)
		{
			this.gridUI.draw(pea.Graphics);
		}
	}

	public class GridUI
	{
		public Bitmap Bitmap;
		private int _gridSize;
		private Character _character;
		private List<Point> _blockedCells;
		private Point? _endPoint;

		public Graphics BitmapGraphics
		{
			get { return Graphics.FromImage(Bitmap); }
		}

		public GridUI(Character character, int gridSize, List<Point> blockedCells, Point? endPoint)
		{
			this.Bitmap = new Bitmap(1, 1);
			_character = character;
			_blockedCells = blockedCells;
			_endPoint = endPoint;
			_gridSize = gridSize;
		}

		public void draw(Graphics g)
		{
			int cellSize = this.Bitmap.Width / this._gridSize;
			Graphics gr = this.BitmapGraphics;
			gr.FillRectangle(Brushes.White, 0, 0, this.Bitmap.Width, this.Bitmap.Height);
			for(int i = 0; i < this._gridSize; i++)
			{
				for (int j = 0; j < this._gridSize; j++)
				{
					gr.DrawRectangle(Pens.Black, cellSize * i, cellSize * j, this.Bitmap.Width, this.Bitmap.Height);
				}
			}
			gr.FillEllipse(Brushes.Blue, (_character.position.X * cellSize) + 1, (_character.position.Y) + 1 * cellSize, cellSize - 2, cellSize - 2);
			foreach (Point cell in _blockedCells)
			{
				gr.FillRectangle(Brushes.Orange, (cell.X * cellSize) + 1, (cell.Y * cellSize) + 1, cellSize - 2, cellSize - 2);
			}
			g.DrawImage(this.Bitmap, 0, 0);
		}
	}
}
