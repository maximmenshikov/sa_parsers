/// <summary>
/// cppcheck analyzer output parser in respect to 5W+1H system.
/// Please note:
/// 1) As it is seen that not all data is provided, we don't even try to fill them.
/// 2) It is a 5-minute project. Errors are not checked in principle.
/// 3) Input is limited to what we expect. The code is not generic.
/// </summary>
using System;
using System.Linq;
using System.IO;

namespace cppcheck_parser
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            if (args.Count() < 1)
            {
                Console.WriteLine("Invalid number of arguments");
                return;
            }

            var fileName = args[0];
            var sr = File.OpenText(fileName);
            var line = sr.ReadLine();
            string path = "";
            while (line != null)
            {
                string filePath = null;
                string errorText = "";
                if (line.Contains("["))
                {
                    filePath = line.Substring(1);
                    filePath = filePath.Substring(0, filePath.IndexOf("]"));
                }
                else if (line.StartsWith("########"))
                {
                    path = line.Substring("######## ".Length);
                    line = sr.ReadLine();
                    continue;
                }
                errorText = line.Substring(line.IndexOf(")") + 1);

                Console.WriteLine(path + "/" + filePath);
                Console.WriteLine("cppcheck:");
                Console.WriteLine("What? " + errorText);
                Console.WriteLine("When? -");
                Console.WriteLine("Where? File, line");
                Console.WriteLine("Who? -");
                Console.WriteLine("Why? -");
                Console.WriteLine("How to fix? -");
                Console.WriteLine();

                line = sr.ReadLine();
            }

            sr.Close();
		}
	}
}
