using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace MyGameLibrary
{
    public class vertexAndMatricManipulation
    {
        public static Vector3 floatArrayToVector3(float[] theTriangleVertexArray, int vertexSetNumber)
        {
            return new Vector3(theTriangleVertexArray[vertexSetNumber * 3], theTriangleVertexArray[vertexSetNumber * 3 + 1], theTriangleVertexArray[vertexSetNumber * 3 + 2]);
        }




        /// <summary>
        /// Moves a 2D or 3D shape in 3D space according to its speed and direction
        /// </summary>
        /// <param name="theVertexArray">The Objects position details as a VertexPositionColor array </param>
        /// <param name="theSpeed">The amount in the X,Y,Z directions that each step moves</param>
        /// <param name="theDirection">The direction in the X,Y,Z directions, EG (1,-1,-1) would head Right, Up and Away</param>
        /// <param name="extraMultiplier">Usaully a stabilising factor such as gameTime.seconds</param>
        public static void moveShapePosition(VertexPositionColor[] theVertexArray, Vector3 theSpeed, Vector3 theDirection, float extraMultiplier)
        {
            for (int i = 0; i < theVertexArray.Length; i++)
            {
                theVertexArray[i].Position += theSpeed * theDirection * extraMultiplier;
            }
        }


        public static Vector3 gridToWorldConverter(int x, int y, int maxX, int maxY, float worldArea)
        {
            float topLeftCornerX = -1 * (worldArea / 2);
            float topLeftCornerY = (worldArea / 2);

            float moveDistance = worldArea / maxX;

            float xPosition = (float)Math.Round(x * moveDistance + topLeftCornerX,6);
            float yPosition = (float)Math.Round(-1*(y * moveDistance - topLeftCornerY),6);
            

            return new Vector3(xPosition, yPosition, 0);
        }

        public static int maxPosition(float[] theArray)
        {
            float maxSoFar = theArray[0];
            int maxIndex = 0;

            for (int i = 0; i < theArray.Length; i++)
            {
                if (theArray[i] > maxSoFar)
                {
                    maxSoFar = theArray[i];
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        public static int returnValue(int val)
        {
            return val;
        }



    }
}
