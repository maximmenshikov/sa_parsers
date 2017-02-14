using System;
using System.Collections.Generic;

namespace report_calc
{
	public class Answer
	{

        public Answer()
        {
        }

        public Answer(string input)
        {
            Parse(input);
        }

		/// <summary>
		/// Gets or sets the informational value of the answer.
		/// </summary>
		/// <value>The informational value of the answer</value>
		public double InformationalValue { get; set; }

		/// <summary>
		/// Gets or sets the text of the answer.
		/// </summary>
		/// <value>The text of the answer.</value>
		public string Text { get; set; }

		/// <summary>
		/// Question which is being answered.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Parse message in "W? (50%) Something" format.
		/// </summary>
		/// <param name="input">Input string</param>
		public void Parse(string input)
		{
			Name = input.Substring (0, input.IndexOf ("?"));
			Text = input.Substring (Name.Length + 1).Trim ();
			InformationalValue = 1;
			if (Text.StartsWith ("(") && Text.Contains (")")) {
				string tmp = Text.Substring (Text.IndexOf ("(") + 1, Text.IndexOf (")") - Text.IndexOf ("(") - 1).Replace ("%", "");
				InformationalValue = double.Parse (tmp) / 100;

				Text = Text.Substring (Text.IndexOf (")") + 1).Trim ();
			}
			if (Text.Trim () == "-")
				InformationalValue = 0;
		}

		/// <summary>
		/// Calculate final score for this answer using weight dictionary.
		/// </summary>
		/// <param name="weights">Weight dictionary</param>
		public double Calculate(ref Dictionary<string, double> weights)
		{
			return weights [Name] * InformationalValue;
		}
	}

}

