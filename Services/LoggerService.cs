using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZipTransfer.Services
{
    public class LoggerService
    {
        private string logFileName = "ZipTransferLog.txt";
        private string dateFormat = "MM-dd-yyyy_HH:mm:ss";
        private StreamWriter writer;

        public LoggerService()
        {
            StartLog();
        }

        public void WriteLine(string message)
        {
            var messageWithTimestamp = $"{DateTime.Now.ToString(dateFormat)} - {message}";

            Console.WriteLine(messageWithTimestamp);
            // log to a file
            writer.WriteLine(messageWithTimestamp);
        }

        public void WriteError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteLine(errorMessage);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void StartLog()
        {
            if (!File.Exists(logFileName))
            {
                // create the first log file
                File.Create(logFileName);
            }

            writer = new StreamWriter(logFileName);
            writer.WriteLine("----------------------------------------------------------------------------");
            writer.WriteLine($"Process started at {DateTime.Now.ToString(dateFormat)}");
        }

        public void EndLog()
        {
            writer.Flush();
            writer.Close();
        }

    }
}
