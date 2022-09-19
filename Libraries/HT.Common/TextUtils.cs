using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using HT.Common.Documentation;

namespace HT.Common
{
    public static class TextUtils
    {
        /// <summary>
        /// Test a string against standard operating system wild cards
        /// </summary>
        /// <param name="testValue">String being searched</param>
        /// <param name="searchPattern">? or * in a text string (PH*)</param>
        /// <returns>True if seachPattern matches testValue</returns>
        [Citation("200805100135", AcquiredDate = "200805100135", Author = "reinux", License = "none specified", SourceDate = "2005-09-15", Source = "http://www.codeproject.com/KB/recipes/wildcardtoregex.aspx")]
        public static bool MatchesWildCard(string testValue, string searchPattern)
        {

            Regex regEx = new Regex("^" + Regex.Escape(searchPattern).
              Replace("\\*", ".*").
              Replace("\\?", ".") + "$");

            return regEx.IsMatch(testValue);

        }
        
        public static bool IsAlphaOnly(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            return Regex.IsMatch(input, "^[a-zA-Z]+$");
        }

        public static bool IsAlphaNumericOnly(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            return Regex.IsMatch(input, "^[a-zA-Z0-9]+$");
        }
        public static bool IsNumericOnly(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            return Regex.IsMatch(input, "^[a-zA-Z0-9]+$");
        }
        public static bool IsAlphaNumericWithUnderscoreOnly(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            return Regex.IsMatch(input, "^[a-zA-Z0-9_]+$");
        }


    }
}
