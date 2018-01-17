// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TrayApp
{
    internal class Integration
    {
        public static void AddToStartup(bool add)
        {
            string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            // Empty a few directories. Yes. If your shit is missing, this is the line that does it.
            Type t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
            if (t != null)
            {
                dynamic shell = Activator.CreateInstance(t);
                try
                {
                    if (shell != null)
                    {
                        File.Delete(startupFolder + "\\" + Program.ProductName + ".lnk");
                        if (add)
                        {
                            dynamic startupEntry = shell.CreateShortcut(startupFolder + "\\" + Program.ProductName + ".lnk");
                            try
                            {
                                var currentPathToExe = Directory.GetCurrentDirectory() + "\\" + Program.ProductName + ".exe";
                                startupEntry.TargetPath = currentPathToExe;
                                startupEntry.IconLocation = currentPathToExe;
                                startupEntry.Save();
                            }
                            finally
                            {
                                Marshal.FinalReleaseComObject(startupEntry);
                            }
                        }
                    }
                }
                finally
                {
                    if (shell != null)
                    {
                        Marshal.FinalReleaseComObject(shell);
                    }
                }
            }
            //LogStats();
        }
    }
}
