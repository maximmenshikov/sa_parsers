/// <summary>
/// PVS-Studio analyzer output parser in respect to 5W+1H system.
/// Please note:
/// 1) As it is seen that not all data is provided, we don't even try to fill them.
/// 2) It is a 5-minute project. Errors are not checked in principle.
/// 3) Input is limited to what we expect. The code is not generic.
/// </summary>
using System;
using System.Linq;
using System.IO;

namespace pvs_parser
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args.Count () < 1)
			{
                Console.WriteLine("Invalid number of arguments");
                return;
			}

            var sr = File.OpenText(args[0]);
            var line = sr.ReadLine();

            while (line != null)
            {
                if (line.Contains("www.viva64.com/en/w"))
                {
                    line = sr.ReadLine();
                    continue;
                }

                var data = line.Split('\t');
                if (data.Length < 4)
                {
                    line = sr.ReadLine();
                    continue;
                }

                var fileName = data[0];
                var filePath = "";
                if (fileName.Contains("01.w_Defects"))
                {
                    filePath = Path.Combine("01.w_Defects", Path.GetFileName(fileName));
                }
                else if (fileName.Contains("02.wo_Defects"))
                {
                    filePath = Path.Combine("02.wo_Defects", Path.GetFileName(fileName));
                }
                else
                {
                    /* Only folders from above are interesting. */
                    line = sr.ReadLine();
                    continue;
                }

                Console.WriteLine(filePath + ":" + (int.Parse(data[1]) - 2).ToString());
                Console.WriteLine("pvs:");
                Console.WriteLine("What? " + data[3]);
                Console.WriteLine("When? -");
                Console.WriteLine("Where? File, line.");
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
