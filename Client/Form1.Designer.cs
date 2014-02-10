namespace Client
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
        protected override void Dispose (bool disposing)
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
        private void InitializeComponent ()
        {
            this.UserList = new System.Windows.Forms.ListBox();
            this.ChatBox = new System.Windows.Forms.RichTextBox();
            this.Tile0 = new System.Windows.Forms.Button();
            this.Tile3 = new System.Windows.Forms.Button();
            this.Tile6 = new System.Windows.Forms.Button();
            this.Tile1 = new System.Windows.Forms.Button();
            this.Tile4 = new System.Windows.Forms.Button();
            this.Tile7 = new System.Windows.Forms.Button();
            this.Tile2 = new System.Windows.Forms.Button();
            this.Tile5 = new System.Windows.Forms.Button();
            this.Tile8 = new System.Windows.Forms.Button();
            this.ChatInput = new System.Windows.Forms.TextBox();
            this.SendButton = new System.Windows.Forms.Button();
            this.ChallengeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // UserList
            // 
            this.UserList.FormattingEnabled = true;
            this.UserList.Location = new System.Drawing.Point(330, 12);
            this.UserList.Name = "UserList";
            this.UserList.Size = new System.Drawing.Size(141, 290);
            this.UserList.TabIndex = 0;
            // 
            // ChatBox
            // 
            this.ChatBox.Location = new System.Drawing.Point(12, 334);
            this.ChatBox.Name = "ChatBox";
            this.ChatBox.ReadOnly = true;
            this.ChatBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.ChatBox.Size = new System.Drawing.Size(459, 96);
            this.ChatBox.TabIndex = 1;
            this.ChatBox.Text = "";
            // 
            // Tile0
            // 
            this.Tile0.Enabled = false;
            this.Tile0.Font = new System.Drawing.Font("Arial Black", 50F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tile0.Location = new System.Drawing.Point(12, 12);
            this.Tile0.Name = "Tile0";
            this.Tile0.Size = new System.Drawing.Size(100, 100);
            this.Tile0.TabIndex = 0;
            this.Tile0.UseVisualStyleBackColor = true;
            this.Tile0.Click += new System.EventHandler(this.Tile_Click);
            // 
            // Tile3
            // 
            this.Tile3.Enabled = false;
            this.Tile3.Font = new System.Drawing.Font("Arial Black", 50F);
            this.Tile3.Location = new System.Drawing.Point(12, 118);
            this.Tile3.Name = "Tile3";
            this.Tile3.Size = new System.Drawing.Size(100, 100);
            this.Tile3.TabIndex = 1;
            this.Tile3.UseVisualStyleBackColor = true;
            this.Tile3.Click += new System.EventHandler(this.Tile_Click);
            // 
            // Tile6
            // 
            this.Tile6.Enabled = false;
            this.Tile6.Font = new System.Drawing.Font("Arial Black", 50F);
            this.Tile6.Location = new System.Drawing.Point(12, 224);
            this.Tile6.Name = "Tile6";
            this.Tile6.Size = new System.Drawing.Size(100, 100);
            this.Tile6.TabIndex = 2;
            this.Tile6.UseVisualStyleBackColor = true;
            this.Tile6.Click += new System.EventHandler(this.Tile_Click);
            // 
            // Tile1
            // 
            this.Tile1.Enabled = false;
            this.Tile1.Font = new System.Drawing.Font("Arial Black", 50F);
            this.Tile1.Location = new System.Drawing.Point(118, 12);
            this.Tile1.Name = "Tile1";
            this.Tile1.Size = new System.Drawing.Size(100, 100);
            this.Tile1.TabIndex = 3;
            this.Tile1.UseVisualStyleBackColor = true;
            this.Tile1.Click += new System.EventHandler(this.Tile_Click);
            // 
            // Tile4
            // 
            this.Tile4.Enabled = false;
            this.Tile4.Font = new System.Drawing.Font("Arial Black", 50F);
            this.Tile4.Location = new System.Drawing.Point(118, 118);
            this.Tile4.Name = "Tile4";
            this.Tile4.Size = new System.Drawing.Size(100, 100);
            this.Tile4.TabIndex = 4;
            this.Tile4.UseVisualStyleBackColor = true;
            this.Tile4.Click += new System.EventHandler(this.Tile_Click);
            // 
            // Tile7
            // 
            this.Tile7.Enabled = false;
            this.Tile7.Font = new System.Drawing.Font("Arial Black", 50F);
            this.Tile7.Location = new System.Drawing.Point(118, 224);
            this.Tile7.Name = "Tile7";
            this.Tile7.Size = new System.Drawing.Size(100, 100);
            this.Tile7.TabIndex = 5;
            this.Tile7.UseVisualStyleBackColor = true;
            this.Tile7.Click += new System.EventHandler(this.Tile_Click);
            // 
            // Tile2
            // 
            this.Tile2.Enabled = false;
            this.Tile2.Font = new System.Drawing.Font("Arial Black", 50F);
            this.Tile2.Location = new System.Drawing.Point(224, 12);
            this.Tile2.Name = "Tile2";
            this.Tile2.Size = new System.Drawing.Size(100, 100);
            this.Tile2.TabIndex = 6;
            this.Tile2.UseVisualStyleBackColor = true;
            this.Tile2.Click += new System.EventHandler(this.Tile_Click);
            // 
            // Tile5
            // 
            this.Tile5.Enabled = false;
            this.Tile5.Font = new System.Drawing.Font("Arial Black", 50F);
            this.Tile5.Location = new System.Drawing.Point(224, 118);
            this.Tile5.Name = "Tile5";
            this.Tile5.Size = new System.Drawing.Size(100, 100);
            this.Tile5.TabIndex = 7;
            this.Tile5.UseVisualStyleBackColor = true;
            this.Tile5.Click += new System.EventHandler(this.Tile_Click);
            // 
            // Tile8
            // 
            this.Tile8.Enabled = false;
            this.Tile8.Font = new System.Drawing.Font("Arial Black", 50F);
            this.Tile8.Location = new System.Drawing.Point(224, 224);
            this.Tile8.Name = "Tile8";
            this.Tile8.Size = new System.Drawing.Size(100, 100);
            this.Tile8.TabIndex = 8;
            this.Tile8.UseVisualStyleBackColor = true;
            this.Tile8.Click += new System.EventHandler(this.Tile_Click);
            // 
            // ChatInput
            // 
            this.ChatInput.Location = new System.Drawing.Point(12, 436);
            this.ChatInput.Multiline = true;
            this.ChatInput.Name = "ChatInput";
            this.ChatInput.Size = new System.Drawing.Size(401, 20);
            this.ChatInput.TabIndex = 9;
            this.ChatInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ChatInput_KeyUp);
            // 
            // SendButton
            // 
            this.SendButton.Location = new System.Drawing.Point(419, 436);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(52, 23);
            this.SendButton.TabIndex = 10;
            this.SendButton.Text = "Send";
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // ChallengeButton
            // 
            this.ChallengeButton.Location = new System.Drawing.Point(330, 305);
            this.ChallengeButton.Name = "ChallengeButton";
            this.ChallengeButton.Size = new System.Drawing.Size(141, 23);
            this.ChallengeButton.TabIndex = 11;
            this.ChallengeButton.Text = "Challenge selected user";
            this.ChallengeButton.UseVisualStyleBackColor = true;
            this.ChallengeButton.Click += new System.EventHandler(this.ChallengeButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 468);
            this.Controls.Add(this.ChallengeButton);
            this.Controls.Add(this.SendButton);
            this.Controls.Add(this.ChatInput);
            this.Controls.Add(this.Tile8);
            this.Controls.Add(this.Tile5);
            this.Controls.Add(this.ChatBox);
            this.Controls.Add(this.Tile2);
            this.Controls.Add(this.UserList);
            this.Controls.Add(this.Tile7);
            this.Controls.Add(this.Tile0);
            this.Controls.Add(this.Tile4);
            this.Controls.Add(this.Tile3);
            this.Controls.Add(this.Tile1);
            this.Controls.Add(this.Tile6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Tacs Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox UserList;
        private System.Windows.Forms.RichTextBox ChatBox;
        private System.Windows.Forms.Button Tile0;
        private System.Windows.Forms.Button Tile3;
        private System.Windows.Forms.Button Tile6;
        private System.Windows.Forms.Button Tile1;
        private System.Windows.Forms.Button Tile4;
        private System.Windows.Forms.Button Tile7;
        private System.Windows.Forms.Button Tile2;
        private System.Windows.Forms.Button Tile5;
        private System.Windows.Forms.Button Tile8;
        private System.Windows.Forms.TextBox ChatInput;
        private System.Windows.Forms.Button SendButton;
        private System.Windows.Forms.Button ChallengeButton;
    }
}

