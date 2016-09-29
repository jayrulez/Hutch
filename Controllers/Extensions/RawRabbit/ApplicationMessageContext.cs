using RawRabbit.Context;
using System;

namespace Hutch.Extensions.RawRabbit
{
	public class ApplicationMessageContext : MessageContext, IApplicationMessageContext
	{
		public Action<bool?> Nack { get; set; }
		public Action<TimeSpan> RetryLater { get; set; }
		public RetryInformation RetryInfo { get; set; }
	}
}