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
    public class clickableSprite : simpleSprite
    {
       // const string ASSETNAME = "pngImage";
        const int START_POSITION_X = 125;
        const int START_POSITION_Y = 245;
        private MouseState oldMouseState;
        private MouseState newMouseState;
        private KeyboardState oldKeyboardState;
        private KeyboardState newKeyboardState;
        
        Vector2 mStartingPosition = Vector2.Zero;

        ContentManager mContentManager;

        //public void LoadContent(ContentManager theContentManager)
        //{
        //    mContentManager = theContentManager;

        //    Position = new Vector2(START_POSITION_X, START_POSITION_Y);
        //    base.LoadContent(theContentManager, ASSETNAME);
        //    Source = new Rectangle(0, 0, 100, Source.Height);
        //}

        public void LoadContent(ContentManager theContentManager, string AssetName , int startX, int startY, float scale)
        {
            mContentManager = theContentManager;
            Scale = scale;
            Position = new Vector2(startX, startY);
            base.LoadContent(theContentManager, AssetName);
            Source = new Rectangle(0, 0, Source.Width, Source.Height);
        }

        public void Update(GameTime theGameTime, Action methodIfClicked)
        {
            if (spriteClicked())
            {
                Console.WriteLine("CLICKED THE BIN SPRITE");
                methodIfClicked();
            }

            base.Update(theGameTime, Vector2.Zero, Vector2.Zero);
        }

        private bool spriteClicked()
        {

            int x = newMouseState.X;
            int y = newMouseState.Y;

            if (newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released &&
                x >= Position.X && x <= Size.Width + Position.X && y >= Position.Y && y <= Size.Height + Position.Y)
            {
                return true;
            }
            else
            {
                return false;
            }
            
            
        }

        public void updateInputDevices(KeyboardState oldKeyboard, KeyboardState newKeyboard, MouseState oldMouse, MouseState newMouse)
        {
            oldMouseState = oldMouse;
            newMouseState = newMouse;
            oldKeyboardState = oldKeyboard;
            newKeyboardState = newKeyboard;
        
        }

        public override void Draw(SpriteBatch theSpriteBatch)
        {
            base.Draw(theSpriteBatch);
        }

    }
}
