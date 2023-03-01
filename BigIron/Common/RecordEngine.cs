using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace BigIron.Common
{
    public class RecordEngine<T> where T : class, new()
    {
        private FixedFileEngine<T> m_engine = new FixedFileEngine<T>();

        private Lazy<List<BinaryRecordFormat>> m_format =
            new Lazy<List<BinaryRecordFormat>>(() => new RecordFormatFactory().GetRecordFormats(typeof(T)));

        public IEnumerable<T> ReadFile(string path)
        {
            var reader = new BinaryRecordReader(path, typeof(T));

            var line = reader.ReadRecordString();
            while (line != null)
            {
                yield return m_engine.ReadString(line)[0];
                line = reader.ReadRecordString();
            }

            reader.Close();
        }

        public void WriteFile(string path, IEnumerable<T> records)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                WriteStream(stream, records);
            }
        }

        public void WriteStream(Stream stream, IEnumerable<T> records)
        {
            var writer = GetBinaryWriter();

            var recordstext = m_engine.WriteString(records);
            writer.WriteBinary(stream, recordstext);
        }

        private BinaryWriter GetBinaryWriter()
        {
            return new BinaryWriter(m_format.Value);
        }
    }
}
