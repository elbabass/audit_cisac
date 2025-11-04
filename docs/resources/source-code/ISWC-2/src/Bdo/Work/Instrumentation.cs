namespace SpanishPoint.Azure.Iswc.Bdo.Work
{ 
    public partial class Instrumentation
    {
        public Instrumentation(string code)
        {
            Code = code;
        }
        public string Code { get; }
        public string? Name { get; set; }
    }
}
