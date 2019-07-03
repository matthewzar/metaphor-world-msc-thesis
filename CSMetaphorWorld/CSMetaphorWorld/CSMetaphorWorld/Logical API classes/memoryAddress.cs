using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMetaphorWorld
{
    //think of like a variable that is outside the users immediate access and which doesnt have to contain a value,
    class memoryAddress
    {
        //TODO decide is memory ever has a type...its all just bytes in reality, it might be useful for calculating offsets
        public string type;
        public int address; // a tuple like this -> (frameNo, frameVarNo, memAddrress) might be more usefull. Leaving it as an int for simplicity right now
        Value memsValue;
        public string descriptor = "";
        //TODO add various properties that would allow it to be draw/gamified, eg location, colour, size etc

        /// <summary>
        /// Use to declare a location in memory, becuase all memory is INITIALLY 'empty' no initial value is included in this constructor
        /// </summary>
        /// <param name="theAddress"></param>
        public memoryAddress(int theAddress)
        {
            address = theAddress;
            memsValue = null;
        }

        public bool isEmpty()
        {
            return memsValue == null;
        }


        public void allocateValue(Value theValueToStore, string description = "")
        {
            descriptor = description;
            memsValue = theValueToStore;
        }

        public override string ToString()
        {
            string desc = "";
            if (descriptor != "")
                desc = "  and its marked as " + descriptor;
            if (memsValue == null)
                return "Addr::"+address.ToString() +"  is empty";
            return "Addr::" + address.ToString() + "   contains <" + memsValue.read()+">"+desc;
        }

        
    }
}
