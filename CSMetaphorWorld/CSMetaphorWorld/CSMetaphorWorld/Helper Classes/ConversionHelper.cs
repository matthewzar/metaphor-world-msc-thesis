using System;
using System.Collections.Generic;

using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CSMetaphorWorld
{
    static class ConversionHelper
    {

        internal static Tuple<Color[,], string> convertHeapToSPDrawFormat(Heap heapToConvert, MouseState currentMouseState, spDirectDraw gridDrawer,int gridWidth, int gridHeight)
        {
            int index = 0;
            int alphaVal = 255;

            string displayContent = "";

            Color[,] returnColours = new Color[gridWidth, gridHeight];

            for (int x = 0; x < gridWidth; x++)
                for (int y = 0; y < gridHeight; y++)
                    returnColours[x, y] = Color.Green;
           
            // go through each object stored on the heap (an object is made up of a list of values)
            foreach (heapSector singleSector in heapToConvert.heapChunks)
            {
                alphaVal = 155; //reset the transparancy value... to the default of "no this list is not highlighted" 

                //determine if one of the cells in the current object is being hovered over, so we know later to chnage it's alpha value
                //DONT'T loop through the actual content of the heapsector as its contents do not nessesarilly match up with the number of min-chumks it uses
                int localIndex = 0;
                foreach (string x in singleSector.enumerateOverChunks())
                {
                    if (gridDrawer.isCellHoveredOver(currentMouseState, (index + localIndex) % gridWidth, (index + localIndex) / gridHeight))
                    {
                        //getting here we know to highlight the whole section in the next iteration
                        alphaVal = 255;
                        break;
                    }
                    localIndex++;
                }
                

                //now we now know whether the whole group needs highlighting or not (and have already set the alpha value to reflect what must be done), now go and change colours
               // foreach (Value singleValue in singleSector.getCopyOfAllValues())
                foreach (string x in singleSector.enumerateOverChunks())
                {
                    //Is this value one of the ones being highlighted?
                    if (alphaVal == 255)
                    {
                        displayContent = x;

                    }

                    //TODO: replace this if else with a switch to allow for additional colours based on the heaps content
                    if (singleSector.isFreeSpace())
                    {
                        returnColours[index % gridWidth, index / gridWidth] = new Color(100, 100, 100, alphaVal);
                    }
                    else
                        returnColours[index % gridWidth, index / gridWidth] = new Color(200, 50, 100, alphaVal);


                    index++;//we can use this to decide what x and y values belong where

                    //TODO: figure out why the grid runs a risk of overflowing, I think it has something to to with the heap getting one extra chunk than it is originally allocated
                    //if your going to overflow simply stop sith the drawing
                    if (index >= gridWidth * gridWidth)
                        break;
                }
            }
            return new Tuple<Color[,], string>(returnColours, displayContent);
        }

        
    }
}
