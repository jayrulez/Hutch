using RawRabbit.Context;
using System;

namespace Hutch.Extensions.RawRabbit
{
	public interface IApplicationMessageContext : IMessageContext
	{
		Action<bool?> Nack { get; set; }
		Action<TimeSpan> RetryLater { get; set; }
		RetryInformation RetryInfo { get; set; }
	}
}