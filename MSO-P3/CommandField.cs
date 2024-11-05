using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace MSO_P3
{
	public class CommandField : Panel
	{
		private List<ICommand> _commands;
		private RichTextBox _commandInput;
		private Button _runButton;
		public List<ICommand> Commands
		{
			get { return _commands; }
		}

		public CommandField()
		{
			_commands = new List<ICommand>();

			_commandInput = new RichTextBox();
			_commandInput.Text = "";
			_commandInput.Location = new Point(this.Location.X, this.Location.Y);
			_commandInput.AcceptsTab = true;
			_commandInput.Multiline = true;

			this.Controls.Add(_commandInput);

			_runButton = new Button();
			_runButton.Text = "Run";
			_runButton.Size = new Size(100, 60);
			_runButton.Click += runField;
			this.Controls.Add(_runButton);

			this.Resize += resize;
			this.resize(null!, null!);
		}

		private void runField(object o, EventArgs ea)
		{
			//TBA
		}

		private void resize(object o, EventArgs ea)
		{
			_commandInput.Size = new Size(this.Size.Width - 10, this.Size.Height - 80);
			_runButton.Location = new Point(30, this.Size.Height - 70);
		}
	}
}
