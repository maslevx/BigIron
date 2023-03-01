using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace BigIron.Converters
{
    /// <summary>
    /// Converts signed COMP-3 floating-point values to/from a <c>double</c>
    /// <c>S9(08)V99 COMP-3</c>
    /// </summary>
    /// <inheritdoc/>
    /// <seealso cref="SignedComp3"/>
    public sealed class SignedComp3Double : ConverterBase, IPackedField
    {
        int decimalPlaces = 2;

        /// <summary>
        /// Converts field with 2 digits after decimal point
        /// </summary>
        public SignedComp3Double() { }

        /// <summary>
        /// Convert field with <paramref name="impliedDecimals"/> digits after decimal point
        /// </summary>
        /// <param name="impliedDecimals">number of digits after the (implied) decimal</param>
        public SignedComp3Double(int impliedDecimals)
        {
            if (impliedDecimals < 0) throw new ArgumentOutOfRangeException(nameof(impliedDecimals), "must be positive");

            decimalPlaces = impliedDecimals;
        }

        public override object StringToField(string @from)
        {
            bool isNegative = false;
            StringBuilder result = new StringBuilder(from.Substring(0, from.Length - 1));
            switch(from.Substring(from.Length-1,1))
            {
                case "D":
                    isNegative = true;
                    break;
            }
            result.Insert(result.Length - decimalPlaces, '.');
            double workValue = Double.Parse(result.ToString());

            if (isNegative)
                workValue *= -1;

            return workValue;
        }

        public override string FieldToString(object from)
        {
            bool isNegative = ((double) from < 0);
            StringBuilder result = new StringBuilder(Math.Abs((double)from * 100).ToString());
            if (isNegative)
                result.Append("D");
            else
                result.Append("C");

            return result.ToString();
        }
    }
}
