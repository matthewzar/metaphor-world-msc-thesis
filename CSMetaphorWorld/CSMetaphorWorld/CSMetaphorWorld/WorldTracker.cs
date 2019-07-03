using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CSMetaphorWorld
{
    ///will keep track of all the API classes and objects. This class allows for abstraction of control away from the user/programmer
    ///as any instructions can simply be sent to the class as a string (such as "declarevariable int x") and it will take care of all the details.
    ///If you want to use the API in a less standard way most functions that this class performs will still be available for direct calling.
    ///If you need even more control you can bypass-recreate this class entirely as its primary purpose is simplification of API interfacing.
    public class WorldTracker
    {
        /// <summary>
        /// This list contains a complete list of all the operations that the user has executed, which succeded, this will be neccessary for the undo button 
        /// becuase we don't keep a complete state history for each operation, therefore when we 'undo' an operation we actually restart the current tracker
        /// and simply execute all the operations that the user has done with the exception of the most recent one.
        /// </summary>
        public List<string> completedOperations = new List<string>();

        const int BYTES_IN_THE_HEAP = 1024;
        const string constDefaultExpression = "zy + 20 / 10";

        public Level currentLevel;
       
        public Stack globalStack;
        public Heap globalHeap;
        public MemoryManager theMemManager;
        public Player thePlayer;
        public MethodMechanism methodMech;
        public Calculator theCalculator;
        public valueMoveMode theVariableMode = valueMoveMode.Copy;

        public Action actionOnIncorrectOperation;
        public Action actionOnCorrectOperation;

        /// <summary>
        /// Keeps a count of how many operations are done that don't match the next line of the current level
        /// </summary>
        public int totalErrors;

        public enum valueMoveMode
        {
            Copy = 1,
            Cut = 2
        }

        public bool localVariableHasDefaultValue(int variableNumber)
        {
            return globalStack.getTopFrame().getCopyOfValueFromFramesVariable(variableNumber, false).readOrigin().Contains("default");
        }

        /// <summary>
        /// Sets up the methods to be be called when an operation completes.
        /// For example when the user clicks a sprite, once everything else is done the right/wrong method will be called
        /// </summary>
        /// <param name="actionOnCorrectOperation"></param>
        /// <param name="actionOnIncorrectOperation"></param>
        public void setOperationCompletionActions(Action actionOnCorrectOperation, Action actionOnIncorrectOperation)
        {
            this.actionOnCorrectOperation = actionOnCorrectOperation;
            this.actionOnIncorrectOperation = actionOnIncorrectOperation;
        }

        public void addCompletedOperation(string completedOperation)
        {
            completedOperations.Add(completedOperation);
        }

        public void undoLastOperation(bool freeplay, Action completionAction = null)
        {
            List<string> temp;
            //Check if we have any operations to undo in the first place
            if (completedOperations.Count > 0)
            {
                //we need a temporary list to iterate through seeing as performOperation will 
                //add to the completedOperations list regardless.
                completedOperations.RemoveAt(completedOperations.Count - 1);
                temp = new List<string>(completedOperations); //clone the list 
                completedOperations = new List<string>();
            }
            else
                return; //why reload everything if we haven't made any changes yet?

            initialiseFields();

            //reload level based on current level
            currentLevel.restartLevel();


            //most important part of the whole thing:
            foreach (string op in temp)
                performOperation(op, freeplay);

            if(completionAction != null)
                completionAction();
        }

        private void initialiseFields()
        {
            globalStack = new Stack("Main");
            globalHeap = new Heap(BYTES_IN_THE_HEAP);
            theMemManager = new MemoryManager();

            thePlayer = new Player();

            methodMech = new MethodMechanism();
            theCalculator = new Calculator();
        }

        public WorldTracker(string addressOfLevelToLoad)
        {
            initialiseFields();

            if (System.IO.File.Exists(addressOfLevelToLoad))
                currentLevel = Level.deserialize(addressOfLevelToLoad);
            else
            {
                currentLevel = new Level("empty","sandbox","",new string[0],new List<int>(),new List<string>(),new List<string>());
                setTheCurrentLevelsMethods(new string[]{"action(int x)", "function(int s, double d)", "isPrime(int num)", "longSig(int a, int b, int c, int d, int e, int f)"}); 
            }
            totalErrors = 0;

            //perform declarations based on the behaviour options selected
            if (theVariableMode == valueMoveMode.Copy)
            {
                //when in copy mode the hand and calculator are never allowed to be blank, therefore we must declare them some deafult values

                
                theCalculator.evaluateExpression(false);//this line is to ensure that the expression is marked as being evaluated so that the FSM in the calucator works correctly
                //This line could in theory be left out if the recieve expression function recogised either the first ever expression, or the expression '0', or if we built this into its constructor (this works for now)
            }
            else
            {
                //when in cut mode we have the option of either allowing blank values OR not. 
                //We're just going to leave this space blank but available for now...as we aren't even certain if we are going to leave cut mode in at all, let alone how it should behave
            }
            
             actionOnCorrectOperation = delegate() { };
             actionOnIncorrectOperation = delegate() { };
        }


        delegate bool operationActionDelegate(string input, Action endAction);

        //because we can only have a delegate with one fixed signature, we will need to overload the non-input operations so that they can included
        List<Tuple<string, operationActionDelegate>> opcodeDelegateConversionList = new List<Tuple<string, operationActionDelegate>>()
            {

            };


        private Action nullAction = delegate() { };

        public bool performOperationFromOpList(string opAsString)
        {
            return performOperationFromOpList(opAsString, nullAction);
        }

        public bool performOperationFromOpList(string opAsString, Action finishingAction)
        {
            if (finishingAction == null)
                finishingAction = nullAction;

            //ensure that the input is valid
            if (opAsString == null || opAsString == "")
                throw new Exception("Null and empty string can never be valid operations");

            
            //check whether the operation also has an input part
            string opType = opAsString;
            string opInput = "";
            if (opAsString.Contains(" "))
            {
                opType = opAsString.Substring(0, opAsString.IndexOf(" "));
                opInput = opAsString.Substring(opAsString.IndexOf(" "));
            }

            //check if we have a valid list of operations and opcodes to convert between
            if (opcodeDelegateConversionList != null)
            {
                //if we do then see if the list has an entry that matches the methods input
                for (int i = 0; i < opcodeDelegateConversionList.Count; i++)
                {
                    //if the opCode is a match, call the operation and return the result
                    if (opType == opcodeDelegateConversionList[i].Item1)
                        return opcodeDelegateConversionList[i].Item2(opInput, finishingAction);
                }
            }
            return false;
        }

        public bool performOperation(string opAsString, bool anyActionsAllowed = false)
        {
            return performOperation(opAsString, nullAction, anyActionsAllowed);
        }

        public bool performOperation(string opAsString, Action finishingAction, bool anyActionsAllowed = false)
        {
            //check if the given operation is the correct one
            if (anyActionsAllowed == false && !currentLevel.isNextOpCorrect(opAsString))
            {
                if (currentLevel.isComplete)
                    Console.WriteLine("You finished the current level. To keep playing load a different level.");
                else
                {
                    totalErrors++;
                    Console.WriteLine("Not the correct operation");
                    actionOnIncorrectOperation();
                }
                return false;
            }
            else
                if (anyActionsAllowed)
                    currentLevel.advanceToNextOp(opAsString);
                else 
                    currentLevel.advanceToNextOp();

            //check whether the operation also has an input part
            string opType = opAsString;
            string opInput = "";
            if (opAsString.Contains(" "))
            {
                opType = opAsString.Substring(0, opAsString.IndexOf(" "));
                opInput = opAsString.Substring(opAsString.IndexOf(" ")+1);
            }

            //try and perform the operation based on the list of opcodes and associated delegates
            bool result = performOperationFromOpList(opAsString, finishingAction);
            if (result)
            {
                actionOnCorrectOperation();
                addCompletedOperation(opAsString);
                return true;
            }
            //getting here means that we the 'dynamic' list of matching operations and opcodes either didn't contain the opcode or failed, therefore we will look through all the defaults

            bool succeeded = false;
            switch (opType.ToLower())
            {
                #region macro type operations (multiple low level steps in one), most require input
                case("preparemethod"):
                    succeeded = prepareEntireMethod(opInput, finishingAction);
                    break;

                #endregion
                
                #region operations that require input
                //variables
                case ("declarevariable"):
                    succeeded = declareStackVariableIO(opInput, finishingAction);
                    break;

                case ("assignvalue"):
                    succeeded = assignStackVariableIO(opInput, finishingAction);
                    break;

                case ("readvariable"):
                    succeeded = readValueFromVariableInFrame(opInput, finishingAction);
                    break;

                //calculator
                case ("calculatorexpression"):
                case ("expressionintocalculator"):
                case ("enterexpression"):
                    succeeded = enterCalculatorExpressionIO(opInput, finishingAction);
                    break;

                case ("substituteexpression"):
                case ("replaceexpressionvalue"):
                case ("handtoexpression"):
                    succeeded = substituteCalculatorValue(opInput, finishingAction);
                    break;

                case ("handexpressiontocalculator"):
                case ("handtocalculator"):
                    succeeded = handExpressionToCalculator(finishingAction);
                    break;

                //methods
                case ("namemethod"):
                case ("namefunction"):
                case ("declaremethod"):
                    succeeded = nameMethod(opInput, finishingAction);
                    break;

                case ("declareparameter"):
                case ("createparameter"):
                case ("parameterdeclaration"):
                    succeeded = declareParameter(opInput, finishingAction);
                    break;

                case ("assignparameter"):
                    succeeded = assignParameterAValue(opInput, finishingAction);
                    break;

                //other
                case ("declarearray"):
                case ("createarray"):
                case ("arrayonheap"):
                    succeeded = declareArrayOnHeapIO(opInput, finishingAction);
                    break;

                case ("inputaddresstomemman"):
                case ("inputaddresstomemorymanager"):
                case ("memoryaddressfrominputtomemorymanager"):
                    succeeded = memoryAddressFromInputToMemoryManager(opInput, finishingAction);
                    break;

                case ("inputoffsettomemman"):
                case ("giveinputoffsettomemman"):
                case ("inputoffsettomemorymanager"):
                    succeeded = giveInputMemoryOffsetToMemoryManager(opInput, finishingAction);
                    break;

                case ("directhandwrite"):
                    succeeded = writeValueToHand(opInput, finishingAction);
                    break;

                #endregion

                #region non-input operations

                case("consumebool"):
                    succeeded = consumeBoolFromHand(finishingAction);
                    break;
                    

                ///stack-mem man
                case ("writevaluetostack"):
                case ("memmanvaluetostack"):
                case ("memorymanagervaluetostack"):
                    succeeded = writeValueToStackReference(finishingAction);
                    break;

                case ("readvaluefromstack"):
                case ("stackvaluetomemman"):
                case ("stackvaluetomemorymanager"):
                    succeeded = readValueFromStackToMemMan(finishingAction);
                    break;

                //heap-mem man
                case ("readvaluefromheap"):
                case ("heapvaluetomemman"):
                case ("heapvaluetomemorymanager"):
                    succeeded = readValueFromHeapToMemMan(finishingAction);
                    break;

                case ("writevaluetoheap"):
                case ("memmanvaluetoheap"):
                case ("memorymanagervaluetoheap"):
                    succeeded = writeValueFromMemManToHeap(finishingAction);
                    break;

                //user-mem man
                case ("getvaluefrommemman"):
                case ("getvaluefrommemorymanager"):
                case ("readvaluefrommemorymanager"):
                    succeeded = readValueFromMemoryManager(finishingAction);
                    break;

                case ("handtomemman"):
                case ("handtomemorymanager"):
                case ("handvaluetomemorymanager"):
                    succeeded = handValueToMemoryManager(finishingAction);
                    break;

                case ("handaddresstomemman"):
                case ("handaddresstomemorymanager"):
                case ("memoryaddressfromhandtomemorymanager"):
                    succeeded = memoryAddressFromHandToMemoryManager(finishingAction);
                    break;

                case ("getabsoluteaddress"):
                case ("convertoffsetandaddress"):
                case ("convertaddressandoffsettoabsolute"):
                    succeeded = convertAddressAndOffsetToAbsolute(finishingAction);
                    break;

                case ("handoffsettomemman"):
                case ("givehandoffsettomemman"):
                case ("handoffsettomemorymanager"):
                    succeeded = giveHandMemoryOffsetToMemoryManager(finishingAction);
                    break;

                //Methods
                case ("callfunction"):
                    succeeded = callSelectedMethod(finishingAction);
                    break;

                case ("return"):
                    succeeded = returnFromMethod(finishingAction);
                    break;

                // other
                case ("evaluatecalculatortohand"):
                case ("calculatorevaltohand"):
                case ("getanswertohand"):
                    succeeded = evaluateCalculatorExpToHand(finishingAction);
                    break;

                case ("calculatortohand"):
                case ("calculatorresulttohand"):
                case ("expressiontohand"):
                    succeeded = moveCalculatorExpToHand(finishingAction);
                    break;
        
                case ("evaluatecalculator"):
                case ("simplifyexpression"):
                case ("calculateinplace"):
                    succeeded = evaluateCalculatorExpInPlace(finishingAction);
                    break;
                    
                case ("garbagecollect"):
                    succeeded = garbageCollect(finishingAction);
                    break;

                case ("deletenewestvariable"):
                    succeeded = deleteNewestVariable(finishingAction);
                    break;

                //this operation does not require specifific input in the form of knowing where to get the output string as it always comes from the hand ->
                //either as a memory address in the hand or the actual content of the hand
                case ("consolewrite"):
                    succeeded = consoleOutputFromHand(finishingAction);
                    break;

                #endregion


                default:
                    Console.WriteLine("Thats not a recognised command (inside performOperation)");
                    succeeded = false;
                    break;
            }

            if (succeeded)
            {
                actionOnCorrectOperation();
                addCompletedOperation(opAsString);
            }
            else
                actionOnIncorrectOperation();
            return succeeded;
            
        }

        #region macro operations (many small operations in one go) that require input in some form

        #region Get an entire method signature ready (method name, and default parameter values)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">Takes the form: "methodName(param1Type paramName, param2Type param2Name" for example "factorial(int n)"/></param>
        /// <returns>Return True if the action was successfull otherwise False</returns>
        public bool prepareEntireMethod(string input)
        {
            return prepareEntireMethod(input, nullAction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">Takes the form: "methodName(param1Type paramName, param2Type param2Name" for example "factorial(int n)"/></param>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True if the action was successfull otherwise False</returns>
        public bool prepareEntireMethod(string input, Action finishingAction)
        {
            try
            {
                //Check if we already have a name in the mechanism, then throw the approriate exception if we do (we do this first so as to avoid unneccessary work if there is a mistake
                if (!methodMech.isNotHandlingAnything()) 
                    throw new Exception("You can't name a new method to call until you've finished calling the first method");

                //break input down into 2 main section:
                //the method name
                string methodName = input.Substring(0, input.IndexOf('('));

                //get the string that contains the assorted parameters (not inlcuding the brackets
                string parameters = input.Substring(methodName.Length+1, input.Length - methodName.Length - 2);

                //from parameters remove all substrings that are unneccary whitespace (" ," and ", " need to both be replaced by ",")
                //this will ensure that when we split the spring, we don't get things like this: "int x , int y,int   z" -> {"int x "," int y","int  z"}
                while (true)
                {
                    string temp = parameters;
                    parameters = parameters.Replace(", ", ",");
                    parameters = parameters.Replace(" ,", ",");
                    parameters = parameters.Replace("  ", "  ");
                    if (temp == parameters)
                        break;
                }

                //break the parameters down into a list of parameters (eg {"int age","double height"})
                string[] parametersAsList = new string[0];
                if(parameters != "") //if we alowed empty springs to be split on ','s we would end up having an array with one empty string element, rather than an empty array
                    parametersAsList = parameters.Split(',');


                //we now have the methods name and an array of all the parameters that need to be passed
                

                //name the method to be called
                methodMech.passMethodName(methodName);

                //go through each parameter and declare them with default values 
                foreach (string parameter in parametersAsList)
                {
                    string paramType = parameter.Substring(0, parameter.IndexOf(' '));
                    string paramName = parameter.Substring(parameter.IndexOf(' ') + 1);

                    //regardless of mode we always want the parameters to be declared with some default value so that users get the idea that nothing on a computer is ever really empty
                    methodMech.passSingleParameter(new Value(paramType, "defaultParameter"), paramName);
                }

                
                
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside prepareEntireMethod an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calls the other nameMethod, and passes the users input from the guide to one of the other nameMethod overloads
        /// </summary>
        /// <param name="varDetails"></param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool prepareEntireMethod(IAsyncResult varDetails)
        {
            string details = Guide.EndShowKeyboardInput(varDetails);
            return prepareEntireMethod(details);
        }
        #endregion

        #endregion

        #region Fine grained actions that require input in some form
        //originally these methods where being declared as (void xxxAsAction()) and got their input from the methods that called them 
        //This is not an optimal method for sending input in as in caused the form the input took to come from a constant source
        //(either always from the xna Guide class, the console, or even files....but never from more than one for a single action)
        //These fucntions are going to take on 2 constant formats (bool xyzIOAction(string input) AND bool xyzIOAction(string input, Action endAction))
        //this allows them to be called used interchangably as delegates later in development if an alternative mapping system is implemented or we want to give other 
        //classes/objects the option of calling the function without interfacing. The returned boolean type is in indicator of whether the
        //action was succesfull or not (this can be used for error checking). The endAction is an action to be perfored at the end of the regular API manipulation

        #region write a value directly to the hand (bypasses reading of any kind and gets the value from the user)
        
        /// <summary>
        /// Write a value directly to the hand (bypasses reading of any kind and gets the value from the user)
        /// </summary>
        /// <param name="input">Takes the form: "[type] [value as string]" for example "int x" or "string \"hello\""/></param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool writeValueToHand(string input)
        {
            return writeValueToHand(input, nullAction);
        }

        /// <summary>
        /// If this flag is true any boolean values written to the hand will be removed immediately, but:
        ///      only when the value being held is actually a bool
        ///      only when we are in an actual level
        ///      only when the level is not complete
        ///      only if the next operation is NOT an assignment or substitution of some kind (this is to check if the current operation is handling a conditional that needs a temporary bool) 
        /// </summary>
        public bool autoRemovingBoolAtConditional = false;

        /// <summary>
        /// Write a value directly to the hand (bypasses reading of any kind and gets the value from the user)
        /// </summary>
        /// <param name="input">Takes the form: "[type] [value as string]" for example "int x" or "string \"hello\""/></param>
        /// <param name="finishingAction">This is the action that will be performed once the variable is declared (for example you could pass a void method that refreshes the frame sprites)</param>
        /// <returns>Return True if the action was successfull otherwise False</returns>
        public bool writeValueToHand(string input, Action finishingAction)
        {
            //TODO add checks for validity of varDetails (eg does it already exist, is it a know type, is it a valid name etc)
            //TODO think about using regex here instead of clunky string handling

            try
            {
                if (input == null || input == "")
                {
                    Console.WriteLine("Bad input data for declareStackVariableIO");
                    return false;
                }

                
                string valType = input.Substring(0, input.IndexOf(' '));
                string valContent = input.Substring(input.IndexOf(' ') + 1);

                thePlayer.holdValue(valType, valContent, "user");

                //Under certain conditions, we want to auto remove bool values from the hand ->
                    //only when the value being held is actually a bool
                    //only when we are in an actual level
                    //only when the level is not complete
                    //only if the next operation is NOT an assignment or substitution of some kind (this is to check if it's a conditional that needs a temporary bool) 
                
                if (autoRemovingBoolAtConditional && thePlayer.examineHeld().readType() == "bool" && !currentLevel.isInFreePlayMode() && !currentLevel.isComplete)
                {
                   string nextOpCode = currentLevel.getNextOperationCode();
                   if (nextOpCode.Substring(0, 11) != "assignvalue" && nextOpCode.Substring(0, 11) != "assignparameter" && nextOpCode.Substring(0, 11) != "substituteexpression")
                       thePlayer.takeFromHand();
                }

                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside writeValueToHand an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calls the other writeValueToHand, and passes the users input from the guide to one of the other writeValueToHand overloads
        /// </summary>
        /// <param name="varDetails"></param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool writeValueToHand(IAsyncResult varDetails)
        {
           string details = Guide.EndShowKeyboardInput(varDetails);
           return writeValueToHand(details);
        }
       


        #endregion

        #region declare stack frame variable functions

        /// <summary>
        /// Declare a local variable on the stack that exist in the current frame.
        /// </summary>
        /// <param name="input">Takes the form: "[type] [name]" for example "int x"/></param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool declareStackVariableIO(string input)
        {
            return declareStackVariableIO(input, nullAction);
        }

        /// <summary>
        /// Declare a local variable on the stack that exist in the current frame.
        /// </summary>
        /// <param name="input">Takes the form: "[type] [name]" for example "int x"/></param>
        /// <param name="finishingAction">This is the action that will be performed once the variable is declared (for example you could pass a void method that refreshes the frame sprites)</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool declareStackVariableIO(string input, Action finishingAction)
        {
            //TODO add checks for validity of varDetails (eg does it already exist, is it a know type, is it a valid name etc)
            //TODO think about using regex here instead of clunky string handling

            try
            {
                if (input == null || input == "")
                {
                    Console.WriteLine("Bad input data for declareStackVariableIO");
                    return false;
                }

                
                string varType = input.Substring(0, input.IndexOf(' '));
                string varName = input.Substring(input.IndexOf(' ') + 1);

                //TODO - add it a mechanism that lets the user define (when instatiating this class, whether they want vairables declared with default values every-time, with hand values everytime, or context sensitive (an emtpy hand give a default) otherwise it takes the value of the hand


               
                    //place a default value in the variable
                globalStack.getTopFrame().receiveNewVariable(new Variable(varType, varName, globalStack.advanceMemoryAddress(varType)));




                //TODO - check if the held value shares the correct data type
                
                //if the player is holding something and we are using cut mode then place it in the newly declared variable
                if (theVariableMode == valueMoveMode.Cut && !thePlayer.isHandEmpty())
                {
                        globalStack.getTopFrame().assignNewValue(varName, thePlayer.takeFromHand());
                }
                else if (theVariableMode == valueMoveMode.Copy)
                {
                    //when using copy mode, both the calculator AND the variable and the hand should never be blank, they retain whatever they where holding last

                    globalStack.getTopFrame().assignNewValue(varName, new Value(varType, "default"));
                    //Not sure why this check was in here....I don't recall ever chaning the value to read or new or used
                    /*if (!thePlayer.hasBeenRead())
                        globalStack.getTopFrame().assignNewValue(varName, thePlayer.copyFromHand());
                    else
                        //this happens if we are in copy mode, but the value in hand has been marked as read/used already. It's assigning the default value for the given type
                        globalStack.getTopFrame().assignNewValue(varName, Value.getDefaultValueForType(varType));*/

                }
                else
                {

                    Console.WriteLine("Inside declareStackVariableIO, a situation occured that shouldn't have");
                    Console.WriteLine("Is hand holding a value = {0}. Has the held value already been read = {1}. Mode = {2}", !thePlayer.isHandEmpty(), thePlayer.hasBeenRead(), theVariableMode.ToString());
                }


                finishingAction();

            }
            catch (Exception e)
            {
                Console.WriteLine("Inside declareStackVariableIO an exception occured with these deatails:\n{0}",e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calls the other declareStackVariableIO, and passes the users input from the guide to one of the other declareStackVariableIO overloads
        /// </summary>
        /// <param name="varDetails"></param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool declareStackVariableIO(IAsyncResult varDetails)
        {
           string details = Guide.EndShowKeyboardInput(varDetails);
           return declareStackVariableIO(details);
        }
       
        #endregion

        #region assign value to frame variable from hand
        /// <summary>
        /// Assign a local variable to a stacks variable that exist in the current frame.
        /// </summary>
        /// <param name="input">Takes the form: "[type] [name]" for example "int x"/></param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool assignStackVariableIO(string input)
        {
            return assignStackVariableIO(input, nullAction);
        }

        

        /// <summary>
        /// Assign a local variable to a stacks variable that exist in the current frame.
        /// </summary>
        /// <param name="input">Takes the form: "[varName]" for example "x" OR [varNum] eg "2"/></param>
        /// <param name="finishingAction">This is the action that will be performed once the variable is assigned (for example you could pass a void method that refreshes the frame sprites)</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool assignStackVariableIO(string input, Action finishingAction)
        {
            try
            {
                //TODO add checks for validity of expression (eg correct syntax, unkown chars, strange result etc)
                int num;
                if (int.TryParse(input,out num))
                    //check if we are meant to copy or cut the value in the value in the hand
                   // if(theVariableMode == valueMoveMode.Cut)
                        globalStack.getTopFrame().assignNewValue(globalStack.getTopFrame().getVariableXName(num),  thePlayer.takeFromHand());
                   // else
                    //    globalStack.getTopFrame().assignNewValue(globalStack.getTopFrame().getVariableXName(num), thePlayer.copyFromHand());
                else
                   // if (theVariableMode == valueMoveMode.Cut)
                        globalStack.getTopFrame().assignNewValue(input, thePlayer.takeFromHand());
                   // else
                   //     globalStack.getTopFrame().assignNewValue(input, thePlayer.copyFromHand());

                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside assignStackVariableIO an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calls the other assignStackVariableIO, and passes the users input from the guide to one of the other assignStackVariableIO overloads
        /// </summary>
        /// <param name="varDetails"></param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool assignStackVariableIO(IAsyncResult varDetails)
        {
            string details = Guide.EndShowKeyboardInput(varDetails);
            return assignStackVariableIO(details);
        }

        #endregion

        #region enter expression into calculator

        public bool enterCalculatorExpressionIO(string input)
        {
            return enterCalculatorExpressionIO(input, nullAction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">Takes the form: "" for example ""/></param>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool enterCalculatorExpressionIO(string input, Action finishingAction)
        {
            try
            {
                //TODO add checks for validity of expression (eg correct syntax, unkown chars, strange result etc), this is also done in the calculator as a  double precaustion
                
                theCalculator.recieveNewExpression(input);
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside enterCalculatorExpressionIO an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calls the other enterCalculatorExpressionIO, and passes the users input from the guide to one of the other enterCalculatorExpressionIO overloads
        /// </summary>
        /// <param name="varDetails"></param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool enterCalculatorExpressionIO(IAsyncResult varDetails)
        {
            string details = Guide.EndShowKeyboardInput(varDetails);
            return enterCalculatorExpressionIO(details);
        }
        #endregion

        #region substitute value into one of the variables of the calculators expression

        /// <summary>
        /// Substitute the value in the players hand into a given variable name
        /// </summary>
        /// <param name="input">The name of the variable to replace</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool substituteCalculatorValue(string input)
        {
            return substituteCalculatorValue(input, nullAction);
        }

        /// <summary>
        /// Substitute the value in the players hand into a given variable name
        /// </summary>
        /// <param name="input">The name of the variable to replace</param>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool substituteCalculatorValue(string input, Action finishingAction)
        {
            try
            {
                if (thePlayer.examineHeld() == null)
                {
                    Console.WriteLine("You can't substitute null into an expression");
                    return false;
                }
                //TODO add legality checks for the variable name


                if (theVariableMode == valueMoveMode.Cut)
                    theCalculator.substituteValueIntoExpression(input, thePlayer.takeFromHand().read());
                else
                    theCalculator.substituteValueIntoExpression(input, thePlayer.copyFromHand().read());
            
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside substituteCalculatorValue an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calls the other substituteCalculatorValue, and passes the users input from the guide to one of the other substituteCalculatorValue overload
        /// </summary>
        /// <param name="varDetails"></param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool substituteCalculatorValue(IAsyncResult varDetails)
        {
            string details = Guide.EndShowKeyboardInput(varDetails);
            return substituteCalculatorValue(details);
        }
        #endregion

        #region Read the value of a named variable into the players hand

        /// <summary>
        /// Read the value of a named variable into the players hand
        /// </summary>
        /// <param name="input">The name of the variable to read from</param>
        /// <returns></returns>
        public bool readValueFromVariableInFrame(string input)
        {
            return readValueFromVariableInFrame(input, nullAction);
        }

        /// <summary>
        /// Read the value of a named variable into the players hand
        /// </summary>
        /// <param name="input">The name of the variable to read from OR the number of the variable in question</param>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool readValueFromVariableInFrame(string input, Action finishingAction)
        {
            try
            {
                //TODO add checks for validity of expression
                //TODO check if the player is already holding something and decide what happens to that which is being held (it might get thrown away, places in a pocket or left on the notepad)

                int num;
                if (int.TryParse(input, out num))
                    thePlayer.holdValue(globalStack.getTopFrame().readVariable(globalStack.getTopFrame().getVariableXName(num)));
                else
                    thePlayer.holdValue(globalStack.getTopFrame().readVariable(input));
                
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside readValueFromVariableInFrame an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calls the other readValueFromVariableInFrame, and passes the users input from the guide to one of the other readValueFromVariableInFrame overloads
        /// </summary>
        /// <param name="varDetails"></param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool readValueFromVariableInFrame(IAsyncResult varDetails)
        {
            string details = Guide.EndShowKeyboardInput(varDetails);
            return readValueFromVariableInFrame(details);
        }
        #endregion


        #region Give memory address value to the memory manager from input

        /// <summary>
        /// Take a memory value from input and gives it to the memory manager
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool memoryAddressFromInputToMemoryManager(string input)
        {
            return memoryAddressFromInputToMemoryManager(input, nullAction);
        }

        /// <summary>
        /// Take a memory value from input and give it to the memory manager
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool memoryAddressFromInputToMemoryManager(string input, Action finishingAction)
        {
            try
            {
                theMemManager.holdMemoryAddressFromPlayer(int.Parse(input));
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside memoryAddressFromInputToMemoryManager an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        #endregion 

        #region Offset from input To memory manager

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool giveInputMemoryOffsetToMemoryManager(string input)
        {
            return giveInputMemoryOffsetToMemoryManager(input, nullAction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool giveInputMemoryOffsetToMemoryManager(string input, Action finishingAction)
        {
            try
            {
                //TODO: add checks that it is in fact a valid offset
                theMemManager.holdOffsetFromPlayer(int.Parse(input));
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside giveInputMemoryOffsetToMemoryManager an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        #endregion

        #region Declare an array in the heap

        /// <summary>
        /// Declares an array on the heap
        /// </summary>
        /// <param name="input">Takes the form: "[type] [name] [size]" for example "int xs 5"</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool declareArrayOnHeapIO(string input)
        {
            return declareArrayOnHeapIO(input, nullAction);
        }

        /// <summary>
        /// Declares an array on the heap
        /// </summary>
        /// <param name="input">Takes the form: "[type] [name] [size]" for example "int xs 5"></param>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool declareArrayOnHeapIO(string input, Action finishingAction)
        {
            try
            {
                //TODO add checks for validity of details (eg does it already exist, is it a know type, is it a valid name etc)

                if (input == null || input == "")
                {
                    return false;
                }
                //TODO think about using regex here instead of clunky string handling

                string arrType = input.Substring(0, input.IndexOf(' '));
                input = input.Substring(input.IndexOf(' ') + 1);
                string arrName = input.Substring(0, input.IndexOf(' '));
                int arrSize = int.Parse(input.Substring(input.IndexOf(' ') + 1));
                globalHeap.allocateArraySpace(arrSize, Value.getDefaultValueForType(arrType), arrName);
                
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside declareArrayOnHeapIO an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calls the other declareArrayOnHeapIO, and passes the users input from the guide to one of the other declareArrayOnHeapIO overloads
        /// </summary>
        /// <param name="varDetails"></param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool declareArrayOnHeapIO(IAsyncResult varDetails)
        {
            string details = Guide.EndShowKeyboardInput(varDetails);
            return declareArrayOnHeapIO(details);
        }
        #endregion

        #region Declare a parameter

        public bool declareParameter(string input)
        {
            return declareParameter(input, nullAction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">Takes the form: "[name] [type]" for example "int x"/></param>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool declareParameter(string input, Action finishingAction)
        {  //TODO add checks for validity of userInput (eg does it already exist, is it a know type, is it a valid name etc)
            try
            {
                if (!isMethodNamed())
                {
                    Console.WriteLine("There is no method name declared...so you shouldn't be trying to declare parameters");
                    return false;
                }

                if (input == null || input == "")
                {
                    return false;
                }

                string paramType = input.Substring(0, input.IndexOf(' '));
                string paramName = input.Substring(input.IndexOf(' ') + 1);
                
                //here (depending on if we are in cut or copy mode) we need to either leave it as it is with no value to start with (cut),
                //or we need to change it to move the users hand value in as the default value. 

                //be warned that in this mode the 'default' value of the parameter is the same as it's type (if we are going to leave cut mode in the game we might need to change this)
                if (theVariableMode == valueMoveMode.Cut)
                    methodMech.passSingleParameter(new Value(paramType, "defaultParameter"), paramName);
                else
                    methodMech.passSingleParameter(thePlayer.copyFromHand(), paramName);
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside declareParameter an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calls the other declareParameter, and passes the users input from the guide to one of the other xxx overloads
        /// </summary>
        /// <param name="varDetails"></param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool declareParameter(IAsyncResult varDetails)
        {
            string details = Guide.EndShowKeyboardInput(varDetails);
            return declareParameter(details);
        }
        #endregion
        
        #region Name a function/method

        /// <summary>
        /// Name a method/function to call later
        /// </summary>
        /// <param name="input">Takes the form: "" for example ""/></param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool nameMethod(string input)
        {
            return nameMethod(input, nullAction);
        }

        /// <summary>
        /// Name a method/function to call later
        /// </summary>
        /// <param name="input">Takes the form: "" for example ""/></param>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool nameMethod(string input, Action finishingAction)
        {
            try
            {
                //TODO - ensure that all of these things are done:
                //must do several things:
                //first - check that the button is enabled
                //make the button un-clickable
                //ask the user to enter the name of a function
                //make the "add new parameter variable box & button appear
                //consider disabling the addition of new variables to the current frame (maybe even remove the frames 'add variable' button)

                if (!methodMech.isNotHandlingAnything())
                    throw new Exception("You can't name another method to call until you've finished calling the first one");

                //TODO add checks for validity of user input
                methodMech.passMethodName(input);
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside nameMethod an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calls the other nameMethod, and passes the users input from the guide to one of the other nameMethod overloads
        /// </summary>
        /// <param name="varDetails"></param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool nameMethod(IAsyncResult varDetails)
        {
            string details = Guide.EndShowKeyboardInput(varDetails);
            return nameMethod(details);
        }
        #endregion

        #region Assign parameter a value

        /// <summary>
        /// Takes the value from the hand and assigns it to a named parameter
        /// </summary>
        /// <param name="input">Takes the form: "[parameterName]" for example "x"</param>
        /// <returns></returns>
        public bool assignParameterAValue(string input)
        {
            return assignParameterAValue(input, nullAction);
        }

        /// <summary>
        /// Takes the value from the hand and assigns it to a named parameter
        /// </summary>
        /// <param name="input">Takes the form: "[parameterName]" for example "x"></param>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool assignParameterAValue(string input, Action finishingAction)
        {
            try
            {
                if (input == null)
                {
                    //TODO add null event handling code for when the user cancels input
                }

                //this check is to ensure that no parameters are assigned out of order (or re-assigned to)
                //It's a first in first assigned system
                if (input != methodMech.nextParameterToAssignTo())
                    throw new Exception("You have to assign to parameters in the order they where declared");


               // if (theVariableMode == valueMoveMode.Cut)
                    methodMech.assignValueToParameter(input, thePlayer.takeFromHand());
               // else
               //    methodMech.assignValueToParameter(input, thePlayer.copyFromHand());
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside assignParameterAValue an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calls the other assignParameterAValue, and passes the users input from the guide to one of the other assignParameterAValue overloads
        /// </summary>
        /// <param name="varDetails"></param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool assignParameterAValue(IAsyncResult varDetails)
        {
            string details = Guide.EndShowKeyboardInput(varDetails);
            return assignParameterAValue(details);
        }
        #endregion

        #endregion

        #region Fine grained actions that don't require any sort of input
        
        #region move the calculators expression to the hand

        /// <summary>
        /// Copies or cuts the expression held by the calculator into the hand depending on you mode.
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool moveCalculatorExpToHand()
        {
            return moveCalculatorExpToHand(nullAction);
        }

        /// <summary>
        /// Copies or cuts the expression held by the calculator into the hand depending on you mode.
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the expressions is copied (for example you could pass a void method that refreshes the calculator sprites text)</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool moveCalculatorExpToHand(Action finishingAction)
        {
            try
            {
                if (!theCalculator.isSimplest())
                {
                    Console.WriteLine("The expression is not in it's simplest form, why are you trying to copy that into the hand?");
                    return false;
                }
                //getting here we KNOW that whatever the calculator is holding is NOT and expression becuase it's FSM only goes to simplified mode after suceeding at an evaluation
                
                //attempt an inference of type held by the calculator 
                var inferedType = Calculator.attemptValueTypeInference(theCalculator.getCurrentExpression());
                if(inferedType != theCalculator.currentType)
                    Console.WriteLine("Investigate what went wrong with the calculators type inferencing");

                thePlayer.holdValue(inferedType, theCalculator.getCurrentExpression(), "calculator");

                if (theVariableMode == valueMoveMode.Cut)
                    theCalculator.recieveNewExpression("");

                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside moveCalculatorExpToHand an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        public bool moveCalculatorExpToHand(string pointlessInput, Action finishingAction)
        {
            return moveCalculatorExpToHand(finishingAction);
        }

        #endregion


        #region evaluate the calculators expression, but keep the result

        /// <summary>
        /// Evaluates the expression being held by the caluclator, but keeps the result
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool evaluateCalculatorExpInPlace()
        {
            return evaluateCalculatorExpInPlace(nullAction);
        }

        /// <summary>
        /// Evaluates the expression being held by the caluclator, but keeps the result
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the expressions is evaluated (for example you could pass a void method that refreshes the calculator sprites text)</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool evaluateCalculatorExpInPlace(Action finishingAction)
        {
            try
            {
                //TODO loads of error checking and validation
                //Also make sure that if the value in hand to be evaluated is a strng type or contains no operators that the calculator does not simply return null (this does currently happen with hand content like (exampleString) with ("exampleString") the quotes are smiply stripped off
                //Also these steps may need changing later

                if (theCalculator.ToString() == "")
                {
                    Console.WriteLine("The calculator is blank, so how can you expect it to evaluate?");
                    return false;
                }

                if (!theCalculator.isNewExp())
                    throw new Exception("The calculator has 3 state, if it's in its simplest form or is unsimplifiable, you can't evaluate it further");
               

                if (theVariableMode == valueMoveMode.Cut)
                    theCalculator.evaluateExpression(true);
                else
                    theCalculator.evaluateExpression(false);

                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside evaluateCalculatorExpInPlace an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        public bool evaluateCalculatorExpInPlace(string pointlessInput, Action finishingAction)
        {
            return evaluateCalculatorExpInPlace(finishingAction);
        }

        #endregion

        #region evaluate the calculators expression and pass the result to the hand

        /// <summary>
        /// Evaluates the expression being held by the caluclator and passes the result to the hand
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool evaluateCalculatorExpToHand()
        {
            return evaluateCalculatorExpToHand(nullAction);
        }

        /// <summary>
        /// Evaluates the expression being held by the caluclator and passes the result to the hand
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the expressions is evaluated (for example you could pass a void method that refreshes the calculator sprites text)</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool evaluateCalculatorExpToHand(Action finishingAction)
        {
            try
            {
                //TODO loads of error checking and validation
                //Also make sure that if the value in hand to be evaluated is a strng type or contains no operators that the calculator does not simply return null (this does currently happen with hand content like (exampleString) with ("exampleString") the quotes are smiply stripped off
                //Also these steps may need changing later

                if (theCalculator.ToString() == "")
                {
                    Console.WriteLine("The calculator is blank, so how can you expect it to evaluate ?");
                    return false;
                }

                //If the evaluated expression contains a variable without a value, simply place the unevaluated expression into the hand

                if(theVariableMode == valueMoveMode.Cut)
                    thePlayer.holdValue(theCalculator.evaluateExpression(true)); //place the value spat out from the calculator into the players hand
                else
                    thePlayer.holdValue(theCalculator.evaluateExpression(false)); //place the value spat out from the calculator into the players hand

                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside evaluateCalculatorExpToHand an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        public bool evaluateCalculatorExpToHand(string pointlessInput, Action finishingAction)
        {
            return evaluateCalculatorExpToHand(finishingAction);
        }

        #endregion

        #region Offset from hand To memory manager

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool giveHandMemoryOffsetToMemoryManager()
        {
            return giveHandMemoryOffsetToMemoryManager(nullAction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool giveHandMemoryOffsetToMemoryManager(Action finishingAction)
        {
            try
            {
                string input; //take the value from the players hand as a string

                if (theVariableMode == valueMoveMode.Cut)
                    input = thePlayer.takeFromHand().read(); 
                else
                    input = thePlayer.copyFromHand().read();

                //TODO: add checks that it is in fact a valid offset
                theMemManager.holdOffsetFromPlayer(int.Parse(input));
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside giveHandMemoryOffsetToMemoryManager an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        public bool giveHandMemoryOffsetToMemoryManager(string pointlessInput, Action finishingAction)
        {
            return giveHandMemoryOffsetToMemoryManager(finishingAction);
        }
        #endregion

        #region Garbage collection

        /// <summary>
        /// performs a garbage collection on the items in the heap (anything that isn't being refered to will be removed)
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool garbageCollect()
        {
            return garbageCollect(nullAction);
        }

        /// <summary>
        /// performs a garbage collection on the items in the heap (anything that isn't being refered to will be removed)
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool garbageCollect(Action finishingAction)
        {
            try
            {
                theMemManager.garbageCollection(globalHeap, globalStack);
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside garbageCollect an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        public bool garbageCollect(string pointlessInput, Action finishingAction)
        {
            return garbageCollect(finishingAction);
        }

        #endregion

        #region Output the hand contents/address to the 'console'
        

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool consoleOutputFromHand()
        {
            return consoleOutputFromHand(nullAction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the variable frames)</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool consoleOutputFromHand(Action finishingAction)
        {
            try
            {
                //TODO -> check if the item being held is a reference or not - if it is then we need to find it's value from the heap
                //TODO -> change this to either save the output, or compare it agains't the levels console...or maybe non of these...it seems like the worldTracker and the Form Console don't communicate
                Console.WriteLine(thePlayer.examineHeld().read());
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside consoleOutputFromHand an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        public bool consoleOutputFromHand(string pointlessInput, Action finishingAction)
        {
            return consoleOutputFromHand(finishingAction);
        }



        #endregion

        #region Deleting the newest variable (occurs when scope changes)

        /// <summary>
        /// Removes the variable most recently placed on the stack, this is to show how variables can move out of scope
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool deleteNewestVariable()
        {
            return deleteNewestVariable(nullAction);
        }

        /// <summary>
        /// Removes the variable most recently placed on the stack, this is to show how variables can move out of scope
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the variable frames)</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool deleteNewestVariable(Action finishingAction)
        {
            try
            {
                globalStack.getTopFrame().removeNewestVariable();
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside deleteNewestVariable an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        public bool deleteNewestVariable(string pointlessInput, Action finishingAction)
        {
            return deleteNewestVariable(finishingAction);
        }

        #endregion
        
        #region Give value in hand to the memory manager

        /// <summary>
        /// Take value from hand and give it to the memory manager
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool handValueToMemoryManager()
        {
            return handValueToMemoryManager(nullAction);
        }

        /// <summary>
        /// Take value from hand and give it to the memory manager
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool handValueToMemoryManager(Action finishingAction)
        {
            try
            {
                if (theVariableMode == valueMoveMode.Cut)
                    theMemManager.holdValueFromPlayer(thePlayer.takeFromHand());
                else
                    theMemManager.holdValueFromPlayer(thePlayer.copyFromHand());

                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside handValueToMemoryManager an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        public bool handValueToMemoryManager(string pointlessInput, Action finishingAction)
        {
            return handValueToMemoryManager(finishingAction);
        }

        #endregion

        #region Take a memory value from hand and give it to the memory manage

        /// <summary>
        /// Take a memory value from hand and give it to the memory manager
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool memoryAddressFromHandToMemoryManager()
        {
            return memoryAddressFromHandToMemoryManager(nullAction);
        }

        /// <summary>
        /// Take a memory value from hand and give it to the memory manager
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool memoryAddressFromHandToMemoryManager(Action finishingAction)
        {
            try
            {
                string input; //take the value from the players hand as a string

                if (theVariableMode == valueMoveMode.Cut)
                    input = thePlayer.takeFromHand().read();
                else
                    input = thePlayer.copyFromHand().read();

                if (input == null || input == "")
                {
                    Console.WriteLine("The player is not holding a value, and therefore can't pass one on to the memory manager");
                    return false;
                }

                //TODO: add checks that it is in fact a valid address
                theMemManager.holdMemoryAddressFromPlayer(int.Parse(input));
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside memoryAddressFromHandMemoryManager an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        public bool memoryAddressFromHandToMemoryManager(string pointlessInput, Action finishingAction)
        {
            return memoryAddressFromHandToMemoryManager(finishingAction);
        }

        #region Read value from the heap to the memory manager

        /// <summary>
        /// reads a value from the heap at the held address and stores said value in the memory manager
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool readValueFromHeapToMemMan()
        {
            return readValueFromHeapToMemMan(nullAction);
        }

        /// <summary>
        /// reads a value from the heap at the held address and stores said value in the memory manager
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool readValueFromHeapToMemMan(Action finishingAction)
        {
            try
            {
                theMemManager.holdValueFromMemory(globalHeap);
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside readValueFromHeapToMemManAsAction an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        public bool readValueFromHeapToMemMan(string pointlessInput, Action finishingAction)
        {
            return readValueFromHeapToMemMan(finishingAction);
        }

        #endregion

        #region Read value from the stack into the memory manager

        /// <summary>
        /// reads a value from the stack using its address, and stores the result in the memory manager
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool readValueFromStackToMemMan()
        {
            return readValueFromStackToMemMan(nullAction);
        }

        /// <summary>
        /// reads a value from the stack using its address, and stores the result in the memory manager
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool readValueFromStackToMemMan(Action finishingAction)
        {
            try
            {
                theMemManager.holdValueFromMemory(globalStack);
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside readValueFromStackToMemManAsAction an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        public bool readValueFromStackToMemMan(string pointlessInput, Action finishingAction)
        {
            return readValueFromStackToMemMan(finishingAction);
        }

        #endregion

        #region Write value from memory manager to the heap

        /// <summary>
        /// makes the memory manager write the value it is holding to the address on the heap (which its also holding)
        /// Both the address and the value are removed from the memory manager.
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool writeValueFromMemManToHeap()
        {
            return writeValueFromMemManToHeap(nullAction);
        }

        /// <summary>
        /// makes the memory manager write the value it is holding to the address on the heap (which its also holding)
        /// Both the address and the value are removed from the memory manager.
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool writeValueFromMemManToHeap(Action finishingAction)
        {
            try
            {
                theMemManager.setValueInHeap(globalHeap);
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside writeValueFromMemManToHeap an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        public bool writeValueFromMemManToHeap(string pointlessInput, Action finishingAction)
        {
            return writeValueFromMemManToHeap(finishingAction);
        }

        #endregion

        #region Consume boolean from hand

        public bool consumeBoolFromHand()
        {
            return consumeBoolFromHand(nullAction);
        }

        /// <summary>
        /// Attempts to consume the value in the players hand...will not consume the value if it is not bool
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool consumeBoolFromHand(Action finishingAction)
        {
            try
            {
                if (thePlayer.examineHeld().readType() != "bool")
                    return false;

                thePlayer.removeHeld();
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside writeValueToStackReference an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }
        #endregion

        public bool consumeBoolFromHand(string pointlessInput, Action finishingAction)
        {
            return consumeBoolFromHand(finishingAction);
        }


        #region Write value from memory manager to a non-local stack-frame address

        /// <summary>
        /// makes the memory manager write the value it is holding to the address on the stack (which its also holding)
        /// Both the address and the value are removed from the memory manager.
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool writeValueToStackReference()
        {
            return writeValueToStackReference(nullAction);
        }

        /// <summary>
        /// makes the memory manager write the value it is holding to the address on the stack (which its also holding)
        /// Both the address and the value are removed from the memory manager.
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool writeValueToStackReference(Action finishingAction)
        {
            try
            {
                theMemManager.setValueInStackByReference(globalStack);
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside writeValueToStackReference an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        public bool writeValueToStackReference(string pointlessInput, Action finishingAction)
        {
            return writeValueToStackReference(finishingAction);
        }

        #endregion

        #region Read value from memory manager to player

        /// <summary>
        /// reads value from the the memory manager and gives it to the player (value must already be held by the mem manager)
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool readValueFromMemoryManager()
        {
            return readValueFromMemoryManager(nullAction);
        }

        /// <summary>
        /// reads value from the the memory manager and gives it to the player (value must already be held by the mem manager)
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool readValueFromMemoryManager(Action finishingAction)
        {
            try
            {
                thePlayer.holdValue(theMemManager.returnHeldValue());
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside writeValueToStackReference an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        public bool readValueFromMemoryManager(string pointlessInput, Action finishingAction)
        {
            return readValueFromMemoryManager(finishingAction);
        }

        #endregion

        #region Convert an address and an offset being held by the memory manager to an absolute address

        /// <summary>
        /// Converts the offset and the address being held by the memory manager into an absolute address which can then be written to/read from
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool convertAddressAndOffsetToAbsolute()
        {
            return convertAddressAndOffsetToAbsolute(nullAction);
        }

        /// <summary>
        /// Converts the offset and the address being held by the memory manager into an absolute address which can then be written to/read from
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool convertAddressAndOffsetToAbsolute(Action finishingAction)
        {
            try
            {
                theMemManager.getAbsoluteAddress(globalHeap);
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside writeValueToStackReference an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        public bool convertAddressAndOffsetToAbsolute(string pointlessInput, Action finishingAction)
        {
            return convertAddressAndOffsetToAbsolute(finishingAction);
        }

        #endregion

        #region hand expression to calculator
        public bool handExpressionToCalculator(Action finishingAction)
        {
            if (theVariableMode == valueMoveMode.Cut)
                return enterCalculatorExpressionIO(thePlayer.takeFromHand().read(), finishingAction);
            else
                return enterCalculatorExpressionIO(thePlayer.copyFromHand().read(), finishingAction);
        }

        public bool handExpressionToCalculator()
        {
            if (theVariableMode == valueMoveMode.Cut)
                return enterCalculatorExpressionIO(thePlayer.takeFromHand().read(), nullAction);
            else
                return enterCalculatorExpressionIO(thePlayer.copyFromHand().read(), nullAction);
        }
        #endregion 

        #region Call a method

        /// <summary>
        /// Calls the method being held by the method mechanism
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool callSelectedMethod()
        {
            return callSelectedMethod(nullAction);
        }

        /// <summary>
        /// Calls the method being held by the method mechanism
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the frame and method mechanism )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool callSelectedMethod(Action finishingAction)
        {
            try
            {
                globalStack.push(methodMech.callSelectedMethod());
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside callSelectedMethod an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        public bool callSelectedMethod(string pointlessInput, Action finishingAction)
        {
            return callSelectedMethod(finishingAction);
        }

        #endregion

        #region return from a method

        /// <summary>
        /// returns from the current method/frame
        /// </summary>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool returnFromMethod()
        {
            return returnFromMethod(nullAction);
        }

        /// <summary>
        /// returns from the current method/frame
        /// </summary>
        /// <param name="finishingAction">This is the action that will be performed once the function is completed (for example you could pass a void method that refreshes the )</param>
        /// <returns>Return True of the action was successfull otherwise False</returns>
        public bool returnFromMethod(Action finishingAction)
        {
            try
            {
                globalStack.pop();
                finishingAction();
            }
            catch (Exception e)
            {
                Console.WriteLine("Inside returnFromMethod an exception occured with these deatails:\n{0}", e.ToString());
                return false;
            }
            return true;
        }

        public bool returnFromMethod(string pointlessInput, Action finishingAction)
        {
            return returnFromMethod(finishingAction);
        }

        #endregion

        #endregion

        #endregion

        /// <summary>
        /// Checks if we are on the lowest of the frames (usually main()), and returns true if we are.
        /// This is useful if you beahviours anywhere to change based on the current frame.
        /// </summary>
        /// <returns></returns>
        public bool areOnBaseFrame()
        {
            return globalStack.getStackSize() <= 1;
        }

        public bool isMethodNamed()
        {
            return methodMech.methodName != null && methodMech.methodName != "";
        }

        /// <summary>
        /// returns true if and only if the hand is holding null
        /// </summary>
        /// <returns></returns>
        public bool isHandEmpty()
        {
            return thePlayer.examineHeld() == null;
        }

        public bool isCalculatorEmpty()
        {
            return theCalculator.ToString() == "";
        }

        public int getVariableNumber(int mouseXPos, int mouseYPos)
        {
            //TODO- change this so that the numbers to be used here are pased as parameters
            mouseXPos -= 50;
            mouseYPos /= 90;
            mouseXPos /= 100;
            return (globalStack.getTopFrame().getVariablesList().Count / 4 - mouseYPos) * 4 + mouseXPos;
        }

        public int getVariableNumber(string variableName)
        {
            for (int i = 0; i < globalStack.getTopFrame().getVariablesList().Count; i++)
            {
                if (getNthLocalVariableName(i) == variableName)
                    return i;
            }
            throw new ArgumentException("No such variable in local frame");
        }

        public string getNoneNullHandValue()
        {
            if (thePlayer.examineHeld() == null)
                return "";
            else
                return thePlayer.examineHeld().read();
        }

        public int getTotalLocalFrameVariables()
        {
            return globalStack.getTopFrame().getVariablesList().Count;
        }

        public string getNthLocalVariableAsCompleteString(int variableNumber)
        {
            return getNthLocalVariableType(variableNumber) + " " + getNthLocalVariableName(variableNumber) + ": \n" + getNthLocalVariableValue(variableNumber);
        }

        public string getNthLocalVariableValue(int variableNumber)
        { 
            return globalStack.getTopFrame().getCopyOfValueFromFramesVariable(variableNumber).read();
        }

        public string getNthLocalVariableType(int variableNumber)
        {
            return globalStack.getTopFrame().getCopyOfValueFromFramesVariable(variableNumber).readType();
        }

        public string getNthLocalVariableName(int variableNumber)
        {
            return globalStack.getTopFrame().getVariableXName(variableNumber);
        }

        public string getHandValueAsString()
        {
            try
            {
                return thePlayer.examineHeld().read();
            }
            catch (NullReferenceException)
            {
                thePlayer.holdValue("", "");
                return "";
            }
        }

        public int getStackSize()
        {
            return globalStack.getStackSize();
        }

        public int getParameterCount()
        {
            return methodMech.parameterValues.Count;
        }

        public int getParameterNumber(string parameterName)
        {
            for (int i = 0; i < methodMech.paramNames.Count; i++)
                if (methodMech.paramNames[i] == parameterName)
                    return i;
            return -1;
        }

        public void assignValueToParameter(string parameterName, Value valueToAssign)
        {
            
        }

        public string[] getTheCurrentLevelsMethods()
        {
            return currentLevel.availableMethodSignatures;
        }

        public bool setTheCurrentLevelsMethods(string[] methodSignatures)
        {
            try
            {
                currentLevel.availableMethodSignatures = methodSignatures;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public bool isNextOperation(string proposedOp)
        {
            return proposedOp == currentLevel.getNextOperationCode();
        }

        public bool isLevelComplete()
        {
            return currentLevel.isComplete;
        }
    }
}
