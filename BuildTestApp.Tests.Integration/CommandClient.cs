// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace BuildTestApp.Tests.Integration
{
    public class CommandClient
    {
        private readonly Process cmdProcess;
        private readonly StreamWriter streamWriter;
        private readonly AutoResetEvent outputWaitHandle;
        private string cmdOutput;

        public CommandClient(
            string commandPath,
            string arguments = "",
            bool redirectStandardOutput = true,
            bool useShellExecute = false,
            bool createNoWindow = false)
        {
            cmdProcess = new Process();
            outputWaitHandle = new AutoResetEvent(false);
            cmdOutput = String.Empty;

            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = commandPath,
                UseShellExecute = useShellExecute,
                RedirectStandardOutput = redirectStandardOutput,
                RedirectStandardInput = true,
                CreateNoWindow = createNoWindow
            };

            if (!string.IsNullOrWhiteSpace(arguments))
            {
                processStartInfo.Arguments = arguments;
            }

            cmdProcess.OutputDataReceived += cmdProcessOutputDataReceived;

            cmdProcess.StartInfo = processStartInfo;
            cmdProcess.Start();

            streamWriter = cmdProcess.StandardInput;
            cmdProcess.BeginOutputReadLine();
        }

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="command">The application to run or document to open.</param>
        /// <returns>Returns a string output for the action taken.</returns>
        public string ExecuteCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return string.Empty;
            }

            return RunCommand(command);
        }

        /// <summary>
        /// Executes multiple commands.
        /// </summary>
        /// <param name="commands">The command list of application to run or documents to open.</param>
        /// <returns>Returns a string output for the action taken.</returns>
        public string ExecuteCommand(List<string> commands)
        {
            if (commands == null || commands.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder commandOutput = new StringBuilder();

            foreach (string command in commands)
            {
                if (string.IsNullOrEmpty(command))
                {
                    continue;
                }

                string result = RunCommand(command);
                commandOutput.AppendLine(this.ExecuteCommand(result));
            }

            return commandOutput.ToString();
        }

        private string RunCommand(string command)
        {
            cmdOutput = String.Empty;

            streamWriter.WriteLine(command);
            streamWriter.WriteLine(" echo end");
            outputWaitHandle.WaitOne();
            return cmdOutput;
        }

        private void cmdProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null || e.Data == "end")
                outputWaitHandle.Set();
            else
                cmdOutput += e.Data + Environment.NewLine;
        }

        public void Dispose()
        {
            cmdProcess.Close();
            cmdProcess.Dispose();
            streamWriter.Close();
            streamWriter.Dispose();
        }
    }
}
