using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FastSQL.Core.Loggers
{
    public class LoggerFactory : IDisposable
    {
        private readonly IApplicationManager applicationResourceManager;
        private readonly ResolverFactory resolverFactory;
        private LoggerConfiguration loggerConfiguration;
        private ILogger _container;

        private bool _writeToConsole;
        private bool _writeToApplication;
        private string _channel;
        private bool _writeToFile;
        private string _file;

        public LoggerFactory(
            IApplicationManager applicationResourceManager,
            ResolverFactory resolverFactory,
            LoggerConfiguration loggerConfiguration)
        {
            this.applicationResourceManager = applicationResourceManager;
            this.resolverFactory = resolverFactory;
            this.loggerConfiguration = loggerConfiguration;
        }

        public LoggerFactory WriteToConsole()
        {
            _writeToConsole = true;
            return this;
        }

        public LoggerFactory WriteToApplication(string channel)
        {
            _writeToApplication = true;
            _channel = channel;
            return this;
        }

        public LoggerFactory WriteToFile(string file = "Application")
        {
            _writeToFile = true;
            _file = file;
            return this;
        }

        public ILogger CreateApplicationLogger()
        {
            if (_writeToConsole)
            {
                loggerConfiguration = loggerConfiguration.WriteTo.Console();
            }
            if (_writeToFile && !string.IsNullOrWhiteSpace(_file))
            {
                loggerConfiguration = loggerConfiguration
                   .WriteTo.File(
                       Path.Combine(
                           applicationResourceManager.BasePath,
                           $"{_file}.log"),
                       rollingInterval: RollingInterval.Day,
                       rollOnFileSizeLimit: true);
            }

            if (_writeToApplication)
            {
                loggerConfiguration = loggerConfiguration
                    .WriteTo.ApplicationOutput(resolverFactory, _channel);
            }

            // todo middlewares

            _container = loggerConfiguration.CreateLogger();
            return _container;
        }

        public ILogger CreateErrorLogger()
        {
            if (_writeToConsole)
            {
                loggerConfiguration = loggerConfiguration.WriteTo.Console();
            }

            loggerConfiguration = loggerConfiguration
                .WriteTo.File(
                    Path.Combine(
                        applicationResourceManager.BasePath,
                        $"Errors.log"),
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true);

            loggerConfiguration = loggerConfiguration
                    .WriteTo.ApplicationOutput(resolverFactory, "Error", null);

            // todo middlewares

            _container = loggerConfiguration.CreateLogger();
            return _container;
        }


        public void Dispose()
        {
            resolverFactory.Release(_container);
            //resolverFactory.Release(_logger);
        }
    }
}
