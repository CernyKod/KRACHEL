using KRACHEL.Infrastructure.ValueObject;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace KRACHEL.Infrastructure.FFvideoBuilder
{
    internal class ProcessWorker
    {
        private readonly ILogger<FFVideoBuilder> _logger;

        private readonly List<Process> _processRegister = new List<Process>();

        /// <summary>
        /// [s]
        /// </summary>
        private int _timeout = 60;

        public ProcessWorker() { }

        public ProcessWorker(ILogger<FFVideoBuilder> logger, int timeout) 
        {
            _logger = logger;
            _timeout = timeout;
        }

        public Task<Tuple<int, string>> RunAsync(Command command)
        {
            return Task.Run(() =>
            {
                return Run(command);
            });
        }

        public Tuple<int, string> Run(Command command)
        {
            _logger.LogInformation($"Calling FF process. \n{command.Program}\n{command.Arguments}");

            int exitCode = 0;
            var outputDataBuilder = new StringBuilder();
            var errorDataBuilder = new StringBuilder();

            using (Process p = new Process())
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.OutputDataReceived += (sender, args) => outputDataBuilder.AppendLine(args.Data);
                p.ErrorDataReceived += (sender, args) => errorDataBuilder.AppendLine(args.Data);
                p.StartInfo.FileName = command.Program;
                p.StartInfo.Arguments = command.Arguments;              
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                p.WaitForExit(_timeout * 1000);

                if (!p.HasExited)
                {
                    _logger.LogWarning("FF process timout. Killing process.");
                    p.Kill();
                }

                exitCode = p.ExitCode;
            }

            var outputData = outputDataBuilder.ToString();
            var errorData = errorDataBuilder.ToString();

            _logger.LogInformation($"FF proecss finished with exitcode {exitCode}. \nSTDOUT:{outputData} \nSTDRERR:{errorData}");

            return new Tuple<int, string>(exitCode, exitCode == 0 ? outputData : errorData);
        }
    }
}
