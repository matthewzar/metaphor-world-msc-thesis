using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace spritePractice
{
    public class clickableSprite : simpleSprite
    {
        const string ASSETNAME = "pngImage";
        const string IMAGE_WHEN_CLICKED = "angryFace";
        const int START_POSITION_X = 125;
        const int START_POSITION_Y = 245;
        
        private MouseState oldState;
        
        Vector2 mStartingPosition = Vector2.Zero;

        ContentManager mContentManager;

        KeyboardState mPreviousKeyboardState;

        public void LoadContent(ContentManager theContentManager)
        {
            mContentManager = theContentManager;

            Position = new Vector2(START_POSITION_X, START_POSITION_Y);
            base.LoadContent(theContentManager, ASSETNAME);
            Source = new Rectangle(0, 0, 100, Source.Height);
        }

        public void Update(GameTime theGameTime)
        {
            KeyboardState aCurrentKeyboardState = Keyboard.GetState();

            mPreviousKeyboardState = aCurrentKeyboardState;
            
            if (spriteClicked())
            {
                mSpriteTexture = mContentManager.Load<Texture2D>(IMAGE_WHEN_CLICKED);
            }

            base.Update(theGameTime, Vector2.Zero, Vector2.Zero);
        }

        private bool spriteClicked()
        {
            MouseState mouseState = Mouse.GetState();
            int x = mouseState.X;
            int y = mouseState.Y;

            if (oldState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released &&
                x >= Position.X && x <= Size.Width + Position.X && y >= Position.Y && y <= Size.Height + Position.Y)
            {
                oldState = mouseState;
                return true;
            }
            else
            {
                oldState = mouseState;
                return false;
            }
            
            
        }


        public override void Draw(SpriteBatch theSpriteBatch)
        {
            base.Draw(theSpriteBatch);
        }

    }
}
