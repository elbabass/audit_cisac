namespace SpanishPoint.Azure.Iswc.Data.Services.Search.Azure.Models
{
    public class WorkNumbersIndexModel
    {
        public string GeneratedID { get; set; }
        public long? WorkID { get; set; }
        public string TypeCode { get; set; }
        public string Number { get; set; }
        public bool? IsDeleted { get; set; }
    }
}

