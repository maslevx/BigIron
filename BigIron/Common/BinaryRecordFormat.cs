using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BigIron.Common
{
    /// <summary>
    /// For reading binary files we need a list of the record formats
    /// this will hold the necessary data for everything to work.
    /// </summary>
    public class BinaryRecordFormat
    {
        public string Name { get; set; }

        public int RecordLength
        {
            get
            {
                if (Fields == null)
                    return 0;
                return Fields.Sum(x => x.Length);
            }
        }

        public string Identifier { get; set; }
        public int IdentifierPosition { get; set; }
        public int IdentifierLength { get; set; }
        public List<RecordFieldInfo> Fields { get; set; }
    }

    public class RecordFieldInfo
    {
        public int Length { get; set; }
        public bool IsPacked { get; set; }
        public Type FieldType { get; set; }
    }
}
