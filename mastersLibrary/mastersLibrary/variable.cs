using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mastersLibrary
{
    public class variable
    {
        public int address;
        public string varType;
        public string value;
        public string name;
        public int scope; //0 -> global 1 -> decalred within the first few linee, 1.1 declared withing a the first non-global structure:
        /*# corrupt [ython-C# pseudo  code:
             def isEven(num):      // 1
                 answer = true;    // 1
                 if(num % 2 != 0): // 1
                    x = 2   //     //1.1
                    answer = false
                 else:             //1
                    x = 2          //1.2
                    answer = true;
                 return answer     //1
              even = isEven(9)  //0
              print(even);
              if(even)
                   x = 3        //0.1 
         */


        public variable(int memoryAddress, string variableType, string varName, string varValue)
        {
            address = memoryAddress;
            varType = variableType;
            value = varValue;
            name = varName;     
        }

        /// <summary>
        /// Takes a string of format <int memAddress>Ʌ<var Type as String>Ʌ<varName as string>Ʌ<var value as string>
        /// </summary>
        /// <param name="varAsString"></param>
        public variable(string varAsString)
        {
            var divisionPositions = new List<int>();
            int count = 0;
            foreach (var x in varAsString)
            {
                if (x.ToString().Equals("Ʌ"))
                {
                    Console.WriteLine("Found a 'Ʌ' at position" + count);
                    divisionPositions.Add(count);
                }
                count++;
            }
            if(divisionPositions.Count != 3)
                throw new Exception("There was an incorrect number of 'Ʌ's in the variable");

            

            int.TryParse(varAsString.Substring(0, divisionPositions[0]), out address);
            varType = varAsString.Substring(divisionPositions[0]+1, divisionPositions[1] - divisionPositions[0]-1);
            name = varAsString.Substring(divisionPositions[1] + 1, divisionPositions[2] - divisionPositions[1] - 1);
            value = varAsString.Substring(divisionPositions[2]+1);
        }
        

        public override string ToString()
        {
            return string.Format("{0} has a value of {1}, it is of type {2}. It is in memory at address {3}",name, value, varType, address);
        }
    }
}
