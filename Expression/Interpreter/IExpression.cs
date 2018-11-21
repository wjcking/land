namespace SimpleSpreadsheet
{
	/// <summary>
	/// The version 2 for Interpreter Pattern 
	/// </summary>
	public interface IExpression
	{
		bool Interpret(string context);
	}
}
