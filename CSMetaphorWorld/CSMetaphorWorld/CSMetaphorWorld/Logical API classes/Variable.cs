using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMetaphorWorld
{
    public class Variable
    {
        public string name;
        public string type;
        int address; // a tuple like this -> (frameNo, frameVarNo, memAddrress) might be more usefull. Leaving it as an int for simcity right now
        Value varsValue;
        bool hexMemMode = false;



        public Variable(string varType, string varName,  int varAddress, string defaultValue = "")
        {
            //checking legality of various paramters
            
            
            name = varName;
            type = varType;
            address = varAddress;
            //if the defaultValue was left out try and figure it out based on the type, if you can't then leave it blank or make it null
            #region defaultValue Evaluation (potentially a pointless parameter as VS does not allow unnassigned variable to be used);
            if (defaultValue == "") //default values might 
            {
                switch(type.ToLower())
                {
                    case ("int"): case ("int16"): case ("int32"): case ("int64"):
                    case ("uint"): case ("uint16"): case ("uint32"): case ("uint64"):
                    case ("sbyte"): case ("byte"): case ("ushort"): case ("short"):
                    case ("long"): case ("ulong"):
                        {
                        defaultValue = "0";
                        break;
                        }
                    case ("float"): case ("double"):

                        {
                            defaultValue = "0.0";
                            break;
                        }
                    case ("bool"): case ("boolean"):
                        {
                            defaultValue = "false";
                            break;
                        }
                    default:
                        {
                            defaultValue = "null";
                            break;
                        }
                }
            }
            #endregion 

            varsValue = new Value(defaultValue, varType, name);

        }

        public string readOrigin()
        {
            return varsValue.readOrigin();
        }

        public int getAddress()
        {
            return address;
        }

        public Variable(string varName, int varAddress, Value varValue)
        {
            name = varName;
            type = varValue.readType();
            address = varAddress;
            varsValue = varValue;
        }

        public string read()
        {
            return varsValue.read();
        }

        public string readType()
        {
            //checking for consistency
            //either the types are the same OR the one is a reference and the value is an int
            //TODO: consider throwing an exceptions no matter what if the type don't match
            if (varsValue.readType() != type)
            {
                //Is it something other than a reference type?
                if (!type.ToLower().Contains("ptr") &&
                    !type.ToLower().Contains("pointer") &&
                    !type.ToLower().Contains("ref") &&
                    !type.ToLower().Contains("$") &&
                    !type.ToLower().Contains("*"))
                    throw new Exception(string.Format("The values type({0}) and the variables type{1} do not match", varsValue.readType(), type));
                else
                {
                    //TODO ensure that if the values type is not int that it still gets flagged as bad (memory shouldnt be refered to by anything except integers)
                    if (varsValue.readType().ToLower() == "int")
                    {
                        Console.WriteLine("The value contained in variable {0} is of type {1} (should be int), while the variable is a reference type ({2})", name, varsValue.readType(), type);
                        Console.WriteLine("This is acceptable only because references are numbers/addresses but we thought you should know just in case");
                        Console.WriteLine("we're going to cast it to a reference type");
                        varsValue = new Value(varsValue.read(), type, varsValue.readOrigin());
                        return type; 
                    }
                    else
                        throw new Exception(string.Format("The values type({0}) and the variables type{1} do not match", varsValue.readType(), type));

                    
                }
            }
            //TODO decide whether to return values type or the variables type (which are usually the same)...or maybe return the values type when the variable IS NOT a reference
            return varsValue.readType();//this allows the user to read the type of the value not the type of the variable
            //the only time varsValue.readType() should deviate from the variables type is when the variable is a reference
        }

        public void consoleWriteContent()
        {   //address    int x -> 9    
            Console.WriteLine("{0}\t\t{1} {2} -> {3}",address.ToString(),type,name,varsValue.read());
        }

        public override string ToString()
        {
            if(hexMemMode)
                return string.Format("Addr::{0}      ({1}) {2} <- {3}", Calculator.intToHex(address), type, name, varsValue.read());
            else
                return string.Format("Addr::{0}      ({1}) {2} <- {3}", address.ToString(), type, name, varsValue.read());
        }

        public void assignNewValue(Value newVal)
        {
            varsValue = newVal;
        }
    }
}
