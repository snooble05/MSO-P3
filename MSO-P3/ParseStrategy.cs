using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MSO_P3
{
	public abstract class ParseStrategy
	{
		public abstract List<ICommand> ReadFile();
	}

	public class TXTStrategy : ParseStrategy
	{
		StreamReader? _reader;
		String? fileName;
		Grid? _grid { get; set; }

	    private StreamReader Reader
		{
			get { return _reader; }
		}
		public TXTStrategy(String filePath)
		{
			try
			{
				_reader = new StreamReader(filePath);
			} catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

		}

		public override List<ICommand> ReadFile()
		{
			List<ICommand> commands = new List<ICommand>();

			if (Reader == null)
			{
				return commands;
			}

			String line = Reader.ReadLine();

			while (line != null)
			{

				String[] stringArray = line.Split(" ");

				String command = stringArray[0];
				String addOn = stringArray[1];

				switch (command)
				{
					case "Move":
						commands.Add(new MoveCommand(Convert.ToInt32(addOn)));
						break;
					case "Turn":
						commands.Add(new TurnCommand(addOn));
						break;
					case "Repeat":
						commands.Add(new RepeatCommand(GetRepeatCommands(Reader), Convert.ToInt32(addOn)));
						break;
                    case "RepeatUntil":

                        commands.Add(new RepeatUntilCommand(GetRepeatCommands(Reader), Conditions.GetCondition(addOn), _grid));
                        break;
                    default:
						break;
				};

				line = Reader.ReadLine();
			}


			return commands;
		}

		public List<ICommand> GetRepeatCommands(StreamReader reader)
		{
			List<ICommand> commands = new List<ICommand>();
			String line = reader.ReadLine();

			Console.WriteLine("line: " + line);

			String[] stringArray = line.Split(" ");

			while (line != null)
			{
				string[] commandString = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
				string command = commandString[0];
				string addOn = commandString[1];

				switch (command)
				{
					case "Move":
						commands.Add(new MoveCommand(Convert.ToInt32(addOn)));
						break;
					case "Turn":
						commands.Add(new TurnCommand(addOn));
						break;
					case "Repeat":
						commands.Add(new RepeatCommand(GetRepeatCommands(reader), Convert.ToInt32(addOn)));
						break;
                    case "RepeatUntil":
                        commands.Add(new RepeatUntilCommand(GetRepeatCommands(reader), Conditions.GetCondition(addOn), _grid));
                        break;
                    default: break;
				};

				long currentReaderPosition = reader.BaseStream.Position;
				if (reader.Peek() == -1)
				{
					break;
				}

				//check of volgende line indentation heeft, zo ja dan break en list returnen
				if (reader.ReadLine().Split(" ")[0] != " ")
				{
					reader.BaseStream.Seek(currentReaderPosition, SeekOrigin.Begin);
					reader.DiscardBufferedData();
					break;
				}
				line = reader.ReadLine();

			}

			return commands;
		}
	}
}
