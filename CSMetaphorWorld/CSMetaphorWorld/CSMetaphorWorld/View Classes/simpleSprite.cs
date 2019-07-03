using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CSMetaphorWorld
{
    public class simpleSprite
    {
        //The asset name for the Sprite texture
        public string AssetName;

        //The Size of the Sprite (with Scale applied
        public Rectangle Size;

        //The amount to increase/decrease the size of the original sprite. 
        private float mScale = 1.0f;

        //Current position of the sprite:
        public Vector2 Position = new Vector2(0, 0);

        //The texture object used when drwaing the sprite:
        public Texture2D mSpriteTexture;

        //When the scale is modified throught he property, the Size of the 
        //sprite is recalculated with the new scale applied.
        public float Scale
        {
            get { return mScale; }
            set
            {
                mScale = value;
                //Recalculate the Size of the Sprite with the new scale
                Size = new Rectangle(0, 0, (int)(Source.Width * Scale), (int)(Source.Height * Scale));
            }
        }

        //The rectangular area from the origainal image that defines the sprite
        Rectangle mSource;
        public Rectangle Source
        {
            get { return mSource; }
            set
            {
                mSource = value;
                Size = new Rectangle(0, 0, (int)(mSource.Width * Scale), (int)(mSource.Height * Scale));
            }
        }


        //Load the texture for the sprite using the Content Pipeline
        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
            mSpriteTexture = theContentManager.Load<Texture2D>(theAssetName);
            AssetName = theAssetName;
            Source = new Rectangle(0, 0, mSpriteTexture.Width, mSpriteTexture.Height);
            Size = new Rectangle(0, 0, (int)(mSpriteTexture.Width * Scale), (int)(mSpriteTexture.Height * Scale));
        }

      

        //Load the texture for the sprite using the Content Pipeline
        public void LoadContent(ContentManager theContentManager, string theAssetName,int xPos, int yPos, float scale)
        {
            Position = new Vector2(xPos, yPos);
            Scale = mScale = scale;
 
            mSpriteTexture = theContentManager.Load<Texture2D>(theAssetName);
            AssetName = theAssetName;
            Source = new Rectangle(0, 0, mSpriteTexture.Width, mSpriteTexture.Height);
            Size = new Rectangle(0, 0, (int)(mSpriteTexture.Width * Scale), (int)(mSpriteTexture.Height * Scale));
        }

        public float rotation = 0.0f;

        public Color spriteTintColour = Color.White;
        public virtual void Draw(SpriteBatch theSpriteBatch)
        {
            if (mSpriteTexture != null)
                theSpriteBatch.Draw(mSpriteTexture, Position, Source,
                    spriteTintColour, rotation, Vector2.Zero, Scale, SpriteEffects.None, 0);
            else
                Console.WriteLine("Attempted a draw with no texture");
        }

        public void Draw(SpriteBatch theSpriteBatch, float theScale)
        {
            theSpriteBatch.Draw(mSpriteTexture, Position,
                new Rectangle(0, 0, mSpriteTexture.Width, mSpriteTexture.Height),
                spriteTintColour, 0.0f, Vector2.Zero, theScale, SpriteEffects.None, 0);
        }

        public void Update(GameTime theGameTime, Vector2 theSpeed, Vector2 theDirection)
        {
            Position += theDirection * theSpeed * (float)theGameTime.ElapsedGameTime.TotalSeconds;
        }

        
    }
}
