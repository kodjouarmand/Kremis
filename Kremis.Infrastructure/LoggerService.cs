using System;
using System.IO;
using Kremis.Infrastructure.Contracts;
using Kremis.Utility.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using NLog;

namespace Kremis.Infrastructure
{
    public class LoggerService : ILoggerService
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly LoggingOptions _loggingOptions;
        private readonly IWebHostEnvironment _hostEnvironment;

        public LoggerService(IOptions<LoggingOptions> loggingOptions, IWebHostEnvironment hostEnvironment)
        {
            _loggingOptions = loggingOptions.Value;
            _hostEnvironment = hostEnvironment;
            SetConfig();
        }

        public void SetConfig( )
        {
            _loggingOptions.LogFileName = Path.Combine(_hostEnvironment.WebRootPath, $"{_loggingOptions.LogFileName}");
            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = _loggingOptions.LogFileName };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            var config = new NLog.Config.LoggingConfiguration();
            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            // Apply config           
            LogManager.Configuration = config;
        }

        public void LogDebug(string message)
        {
            _logger.Debug(message);
            _logger.Debug("\n");
        }

        public void LogError(Exception ex)
        {
            _logger.Error($"{ex.Message}");
            if(ex.InnerException != null)
                _logger.Error($"{ex.InnerException.Message}");
            _logger.Error($"{ex.Source}");
            _logger.Error($"{ex.TargetSite}");
            _logger.Error($"{ex.StackTrace.Trim()}");
            _logger.Error("\n");
        }
        public void LogError(string message)
        {
            _logger.Error(message);
            _logger.Error("\n");
        }
        public void LogInfo(string message)
        {
            _logger.Info(message);
            _logger.Info("\n");
        }
        public void LogWarning(string message)
        {
            _logger.Warn(message);
            _logger.Warn("\n");
        }

    }
}
