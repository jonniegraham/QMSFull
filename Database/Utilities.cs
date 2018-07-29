using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace Database
{
    public class Utilities
    {
        public static string Replace(string pattern, string replacement, string input)
        {
            var rgx = new Regex(pattern);
            return rgx.Replace(input, replacement);
        }

        public static string EqualityExpression(Dictionary<string, Tuple<MySqlDbType, dynamic>> columnValues)
        {
            var setExpression = "";
            foreach (var columnValue in columnValues)
            {
                setExpression += $"{columnValue.Key} = @{columnValue.Key}, ";
            }

            return setExpression.Substring(0, setExpression.Length - 2);
        }

        public static string TermsList(Dictionary<string, Tuple<MySqlDbType, object>> columnValues)
        {
            var listExpression = "";
            foreach (var columnValue in columnValues)
            {
                listExpression += $"{columnValue.Key}, ";
            }
            return listExpression.Substring(0, listExpression.Length - 2);
        }
    }
}
