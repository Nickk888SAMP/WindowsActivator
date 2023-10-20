using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32.TaskScheduler;

namespace WindowsActivator.Classes
{
    public static class Automator
    {
        public static bool Install()
        {
            try
            {
                // Paths
                string appDirectory = Paths.GetPath(Paths.Path.AppDirectory);
                string appFileName = AppDomain.CurrentDomain.FriendlyName;
                string appFileDir = AppDomain.CurrentDomain.BaseDirectory;
                string combinedFileDir = $"{appFileDir}{appFileName}";
                string combinedCopyFileToDir = $@"{appDirectory}\app.exe";
                string combinedAutomatorScriptFileDir = $@"{appDirectory}\run.vbs";
                string automatorScriptContent = GetAutomatorScriptContent("app.exe");
                
                //Files
                CreateFiles(combinedFileDir, combinedCopyFileToDir, combinedAutomatorScriptFileDir, automatorScriptContent);

                //Task Scheduler
                CreateScheduledTask(combinedAutomatorScriptFileDir);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool Uninstall()
        {
            try
            {
                // Files
                if (Directory.Exists(Paths.GetPath(Paths.Path.AppDirectory)))
                {
                    Directory.Delete(Paths.GetPath(Paths.Path.AppDirectory), true);
                }

                // Scheduled Task
                RemoveScheduledTask();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public static bool IsInstalled()
        {
            return Directory.Exists(Paths.GetPath(Paths.Path.AppDirectory)) && TaskService.Instance.GetTask("Windows Activator") != null;
        }

        private static string GetAutomatorScriptContent(string appName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WindowsActivator.Resources.automatorscript.vbs"))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    string content = sr.ReadToEnd();
                    return content.Replace("*FILENAME*", appName);
                }
            }
        }


        private static void CreateFiles(string combinedFileDir, string combinedCopyFileToDir, string combinedAutomatorScriptFileDir, string automatorScriptContent)
        {
            // Create the directory for the files.
            if (!Directory.Exists(Paths.GetPath(Paths.Path.AppDirectory)))
            {
                Directory.CreateDirectory(Paths.GetPath(Paths.Path.AppDirectory));
            }

            // Copy app file to the directory.
            File.Copy(combinedFileDir, combinedCopyFileToDir, true);

            // Create the activation script.
            using (var stream = new StreamWriter(combinedAutomatorScriptFileDir))
            {
                stream.Write(automatorScriptContent);
            }
        }

        private static void RemoveScheduledTask()
        {
            TaskService.Instance.RootFolder.DeleteTask("Windows Activator", false);
        }

        private static void CreateScheduledTask(string startScriptPath)
        {
            // Remove any existent Task
            RemoveScheduledTask();

            // Task
            TaskDefinition td = TaskService.Instance.NewTask();
            td.RegistrationInfo.Description = "Reactivates Windows every 7 days with the KMS method to keep the system activated.";
            td.Principal.LogonType = TaskLogonType.S4U;
            td.Principal.RunLevel = TaskRunLevel.Highest;
            td.Settings.StopIfGoingOnBatteries = false;
            td.Settings.StartWhenAvailable = true;

            // Trigger
            WeeklyTrigger wt = new WeeklyTrigger();
            wt.StartBoundary = DateTime.Today.AddDays(7).AddHours(6);
            wt.DaysOfWeek = DaysOfTheWeek.AllDays;
            wt.WeeksInterval = 1;
            wt.Repetition.Duration = TimeSpan.Zero;
            wt.Repetition.Interval = TimeSpan.FromDays(7);
            td.Triggers.Add(wt);

            // Action
            td.Actions.Add(startScriptPath);

            // Register
            Task task = TaskService.Instance.RootFolder.RegisterTaskDefinition("Windows Activator", td);
        }

    }
}
