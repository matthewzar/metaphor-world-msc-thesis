using System;
using System.Collections.Generic;

using System.Text;

namespace CSMetaphorWorld
{
    public class Value
    {
        //TODO make string all values of type string begin and end with ' " 's 
        string valueAsString; //also known as actual value
        string type; //values have types too and trying to put a wrongly typed value into a different Variable type should result in a crash
                     //the type can also be used to determine how much space the value takes up in memory
        string state; //may not be neccesary, nice for self tracking eg: am I torn up? am I in Hand? am I in a Variable
        string originLocation;

        public Value(string valAsString, string valueType, string originOfValue)
        {
            //TODO error checking on params

            type = valueType;
            valueAsString = valAsString;
            state = "new";
            originLocation = originOfValue;
        }

        /// <summary>
        /// This overload give the Value a default value based on the type
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="originOfValue"></param>
        public Value(string valueType, string originOfValue)
        {
            //TODO error checking on params

            type = valueType;
            valueAsString = getDefaultValueForType(valueType).read();
            state = "new";
            originLocation = originOfValue;
        }


        public Value clone()
        {
            return new Value(valueAsString, type, originLocation);
        }

        /// <summary>
        /// read the actual value of Value as a string 
        /// </summary>
        /// <returns>content of value as a string</returns>
        public string read()
        {
            return valueAsString;
        }

        public string readType()
        {
            return type;
        }

        public string readOrigin()
        {
            return originLocation;
        }

        /// <summary>
        /// When values come out of the calculator they are marked as unassigned and once they have been placed in a variable they can't be re-assigned, only copied
        /// </summary>
        public void setAsAssigned()
        {
            if (state != "assigned")
                state = "assigned";
            else
                throw new Exception("This value is already assigned and thus cannot be re-assigned");
        }

        /// <summary>
        /// When provided with a data type this function returns the number of bytes that primitive should use in memory
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int getNumberOfBytesForPrimitiveType(string type)
        {
            //TODO: consider if references and pointer types belong in here...they are probably ints and therefore will probably need calculating
            switch (type.ToLower())
            {
                case("bool"):
                case("byte"):
                    return 1;
                case ("short"):
                case ("ushort"):
                    return 2;
                case ("int"):
                case ("uint"):
                    return 4;
                case ("long"):
                case ("ulong"):
                    return 8;
                case ("float"):
                    return 8; //TODO find the actual value for this
                case ("double"):
                    return 16; // TODO find the proper length of a double
                case ("string"):
                    //TODO: consider removing this all together as strings are immutable objects that sit on the heap
                    return 0;
                case ("char"):
                    return 2;//TODO find a proper value for this

            }
            throw new Exception(string.Format("The data type {0} is not a primitive and therefore doesn't have a pre=determined size...try looking up its size from the heap where it belongs", type));
        }

        public static Value getDefaultValueForType(string type)
        {
            
            switch (type.ToLower())
            {
                //TODO: neaten this up so that all that needs to be determined is the default value and then we only need 1 return statement. Do the same for the default size calculator
                    //also add in any other primitives that I may have forgotten 
                    //also inlude things like "Int32"
                case ("bool"):
                    return new Value("false", "bool", "default value");
                case("byte"):
                    return new Value("0", "byte", "default value");
                case ("short"):
                    return new Value("0", "short", "default value");
                case ("ushort"):
                    return new Value("0", "ushort", "default value");
                case ("int"):
                    return new Value("0", "int", "default value");
                case ("uint"):
                    return new Value("0", "uint", "default value");
                case ("long"):
                    return new Value("0", "long", "default value");
                case ("ulong"):
                    return new Value("0", "ulong", "default value");
                case ("float"):
                    return new Value("0.0", "float", "default value");
                case ("double"):
                    return new Value("0.0", "double", "default value");
                case ("string"):
                    return new Value("null", "string", "default value");//TODO think about removing this option as a string is not a primitive and belongs on the heap
                case ("char"):
                    return new Value('\0'.ToString(), "", "default value");
            }
            //the default for any non-primitive (which strictly speaking includes strings) is null as they don't point to anywhere in memory yet...however non-primitives (i.e. objects) are not values, but are in fact
            //sets of values and therefore getting here shoulld perhaps throw an exception or a warning
            throw new Exception(string.Format("The data type {0} is not a primitive and therefore can't be stored inside of the value class, it probably belongs on the heap", type));
        }

        public override string ToString()
        {
            return string.Format("{0} type, with value <{1}>   (origin = {2})", type, valueAsString,originLocation);
        }
    }
}
