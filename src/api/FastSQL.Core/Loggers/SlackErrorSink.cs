using Prism.Events;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Core.Loggers
{
    public class SlackErrorSink : ILogEventSink
    {
        IFormatProvider _formatProvider;
        private string _channel;
        private readonly IEventAggregator eventAggregator;

        public SlackErrorSink(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void SetFormatPrivider(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);
            // Queue slack message
        }

        public void SetChannel(string channel)
        {
            _channel = channel;
        }
    }

    public static class SlackErrorSinkExtensions
    {
        public static LoggerConfiguration SlackError(
                  this LoggerSinkConfiguration loggerConfiguration,
                  ResolverFactory resolverFactory,
                  string channel,
                  IFormatProvider fmtProvider = null)
        {
            var sink = resolverFactory.Resolve<SlackErrorSink>();
            sink.SetChannel(channel);
            sink.SetFormatPrivider(fmtProvider);
            return loggerConfiguration.Sink(sink);
        }
    }
}
