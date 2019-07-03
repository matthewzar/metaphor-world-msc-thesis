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
    public class SquaresGrid
    {
        List<Square> theGrid;
        int width;
        int height;
        Vector3 position;
        float squareSize;
        float gridScale;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theWidth">Number of square for the width of the grid</param>
        /// <param name="theHeight">Number of square for the height of the grid</param>
        /// <param name="xPos">Top left x coord of the grid</param>
        /// <param name="yPos">Top left y coord of the grid</param>
        public SquaresGrid(int theWidth, int theHeight, float xPos, float yPos, float sizeOfEachSquare, float scale)
        {
            width = theWidth;
            height = theHeight;
            position = new Vector3(xPos, yPos, 0);
            squareSize = sizeOfEachSquare;
            gridScale = scale;


            populateGrid();
        }

        private void populateGrid()
        {
            theGrid = new List<Square>();
            for (int i = 0; i < width * height; i++)
            {
                theGrid.Add(new Square());
            }
        }

        public void LoadContent(ContentManager conMan)
        {
            //http://stackoverflow.com/questions/3538820/xna-about-the-relation-between-world-space-and-the-screen-space/3539665#3539665
            ///This version sort of works, but only in the sense that it draws a 2d Grid top down and left to right...it doesn't factor screen position in at all
            //for (int i = 0; i < theGrid.Count; i++)
            //{
            //    float x = position.X + ((i % (int)width) * (squareSize + 0.2f));
            //    float y = position.Y - ((i / (int)width) * (squareSize + 0.2f));

            //    theGrid[i].LoadContent(conMan, new Vector3(x, y, 0), gridScale, squareSize, new Color(i%255, i%255, i%255));
            //}

            Vector3 myCoords;
            for (int i = 0; i < theGrid.Count; i++)
            {
                //find the xy positions on a grid 
                int x = i % width; 
                int y = i / width;

                float newScale = (1f / width) * gridScale;
                
                myCoords = vertexAndMatricManipulation.gridToWorldConverter(x, y, width, height, width * 2);
                myCoords.X += position.X;
                myCoords.Y += position.Y;
                theGrid[i].LoadContent(conMan, myCoords, newScale, squareSize, new Color(i % 255, i % 255, i % 255));
            }

            
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < theGrid.Count; i++)
                theGrid[i].Update(gameTime);
        }

        public void Draw(GraphicsDevice graphicsDevice, BasicEffect basicEffect)
        {
            for (int i = 0; i < theGrid.Count; i++)
            {
                theGrid[i].Draw(graphicsDevice, basicEffect);
            }
        }


    }
}
