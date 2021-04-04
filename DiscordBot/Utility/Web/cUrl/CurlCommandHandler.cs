using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Utility.Web.cUrl
{
    /// <summary>
    /// Handles Http requests through a Process using curl.exe
    /// A temporary work around for TLS 1.3 issues I was having on Windows 7
    /// </summary>
    public class CurlCommandHandler
    {
        const int TIMEOUT_MILLISECONDS = 10000;
        const string CURL_EXE_PATH = @".\Utility\Web\cUrl\curl.exe";

        private readonly ILogger Logger;

        public CurlCommandHandler(ILogger logger)
        {
            Logger = logger;
        }

        public async Task<CurlReponse> GetRequest(string url)
        {
            var response = new CurlReponse { Success = true };
            try
            {
                Process process = new Process()
                {
                    StartInfo = new ProcessStartInfo(CURL_EXE_PATH, url) { RedirectStandardOutput = true },
                };

                process.Start();

                string output = "";

                // You can pass any delegate that matches the appropriate 
                // signature to ErrorDataReceived and OutputDataReceived
                //process.ErrorDataReceived += (sender, errorLine) => { if (errorLine.Data != null) Trace.WriteLine(errorLine.Data); };
                process.OutputDataReceived += (sender, outputLine) => { if (outputLine.Data != null) output += outputLine.Data; };
                //process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                // Wait for process to end or timeout
                if (process.WaitForExit(TIMEOUT_MILLISECONDS))
                {
                    //var reader = process.StandardOutput;
                    //string output = reader.ReadToEnd();
                    response.Message = output;
                }
                else
                {
                    Logger.LogWarning($"Process timed out: GET {url}");
                    response.Success = false;
                    response.Message = "Process timed out";
                }
                //process.BeginOutputReadLine();
                process.Close();

                return response;

            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }

        public class CurlReponse
        {
            public string Message { get; set; }
            public bool Success { get; set; }
        }
    }
}
