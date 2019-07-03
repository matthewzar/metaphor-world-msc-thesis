using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace levelEditor
{
    public partial class form_LevelEditor : Form
    {
        CSMetaphorWorld.Level newLevel = new CSMetaphorWorld.Level("noName", "testDecription", "", new string[0], new List<int>(), new List<string>(), new List<string>());

        public form_LevelEditor()
        {
            InitializeComponent();
        }

        int selectStep = 0;
        private void rtb_Code_SelectionChanged(object sender, EventArgs e)
        {
            //get the char index of the selection starting from 0 at at the very top every time....
            //tb_highlightStart.Text = rtb_Code.SelectionStart.ToString();

            int highLen = rtb_Code.SelectionLength;
            int highStart = rtb_Code.SelectionStart;

            //get the length of the highlighted area...no problem so far

            tb_highlightLength.Text = rtb_Code.SelectionLength.ToString();

            //get the starting line number (from 1) of the selection
            tb_LineNo.Text = (rtb_Code.GetLineFromCharIndex(rtb_Code.GetFirstCharIndexOfCurrentLine()) + 1) + "";

            //Get the offset from the start of THE CURRENT LINE
            tb_highlightStart.Text = (rtb_Code.SelectionStart - rtb_Code.GetFirstCharIndexOfCurrentLine()).ToString();
                

        }

        int currentIndex = 0;

        private void btn_Previous_Click(object sender, EventArgs e)
        {
            if(currentIndex > 0)
                currentIndex--;
            selectCurrentText();

            //Random myRandom = new Random();
            //rtb_Code.SelectionBackColor = Color.FromArgb(myRandom.Next(255),myRandom.Next(255),myRandom.Next(255));
            //rtb_Code.Select(myRandom.Next(rtb_Code.Text.Length), myRandom.Next(10));
        }


        private void btn_Next_Click(object sender, EventArgs e)
        {
            if(currentIndex < newLevel.lineOrder.Count)
                currentIndex++;
            selectCurrentText();
        }



        private void selectCurrentText()
        {
            rtb_Code.Select(0, rtb_Code.Text.Length);
            rtb_Code.SelectionBackColor = Color.White;
            if (currentIndex >= newLevel.lineOrder.Count || currentIndex < 0)
                return;
            else
            {
                //highlight those things that correspond to the current line
                int lineToSelect = newLevel.lineOrder[currentIndex] - 1;
                int highStart = newLevel.highlightStarts[currentIndex];
                int highLength = newLevel.highlightLengths[currentIndex];

                rtb_Code.Select(rtb_Code.GetFirstCharIndexFromLine(lineToSelect) + highStart, highLength);
                rtb_Code.SelectionBackColor = Color.Yellow;
            }

            //now something similar for the code that represents the complete level state
            rtb_levelDetails.Select(0, rtb_levelDetails.Text.Length);
            rtb_levelDetails.SelectionBackColor = Color.White;

            rtb_levelDetails.Select(rtb_levelDetails.GetFirstCharIndexFromLine(currentIndex), 4);
            rtb_levelDetails.SelectionBackColor = Color.Yellow;

        }

        private void comBox_OpCodes_SelectedValueChanged(object sender, EventArgs e)
        {
            switch (comBox_OpCodes.Text.ToLower())
            {
                case ("directhandwrite"):
                    tb_UserInput.Text = "<int 2>";
                    return;
                case ("declarevariable"):
                    tb_UserInput.Text = "<int name>";
                    return;
                case ("assignvalue"):
                    tb_UserInput.Text = "<varName>";
                    return;
                case ("readvariable"):
                    tb_UserInput.Text = "<varName>";
                    return;
                case ("preparemethod"):
                    tb_UserInput.Text = "<methSig(int n)>";
                    return;
                case ("assignparameter"):
                    tb_UserInput.Text = "<paramName>";
                    return;
                default:
                    tb_UserInput.Text = "";
                    return;
            }
        }

        private void tb_UserInput_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_Insert_Click(object sender, EventArgs e)
        {
            currentIndex++;
            string opCode = comBox_OpCodes.Text;
            if (tb_UserInput.Text != "")
                opCode += " " + tb_UserInput.Text;

            if(currentIndex < newLevel.lineOrder.Count)
                newLevel.insertNewOperation(currentIndex, Convert.ToInt32(tb_LineNo.Text), Convert.ToInt32(tb_highlightStart.Text), Convert.ToInt32(tb_highlightLength.Text),
                                    opCode, tbHint.Text, tb_ConsoleOutput.Text);
            else                
                newLevel.addNewOperation(Convert.ToInt32(tb_LineNo.Text), Convert.ToInt32(tb_highlightStart.Text), Convert.ToInt32(tb_highlightLength.Text),
                                     opCode, tbHint.Text, tb_ConsoleOutput.Text);
            
            
            rtb_levelDetails.Text = newLevel.ToString();
        }


        private void btn_Alter_Click(object sender, EventArgs e)
        {
            newLevel.lineOrder[currentIndex]                 = Convert.ToInt32(tb_LineNo.Text);

            newLevel.completeLevelInstructions[currentIndex] = comBox_OpCodes.Text;
            if(tb_UserInput.Text != "")
                newLevel.completeLevelInstructions[currentIndex] += " " + tb_UserInput.Text;

            newLevel.highlightStarts[currentIndex]           = Convert.ToInt32(tb_highlightStart.Text);
            newLevel.highlightLengths[currentIndex]          = Convert.ToInt32(tb_highlightLength.Text);
            newLevel.hintForEachInstruction[currentIndex]    = tbHint.Text;
            newLevel.consoleOutputs[currentIndex]            = tb_ConsoleOutput.Text;

            rtb_levelDetails.Text = newLevel.ToString();
        }

        private void btn_SaveLevel_Click(object sender, EventArgs e)
        {
            newLevel.code = rtb_Code.Text;
            newLevel.serialize("newLevel.xml");
            //newLevel = new CSMetaphorWorld.Level("noName", "testDecription", "", new string[0], new List<int>(), new List<string>(), new List<string>());
        }

        private void rtb_Code_Enter(object sender, EventArgs e)
        {
            rtb_Code.SelectionBackColor = Color.White;
        }

        private void rtb_Code_Leave(object sender, EventArgs e)
        {
            rtb_Code.SelectionBackColor = Color.Yellow;
        }

        private void btn_LoadLevel_Click(object sender, EventArgs e)
        {
            String input = string.Empty;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Level files (*.xml)|*.xml";
            
            //the path to the level editor
            string appPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            //Set the starting directory and the title.
            dialog.InitialDirectory = appPath; 
            dialog.Title = "Select a level file";


            if (dialog.ShowDialog() == DialogResult.OK)
                input = dialog.FileName;
            if (input == String.Empty)
                return;//user didn't select a file
            
            newLevel = CSMetaphorWorld.Level.loadLevelFromFile(input);

            //display the details of the current level
            rtb_Code.Text = newLevel.code;
            rtb_levelDetails.Text = newLevel.ToString();
            currentIndex = newLevel.lineOrder.Count;
        }

    }
}
