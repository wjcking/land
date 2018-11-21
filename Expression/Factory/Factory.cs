using System;
using System.Text;

namespace SimpleSpreadsheet
{
	/// <summary>
	/// A factory that we can put the common things here
	/// </summary>
	public abstract class Factory
	{
		protected const char DELIMITER_CONNECT = '-';
		protected const char DELIMITER_SLICE = '|';
		//the dimension of vectors
		protected static int width = 20;
		protected static int height = 4;
		/// <summary>
		///A 2d int array for storing data, you can change it into float.
		/// Attention: height(y) is first parameter, width(x) is the second one.
		/// </summary>
		protected static int[,] vectors = new int[4, 20];

		protected static StringBuilder tableBuilder = new StringBuilder();

		/// <summary>
		/// has to be a property or function, cause need a refresh after a sheet created
		/// </summary>
		protected static int WidthOffset
		{
			get { return width * 3 + 3; }
		}
		/// <summary>
		/// Used  function pointers for  iteration print or a sum up
		/// </summary>
		/// <param name="each">y,currentValue, isRowEnded</param> 
		/// <param name="startX">default or customize</param>
		/// <param name="startY">default or customize</param>
		/// <param name="endX">default or customize</param>
		/// <param name="endY">default or customize</param>
		protected void ProcessVector(Action<int, int, bool> each = null,
									 int startX = 0, int startY = 0, int endX = -1, int endY = -1)
		{

			//traverse all of vectors
			endX = endX < 0 ? width - 1 : endX;
			endY = endY < 0 ? height - 1 : endY;
			// for sum up
			// if end subscripts are 0 then set to 1 in order to proceed iteration
			// otherwise
			endX = endX == 0 ? ++endX : endX;
			endY = endY == 0 ? ++endY : endY;

			//attetion y first , x second, if sum up ,must include start and end element 
			//that is why <= used. not <
			for (int y = startY; y <= endY; y++)
			{
				for (int x = startX; x <= endX; x++)
					each(y, vectors[y, x], x == endX);
			}
		}
		/// <summary>
		/// Interpret and puting values into vector array.
		/// </summary>
		/// <param name="splited">without command prefix like Q N S C</param>
		public abstract void Interpret(string[] splited);

		/// <summary>
		/// Print the values and calculate the table's width and height
		/// </summary>
		protected void RenderValues()
		{
			int currentRow = -1;
			string output = string.Empty;
			ProcessVector(
				(row, value, isRowEnd) =>
				{
					if (currentRow != row)
					{
						output = DELIMITER_SLICE.ToString().PadRight(3);
						tableBuilder.Append(output);
						currentRow = row;
					}
					//organize the values for 
					//tableBuilder.Append(value == 0 ? string.Empty.PadRight(3): value.ToString().PadRight(3- value.ToString().Length));
					//or this , just put each value into each sheet
					tableBuilder.Append(value == 0 ? string.Empty.PadRight(3) : value.ToString().PadRight(3));

					if (isRowEnd)
						tableBuilder.AppendLine(DELIMITER_SLICE.ToString());

				}
			);
		}
		/// <summary>
		/// Print the header or footer of the sheets
		/// </summary>
		protected void RenderLine()
		{
			tableBuilder.AppendLine(string.Empty.PadRight(WidthOffset, DELIMITER_CONNECT));
		}

		/// <summary>
		/// print the footer of the table
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			//footer
			RenderLine();
			return tableBuilder.ToString();
		}
		/// <summary>
		/// clear the content of table builder so that it could be displayed nicely
		/// </summary>
		public void Clear()
		{
			tableBuilder.Clear();
		}
	}
}
