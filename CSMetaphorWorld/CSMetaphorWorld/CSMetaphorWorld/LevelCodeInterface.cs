using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CSMetaphorWorld
{
    public partial class LevelCodeInterface : Form
    {
        /// <summary>
        /// This property is set to null when there is no level to load, and when it has any other value we should load a new level based on the directory it stores
        /// </summary>
        public string levelToOpen = null;

        private Level levelState = null;

        public LevelCodeInterface()
        {    
            InitializeComponent();
        }

        public void changeBasedOnLevelState(Level level)
        {
            //display the current level code
            levelState = level;
            displayAndHighlightCode(level);

            if (rb_LevelDescription.Checked)
                displayLevelDescription(level);
            if (rb_consoleOut.Checked)
                displayConsoleOutput(level);
            if (rb_hintsOut.Checked)
                displayHints(level);
            if (rb_OpsOut.Checked)
                displayOpCodes(level);



            //for (int i = 0; i < rtb_Outputs.Lines.Length+1; i++)
            //{
            //    if (rtb_Outputs.Lines.Length < level.operationsSoFar.Count)
            //        rtb_Outputs.AppendText(level.operationsSoFar[i]+"\n");
            //}
        }

        private void displayLevelDescription(Level level)
        {
            rtb_Outputs.Text = level.description;
        }

        private string addLineNumbers(string code)
        {
            if (code.Length == 0)
                return code;
            code = " 1. " + code;
            int currentLine = 2;
            for (int i = 0; i < code.Length; i++)
            {
                if (code[i] == '\n')
                {
                    if (currentLine < 10)
                    {
                        code = code.Insert(i + 1, " " + currentLine + ". ");
                    }
                    else
                    {
                        code = code.Insert(i + 1, currentLine + ". ");
                    }
                    i+=4;

                    currentLine++;
                }
            }

            //put in some extra spaces so that we have a nice continuous line of spaces until the end of the rtb_code thing
            for (int i = 0; i < 28 - currentLine; i++)
                code += "\n    ";

            return code;
        }

        private void displayAndHighlightCode(Level level)
        {
            if (level == null)
                return; //theres no point attempting to display the details of a level that isn't initialised (this happens espesially when using sandbox levels)

            try
            {
                rtb_Code.Text = addLineNumbers(level.code);

                int lineToSelect = level.getCurrentLineNumber() - 1; //minus one becuase the level code indexes start at 1, while the text box line indexes start at 0

                //check if there was no line to select (caused by no level being loaded yet)
                if (lineToSelect == -2)
                    return;

                int highStart = level.getCurrentHighlightStart();
                int highLength = level.getCurrentHighlightLength();

                //whatever the previous selection was make it white now
                rtb_Code.Select(0, rtb_Code.Text.Length);
                rtb_Code.SelectionBackColor = Color.White;
                rtb_Code.SelectionColor = Color.Black;



                //now highlight all the code number sections
                int currentLength = 0;
                foreach (var line in rtb_Code.Lines)
                {
                    rtb_Code.Select(currentLength, 3);
                    rtb_Code.SelectionBackColor = Color.DarkGray;
                    rtb_Code.SelectionColor = Color.White;

                    rtb_Code.Select(currentLength + 3, 1);
                    rtb_Code.SelectionBackColor = Color.LightGray;

                    currentLength += line.Length + 1;
                }


                //now change the selection to what we want done next (+4 becuase we have 4 charachters worth of line numbering
                rtb_Code.Select(rtb_Code.GetFirstCharIndexFromLine(lineToSelect) + highStart + 4, highLength);

                //now make the new selection yellow:
                rtb_Code.SelectionColor = Color.Black;
                rtb_Code.SelectionBackColor = Color.Yellow;
            }
            catch (ObjectDisposedException)
            {
                //Dont't do anything if the interaction form has been disposed of, we are going to close the program if it is after all
            }
            catch (Exception unexpectedError)
            {
                Console.WriteLine("Error occured in interaction form in displayAndHighlightCode {0}", unexpectedError.Message); 
            }
            
            //int lineCounter = 0;
            //foreach (string line in rtb_Code.Lines)
            //{
            //    //add conditional statement if not selecting all the lines
            //    rtb_Code.Select(rtb_Code.GetFirstCharIndexFromLine(lineCounter)+2, line.Length-4);

            //    if (lineCounter == lineToSelect)
            //        rtb_Code.SelectionBackColor = Color.Yellow;
            //    else
            //        rtb_Code.SelectionBackColor = Color.White;
                    
            //    lineCounter++;
            //}
        }

        private void displayHints(Level level)
        {
            string temp = level.getNextOpsHint();
            if (rtb_Outputs.Text == temp)
                return;
            rtb_Outputs.Text = temp;
        }

        private void displayConsoleOutput(Level level)
        {
            //need to add output/console tracking to the level class before I can implement this disaply method
            string temp = level.getConsoleOutputSoFar();
            if (rtb_Outputs.Text == temp)
                return;
            rtb_Outputs.Text = temp;
        }

        private void displayOpCodes(Level level)
        {
            string outputBoxText = "";
            foreach (string op in level.operationsSoFar)
                outputBoxText += op + "\n";
            if (outputBoxText == rtb_Outputs.Text)
                return;
            rtb_Outputs.Text = outputBoxText;
        }

        public string openNewLevel()
        {
            string temp = levelToOpen;
            levelToOpen = null;
            return temp;
        }

        public bool isThereALevelToLoad()
        {
            return levelToOpen != null;
        }

        private void openLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog filePicker = new OpenFileDialog();
            
            filePicker.ShowDialog();
           
            levelToOpen = filePicker.FileName;
        }

        private void interactionForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void rb_description_checkChange(object sender, EventArgs e)
        {
            changeBasedOnLevelState(levelState);
        }

        private void rb_consoleOut_checkChange(object sender, EventArgs e)
        {
            changeBasedOnLevelState(levelState);
        }

        private void rb_hintsOut_checkChange(object sender, EventArgs e)
        {
            changeBasedOnLevelState(levelState);
        }

        private void rb_OpsOut_checkChange(object sender, EventArgs e)
        {
            changeBasedOnLevelState(levelState);
        }

        private void rtb_Code_VScroll(object sender, EventArgs e)
        {

        }

        private void rtb_Code_TextChanged(object sender, EventArgs e)
        {
            //highlight the line numbers section of code based on the new text
            
        }

        private void interactionForm_Load(object sender, EventArgs e)
        {

        }

        public void changePosition(int X, int Y)
        {
            this.Location = new System.Drawing.Point(X, Y);
        }
    }
}
