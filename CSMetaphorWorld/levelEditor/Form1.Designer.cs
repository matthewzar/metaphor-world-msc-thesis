namespace levelEditor
{
    partial class form_LevelEditor
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
            this.label1 = new System.Windows.Forms.Label();
            this.rtb_levelDetails = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_Previous = new System.Windows.Forms.Button();
            this.btn_Next = new System.Windows.Forms.Button();
            this.btn_Insert = new System.Windows.Forms.Button();
            this.btn_Alter = new System.Windows.Forms.Button();
            this.tb_LineNo = new System.Windows.Forms.TextBox();
            this.tb_UserInput = new System.Windows.Forms.TextBox();
            this.comBox_OpCodes = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_highlightStart = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tb_highlightLength = new System.Windows.Forms.TextBox();
            this.tbHint = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tb_ConsoleOutput = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btn_SaveLevel = new System.Windows.Forms.Button();
            this.btn_LoadLevel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rtb_Code
            // 
            this.rtb_Code.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb_Code.Location = new System.Drawing.Point(16, 41);
            this.rtb_Code.Name = "rtb_Code";
            this.rtb_Code.Size = new System.Drawing.Size(439, 515);
            this.rtb_Code.TabIndex = 0;
            this.rtb_Code.Text = "";
            this.rtb_Code.SelectionChanged += new System.EventHandler(this.rtb_Code_SelectionChanged);
            this.rtb_Code.Enter += new System.EventHandler(this.rtb_Code_Enter);
            this.rtb_Code.Leave += new System.EventHandler(this.rtb_Code_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Code";
            // 
            // rtb_levelDetails
            // 
            this.rtb_levelDetails.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb_levelDetails.Location = new System.Drawing.Point(461, 41);
            this.rtb_levelDetails.Name = "rtb_levelDetails";
            this.rtb_levelDetails.Size = new System.Drawing.Size(836, 515);
            this.rtb_levelDetails.TabIndex = 0;
            this.rtb_levelDetails.Text = "";
            this.rtb_levelDetails.WordWrap = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(458, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Level Details";
            // 
            // btn_Previous
            // 
            this.btn_Previous.Location = new System.Drawing.Point(1141, 562);
            this.btn_Previous.Name = "btn_Previous";
            this.btn_Previous.Size = new System.Drawing.Size(75, 23);
            this.btn_Previous.TabIndex = 2;
            this.btn_Previous.Text = "Previous";
            this.btn_Previous.UseVisualStyleBackColor = true;
            this.btn_Previous.Click += new System.EventHandler(this.btn_Previous_Click);
            // 
            // btn_Next
            // 
            this.btn_Next.Location = new System.Drawing.Point(1222, 562);
            this.btn_Next.Name = "btn_Next";
            this.btn_Next.Size = new System.Drawing.Size(75, 23);
            this.btn_Next.TabIndex = 3;
            this.btn_Next.Text = "Next";
            this.btn_Next.UseVisualStyleBackColor = true;
            this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
            // 
            // btn_Insert
            // 
            this.btn_Insert.Location = new System.Drawing.Point(496, 616);
            this.btn_Insert.Name = "btn_Insert";
            this.btn_Insert.Size = new System.Drawing.Size(75, 23);
            this.btn_Insert.TabIndex = 4;
            this.btn_Insert.Text = "Insert New";
            this.btn_Insert.UseVisualStyleBackColor = true;
            this.btn_Insert.Click += new System.EventHandler(this.btn_Insert_Click);
            // 
            // btn_Alter
            // 
            this.btn_Alter.Location = new System.Drawing.Point(496, 645);
            this.btn_Alter.Name = "btn_Alter";
            this.btn_Alter.Size = new System.Drawing.Size(75, 23);
            this.btn_Alter.TabIndex = 2;
            this.btn_Alter.Text = "changeCurrent";
            this.btn_Alter.UseVisualStyleBackColor = true;
            this.btn_Alter.Click += new System.EventHandler(this.btn_Alter_Click);
            // 
            // tb_LineNo
            // 
            this.tb_LineNo.Enabled = false;
            this.tb_LineNo.Location = new System.Drawing.Point(16, 592);
            this.tb_LineNo.Name = "tb_LineNo";
            this.tb_LineNo.Size = new System.Drawing.Size(62, 20);
            this.tb_LineNo.TabIndex = 5;
            // 
            // tb_UserInput
            // 
            this.tb_UserInput.Location = new System.Drawing.Point(211, 592);
            this.tb_UserInput.Name = "tb_UserInput";
            this.tb_UserInput.Size = new System.Drawing.Size(106, 20);
            this.tb_UserInput.TabIndex = 5;
            this.tb_UserInput.TextChanged += new System.EventHandler(this.tb_UserInput_TextChanged);
            // 
            // comBox_OpCodes
            // 
            this.comBox_OpCodes.FormattingEnabled = true;
            this.comBox_OpCodes.Items.AddRange(new object[] {
            "declarevariable",
            "directhandwrite",
            "assignvalue",
            "consumeBool",
            "readvariable",
            "preparemethod",
            "assignparameter",
            "callfunction",
            "return",
            "deletenewestvariable",
            "garbagecollect",
            "consolewrite"});
            this.comBox_OpCodes.Location = new System.Drawing.Point(84, 592);
            this.comBox_OpCodes.Name = "comBox_OpCodes";
            this.comBox_OpCodes.Size = new System.Drawing.Size(121, 21);
            this.comBox_OpCodes.TabIndex = 6;
            this.comBox_OpCodes.SelectedValueChanged += new System.EventHandler(this.comBox_OpCodes_SelectedValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 562);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Line no.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(81, 562);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Op. Code";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(208, 562);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Input Portion";
            // 
            // tb_highlightStart
            // 
            this.tb_highlightStart.Enabled = false;
            this.tb_highlightStart.Location = new System.Drawing.Point(323, 592);
            this.tb_highlightStart.Name = "tb_highlightStart";
            this.tb_highlightStart.Size = new System.Drawing.Size(64, 20);
            this.tb_highlightStart.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(320, 562);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 26);
            this.label6.TabIndex = 1;
            this.label6.Text = "Highlight\r\nStart";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(388, 562);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 26);
            this.label7.TabIndex = 1;
            this.label7.Text = "Highlight\r\nLength";
            // 
            // tb_highlightLength
            // 
            this.tb_highlightLength.Enabled = false;
            this.tb_highlightLength.Location = new System.Drawing.Point(391, 592);
            this.tb_highlightLength.Name = "tb_highlightLength";
            this.tb_highlightLength.Size = new System.Drawing.Size(64, 20);
            this.tb_highlightLength.TabIndex = 5;
            // 
            // tbHint
            // 
            this.tbHint.Location = new System.Drawing.Point(84, 618);
            this.tbHint.Name = "tbHint";
            this.tbHint.Size = new System.Drawing.Size(371, 20);
            this.tbHint.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 621);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Line Hint";
            // 
            // tb_ConsoleOutput
            // 
            this.tb_ConsoleOutput.Location = new System.Drawing.Point(84, 644);
            this.tb_ConsoleOutput.Name = "tb_ConsoleOutput";
            this.tb_ConsoleOutput.Size = new System.Drawing.Size(371, 20);
            this.tb_ConsoleOutput.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 647);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Console Out";
            // 
            // btn_SaveLevel
            // 
            this.btn_SaveLevel.Location = new System.Drawing.Point(1222, 644);
            this.btn_SaveLevel.Name = "btn_SaveLevel";
            this.btn_SaveLevel.Size = new System.Drawing.Size(75, 23);
            this.btn_SaveLevel.TabIndex = 3;
            this.btn_SaveLevel.Text = "Save Level";
            this.btn_SaveLevel.UseVisualStyleBackColor = true;
            this.btn_SaveLevel.Click += new System.EventHandler(this.btn_SaveLevel_Click);
            // 
            // btn_LoadLevel
            // 
            this.btn_LoadLevel.Location = new System.Drawing.Point(1141, 644);
            this.btn_LoadLevel.Name = "btn_LoadLevel";
            this.btn_LoadLevel.Size = new System.Drawing.Size(75, 23);
            this.btn_LoadLevel.TabIndex = 3;
            this.btn_LoadLevel.Text = "Load Level";
            this.btn_LoadLevel.UseVisualStyleBackColor = true;
            this.btn_LoadLevel.Click += new System.EventHandler(this.btn_LoadLevel_Click);
            // 
            // form_LevelEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1309, 672);
            this.Controls.Add(this.comBox_OpCodes);
            this.Controls.Add(this.tb_ConsoleOutput);
            this.Controls.Add(this.tbHint);
            this.Controls.Add(this.tb_highlightLength);
            this.Controls.Add(this.tb_highlightStart);
            this.Controls.Add(this.tb_UserInput);
            this.Controls.Add(this.tb_LineNo);
            this.Controls.Add(this.btn_Insert);
            this.Controls.Add(this.btn_LoadLevel);
            this.Controls.Add(this.btn_SaveLevel);
            this.Controls.Add(this.btn_Next);
            this.Controls.Add(this.btn_Alter);
            this.Controls.Add(this.btn_Previous);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rtb_levelDetails);
            this.Controls.Add(this.rtb_Code);
            this.Name = "form_LevelEditor";
            this.Text = "Level Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtb_Code;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rtb_levelDetails;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_Previous;
        private System.Windows.Forms.Button btn_Next;
        private System.Windows.Forms.Button btn_Insert;
        private System.Windows.Forms.Button btn_Alter;
        private System.Windows.Forms.TextBox tb_LineNo;
        private System.Windows.Forms.TextBox tb_UserInput;
        private System.Windows.Forms.ComboBox comBox_OpCodes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_highlightStart;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tb_highlightLength;
        private System.Windows.Forms.TextBox tbHint;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tb_ConsoleOutput;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btn_SaveLevel;
        private System.Windows.Forms.Button btn_LoadLevel;
    }
}

