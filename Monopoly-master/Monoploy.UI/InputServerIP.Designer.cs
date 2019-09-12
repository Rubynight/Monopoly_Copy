namespace Monoploy.UI
{
    partial class InputServerIP
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
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.ipAddressTxt = new System.Windows.Forms.TextBox();
            this.connectBtn = new System.Windows.Forms.Button();
            this.hostButton = new System.Windows.Forms.Button();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.colorButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(12, 63);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(0, 13);
            this.linkLabel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enter Server IP:";
            // 
            // ipAddressTxt
            // 
            this.ipAddressTxt.Location = new System.Drawing.Point(107, 10);
            this.ipAddressTxt.Name = "ipAddressTxt";
            this.ipAddressTxt.Size = new System.Drawing.Size(128, 20);
            this.ipAddressTxt.TabIndex = 2;
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(160, 85);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(75, 23);
            this.connectBtn.TabIndex = 3;
            this.connectBtn.Text = "Connect";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // hostButton
            // 
            this.hostButton.Location = new System.Drawing.Point(79, 85);
            this.hostButton.Name = "hostButton";
            this.hostButton.Size = new System.Drawing.Size(75, 23);
            this.hostButton.TabIndex = 4;
            this.hostButton.Text = "Host";
            this.hostButton.UseVisualStyleBackColor = true;
            this.hostButton.Click += new System.EventHandler(this.hostButton_Click);
            // 
            // colorButton
            // 
            this.colorButton.Location = new System.Drawing.Point(107, 36);
            this.colorButton.Name = "colorButton";
            this.colorButton.Size = new System.Drawing.Size(128, 23);
            this.colorButton.TabIndex = 5;
            this.colorButton.Text = "Choose Color";
            this.colorButton.UseVisualStyleBackColor = true;
            this.colorButton.Click += new System.EventHandler(this.colorButton_Click);
            // 
            // InputServerIP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(254, 120);
            this.Controls.Add(this.colorButton);
            this.Controls.Add(this.hostButton);
            this.Controls.Add(this.connectBtn);
            this.Controls.Add(this.ipAddressTxt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.linkLabel1);
            this.Name = "InputServerIP";
            this.Text = "Input Server IP";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ipAddressTxt;
        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.Button hostButton;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button colorButton;
    }
}