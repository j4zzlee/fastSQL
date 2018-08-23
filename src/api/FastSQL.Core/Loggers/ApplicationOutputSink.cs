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
        //private string _owner;
        private string _channel;
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
            var message = logEvent.RenderMessage(_formatProvider);
            eventAggregator.GetEvent<ApplicationOutputEvent>()?.Publish(new ApplicationOutputEventArgument
            {
                //Owner = _owner,
                Channel = _channel,
                Message = message
            });
        }

        //public void SetOwner(string owner)
        //{
        //    _owner = owner;
        //}

        public void SetChannel(string channel)
        {
            _channel = channel;
        }
    }

    public static class ApplicationOutputSinkExtensions
    {
        public static LoggerConfiguration ApplicationOutput(
                  this LoggerSinkConfiguration loggerConfiguration,
                  ResolverFactory resolverFactory,
                  string channel,
                  IFormatProvider fmtProvider = null)
        {
            var sink = resolverFactory.Resolve<ApplicationOutputSink>();
            sink.SetFormatPrivider(fmtProvider);
            //sink.SetOwner(owner);
            sink.SetChannel(channel);
            return loggerConfiguration.Sink(sink);
        }
    }
}
