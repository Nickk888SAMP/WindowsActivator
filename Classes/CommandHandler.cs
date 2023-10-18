using System.Diagnostics;

namespace WindowsActivator.Classes
{
    static class CommandHandler
    {
        public static string RunCommand(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = Paths.GetPath(Paths.Path.CMD),
                Arguments = $"/c {command}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process ps = new Process { StartInfo = psi };
            ps.Start();

            string output = ps.StandardOutput.ReadToEnd();
            ps.WaitForExit();
            return output;
        }
    }
}
