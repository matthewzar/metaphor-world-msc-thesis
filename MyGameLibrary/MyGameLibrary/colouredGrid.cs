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
    //creates a new texture2d from scratch(image), based on an array of colours(Pixels)
    public class colouredGrid 
    {
        public Color[] Pixels { get; private set; }
        public Texture2D image { get; private set; }
        private int pixWidth;
        private int pixHeight;

        

        public colouredGrid(GraphicsDevice graphics,int pixelWidth, int pixelHeight)
        {
                       
            pixWidth = pixelWidth;
            pixHeight = pixelHeight;

            Pixels = new Color[pixelWidth * pixelHeight];
            Initialize(Color.White);
            image = new Texture2D(graphics, pixWidth, pixHeight);
            image.SetData(Pixels);
        }

        public void Initialize(Color initialColour)
        {
            Random myRan = new Random();
            int r = 0;
            int g = 0;
            int b = 0;
            for(int i = 0; i<Pixels.Length;i++)
            {
                if (r >= 255)
                {
                    r = 0;
                    b+=3;
                }
                if (b >= 255)
                {
                    b = 0;
                    g += 3;
                }
                if (g >= 255)
                {
                    g = 0;
                }
                r += 3;
                Pixels[i] = new Color(r,g,b);
                    //Pixels[i] = initialColour;
            }
            
        }
    }
}
