using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace BigIron.Converters
{
    /// <summary>
    /// Converts signed COMP3 values to/from an <c>int</c>.
    /// <c>S9(06) COMP-3</c> 
    /// </summary>
    /// <remarks>
    /// Last nibbles denotes the sign. 'D' always indicates a negative value, 'C' denotes positive.
    /// However, 'C' is not the only valid indicator for a positive; when reading from a file the
    /// value is assumed positive if the last byte is anything but 'D'
    /// </remarks>
    /// <inheritdoc/>
    /// <seealso cref="SignedComp3Double"/>
    public sealed class SignedComp3 : ConverterBase, IPackedField
    {
        public override object StringToField(string from)
        {
            string value = from.Substring(0, from.Length - 1); //get the value without sign
            string sign = from.Substring(from.Length - 1, 1); //get the sign
            int result = Int32.Parse(value);

            if (sign == "D")
                result *= -1;

            return result;
        }

        public override string FieldToString(object from)
        {
            if (from == null) return String.Empty;

            int value = (int) from;
            string signBit = (value < 0) ? "D" : "C";

            return Math.Abs(value) + signBit;
        }
    }
}
