namespace Speech_Recognition_test
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.battleModeLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.gameState = new System.Windows.Forms.Label();
            this.moveDirY = new System.Windows.Forms.Label();
            this.moveDirX = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(284, 320);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(447, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(321, 292);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // battleModeLabel
            // 
            this.battleModeLabel.AutoSize = true;
            this.battleModeLabel.Location = new System.Drawing.Point(291, 13);
            this.battleModeLabel.Name = "battleModeLabel";
            this.battleModeLabel.Size = new System.Drawing.Size(64, 13);
            this.battleModeLabel.TabIndex = 2;
            this.battleModeLabel.Text = "Not in battle";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(291, 26);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(64, 13);
            this.statusLabel.TabIndex = 3;
            this.statusLabel.Text = "Not in battle";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(294, 78);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(147, 56);
            this.listBox1.TabIndex = 4;
            // 
            // gameState
            // 
            this.gameState.AutoSize = true;
            this.gameState.Location = new System.Drawing.Point(406, 13);
            this.gameState.Name = "gameState";
            this.gameState.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.gameState.Size = new System.Drawing.Size(35, 13);
            this.gameState.TabIndex = 5;
            this.gameState.Text = "label1";
            this.gameState.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // moveDirY
            // 
            this.moveDirY.AutoSize = true;
            this.moveDirY.Location = new System.Drawing.Point(406, 53);
            this.moveDirY.Name = "moveDirY";
            this.moveDirY.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.moveDirY.Size = new System.Drawing.Size(35, 13);
            this.moveDirY.TabIndex = 6;
            this.moveDirY.Text = "label1";
            this.moveDirY.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // moveDirX
            // 
            this.moveDirX.AutoSize = true;
            this.moveDirX.Location = new System.Drawing.Point(365, 53);
            this.moveDirX.Name = "moveDirX";
            this.moveDirX.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.moveDirX.Size = new System.Drawing.Size(35, 13);
            this.moveDirX.TabIndex = 7;
            this.moveDirX.Text = "label1";
            this.moveDirX.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 320);
            this.Controls.Add(this.moveDirX);
            this.Controls.Add(this.moveDirY);
            this.Controls.Add(this.gameState);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.battleModeLabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.richTextBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.Label battleModeLabel;
        public System.Windows.Forms.Label statusLabel;
        public System.Windows.Forms.ListBox listBox1;
        public System.Windows.Forms.Label gameState;
        public System.Windows.Forms.Label moveDirY;
        public System.Windows.Forms.Label moveDirX;
    }
}

