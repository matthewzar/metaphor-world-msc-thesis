using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMetaphorWorld
{
    public class Stack
    {
        List<Frame> stackFrames;
        int topMemAddress;

        public int getStackSize()
        {
            return stackFrames.Count;
        }

        public Stack(string nameOfBaseFrame = "")
        {
            //TODO param error checking
            stackFrames = new List<Frame>();
            topMemAddress = 0;

            if (nameOfBaseFrame != "")
                stackFrames.Add(new Frame(nameOfBaseFrame));

        }

        public int advanceMemoryAddress()
        {
            return advanceMemoryAddress(8);
        }

        public int advanceMemoryAddress(int numberOfBytesToAdvanceBy)
        {
            topMemAddress += numberOfBytesToAdvanceBy;
            return topMemAddress - numberOfBytesToAdvanceBy;
        }

        public int advanceMemoryAddress(string typeToAdvanceBye)
        {
            //TODO ensure that the type being advanced by really is what it claims to be, perhaps place a call to this method after pushes, pops and variable declarations 
            //currently advances stack pointer by arbitrary values
            //TODO give reference types a size of 4 (they are int sized after all)
            switch (typeToAdvanceBye.ToLower())
            {
                case ("int"):
                        return advanceMemoryAddress(4);
                case ("double"):
                        return advanceMemoryAddress(4);
                case ("address"):
                        return advanceMemoryAddress(4);
                case ("methodcall"):
                        return advanceMemoryAddress(8);
                default:
                        return advanceMemoryAddress(8);
            }
        }

        public void push(string nameOfNewFrame)
        {
            push(new Frame(nameOfNewFrame));
        }

        public void push(Frame newFrame)
        {
            if (newFrame == null || newFrame.frameName == "")
                throw new Exception("You can't push a non-existent/nameless method onto the stack");

            advanceMemoryAddress("methodcall"); //advance the memory address to accomodate method header information
            stackFrames.Add(newFrame);
        }

        /// <summary>
        /// returns the top frame WITHOUT removing it from the top of the stack
        /// </summary>
        /// <returns></returns>
        public Frame getTopFrame()
        {
            if (stackFrames.Count >= 1)
                return stackFrames[stackFrames.Count - 1];
            else
                throw new Exception("You can't get a frame from an empty stack");
        }

        /// <summary>
        /// removes the topmost frame from the list
        /// </summary>
        public void pop()
        {
            //TODO add error checking such as whether there are frames left to pop
            if (stackFrames.Count > 1)
                stackFrames.RemoveAt(stackFrames.Count - 1);
            else
                Console.WriteLine("Why did you attempt to pop off a frame when there are none left?");

            //now look at the new top of the stack and attempt to find the address of its 'latest' variable
            topMemAddress = getTopFrame().getAddressOfNewestVariable();
        }


        //for outputting:
        public void consoleWriteContent()
        {
            int x = 0;
            foreach (Frame frame in stackFrames)
            {
                Console.WriteLine("Frame {0} content:",x);
                frame.consoleWriteContent();
                x++;
            }
        }

        public override string ToString()
        {
            string state = "";
            int x = 0;
            foreach (Frame frame in stackFrames)
            {
                state += string.Format("\nFrame {0} ({1}) content:", x,frame.frameName);
                state += frame.ToString();
                x++;
            }
            return state;
        }

        public string getNonLocalsAsString()
        {
            string state = "";
            for(int i = 0; i < stackFrames.Count - 1; i++)
            {
                state += string.Format("\nFrame {0} ({1}) content:", i, stackFrames[i].frameName);
                state += stackFrames[i].ToString();
            }
            state += string.Format("\nFrame {0} ({1}) content:", stackFrames.Count - 1, stackFrames[stackFrames.Count - 1].frameName);
            return state;
        }

        /// <summary>
        /// return formats as a list of [variableName,variableAsString]
        /// </summary>
        /// <returns></returns>
        public List<Tuple<string,string>> getLocalsAsList()
        {
            return stackFrames[stackFrames.Count - 1].getVariablesList();
        }

        /// <summary>
        /// Goes through each variable in each frame looking for ones that are pointer types, and returns a list of all the addresses that are pointed to by the stack 
        /// the returned addresses could be on the stack(passed by reference) but are most likely pointers to the heap
        /// </summary>
        /// <param name="stackToSearch"></param>
        /// <returns></returns>
        public List<int> getListOfContainedReferences()
        {
            var pointersOnTheStack = new List<int>();
            foreach (Frame aFrame in stackFrames)
            {
                foreach (Tuple<string, string> typeValuePair in aFrame.getVariablesTypeValuePairs())
                {
                    //TODO decide which method to use to refer to reference variables (this also needs to be changed in Variable for when an int gets assigned to a reference type)
                    if (typeValuePair.Item1.ToLower().Contains("ptr") ||
                        typeValuePair.Item1.ToLower().Contains("pointer") ||
                        typeValuePair.Item1.ToLower().Contains("ref") ||
                        typeValuePair.Item1[0] == '$' ||
                        typeValuePair.Item1[0] == '*')
                    {
                        try
                        {
                            pointersOnTheStack.Add(int.Parse(typeValuePair.Item2)); //A crash here means that the value (item2) was not a pointer
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("The variable of type {0} containing {1}, is marked as a pointer...pointers are supposed to be in an int format, why isn't this one?", typeValuePair.Item1, typeValuePair.Item2);
                        }
                    }
                }
            }
            return pointersOnTheStack;
        }

        /// <summary>
        /// Goes through the stack frame by frame and returns a copy of the value at a certain address, this method should only be called by the memory manager
        /// </summary>
        /// <param name="memoryAddress"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal Value getCopyOfValueByReference(int memoryAddress)
        {
            Value returnValue = null;
            for (int i = 0; i < stackFrames.Count; i++)
            {
                returnValue = stackFrames[i].getCopyOfValueAtMemoryAddressX(memoryAddress);
                if (returnValue != null)
                {
                    if (i == stackFrames.Count - 1)
                        Console.WriteLine("WARNING, you are trying to access a value through the memory manager that is currently in scope");
                    return returnValue;
                }
            }
            return returnValue; //if this one is reached then no matching address was found and thus a null value will be returned
        }

        internal void setValueAtAddress(Value newValue, int memoryAddress)
        {
            int resultType = 0;
            for (int i = 0; i < stackFrames.Count; i++)
            {
                resultType = stackFrames[i].assignNewValue(memoryAddress, newValue);
                if (resultType == 1) //if the assignment was succesfull: 
                {
                    if (i == stackFrames.Count - 1)
                        Console.WriteLine("WARNING, you are trying to access a value through the memory manager that is currently in scope");
                    return;
                }
            }
            if (resultType == 0)
                throw new Exception("Something went wrong...it could be due to the frame having no variables (if thats the case then change this error code)");
            if (resultType == -1)
                Console.WriteLine("No address found matches {0}, and the value {1} was not assigned to anything", memoryAddress, newValue.ToString()); 
        }
    }
}
