using System;
using System.Collections.Generic;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Bdo.ComplexWork
{
    public class Name
    {
        public Name()
        {
            WorkName = string.Empty;
        }
        public string WorkName { get; set; }
        public int WorkNameType { get; set; }
    }
}
