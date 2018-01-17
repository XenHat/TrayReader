// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Drawing;
using System.Windows.Forms;
using TrayApp.Properties;

namespace TrayApp
{
    public partial class AddFeed : Form
    {
        private bool can_add;
        private string currentURLInput;

        public AddFeed()
        {
            InitializeComponent();
        }

        private void AddFeed_Load(object sender, EventArgs e)
        {
        }

        private void FeedSubmitButton_Click(object sender, EventArgs e)
        {
            // Get value of field
            if (!can_add)
            {
                return;
            }
            try
            {
                var potential_url = this.currentURLInput;
                if (Helper.ValidateInput(potential_url))
                {
                    Settings.Default.SettingFeedList.Add(potential_url.TrimEnd('/'));
                    Settings.Default.Save();
                    TrayApp.ProcessIcon.ni.ContextMenuStrip = new ContextMenus().CreateFeedsMenu();
                    this.Close();
                }
            }
            catch (Exception exception)
            {
                Program.ExceptionHandler(exception);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var text = textBox1.Text;
            if (!text.Contains("http"))
            {
                text = "http://" + text;
            }
            can_add = Helper.ValidateInput(text);
            if (can_add)
            {
                textBox1.BackColor = Color.Empty;
                can_add = true;
            }
            else
            {
                textBox1.BackColor = Color.Red;
                can_add = false;
            }
            currentURLInput = text;
        }
    }
}
