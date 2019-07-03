using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;
using System.Diagnostics;

using System.Text.RegularExpressions;

//using myHugeLibrary;
namespace CSMetaphorWorld
{
    public class Calculator
    {
        public string currentType = null;
        string currentExpression;

        //added this state tracker to turn the calculator into an FSM
        enum state
        {
            newExpression, //when we have just entered a new expression, or substituted. It basically memans we aren't sure if it's simplifiable or not
            unsimplifiable, //we have tried evaluating it, and it appears to not change
            simplified //once the evaluate has been called and the result is different, now we are allowed to overwrite it
        }
        state currentState = state.simplified;

        public string getCurrentExpression()
        {
            return currentExpression;
        }

        public override string ToString()
        {
            return currentExpression;
        }

        public bool isSimplest()
        {
            return currentState == state.simplified;
        }

        public bool isNewExp()
        {
            return currentState == state.newExpression;
        }

        public bool isUnsimplifiable()
        {
            return currentState == state.unsimplifiable;
        }

        public void recieveNewExpression(string newExpression)
        {
            if (currentState != state.simplified)
                throw new Exception("You are overwritting an expression that was never fully evaluated. Bad, you're not allowed to do that!");


            //TODO add checks to determine the legality of an expression entered, for example fill all variables with '0' and see if it compiles
            currentExpression = newExpression;

            currentState = state.newExpression;
        }

        public Value evaluateExpression(bool deleteExpressionOnceEvaluated)
        {
            if (currentState == state.unsimplifiable)
                throw new Exception("Why the hell are you trying to simplify an expression that can't be simplified?");

            Tuple<String, String> result = RuntimeCompiler.getAnswerAndTypeOfEquation(currentExpression);
            
            //error checking to determine if there have been any changes (while still having more than a single element) eg "xy+1" is not ok, while "34" is ok

            if (result.Item2.Length > 2 && //check if the result is long enough to have come out as an unchanged string
                result.Item2.Substring(1, result.Item2.Length - 2) == currentExpression)
            {
                result = new Tuple<string, string>("Expression", currentExpression);
                currentState = state.unsimplifiable;
            }
            else
            {
                currentState = state.simplified;
            }

            if (deleteExpressionOnceEvaluated)
                currentExpression = "";
            else
                currentExpression = result.Item2;

            currentType = result.Item1;
            return new Value(result.Item2, result.Item1, "calculator");
        }


        /// <summary>
        /// Goes through the expression held by the calculator and takes out the first string that looks like a variable. 
        /// </summary>
        /// <returns></returns>
        public string getFirstVariableName()
        {

            //throw new NotImplementedException("getFirstVariableName is not implemented yet");
            if (currentExpression == null || currentExpression == "")
            {
                Console.WriteLine("Can't extract a variable from an empty calculator expression");
                return "";
            }



            return null;
        }

        /// <summary>
        /// Substitutes a given value into the first empty variable in the calculators expression
        /// </summary>
        /// <param name="value"></param>
        public void substituteValueIntoExpression(string value)
        {

            substituteValueIntoExpression(getFirstVariableName(), value);
        }


        /// <summary>
        /// Subtitutes a value into the calculator in the place of the first intance of a specified variable
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        public void substituteValueIntoExpression(string variableName, string value)
        {
            if (currentState == state.simplified)
                throw new Exception("This is in it's simplest form, how do you suppose you'll subsitute a value into another value (not a variable)?");

            if (!isLegalVariableName(variableName)) //checks if the variable name is in a legal format
            {
                Console.WriteLine("'{0}' is not a legal variable name, o we're not substituting anything into it", variableName);
                return;
            }

            if (!currentExpression.Contains(variableName)) //checks that there is actually an instance of the variable somewhere
            {
                Console.WriteLine("'{0}' occurs nowhere in the expression '{1}', not even as part of another variables name, so we can't substitute in the value {2}", variableName, currentExpression,value);
                return;
            }

            
            //TODO consider how you would substitute a value into an expression that contains a string (the string might contain the variables name by chance), for example:
            //string example = "variable x = " + x;


            //DONE: make sure this is not going to pick out the variable 'x' in an expression of "six plus six is twelve" AND not the x in a var like 'xs'
            int startPos = getIndexOfVariableName(variableName);
            //currentExpression = currentExpression.Replace(variableName, value);

            if (startPos == -1) //if we counld't find a valid index for variablename then return and don't substitute anything anywhere
                return;

            currentExpression = currentExpression.Remove(startPos, variableName.Length);
            currentExpression = currentExpression.Insert(startPos, value);

            //arguably here we could leave out the change of state back to newExpression,
            //but for all we know the newest substutution might make the expression simplifiable, but not simplest. which means checks must be done
            currentState = state.newExpression;
        }

