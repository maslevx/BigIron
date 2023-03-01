using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace BigIron.Converters
{
    /// <summary>
    /// converts numeric fields with implied decimal and overpunch to/from a <c>double</c> value
    /// </summary>
    /// <seealso cref="SignedInt"/>
    /// <inheritdoc/>
    public sealed class SignedImpliedDouble : ConverterBase
    {
        private int DecimalPlaces { get; set; }

        public SignedImpliedDouble(int impliedDecimals)
        {
            if (impliedDecimals < 0) throw new ArgumentOutOfRangeException(nameof(impliedDecimals), "negative decimal places, really?");

            DecimalPlaces = impliedDecimals;
        }

        public SignedImpliedDouble() : this(2) { }

        public override string FieldToString(object from)
        {
            StringBuilder result = new StringBuilder();
            bool isNegative = ((double) from) < 0;
            char lastChar;
            if (isNegative)
            {
                result.Append(Convert.ToInt32((double) from*-100).ToString());
                lastChar = Overpunch.Negative[int.Parse(result[result.Length - 1].ToString())];
            }
            else
            {
                result.Append(Convert.ToInt32((double) from*100).ToString());
                lastChar = Overpunch.Positive[int.Parse(result[result.Length - 1].ToString())];
            }
            //change last digit to the items...
            result[result.Length - 1] = lastChar;

            return result.ToString();
        }

        public override object StringToField(string from)
        {
            string value = Overpunch.ConvertFromOverpunch(from);
            value = value.Insert(value.Length - DecimalPlaces, ".");

            return Double.Parse(value);
        }
    }
}
