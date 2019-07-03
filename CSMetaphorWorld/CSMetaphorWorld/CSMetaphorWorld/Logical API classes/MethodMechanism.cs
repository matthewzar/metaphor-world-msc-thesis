using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMetaphorWorld
{

    public class MethodMechanism
    {
        //This could be done with a list of variables...but parameters aren't really variables and thus need to be treated differently
        public List<string> paramNames;
        public List<Value> parameterValues { private set; get; }
        public string methodName {private set; get;}
       
        public MethodMechanism()
        {
            //create an empty method mechanism
            parameterValues = new List<Value>();
            paramNames = new List<string>();
            methodName = "";
        }

        /// <summary>
        /// Finds the first value whos origin is "defaultparameter" and returns it's name.
        /// </summary>
        /// <returns></returns>
        public string nextParameterToAssignTo()
        {
            for (int i = 0; i < paramNames.Count; i++)
            {
                if (parameterValues[i].readOrigin().ToLower() == "defaultparameter")
                    return paramNames[i];
            }
            return "";
        }

        /// <summary>
        /// Gets the index of the first parameter that has an origin of "defaultparameter" and returns it.
        /// </summary>
        /// <returns></returns>
        public int getIndexOfFirstUnasignedParameter()
        {
            for (int i = 0; i < paramNames.Count; i++)
            {
                if (parameterValues[i].readOrigin().ToLower() == "defaultparameter")
                    return i;
            }
            return -1;
        }

        public void assignValueToParameter(string parameterName, Value valueToAssign)
        {
            for (int i = 0; i < paramNames.Count; i++)
            {
                if (paramNames[i] == parameterName)
                {
                    parameterValues[i] = valueToAssign;
                    return;
                }
            }
        }

        /// <summary>
        /// Takes all the parameters passed and the selected method name and generates a new frame frame with those details 
        /// </summary>
        public Frame callSelectedMethod()
        {
            //Legality checks
            if(methodName == "")
                throw new Exception("You can't call a method with no name...not unless we implement λ functions");
            if (getIndexOfFirstUnasignedParameter() != -1) //check if there are parameters that have not been assigned to 
                throw new Exception("You can't call a method while it has unassigned parameters...fill them all in then try again");

            Frame newFrame = new Frame(methodName, parameterValues, paramNames);
            clearMechanism();
            return newFrame;
        }

        /// <summary>
        /// Takes all the parameters passed and the selected method name and generates a new frame frame with those details 
        /// </summary>
        public void callSelectedMethod(Stack aStack)
        {
            if (methodName == "")
                throw new Exception("You can't call a method with no name...not unless we implement λ functions");
            Frame newFrame =  new Frame(methodName, paramNames, parameterValues, aStack);
            clearMechanism();
        }

        /// <summary>
        /// checks that the method mechanism currently has no parameters and no methodName for calling
        /// </summary>
        /// <returns></returns>
        public bool isNotHandlingAnything()
        {
            return methodName == "" && parameterValues.Count == 0;
        }


        /// <summary>
        /// Much like the contructor this method resets all the information about an instance
        /// </summary>
        private void clearMechanism()
        {
            parameterValues = new List<Value>();
            paramNames = new List<string>();
            methodName = "";
        }

        /// <summary>
        /// Adds a new value to the list of parameters that will eventually be passed when the method is called, this overload
        /// gives the parameter a mostly meaningless name...and thus it is not the best overload to use
        /// </summary>
        /// <param name="newParameter"></param>
        public void passSingleParameter(Value newParameter)
        {
            parameterValues.Add(newParameter);
            paramNames.Add("NameLess Parameter " + paramNames.Count);
        }

        /// <summary>
        /// Adds a new value and name to the list of parameters that will eventually be passed when the method is called
        /// </summary>
        /// <param name="newParameter"></param>
        /// <param name="parameterName"></param>
        public void passSingleParameter(Value newParameter, string parameterName)
        {
            //TODO: check that the parameter name is legal

            //Check that the parameter name is not a duplicate
            foreach (string name in paramNames)
                if (name == parameterName)
                    throw new Exception("You can't have two parameters with the same name");

            parameterValues.Add(newParameter);

            paramNames.Add(parameterName);
        }

        public void passMethodName(string name)
        {
            methodName = name;
        }

        public override string ToString()
        {
            string mechAsString = "Method Mechanism:\n Values being held for passing:";
            foreach (Value x in parameterValues)
            {
                if (x == null)
                    mechAsString += "\n       null";
                else
                    mechAsString += "\n       " + x.ToString();
            }
            if(methodName != "")
                mechAsString += "\n Method to call: " + methodName;
            return mechAsString;
        }



        public string getParameterXName(int variableClicked)
        {
            if (variableClicked < 0 || variableClicked > paramNames.Count)
                throw new Exception("There is no parameter numbered " + variableClicked);
            if (paramNames.Count == 0)
            {
                Console.WriteLine("No parameters to retrieve from");
                return "";
            }
            return paramNames[variableClicked];
        }
    }
}
