namespace SimpleSpreadsheet
{
	/// <summary>
	/// Basic settings of Spreadsheet.
	/// </summary>
	public class CreateExpression : Factory,IExpression
	{
		public CreateExpression()
		{
			
		}

		/// <summary>
		/// This is version 1 for puting values into vector array and printing.
		/// </summary>
		/// <param name="splited">without command prefix like Q N S C</param>
		public override void Interpret(string[] splited)
		{
			int.TryParse(splited[0], out width);
			int.TryParse(splited[1], out height);
			// assgin a new for Creating
			vectors = new int[height, width];
			//header
			RenderLine();
			// empty values
			for (int i = 0; i < height; i++) 
			{
				tableBuilder.Append(DELIMITER_SLICE.ToString().PadRight(WidthOffset));
				tableBuilder.AppendLine(DELIMITER_SLICE.ToString());
			}
		}

		#region Interpreter Pattern Version 2

		private IExpression expression = null;
		public CreateExpression(IExpression exp)
		{
			expression = exp;
		}
		public   bool Interpret(string context)
		{
			if (!expression.Interpret(context))
				return false;

			Interpret(Translator.Translate(context).Item2);

			return true;
		}
		#endregion
	}
}
