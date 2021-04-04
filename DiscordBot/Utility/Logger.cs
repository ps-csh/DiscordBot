using DiscordBot.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DiscordBot.Utility
{
    public class Logger : ILogger
    {
        readonly string logDirectory;

        private List<string> logs;

        public IReadOnlyList<string> Logs => logs;

        public Logger(IOptions<LoggerConfiguration> options)
        {
            logDirectory = options.Value.Path;
            logs = new List<string>();
        }

        public void LogInfo(string message)
        {
            logs.Add(message);
        }

        public void LogWarning(string message)
        {
            logs.Add($"Warning: {message}");
        }

        public void LogException(Exception ex)
        {
            var baseException = ex.GetBaseException();
            logs.Add($"---EXCEPTION--- {ex.StackTrace}: {baseException.Message}");
        }

        public void LogException(Exception ex, string message)
        {
            var baseException = ex.GetBaseException();
            logs.Add($"---EXCEPTION--- {message}\n{ex.StackTrace}: {baseException.Message}");
        }

        public void WriteLogsToFile()
        {
            try
            {
                if (logs.Count > 0)
                {
                    string data = "";
                    for (int i = 0; i < logs.Count; ++i)
                    {
                        data += logs[i] + "\r\n\r\n";
                    }

                    string filePath = logDirectory + DateTime.Now.ToString("yyyy-MMM-d_HH-mm-ss") + ".txt";
                    FileInfo fileInfo = new FileInfo(filePath);
                    fileInfo.Directory.Create();
                    File.WriteAllText(fileInfo.FullName, data);
                }

            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }

        public void WriteLogsToFile(string filePath)
        {
            try
            {
                if (logs.Count > 0)
                {
                    string data = "";
                    for (int i = 0; i < logs.Count; ++i)
                    {
                        data += logs[i] + "\r\n\r\n";
                    }

                    FileInfo fileInfo = new FileInfo(filePath);
                    fileInfo.Directory.Create();
                    File.WriteAllText(fileInfo.FullName, data);
                }

            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }
    }
}
