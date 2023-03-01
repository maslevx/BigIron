using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigIron.Common
{
    /// <summary>
    /// Simple adapter that lets a Filehelpers engine use BinaryReader
    /// </summary>
    /// <inheritdoc/>
    /// <seealso cref="BinaryReader"/>
    public class BinaryRecordReader : FileHelpers.IRecordReader
    {
        private readonly IEnumerator<string> m_reader;

        public BinaryRecordReader(string file, params Type[] recordTypes)
        {
            var formatFactory = new RecordFormatFactory();
            var formats = formatFactory.GetRecordFormats(recordTypes);
            var reader = new BinaryReader(formats);

            m_reader = reader.ReadBinaryFile(file).GetEnumerator();
        }

        public void Close()
        {
            m_reader.Dispose();
        }

        public string ReadRecordString()
        {
            if (m_reader.MoveNext())
                return m_reader.Current;
            else
                return null;
        }
    }
}
