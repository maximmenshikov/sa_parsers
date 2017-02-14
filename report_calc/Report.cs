using System;
using System.Collections.Generic;
using System.Linq;

namespace report_calc
{
	public class Report
	{
		private List<Answer> _answers = new List<Answer>();

        /// <summary>
        /// Gets the answers associated with this report.
        /// </summary>
        /// <value>The answers.</value>
        public List<Answer> Answers
        {
            get
            {
                return _answers;
            }
        }

		/// <summary>
		/// Utility name
		/// </summary>
		/// <value>Utility name</value>
		public string UtilityName { get; set; }
       
        /// <summary>
        /// Is report correct or false?
        /// </summary>
        /// <value><c>true</c> if correct; otherwise, <c>false</c>.</value>
        public bool Correct { get; set; }

		/// <summary>
		/// Parses the inner name, error class and supported flat from "report (another report) {notfullsupport}" format.
		/// </summary>
		/// <param name="input">Input string</param>
		public void ParseNameAndCorrectness(string input)
		{
			UtilityName = input;
			
			if (UtilityName.Contains ("(")) {
                var correctness = UtilityName.Substring(UtilityName.IndexOf("(") + 1);
                correctness = correctness.Replace(")", "").Replace(":", "").Trim();
                Correct = correctness == "correct";

				UtilityName = UtilityName.Substring (0, UtilityName.LastIndexOf ("(")).Trim ();
			}
		}

        /// <summary>
        /// Calculate the final score for this report based on weights.
        /// </summary>
        /// <param name="weights">Question weights.</param>
		public double Calculate(ref Dictionary<string, double> weights)
		{
            double result = 0.0;
            foreach (var answer in Answers)
            {
                result += answer.Calculate(ref weights);
            }
            return result;
		}
	}
}

