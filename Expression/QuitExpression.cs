namespace SimpleSpreadsheet
{
	public class QuitExpression:IExpression
	{
		#region Interpreter Pattern Version 2

		private IExpression expression = null;
		public QuitExpression(IExpression exp)
		{
			expression = exp;
		}
		public bool Interpret(string context)
		{
			if (!expression.Interpret(context))
				return false;
			//just exit the console

			return true;
		}
		#endregion
	}
}
