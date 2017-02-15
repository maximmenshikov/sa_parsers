using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
namespace csv_builder
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Invalid number of arguments");
                return;
            }
                
            var qualities = new Dictionary<string, SortedDictionary<string, double>>();
            var precisions = new Dictionary<string, SortedDictionary<string, double>>();
            var recalls = new Dictionary<string, SortedDictionary<string, double>>();
            var f1s = new Dictionary<string, SortedDictionary<string, double>>();
            var sr = File.OpenText(args[0]);
            string line;
            string currentUtilityName = "";

            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("####"))
                {
                    currentUtilityName = line.Substring(5);
                    continue;
                }
                var file = line.Trim();
                var quality = double.Parse(sr.ReadLine().Trim());
                var precision = double.Parse(sr.ReadLine().Trim());
                var recall = double.Parse(sr.ReadLine().Trim());
                var f1 = double.Parse(sr.ReadLine().Trim());
                sr.ReadLine();

                if (!qualities.ContainsKey(file))
                    qualities.Add(file, new SortedDictionary<string, double>());
                if (!precisions.ContainsKey(file))
                    precisions.Add(file, new SortedDictionary<string, double>());
                if (!recalls.ContainsKey(file))
                    recalls.Add(file, new SortedDictionary<string, double>());
                if (!f1s.ContainsKey(file))
                    f1s.Add(file, new SortedDictionary<string, double>());
                
                qualities[file].Add(currentUtilityName, quality);
                precisions[file].Add(currentUtilityName, precision);
                recalls[file].Add(currentUtilityName, recall);
                f1s[file].Add(currentUtilityName, f1);
            }
                
            foreach (var file in qualities.Keys)
            {
                var s = file + ",";
                foreach (var v in qualities[file])
                {
                    s += "," + v.Value;
                }
                foreach (var v in precisions[file])
                {
                    s += "," + v.Value;
                }
                foreach (var v in recalls[file])
                {
                    s += "," + v.Value;
                }
                foreach (var v in f1s[file])
                {
                    s += "," + v.Value;
                }
            }
        }
    }
}
