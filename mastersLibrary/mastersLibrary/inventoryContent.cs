using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mastersLibrary
{
    public class inventoryContent
    {
        //TODO:
        ///Add check for duplicate variable name or address when adding a new variable
        ///Add variable interactions that don't require 2 variables, for example: myVar * 2 OR pow(myVar,3)
        ///
        //
        // Create a new linked list object instance.
        //
        public LinkedList<variable> variables;

        /// <summary>
        /// creates an empty inventory (no variables either global or local))
        /// </summary>
        public inventoryContent()
        {
            variables = new LinkedList<variable>();
        }

        /// <summary>
        /// Takes a list of variables of the form [variable]¦[variable]¦[variable]¦
        /// one '¦' at the END of each variable means that if you have no variables to store then listOfVariables should equal "", and NOT "¦"
        /// </summary>
        /// <param name="listOfVariables">A string of the form [stringVariable]¦[stringVariable]¦[stringVariable]¦</param>
        public inventoryContent(string listOfVariables) 
        {
            variables = new LinkedList<variable>(); //declare the initial list as an empty list of variables
            int currentPos, previousPos = 0, count = 0;  //decalre some place holding variables
            currentPos = listOfVariables.IndexOf('¦');   //find the location of the end of the first variable
            foreach (var x in listOfVariables)           //go through every character of the list of variables
            {
                if(x == '¦')                            //check if the current location is at the end of a variable
                {
                    variables.AddLast(new variable(listOfVariables.Substring(previousPos, count - previousPos))); //cut out a substring that represents the most recent variable
                    currentPos = count;
                    previousPos = currentPos+1; //change the values of our place holder variables
                    
                }
                count++;
            }
        }

        public void listVariables()
        {

            // Loop through the linked list with the foreach-loop.
            foreach (var item in variables)
            {
                Console.WriteLine(item.ToString());
            }
        }

        public void assignValueToVariable(string variableName, string newValue)
        {
            for (int i = 0; i < variables.Count; i++)
            {
                if (variables.ElementAt(i).name.Equals(variableName))
                {
                    variables.ElementAt(i).value = newValue;
                }
            }   
        }

        public void assignValueToVariableFromVars(string assignToName, string assignFromName)
        {
            assignValueToVariableFromVars(assignToName, assignFromName, "", "+");
        }

        public void assignValueToVariableFromVars(string assignToName, string assignFromName1, string assignFromName2, string operation)
        {
            for (int i = 0; i < variables.Count; i++)
            {
                if (variables.ElementAt(i).name.Equals(assignToName))
                {
                    variables.ElementAt(i).value = variableInteraction(assignFromName1,assignFromName2,operation);
                }
            }  
        }

        /// <summary>
        /// To be used later with operations that can act on a single variable, eg var^2
        /// </summary>
        /// <param name="var1Name"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public string variableInteraction(string var1Name, string operation)
        {
            return variableInteraction(var1Name, "", operation);
        }

        public string variableInteraction(string var1Name, string var2Name, string operation)
        {
            variable tempVar1 = null;
            variable tempVar2 = null;

            foreach (var item in variables)
            {
                if (item.name.Equals(var1Name))
                {
                    tempVar1 = item;
                    break;
                }
            }

            if (var2Name.Equals(""))
            {
                return variableInteraction(tempVar1.value, tempVar1.varType, "", "string", operation);
            }

            foreach (var item in variables)
            {
                if (item.name.Equals(var2Name))
                {
                    tempVar2 = item;
                    break;
                }
            }

            if (tempVar1 == null)
            {
                Console.WriteLine("Problem searching for variable "+ var1Name);
            }
            if (tempVar2 == null)
            {
                Console.WriteLine("Problem searching for variable " + var2Name);
            }

            return variableInteraction(tempVar1.value, tempVar1.varType, tempVar2.value, tempVar1.varType, operation);
        }

        /// <summary>
        /// takes 2 variables and an operator and returns the VALUE of the operation between the 2 variables values as a string
        /// this is if you know the Variable in question
        /// </summary>
        /// <param name="var1"></param>
        /// <param name="var2"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public string variableInteraction(variable var1, variable var2, string operation)
        {
            return variableInteraction(var1.value, var1.varType, var2.value, var2.varType, operation);
        }

        public void addNewVariable(int address, string varType, string varName, string varValue)
        {
            foreach (var item in variables)
            {
                if (item.name.Equals(varName))
                {
                    throw new Exception("Variable already exists");
                }
            }
            variables.AddLast(new variable(address, varType, varName, varValue));
        }


        private string variableInteraction(string value1, string var1Type  ,string value2, string var2Type, string operation)
        {
            if(var1Type.ToUpper().Contains("STRING"))
            {
                #region var1 type is string
                if (var2Type.ToUpper().Contains("STRING") || 
                    var2Type.ToUpper().Contains("INT") ||
                    var2Type.ToUpper().Contains("DOUBLE")) //known cases (everything is treated as a string
                {
                    return stringAnything(value1,value2,operation);
                }

                //unkowns default to:
                Console.WriteLine("Unkown Variable Type for value2, value1 was a string");
                return "";

                #endregion
            }
            if (var1Type.ToUpper().Contains("INT"))
            {
                #region var1 type is int
                if (var2Type.ToUpper().Contains("STRING"))
                {
                    return stringAnything(value1,value2, operation);
                }
                if (var2Type.ToUpper().Contains("INT"))
                {
                    return (int.Parse(numNum(value1,value2,operation).ToString())).ToString();
                }
                if (var2Type.ToUpper().Contains("DOUBLE"))
                {
                    return (double.Parse(numNum(value1, value2, operation).ToString())).ToString();
                }

                //unkowns default to:
                Console.WriteLine("Unkown Variable Type for value2, value1 was a string");
                return "";

                #endregion
            }
            if (var1Type.ToUpper().Contains("DOUBLE"))
            {
                #region var1 type is double
                if (var2Type.ToUpper().Contains("STRING"))
                {
                    return stringAnything(value1, value2, operation);
                }
                if (var2Type.ToUpper().Contains("INT"))
                {
                    return (double.Parse(numNum(value1, value2, operation).ToString())).ToString();
                }
                if (var2Type.ToUpper().Contains("DOUBLE"))
                {
                    return (double.Parse(numNum(value1, value2, operation).ToString())).ToString();
                }

                //unkowns default to:
                Console.WriteLine("Unkown Variable Type for value2, value1 was a string");
                return "";

                #endregion
            }
            
            //unkowns default to:
            Console.WriteLine("Unkown Variable Type for value1");
            return "";
        }

        private string stringAnything(string value1, string value2, string operation)
        {
            if (operation.Equals("+"))
            {
                return value1 + value2;
            }

            Console.WriteLine("Unkown string operator:" + operation);
            return "ERROR IN stringAnything";
        }

        private double numNum(string value1, string value2, string operation)
        {
            double val1 = double.Parse(value1);
            double val2 = double.Parse(value2);


            switch (operation.ToUpper())
            {
                case ("+"):
                    {
                        return (val1 + val2);
                    }
                case ("-"):
                    {
                        return (val1 - val2);
                    }
                case ("*"):
                    {
                        return (val1 * val2);
                    }
                case ("/"):
                    {
                        return (val1 / val2);
                    }
            }

            Console.WriteLine("Error in numNum");
            return 0;
        }

        public List<string> ToStringList()
        {
            var tempList = new List<string>();
            foreach (variable x in variables)
            {
                tempList.Add(x.ToString());
            }
            return tempList;
        }

       

        
    }


}
