using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BigIron.Common
{
    /// <summary>
    /// Reads mainframe files, unpacks values, and converts ebcdic -> ascii
    /// </summary>
    public class BinaryReader
    {
        private List<BinaryRecordFormat> Formats { get; set; }

        public BinaryReader(IEnumerable<BinaryRecordFormat> recordFormats)
        {
            Formats = new List<BinaryRecordFormat>(recordFormats);
            EnsureEqualLengthRecords();
            EnsureEqualIdentifiers();
        }

        private void EnsureEqualLengthRecords()
        {
            int length = Formats[0].RecordLength;
            if (!Formats.All(x => x.RecordLength == length))
                throw new Exception("Variables length records not currently supported");
        }

        private void EnsureEqualIdentifiers()
        {
            int position = Formats[0].IdentifierPosition;
            int length = Formats[0].IdentifierLength;

            if (!Formats.All(x => x.IdentifierLength == length && x.IdentifierPosition == position))
                throw new Exception("variable record identifier positions are not supported");
        }

        public IEnumerable<string> ReadBinaryFile(string fileName)
        {
            int recLength = Formats[0].RecordLength;
            int recTypeStart = Formats[0].IdentifierPosition;
            int recTypeLength = Formats[0].IdentifierLength;

            foreach(byte[] rec in LazyReadBinaryFile(fileName, recLength))
            {
                byte[] recTypeBuffer = new byte[recTypeLength];
                Buffer.BlockCopy(rec, recTypeStart, recTypeBuffer, 0, recTypeLength);
                string recType = Encoding.ASCII.GetString(ConvertEbcdicToAscii(recTypeBuffer));
                if (Formats.Exists(x => x.Identifier == recType))
                {
                    BinaryRecordFormat format = Formats.Find(x => x.Identifier == recType);
                    yield return Convert(rec, format);
                }
                else
                    yield return Encoding.ASCII.GetString(ConvertEbcdicToAscii(rec));
            }
        }

        private static IEnumerable<byte[]> LazyReadBinaryFile(string fileName, int recordLen)
        {
            var buffer = new byte[recordLen];

            using (var stream = File.OpenRead(fileName))
            {
                long remaining = stream.Length;
                int read = 0;
                while (remaining > 0)
                {
                    int c = stream.Read(buffer, read, recordLen - read);
                    remaining -= c;
                    read += c;
                    if (read == recordLen)
                    {
                        read = 0;
                        var result = new byte[recordLen];
                        Buffer.BlockCopy(buffer, 0, result, 0, recordLen);
                        yield return result;
                    }
                }
            }
        }

        #region " Conversions "

        private static byte[] ConvertEbcdicToAscii(byte[] ebcdicData, bool scrubNonPrint = false)
        {
            Encoding ascii = Encoding.ASCII;
            Encoding ebcdic = Encoding.GetEncoding("IBM037");

            var result = Encoding.Convert(ebcdic, ascii, ebcdicData);
            if(scrubNonPrint)
                for(int i=0; i < result.Length; i++)
                    if (result[i] < 32 || result[i] > 126)
                        result[i] = 32;

            return result;
        }

        private static byte[] UnpackInt(byte[] value)
        {
            var t = new StringBuilder();
            for (int x = 0; x < value.Length; x++)
                t.AppendFormat("{0:X2}", value[x]);

            return Encoding.ASCII.GetBytes(t.ToString());
        }

        private string Convert(byte[] record, BinaryRecordFormat format)
        {
            var line = new StringBuilder();

            int position = 0;
            foreach(var field in format.Fields)
            {
                int length = field.Length;
                byte[] buffer = new byte[length];
                Buffer.BlockCopy(record, position, buffer, 0, length);

                byte[] fieldValue;
                if (field.IsPacked)
                    fieldValue = UnpackInt(buffer);
                else
                    fieldValue = ConvertEbcdicToAscii(buffer, true);

                line.Append(Encoding.ASCII.GetString(fieldValue));
                position += length;
            }

            return line.ToString();
        }

        #endregion
    }
}
