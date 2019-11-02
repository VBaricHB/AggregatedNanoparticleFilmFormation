using System;
namespace DirectDepositionAlgorithm
{
    public class ConsoleLogger : ILogger
    {
        public ConsoleLogger()
        {
        }

        public void Info(string message)
        {
            Console.Write($"Info: {message}");
        }

        public void Warn(string message)
        {
            Console.Write($"Warn: {message}");
        }
    }
}
