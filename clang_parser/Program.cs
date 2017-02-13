/// <summary>
/// Clang analyzer output parser in respect to 5W+1H system.
/// Please note:
/// 1) As it is seen that not all data is provided, we don't even try to fill them.
/// 2) It is a 5-minute project. Errors are not checked in principle.
/// 3) Input is limited to what we expect. The code is not generic.
/// </summary>
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace clang_parser
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

        /// <summary>
        /// Get data listed under "<!-- TAG --> in Clang files.
        /// </summary>
        /// <returns>The data under tag.</returns>
        /// <param name="items">Lines from Clang result file</param>
        /// <param name="tag">Tag.</param>
        private static string getDataUnderTag(ref List<string> items, string tag)
        {
            var line = items.Where((a) => a.StartsWith("<!-- " + tag)).FirstOrDefault();
            if (line == null)
                return null;
            line = line.Substring(("<!-- " + tag + " ").Length);
            line = line.Substring(0, line.LastIndexOf(" -->"));
            return line;
        }

		public static void Main (string[] args)
		{
			if (args.Count() < 1)
            {
                Console.WriteLine("Invalid number of arguments");
                return;
			}

			var path = args[0];
            var files = Directory.GetFiles(path, "*.html", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var lines = getLines(file);
                var fileName = getDataUnderTag(ref lines, "BUGFILE");
                if (fileName == null)
                    continue;
                var pathLenStr = getDataUnderTag(ref lines, "BUGPATHLENGTH");
                var pathLen = int.Parse(pathLenStr);
                string shortPath = "";

                if (fileName.Contains("01.w_Defects"))
                {
                    shortPath = "01.w_Defects/" + Path.GetFileName(fileName);
                }
                else if (fileName.Contains("02.wo_Defects"))
                {
                    shortPath = "02.wo_Defects/" + Path.GetFileName(fileName);
                }
                else
                {
                    continue;
                }

                Console.WriteLine(shortPath + ":" + getDataUnderTag(ref lines, "BUGLINE"));
                Console.WriteLine("clang:");
                Console.WriteLine("What? " + getDataUnderTag(ref lines, "BUGTYPE"));
                if (pathLen > 1)
                {
                    Console.WriteLine("When? Path (" + pathLen.ToString() + " calls) provided");
                }
                else
                {
                    Console.WriteLine("When? -");
                }
                Console.WriteLine("Where? File " + getDataUnderTag(ref lines, "FILENAME") + ", line " + getDataUnderTag(ref lines, "BUGLINE"));
                Console.WriteLine("Why? " + getDataUnderTag(ref lines, "BUGDESC"));
                Console.WriteLine("How to fix? -");
                Console.WriteLine();
            }
		}
	}
}
