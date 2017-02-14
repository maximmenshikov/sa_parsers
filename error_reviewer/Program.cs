/// <summary>
/// This utility lets you review reports correctness by
/// showing the report + lines of code in question and letting
/// you press y or n. 'y' reports get into the stderr.
/// 
/// Please note:
/// 1) It is a 5-minute project. Errors are not checked in principle.
/// 2) Input is limited to what we expect. The code is not generic.
/// </summary>
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace error_reviewer
{
    class MainClass
    {

        private static List<string> getLinesNearPosition(string path, string fileLine)
        {
            var file = fileLine.Substring(0, fileLine.IndexOf(":"));
            var line = int.Parse(fileLine.Substring(fileLine.IndexOf(":") + 1));
            var lines = File.ReadAllLines(Path.Combine(path, file));

            lines[line - 1] = "!" + lines[line - 1];
            var startIdx = line - 10;
            if (startIdx < 0)
                startIdx = 0;
            var endIdx = line + 10;
            if (endIdx >= lines.Length)
                endIdx = lines.Length - 1;

            return lines.Skip(startIdx).Take(endIdx - startIdx).ToList();
        }

        public static void Main(string[] args)
        {
            const bool framac = false;
            if (args.Length < 2)
            {
                Console.WriteLine("Invalid number of arguments");
                return;
            }

            var fileName = args[0];
            var path = args[1];

            var sr = File.OpenText(fileName);
            var s = sr.ReadLine();
            while (s != null)
            {
                var fileline = s;
                var name = sr.ReadLine();
                var what = sr.ReadLine();
                var when = sr.ReadLine();
                var where = sr.ReadLine();
                var who = sr.ReadLine();
                var why = sr.ReadLine();
                var howtofix = sr.ReadLine();
                sr.ReadLine();

                Console.Clear();
                Console.WriteLine(what);
                Console.WriteLine(when);
                Console.WriteLine(where);
                Console.WriteLine(why);
                Console.WriteLine(howtofix);

                Console.WriteLine("#########################");
                Console.WriteLine();
                var lineWithErrorDefinition = "";
                var lines = getLinesNearPosition(path, fileline);
                foreach (var line in lines)
                {
                    var color = Console.ForegroundColor;
                    var tmp = line;
                    if (tmp.StartsWith("!"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        tmp = tmp.Substring(1);
                        lineWithErrorDefinition = tmp;
                    }
                    Console.WriteLine(tmp);
                    Console.ForegroundColor = color;
                }
                bool answerProvided = false;
                bool print = false;
                if (framac)
                {
                    if ((what.ToLower().Contains("shift") && lineWithErrorDefinition.ToLower().Contains("shift")) ||
                        (what.ToLower().Contains("bounds") && lineWithErrorDefinition.ToLower().Contains("buffer")) ||
                        (what.ToLower().Contains("verflow") && lineWithErrorDefinition.ToLower().Contains("verflow")))
                    {
                        answerProvided = true;
                        print = true;
                    }
                }

                while (!answerProvided)
                {
                    string result = "";
                    result = Console.ReadLine();
                    if (result == "y")
                    {
                        print = true;
                        answerProvided = true;
                    }
                    else if (result == "n")
                    {
                        print = false;
                        answerProvided = true;
                    }
                }
                if (print)
                {
                    Console.Error.WriteLine(fileline);
                    Console.Error.WriteLine(name);
                    Console.Error.WriteLine(what);
                    Console.Error.WriteLine(when);
                    Console.Error.WriteLine(where);
                    Console.Error.WriteLine(who);
                    Console.Error.WriteLine(why);
                    Console.Error.WriteLine(howtofix);
                    Console.Error.WriteLine();
                }
                s = sr.ReadLine();
            }
            sr.Close();
        }
    }
}
