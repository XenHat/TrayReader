namespace TrayApp
{
    public class NotificationManager
    {
        public static void PushNotificationToOS(string content, string title = "")
        {
            if (!MainApplication.Properties.Settings.Default.ShowNotifications)
            {
                return;
            }
            if (title == "")
            {
                title = Program.ProductName;
            }
            Program.sTrayIcon.BalloonTipTitle = title;
            Program.sTrayIcon.BalloonTipText = content;
            Program.sTrayIcon.ShowBalloonTip(1);
        }
    }
}
