using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MyGameLibrary
{
    public class Square : TwoDimensionalShape
    {
        float edgeSize;
        Color generalColour;
        ContentManager secretManager;
        public void LoadContent(ContentManager theContentManager, Vector3 startingPosition, float scale, float theEdgeSize, Color colour)
        {

            secretManager = theContentManager;
            edgeSize = theEdgeSize;
            mScale = scale;
            generalColour = colour;
            float[] triangleVertices = new float[2 * 3 * 3];
            //triangleVertices[0] = 0;
            //triangleVertices[1] = 0;
            //triangleVertices[2] = 0;

            //triangleVertices[3] = theEdgeSize;
            //triangleVertices[4] = theEdgeSize;
            //triangleVertices[5] = 0;

            //triangleVertices[6] = theEdgeSize;
            //triangleVertices[7] = 0;
            //triangleVertices[8] = 0;

            //triangleVertices[9] = 0;
            //triangleVertices[10] = theEdgeSize;
            //triangleVertices[11] = 0;

            triangleVertices[0] = 0;
            triangleVertices[1] = 0;
            triangleVertices[2] = 0;

            triangleVertices[3] = edgeSize;
            triangleVertices[4] = -edgeSize;
            triangleVertices[5] = 0;

            triangleVertices[6] = 0;
            triangleVertices[7] = -edgeSize;
            triangleVertices[8] = 0;

            triangleVertices[9] = edgeSize;
            triangleVertices[10] = 0;
            triangleVertices[11] = 0;

            short[] vertexDrawOrder = new short[2*3];
            vertexDrawOrder[0] = 0;
            vertexDrawOrder[1] = 1;
            vertexDrawOrder[2] = 2;
            vertexDrawOrder[3] = 0;
            vertexDrawOrder[4] = 3;
            vertexDrawOrder[5] = 1;

            
            Color[] triangleColours = new Color[4];
            triangleColours[0] = generalColour;
            triangleColours[1] = generalColour;
            triangleColours[2] = generalColour;
            triangleColours[3] = generalColour;
            
            base.LoadContent(theContentManager, vertexDrawOrder, triangleVertices, triangleColours, startingPosition,2, scale);
        }

        

        public void changeColour(Color newColour)
        {
            generalColour = newColour;
            for (int i = 0; i < userPrimitives.Length; i++)
            {
                userPrimitives[i].Color = newColour;
            }
        }


       

    }
}
