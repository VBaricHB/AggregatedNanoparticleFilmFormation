using System;
using System.IO;
using Common.interfaces;

namespace Common
{
    public class FileLogger : ILogger
    {
        private readonly string _file;

        public FileLogger(string file)
        {
            _file = file;
        }

        public void Info(string message)
        {
            var currentTime = DateTime.Now.ToString("u");
            using StreamWriter file =
            new StreamWriter(_file, true);
            file.WriteLine($"{currentTime} - Info: {message}");
        }

        public void Warn(string message)
        {
            var currentTime = DateTime.Now.ToString("u");
            using StreamWriter file =
            new StreamWriter(_file, true);
            file.WriteLine($"{currentTime} - Warn: {message}");
        }
    }
}
