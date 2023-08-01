using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Bet365LiveAgent
{    
    static class Utils
    {
        public static string TrimStart(this string sourceString, string trimString)
        {
            if (string.IsNullOrEmpty(trimString))
                return sourceString;

            string result = sourceString;
            while (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
            }

            return result;
        }

        public static string TrimEnd(this string sourceString, string trimString)
        {
            if (string.IsNullOrEmpty(trimString))
                return sourceString;

            string result = sourceString;
            while (result.EndsWith(trimString))
            {
                result = result.Substring(0, result.Length - trimString.Length);
            }

            return result;
        }

        public static string Trim(this string sourceString, string trimString)
        {
            if (string.IsNullOrEmpty(trimString))
                return sourceString;

            string result = sourceString;
            while (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
            }
            while (result.EndsWith(trimString))
            {
                result = result.Substring(0, result.Length - trimString.Length);
            }

            return result;
        }

        public static string GenerateRandomNumberString(int length)
        {            
            StringBuilder strBuilder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {                
                strBuilder.Append(random.Next(0, 10));
            }

            return strBuilder.ToString();
        }        
    }
}
