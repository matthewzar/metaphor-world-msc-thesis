using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using mastersLibrary;
using System.IO;


namespace mastersLibrary
{
    /// <summary>
    /// Keeps track of several things:
    /// The actual code of a program
    /// The order that the lines are executed in
    /// The value of each variable after each line
    /// TODO-Outputs
    /// </summary>
    public class codeTracker
    {
        List<String> codeLines; //the actual code
        List<Tuple<int,string>> lineOrder; //the order that each line is executed it
        List<inventoryContent> theVariables; //the value of each variable after each line of code
        List<IO> theInputs;
        List<IO> theOutputs;
        int currentLine;


        public int getCurrentLineNumber()
        {
            if (currentLine >= lineOrder.Count)
                return 0;
            return lineOrder[currentLine].Item1;
        }

        public string getCurrentLineDescriptor()
        {
            if (currentLine >= lineOrder.Count)
                return "";
            return lineOrder[currentLine].Item2;
        }

        public List<string> getCurrentVariables()
        {
            if (currentLine >= theVariables.Count)
                return new List<string>() { "" };
            return theVariables[currentLine].ToStringList();
        }


        public List<string> getCode()
        {
            return codeLines;
        }

        public void counterAdvance()
        {
            if (currentLine < lineOrder.Count)
                currentLine++;
            else
            {
                Console.WriteLine("STARTING AGAIN");
                currentLine = 0;
            }
        }

        public void counterRetreat()
        {
            if (currentLine > 0)
                currentLine--;
            else
            {
                Console.WriteLine("Can't retreat before the start");
                currentLine = 0;
            }

        }



        public codeTracker(string programName, string versionNumber)
        {
            currentLine = 0;
            //read everything from files
            codeLines = mfl.loadDataFromFile("...\\samplePrograms\\"+programName + "Ʌ" + versionNumber + "Ʌco.txt");
            List<string> lineOrderAsString = mfl.loadDataFromFile("...\\samplePrograms\\" + programName + "Ʌ" + versionNumber + "Ʌlo.txt");
            List<string> theVariablesAsAString = mfl.loadDataFromFile("...\\samplePrograms\\" + programName + "Ʌ" + versionNumber + "Ʌvv.txt"); //the value of each variable after each line of code without converting them to inventory objects yet

            //check that nothing is null
            if (codeLines == null || lineOrderAsString == null || theVariablesAsAString == null)
                throw new Exception("At least one file was either missing or invalid");

            
            //lineOrder = lineOrderAsString.Select(n => Convert.ToInt32(n)).ToList();
            lineOrder = new List<Tuple<int, string>>();
            for(int i = 0; i < lineOrderAsString.Count; i++)
            {
                if(lineOrderAsString[i].Contains("Ʌ"))                    
                    lineOrder.Add(new Tuple<int,string>(Convert.ToInt32(lineOrderAsString[i].Substring(0,lineOrderAsString[i].IndexOf("Ʌ"))),"Theres something wrong with this line"));
                else
                    lineOrder.Add(new Tuple<int,string>(Convert.ToInt32(lineOrderAsString[i]),""));
            }



            theVariables = new List<inventoryContent>();
            foreach (var x in theVariablesAsAString)
            {
                theVariables.Add(new inventoryContent(x));
            }

            if (theVariables.Count != lineOrder.Count)
                throw new Exception("Number of lines in Variable Value files do not match those of lineOrder file");
        }



        
    }

    struct IO
    {
        public IO(string type, string io)
        {
            IOType = type;
            actualIO = io;
        }
        public string IOType;
        public string actualIO;
    }
}
