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

		public Window()
		{
			this.Size = new Size(1600, 860);
			this.BackColor = Color.Azure;

			_menu = new MenuStrip();
			_menu.BackColor = Color.LightBlue;
			makeFileMenu();
			makeExerciseMenu();
			this.Controls.Add( _menu );

			_grid = new Grid(new Character(new Point(0, 0), Direction.ViewDir.East), 8, new List<Point>() { });
			_grid.MaximumSize = new Size(770, 770);
			this.Controls.Add(_grid);

			_commandField = new CommandField(_grid);
			_commandField.Location = new Point(10, 30);
			this.Controls.Add(_commandField);

			this.Resize += resize;
			this.resize(null!, null!);
		}

		private void resize(object o, EventArgs ea)
		{
			_grid.Location = new Point(this.ClientSize.Width / 2, 30);
			_grid.Size = new Size((this.ClientSize.Width / 2) - 10, this.ClientSize.Height - 30);
			_grid.Invalidate();
			_commandField.Size = new Size((this.ClientSize.Width / 2) - 10, this.ClientSize.Height - 30);
		}

		private void makeFileMenu()
		{
			ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
			ToolStripMenuItem openStrategyMenu = new ToolStripMenuItem("Load file type:");
			openStrategyMenu.DropDownItems.Add(".txt", null, loadTXTFile);
			fileMenu.DropDownItems.Add(openStrategyMenu);
			_menu.Items.Add(fileMenu);
		}

		private void makeExerciseMenu()
		{
			ToolStripMenuItem exerciseMenu = new ToolStripMenuItem("Exercise");
			exerciseMenu.DropDownItems.Add("Load exercise", null, loadExercise);
			_menu.Items.Add(exerciseMenu);
		}

		private void loadTXTFile(object o, EventArgs ea)
		{
			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Filter = "Textfile (*.txt)|*.txt";
			fileDialog.Title = "Choose a file...";
			if (fileDialog.ShowDialog() == DialogResult.OK)
			{
				FileParser parser = new FileParser("txt", fileDialog.FileName);
				parser.ReadFile();
				_commandField.setInputText(File.ReadAllText(fileDialog.FileName));
			}
		}

		private void loadExercise(object o, EventArgs ea)
		{
			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Filter = "Textfile (*.txt)|*.txt";
			fileDialog.Title = "Choose an exercise...";
			if (fileDialog.ShowDialog() == DialogResult.OK)
			{
				_grid.setGrid(File.ReadAllText(fileDialog.FileName));
				_grid.Invalidate();
				_commandField.clearField(null, EventArgs.Empty);
			}
		}
	}
}
