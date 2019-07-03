using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libraryTester
{
    class CA_Crowd
    {
        public List<CA_Cell> cellsOfTheCrowd;
        /* Methods of list<> traversal
            foreach (CA_Cell cell in cellsOfTheCrowd)
            {
                Console.WriteLine(cell.xCoord_Current);
            }

            for (int i = 0; i < cellsOfTheCrowd.Count - 1; i++) // Loop through List with for
            {
                Console.WriteLine(cellsOfTheCrowd[i].xCoord_Current);
            }*/

        public CA_Crowd()
            :this(new List<CA_Cell>())
        {
        }

        public CA_Crowd(List<CA_Cell> membersOfTheCrowd)
        {
            cellsOfTheCrowd = membersOfTheCrowd;
        }

        /// <summary>
        /// populates a crowd with a people who have random locations and destinations
        /// </summary>
        /// <param name="numberOfPeopleInCrowd"></param>
        /// <param name="maxXCoord"></param>
        /// <param name="maxYCoord"></param>
        public CA_Crowd(short numberOfPeopleInCrowd, short maxXCoord, short maxYCoord, float nonOptimalMoveChance)
            :this() //calls more basic constructor to create initial list
        {
            Random myRandom = new Random();
            for (short i = 0; i < numberOfPeopleInCrowd; i++)
            {
                cellsOfTheCrowd.Add(new CA_Cell((short)myRandom.Next(maxXCoord), (short)myRandom.Next(maxYCoord), (short)myRandom.Next(maxXCoord), (short)myRandom.Next(maxYCoord), maxXCoord, maxYCoord, nonOptimalMoveChance));
            }

            //get the desired next step for each of the cells
            foreach (CA_Cell cell in cellsOfTheCrowd)
            {
                cell.calculateDesiredNextStep();
            }
        }

        public void moveAll()
        {
            foreach (CA_Cell cell in cellsOfTheCrowd)
                cell.moveToDesiredNextStep();
        }

        public void moveThoseWithNoSharedDestination()
        {
            bool shared;
            for (short i = 0; i < cellsOfTheCrowd.Count; i++)
            {
                shared = false;
                for (short otherIndex = 0; otherIndex < cellsOfTheCrowd.Count; otherIndex++)
                {
                    if (i != otherIndex && sharedDestination(i, otherIndex))
                        shared = true;
                }
                if (shared == false)
                    cellsOfTheCrowd[i].moveToDesiredNextStep();
            }

            for (short i = 0; i < cellsOfTheCrowd.Count; i++)
            {
                cellsOfTheCrowd[i].calculateDesiredNextStep();
            }
        }

        public void moveThoseWithNoColissionsAndNoSharedDestination()
        {

            //check for blocked cells based on shared destinations AND crossing paths
            for (short i = 0; i < cellsOfTheCrowd.Count; i++)
            {
                for (short otherIndex = 0; otherIndex < cellsOfTheCrowd.Count; otherIndex++)
                {
                    if (i != otherIndex && (sharedDestination(i, otherIndex) || crossover(i, otherIndex)))
                    {
                        cellsOfTheCrowd[i].blockCell();
                        cellsOfTheCrowd[otherIndex].blockCell();
                    }

                    //check for blocked cells based on the target cell being blocked --- !!!This loop should probably keep going until no more blocks are assigned!!!
                    if (i != otherIndex && (isTargetBlocked(i)))
                    {
                        cellsOfTheCrowd[i].blockCell();
                    }
                }   
            }

            bool foundABlockedPath;

            do
            {
                foundABlockedPath = false;
                for (short i = 0; i < cellsOfTheCrowd.Count; i++)
                {
                    //check for blocked cells based on the target cell being blocked --- !!!This loop should probably keep going until no more blocks are assigned!!!
                    if (isTargetBlocked(i))
                    {
                        cellsOfTheCrowd[i].blockCell();
                        foundABlockedPath = true;
                    }
                }
            } while (foundABlockedPath);

            



            for (short i = 0; i < cellsOfTheCrowd.Count; i++)
            {
                cellsOfTheCrowd[i].moveToDesiredNextStep();
            }
            

            for (short i = 0; i < cellsOfTheCrowd.Count; i++)
            {
                cellsOfTheCrowd[i].calculateDesiredNextStep();
            }

            for (short i = 0; i < cellsOfTheCrowd.Count; i++)
            {
                for (short otherIndex = 0; otherIndex < cellsOfTheCrowd.Count; otherIndex++)
                {
                    if (i != otherIndex && occupySameSpace(i, otherIndex))
                        Console.WriteLine("DISASTER!"); //occurs when 2 cells share a space, the shared space is
                }
            }
        }

        private bool isTargetBlocked(short cellIndexToCheck)
        {
            if (cellsOfTheCrowd[cellIndexToCheck].blocked)
                return false;
            //search all cells to find one at the destination, if that one is blocked return true
            for (short i = 0; i < cellsOfTheCrowd.Count; i++)
            {
                if (i != cellIndexToCheck &&
                    cellsOfTheCrowd[i].xCoord_Current == cellsOfTheCrowd[cellIndexToCheck].xCoord_DesiredNextStep &&
                    cellsOfTheCrowd[i].yCoord_Current == cellsOfTheCrowd[cellIndexToCheck].yCoord_DesiredNextStep &&
                    cellsOfTheCrowd[i].blocked)
                    return true;
            }
            return false;
        }

        private bool sharedDestination(short cell1Index, short cell2Index)
        {
            if (cellsOfTheCrowd[cell1Index].xCoord_DesiredNextStep == cellsOfTheCrowd[cell2Index].xCoord_DesiredNextStep &&
                cellsOfTheCrowd[cell1Index].yCoord_DesiredNextStep == cellsOfTheCrowd[cell2Index].yCoord_DesiredNextStep)
                return true;
            else
                return false;
        }

        private bool crossover(short cell1Index, short cell2Index)
        {
            if (cellsOfTheCrowd[cell1Index].xCoord_DesiredNextStep == cellsOfTheCrowd[cell2Index].xCoord_Current &&
                cellsOfTheCrowd[cell1Index].yCoord_DesiredNextStep == cellsOfTheCrowd[cell2Index].yCoord_Current &&
                cellsOfTheCrowd[cell2Index].xCoord_DesiredNextStep == cellsOfTheCrowd[cell1Index].xCoord_Current &&
                cellsOfTheCrowd[cell2Index].yCoord_DesiredNextStep == cellsOfTheCrowd[cell1Index].yCoord_Current)
                return true;
            else
                return false;
        }

        private bool occupySameSpace(short cell1Index, short cell2Index)
        {
            if (cellsOfTheCrowd[cell1Index].xCoord_Current == cellsOfTheCrowd[cell2Index].xCoord_Current &&
                cellsOfTheCrowd[cell1Index].yCoord_Current == cellsOfTheCrowd[cell2Index].yCoord_Current)
                return true;
            else
                return false;
        }
    }
}
