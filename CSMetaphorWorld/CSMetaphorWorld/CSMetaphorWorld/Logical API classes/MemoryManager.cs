using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMetaphorWorld
{
    public class MemoryManager
    {
        public int heldMemoryAddress{ private set; get; }
        int heldOffset;
        public Value heldValue { private set; get; }
       
        public MemoryManager()
        {
            heldMemoryAddress = -1;
            heldOffset = -1;
            heldValue = null;
        }

        //Note on extensibility && complexification: by not binding the memory manager to a single heap/stack the system can be extended to use multiple heaps and/or stacks,
        //this in turn allows teaching of various architectures and even certain security/permission ideas for example threads share a single heap, but have their own stacks
        //while multiple processes have completely seperate memory (both stack and heap) <whether or not multiple memory managers would be called for I don't know, though I don't think it matters>

        //write to the heap -> needs address & value

        //Tell the heap to allocate X spaces 

        //Garbage collection, looks through the stack for pointer types and compares them to what exists on the stack making a list address that gets sent to the heap for removal
        //Don't forget that a single pointer to the start of a large object may only have its first heap element referenced to on the the stack
        public void garbageCollection(Heap theHeap, Stack theStack)
        {
            var knownAddresses = theStack.getListOfContainedReferences();
            var unusedAddresses = new List<int>();
            var heapsMem = theHeap.getNon_FreeHeaderAddresses();
            for (int heapIndex = 0; heapIndex < heapsMem.Count; heapIndex++)
            {
                if (!knownAddresses.Contains(heapsMem[heapIndex]))
                
                    unusedAddresses.Add(heapsMem[heapIndex]); //make sure to also add the end element
                 
                
            }
            theHeap.garbageCollect(unusedAddresses);
        }

        /// <summary>
        /// Asks the memory manager to please go to the heap, look the chunk starting at memoryAddress, and retrieve the value at a particular offset
        /// </summary>
        /// <param name="theHeap"></param>
        /// <param name="memoryAddress"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Value getCopyOfValueInHeap(Heap theHeap, int memoryAddress, int offset = 0)
        {   //TODO add bounds checking
            return theHeap.getCopyOfValue(memoryAddress, offset);
        }

        /// <summary>
        /// An overload of getCopyOfValueInHeap which relies on the memory manager being given addresses and offsets by the player to hold
        /// </summary>
        /// <param name="theHeap"></param>
        /// <returns></returns>
        public Value getCopyOfValueInHeap(Heap theHeap)
        {
            if (heldMemoryAddress == -1 || heldOffset == -1)
                throw new Exception("The memory manager is either not holding a memory adress or an offset and thus you crashed");
            if (heldValue != null)
                Console.WriteLine("WARNING: You overwrote the value currently being held my the mem manager of {0}",heldValue.ToString());

            try
            {
                return getCopyOfValueInHeap(theHeap, heldMemoryAddress, heldOffset);
            }
            finally
            {   //Perform some clean up (that occurs before a temporary variable is returned) so that the memory manager is no longer holding anything
                heldValue = null;
                heldOffset = -1;
                heldMemoryAddress = -1;
            }
        }

        /// <summary>
        /// Asks the memory manager to please go to the heap, look the chunk starting at memoryAddress, and retrieve the value at a particular offset
        /// </summary>
        /// <param name="theHeap"></param>
        /// <param name="memoryAddress"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Value getCopyOfValueInStack(Stack theStack, int memoryAddress)
        {   //TODO add bounds checking
            return theStack.getCopyOfValueByReference(memoryAddress);
        }


        public Value getCopyOfValueInStack(Stack theStack)
        {
            if (heldMemoryAddress == -1)
                throw new Exception("The memory manager is not holding a memory adress thus you crashed");
            if (heldValue != null)
                Console.WriteLine("WARNING: You overwrote the value currently being held my the mem manager of {0}", heldValue.ToString());
            if (heldOffset != 0 && heldOffset != -1)
                Console.WriteLine("WARNING: A reference to the stack cannot have an offset of more than 0 but you have one of size {0}, we'll just assume it was a mistake and ignore it", heldOffset);

            try
            {
                return getCopyOfValueInStack(theStack, heldMemoryAddress);
            }
            finally
            {   //Perform some clean up (that occurs before a temporary variable is returned) so that the memory manager is no longer holding anything
                heldValue = null;
                heldOffset = -1;
                heldMemoryAddress = -1;
            }
        }

        internal void setValueInStackByReference(Stack theStack)
        {
            if (heldMemoryAddress == -1 || heldValue == null)
                throw new Exception("The memory manager is either not holding a memory adress or a value and thus you crashed");
            if(heldOffset != -1 && heldOffset != 0)
                Console.WriteLine("WARNING: A reference to the stack cannot have an offset of more than 0 but you have one of size {0}, we'll just assume it was a mistake and ignore it", heldOffset);

            setValueInStackByReference(theStack, heldValue, heldMemoryAddress);

            //perform some cleanup so that the memory manager is no longer holding anything
            heldValue = null;
            heldOffset = -1;
            heldMemoryAddress = -1;
            
        }

        internal void setValueInStackByReference(Stack theStack, Value newValue, int memoryAddress)
        {
            theStack.setValueAtAddress(newValue, memoryAddress);   
        }

        public void setValueInHeap(Heap theHeap, Value newValue, int memoryAddress, int offset = 0)
        {//TODO add bounds checking
            if (offset == -1)
                theHeap.setValueAtLocation(newValue, memoryAddress);
            else
                theHeap.setValueAtLocation(newValue, memoryAddress, offset);
        }

        /// <summary>
        /// makes the memory manager write the value it is holding to the address (which its also holding) on the heap 
        /// Both the address and the value are removed from the memory manager.
        /// </summary>
        /// <param name="theHeap"></param>
        public void setValueInHeap(Heap theHeap)
        {
            if (heldMemoryAddress == -1 || heldValue == null)
                throw new Exception("The memory manager is either not holding a memory adress, or a value and thus you crashed");

            setValueInHeap(theHeap, heldValue, heldMemoryAddress, heldOffset);

            //perform some cleanup so that the memory manager is no longer holding anything
            heldValue = null;
            heldOffset = -1;
            heldMemoryAddress = -1;
            
        }

        public void holdMemoryAddressFromPlayer(int memoryAddress)
        {
            if (heldMemoryAddress != -1)
                Console.WriteLine("WARNING: You overwrote the memory address currently being held my the mem manager of {0}", heldMemoryAddress);
            heldMemoryAddress = memoryAddress;
        }

        public void holdOffsetFromPlayer(int offset)
        {
            if (heldOffset != -1)
                Console.WriteLine("WARNING: You overwrote the offset currently being held my the mem manager of {0}", offset);
            heldOffset = offset;
        }

        public void holdValueFromPlayer(Value value)
        {
            if (value == null)
            {
                Console.WriteLine("You can't send a null value to the memory manager");
                return;
            }
            if (heldValue != null && value != null)
                Console.WriteLine("WARNING: You overwrote the value currently being held my the mem manager of {0}", value.ToString());
            heldValue = value;
        }

        public int getActualMemoryAddressFromHeaderAndOffset(Heap theHeap)
        {
            return theHeap.getMemoryAddressAtLocation(heldMemoryAddress, heldOffset);
        }

        public void holdValueFromMemory(Heap theHeap)
        {
            if (heldMemoryAddress == -1)
            {
                Console.WriteLine("How are you expecting me to fetch a value from memory that has no address?");
                return;
            }

            if (heldOffset == -1)
            {
                Console.WriteLine("You haven't got an offset so we're assuming you want an absolute adress");
                heldValue = theHeap.getCopyOfValue(heldMemoryAddress);
            }
            else
                heldValue = theHeap.getCopyOfValue(heldMemoryAddress, heldOffset);
            
            //cleanup so that the manager is no longer holding an address or offset
            heldOffset = -1;
            heldMemoryAddress = -1;
        }

        public void holdValueFromMemory(Stack theStack)
        {
            heldValue = theStack.getCopyOfValueByReference(heldMemoryAddress);
            //cleanup so that the manager is no longer holding an address or offset
            heldOffset = -1;
            heldMemoryAddress = -1;
        }

        /// <summary>
        /// Removes the value being held by the memory manager and returns it
        /// </summary>
        /// <returns></returns>
        public Value returnHeldValue()
        {
            try
            {
                if (heldValue == null)
                    Console.WriteLine("WARNING: The memory managaer is not holding a value, are you sure you meant to take a null value from him?");
                
                return heldValue;
            }
            finally
            {   //Perform some clean up (that occurs before a temporary variable containing the value is returned) so that the memory manager is no longer holding anything
                heldValue = null;
                heldOffset = -1;
                heldMemoryAddress = -1;
            }
        }

        public void getAbsoluteAddress(Heap theHeap)
        {
            heldMemoryAddress = theHeap.convertHeaderAndOffsetToAbsolute(heldMemoryAddress,heldOffset);
            heldValue = null;
            heldOffset = -1;
        }


        public override string ToString()
        {
            string asString = "Memory Manager:\n";
            asString += (heldMemoryAddress == -1) ? "Is holding no address\n" : string.Format("Is holding the address: {0}\n", heldMemoryAddress);
            asString += (heldOffset == -1) ? "Is holding no offset\n" : string.Format("Is holding the offset: {0}\n", heldOffset);
            asString += (heldValue == null) ? "Is holding no value" : string.Format("Is holding the value: {0}", heldValue.ToString());
            return asString;
        }

    }
}
