using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMetaphorWorld
{
    public class Heap
    {
        public List<heapSector> heapChunks {get; private set;}
        int headerSize;
        public int heapStartInMemory;
        string memoryAssignmentTechnique = "best fit"; //option include "best fit" and "first fit"
        int minHeapChunkSize = 4;

        public Value getCopyOfValue(int chunkAddress, int offset)
        {
            foreach (heapSector chunk in heapChunks)
                if (chunk.headerAddress == chunkAddress)
                    return chunk.getCopyOfValueAtOffset(offset);
            throw new Exception(string.Format("The memory address {0} is not a recognized header address", chunkAddress));
        }

        /// <summary>
        /// Creates a deep-clone of the content of each heapChunk and return it as a list. This allows for reading but not writing 
        /// </summary>
        /// <returns>A list of each sector, each sector contains a list of values</returns>
        public List<List<Value>> getCopyOfAllValuesInAllSectors()
        {
            List<List<Value>> returnLists = new List<List<Value>>();
            foreach (heapSector item in heapChunks)
            {
                returnLists.Add(item.getCopyOfAllValues());
            }
            return returnLists;
        }

        internal Value getCopyOfValue(int heldMemoryAddress)
        {
            int index = 0;
            foreach (heapSector chunk in heapChunks)
            {
                if (chunk.headerAddress == heldMemoryAddress)
                    throw new Exception(string.Format("The memory address {0} is a header not an absolute address", heldMemoryAddress));
                
                
                //check if we've gone past wherever the memory address is supposed to be sitting
                if (chunk.headerAddress > heldMemoryAddress)
                {
                    index--;
                    break;//the previous chunk must therefore contains the required address, so break out and search that chunk
                }
                index++;
            }

            Value returnVal = heapChunks[index].getCopyOfValueAtAddress(heldMemoryAddress);
            if (returnVal != null)
                return returnVal;
            
            //getting this far means that the reequested address could not be found
            throw new Exception(string.Format("The memory address {0} is not a recognized address", heldMemoryAddress));
        }

        public void setValueAtLocation(Value newValue, int chunkAddress, int offset)
        {
            for(int i = 0; i < heapChunks.Count; i++)
                if (heapChunks[i].headerAddress == chunkAddress)
                {
                    heapChunks[i].setValueAtOffset(newValue,offset);
                    return;//There should only be one of each address --TODO: consider looping to the very end in order to check for duplicates
                }
            throw new Exception(string.Format("The memory address {0} is not a recognized header address", chunkAddress));
        }

        public void setValueAtLocation(Value newValue, int absoluteAddress)
        {
            int index = 0;
            foreach (heapSector chunk in heapChunks)
            {
                if (chunk.headerAddress == absoluteAddress)
                    throw new Exception(string.Format("The memory address {0} is a header not an absolute address", absoluteAddress));


                //check if we've gone past wherever the memory address is supposed to be sitting
                if (chunk.headerAddress > absoluteAddress)
                {
                    index--;
                    break;//the previous chunk must therefore contains the required address, so break out and search that chunk
                }
                index++;
            }
            
            if (index >= 0 && index < heapChunks.Count)
            {
                heapChunks[index].setValueAtAddress(newValue, absoluteAddress);
                return;
            }
            //getting this far means that the reequested address could not be found
            throw new Exception(string.Format("The memory address {0} is not a recognized address", absoluteAddress));
        }

        public Heap(int maxBytes = 800, int startOfHeapInMemory = 10000)
        {//TODO: consider additional paramteres
            heapStartInMemory = startOfHeapInMemory;

            heapChunks = new List<heapSector>();
            heapChunks.Add(new heapSector("free space", "free", maxBytes / minHeapChunkSize, startOfHeapInMemory, minHeapChunkSize)); 
        }

        private void assignNewChunk(string chunkDescription, int totalElements, int byteSizePerElement, Value defaultValue,string fittingTechnique = "first fit")
        {
            int freeChunkIndex = 0;
            switch (fittingTechnique)
            {
                case("first fit"):
                    freeChunkIndex = findFirstFitIndex(totalElements * byteSizePerElement / minHeapChunkSize + 1);
                    break;
                case("best fit"):
                    freeChunkIndex = findBestFitIndex(totalElements * byteSizePerElement / minHeapChunkSize + 1);
                    break;
                default:
                    findFirstFitIndex(totalElements * byteSizePerElement / minHeapChunkSize + 1);
                    break;
            }

            splitFreeChunk(freeChunkIndex, chunkDescription, totalElements, defaultValue.readType());
          //  heapChunks[freeChunkIndex].assignDefaultValueToAll(defaultValue);
        }


        private void splitFreeChunk(int freeSectorIndex, string chunkDescription, int totalElements, string elementType)
        {
            
            int addressOfNewHeader = heapChunks[freeSectorIndex].headerAddress;
            
            //insert a new chunk at the start of the free chunk you want to split
            heapChunks.Insert(freeSectorIndex, new heapSector(chunkDescription, elementType, totalElements, addressOfNewHeader, minHeapChunkSize)); //inserts the new chunk into the start of the old free space chunk
            
            //once here the heap is a little larger than its usual max


            //now at position (chunkIndex+1) the old (unchanged) free space is still sitting
            //check if the size of the space being fitted into was exact, if it is then remove that 'free' space, don't shrink it
            int freeChunkDifference = heapChunks[freeSectorIndex + 1].findFreeMemorySpaceDifference(totalElements * Value.getNumberOfBytesForPrimitiveType(elementType) / minHeapChunkSize + 1);
            if (freeChunkDifference == 0)
            {
                heapChunks.RemoveAt(freeSectorIndex + 1);
                return;
            }
            if(freeChunkDifference > 0)
            {
                heapChunks[freeSectorIndex + 1].shrinkFreeSpace(totalElements * Value.getNumberOfBytesForPrimitiveType(elementType) / minHeapChunkSize + 1);
                heapChunks[freeSectorIndex + 1].headerAddress = heapChunks[freeSectorIndex].getExpectedHeaderAddressOfNextSector();
                return;
            }
            throw new Exception("Attempted a to assign overlarge object to undersized free space in splitFreeChunk");

        }

        

        //These memory fitting algorithms allow the memory manager to select regions of memory to use based on different criteria
        //This sort of thing would allow the game to be extended to a 3rd year OS level by making the user manage the memory themselves based on what the selected algorithm is (proof of extensibility/complexification)
        #region memory fitting algorithms
        private int findFirstFitIndex(int totalChunksRequired)
        {
            for (int i = 0; i < heapChunks.Count; i++)
            {
                if (heapChunks[i].findFreeMemorySpaceDifference(totalChunksRequired) >= 0) // check if what we need can fit (regardless of how well)
                    return i;
            }
            Console.WriteLine("Inside Heap you asked for free space of {0} bytes...a chunk that large could not be found, try garbage collection to compact the free space", totalChunksRequired);
            return -1;
        }

        private int findBestFitIndex(int totalChunksRequired)
        {
            int smallestGap = int.MaxValue;
            int bestIndex = -1;
            for (int i = 0; i < heapChunks.Count; i++)
            {
                if (heapChunks[i].isFreeSpace() && heapChunks[i].findFreeMemorySpaceDifference(totalChunksRequired) < smallestGap) // check if what we need can fit (regardless of how well)
                {
                    smallestGap = heapChunks[i].findFreeMemorySpaceDifference(totalChunksRequired);
                    bestIndex = i;
                }
            }
            if(bestIndex==-1)
                throw new Exception(string.Format("Inside Heap you asked for free space of {0} bytes...a chunk that large could not be found, try garbage collection to compact the free space", totalChunksRequired));
            return bestIndex;
        }
        #endregion

        public void allocateArraySpace(int arraySize, Value defaultValue, string arrayName)
        {
            int byteSize = 8;
            switch(defaultValue.readType())
            {
                case ("short"):
                    byteSize = 2;
                    break;
                case ("int"):
                    byteSize = 4;
                    break;
                case ("uint"):
                    byteSize = 4;
                    break;
                case ("long"):
                    byteSize = 8;
                    break;
                case ("float"):
                    byteSize = 4;
                    break;
                case("double"):
                    byteSize = 8;
                    break;
                default:
                    byteSize = 8;
                    break;
            }
            //TODO consider making the description of the chunk different to just it's name
            assignNewChunk(arrayName,arraySize,byteSize,defaultValue, memoryAssignmentTechnique);
        }

        public List<int> getNon_FreeHeaderAddresses()
        {
            List<int> headers = new List<int>();
            foreach (heapSector chunk in heapChunks)
            {
                if (!chunk.isFreeSpace())
                    headers.Add(chunk.headerAddress);
            }
            return headers;
        }

        /// <summary>
        /// Get a nice texty representation of the heaps contents
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string asString = "";
            foreach (var chunk in heapChunks)
                asString += chunk.ToString() + "\n";
            return asString;
        }

        /// <summary>
        /// Goes to each chunk addressed in the list and marks it as free space, then merges free chunks 
        /// </summary>
        /// <param name="headersAddressesToFree"></param>
        public void garbageCollect(List<int> headersAddressesToFree)
        {
            for( int i = 0; i < heapChunks.Count; i++)
            {
                if (headersAddressesToFree.Contains(heapChunks[i].headerAddress) && !heapChunks[i].isFreeSpace())
                    heapChunks[i].markAsFree();
            }
            mergeFreeSpace();
        }

        /// <summary>
        /// Goes through all the chunks of the heap and merged adjacent chunks that are free into 1 larger free chunk 
        /// </summary>
        private void mergeFreeSpace()
        {
            bool isMerged = false;
            do
            {
                isMerged = false;
                for (int i = 0; i < heapChunks.Count - 1; i++) //search till one BEFORE the last element (checking the last elemtent+1 would yield an out of bounds exception)
                {
                    if (heapChunks[i].isFreeSpace() && heapChunks[i + 1].isFreeSpace())
                    {
                        heapChunks[i].enlargeFreeSpace(heapChunks[i + 1].usedSectors);
                        heapChunks.RemoveAt(i + 1);
                        isMerged = true;
                    }
                }
            } while (isMerged);
        }

        private int getIndexOfChunkWithAddress(int heldMemoryAddress)
        {
            for(int i = 0; i < heapChunks.Count; i++)
            {
                //TODO add multi-occurence checking
                if (heapChunks[i].headerAddress == heldMemoryAddress)
                    return i;
            }
            throw new Exception("Address not found");
           
        }

        //The key word internal means that other members inside the assebly (that are outside the current class) can access it, but it is as accessible as public
        internal int getMemoryAddressAtLocation(int heldMemoryAddress, int heldOffset)
        {
            return heapChunks[getIndexOfChunkWithAddress(heldMemoryAddress)].getMemoryAddressOfElementX(heldOffset);
        }

        /// <summary>
        /// converts a header address for the heap plus an offset into an absolute address, for example: 10000 2 -> 10016
        /// </summary>
        /// <param name="headerAddress"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal int convertHeaderAndOffsetToAbsolute(int headerAddress, int offset)
        {
            for (int i = 0; i < heapChunks.Count; i++)
            {
                if (heapChunks[i].headerAddress == headerAddress)
                    return heapChunks[i].getMemoryAddressOfElementX(offset);
            }
            return -1; //if it isn't found
        }


    }
}
