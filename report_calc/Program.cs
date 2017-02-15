/// <summary>
/// This utility processes the output from parsers and returns informativeness scores.
/// 
/// Please note:
/// 1) It is a 5-minute project. Errors are not checked in principle.
/// 2) Input is limited to what we expect. The code is not generic.
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace report_calc
{
	class MainClass
	{
        private static void ReadBenchmark(ref Dictionary<string, ErrorDescription> dict, string path)
        {
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
                        dict.Add(target, new ErrorDescription() { True = true,
                            Comment = s.Substring(s.IndexOf("/*") + 2).Replace("*/", "") });
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
                        dict.Add(target, new ErrorDescription() { True = false,
                            Comment = s.Substring(s.IndexOf("/*") + 2).Replace("*/", "") });
                    }
                    s = sr.ReadLine();
                    i++;
                }
                sr.Close();
            }
        }

        private static double getRateForQuestion(ref Dictionary<string, ErrorDescription> errorLines, string question)
        {
            var value = errorLines.SelectMany((a) => a.Value.Reports).SelectMany((b) => b.Answers.Where((c) => c.Name == question)).Sum((d) => d.InformationalValue);
            var count = errorLines.SelectMany((a) => a.Value.Reports).SelectMany((b) => b.Answers.Where((c) => c.Name == question)).Count();
            return value / count;
        }
		public static void Main (string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine ("Invalid number of arguments");
				return;
			}

			if (System.IO.File.Exists (args [0]) == false)
			{
				Console.WriteLine("File not found");
				return;
			}


			Dictionary<string, double> weights = new Dictionary<string, double> () {
				{"What", 0.2},
				{"When", 0.15},
				{"Where", 0.1},
				{"Who", 0.05},
				{"Why", 0.2},
				{"How to fix", 0.3}
			};

            bool incomplete = false;

            var errorLines = new Dictionary<string, ErrorDescription>();
            ReadBenchmark(ref errorLines, args[1]);

            var sr = System.IO.File.OpenText(args[0]);
            string line;
            while ((line = sr.ReadLine()) != null)
            {

                var fileLine = line.Trim();
                var analyzer = sr.ReadLine().Trim();
                var what = sr.ReadLine().Trim();
                var when = sr.ReadLine().Trim();
                var where = sr.ReadLine().Trim();
                var who = sr.ReadLine().Trim();
                var why = sr.ReadLine().Trim();
                var howtofix = sr.ReadLine().Trim();
                sr.ReadLine();

                if (!errorLines.ContainsKey(fileLine))
                {
                    if (!incomplete)
                    {
                        Console.WriteLine("Unknown error reports found, report is incomplete");
                        incomplete = true;
                    }
                    errorLines.Add(fileLine, new ErrorDescription() { True = false });
                }
                var report = new Report();
                report.ParseNameAndCorrectness(analyzer);
                report.Answers.Add(new Answer(what));
                report.Answers.Add(new Answer(when));
                report.Answers.Add(new Answer(where));
                report.Answers.Add(new Answer(who));
                report.Answers.Add(new Answer(why));
                report.Answers.Add(new Answer(howtofix));

                errorLines[fileLine].Reports.Add(report);
            }
                
            Console.WriteLine("What? " + Math.Round(getRateForQuestion(ref errorLines, "What") * 100, 2) + "%");
            Console.WriteLine("When? " + Math.Round(getRateForQuestion(ref errorLines, "When") * 100, 2) + "%");
            Console.WriteLine("Where? " + Math.Round(getRateForQuestion(ref errorLines, "Where") * 100, 2) + "%");
            Console.WriteLine("Who? " + Math.Round(getRateForQuestion(ref errorLines, "Who") * 100, 2) + "%");
            Console.WriteLine("Why? " + Math.Round(getRateForQuestion(ref errorLines, "Why") * 100, 2) + "%");
            Console.WriteLine("How to fix? " + Math.Round(getRateForQuestion(ref errorLines, "How to fix") * 100, 2) + "%");

            Console.WriteLine();

            if (!incomplete)
            {
                var errorLinesByFile = errorLines.GroupBy((a) => Path.GetFileName(a.Key).Substring(0, Path.GetFileName(a.Key).IndexOf(":")));
                foreach (var el in errorLinesByFile)
                {   
                    var tmpLines = el.ToList();
                   
                    double tpi = tmpLines.Where((a) => a.Value.True).Select((a) => a.Value.Calculate(ref weights)).Sum();
                    double tp = tmpLines.Where((a) => a.Value.True).Select((a) => a.Value.Calculate(ref weights)).Where((a) => a > 0.0).Count();

                    double fpi = tmpLines.Where((a) => !a.Value.True).Select((a) => a.Value.Calculate(ref weights)).Sum();
                    double fp = tmpLines.Where((a) => !a.Value.True).Select((a) => a.Value.Calculate(ref weights)).Where((a) => a > 0.0).Count();

                    var realTrue = tmpLines.Where((a) => a.Value.True).Count();
                    var realFalse = tmpLines.Where((a) => !a.Value.True).Count();

                    var fn = realTrue - tp;
                    double precision = tp / (tp + fp);
                    double recall = tp / (tp + fn);
                    double f1 = 2 * precision * recall / (precision + recall);

                    if (tpi > 0)
                    {
                        Console.Error.WriteLine(el.Key);
                        Console.Error.WriteLine(/*"Quality: " + */(tpi / tp));
                        Console.Error.WriteLine(/*"Precision: " + */precision);
                        Console.Error.WriteLine(/*"Recall: " + */recall);
                        Console.Error.WriteLine(/*"F1: " + */f1);
                        Console.Error.WriteLine();
                    }
                }
            }

            if (!incomplete)
            {
                double tpi = errorLines.Where((a) => a.Value.True).Select((a) => a.Value.Calculate(ref weights)).Sum();
                double tp = errorLines.Where((a) => a.Value.True).Select((a) => a.Value.Calculate(ref weights)).Where((a) => a > 0.0).Count();

                double fpi = errorLines.Where((a) => !a.Value.True).Select((a) => a.Value.Calculate(ref weights)).Sum();
                double fp = errorLines.Where((a) => !a.Value.True).Select((a) => a.Value.Calculate(ref weights)).Where((a) => a > 0.0).Count();

                var realTrue = errorLines.Where((a) => a.Value.True).Count();
                var realFalse = errorLines.Where((a) => !a.Value.True).Count();

                var fn = realTrue - tp;
                double precision = tp / (tp + fp);
                double recall = tp / (tp + fn);
                double f1 = 2 * precision * recall / (precision + recall);
                Console.WriteLine("Quality: " + (tpi / tp));
                Console.WriteLine("TP: " + tpi + "(" + tp + ") / " + realTrue);
                Console.WriteLine("FP: " + fpi + "(" + fp + ") / " + realFalse);
                Console.WriteLine("Precision: " + precision);
                Console.WriteLine("Recall: " + recall);
                Console.WriteLine("F1: " + f1);
                Console.WriteLine("TP/(TP + FP): " + Math.Round(tp / (tp + fp) * 100, 2) + "%");
                Console.WriteLine();
            }
            sr.Close();
		}
	}
}
