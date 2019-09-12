namespace Monopoly.Board
{
    partial class SellProperty
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
            this.playerOneBtn = new System.Windows.Forms.Button();
            this.playerTwoBtn = new System.Windows.Forms.Button();
            this.playerThreeBtn = new System.Windows.Forms.Button();
            this.playerFourBtn = new System.Windows.Forms.Button();
            this.propertyNameTxt = new System.Windows.Forms.Label();
            this.propertyCostTxt = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // playerOneBtn
            // 
            this.playerOneBtn.Location = new System.Drawing.Point(56, 79);
            this.playerOneBtn.Name = "playerOneBtn";
            this.playerOneBtn.Size = new System.Drawing.Size(93, 41);
            this.playerOneBtn.TabIndex = 0;
            this.playerOneBtn.Text = "Player 1";
            this.playerOneBtn.UseVisualStyleBackColor = true;
            this.playerOneBtn.Click += new System.EventHandler(this.playerOneBtn_Click);
            // 
            // playerTwoBtn
            // 
            this.playerTwoBtn.Location = new System.Drawing.Point(155, 79);
            this.playerTwoBtn.Name = "playerTwoBtn";
            this.playerTwoBtn.Size = new System.Drawing.Size(93, 41);
            this.playerTwoBtn.TabIndex = 1;
            this.playerTwoBtn.Text = "Player 2";
            this.playerTwoBtn.UseVisualStyleBackColor = true;
            this.playerTwoBtn.Click += new System.EventHandler(this.playerTwoBtn_Click);
            // 
            // playerThreeBtn
            // 
            this.playerThreeBtn.Location = new System.Drawing.Point(56, 126);
            this.playerThreeBtn.Name = "playerThreeBtn";
            this.playerThreeBtn.Size = new System.Drawing.Size(93, 41);
            this.playerThreeBtn.TabIndex = 2;
            this.playerThreeBtn.Text = "Player 3";
            this.playerThreeBtn.UseVisualStyleBackColor = true;
            this.playerThreeBtn.Click += new System.EventHandler(this.playerThreeBtn_Click);
            // 
            // playerFourBtn
            // 
            this.playerFourBtn.Location = new System.Drawing.Point(155, 126);
            this.playerFourBtn.Name = "playerFourBtn";
            this.playerFourBtn.Size = new System.Drawing.Size(93, 41);
            this.playerFourBtn.TabIndex = 3;
            this.playerFourBtn.Text = "Player 4";
            this.playerFourBtn.UseVisualStyleBackColor = true;
            this.playerFourBtn.Click += new System.EventHandler(this.playerFourBtn_Click);
            // 
            // propertyNameTxt
            // 
            this.propertyNameTxt.AutoSize = true;
            this.propertyNameTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertyNameTxt.Location = new System.Drawing.Point(12, 9);
            this.propertyNameTxt.Name = "propertyNameTxt";
            this.propertyNameTxt.Size = new System.Drawing.Size(109, 20);
            this.propertyNameTxt.TabIndex = 4;
            this.propertyNameTxt.Text = "propertyName";
            // 
            // propertyCostTxt
            // 
            this.propertyCostTxt.AutoSize = true;
            this.propertyCostTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertyCostTxt.Location = new System.Drawing.Point(13, 29);
            this.propertyCostTxt.Name = "propertyCostTxt";
            this.propertyCostTxt.Size = new System.Drawing.Size(85, 16);
            this.propertyCostTxt.TabIndex = 5;
            this.propertyCostTxt.Text = "propertyCost";
            // 
            // SellProperty
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 188);
            this.Controls.Add(this.propertyCostTxt);
            this.Controls.Add(this.propertyNameTxt);
            this.Controls.Add(this.playerFourBtn);
            this.Controls.Add(this.playerThreeBtn);
            this.Controls.Add(this.playerTwoBtn);
            this.Controls.Add(this.playerOneBtn);
            this.Name = "SellProperty";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sell Property";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button playerOneBtn;
        private System.Windows.Forms.Button playerTwoBtn;
        private System.Windows.Forms.Button playerThreeBtn;
        private System.Windows.Forms.Button playerFourBtn;
        private System.Windows.Forms.Label propertyNameTxt;
        private System.Windows.Forms.Label propertyCostTxt;
    }
}