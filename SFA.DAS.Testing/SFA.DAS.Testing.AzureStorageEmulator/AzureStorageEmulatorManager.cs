using System;
using System.Diagnostics;
using System.Linq;

namespace SFA.DAS.Testing.AzureStorageEmulator
{
    public static class AzureStorageEmulatorManager
    {
        public static bool IsProcessRunning()
        {
            try
            {
                bool status;

                using (Process process = Process.Start(StorageEmulatorProcessFactory.Create(ProcessCommand.Status)))
                {
                    if (process == null)
                    {
                        return false;
                    }

                    status = GetStatus(process);
                    process.WaitForExit();
                }

                return status;
            }
            catch
            {
                return false;
            }
        }

        public static void StartStorageEmulator()
        {
            try
            {
                if (!IsProcessRunning())
                {
                    ExecuteProcess(ProcessCommand.Start);
                }
            }
            catch
            {
            }
        }

        public static void StopStorageEmulator()
        {
            if (IsProcessRunning())
            {
                ExecuteProcess(ProcessCommand.Stop);
            }
        }

        private static void ExecuteProcess(ProcessCommand command)
        {
            string error;

            using (Process process = Process.Start(StorageEmulatorProcessFactory.Create(command)))
            {
                if (process == null)
                {
                    return;
                }

                error = GetError(process);
                process.WaitForExit();
            }
        }

        private static class StorageEmulatorProcessFactory
        {
            public static ProcessStartInfo Create(ProcessCommand command)
            {
                return new ProcessStartInfo {
                    FileName = @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe",
                    Arguments = command.ToString().ToLower(),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }
        }

        private enum ProcessCommand
        {
            Start,
            Stop,
            Status
        }

        private static bool GetStatus(Process process)
        {
            string output = process.StandardOutput.ReadToEnd();
            string isRunningLine = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).SingleOrDefault(line => line.StartsWith("IsRunning"));

            if (isRunningLine == null)
            {
                return false;
            }

            return Boolean.Parse(isRunningLine.Split(':').Select(part => part.Trim()).Last());
        }

        private static string GetError(Process process)
        {
            string output = process.StandardError.ReadToEnd();
            return output.Split(':').Select(part => part.Trim()).Last();
        }
    }
}
