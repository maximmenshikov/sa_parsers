/// <summary>
/// Frama-C analyzer output parser in respect to 5W+1H system.
/// Please note:
/// 1) As it is seen that not all data is provided, we don't even try to fill them.
/// 2) It is a 5-minute project. Errors are not checked in principle.
/// 3) Input is limited to what we expect. The code is not generic.
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace framac_parser
{
	class MainClass
	{
        private static string Capitalize(string s)
        {
            return s.First().ToString().ToUpper() + String.Join("", s.Skip(1));
        }
 
		public static void Main (string[] args)
		{
            if (args.Length < 2)
            {
                Console.WriteLine("Invalid number of arguments");
                return;
            }

            var fileName = args[0];
            var intermediateFileName = args[1];

            // Remove new lines when they are not really needed.
            var fullText = File.ReadAllText(fileName); 
            fullText = Regex.Replace(fullText, "<- \n                                ", "<- ");
            fullText = Regex.Replace(fullText, "\n        Called from", " Called from");
            fullText = Regex.Replace(fullText, "\n                 ", " ");
            File.WriteAllText(intermediateFileName, fullText);

            // Parsing intermediate file and retriving output.
			var list = new List<string> ();
			var sr = System.IO.File.OpenText (intermediateFileName);
			var s = sr.ReadLine ();
            var folder = "";
			while (s != null)
            {
                if (s.Contains("[value] computing for "))
                {
                    list.Add(Capitalize(s.Substring("[value] computing for ".Length)));
                }
                else if (s.Contains("[value] Computing initial state"))
                {
                    list.Add("main");
                }
                else if (s.Contains("[value] Done for function"))
                {
                    list.RemoveAt(list.Count - 1);
                }
                else if (s.Contains("[value] warning: ") && !s.Contains("got status invalid"))
                {
                    var errString = s.Substring(s.IndexOf("[value] warning: ") + "[value] warning: ".Length);
                    var assertString = "-";
                    if (errString.Contains("assert"))
                    {
                        assertString = errString.Substring(errString.IndexOf("assert "));
                        errString = errString.Substring(0, errString.IndexOf("assert "));
                    }
                    var file = s.Substring(0, s.IndexOf(":[value] warning: "));
                    if (!file.Contains("FRAMAC_SHARE") &&
                        !errString.Contains("Ignoring"))
                    {
                        Console.WriteLine(folder + "/" + file);
                        Console.WriteLine("framac:");
                        errString = Capitalize(errString);
                        Console.WriteLine("What? " + errString);
                        Console.WriteLine("When? " + list[list.Count - 1]);
                        Console.WriteLine("Where? File, line, function");
                        Console.WriteLine("Who? -");
                        Console.WriteLine("Why? " + assertString);
                        Console.WriteLine("How to fix? -");
                        Console.WriteLine();
                    }
                }
                else if (s.StartsWith("######## "))
                {
                    folder = s.Substring("######## ".Length);
                }
				s = sr.ReadLine ();
			}
            sr.Close();
		}
	}
}
