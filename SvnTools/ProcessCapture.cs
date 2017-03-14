using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace SvnTools
{
    public struct ProcessResult
    {
        public string Output;
        public string Error;
        public int ExitCode;
    }

    public static class ProcessCapture
    {
        const int kDefaultTimeoutMs = 60000;

        public static ProcessResult Get(string filename, string arguments, int timeoutMs = kDefaultTimeoutMs)
        {
            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {
                using (var process = new Process())
                {
                    process.StartInfo.FileName = filename;
                    process.StartInfo.Arguments = arguments;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

                    StringBuilder outputBuilder = new StringBuilder();
                    StringBuilder errorBuilder = new StringBuilder();

                    try
                    {
                        process.OutputDataReceived += (sender, e) =>
                        {
                            if (e.Data == null)
                            {
                                outputWaitHandle.Set();
                            }
                            else
                            {
                                outputBuilder.AppendLine(e.Data);
                            }
                        };
                        process.ErrorDataReceived += (sender, e) =>
                        {
                            if (e.Data == null)
                            {
                                errorWaitHandle.Set();
                            }
                            else
                            {
                                errorBuilder.AppendLine(e.Data);
                            }
                        };

                        process.Start();

                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        if (process.WaitForExit(timeoutMs))
                        {
                            return new ProcessResult()
                            {
                                Output = outputBuilder.ToString(),
                                Error = errorBuilder.ToString(),
                                ExitCode = process.ExitCode
                            };
                        }
                        else
                        {
                            throw new Exception(filename + " process timed out!");
                        }
                    }
                    finally
                    {
                        outputWaitHandle.WaitOne(timeoutMs);
                        errorWaitHandle.WaitOne(timeoutMs);
                    }
                }
            }
        }
    }
}
