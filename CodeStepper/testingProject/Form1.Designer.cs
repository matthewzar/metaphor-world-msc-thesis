namespace testingProject
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
            this.button1 = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.bt_forward = new System.Windows.Forms.Button();
            this.bt_back = new System.Windows.Forms.Button();
            this.rtb_VariableVals = new System.Windows.Forms.RichTextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.tb_fileName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 43);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Load";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(158, 12);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(380, 432);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // bt_forward
            // 
            this.bt_forward.Location = new System.Drawing.Point(12, 99);
            this.bt_forward.Name = "bt_forward";
            this.bt_forward.Size = new System.Drawing.Size(100, 23);
            this.bt_forward.TabIndex = 3;
            this.bt_forward.Text = "Step Forward";
            this.bt_forward.UseVisualStyleBackColor = true;
            this.bt_forward.Click += new System.EventHandler(this.bt_forward_Click);
            // 
            // bt_back
            // 
            this.bt_back.Location = new System.Drawing.Point(12, 128);
            this.bt_back.Name = "bt_back";
            this.bt_back.Size = new System.Drawing.Size(100, 23);
            this.bt_back.TabIndex = 3;
            this.bt_back.Text = "Step Back";
            this.bt_back.UseVisualStyleBackColor = true;
            this.bt_back.Click += new System.EventHandler(this.bt_back_Click);
            // 
            // rtb_VariableVals
            // 
            this.rtb_VariableVals.Location = new System.Drawing.Point(544, 12);
            this.rtb_VariableVals.Name = "rtb_VariableVals";
            this.rtb_VariableVals.Size = new System.Drawing.Size(367, 432);
            this.rtb_VariableVals.TabIndex = 2;
            this.rtb_VariableVals.Text = "";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(23, 399);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tb_fileName
            // 
            this.tb_fileName.Location = new System.Drawing.Point(15, 17);
            this.tb_fileName.Name = "tb_fileName";
            this.tb_fileName.Size = new System.Drawing.Size(100, 20);
            this.tb_fileName.TabIndex = 5;
            this.tb_fileName.Text = "primeCheck";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(923, 516);
            this.Controls.Add(this.tb_fileName);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.bt_back);
            this.Controls.Add(this.bt_forward);
            this.Controls.Add(this.rtb_VariableVals);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button bt_forward;
        private System.Windows.Forms.Button bt_back;
        private System.Windows.Forms.RichTextBox rtb_VariableVals;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox tb_fileName;
    }
}

