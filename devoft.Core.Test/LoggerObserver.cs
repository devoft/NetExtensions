using Microsoft.Extensions.Logging;
using System;

namespace devoft.Core.Test
{
    public class LoggerObserver : IObserver<string>
    {
        private ILogger _logger;

        public LoggerObserver(ILogger logger)
        {
            _logger = logger;
        }

        public void OnCompleted()
        {
            _logger.LogInformation("<Completed>");
        }

        public void OnError(Exception error)
        {
            _logger.LogError(error.Message);
        }

        public void OnNext(string value)
        {
            _logger.LogInformation(value);
        }
    }
}