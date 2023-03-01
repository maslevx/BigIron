using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigIron.Common
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class RecordIdentifierAttribute : Attribute
    {
        public string Value { get; private set; }

        public RecordIdentifierAttribute(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));
            Value = value;
        }
    }
}
