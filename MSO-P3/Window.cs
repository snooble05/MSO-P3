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
		private CommandField _commandField;
		private Label _output;

		public Window()
		{
			this.Size = new Size(1600, 860);
			this.BackColor = Color.Azure;

			_menu = new MenuStrip();
			_menu.BackColor = Color.LightBlue;
			makeFileMenu();
			this.Controls.Add( _menu );

			_grid = new Grid(new Character(new Point(0, 0), Direction.ViewDir.East), 8, new List<Point>() { new Point (2, 2)}, new Point(3, 3));
			_grid.MaximumSize = new Size(770, 770);
			this.Controls.Add(_grid);

			_commandField = new CommandField();
			_commandField.Location = new Point(10, 30);
			this.Controls.Add(_commandField);

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

		private void resize(object o, EventArgs ea)
		{
			_grid.Location = new Point(this.ClientSize.Width / 2, 30);
			_grid.Size = new Size((this.ClientSize.Width / 2) - 10, this.ClientSize.Height - 30);
			_grid.Invalidate();
			_commandField.Size = new Size((this.ClientSize.Width / 2) - 10, (this.ClientSize.Height / 2) + 20);
			_output.Location = new Point(10, (this.ClientSize.Height / 2) + 50);
			_output.Size = new Size((this.ClientSize.Width / 2) - 20, (this.ClientSize.Height / 2) - 63);
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
