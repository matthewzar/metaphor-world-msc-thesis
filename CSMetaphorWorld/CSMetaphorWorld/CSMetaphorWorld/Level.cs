using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace CSMetaphorWorld
{
    /// <summary>
    /// Keeps track of a single level:
    /// -The code that the level is based on
    /// -The order of line execution
    /// -The list of correctly ordered operations
    /// -List of hints for each operation
    /// 
    /// NOT the current list of executed operations, that will be stored externally
    /// </summary>
    public class Level
    {
        [XmlAttribute]
        public string name;

        [XmlAttribute]
        public string description;

        /// <summary>
        /// This string determines which components the level will want enabled. For example a basic level that teaches assignment might only have the local frame, pencil, and user-hand enabled. While a level
        /// that starts introducing methods will have everything EXCEPT things for heap interaction. Legal values: "all","minimal","heap","methods"
        /// </summary>
        [XmlAttribute]
        public string componentEnabledMode = "all"; 

        /// <summary>
        /// The actual code the level is based on 
        /// </summary>
        public string code;

        public bool isComplete = false;

        /// <summary>
        /// !!!LINE INDEXING STARTS AT 1!!!
        /// The line order that corresponds to each operation. If a single line needs more than one operation (eg 'int x = 3 * y' needs to declare an int called x, write to calculator,
        /// read from y, replace y in the calculator, evaluate the whole expressions, and then assign the result to x) will have the same line several times.
        /// </summary>
        public List<int> lineOrder;

        /// <summary>
        /// The correct level operations from start to end of execution
        /// </summary>
        public List<string> completeLevelInstructions;
        
        /// <summary>
        /// The hints that might be displayed to a student who is struggling with a particular operation
        /// </summary>
        public List<string> hintForEachInstruction;

        public List<string> consoleOutputs;

        public List<string> operationsSoFar;

        public List<int> highlightStarts;
        public List<int> highlightLengths;

        public string[] availableMethodSignatures;

        /// <summary>
        /// This method lets you restart the current level without actually reloading it, it sets the 'completed operations' fields to their initial values 
        /// </summary>
        public void restartLevel()
        {
            operationsSoFar = new List<string>();
        }

        public Level() { }

        public Level(string levelName, string levelDescription, string levelCode, string[] availableMethodSignatures, List<int> lineOrderPerOp, List<string> completeOperationList, List<string> listOfHintsForEachOp)
        {
            name = levelName;
            description = levelDescription;
            code = levelCode;
            lineOrder = lineOrderPerOp;
            completeLevelInstructions = completeOperationList;
            hintForEachInstruction = listOfHintsForEachOp;
            this.availableMethodSignatures = availableMethodSignatures;
            consoleOutputs = new List<string>();
            operationsSoFar = new List<string>();
        }

        public void insertNewOperation(int indexToInsertAt, int lineNumber, int highlightStart, int highlightLength, string opCode, string hint, string consoleOutput)
        {
            if (lineOrder == null)
                lineOrder = new List<int>();

            if (this.highlightStarts == null)
                this.highlightStarts = new List<int>();

            if (highlightLengths == null)
                highlightLengths = new List<int>();

            if (completeLevelInstructions == null)
                completeLevelInstructions = new List<string>();

            if (hintForEachInstruction == null)
                hintForEachInstruction = new List<string>();

            if (consoleOutputs == null)
                consoleOutputs = new List<string>();

            this.highlightStarts.Insert(indexToInsertAt, highlightStart);
            this.lineOrder.Insert(indexToInsertAt, lineNumber);
            this.highlightLengths.Insert(indexToInsertAt, highlightLength);
            this.completeLevelInstructions.Insert(indexToInsertAt, opCode);
            this.hintForEachInstruction.Insert(indexToInsertAt, hint);
            this.consoleOutputs.Insert(indexToInsertAt, consoleOutput); 
        }

        public void addNewOperation(int lineNumber, int highlightStart, int highlightLength, string opCode, string hint, string consoleOutput)
        {
            if (lineOrder == null)
                lineOrder = new List<int>();
            
            if (highlightStarts == null)
                highlightStarts = new List<int>();
            
            if (highlightLengths == null)
                highlightLengths = new List<int>();

            if (completeLevelInstructions == null)
                completeLevelInstructions = new List<string>();

            if (hintForEachInstruction == null)
                hintForEachInstruction = new List<string>();

            if (consoleOutputs == null)
                consoleOutputs = new List<string>();
            
           
            highlightStarts.Add(highlightStart);
            lineOrder.Add(lineNumber);
            highlightLengths.Add(highlightLength);
            completeLevelInstructions.Add(opCode);
            hintForEachInstruction.Add(hint);
            consoleOutputs.Add(consoleOutput); 
        }

        public override string ToString()
        {
            string theOutput = "";
            
            for (int i = 0; i < lineOrder.Count; i++)
            {
                theOutput += lineOrder[i] + "  -  " + completeLevelInstructions[i] + "   -   " + hintForEachInstruction[i] + "   -   " + consoleOutputs[i] + "\n";
            }
                
            return theOutput;
        }


        public bool isNextOpCorrect(string nextOp)
        {
            if (isInFreePlayMode())
                return true;

            //is it calculator related operation
            if (nextOp.Contains("calculator") && nextOp.Contains(" "))
            {
                return stripSpacesFromExpression(nextOp) == stripSpacesFromExpression(completeLevelInstructions[operationsSoFar.Count]);
            }

            if (completeLevelInstructions.Count > operationsSoFar.Count)
                return completeLevelInstructions[operationsSoFar.Count] == nextOp;
            else
            {
                Console.WriteLine("There are no more operation to perform, the level is complete");
                return false;
            }
        }

        private string stripSpacesFromExpression(string expression)
        {
            return expression.Replace(" ", "");
        }

        public bool isInFreePlayMode()
        {
            return code == null || code == "" || completeLevelInstructions.Count == 0;
        }

        public string getConsoleOutputSoFar()
        {
            if (isInFreePlayMode())
            {
                return "";
            }

            string returnText = "";
            for (int i = 0; i < operationsSoFar.Count; i++)
            {
                returnText += consoleOutputs[i];
                //TODO - add in a special charachter (like '╖' which clears the consoletext)
            }
            return returnText;
        }

        /// <summary>
        /// This could be used to cheat, however it's primary goal is as a tool so that operation specific actions can be taken by whomever calls this method. For example
        /// I might want to display certain sprites only for certain operations (like hints), thus I would need to look at the next operation that the user needs to perform.
        /// </summary>
        /// <returns></returns>
        public string getNextOperationCode()
        {
            if (!isComplete && operationsSoFar.Count < completeLevelInstructions.Count)
                return completeLevelInstructions[operationsSoFar.Count];
            else
                return "";
        }

        public void advanceToNextOp(string specificOp = null)
        {
            if (specificOp == null)
                operationsSoFar.Add(completeLevelInstructions[operationsSoFar.Count]);
            else
                operationsSoFar.Add(specificOp);
        }

        public string getNextOpsHint()
        {
            if (operationsSoFar.Count >= hintForEachInstruction.Count)
                return "";
            return hintForEachInstruction[operationsSoFar.Count];
        }

        public int getCurrentHighlightLength()
        {
            if (operationsSoFar.Count >= lineOrder.Count)
            {
                //getting here means the level is in fact complete, first time round we need to mark it as such
                if (!isComplete)
                {
                    Console.WriteLine("Finished this level");
                    isComplete = true;
                }
                return 0;
            }
            return highlightLengths[operationsSoFar.Count];
        }

        public int getCurrentHighlightStart()
        {
            if (operationsSoFar.Count >= lineOrder.Count)
            {
                //getting here means the level is in fact complete, first time round we need to mark it as such
                if (!isComplete)
                {
                    Console.WriteLine("Finished this level");
                    isComplete = true;
                }
                return 0;
            }
            return highlightStarts[operationsSoFar.Count];
        }

        public int getCurrentLineNumber()
        {
            if (operationsSoFar.Count >= lineOrder.Count)
            {
                //getting here means the level is in fact complete, first time round we need to mark it as such
                if (!isComplete)
                {
                    Console.WriteLine("Finished this level");
                    isComplete = true;
                }
                return -1;
            }
            return lineOrder[operationsSoFar.Count];
        }


        #region serialization, saving, loading etc
        /// <summary>
        /// Serializes the instance of the class and save it in an XML format in the address/file that you specify
        /// </summary>
        /// <param name="saveDirectory"></param>
        public void serialize(string saveDirectory)
        {
            // Create an instance of the XmlSerializer class; 
            // specify the type of object to serialize.
            XmlSerializer serializer = new XmlSerializer(typeof(Level));
            TextWriter writer = new StreamWriter(saveDirectory);

            // Serialize the class, and close the TextWriter.
            serializer.Serialize(writer, this);
            writer.Close();
        }

        public static Level loadLevelFromFile(string openDiectory)
        {
            return deserialize(openDiectory);
        }

        /// <summary>
        /// Use this an an alterative to the standard constuctor in order to create a fully initilized instance of the Level calss from a saved file
        /// </summary>
        /// <returns></returns>
        public static Level deserialize(string openDirectory)
        {
            // Create an instance of the XmlSerializer class; 
            // specify the type of object to be deserialized.
            XmlSerializer serializer = new XmlSerializer(typeof(Level));

            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(openDirectory, FileMode.Open);
            
            // Declare an object variable of the type to be deserialized.
            Level deSerialResult;

            /* Use the Deserialize method to restore the object's state with
             data from the XML document. */
            deSerialResult = (Level)serializer.Deserialize(fs);
            fs.Close();

            deSerialResult.description = deSerialResult.description.Replace(@"\n", "\n").Replace(@"\r", "\r");

            return deSerialResult;
        }
#endregion
  
        /* Calling example code: 
           xmlSerializationExample test = new xmlSerializationExample(3);
           test.serialize();
           xmlSerializationExample x = xmlSerializationExample.deserialize();
           Console.WriteLine();*/
    }
}
