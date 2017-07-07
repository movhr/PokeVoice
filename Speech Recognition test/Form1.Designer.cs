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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.battleModeLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.gameState = new System.Windows.Forms.Label();
            this.continuePicture = new System.Windows.Forms.PictureBox();
            this.continueProbability = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.moveDirX = new System.Windows.Forms.Label();
            this.moveDirY = new System.Windows.Forms.Label();
            this.Pokemons = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.continuePicture)).BeginInit();
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
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
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
            // continuePicture
            // 
            this.continuePicture.Location = new System.Drawing.Point(409, 257);
            this.continuePicture.Name = "continuePicture";
            this.continuePicture.Size = new System.Drawing.Size(32, 35);
            this.continuePicture.TabIndex = 8;
            this.continuePicture.TabStop = false;
            // 
            // continueProbability
            // 
            this.continueProbability.AutoSize = true;
            this.continueProbability.Location = new System.Drawing.Point(409, 238);
            this.continueProbability.Name = "continueProbability";
            this.continueProbability.Size = new System.Drawing.Size(35, 13);
            this.continueProbability.TabIndex = 9;
            this.continueProbability.Text = "label1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(294, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "move list";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(294, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "pokemon list";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(297, 238);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "walking direction";
            // 
            // moveDirX
            // 
            this.moveDirX.AutoSize = true;
            this.moveDirX.Location = new System.Drawing.Point(297, 251);
            this.moveDirX.Name = "moveDirX";
            this.moveDirX.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.moveDirX.Size = new System.Drawing.Size(35, 13);
            this.moveDirX.TabIndex = 14;
            this.moveDirX.Text = "label1";
            this.moveDirX.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // moveDirY
            // 
            this.moveDirY.AutoSize = true;
            this.moveDirY.Location = new System.Drawing.Point(297, 264);
            this.moveDirY.Name = "moveDirY";
            this.moveDirY.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.moveDirY.Size = new System.Drawing.Size(35, 13);
            this.moveDirY.TabIndex = 13;
            this.moveDirY.Text = "label1";
            this.moveDirY.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // Pokemons
            // 
            this.Pokemons.FormattingEnabled = true;
            this.Pokemons.Location = new System.Drawing.Point(294, 157);
            this.Pokemons.Name = "Pokemons";
            this.Pokemons.Size = new System.Drawing.Size(147, 82);
            this.Pokemons.TabIndex = 15;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 320);
            this.Controls.Add(this.Pokemons);
            this.Controls.Add(this.moveDirX);
            this.Controls.Add(this.moveDirY);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.continueProbability);
            this.Controls.Add(this.continuePicture);
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
            ((System.ComponentModel.ISupportInitialize)(this.continuePicture)).EndInit();
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
        public System.Windows.Forms.PictureBox continuePicture;
        public System.Windows.Forms.Label continueProbability;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label moveDirX;
        public System.Windows.Forms.Label moveDirY;
        public System.Windows.Forms.ListBox Pokemons;
    }
}

