// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Reflection;
using System.Windows.Forms;
using TrayApp.Properties;

namespace TrayApp
{
    /// <summary>
    ///
    /// </summary>
    internal class ProcessIcon : IDisposable
    {
        /// <summary>
        /// The NotifyIcon object.
        /// </summary>
        public static NotifyIcon ni;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessIcon"/> class.
        /// </summary>
        public ProcessIcon()
        {
            // Instantiate the NotifyIcon object.
            ni = new NotifyIcon();
        }

        /// <summary>
        /// Displays the icon in the system tray.
        /// </summary>
        public void Display()
        {
            // Put the icon in the system tray and allow it react to mouse clicks.
            ni.MouseClick += new MouseEventHandler(ni_MouseClick);
            ni.Icon = Resources.TrayReader;
            ni.Text = "TrayReader";
            ni.Visible = true;

            // Attach a context menu.
            //ni.ContextMenuStrip = new ContextMenus().Create();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            // When the application closes, this will remove the icon from the system tray immediately.
            ni.Dispose();
        }

        /// <summary>
        /// Handles the MouseClick event of the ni control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void ni_MouseClick(object sender, MouseEventArgs e)
        {
            MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
            // Handle mouse button clicks.
            if (e.Button == MouseButtons.Left)
            {
                ni.ContextMenuStrip = new ContextMenus().CreateLoadingMenu();
                mi.Invoke(ni, null);
                ni.ContextMenuStrip = new ContextMenus().CreateFeedsMenu(false);
                mi.Invoke(ni, null);
            }
            else if (e.Button == MouseButtons.Right)
            {
                ni.ContextMenuStrip = new ContextMenus().CreateOptionsMenu();
                mi.Invoke(ni, null);
            }
        }
    }
}
