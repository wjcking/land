/*
 * Created by Jacky.
 */
using System;

namespace SimpleSpreadsheet
{
	/// <summary>
	/// Sum up formula
	/// </summary>
	public class SumExpression : Factory, IExpression
	{
		public SumExpression()
		{

		}

		/// <summary>
		/// [sum up] e.g.S 1 2 1 3 1 4
		/// Put values into the vector array.
		/// </summary>
		/// <param name="splited">without command prefix like Q N S C</param>
		public override void Interpret(string[] splited)
		{
			if (splited.Length < 6)
				throw new Exception("[Sum]should be 6 params like: S 1 2 1 3 1 4 ");
			var coordinates = Convert(splited);
			/*
			 * should use Assert,
			 **/
			if (coordinates[0] == coordinates[2] && coordinates[1] == coordinates[3])
				return;
			
			if (coordinates[0] > coordinates[2]  )
				throw new Exception("start X should be less than end X");
			if (coordinates[1] > coordinates[3]  )
				throw new Exception("start Y should be less than end Y");
			int sum = 0;
			ProcessVector((row, value, isRowEnd) =>
			{
				sum += value;
			}, coordinates[0], coordinates[1], coordinates[2], coordinates[3]);

			//attention y first, x second
			vectors[coordinates[5], coordinates[4]] = sum;
		}

		/// <summary>
		/// A tool for convering string[] to int[] with well-calucated subscripts
		/// </summary>
		/// <param name="splited">command parameters</param>
		/// <returns></returns>
		private int[] Convert(string[] splited)
		{
			var coordinates = new int[splited.Length];
			for (int i = 0; i < splited.Length; i++)
			{
				int.TryParse(splited[i], out coordinates[i]);

				if (coordinates[i] == 0)
					throw new Exception("params:" + (i + 1).ToString() + " out of boundary");
				else
					--coordinates[i];
			}

			return coordinates;
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
		public SumExpression(IExpression exp)
		{
			expression = exp;
		}
		public bool Interpret(string context)
		{
			if (!expression.Interpret(context))
				return false;

			Interpret(Translator.Translate(context).Item2);

			return true;
		}
		#endregion
	}
}
