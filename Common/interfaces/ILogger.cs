using System;
namespace Common.interfaces
{
    public interface ILogger
    {
        void Info(string message);
        void Warn(string message);
    }
}
