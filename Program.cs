// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// Based on https://www.codeproject.com/Articles/290013/Formless-System-Tray-Application
using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace TrayApp
{
    internal static class Helper
    {
        public static bool NeedUpgrade = true;

        public static System.Collections.Generic.List<string> Convert(System.Collections.Specialized.StringCollection collection)
        {
            System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
            foreach (string item in collection)
            {
                list.Add(item);
            }
            return list;
        }

        public static System.Collections.Specialized.StringCollection Convert(System.Collections.Generic.List<string> list)
        {
            System.Collections.Specialized.StringCollection collection = new System.Collections.Specialized.StringCollection();
            foreach (string item in list)
            {
                collection.Add(item);
            }
            return collection;
        }

        public static bool ValidateURL(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute) && url.Contains(".") && url.Contains("http") && url.Contains("://");
        }
    }

    internal static class Program
    {
        public static ProcessIcon currentIconInstance = new ProcessIcon();
        public static string ProductName = ((AssemblyProductAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), true)[0]).Product;

        internal static void ExceptionHandler(Exception exception)
        {
            // Meep.
            System.Windows.Forms.MessageBox.Show(
                exception.ToString(), ":( Sortahandled Exception! - " + ((AssemblyProductAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), true)[0]).Product.ToString(),
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Quit if already running
            // https://stackoverflow.com/a/6392264
            Mutex mutex = new System.Threading.Mutex(false, ProductName);
            try
            {
                if (mutex.WaitOne(0, false))
                {
                    // Run the application
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    // Show the system tray icon.
                    currentIconInstance.Display();
                    // Make sure the application runs!
                    Application.Run();
                }
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.Close();
                    mutex = null;
                }
            }
        }
    }
}
