using RawRabbit.Context;
using System;
using RawRabbit.Context.Enhancer;
using RawRabbit.Channel.Abstraction;
using RawRabbit.Common;
using RawRabbit.Consumer.Abstraction;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using RabbitMQ.Client;

namespace Hutch.Extensions.RawRabbit
{
    public class ApplicationContextEnhancer : IContextEnhancer
    {
		private readonly IChannelFactory _channelFactory;
		private readonly INamingConventions _conventions;
        public ApplicationContextEnhancer(IChannelFactory channelFactory, INamingConventions conventions)
        {
			_channelFactory = channelFactory;
			_conventions = conventions;
        }

		public void WireUpContextFeatures<TMessageContext>(TMessageContext context, IRawConsumer consumer, BasicDeliverEventArgs args) where TMessageContext : IMessageContext
		{
			var advancedCtx = context as IApplicationMessageContext;
			if (advancedCtx == null)
			{
				return;
			}

			advancedCtx.Nack = (requeue) =>
			{
                if(requeue == null)
                {
                    requeue = true;
                }

				consumer.AcknowledgedTags.Add(args.DeliveryTag);
				consumer.Model.BasicNack(args.DeliveryTag, false, requeue.Value);
			};

			advancedCtx.RetryLater = timespan =>
			{
				var dlxName = _conventions.RetryLaterExchangeConvention(timespan);
				var dlQueueName = _conventions.RetryLaterExchangeConvention(timespan);
				var channel = _channelFactory.CreateChannel();
				channel.ExchangeDeclare(dlxName, ExchangeType.Direct, true, true, null);
				channel.QueueDeclare(dlQueueName, true, false, true, new Dictionary<string, object>
				{
						{QueueArgument.DeadLetterExchange, args.Exchange},
						{QueueArgument.Expires, Convert.ToInt32(timespan.Add(TimeSpan.FromSeconds(1)).TotalMilliseconds)},
						{QueueArgument.MessageTtl, Convert.ToInt32(timespan.TotalMilliseconds)}
				});
				channel.QueueBind(dlQueueName, dlxName, args.RoutingKey, null);
				UpdateHeaders(args.BasicProperties);
				channel.BasicPublish(dlxName, args.RoutingKey, args.BasicProperties, args.Body);
				channel.QueueUnbind(dlQueueName, dlxName, args.RoutingKey, null);
			};

			advancedCtx.RetryInfo = GetRetryInformatino(args.BasicProperties);
		}

        		private static void UpdateHeaders(IBasicProperties basicProperties)
		{
			if (basicProperties.Headers.ContainsKey(PropertyHeaders.EstimatedRetry))
			{
				basicProperties.Headers.Remove(PropertyHeaders.EstimatedRetry);
			}
			basicProperties.Headers.Add(PropertyHeaders.EstimatedRetry, DateTime.UtcNow.ToString("u"));

			var currentRetry = 0;
			if (basicProperties.Headers.ContainsKey(PropertyHeaders.RetryCount))
			{
				var valueStr = GetHeaderString(basicProperties.Headers, PropertyHeaders.RetryCount);
				currentRetry = int.Parse(valueStr);
				basicProperties.Headers.Remove(PropertyHeaders.RetryCount);
			}
			var nextRetry = (++currentRetry).ToString();
			basicProperties.Headers.Add(PropertyHeaders.RetryCount, nextRetry);
		}

		private static string GetHeaderString(IDictionary<string, object> headers, string key)
		{
			var headerBytes = headers[key] as byte[] ?? new byte[0];
			var headerStr = System.Text.Encoding.UTF8.GetString(headerBytes);
			return headerStr;
		}

		private static RetryInformation GetRetryInformatino(IBasicProperties basicProperties)
		{
			return new RetryInformation
			{
				OriginalSent = GetOriginalSentDate(basicProperties),
				NumberOfRetries = GetCurentRetryCount(basicProperties)
			};
		}

		private static DateTime GetOriginalSentDate(IBasicProperties basicProperties)
		{
			if (!basicProperties.Headers.ContainsKey(PropertyHeaders.Sent))
			{
				return DateTime.MinValue;
			}
			var sentBytes = basicProperties?.Headers[PropertyHeaders.Sent] as byte[] ?? new byte[0];
			var sentStr = System.Text.Encoding.UTF8.GetString(sentBytes);
			return DateTime.Parse(sentStr);
		}

		private static long GetCurentRetryCount(IBasicProperties basicProperties)
		{
			if (basicProperties.Headers.ContainsKey(PropertyHeaders.RetryCount))
			{
				var countStr = GetHeaderString(basicProperties.Headers, PropertyHeaders.RetryCount);
				return int.Parse(countStr);
			}
			return 0;
		}
    }
}