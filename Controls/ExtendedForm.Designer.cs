using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UT.Data.Controls
{
    public partial class ExtendedForm
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            infoBar1 = new InfoBar();
            SuspendLayout();
            // 
            // infoBar1
            // 
            infoBar1.BackColor = Color.LightGoldenrodYellow;
            infoBar1.Dock = DockStyle.Top;
            infoBar1.Location = new Point(0, 0);
            infoBar1.MaximumSize = new Size(int.MaxValue, 50);
            infoBar1.MinimumSize = new Size(75, 50);
            infoBar1.Name = "infoBar1";
            infoBar1.Size = new Size(931, 50);
            infoBar1.TabIndex = 0;
            // 
            // ExtendedForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(931, 498);
            Controls.Add(infoBar1);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(3, 4, 3, 4);
            Name = "ExtendedForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ExtendedForm";
            ResumeLayout(false);
        }

        private InfoBar infoBar1;
    }
}
