namespace SimpleSpreadsheet
{
	/// <summary>
	/// Once read the terminal string, check whether you can interpret or not
	/// what if more formulas like A=B+C or C=Avg(A) this pattern would be useful.
	/// </summary>
	public class TerminalExpression : IExpression
	{
		/// <summary>
		/// N C S
		/// </summary>
		private string commandPrefix;

		public TerminalExpression(string commandPrefix)
		{
			this.commandPrefix = commandPrefix;
		}

		/// <summary>
		/// if the 1st char is N C S Q,then do next
		/// </summary>  
		public bool Interpret(string context)
		{
			if (Translator.Translate(context).Item1 == commandPrefix)
			{
				return true;
			}
			return false;
		}

	}
}
