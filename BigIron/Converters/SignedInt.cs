using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace BigIron.Converters
{
    /// <summary>
    /// Converts numeric fields with overpunch to/from an <c>int</c>
    /// </summary>
    /// <seealso cref="SignedImpliedDouble"/>
    /// <inheritdoc/>
    public sealed class SignedInt : ConverterBase
    {
        public override string FieldToString(object from)
        {
            int value = (int) from;
            return Overpunch.ConvertToOverpunch(value);
        }

        public override object StringToField(string from)
        {
            string value = Overpunch.ConvertFromOverpunch(from);
            return Int32.Parse(value);
        }
    }
}
