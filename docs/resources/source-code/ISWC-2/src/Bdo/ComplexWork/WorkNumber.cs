using System;
using System.Collections.Generic;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Bdo.ComplexWork
{
    public class WorkNumber
    {
        public WorkNumber()
        {
            TypeCode = string.Empty;
            Number = string.Empty;
        }
        public string TypeCode { get; set; }
        public string Number { get; set; }
    }
}
