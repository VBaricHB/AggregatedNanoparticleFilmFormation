using System;
namespace DirectDepositionAlgorithm
{
    public interface ILogger
    {
        void Info(string message);
        void Warn(string message);
    }
}
