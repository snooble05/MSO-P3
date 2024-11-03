using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing;

namespace MSO_P3
{
	public class Window : Form
	{
		private Grid _grid;
		private MenuStrip _menu;

		public Window()
		{
			_menu = new MenuStrip();
			_menu.Visible = false;
			this.Controls.Add( _menu );
			_grid = new Grid(new Character(new Point(0, 0), Direction.ViewDir.East), 5, new List<Point>());
			this._grid.Size = new Size(this.ClientSize.Width / 2, this.ClientSize.Height / 2);
		}

		private void makeFileMenu()
		{
			ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
			ToolStripMenuItem openStrategyMenu = new ToolStripMenuItem("Load file type:");
			openStrategyMenu.DropDownItems.Add(".txt", null, loadTXTFile);
			ToolStripMenuItem exportStrategyMenu = new ToolStripMenuItem("Export to file type:");
			exportStrategyMenu.DropDownItems.Add(".txt", null, exportTXTFile);
			fileMenu.DropDownItems.Add(openStrategyMenu);
			_menu.Items.Add(fileMenu);
		}

		private void loadTXTFile(object o, EventArgs ea)
		{
			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Filter = "Textfile (*.txt)|*.txt";
			fileDialog.Title = "Choose a file...";
			if(fileDialog.ShowDialog() == DialogResult.OK)
			{
				FileParser parser = new FileParser("txt", fileDialog.FileName);
				parser.ReadFile();
			}
		}

		private void exportTXTFile(object o, EventArgs ea)
		{
			//TBA
		}
	}
}
