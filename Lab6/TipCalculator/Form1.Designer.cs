
namespace TipCalculator
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
            this.totalBillLabel = new System.Windows.Forms.Label();
            this.billTextBox = new System.Windows.Forms.TextBox();
            this.resultTextBox = new System.Windows.Forms.TextBox();
            this.tipTextLabel = new System.Windows.Forms.Label();
            this.tipTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // totalBillLabel
            // 
            this.totalBillLabel.AutoSize = true;
            this.totalBillLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalBillLabel.Location = new System.Drawing.Point(38, 28);
            this.totalBillLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.totalBillLabel.Name = "totalBillLabel";
            this.totalBillLabel.Size = new System.Drawing.Size(88, 16);
            this.totalBillLabel.TabIndex = 1;
            this.totalBillLabel.Text = "Enter Total Bill";
            // 
            // billTextBox
            // 
            this.billTextBox.BackColor = System.Drawing.Color.Silver;
            this.billTextBox.Font = new System.Drawing.Font("MV Boli", 8.25F);
            this.billTextBox.Location = new System.Drawing.Point(130, 24);
            this.billTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.billTextBox.Name = "billTextBox";
            this.billTextBox.Size = new System.Drawing.Size(145, 25);
            this.billTextBox.TabIndex = 2;
            this.billTextBox.TextChanged += new System.EventHandler(this.billTextBox_TextChanged);
            // 
            // resultTextBox
            // 
            this.resultTextBox.BackColor = System.Drawing.Color.Silver;
            this.resultTextBox.Font = new System.Drawing.Font("MV Boli", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultTextBox.Location = new System.Drawing.Point(130, 82);
            this.resultTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.resultTextBox.Name = "resultTextBox";
            this.resultTextBox.Size = new System.Drawing.Size(145, 25);
            this.resultTextBox.TabIndex = 3;
            // 
            // tipTextLabel
            // 
            this.tipTextLabel.AutoSize = true;
            this.tipTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tipTextLabel.Location = new System.Drawing.Point(67, 57);
            this.tipTextLabel.Name = "tipTextLabel";
            this.tipTextLabel.Size = new System.Drawing.Size(59, 13);
            this.tipTextLabel.TabIndex = 4;
            this.tipTextLabel.Text = "Enter Tip";
            // 
            // tipTextBox
            // 
            this.tipTextBox.BackColor = System.Drawing.Color.Silver;
            this.tipTextBox.Font = new System.Drawing.Font("MV Boli", 8.25F);
            this.tipTextBox.Location = new System.Drawing.Point(130, 52);
            this.tipTextBox.Name = "tipTextBox";
            this.tipTextBox.Size = new System.Drawing.Size(85, 25);
            this.tipTextBox.TabIndex = 5;
            this.tipTextBox.TextChanged += new System.EventHandler(this.tipTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(89, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Total";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(600, 366);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tipTextBox);
            this.Controls.Add(this.tipTextLabel);
            this.Controls.Add(this.resultTextBox);
            this.Controls.Add(this.billTextBox);
            this.Controls.Add(this.totalBillLabel);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label totalBillLabel;
        private System.Windows.Forms.TextBox billTextBox;
        private System.Windows.Forms.TextBox resultTextBox;
        private System.Windows.Forms.Label tipTextLabel;
        private System.Windows.Forms.TextBox tipTextBox;
        private System.Windows.Forms.Label label1;
    }
}

