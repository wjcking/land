/*
 * Created by Jacky.wu.
 */
using System;

namespace SimpleSpreadsheet
{
	/// <summary>
	///  Insert a new.
	/// </summary>
	public class NInsertExpression : Factory, IExpression
	{
		public NInsertExpression()
		{

		}
		/// <summary>
		/// put values into vector array.
		/// e.g.N 1 1 1
		/// </summary>
		/// <param name="splited">without command prefix like Q N S C</param>
		public override void Interpret(string[] splited)
		{
			if (splited.Length < 3)
				throw new Exception(" should be 3 params like: N x y v ");

			int.TryParse(splited[0], out int x);
			int.TryParse(splited[1], out int y);

			if (!int.TryParse(splited[2], out int v))
				throw new Exception("v is not a number");
			//the offset of subscript 
			if (x < 0 || x > width)
				throw new Exception("x out of boundary");
			if (y < 0 || y > height)
				throw new Exception("y out of boundary");

			vectors[y - 1, x - 1] = v;
		}
		/// <summary>
		/// print the table with header and and values
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			//header
			RenderLine();
			RenderValues();
			return base.ToString();
		}

		#region Interpreter Pattern Version 2

		private IExpression expression = null;
		public NInsertExpression(IExpression exp)
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
