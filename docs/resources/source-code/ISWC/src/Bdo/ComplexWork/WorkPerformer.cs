using System;
using System.Collections.Generic;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Bdo.ComplexWork
{
    public class WorkPerformer
    {
        public WorkPerformer()
        {
            PersonFullName = string.Empty;
            LastName = string.Empty;
            RoleType = string.Empty;
        }
        public long PersonID { get; set; }
        public string PersonFullName { get; set; }
        public string LastName { get; set; }
        public string RoleType { get; set; }
    }
}
