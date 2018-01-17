using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace TrayApp
{
    public class SettingsManager
    {
        private static bool NeedUpgrade = true;

        // TODO: Add a "Clear Settings" button and set NeedUpgrade to false before calling this again
        public static void LoadSettings()
        {
            if (NeedUpgrade)
            {
                MainApplication.Properties.Settings.Default.Upgrade();
                MainApplication.Properties.Settings.Default.Save();
            }

            StringCollection known_processes_from_settings = MainApplication.Properties.Settings.Default.KnownGPUProcesses;
            StringCollection processes_list = new StringCollection();
            //NotificationManager.PushNotificationToOS("Loading processes list...");

            foreach (string url_iter in known_processes_from_settings)
            {
                bool success = Helper.ValidateExecutableName(url_iter);

                if (success)
                {
                    success = !processes_list.Contains(url_iter);
                }

                if (success)
                {
                    processes_list.Add(url_iter);
                }
            }

            // Must be saved after the foreach loop to prevent overwriting the working data
            SettingsManager.WriteNewProcessesList(processes_list);
            List<string> processes_list_real = Helper.Convert(processes_list);
            string first = processes_list_real.First();
            string last = processes_list_real.Last();
            string processes_list_string = "";
            foreach (string process in processes_list_real)
            {
                if (process == first)
                {
                    processes_list_string = process;
                }
                else if (process != last)
                {
                    processes_list_string += ", " + process;
                }
                else
                {
                    processes_list_string += " and " + process;
                }
            }
            NotificationManager.PushNotificationToOS("Processes that will be killed: " + processes_list_string);
        }

        public static void WriteNewProcessesList(StringCollection coll)
        {
            MainApplication.Properties.Settings.Default.KnownGPUProcesses.Clear();
            MainApplication.Properties.Settings.Default.KnownGPUProcesses = coll;
            MainApplication.Properties.Settings.Default.Save();
            MainApplication.Properties.Settings.Default.Reload();
        }
    }
}
