using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigIron.Converters
{
    internal static class Overpunch
    {
        public static readonly char[] Positive = {'{', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I'};
        public static readonly char[] Negative = {'}', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R'};

        private static int GetNumericValue(char value)
        {
            int position = Math.Max(Array.IndexOf(Positive, value), Array.IndexOf(Negative, value));
            if (position == -1)
                throw new ArgumentException("not an overpunch character", nameof(value));

            return position;
        }

        private static bool IsNegative(string value)
        {
            return Negative.Contains(value.Last());
        }

        public static string ConvertFromOverpunch(string value)
        {
            StringBuilder result = new StringBuilder(value.Length);
            if (IsNegative(value))
                result.Append('-');

            result.Append(value.Substring(0, value.Length - 1));
            result.Append(GetNumericValue(value.Last()));

            return result.ToString();
        }

        public static string ConvertToOverpunch(double value)
        {
            StringBuilder result = new StringBuilder();
            result.Append(Math.Abs(value));

            AddSignBit(result, value);

            return result.ToString();
        }

        public static string ConvertToOverpunch(int value)
        {
            StringBuilder result = new StringBuilder();
            result.Append(Math.Abs(value));

            AddSignBit(result, value);

            return result.ToString();
        }

        static void AddSignBit(StringBuilder result, double value)
        {
            int last = Int32.Parse(result[result.Length - 1].ToString());
            char overpunch = (value < 0) ? Negative[last] : Positive[last];
            result[result.Length - 1] = overpunch;
            result.Replace(".", String.Empty);
        }
    }
}
