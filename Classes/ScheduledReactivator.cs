using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32.TaskScheduler;

namespace WindowsActivator.Classes
{
    public static class ScheduledReactivator
    {
        /// <summary>
        /// Installs the activator files in the App's directory and creates a task in the Task Scheduler that runs the files every 7 days.
        /// </summary>
        /// <returns></returns>
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
                string runScriptContent = GetAutomatorScriptContent("app.exe");
                
                //Files
                CreateFiles(combinedFileDir, combinedCopyFileToDir, combinedAutomatorScriptFileDir, runScriptContent);

                //Task Scheduler
                CreateScheduledTask(combinedAutomatorScriptFileDir);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Removes all the Activators files in the Activator's directory and deletes the task from the Task Scheduler.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the content from the Resources script file and  replaces the *FILENAME* to the App's Name.
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        private static string GetAutomatorScriptContent(string appName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WindowsActivator.Resources.run.vbs"))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    string content = sr.ReadToEnd();
                    return content.Replace("*FILENAME*", appName);
                }
            }
        }

        /// <summary>
        /// Creates a Directory and creates a copy of the Activator in it, also a script that runs the activator in the background.
        /// </summary>
        /// <param name="combinedFileDir"></param>
        /// <param name="combinedCopyFileToDir"></param>
        /// <param name="combinedAutomatorScriptFileDir"></param>
        /// <param name="runScriptContent"></param>
        private static void CreateFiles(string combinedFileDir, string combinedCopyFileToDir, string combinedAutomatorScriptFileDir, string runScriptContent)
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
                stream.Write(runScriptContent);
            }
        }

        /// <summary>
        /// Creates the scheduled Task.
        /// </summary>
        /// <param name="startScriptPath"></param>
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
            WeeklyTrigger wt = new WeeklyTrigger
            {
                StartBoundary = DateTime.Today.AddDays(7).AddHours(6),
                DaysOfWeek = DaysOfTheWeek.AllDays,
                WeeksInterval = 1
            };
            wt.Repetition.Duration = TimeSpan.Zero;
            wt.Repetition.Interval = TimeSpan.FromDays(7);
            td.Triggers.Add(wt);

            // Action
            td.Actions.Add(startScriptPath);

            // Register
            TaskService.Instance.RootFolder.RegisterTaskDefinition("Windows Activator", td);
        }

        /// <summary>
        /// Checks if the Scheduled reactivation is Installed/Active or not.
        /// </summary>
        /// <returns></returns>
        public static bool IsInstalled() => Directory.Exists(Paths.GetPath(Paths.Path.AppDirectory)) && TaskService.Instance.GetTask("Windows Activator") != null;

        /// <summary>
        /// Removes the schedules Task if exists.
        /// </summary>
        private static void RemoveScheduledTask() => TaskService.Instance.RootFolder.DeleteTask("Windows Activator", false);
    }
}
