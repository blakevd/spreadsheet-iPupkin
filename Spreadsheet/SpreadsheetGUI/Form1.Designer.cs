

using System.Windows.Forms;

namespace SpreadsheetGUI
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CurrentSelectedCellName = new System.Windows.Forms.TextBox();
            this.CurrentSelectedCellValue = new System.Windows.Forms.TextBox();
            this.InputCellContents = new System.Windows.Forms.TextBox();
            this.SelectedCellLabel = new System.Windows.Forms.Label();
            this.SelectedCellValueLabel = new System.Windows.Forms.Label();
            this.CellContents = new System.Windows.Forms.Label();
            this.RainbowButton = new System.Windows.Forms.Button();
            this.spreadsheetPanel1 = new SS.SpreadsheetPanel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(632, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // CurrentSelectedCellName
            // 
            this.CurrentSelectedCellName.Location = new System.Drawing.Point(87, 23);
            this.CurrentSelectedCellName.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.CurrentSelectedCellName.Name = "CurrentSelectedCellName";
            this.CurrentSelectedCellName.ReadOnly = true;
            this.CurrentSelectedCellName.Size = new System.Drawing.Size(68, 20);
            this.CurrentSelectedCellName.TabIndex = 1;
            // 
            // CurrentSelectedCellValue
            // 
            this.CurrentSelectedCellValue.Location = new System.Drawing.Point(189, 23);
            this.CurrentSelectedCellValue.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.CurrentSelectedCellValue.Name = "CurrentSelectedCellValue";
            this.CurrentSelectedCellValue.ReadOnly = true;
            this.CurrentSelectedCellValue.Size = new System.Drawing.Size(68, 20);
            this.CurrentSelectedCellValue.TabIndex = 2;
            // 
            // InputCellContents
            // 
            this.InputCellContents.Location = new System.Drawing.Point(283, 23);
            this.InputCellContents.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.InputCellContents.Name = "InputCellContents";
            this.InputCellContents.Size = new System.Drawing.Size(68, 20);
            this.InputCellContents.TabIndex = 3;
            this.InputCellContents.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.InputCellContents_KeyPress);
            // 
            // SelectedCellLabel
            // 
            this.SelectedCellLabel.AutoSize = true;
            this.SelectedCellLabel.Location = new System.Drawing.Point(85, 8);
            this.SelectedCellLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SelectedCellLabel.Name = "SelectedCellLabel";
            this.SelectedCellLabel.Size = new System.Drawing.Size(69, 13);
            this.SelectedCellLabel.TabIndex = 4;
            this.SelectedCellLabel.Text = "Selected Cell";
            // 
            // SelectedCellValueLabel
            // 
            this.SelectedCellValueLabel.AutoSize = true;
            this.SelectedCellValueLabel.Location = new System.Drawing.Point(195, 8);
            this.SelectedCellValueLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SelectedCellValueLabel.Name = "SelectedCellValueLabel";
            this.SelectedCellValueLabel.Size = new System.Drawing.Size(54, 13);
            this.SelectedCellValueLabel.TabIndex = 5;
            this.SelectedCellValueLabel.Text = "Cell Value";
            // 
            // CellContents
            // 
            this.CellContents.AutoSize = true;
            this.CellContents.Location = new System.Drawing.Point(280, 8);
            this.CellContents.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.CellContents.Name = "CellContents";
            this.CellContents.Size = new System.Drawing.Size(69, 13);
            this.CellContents.TabIndex = 6;
            this.CellContents.Text = "Cell Contents";
            // 
            // RainbowButton
            // 
            this.RainbowButton.Location = new System.Drawing.Point(401, 12);
            this.RainbowButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.RainbowButton.Name = "RainbowButton";
            this.RainbowButton.Size = new System.Drawing.Size(85, 27);
            this.RainbowButton.TabIndex = 7;
            this.RainbowButton.Text = "Rainbow";
            this.RainbowButton.UseVisualStyleBackColor = true;
            this.RainbowButton.Click += new System.EventHandler(this.RainbowButton_Click);
            // 
            // spreadsheetPanel1
            // 
            this.spreadsheetPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spreadsheetPanel1.Location = new System.Drawing.Point(9, 44);
            this.spreadsheetPanel1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.spreadsheetPanel1.Name = "spreadsheetPanel1";
            this.spreadsheetPanel1.Size = new System.Drawing.Size(623, 315);
            this.spreadsheetPanel1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 359);
            this.Controls.Add(this.RainbowButton);
            this.Controls.Add(this.CellContents);
            this.Controls.Add(this.SelectedCellValueLabel);
            this.Controls.Add(this.SelectedCellLabel);
            this.Controls.Add(this.InputCellContents);
            this.Controls.Add(this.CurrentSelectedCellValue);
            this.Controls.Add(this.CurrentSelectedCellName);
            this.Controls.Add(this.spreadsheetPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MinimumSize = new System.Drawing.Size(172, 176);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion



        private SS.SpreadsheetPanel spreadsheetPanel1;
        private System.Windows.Forms.TextBox CurrentSelectedCellName;
        private System.Windows.Forms.TextBox CurrentSelectedCellValue;
        private System.Windows.Forms.TextBox InputCellContents;
        private System.Windows.Forms.Label SelectedCellLabel;
        private System.Windows.Forms.Label SelectedCellValueLabel;
        private System.Windows.Forms.Label CellContents;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.Button RainbowButton;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
    }
}

