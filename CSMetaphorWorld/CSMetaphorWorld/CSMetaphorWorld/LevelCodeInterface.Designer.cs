namespace CSMetaphorWorld
{
    partial class LevelCodeInterface
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
            this.rtb_Code = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rtb_Outputs = new System.Windows.Forms.RichTextBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.rb_consoleOut = new System.Windows.Forms.RadioButton();
            this.rb_hintsOut = new System.Windows.Forms.RadioButton();
            this.rb_OpsOut = new System.Windows.Forms.RadioButton();
            this.rb_LevelDescription = new System.Windows.Forms.RadioButton();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtb_Code
            // 
            this.rtb_Code.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb_Code.Location = new System.Drawing.Point(12, 71);
            this.rtb_Code.Name = "rtb_Code";
            this.rtb_Code.ReadOnly = true;
            this.rtb_Code.Size = new System.Drawing.Size(521, 518);
            this.rtb_Code.TabIndex = 0;
            this.rtb_Code.Text = "";
            this.rtb_Code.VScroll += new System.EventHandler(this.rtb_Code_VScroll);
            this.rtb_Code.TextChanged += new System.EventHandler(this.rtb_Code_TextChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(545, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openLevelToolStripMenuItem,
            this.saveLevelToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openLevelToolStripMenuItem
            // 
            this.openLevelToolStripMenuItem.Name = "openLevelToolStripMenuItem";
            this.openLevelToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.openLevelToolStripMenuItem.Text = "Open Level";
            this.openLevelToolStripMenuItem.Click += new System.EventHandler(this.openLevelToolStripMenuItem_Click);
            // 
            // saveLevelToolStripMenuItem
            // 
            this.saveLevelToolStripMenuItem.Name = "saveLevelToolStripMenuItem";
            this.saveLevelToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.saveLevelToolStripMenuItem.Text = "Save Level";
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            // 
            // rtb_Outputs
            // 
            this.rtb_Outputs.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.rtb_Outputs.Location = new System.Drawing.Point(12, 663);
            this.rtb_Outputs.Name = "rtb_Outputs";
            this.rtb_Outputs.Size = new System.Drawing.Size(521, 227);
            this.rtb_Outputs.TabIndex = 2;
            this.rtb_Outputs.Text = "";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
            this.linkLabel1.Location = new System.Drawing.Point(13, 28);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(70, 26);
            this.linkLabel1.TabIndex = 3;
            this.linkLabel1.Text = "Code:";
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.linkLabel2.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
            this.linkLabel2.Location = new System.Drawing.Point(12, 631);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(167, 26);
            this.linkLabel2.TabIndex = 3;
            this.linkLabel2.Text = "Output Window:";
            // 
            // rb_consoleOut
            // 
            this.rb_consoleOut.AutoSize = true;
            this.rb_consoleOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.rb_consoleOut.Location = new System.Drawing.Point(213, 633);
            this.rb_consoleOut.Name = "rb_consoleOut";
            this.rb_consoleOut.Size = new System.Drawing.Size(115, 24);
            this.rb_consoleOut.TabIndex = 4;
            this.rb_consoleOut.Text = "Console Out";
            this.rb_consoleOut.UseVisualStyleBackColor = true;
            this.rb_consoleOut.CheckedChanged += new System.EventHandler(this.rb_consoleOut_checkChange);
            // 
            // rb_hintsOut
            // 
            this.rb_hintsOut.AutoSize = true;
            this.rb_hintsOut.Checked = true;
            this.rb_hintsOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rb_hintsOut.Location = new System.Drawing.Point(213, 603);
            this.rb_hintsOut.Name = "rb_hintsOut";
            this.rb_hintsOut.Size = new System.Drawing.Size(64, 24);
            this.rb_hintsOut.TabIndex = 4;
            this.rb_hintsOut.TabStop = true;
            this.rb_hintsOut.Text = "Hints";
            this.rb_hintsOut.UseVisualStyleBackColor = true;
            this.rb_hintsOut.CheckedChanged += new System.EventHandler(this.rb_hintsOut_checkChange);
            // 
            // rb_OpsOut
            // 
            this.rb_OpsOut.AutoSize = true;
            this.rb_OpsOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.rb_OpsOut.Location = new System.Drawing.Point(357, 633);
            this.rb_OpsOut.Name = "rb_OpsOut";
            this.rb_OpsOut.Size = new System.Drawing.Size(176, 24);
            this.rb_OpsOut.TabIndex = 4;
            this.rb_OpsOut.Text = "Ops To WorldTracker";
            this.rb_OpsOut.UseVisualStyleBackColor = true;
            this.rb_OpsOut.CheckedChanged += new System.EventHandler(this.rb_OpsOut_checkChange);
            // 
            // rb_LevelDescription
            // 
            this.rb_LevelDescription.AutoSize = true;
            this.rb_LevelDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rb_LevelDescription.Location = new System.Drawing.Point(357, 603);
            this.rb_LevelDescription.Name = "rb_LevelDescription";
            this.rb_LevelDescription.Size = new System.Drawing.Size(148, 24);
            this.rb_LevelDescription.TabIndex = 5;
            this.rb_LevelDescription.Text = "Level Description";
            this.rb_LevelDescription.UseVisualStyleBackColor = true;
            this.rb_LevelDescription.CheckedChanged += new System.EventHandler(this.rb_description_checkChange);
            // 
            // interactionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 902);
            this.Controls.Add(this.rb_LevelDescription);
            this.Controls.Add(this.rb_OpsOut);
            this.Controls.Add(this.rb_hintsOut);
            this.Controls.Add(this.rb_consoleOut);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.rtb_Outputs);
            this.Controls.Add(this.rtb_Code);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximumSize = new System.Drawing.Size(561, 940);
            this.MinimumSize = new System.Drawing.Size(561, 940);
            this.Name = "interactionForm";
            this.Text = "interactionForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.interactionForm_FormClosing);
            this.Load += new System.EventHandler(this.interactionForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtb_Code;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem openLevelToolStripMenuItem; //made this public so that external classes can add extra handlers for when a level is opened
        private System.Windows.Forms.ToolStripMenuItem saveLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.RichTextBox rtb_Outputs;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.RadioButton rb_consoleOut;
        private System.Windows.Forms.RadioButton rb_hintsOut;
        private System.Windows.Forms.RadioButton rb_OpsOut;
        private System.Windows.Forms.RadioButton rb_LevelDescription;
    }
}