using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CSMetaphorWorld
{
    class textSprite
    {
        string textToDisplay;
        Vector2 clickableRegionTopLeft;
        Vector2 clickableRegionBottonRight;
        bool interactionEnabled = false;
        public string spriteName = "";


        public textSprite(string startingText, Vector2 topLeft, Vector2 bottomRight, string name, bool clickingEnabled)
        {
            clickableRegionBottonRight = bottomRight;
            clickableRegionTopLeft = topLeft;
            textToDisplay = startingText;
            spriteName = name;
            interactionEnabled = clickingEnabled;
        }

        public textSprite(string startingText, Vector2 topLeft, Vector2 bottomRight)
            : this(startingText, topLeft, bottomRight, "", false)
        {
        }


        public textSprite(string startingText, int topX, int topY, int bottomX,int bottomY) : this(startingText, new Vector2(topX, topY), new Vector2(bottomX, bottomY))
        { ///the chained contructor would call this code after calling the first contructor
        }

        
        internal bool isClicked(int mouseXCoord, int mouseYCoord)
        {
            return  interactionEnabled &&
                    mouseXCoord >= clickableRegionTopLeft.X && mouseXCoord <= clickableRegionBottonRight.X &&
                    mouseYCoord >= clickableRegionTopLeft.Y && mouseYCoord <= clickableRegionBottonRight.Y;
        }

        internal string getTextToDisplay()
        {
            return textToDisplay;
        }

        internal void changeText(string newText)
        {
            textToDisplay = newText;
        }

        internal void changeClickableRegion(int topX, int topY, int bottomX, int bottomY)
        {
            changeClickableRegion(new Vector2(topX, topY), new Vector2(bottomX, bottomY));
        }

        internal void changeClickableRegion(Vector2 topLeft, Vector2 bottomRight)
        {
            clickableRegionBottonRight = bottomRight;
            clickableRegionTopLeft = topLeft;
        }

        internal Vector2 getPosition()
        {
            return clickableRegionTopLeft;
        }

        
    }
}
