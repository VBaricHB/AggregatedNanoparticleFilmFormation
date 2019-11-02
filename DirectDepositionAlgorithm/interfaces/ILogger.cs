using System;
namespace DirectDepositionAlgorithm.interfaces
{
    public interface ILogger
    {
        void Info(string message);
        void Warn(string message);
    }
}
