using MSO_P3;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_MSO_P3
{
	public class UnitTestGrid
	{
		[Fact]
		public void Constructor_InvalidCharacter()
		{
			Character c = new Character(new Point(2, 8), Direction.ViewDir.East);
			int size = 4;
			List<Point> blockedCells = new List<Point>();
			Point? endPoint = null;

			Action a = () => new Grid(c, size, blockedCells, endPoint);

			Assert.Throws<ArgumentOutOfRangeException>(a);
		}
	}
}
