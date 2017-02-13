/// <summary>
/// Resharper C++ analyzer output parser in respect to 5W+1H system.
/// Please note:
/// 1) As it is seen that not all data is provided, we don't even try to fill them.
/// 2) It is a 5-minute project. Errors are not checked in principle.
/// 3) Input is limited to what we expect. The code is not generic.
/// </summary>
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace resharper_parser
{
    class MainClass
    {
        /// <summary>
        /// Gets all lines from the file.
        /// </summary>
        /// <returns>The lines.</returns>
        /// <param name="fileName">File name.</param>
        private static List<string> getLines(string fileName)
        {
            var sr = System.IO.File.OpenText(fileName);
            var s = sr.ReadToEnd();
            sr.Close();
            return s.Split(new char[]{ '\n', '\r' }).Select((a) => a.Trim()).Where((a) => !String.IsNullOrEmpty(a)).ToList();
        }

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Invalid number of arguments");
                return;
            }

            var inspectionsWithFixes = getLines(args[1]);
            var sr = File.OpenText(args[0]);
            var line = sr.ReadLine();

            while (line != null)
            {
                if (line.Contains("Solution") ||
                    line.Contains("Project") ||
                    !line.Trim().Contains(" "))
                {
                    line = sr.ReadLine();
                    continue;
                }
                line = line.Trim();
                var error = line.Substring(line.IndexOf(" ") + 1);
                var filePath = line.Substring(0, line.IndexOf(" ")).Replace(@"..\", "").Replace(@"\", "/");

                Console.WriteLine(filePath);
                Console.WriteLine("resharper:");
                Console.WriteLine("What? " + error);
                Console.WriteLine("When? -");
                Console.WriteLine("Where? File, line");
                Console.WriteLine("Who? -");
                Console.WriteLine("Why? -");

                if (inspectionsWithFixes.Where((a) => error.ToLower().Contains(a.ToLower())).Count() > 0)
                {
                    Console.WriteLine("How to fix? Solution provided.");
                }
                else
                {
                    Console.WriteLine("How to fix? -");
                }

                Console.WriteLine();

                line = sr.ReadLine();
            }
            sr.Close();
        }
    }
}
