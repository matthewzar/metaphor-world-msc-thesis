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
    public class Circle : TwoDimensionalShape
    {
        const int defaultTriangleCount = 100;
        float mRadius;
        public void LoadContent(ContentManager theContentManager, int totalTriangles, Vector3 startingPosition, float scale, float theRadius, String colourType, Color centreColour, Color borderColour1, Color borderColour2)
        {

           // if (totalTriangles % 2 == 0 && totalTriangles != 2)
            //    totalTriangles++;
            mRadius = theRadius;
            
            float[] circleVertices = getCircleVertices(mRadius, totalTriangles);
            Color[] circleColours = new Color[circleVertices.Length / 3];
            Random myRandom = new Random(DateTime.Now.Millisecond * DateTime.Now.Second * DateTime.Now.Minute * DateTime.Now.Day);

            #region colour selection
            switch (colourType.ToLower())
            {
                case ("allrandom"):
                    {
                        randomColouring(circleColours, myRandom,
                            Color.FromNonPremultiplied(myRandom.Next() % 255, myRandom.Next() % 255, myRandom.Next() % 255, 255));
                        break;
                    }
                case ("randomborder"):
                    {
                        randomColouring(circleColours, myRandom,centreColour);
                        break;
                    }
                case ("randomcentresingleborder"):
                    {
                        colouring(circleColours,Color.FromNonPremultiplied(myRandom.Next()%255,myRandom.Next()%255,myRandom.Next()%255,255),
                            borderColour1,borderColour1);
                        break;
                    }
                case ("randomcentredoubleborder"):
                    {
                        colouring(circleColours, Color.FromNonPremultiplied(myRandom.Next() % 255, myRandom.Next() % 255, myRandom.Next() % 255, 255),
                            borderColour1, borderColour2);
                        break;
                    }
                case ("singleborder"):
                    { 
                        colouring(circleColours, centreColour, borderColour1, borderColour1);
                        break;
                    }
                case ("doubleborder"):
                    {
                        colouring(circleColours, centreColour, borderColour1, borderColour2);
                        break;
                    }
                case ("solidcolour"):
                    {
                        colouring(circleColours, centreColour, centreColour, centreColour);
                        break;
                    }
                default:
                    {
                        colouring(circleColours,Color.Black,Color.Black,Color.Black);
                        break;
                    }
            }
            #endregion


            SetUpVertices(theContentManager, circleVertices, circleColours);

            base.LoadContent(theContentManager, userPrimitivesIndices, circleVertices, circleColours, startingPosition, totalTriangles, scale);

            mTotalTriangles *= 2; //the traingles are doubled because the draw function needs to draw the top and bottom half of the circle
        }

        //No Colour
        public void LoadContent(ContentManager theContentManager, Vector3 startingPosition, float scale, String colourType)
        {
            if (colourType.ToLower().Equals("solidcolour"))
            {
                LoadContent(theContentManager, defaultTriangleCount, startingPosition, scale, 1, "randomborder", Color.Black, Color.Black, Color.Black);
            }
            else
            {
                if (colourType.ToLower().Equals("allrandom"))
                {
                    LoadContent(theContentManager, defaultTriangleCount, startingPosition, scale, 1, "randomcentresingleborder", Color.Black, Color.Black, Color.Black);
                }
                else
                {
                    throw new Exception("Unkown Colour Type");
                }
            }
            LoadContent(theContentManager, defaultTriangleCount, startingPosition, scale, 1, "solidcolour", Color.Black, Color.Black, Color.Black);          
        }

        //One Colour
        /// <summary>
        /// 
        /// </summary>
        /// <param name="theContentManager"></param>
        /// <param name="startingPosition"></param>
        /// <param name="scale"></param>
        /// <param name="colourType">Random Border OR randomCentreSingleBorder OR SolidColor</param>
        /// <param name="nonRandomColour"></param>
        public void LoadContent(ContentManager theContentManager, Vector3 startingPosition, float scale, String colourType , Color nonRandomColour)
        {
            if(colourType.ToLower().Equals("randomborder"))
            {
                LoadContent(theContentManager, defaultTriangleCount, startingPosition, scale, 1, "randomborder", nonRandomColour, nonRandomColour, nonRandomColour);
            }
            else
            {
                if(colourType.ToLower().Equals("randomcentresingleborder"))
                {
                    LoadContent(theContentManager, defaultTriangleCount, startingPosition, scale, 1, "randomcentresingleborder", nonRandomColour, nonRandomColour, nonRandomColour);
                }
                else
                {
                    if (colourType.ToLower().Equals("solidcolour"))
                    {
                        LoadContent(theContentManager, defaultTriangleCount, startingPosition, scale, 1, "solidcolour", nonRandomColour, nonRandomColour, nonRandomColour);
                    }
                    else
                        throw new Exception("Unkown Single Colour Type");
                }
            }  
        }

        //Two Colours
        /// <summary>
        /// 
        /// </summary>
        /// <param name="theContentManager"></param>
        /// <param name="startingPosition"></param>
        /// <param name="scale"></param>
        /// <param name="colourType">RanddomcentreDoubleBorder OR singleBorder</param>
        /// <param name="firstColour"></param>
        /// <param name="borderColour"></param>
        public void LoadContent(ContentManager theContentManager, Vector3 startingPosition, float scale, String colourType, Color firstColour, Color borderColour)
        {
            if (colourType.ToLower().Equals("randomcentredoubleborder"))
            {
                LoadContent(theContentManager, defaultTriangleCount, startingPosition, scale, 1, "randomcentredoubleborder", firstColour, firstColour, borderColour);
            }
            else
            {
                if (colourType.ToLower().Equals("singleborder"))
                {
                    LoadContent(theContentManager, defaultTriangleCount, startingPosition, scale, 1, "singleBorder", firstColour, borderColour, borderColour);
                }
                else
                {
                    throw new Exception("Unkown Dual Colour Type");
                }
            }
        }

        //Three Colours
        /// <summary>
        /// 
        /// </summary>
        /// <param name="theContentManager"></param>
        /// <param name="startingPosition"></param>
        /// <param name="scale"></param>
        /// <param name="colourType">doubleborder</param>
        /// <param name="centreColour"></param>
        /// <param name="borderColour1"></param>
        /// <param name="borderColour2"></param>
        public void LoadContent(ContentManager theContentManager, Vector3 startingPosition, float scale, String colourType, Color centreColour, Color borderColour1, Color borderColour2)
        {
            if (colourType.ToLower().Equals("doubleborder"))
                LoadContent(theContentManager, defaultTriangleCount, startingPosition, scale, 1, "doubleborder", centreColour, borderColour1, borderColour2);
            else
                throw new Exception("Unkown Triple Colour Type");
            
        }


        private void randomColouring(Color[] theCurrentColorArray, Random myRandom, Color centreColor)
        {
            theCurrentColorArray[0] = centreColor;
            for (int i = 1; i < theCurrentColorArray.Length; i++)
            {
                theCurrentColorArray[i] = Color.FromNonPremultiplied(myRandom.Next() % 255, myRandom.Next() % 255, myRandom.Next() % 255, 255);
            }
        }

        private void colouring(Color[] theCurrentColorArray, Color centreColour, Color borderColour1, Color borderColour2)
        {
            theCurrentColorArray[0] = centreColour;

            for (int i = 1; i < theCurrentColorArray.Length; i++)
            {
                if(i % 2 == 0 )
                    theCurrentColorArray[i] = borderColour1;
                else
                    theCurrentColorArray[i] = borderColour2;
            }
        }

        private float[] getCircleVertices(float radius, int totalTriangles)
        {
            int totalVertices = totalTriangles + 2;

            float[] circleVertices = new float[totalVertices * 6 - 9];
            
            userPrimitivesIndices = new short[totalTriangles * 6];

            circleVertices[0] = 0;
            circleVertices[1] = 0;
            circleVertices[2] = 0;


            float distanceMeasure = radius / totalTriangles * 2;
            float currentDistance = -radius;


            for (int i = 3; i < totalVertices * 3; i += 3)
            {
                circleVertices[i] = currentDistance;
                circleVertices[i + 1] = (float)Math.Sqrt((radius * radius - circleVertices[i] * circleVertices[i]));
                circleVertices[i + 2] = 0;

                if (i != 3)
                {
                    circleVertices[circleVertices.Length - i + 3] = currentDistance;
                    circleVertices[circleVertices.Length - i + 4] = -circleVertices[i + 1];
                    circleVertices[circleVertices.Length - i + 5] = 0;
                }

                currentDistance += distanceMeasure;
            }

            int previousVertex = 1;

            for (int i = 0; i < userPrimitivesIndices.Length - 3; i += 3)
            {
                userPrimitivesIndices[i] = 0;
                userPrimitivesIndices[i + 1] = (short)previousVertex++;
                userPrimitivesIndices[i + 2] = (short)previousVertex;
            }

            userPrimitivesIndices[userPrimitivesIndices.Length - 3] = 0;
            userPrimitivesIndices[userPrimitivesIndices.Length - 2] = (short)previousVertex++;
            userPrimitivesIndices[userPrimitivesIndices.Length - 1] = userPrimitivesIndices[1];

            //userPrimitivesIndices = new short[] {0,1,2,   0,2,3,   3,4,0,  0,1,4};

            return circleVertices;
        }

    }
}
