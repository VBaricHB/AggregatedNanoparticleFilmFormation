using System.Collections.Generic;
using Common.interfaces;

namespace Common
{
    public class MultiLogger : ILogger
    {
        private List<ILogger> Logger { get; }

        public MultiLogger()
        {
            Logger = new List<ILogger>();
        }

        public void AddLogger(ILogger logger)
        {
            Logger.Add(logger);
        }

        public void Info(string message)
        {
            foreach(var log in Logger)
            {
                log.Info(message);
            }
        }

        public void Warn(string message)
        {
            foreach(var log in Logger)
            {
                log.Warn(message);
            }
        }
    }
}
