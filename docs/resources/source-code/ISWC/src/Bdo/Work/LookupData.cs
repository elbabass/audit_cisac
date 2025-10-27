using System.Collections.Generic;

namespace SpanishPoint.Azure.Iswc.Bdo.Work
{
    public class LookupData
    {
        public LookupData(string key, IEnumerable<object> values)
        {
            Key = key;
            Values = values;
        }

        public string Key { get; }
        public IEnumerable<object> Values { get; }
    }
}
