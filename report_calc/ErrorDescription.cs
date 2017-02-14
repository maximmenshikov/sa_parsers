using System;
using System.Collections.Generic;
using System.Linq;

namespace report_calc
{
    public class ErrorDescription
    {
        private List<Report> _reports = new List<Report>();

        /// <summary>
        /// Gets the reports associated with error.
        /// </summary>
        /// <value>The reports.</value>
        public List<Report> Reports
        {
            get
            {
                return _reports;
            }
        }

        /// <summary>
        /// Gets the "file:line" string showing location of the error.
        /// </summary>
        /// <value>The file line.</value>
        public string FileLine { get; set; }

        /// <summary>
        /// Gets the comment associated with the error.
        /// </summary>
        /// <value>The comment.</value>
        public string Comment { get; set; }

        /// <summary>
        /// Is the error true or false?
        /// </summary>
        /// <value>true if error exists, false otherwise</value>
        public bool True { get; set; }

        public double Calculate(ref Dictionary<string, double> weights)
        {
            var results = new Dictionary<string, double>();
            foreach (var report in Reports)
            {
                if (report.Correct != True)
                    continue;
                foreach (var answer in report.Answers)
                {
                    if (!results.ContainsKey(answer.Name))
                    {
                        results.Add(answer.Name, answer.Calculate(ref weights));
                    }
                    else
                    {
                        results[answer.Name] = Math.Max(results[answer.Name], answer.Calculate(ref weights));
                    }
                }
            }
            return results.Values.Sum();
        }
    }
}

