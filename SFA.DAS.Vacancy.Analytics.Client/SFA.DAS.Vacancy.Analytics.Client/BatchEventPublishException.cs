namespace Esfa.Vacancy.Analytics
{
	[System.Serializable]
	public class BatchEventPublishException : System.Exception
	{
		public BatchEventPublishException(string message) : base(message) { }
		public BatchEventPublishException(string message, System.Exception inner) : base(message, inner) { }
		protected BatchEventPublishException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}