        /// <summary>
        /// Searches through the current expression, and attempts to find the first index at which a variable name occurs
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        private int getIndexOfVariableName(string variableName)
        {
            if (!currentExpression.Contains(variableName)) //checks that there is actually an instance of the variable somewhere
            {
                Console.WriteLine("'{0}' occurs nowhere in the expression '{1}', not even as part of another variables name", variableName, currentExpression);
                return -1;
            }

            if(variableName == currentExpression)
                return 0;
            
            

            //check if a match is impossible
            if(variableName.Length < 1 || variableName.Length > currentExpression.Length)
                return -1;


            //loop through the entire expression, starting at the very first potential match
            //i will always be the start of the next potential match
            for (int i = currentExpression.IndexOf(variableName); i > -1 && i < currentExpression.Length; i = currentExpression.IndexOf(variableName,i+1))
            {
                var endIndex = i+variableName.Length - 1; //the index of the last names char in the expression 

                //if the match is at the start of the expression
                if (i == 0 && isNonVariableChar(currentExpression[endIndex+1]))
                {
                    return i;
                }
                if (endIndex == currentExpression.Length - 1 && isNonVariableChar(currentExpression[i-1]))
                {
                    return i;
                }
                if (isNonVariableChar(currentExpression[endIndex + 1]) && isNonVariableChar(currentExpression[i - 1]))
                {
                    return i;
                }
                
            }
            return -1;
        }

        private bool isNonVariableChar(char theChar)
        {
            string operators = " /*-+!@#$%^&*()[].";
            return operators.Contains(theChar);
        }

        private static bool isLegalVariableName(string variableName)
        {
            //regular expression-> starts with a letter and ends in any number of letters, numbers or underscores
            //Regex regex = new Regex(@"((?:[\w][\w|\d|_]*&))");
            Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
            
            Match match = regex.Match(variableName);
           
            return match.Success;
        }

        public Calculator()//TODO: think about adding parameters to this constructor in order to limit the functionality/behaviour of the calculator
        {
            currentExpression = "";
        }

        public static string attemptValueTypeInference(string valueAsString)
        {
            return myHugeLibrary.runtimeCompiler.dynamicTypeInferenceToCompilerFormat(valueAsString);
        }

        public static string calculateToString(string theEquation)
        {
            //validity checking of theEquation
            //error checking
            //edge cases:
            if (theEquation.Length == 0) //this is the default behaviour of most calculators
                return "";

            try
            {
                return myHugeLibrary.runtimeCompiler.calculateToString(theEquation);
            }
            catch
            {
                Console.WriteLine("That Appears to be a malformed expression or you haven't inserted values in place of variables");
            }
            return "";
        }

        /// <summary>
        /// takes an equation calcuates the result and creates a returned Value with the result
        /// it makes a best guess about the type of the 'equations' result, currently guesses are either bool, int, double or string 
        /// </summary>
        /// <param name="theEquation"></param>
        /// <returns></returns>
        public static Value calculateToValue(string theEquation) 
        {
            //by calling calculateToString rather than dynamicCalc() I can leave error checking entirely to calculateToString() <may change this>
            Tuple<String, String> result = RuntimeCompiler.getAnswerAndTypeOfEquation(theEquation);
            return new Value(result.Item2, result.Item1, "calculator result");
        }

        /// <summary>
        /// does not guess 
        /// </summary>
        /// <param name="theEquation"></param>
        /// <returns></returns>
        public static Value calculateToValue(string theEquation, string expectedType)
        {
            Tuple<String, String> result = RuntimeCompiler.getAnswerAndTypeOfEquation(theEquation);
            if(result.Item1.ToLower() != expectedType.ToLower())
                Console.WriteLine("WARNING: for equation \"{0}\" the answer was found to be {1}. BUT the type was calculated as being {2}, while the expected type was given as {3}", theEquation, result.Item2, result.Item1, expectedType);
            return new Value(result.Item2, result.Item1, "calculator");
        }

        public static string intToHex(int decimalNum)
        {
            return myHugeLibrary.converter.intToHex(decimalNum);
        }

        

        public static string intToBinary(int decimalNum)
        {
            return myHugeLibrary.converter.intToBinary(decimalNum);
        }

    }
}
