/// <summary>
/// This utility processes the output from parsers, excludes skipped files,
/// and marks reports as correct if they correspond to error/non-error lines from
/// benchmark.
/// 
/// Please note:
/// 1) It is a 5-minute project. Errors are not checked in principle.
/// 2) Input is limited to what we expect. The code is not generic.
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;

namespace success_rate
{
	class MainClass
	{
        /// <summary>
        /// Get the skipped files from skipped.txt.
        /// </summary>
        /// <returns>The skipped files.</returns>
        /// <param name="fileName">File name.</param>
        private static List<string> getSkippedFiles(string fileName)
        {
            var sr = System.IO.File.OpenText(fileName);
            var s = sr.ReadToEnd();
            sr.Close();
            return s.Split(new char[]{ '\n', '\r' }).Select((a) => a.Trim()).Distinct().Where((a) => !String.IsNullOrEmpty(a)).ToList();
        }

		public static void Main (string[] args)
		{
            if (args.Count() < 3)
            {
                Console.WriteLine("Invalid number of arguments");
                return;
            }

            string fileName = args[0];
            string path = args[1];
            string skippedFile = args[2];

            var skippedFiles = getSkippedFiles(skippedFile);
			var knownDefects = new List<string> ();
            var falseDefects = new List<string> ();
            var files = System.IO.Directory.GetFiles (Path.Combine(path, "01.w_Defects"));
			foreach (var file in files)
            {
                var sr = System.IO.File.OpenText(file);
                int i = 1;
                var s = sr.ReadLine();
                while (s != null)
                {
                    if (s.ToUpper().Contains("*ERROR:") ||
                        s.ToUpper().Contains("* ERROR:"))
                    {
                        string target = "01.w_Defects/" + Path.GetFileName(file) + ":" + i;
                        knownDefects.Add(target);
                    }
                    s = sr.ReadLine();
                    i++;
                }
                sr.Close();
			}

            files = System.IO.Directory.GetFiles (Path.Combine(path, "02.wo_Defects"));
            foreach (var file in files)
            {
                var sr = System.IO.File.OpenText(file);
                int i = 1;
                var s = sr.ReadLine();
                while (s != null)
                {
                    if (s.ToUpper().Contains("*NO ERROR:") ||
                        s.ToUpper().Contains("* NO ERROR:"))
                    {
                        string target = "02.wo_Defects/" + Path.GetFileName(file) + ":" + i;
                        falseDefects.Add(target);
                    }
                    s = sr.ReadLine();
                    i++;
                }
                sr.Close();
            }

            var res_sr = System.IO.File.OpenText(fileName);
            var line = res_sr.ReadLine();
            bool skipOne = false;
            while (line != null)
            {
                /* File+line */
                string fileline = null;
                if (line.Contains("01.w_Defects") ||
                    line.Contains("02.wo_Defects"))
                {
                    fileline = line;

                    if (skippedFiles.Find((a) => line.Contains(a)) != null)
                    {
                        skipOne = true;
                        line = res_sr.ReadLine();
                        continue;
                    }

                    skipOne = false;

                    line = res_sr.ReadLine();

                    /* Analyzer name */
                    string foundName = line.Replace(":", "");
                    if (foundName.Contains("("))
                        foundName = foundName.Substring(0, foundName.IndexOf("(")).Trim();

                    if (falseDefects.Contains(fileline))
                    {
                        Console.WriteLine(fileline);
                        Console.WriteLine(foundName + " (false):");
                    }
                    else if (knownDefects.Contains(fileline))
                    {
                        Console.WriteLine(fileline);
                        Console.WriteLine(foundName + " (correct):");
                    }
                    else
                    {
                        /* Skip totally unknown reports */
                        skipOne = true;
                        line = res_sr.ReadLine();
                        continue;
                    }
                }
                else if (!skipOne)
                {
                    Console.WriteLine(line);
                }
                line = res_sr.ReadLine();
            }

            res_sr.Close();

		}
	}
}
