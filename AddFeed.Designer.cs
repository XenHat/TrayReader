namespace TrayReader
{
    partial class AddFeed
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.addFeedButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox1.SuspendLayout();
            this.addFeedButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // addFeedButton
            // 
            this.addFeedButton.FlatAppearance.BorderSize = 0;
            this.addFeedButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.addFeedButton.Location = new System.Drawing.Point(386, -1);
            this.addFeedButton.Name = "addFeedButton";
            this.addFeedButton.Size = new System.Drawing.Size(53, 22);
            this.addFeedButton.TabIndex = 1;
            this.addFeedButton.TabStop = false;
            this.addFeedButton.Text = "Add";
            this.addFeedButton.UseVisualStyleBackColor = true;
            this.addFeedButton.Click += new System.EventHandler(this.FeedSubmitButton_Click);
            this.addFeedButton.ResumeLayout(false);
            this.addFeedButton.PerformLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(387, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.ResumeLayout(false);
            this.textBox1.PerformLayout();
            // 
            // AddFeed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 20);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.addFeedButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "AddFeed";
            this.Text = "RSS Feed URL";
            this.Load += new System.EventHandler(this.AddFeed_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button addFeedButton;
    }
}