namespace SpanishPoint.Azure.Iswc.Bdo.Portal
{
	public class Message
	{
		public Message(string header, string messageBody)
		{
			MessageHeader = header;
			MessageBody = messageBody;
		}

		public string MessageHeader { get; set; }
		public string MessageBody { get; set; }
	}
}
