
using System;

namespace SimpleSpreadsheet
{
	/// <summary>
	/// There are 2 versions for the design of the project
	/// Version 1:Simple factory 
	/// Version 2:Simple factory + Interpretor
	/// </summary>
	class Program
	{
		public static void Main(string[] args)
		{
			while (true)
			{
				Console.Write("enter command:");
				try
				{
					//[version1] factory
					 var result = Translator.GetResult(Console.ReadLine());
					//[version2] factory + interpreter 
					//var  result = Translator.GetResultByInterpretor(Console.ReadLine());
					Console.WriteLine(result);
				 
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
				System.Threading.Thread.Sleep(100);
			}
		}
	}

	
}