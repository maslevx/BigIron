using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BigIron.Converters;
using FileHelpers;

namespace BigIron.Common
{
    public class RecordFormatFactory
    {
        public List<BinaryRecordFormat> GetRecordFormats(params Type[] recordTypes)
        {
            if(recordTypes == null || recordTypes.Length == 0)
                throw new ArgumentException("No record types provided");

            return recordTypes.Select(GetRecordFormat).ToList();
        }

        public BinaryRecordFormat GetRecordFormat(Type recordType)
        {
            ResetFieldInfoCache(recordType);
            BinaryRecordFormat format = new BinaryRecordFormat {Name = recordType.Name};
            format.Fields = new List<RecordFieldInfo>(GetFields(recordType));
            AssignIdentifier(format,recordType);

            return format;
        }

        private void AssignIdentifier(BinaryRecordFormat format, Type record)
        {
            format.Identifier = String.Empty; // default

            FieldInfo[] fields = record.GetFields();
            int position = 0;
            for(int i=0; i < fields.Length; i++)
            {
                var id = fields[i].GetCustomAttribute<RecordIdentifierAttribute>();
                if (id != null)
                {
                    format.Identifier = id.Value;
                    format.IdentifierPosition = position;
                    format.IdentifierLength = format.Fields[i].Length;
                    break;
                }
                position += format.Fields[i].Length;
            }
        }

        private IEnumerable<RecordFieldInfo> GetFields(Type record)
        {
            foreach(FieldInfo field in record.GetFields())
                yield return DetermineFieldFormat(field);
        }

        internal RecordFieldInfo DetermineFieldFormat(FieldInfo field)
        {
            RecordFieldInfo format = null;
            field.WithAttribute<FieldFixedLengthAttribute>(x =>
                format = new RecordFieldInfo {Length = (int) x.GetFieldValue("Length")});

            if (format == null)
                throw new Exception("Missing FieldFixedLength attribute on " + field.Name);

            field.WithAttribute<FieldConverterAttribute>(x =>
            {
                var convert = x.GetFieldValue("Converter");
                if(convert is IPackedField)
                {
                    format.IsPacked = true;
                    format.Length = format.Length/2;
                }
            });

            format.FieldType = field.FieldType;

            return format;
        }

        private static PropertyInfo m_reflectionCache;
        /// <summary>
        /// clear the CLR reflection cache to guarantee we get fields in order
        /// </summary>
        /// <param name="type"></param>
        static void ResetFieldInfoCache(Type type)
        {
            if (m_reflectionCache == null)
            {
                m_reflectionCache = type.GetType().GetProperty("Cache",
                    BindingFlags.DeclaredOnly |
                    BindingFlags.Instance |
                    BindingFlags.NonPublic);
            }

            if (m_reflectionCache != null)
            {
                var cacheObject = m_reflectionCache.GetValue(type, null);

                var cacheField = cacheObject.GetType().GetField("m_fieldInfoCache",
                    BindingFlags.FlattenHierarchy | BindingFlags.Instance |
                    BindingFlags.NonPublic);

                if (cacheField != null)
                    cacheField.SetValue(cacheObject,null);
            }
        }
    }
}
