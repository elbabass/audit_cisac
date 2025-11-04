using System;
using System.ComponentModel;

namespace SpanishPoint.Azure.Iswc.Framework.Converters
{
    /// <summary>
    /// Source: https://stackoverflow.com/a/1833128/2509703
    /// </summary>
    public static class TConverter
    {
        public static T ChangeType<T>(object value)
        {
            return (T)ChangeType(typeof(T), value);
        }

        public static object ChangeType(Type t, object value)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(t);
            return tc.ConvertFrom(value)!;
        }

        public static void RegisterTypeConverter<T, TC>() where TC : TypeConverter
        {
            TypeDescriptor.AddAttributes(typeof(T), new TypeConverterAttribute(typeof(TC)));
        }
    }
}
