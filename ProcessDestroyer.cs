using System.Diagnostics;
using System.Windows.Forms;

namespace TrayApp
{
    public class ProcessDestroyer
    {
        public static void KillCompilerProcesses()
        {
            // TODO: Move to idle loop
            //compiler_processes = Helper.Convert(Properties.Settings.Default.KnownGPUProcesses);
            foreach (string process_name in MainApplication.Properties.Settings.Default.KnownGPUProcesses)
            {
                KillProcessByName(process_name);
            }
        }

        public static void KillProcessByName(string processToKill)
        {
            if (processToKill == null)
                return;
            // save cycles and do this once
            System.Collections.Generic.IEnumerable<Process> processes_list = Process.GetProcessesByName(processToKill);
            foreach (Process x in processes_list)
            {
                if (x.MainWindowTitle == processToKill // Kill by window Title! Works with UWP Apps!
                    || x.ProcessName == processToKill // Kill by app name!
                    )
                {
                    if (processToKill == "Dropbox" && !MainApplication.Properties.Settings.Default.KillDropbox)
                    {
                        continue;
                    }
                    string process_with_id = processToKill + " [" + x.Id + "]";
                    try
                    {
                        //Debug.WriteLine("Killing " + process_with_id);
                        x.Kill();
                        x.WaitForExit();
                        x.Dispose();
                    }
                    //#if DEBUG
                    catch (System.Exception e)
                    {
                        MessageBox.Show("I'm sorry master..." + System.Environment.NewLine + System.Environment.NewLine +
                            "I couldn't kill " + process_with_id + "... (シ_ _)シ" + System.Environment.NewLine + System.Environment.NewLine +
                            "It said:" + System.Environment.NewLine + e.Message + System.Environment.NewLine + System.Environment.NewLine + e.ToString(), "Gomenasai!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    //#endif
                    finally
                    {
                        Debug.WriteLine("Killed " + process_with_id);
                    }
                }
            }
        }
    }
}
