using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMetaphorWorld
{
    public class heapSector
    {
        public int headerAddress;
        public int usedSectors {private set; get; } //the total min-chunks that this sector uses
        int chunkByteSize;
        string descriptor;
        List<Value> contents;//this is the contents of the sector...it could be a tuple, an array or something more complicated like a custom object. It's length does not determine the size required of the heapSector to store it. For example byte[8].memUsage == long[1].memUsage
        int expectedLengthOfContent;

        //ran into something of a problem: a list of int values does not equal the length in memory of a list of, say, doubles...so how do I calculate the required number of heap chunks?
        //I think that a possible solution would be to have a function that stores the size of every primitive value that an object contains, as well as each ones length and translate that, automatically, into 'chunks used'
        //could perhaps store the value as something static inside of the Value class, seeing as the size is a property of the value itself after all
        
        //for a 'simple' object like a list, array or string a simple descriptor would be enough to base everything on...but what about more complex things like custom classes, or objects that have objects as properties
        //for example a person class could have a name (string) and date of birth (system.DateTime)...so this bears thinking about


        //this particular constructor is a first pass at making a heapsector that can contain multiple value types such as a tuple or more complex object. Different parameters might be a good idea
        public heapSector(string chunkHeaderDescription, int addressOfHeader, int minimumSpaceSize, List<heapSector> detailedTypeBreakDown)
        {
            throw new NotImplementedException("the more detailed contructor for heapSector has not been implemented yet");
        }
       
        /// <summary>
        /// This constructor assumes that the object being placed into memory is 'relatively' simple: either a list, an array or a string (Strings are not that simple and might require a constuctor of their own due to immutablity)
        /// </summary>
        /// <param name="chunkHeaderDescription">A distription of the object being represented, such as its name. Eg: randomNumberList</param>
        /// <param name="elementType">This is the type of the structures contents. For example an int[16] would be type int</param>
        /// <param name="totalElementsInObject">The number of elements in the list-like object being created</param>
        /// <param name="addressOfHeader">Where on the heap is this object going to begin</param>
        /// <param name="minimumSpaceSize">the size of a single chunk in memory/on the heap...usually 4 bytes</param>
        public heapSector(string chunkHeaderDescription, string elementType, int totalElementsInObject, int addressOfHeader, int minimumSpaceSize)
        {
            if (chunkHeaderDescription.Contains(' '))
                descriptor = chunkHeaderDescription;
            else
                descriptor = string.Format("{0} {1}[{2}]", chunkHeaderDescription,elementType,totalElementsInObject);
            chunkByteSize = minimumSpaceSize;
            expectedLengthOfContent = totalElementsInObject;
            //Even free space starts with a header to say how much of it there is, however the headers size is determined by the minimum size of cell in the heap
            /* if (isFreeSpace())
            {
                headerSize = 0;
            }
            else
            {
                headerSize = headerByteSize;
            }*/
            
            //the headers address is used to calculate the offset of later elements, but it can't be calculated by the heapSector class itself due to this class being a small part of the heap
            headerAddress = addressOfHeader;

            //usedSectors is how many elements are inside the sector, INCLUDING the header...therefore a minimum length of 1 is possible but meaningless as it is just a header, 2 is the min for something with data
            //it needs to be calculated based on the data types and theirs lengths as well as how large a chunk is  

            contents = new List<Value>(totalElementsInObject);

            if (isFreeSpace())
            {
                usedSectors = 1 + (int)chunkByteSize * totalElementsInObject / minimumSpaceSize;
                
            }
            else
            {
                usedSectors = 1 + (int)Math.Ceiling(((double)Value.getNumberOfBytesForPrimitiveType(elementType) * (double)totalElementsInObject) / (double)minimumSpaceSize);
                assignDefaultValueToAll(Value.getDefaultValueForType(elementType));
            }
                    //headerCHunks + totalRequiredBytes/chunkSize rounded up

            //this is probably a mostly meaningless value now due to the heap being divided into equal sized mini-chunks, and objects can have different sized properties eg: tuple<string,double,int> 
            //elementByteSize = byteSizePerElement;

            //when declaring a list or array in regular code (say int[] x = new int[16]) what value would x[0] have BEFORE being given a value?
            //the answer depends on the type being stored, primitives get a default value while more complex things/objects (that themselves need storing on the heap elseware) get set to null 
  
            
        }

        /// <summary>
        /// This enumerator goes through each of the used chunks in the heapSector and returns an int for each one that reresents something about the whole sectorand/or the current chunk.
        /// This allows for the heapSector to keep the details of it's contents private, but still allow external classes to do things based on the details this enumerator provides, for 
        /// example: you can use it to draw a grid of colours representing the heapsector with colours based on things like "is free", "is memory pointer", "is no longer used" etc...
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> enumerateOverChunks()
        {
            for (int i = 0; i < usedSectors; i++)
            {
                //TODO: insert conditions under which to choose a different value to return. EG: is free, is memory pointer, is no longer used etc...
                yield return descriptor + "\n" + headerAddress;
            }
        }

        /// <summary>
        /// Based on the number of usedChunks in the heap sector as well as its own headerAddress this returns what we expect the subsequent sectors header to be. 
        /// </summary>
        /// <returns></returns>
        public int getExpectedHeaderAddressOfNextSector()
        {
            return headerAddress + usedSectors * chunkByteSize;
        }


        /// <summary>
        /// A function that allows you to determine how suitable a particular chunk of memory is for the object you want to store.
        /// It compares the difference between the avaiable space and the space you require.
        /// </summary>
        /// <param name="chunksRequired">The number of memory chunks you require for your object</param>
        /// <returns>A positive number = how many extra free spaces we have in this sector over and above what you require. 
        /// A negative number is the amount of space we are missing in order to incorporate your required chunks
        /// A zero indicates a perfect fit</returns>
        public int findFreeMemorySpaceDifference(int chunksRequired)
        {
            if (chunksRequired < 1)
                throw new Exception("How can you require 0 or less chunks?");
            if (!isFreeSpace()) //check if the space is in fact free
                return -chunksRequired; //if it isn't free then return a negative version of the chunks required (we are missing that many 'free' chunks after all)

            //if it is free then want the difference between what we have and what you require
            return usedSectors - chunksRequired; //otherwise return the actual difference back to the caller
        }

        /// <summary>
        /// If you want to assign something simple like int[9] this allows you to fill all the spaces with 0's (or whatever default you supply).
        /// This is useful if you don't want to pre-declare an entire object before storing it.
        /// </summary>
        /// <param name="defaultValue"></param>
        public void assignDefaultValueToAll(Value defaultValue)
        {
            if (expectedLengthOfContent == contents.Count)
                for(int i = 0; i < contents.Count;i++)
                    contents[i] = new Value(defaultValue.read(), defaultValue.readType(), "default value");
            else
                while(expectedLengthOfContent > contents.Count)
                    contents.Add(new Value(defaultValue.read(), defaultValue.readType(), "default value"));
        }

        /// <summary>
        /// This method should only be called by the heap, and only when you want to break up a free sector into 2 new sectors (one free and one used)
        /// </summary>
        /// <param name="chunksToShrinkBy"></param>
        public void shrinkFreeSpace(int chunksToShrinkBy)
        {
            if (!isFreeSpace())
                throw new Exception("you can't shrink space that isn't free");
            if (chunksToShrinkBy < 1)
                throw new Exception("You can't shrink by anything less than 1 mini-chunk");
            if (chunksToShrinkBy > usedSectors)
                throw new Exception("You can't shrink by more than this sector has available");
            if (chunksToShrinkBy == 1)
                Console.WriteLine("Shrinking by one might cause you to have only enough space for a header in whatever your shrinking for...just thought I'd warn you");
            
            usedSectors -= chunksToShrinkBy;
        }

        /// <summary>
        /// This method should only be called by the heap, and only when you want to merge 2 free sectors into one largeer sector
        /// </summary>
        /// <param name="bytesToEnlargeBy"></param>
        public void enlargeFreeSpace(int chunksToEnlargeBy)
        {
            if (!isFreeSpace())
                throw new Exception("you can't enlarge space that isn't free");
            if (chunksToEnlargeBy < 1)
                throw new Exception("Enlarging by less than one doesn't make sense, you probably have a mistake somewhere");

            usedSectors += chunksToEnlargeBy;
        }

        /// <summary>
        /// Decides whether the sector in question is demarkated as empty space based on its descriptor: 
        /// </summary>
        /// <returns>Sector descriptors of "free", "free space", "empty", or "empty space" will return true, all else will return false</returns>
        public bool isFreeSpace()
        {
            return descriptor.ToLower() == "free space" || descriptor.ToLower() == "free" || descriptor.ToLower() == "empty" || descriptor.ToLower() == "empty space";
        }


        /// <summary>
        /// This function is assuming that the object this sector represents is an indexable type of equal sizes, such as an array of ints or a list, or a string...if you use it on a more complex
        /// object or tuple it will return an incorrect result.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int getMemoryAddressOfElementX(int index)
        {
            //TODO: add in a *whole lot* of error checks to ensure that this functio is only used on array/list/string types

               //(first address after the header)+ (size of a single element * index)
            return headerAddress + chunkByteSize + Value.getNumberOfBytesForPrimitiveType(contents[0].readType()) * index; //start of first element regardless of its size becuase the header is always the size of a chunk
            
        }

        /// <summary>
        /// Takes a memory address that is within the bound of the sector and finds what offset from the header the address refers to. 
        /// </summary>
        /// <param name="memAddress"></param>
        /// <returns></returns>
        public int getOffsetFromHeaderForMemoryAddress(int memAddress)
        {
            memAddress -= headerAddress;
            if (memAddress < 0)
            {
                throw new Exception("That memory address come from before this sectors header even starts");
            }

            if (memAddress == 0)
            {
                Console.WriteLine("From withing getIndexOfMemoryAddress of heapSector: you are trying to get a memory offset that points to the headers address");
                return -1;
            }

            //getting here we now that memaddress is positive and should be divisble by however many bytes are taken up by a single element
            if (memAddress % chunkByteSize != 0)
                throw new Exception(string.Format("The memory address {0} does not point to a valid element in this sector as each element is of size {1}", memAddress + headerAddress, chunkByteSize));

            return (memAddress - chunkByteSize) / chunkByteSize;

        }

        /// <summary>
        /// Returns a deep-clone of all the values in this heapsector (this essentially allows for getting but not setting, as well as simplifying clone calls)
        /// </summary>
        /// <returns></returns>
        public List<Value> getCopyOfAllValues()
        {
            List<Value> returnList = new List<Value>();
            foreach (Value item in contents)
            {
                returnList.Add(item.clone());
            }
            return returnList;
        }

        /// <summary>
        /// This gets a copy of a value at a particulare location in the content list...this isn't really something you should use unless you know the exact structure of the content list
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Value getCopyOfValueAtOffset(int offset)
        {
            return new Value(contents[offset].read(),contents[offset].readType(), contents[0].readOrigin());
        }

        //use for absolute addressing
        /// <summary>
        /// Currently utilizes getMemoryAddressOfElementX and therefore it is subject to the same restrictions on what data types can be found accurately this way 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public Value getCopyOfValueAtAddress(int address)
        {
            //TODO figure out a way of doing this (hopefully efficiently) that can work on more complicated data structures/object such as tuples
            for (int i = 0; i < contents.Count; i++)
            {
                if (address == getMemoryAddressOfElementX(i))
                    return getCopyOfValueAtOffset(i);
            }
            return null;
        }

        /// <summary>
        /// This allows you to change any value in the content list, again like getCopyOfValueAtOffset, you should only use this if you know the exact structure of the content list. 
        /// AND you are willing to compromise the structure of the sector. For safety reasons if the type of the object you want to write is not the same as the one thats already there
        /// don't match an exception will be thrown
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="offset"></param>
        public void setValueAtOffset(Value newValue, int offset)
        {
            if (newValue.readType() != contents[offset].readType())
                throw new Exception(string.Format("Data types don't match in setValueAtOffset function call, you tried to assign a {0} type value to a {1} type value", newValue.readType(), contents[offset].readType()));
            contents[offset] = newValue;
        }

        public void setValueAtAddress(Value newValue, int address)
        {
            int offset = getOffsetFromHeaderForMemoryAddress(address);
            //TODO: add checks that ensure the passed and returned values are valid

            if (offset < 0 || offset >= contents.Count)
            {
                Console.WriteLine("The address ({0}) that got passed into setValueAtAddress in heapSector got converted into an invalid offset ({1})...we're defaulting to an offset of 0, but you really should look into how this happened");
                offset = 0;
            }

            contents[offset] = newValue;
        }


        public override string ToString()
        {
            if (isFreeSpace())
                return string.Format("Addr::{0} -> Addr::{1} contain a total of {2} free bytes", headerAddress, headerAddress + usedSectors * chunkByteSize, usedSectors * chunkByteSize);
            string asString = string.Format("Addr::{0}: '{1}' contains {2} elements", headerAddress, descriptor, contents.Count);
            for(int i = 0; i < contents.Count;i++)
                asString += "\n        Addr::" + getMemoryAddressOfElementX(i) + "  " + contents[i].readType() + " of value:   " + contents[i].read();
            return asString;
        }

        public void markAsFree()
        {
            descriptor = "free space";
            
            contents = new List<Value>();
            contents.Add(new Value("empty", "",""));
        }

        /// <summary>
        /// Gets the total number of bytes designated to a particular chunk (including the size of its header <0 for free space>)
        /// </summary>
        /// <returns></returns>
        public int getTotalBytesUsedByChunk()
        {
            return usedSectors*chunkByteSize;
        }
    }
}
