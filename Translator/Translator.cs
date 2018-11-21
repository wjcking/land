using System;

namespace SimpleSpreadsheet
{
	/// <summary>
	/// A Translator class is used for translating,inputing command lines and displaying sheets.
	/// There are 2 versions for the design of the project
	/// Version 1:Simple factory 
	/// Version 2:Simple factory + Interpretor
	/// </summary>
	public static class Translator
	{
		private static Factory sheetFactory;
		private const char DELIMITER_SPACE = ' ';

		static Translator()
		{
			sheetFactory = new CreateExpression();
		}

		/// <summary>
		/// 【Version 1 】for displaying sheets with simple Factory pattern
		/// </summary>
		/// <param name="command">input a string</param>
		/// <returns></returns>
		public static string GetResult(string command)
		{
			var decoded = Translate(command);
			var result = string.Empty;
			switch (decoded.Item1)
			{
				case Constant.COMMAND_Q:
					Environment.Exit(0);
					return "Exiting";				 
				case Constant.COMMAND_C:
					sheetFactory = new CreateExpression();
					((CreateExpression)sheetFactory).Interpret(decoded.Item2);
					result = sheetFactory.ToString();
					break;
				case Constant.COMMAND_S:
					sheetFactory = new SumExpression();
					((SumExpression)sheetFactory).Interpret(decoded.Item2);
					result = ((SumExpression)sheetFactory).ToString();
					break;
				case Constant.COMMAND_N:
					sheetFactory = new NInsertExpression();
					((NInsertExpression)sheetFactory).Interpret(decoded.Item2);
					result = ((NInsertExpression)sheetFactory).ToString();
					break;			
			}
			//clear all of the content 
			if (null != sheetFactory)
				sheetFactory.Clear();
			return result;
		}

		/// <summary>
		/// 【Version 2】 Utilized Interpreter and Factory patterns,more extentable but might be more complicated
		/// what if more formulas like A=B+C or C=Avg(A) this pattern would be useful.
		/// </summary>
		/// <param name="command">input a string</param>
		/// <returns></returns>
		public static string GetResultByInterpreter(string command)
		{
			var terminalQ = new TerminalExpression(Constant.COMMAND_Q);
			var terminalC = new TerminalExpression(Constant.COMMAND_C);
			var terminalS = new TerminalExpression(Constant.COMMAND_S);
			var terminalN = new TerminalExpression(Constant.COMMAND_N);

			IExpression spreadSheet = new CreateExpression(terminalC);
			IExpression sum = new SumExpression(terminalS);
			IExpression insert = new NInsertExpression(terminalN);
			IExpression quit = new QuitExpression(terminalQ);

			var result = string.Empty;

			if (spreadSheet.Interpret(command))
				result = spreadSheet.ToString();
			else if (sum.Interpret(command))
				result = sum.ToString();
			else if (insert.Interpret(command))
				result = insert.ToString();
			else if (quit.Interpret(command))
				Environment.Exit(0);
			
			((Factory)spreadSheet).Clear();
			((Factory)sum).Clear();
			((Factory)insert).Clear();

			return result;
		}


		/// <summary>
		///  To translate the command string, used tuple returning 2 parameters
		///  or we can wrap them up, put it into a model class
		/// </summary>
		/// <param name="command"></param>
		/// <returns>Item1=Q C N S, Item2=string[] params</returns>
		public static Tuple<string, string[]> Translate(string command)
		{
			if (command.ToUpper() == Constant.COMMAND_Q)
				return new Tuple<string, string[]>(Constant.COMMAND_Q, null);

			if (command.IndexOf(DELIMITER_SPACE) < 0 || command.Length < 3 || !Char.IsLetter(command[0]))
				throw new Exception("Bad command or parameters");

			var commandPrefix = command.Substring(0, 1).ToUpper();
			var splitedArray = command.Substring(2).Split(DELIMITER_SPACE.ToString().ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

			return new Tuple<string, string[]>(commandPrefix, splitedArray);
		}
	}
}
