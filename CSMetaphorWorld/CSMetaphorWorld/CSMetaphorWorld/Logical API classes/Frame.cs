using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMetaphorWorld
{
    public class Frame
    {
        public string frameName;
        List<Variable> framesVariables;
        List<Value> unassignedParameters;

        //TODO*** put in several checks to ensure that while there are unnassigned parameters the uesr cannot advance

        /// <summary>
        /// This technique bypasses the standard "value in hand to value in variable" technique and should only be used for testing or more advances operations (the end user should not be able to use this)
        /// </summary>
        /// <param name="variableName"></param>
        public void assignUnusedParameterToVariable(string variableName)
        {
            //recieve new variable outside to create a new variable (this happens before the method is called)

            assignNewValue(variableName, unassignedParameters[0]); //take the first unassigned parameter and place it in the variable
            unassignedParameters.RemoveAt(0);

            
        }

        public Value takeFirstUnassignedParameter()
        {
            if (unassignedParameters.Count == 0)
                throw new Exception("You attempted to take a paramameter when there are none to take");
            var temp = unassignedParameters[0];
            unassignedParameters.RemoveAt(0);
            return temp;
        }

        /// <summary>
        /// Goes through each variable and extracts the address on the stack and its type. This information is used by the stack to create a list of pointers
        /// </summary>
        /// <returns></returns>
        public List<Tuple<string,string>> getVariablesTypeValuePairs()
        {
            //could simply change this to "return framesVariables" but then exterior sources would be able to alter the content of the frame without going through the frame itself
            var returnList = new List<Tuple<string,string>>();

            foreach(Variable oneVar in framesVariables)
            {
                returnList.Add(new Tuple<string,string>(oneVar.readType(),oneVar.read()));
            }
            return returnList;
        }

        /// <summary>
        /// A constructor for the frame class, this one should only be used when the method that creates a frame takes no parameters
        /// </summary>
        /// <param name="name"></param>
        public Frame(string methodName)
        {
            unassignedParameters = new List<Value>();
            framesVariables = new List<Variable>();
            frameName = methodName;
        }


        //Technical descision note: decide if the variables should automatically get created or whether the user needs to create each one when they step into the frame for the first time
        //in the event of the second (and more realistic) option (complete user creation) the values and names would have to be stored in some temporary space visible to the user
        //The list of names might not be neccesary for the end user as they would be able to read the code provided and create the neccessary variables, a state flag might make this easier
        //CONCLUSION so far: both methods are valid, but the method used depends on the lesson being tought: an automatic variable creation technique would be suitable for advanced lessons where
        //paramter passing is considered trivial, while the partial technique of users assigning each variable would be more appropriate for people learning to pass paramters
        //Further note: it might be a good idea to create a flag (say bool areAllParamsAssigned) so that we can determine whether it is safe to advance the code (although the length of unassigned variables would to the job)

        /// <summary>
        /// This constructor assumes that when a frame is created the user passes in several unassigned values (but not their names as he/she can read those from the code they are following)
        /// these values are then displayed (say for example hanging on a washing line or floating in the air) and before the user can advance these paramters need to be assigned to their associated
        /// variables. This constructor is meant for lessons where paramater passing is not yet considered a trivial task and is therefore worth going into detail with
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="unassignedValues"></param>
        public Frame(string methodName, List<Value> passedParameterValues, List<string> parameterNames)
        {
            unassignedParameters = passedParameterValues;
            if (passedParameterValues.Count != parameterNames.Count)
                throw new Exception("You tried to create a frame with mismatched paraeter value and name lists");

            framesVariables = new List<Variable>();
            
            for (int i = 0; i < passedParameterValues.Count; i++)
            {
                //TODO: either give this a more meaningful address, or make an overload than will work out the address for itself
                receiveNewVariable(new Variable(parameterNames[i],0,passedParameterValues[i]));
            }
            frameName = methodName;
        }

        /// <summary>
        /// This constructor assumes that when a frame is created the user passes in several unassigned values as well as their associated names (for when they are assigned) and the correct
        /// variables are then automaticaly created, this method is meant for testing purposes and advanced lesson where parameter passing is no longer considered difficult
        /// </summary>
        /// <param name="name"></param>
        /// <param name="emtpyVariableList"></param>
        /// <param name="unassignedValues"></param>
        public Frame(string methodName, List<string> parameterNameList, List<Value> unassignedValues, Stack theStack)
        {
            if (parameterNameList.Count != unassignedValues.Count)
                throw new Exception("Number of passed parameters does not match number of empected parameters");
            int reverseAmount = theStack.advanceMemoryAddress("methodcall");
            //TODO: create all variables and assign all the values
            framesVariables = new List<Variable>();
            for (int i = 0; i < parameterNameList.Count; i++)
            {
                framesVariables.Add(new Variable(parameterNameList[i], theStack.advanceMemoryAddress(unassignedValues[i].readType()), unassignedValues[i]));
            }
            theStack.advanceMemoryAddress(-reverseAmount);
            frameName = methodName;
        }

        public void receiveNewVariable(string varType, string varName, int varAddress, string defaultValue = "")
        {
            receiveNewVariable(new Variable(varType, varName, varAddress, defaultValue));
        }

        public void receiveNewVariable(Variable theNewVariable)
        {
            ////TODO further consistency checks, such as are the contents of theNewVariable acceptible, if its a string/object/array put up a warning saying that it belongs on the heap and that a 
            //pointer will be created by that name etc....
            //Check that "theNewVariable" does not already exist in the frame
            foreach (Variable variable in framesVariables)
            {
                if (variable.name == theNewVariable.name)
                {
                    Console.WriteLine("The variable called '{0}' already exists in this context", variable.name);
                    return;
                }
            }

            //perform a check to see if the new variable is a primitive or an object, if its an object type make it a (null) pointer object of reference type
            if (!isPrimitiveType(theNewVariable.readType()) && !isReferenceType(theNewVariable.readType()))
            {
                Console.WriteLine("For the variable called {0} with a type {1} and contents {2} (marked as type {3}) :",theNewVariable.name,theNewVariable.type,theNewVariable.read(),theNewVariable.readType());
                Console.WriteLine("Non-primitive types are stored on the heap not the stack, instead of creating a {0} type variable we have changed it to a {0}Ref type which will (eventually) point to the heap somewhere\n", theNewVariable.readType());
                theNewVariable.type += "Ref";
                theNewVariable.assignNewValue(new Value("0", "int", theNewVariable.name));
            }
            framesVariables.Add(theNewVariable);
        }

        private bool isReferenceType(string type)
        {
            type = type.ToLower();
            return (type.Contains("ref") || type.Contains("ptr") || type.Contains("pointer") || type[0] == '$' || type[0] == '*');
        }

        private bool isPrimitiveType(string type)
        {
            type = type.ToLower();
            if (new string[] { "short", "int", "long", "float", "double", "int16", "int32", "int64", "uint", "ulong", "ushort", "byte"}.Contains(type))
                return true;
            else
                return false;
        }

        

        public void consoleWriteContent()
        {
            foreach (Variable var in framesVariables)
            {
                var.consoleWriteContent();
            }

        }

        public int getAddressOfNewestVariable()
        {
            if (framesVariables.Count <= 0)
                return 0;
            return framesVariables[framesVariables.Count - 1].getAddress();
        }

        public override string ToString()
        {
            string state = "";
            if (unassignedParameters.Count > 0)
            {
                state += "\n                    Unassigned Passed Parameters:";
                foreach (Value val in unassignedParameters)
                {
                    state += "\n                              " + val.ToString();
                }
            }

            foreach (Variable var in framesVariables)
            {
                state += "\n        " + var.ToString();
            }
            return state;
        }

        /// <summary>
        /// For separation of behaviour the returned string is the name of each variable followed it value converted to a string
        /// </summary>
        /// <returns></returns>
        public List<Tuple<string,string>> getVariablesList()
        {
            List<Tuple<string,string>> variablesTupleList = new List<Tuple<string,string>>();
            foreach (Variable var in framesVariables)
            {
                variablesTupleList.Add(new Tuple<string,string>(var.name,var.ToString()));
            }
            return variablesTupleList;
        }

        /// <summary>
        /// Used to remove the most recently added variable from the frame, it should be used when a variables goes out of scope, not when we return to an earlier frame in the stack (as everything is removed anyway)
        /// </summary>
        public void removeNewestVariable()
        {
            if (framesVariables == null || framesVariables.Count == 0)
                throw new Exception("You attempted to delete a variable from a frame that has no variables");
            framesVariables.RemoveAt(framesVariables.Count - 1);
        }

        private string memoryAddressToVarName(int memAddress)
        {
            for (int i = 0; i < framesVariables.Count; i++)
            {
                if (framesVariables[i].getAddress() == memAddress)
                    return framesVariables[i].name;
            }
            return ""; //this occurs if the memory address is not in the frame
        }

        internal int assignNewValue(int memoryAddress, Value newValue)
        {
            string referencedName = memoryAddressToVarName(memoryAddress);
            if (referencedName == "")
                return -1; //the memory address was not found in the frame
            else
            {
                assignNewValue(referencedName, newValue);
                return 1;
            }
        }

        public string getVariableXName(int variableNumber)
        {
            return framesVariables[variableNumber].name;
        }

        /// <summary>
        /// Looks for an existing variable in the frame and attempts to give it a new value
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="newValue"></param>
        public void assignNewValue(string variableName, Value newValue)
        {
            if (newValue == null)
                return;
            //TODO add checks for correctness such the existense of a variable and non-null value
            bool foundOneFlag = false;
            for (int i = 0; i < framesVariables.Count; i++)
            {
                if (framesVariables[i].name == variableName)
                {
                    if (foundOneFlag == false)
                    {
                        //check that the types being assigned match, 
                        if ((newValue.readType() == framesVariables[i].readType()) || //either the types are the same OR
                            (newValue.readType() == "int" && //the new type is an int and the variables type is some kind of pointer/reference
                            (framesVariables[i].readType().ToLower().Contains("ptr") ||
                            framesVariables[i].readType().ToLower().Contains("pointer") ||
                            framesVariables[i].readType().ToLower().Contains("ref") ||
                            framesVariables[i].readType().ToLower()[0] == '$' ||
                            framesVariables[i].readType().ToLower()[0] == '*')))
                        {
                            foundOneFlag = true;
                            framesVariables[i].assignNewValue(newValue);
                        }
                        else
                            throw new Exception(string.Format("Attempted to assign a value of type {0} to variable {1} which is supposed to be of type {2}", newValue.readType(), variableName, framesVariables[i].readType()));
                            //a common reason for failing this check is attempting to place an unevaluated expression (which is stored as a value) into a proper value 
                        
                    }
                    else
                    {
                        throw new Exception("Frame appears to have more than one variable of the same name");
                    }
                }
            }
        }

        /// <summary>
        /// Looks for an existing variable in the frame and attempts to give it a new value
        /// bases new value type on the type of the old value
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="newValue"></param>
        public void assignNewValue(string variableName, string newValue)
        {
            foreach(Variable var in framesVariables)
            {
                if(var.name == variableName)
                {
                    assignNewValue(variableName, (new Value(newValue, var.readType(), variableName)));
                    break; //break because we only need to compare against the first occurance (any more exist and it indicates a fault)
                }
            }
        }

        public void assignNewValue(int variableNumber, string newValue)
        {
            assignNewValue(framesVariables[variableNumber].name,newValue);
        }


        public Value readVariable(string variableName)
        {
            return getCopyOfValueFromFramesVariable(variableName);
        }

        public Value readVariable(int variableNumber)
        {
            return readVariable(framesVariables[variableNumber].name);
        }

        public Value getCopyOfValueFromFramesVariable(int variableNumber, bool makeOriginEqualVarName = true)
        {
            if (variableNumber >= framesVariables.Count  || variableNumber < 0)
                return null;

            if(makeOriginEqualVarName)
                return new Value(framesVariables[variableNumber].read(), 
                             framesVariables[variableNumber].readType(),
                             framesVariables[variableNumber].name);
            else
                return new Value(framesVariables[variableNumber].read(),
                             framesVariables[variableNumber].readType(),
                             framesVariables[variableNumber].readOrigin()); 
        }

        /// <summary>
        /// Get a COPY (not the origininal) of the variable in question, you have the option to make the copies origin appear as it was or as originating from the variable
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="makeOriginEqualVarName">True will make the origin == the variables name, false will not alter anything</param>
        /// <returns></returns>
        public Value getCopyOfValueFromFramesVariable(string variableName, bool makeOriginEqualVarName = true)
        {
            bool foundOneFlag = false;
            Value result = null;
            for (int i = 0; i < framesVariables.Count; i++)
            {
                if (framesVariables[i].name == variableName)
                {
                    if (foundOneFlag == false)
                    {
                        foundOneFlag = true;
                        if (makeOriginEqualVarName)
                            result = new Value(framesVariables[i].read(), framesVariables[i].readType(), framesVariables[i].name);
                        else
                            result = new Value(framesVariables[i].read(), framesVariables[i].readType(), framesVariables[i].readOrigin()); 
                    }
                    else
                    {
                        throw new Exception("Frame appears to have more than one variable of the same name");
                    }
                }
            }
            if (result == null)
                throw new Exception(string.Format("There was no variable of the name {0} to be found",variableName));
            return result;
        }



        internal Value getCopyOfValueAtMemoryAddressX(int memoryAddress)
        {
            //TODO: add checks to ensure the memory address is in scope
            //add checks to ensure that there are no duplicate variables with the same address
            foreach (Variable x in framesVariables)
            {
                if (x.getAddress() == memoryAddress)
                    return new Value(x.read(), x.readType(), x.name);
            }
            return null; //this happens when the frame does not find a varible at the desigantaed address
        }
    }
}
