using FastSQL.Core.Events;
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
    public class ApplicationOutputSink : ILogEventSink
    {
        IFormatProvider _formatProvider;
        private readonly IEventAggregator eventAggregator;

        public ApplicationOutputSink(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void SetFormatPrivider(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            var channel = logEvent.Properties?.ContainsKey("Channel") == true
                ? logEvent.Properties["Channel"].ToString()
                : "Application";
            var message = logEvent.RenderMessage(_formatProvider);
            eventAggregator.GetEvent<ApplicationOutputEvent>().Publish(new ApplicationOutputEventArgument
            {
                Channel = channel,
                Message = message
            });
        }
    }

    public static class ApplicationOutputSinkExtensions
    {
        public static LoggerConfiguration ApplicationOutput(
                  this LoggerSinkConfiguration loggerConfiguration,
                  ResolverFactory resolverFactory,
                  IFormatProvider fmtProvider = null)
        {
            var sink = resolverFactory.Resolve<ApplicationOutputSink>();
            sink.SetFormatPrivider(fmtProvider);
            return loggerConfiguration.Sink(sink);
        }
    }
}
