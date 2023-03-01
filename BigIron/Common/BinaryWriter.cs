using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigIron.Common
{
    /// <summary>
    /// Writes mainframe files. Will convert to ebcdic and pack values. 
    /// </summary>
    /// <seealso cref="BinaryReader"/>
    public class BinaryWriter
    {
        private List<BinaryRecordFormat> recFormats;
        private Stream outStream;
        private bool writing;
        Encoding ascii = Encoding.ASCII;
        Encoding ebcdic = Encoding.GetEncoding("IBM037");

        public BinaryWriter(IEnumerable<BinaryRecordFormat> formats)
        {
            recFormats = new List<BinaryRecordFormat>(formats);
        }

        /// <summary>
        /// Opens a file for writing
        /// </summary>
        /// <param name="path"></param>
        public void BeginWriteFile(string path)
        {
            BeginWriteStream(new FileStream(path, FileMode.Create));
        }

        /// <summary>
        /// Begin a Write operation against stream
        /// </summary>
        /// <param name="stream"></param>
        public void BeginWriteStream(Stream stream)
        {
            outStream = stream;
            writing = true;
        }

        /// <summary>
        /// Write a record to the currently open file.
        /// </summary>
        /// <param name="value"></param>
        public void WriteNext(string value)
        {
            if(!writing) throw new InvalidOperationException("Must call BeginWrite* before writing");

            var recType = value.Substring(recFormats[0].IdentifierPosition, recFormats[0].IdentifierLength);
            var format = recFormats.FirstOrDefault(x => x.Identifier.Trim() == recType);

            var data = Convert(value, format);
            outStream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Ends writing, closing underlying stream and releasing resources
        /// </summary>
        public void EndWrite()
        {
            outStream.Flush();
            outStream.Close();

            outStream = null;
            recFormats = null;
            writing = false;
        }

        /// <summary>
        /// Writes a string of newline seperated records to a file
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="values"></param>
        public void WriteBinary(Stream stream, string values)
        {
            BeginWriteStream(stream);
            WriteRecordSet(values);
            EndWrite();
        }

        private void WriteRecordSet(string values)
        {
            var records = values.SplitString(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var record in records)
                WriteNext(record);
        }

        private byte[] Convert(string record, BinaryRecordFormat format)
        {
            MemoryStream line = new MemoryStream();

            var extractor = new FieldExtractor(record);
            foreach(var field in format.Fields)
            {
                string fieldValue = extractor.Extract(field);
                if (field.IsPacked)
                    PackNumber(line, fieldValue);
                else
                    ConvertEbcdic(line, fieldValue);
            }
            return line.ToArray();
        }

        private void PackNumber(MemoryStream line, string value)
        {
            value = value.Replace(' ', '0');
            if ((value.Length%2) != 0)
                value = "0" + value;
            for(int x=0; x < value.Length -1; x = x + 2)
            {
                line.WriteByte(Byte.Parse(value.Substring(x, 2), NumberStyles.HexNumber));
            }
        }

        private void ConvertEbcdic(MemoryStream line, string value)
        {
            var ebcdicValue = Encoding.Convert(ascii, ebcdic, ascii.GetBytes(value));
            line.Write(ebcdicValue,0,ebcdicValue.Length);
        }

        private byte[] ConvertAsciiToEbcdic(string asciiData)
        {
            return Encoding.Convert(ascii, ebcdic, Encoding.ASCII.GetBytes(asciiData));
        }

        class FieldExtractor
        {
            readonly string record;
            int position = 0;

            public FieldExtractor(string record)
            {
                this.record = record;
            }

            public string Extract(RecordFieldInfo field)
            {
                int length = field.IsPacked ? field.Length*2 : field.Length;
                string value = Take(length);

                if (field.FieldType != null)
                    value = PreFormatField(value, field.FieldType);

                return value;
            }

            private string Take(int count)
            {
                string result = record.Substring(position, count);
                position += count;

                return result;
            }

            private string PreFormatField(string original, Type fieldType)
            {
                if (fieldType.IsNumeric())
                    original = original.Replace(' ', '0');
                return original;
            }
        }
    }
}
