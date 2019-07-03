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
    public class spDirectDraw
    {
        public Vector2 Position{ get; private set; }
        Texture2D foreGroundImage;
        Color[,] currentTintings;
        string currentText = "";
        SpriteFont font;

        public bool isCellHoveredOver(MouseState mouseState, int cellXNumber, int cellYNumber)
        {
            if (mouseState.X < Position.X + cellXNumber * 8 + 8 && mouseState.X >= Position.X + cellXNumber * 8 &&
                mouseState.Y < Position.Y + cellYNumber * 8 + 8 && mouseState.Y >= Position.Y + cellYNumber * 8)
                return true;
            return false;
        }

        public spDirectDraw(Vector2 startPosition,int width, int height)
        {
            Position = startPosition;
            currentTintings = new Color[width,height];
        }

        public void LoadContent(ContentManager theContentManager, string forgroundImageName, string backgroundImageName, string fontName)
        {
            foreGroundImage = theContentManager.Load<Texture2D>(forgroundImageName);
            font = theContentManager.Load<SpriteFont>(fontName);
            //Source = new Rectangle(0, 0, mSpriteTexture.Width, mSpriteTexture.Height);
            //Size = new Rectangle(0, 0, (int)(mSpriteTexture.Width * Scale), (int)(mSpriteTexture.Height * Scale));
        }

        public void DrawGrid(SpriteBatch theSpriteBatch)
        {
            if (currentTintings != null)
            {
                //draw the background (using the forground image)
                theSpriteBatch.Draw(foreGroundImage, new Rectangle((int)Position.X - 5, (int)Position.Y - 5, currentTintings.GetLength(0) * 8 + 10, currentTintings.GetLength(1) * 8 + 10), Color.White);
                for (int x = 0; x < currentTintings.GetLength(0); x++)
                    for (int y = 0; y < currentTintings.GetLength(1); y++)
                    {
                        theSpriteBatch.Draw(foreGroundImage, Vector2.Add(Position, new Vector2(x * 8, y * 8)), currentTintings[x, y]);
                    }
            }
            else
                Console.WriteLine("In the spFirectDraw class you attemted to draw a drid BEFORE declaring a colour grid...how the !@#$ you did that I can't figure out");

            if (currentText.Length > 30)
            {
                currentText = currentText.Substring(0, 30);
            }

            theSpriteBatch.DrawString(font, currentText, new Vector2(Position.X,Position.Y-30), Color.Red);

        }

        public void Update(GameTime theGameTime, Tuple<Color[,],string> newContent)
        {
            
            /////////////////////

            //Color[,] returnColourGrid = new Color[width, height];
            currentTintings = newContent.Item1;
            currentText = newContent.Item2;
            

          

        }
    }
}
