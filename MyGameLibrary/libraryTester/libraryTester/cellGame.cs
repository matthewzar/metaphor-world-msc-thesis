using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libraryTester
{
    class cellGame
    {
        public short[,] cellArray;
        public short[] breedRule;
        public short[] surviveRule;
        public short width;
        public short height;

        public cellGame()
        {
            width = 5;
            height = 5;
            breedRule = new short[10];
            surviveRule = new short[10];
            cellArray = new short[width, height];
            clearRules();
        }

        public cellGame(short theWidth, short theHeight)
        {
            width = theWidth;
            height = theHeight;
            breedRule = new short[10];
            surviveRule = new short[10];
            cellArray = new short[width, height];
            clearRules();
        }

        public cellGame(short theWidth, short theHeight, short[,] theStartingState)
        {
            width = theWidth;
            height = theHeight;
            breedRule = new short[10];
            surviveRule = new short[10];
            cellArray = (short[,])theStartingState.Clone();
            clearRules();
        }

        private void clearRules()
        {
            for(int i = 0; i<breedRule.Length; i++)
            {
                breedRule[i] = 9;
                surviveRule[i] = 9;
            }
            
        }

        //same as creating a new game
        private void clearWorld()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    cellArray[x, y] = 0;
                }
        }

        public void randomizeBoard(int amountOfLife)
        {
            
            Random myRandom = new Random();
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    if(myRandom.Next() % amountOfLife == 0)
                        cellArray[x, y] = 1;

                }
        }

        private void applyRules()
        {
            short neighbors = 0;
            short[,] newCellArray = new short[width, height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    neighbors = getNeighbors(x, y);
                    if ((cellArray[x, y] == 0 && checkRule(neighbors, breedRule)) ||
                             (cellArray[x, y] == 1 && checkRule(neighbors, surviveRule)))
                        newCellArray[x, y] = 1;

                   // if (cellArray[x, y] == 1 && checkRule(neighbors, surviveRule))
                    //    newCellArray[x, y] = 1;

                }
            cellArray = newCellArray;
        }

        private Boolean checkRule(short neighbors, short[] rule)
        {
            for (int i = 0; i < 10; i++)
            {
                if (neighbors == rule[i])
                    return true;
            }
            return false;
        }

        private short getNeighbors(int x, int y)
        {
            short cnt = 0;
            //if a non edge block
            if (x > 0 && y > 0 && x < width - 1 && y < height - 1)
            {
                cnt = (short)(cnt + cellArray[x - 1, y - 1]);
                cnt = (short)(cnt + cellArray[x - 1, y]);
                cnt = (short)(cnt + cellArray[x - 1, y + 1]);

                cnt = (short)(cnt + cellArray[x, y - 1]);
                cnt = (short)(cnt + cellArray[x, y + 1]);

                cnt = (short)(cnt + cellArray[x + 1, y - 1]);
                cnt = (short)(cnt + cellArray[x + 1, y]);
                cnt = (short)(cnt + cellArray[x + 1, y + 1]);
            }

            //top left
            if (x == 0 && y == 0)
            {
                cnt = (short)(cnt + cellArray[x, y + 1]);
                cnt = (short)(cnt + cellArray[x + 1, y + 1]);
                cnt = (short)(cnt + cellArray[x + 1, y]);
            }

            //bottom left
            if (x == 0 && y == height - 1)
            {
                cnt = (short)(cnt + cellArray[x, y - 1]);
                cnt = (short)(cnt + cellArray[x + 1, y - 1]);
                cnt = (short)(cnt + cellArray[x + 1, y]);
            }

            //top right
            if (x == width-1 && y == 0)
            {
                cnt = (short)(cnt + cellArray[x - 1, y]);
                cnt = (short)(cnt + cellArray[x - 1, y + 1]);
                cnt = (short)(cnt + cellArray[x, y + 1]);
            }

            //bottom right
            if (x == width-1 && y == height - 1)
            {
                cnt = (short)(cnt + cellArray[x, y - 1]);
                cnt = (short)(cnt + cellArray[x - 1, y - 1]);
                cnt = (short)(cnt + cellArray[x - 1, y]);
            }

            //left edge
            if (x == 0 && y != 0 && y != height - 1)
            {
                cnt = (short)(cnt + cellArray[x, y - 1]);
                cnt = (short)(cnt + cellArray[x, y + 1]);
                cnt = (short)(cnt + cellArray[x + 1, y]);
                cnt = (short)(cnt + cellArray[x + 1, y - 1]);
                cnt = (short)(cnt + cellArray[x + 1, y + 1]);
            }

            //right edge
            if (x == width-1 && y != 0 && y != height - 1)
            {
                cnt = (short)(cnt + cellArray[x, y - 1]);
                cnt = (short)(cnt + cellArray[x, y + 1]);
                cnt = (short)(cnt + cellArray[x - 1, y]);
                cnt = (short)(cnt + cellArray[x - 1, y - 1]);
                cnt = (short)(cnt + cellArray[x - 1, y + 1]);
            }

            //top edge
            if (y == 0 && x != 0 && x != width - 1)
            {
                cnt = (short)(cnt + cellArray[x - 1, y]);
                cnt = (short)(cnt + cellArray[x + 1, y]);
                cnt = (short)(cnt + cellArray[x, y + 1]);
                cnt = (short)(cnt + cellArray[x - 1, y + 1]);
                cnt = (short)(cnt + cellArray[x + 1, y + 1]);
            }

            //bottom edge
            if (y == height-1 && x != 0 && x != width - 1)
            {
                cnt = (short)(cnt + cellArray[x - 1, y]);
                cnt = (short)(cnt + cellArray[x + 1, y]);
                cnt = (short)(cnt + cellArray[x, y - 1]);
                cnt = (short)(cnt + cellArray[x - 1, y - 1]);
                cnt = (short)(cnt + cellArray[x + 1, y - 1]);
            }

            return cnt;
        }

        public void nextGeneration()
        {
            applyRules();
        }
    }
}
