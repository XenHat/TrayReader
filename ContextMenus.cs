// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace TrayApp
{
    /// <summary>
    ///
    /// </summary>
    internal class ContextMenus
    {
        /// <summary>
        /// Is the About box displayed?
        /// </summary>
        private bool isAboutLoaded = false;

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>ContextMenuStrip</returns>
        public ContextMenuStrip CreateFeedsMenu(bool allow_notifications = true)
        {
            // Add the default menu options.
            // TODO: Cache the feeds results to avoid heavy rebuilding every time a checkbox value changes :(
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem item;

            if (Helper.NeedUpgrade)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.Save();
                Helper.NeedUpgrade = false;
            }
            Properties.Settings.Default.Reload();
            bool show_notifications = Properties.Settings.Default.ShowNotifications;
            if (!allow_notifications)
            {
                show_notifications = false;
            }
            bool debug = Properties.Settings.Default.Debug;
            List<string> FeedList = Helper.Convert(Properties.Settings.Default.SettingFeedList);
            List<string> feed_title_list = new List<string>();
            if (FeedList != null)
            {
                short max_entry_per_site = Properties.Settings.Default.EntriesPerFeed;
                StringCollection loaded_urls = new StringCollection();

                string temporaryRssFile = System.IO.Path.GetTempFileName();
                foreach (var url_iter in FeedList)
                {
                    string tip_title = "";
                    if (show_notifications)
                    {
                        tip_title = "Loading";
                    }
                    if (loaded_urls.Contains(url_iter))
                    {
                        if (show_notifications)
                        {
                            tip_title += "... (Ignored)";
                        }
                    }
                    else if (!Helper.ValidateInput(url_iter))
                    {
                        if (show_notifications)
                        {
                            tip_title += "... (Invalid)";
                        }
                    }
                    else
                    {
                        // Hack to handle invalid RSS 2.0 dates
                        // https://stackoverflow.com/a/3936714
                        XmlReader r = new MyXmlReader(url_iter);
                        SyndicationFeed feed = SyndicationFeed.Load(r);
                        Rss20FeedFormatter rssFormatter = feed.GetRss20Formatter();
                        XmlTextWriter rssWriter = new XmlTextWriter(temporaryRssFile, Encoding.UTF8);
                        rssWriter.Formatting = Formatting.Indented;
                        rssFormatter.WriteTo(rssWriter);
                        rssWriter.Close();
                        item = new ToolStripMenuItem
                        {
                            Text = rssFormatter.Feed.Title.Text,
                            Image = Properties.Resources.Rss
                        };
                        //if (show_notifications)
                        //{
                        //    showToolTip(tip_title + " " + item.Text + ((debug) ? saved_url : ""));
                        //}
                        feed_title_list.Add(item.Text);
                        {// I cannot find how to obtain the base url within the first <link>blah</link> element, so I'll do a dirty split
                            string main_website_url = "";
                            foreach (string baseurl in url_iter.Split('/').ToList().GetRange(0, 3))
                            {
                                main_website_url += baseurl + "/";
                            }
                            item.Click += delegate (object sender, EventArgs e) { FeedEntry_Click(sender, e, main_website_url); };
                        }

                        // TODO: get image
                        //foreach (SyndicationElementExtension extension in f.ElementExtensions)
                        //{
                        //    XElement element = extension.GetObject<XElement>();
                        //
                        //    if (element.HasAttributes)
                        //    {
                        //        foreach (var attribute in element.Attributes())
                        //        {
                        //            string value = attribute.Value.ToLower();
                        //            if (value.StartsWith("http://") && (value.EndsWith(".jpg") || value.EndsWith(".png") || value.EndsWith(".gif")))
                        //            {
                        //                rssItem.ImageLinks.Add(value); // Add here the image link to some array
                        //            }
                        //        }
                        //    }
                        //}

                        menu.Items.Add(item); // Add menu entry with the feed name
                        for (int i = 0; i < max_entry_per_site; i++) // Add elements from feed
                        {
                            var syndicationItem = rssFormatter.Feed.Items.ElementAt(i);
                            item = new ToolStripMenuItem()
                            {
                                Text = syndicationItem.Title.Text
                            };
                            item.Click += delegate (object sender, EventArgs e) { FeedEntry_Click(sender, e, syndicationItem.Links.ToList().First().Uri.ToString()); };
                            menu.Items.Add(item);
                            //TODO: Get "Info" field and set as tooltip
                        }
                        loaded_urls.Add(url_iter);
                        if (url_iter != FeedList.Last())
                        {
                            menu.Items.Add(new ToolStripSeparator()); // Separator.
                        }
                    }
                }
                // Must be saved after the foreach loop to prevent overwriting the working data
                Properties.Settings.Default.SettingFeedList.Clear();
                Properties.Settings.Default.SettingFeedList = loaded_urls;
                Properties.Settings.Default.Save();
                if (File.Exists(temporaryRssFile))
                {
                    try
                    {
                        File.Delete(temporaryRssFile);
                    }
                    catch (Exception ex)
                    {
                        Program.ExceptionHandler(ex);
                    }
                }
            }

            if (show_notifications)
            {
                string titles = "";
                foreach (string title in feed_title_list)
                {
                    titles += title + System.Environment.NewLine;
                }
                if (titles != "")
                {
                    showToolTip(titles, "Loaded Feeds");
                }
            }
            return menu;
        }

        public ContextMenuStrip CreateLoadingMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem item;
            // Add a feed.
            item = new ToolStripMenuItem()
            {
                Text = "Loading..."
            };
            item.Enabled = false;
            menu.Items.Add(item);
            return menu;
        }

        public ContextMenuStrip CreateOptionsMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem item;

            // Add a feed.
            item = new ToolStripMenuItem()
            {
                Text = "Add Feed",
                Image = Properties.Resources.Rss
            };
            item.Click += new EventHandler(AddFeed_Click);
            menu.Items.Add(item);

            // About box
            item = new ToolStripMenuItem()
            {
                Text = "About",
                Image = Properties.Resources.About
            };
            item.Click += new EventHandler(About_Click);
            menu.Items.Add(item);

            // Notifications On/Off
            item = new ToolStripMenuItem()
            {
                Text = "Notifications",
                Checked = Properties.Settings.Default.ShowNotifications,
            };
            if (item.Checked)
            {
                item.Image = Properties.Resources.checkmark;
            }
            item.Click += new EventHandler(Notification_Setting_Click);
            menu.Items.Add(item);

            // Add to Startup
            item = new ToolStripMenuItem()
            {
                Text = "Run at Login",
                Checked = Properties.Settings.Default.AutomaticStartup,
            };
            if (item.Checked)
            {
                item.Image = Properties.Resources.checkmark;
            }
            item.Click += new EventHandler(Startup_Click);
            menu.Items.Add(item);

            // Separator.
            menu.Items.Add(new ToolStripSeparator());

            // Exit.
            item = new ToolStripMenuItem()
            {
                Text = "Exit",
                Image = Properties.Resources.Exit
            };
            item.Click += new System.EventHandler(Exit_Click);
            menu.Items.Add(item);

            System.GC.Collect(3, System.GCCollectionMode.Forced);
            System.GC.WaitForFullGCComplete();

            return menu;
        }

        /// <summary>
        /// Handles the Click event of the About control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void About_Click(object sender, EventArgs e)
        {
            if (!isAboutLoaded)
            {
                isAboutLoaded = true;
                new AboutBox().ShowDialog();
                isAboutLoaded = false;
            }
        }

        /// <summary>
        /// Handles the Click event of the Add Feed control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void AddFeed_Click(object sender, EventArgs e)
        {
            new AddFeed().ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of the Exit control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Exit_Click(object sender, EventArgs e)
        {
            // Quit without further ado.
            TrayApp.ProcessIcon.ni.Visible = false;
            Application.Exit();
        }

        /// <summary>
        /// Handles the Click event of the Explorer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Explorer_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", null);
        }

        private void FeedEntry_Click(object sender, EventArgs e, string u)
        {
            try
            {
                Process.Start(u);
            }
            catch (Exception ex)
            {
                Program.ExceptionHandler(ex);
            }
        }

        /// <summary>
        /// Handles the Click event of the Notification Setting control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Notification_Setting_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowNotifications = !Properties.Settings.Default.ShowNotifications;
            Properties.Settings.Default.Save();
            TrayApp.ProcessIcon.ni.ContextMenuStrip = new ContextMenus().CreateFeedsMenu(false);
        }

        private void showToolTip(string text, string title = "")
        {
            if (title == "")
            {
                title = Program.ProductName;
            }
            ProcessIcon.ni.BalloonTipTitle = title;
            ProcessIcon.ni.BalloonTipText = text;
            ProcessIcon.ni.ShowBalloonTip(1);
        }

        /// <summary>
        /// Handles the Click event of the Startup control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Startup_Click(object sender, EventArgs e)
        {
            bool startUp = !Properties.Settings.Default.AutomaticStartup;
            Integration.AddToStartup(startUp);
            Properties.Settings.Default.AutomaticStartup = startUp;
            Properties.Settings.Default.Save();
            TrayApp.ProcessIcon.ni.ContextMenuStrip = new ContextMenus().CreateFeedsMenu(false);
        }

        private class MyXmlReader : XmlTextReader
        {
            private const string CustomUtcDateTimeFormat = "ddd MMM dd HH:mm:ss Z yyyy";
            private bool readingDate = false;
            // Wed Oct 07 08:00:07 GMT 2009

            public MyXmlReader(Stream s) : base(s)
            {
            }

            public MyXmlReader(string inputUri) : base(inputUri)
            {
            }

            public override void ReadEndElement()
            {
                if (readingDate)
                {
                    readingDate = false;
                }
                base.ReadEndElement();
            }

            public override void ReadStartElement()
            {
                if (string.Equals(base.NamespaceURI, string.Empty, StringComparison.InvariantCultureIgnoreCase) &&
                    (string.Equals(base.LocalName, "lastBuildDate", StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(base.LocalName, "pubDate", StringComparison.InvariantCultureIgnoreCase)))
                {
                    readingDate = true;
                }
                base.ReadStartElement();
            }

            public override string ReadString()
            {
                if (readingDate)
                {
                    string dateString = base.ReadString();
                    DateTime dt;
                    if (!DateTime.TryParse(dateString, out dt))
                        dt = DateTime.ParseExact(dateString, CustomUtcDateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
                    return dt.ToUniversalTime().ToString("R", System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    return base.ReadString();
                }
            }
        }
    }
}
