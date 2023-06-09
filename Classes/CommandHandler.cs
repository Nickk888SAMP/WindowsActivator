﻿using System.Diagnostics;

namespace WindowsActivator.Classes
{
    static class CommandHandler
    {
        public enum CommandType
        {
            CMD,
            PowerShell
        }

        public static string RunCommand(CommandType commandHandler, string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = commandHandler == CommandType.CMD ? Paths.GetPath(Paths.Path.CMD) : Paths.GetPath(Paths.Path.PowerShell),
                Arguments = commandHandler == CommandType.CMD ? $"/c {command}" : command,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process ps = new Process
            {
                StartInfo = psi,
            };

            ps.Start();

            string output = ps.StandardOutput.ReadToEnd();
            ps.WaitForExit();

            return output;
        }
    }
}
