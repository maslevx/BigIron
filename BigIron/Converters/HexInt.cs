using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace BigIron.Converters
{
    /// <summary>
    /// Converter handling 32-bit integers as base 16 hex strings
    /// </summary>
    public sealed class HexInt : ConverterBase, IPackedField
    {
        public override object StringToField(string from)
        {
            return Convert.ToInt32(from, 16);
        }

        public override string FieldToString(object from)
        {
            return String.Format("{0:X2}", from);
        }
    }
}
