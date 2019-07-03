using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CSMetaphorWorld
{
    public class dynamicSprite: simpleSprite
    {
        /// <summary>
        /// A disabled sprite does not get drawn, and only performs the very first portion of the Update method (checking if it needs to be re-enabled)
        /// </summary>
        public bool isEnabled = true;

        string primarySpriteImage;
        string imageWhenClicked = "angryFace";
        string imageWhenHoveredOver = "angryFace";
        string imageWhenPressedOn = "angryFace";

        SpriteFont font;
        int textXOffset = 0;
        int textYOffset = 0;
        string spriteText = "";
        public Color textColour = Color.Red;

        int start_Position_X = 0;
        int start_Position_Y = 0;
        
        Vector2 mStartingPosition = Vector2.Zero;
        ContentManager mContentManager;

        private MouseState oldMouseState;
        private MouseState newMouseState;
        private KeyboardState oldKeyboardState;
        private KeyboardState newKeyboardState;

        
        List<interactiveRegion> interactions;

        public string getPrimarySpriteImageName()
        {
            return primarySpriteImage;
        }

        public dynamicSprite(int startingX, int startingY, float scale, string primarySpriteFileName, string imageWhenClickedFileName, string imageWhenHoveredOverFileName,
                             string imageWhenPressedOnFileName,  List<interactiveRegion> actionZones, SpriteFont textFont, Color originalSpriteTint, Color originalTextTint)
        {
            interactions = actionZones;
            foreach (interactiveRegion region in interactions)
            {
                region.addPositionalOffset(startingX, startingY);
            }

   
            imageWhenHoveredOver = imageWhenHoveredOverFileName;
            imageWhenPressedOn = imageWhenPressedOnFileName;
            primarySpriteImage = primarySpriteFileName;
            imageWhenClicked = imageWhenClickedFileName;

            start_Position_X = startingX;
            start_Position_Y = startingY;

            textColour = originalTextTint;
            spriteTintColour = originalSpriteTint;

            Scale = scale;
            font = textFont;

        }

        public dynamicSprite(int startingX, int startingY, float scale, string primarySpriteFileName, string imageWhenClickedFileName, string imageWhenHoveredOverFileName,
                             string imageWhenPressedOnFileName,  List<interactiveRegion> actionZones, SpriteFont textFont, Color originalTextTint) 
                                : this(startingX, startingY, scale, primarySpriteFileName, imageWhenClickedFileName, imageWhenHoveredOverFileName,
                                    imageWhenPressedOnFileName, actionZones, textFont, Color.White, originalTextTint) {}

        public dynamicSprite(int startingX, int startingY, float scale, string primarySpriteFileName, string imageWhenClickedFileName, string imageWhenHoveredOverFileName,
                             string imageWhenPressedOnFileName, List<interactiveRegion> actionZones, SpriteFont textFont)
            : this(startingX, startingY, scale, primarySpriteFileName, imageWhenClickedFileName, imageWhenHoveredOverFileName,
                imageWhenPressedOnFileName, actionZones, textFont, Color.White, Color.DarkRed) { }
        

        public void refreshSizes()
        {
            LoadContent(mContentManager, primarySpriteImage);
        }


        /// <summary>
        /// Of the list of interactive Regions that the dynamic sprite has only one can be executed at a time, this method picks the first one that can be performed and does it.
        /// </summary>
        private void performFirstPossibleAction()
        {
            MouseState mouseState = Mouse.GetState();
            int x = mouseState.X;
            int y = mouseState.Y;

            //This check is to ensure that the sprite does not perform any interactions when the mouse is not over it
            if (!isMouseOverSprite())
                return;

            for (int i = 0; i < interactions.Count; i++)
            {
                if (interactions[i].attemptInteraction(oldMouseState, newMouseState, oldKeyboardState, newKeyboardState))
                        return; //only do the first action that suceeds
            }

        }


        public void LoadContent(ContentManager theContentManager)
        {
            
            mContentManager = theContentManager;
            font = mContentManager.Load<SpriteFont>("myFont");
            Position = new Vector2(start_Position_X, start_Position_Y);
            base.LoadContent(theContentManager, primarySpriteImage);
            Source = new Rectangle(0, 0, Source.Width, Source.Height);
        }

        public void Update(GameTime theGameTime, int newXPosition, int newYPosition, KeyboardState oldKeyboard, KeyboardState newKeyboard, MouseState oldMouse, MouseState newMouse)
        {
            if (!isEnabled)
                return;

            textColour.A = spriteTintColour.A;
            Position.X = newXPosition;
            Position.Y = newYPosition;
            Update(theGameTime, oldKeyboard, newKeyboard, oldMouse, newMouse);
        }

        /// <summary>
        /// warning only use this version of update is you don't want the sprites mouse and keyboard states to change...essentially making the sprite non-interactive
        /// </summary>
        /// <param name="theGameTime"></param>
        /// <param name="newXPosition"></param>
        /// <param name="newYPosition"></param>
        public void Update(GameTime theGameTime, int newXPosition, int newYPosition)
        {
            //TODO: perform whatever checks you need to to decide if the sprite should be turned on

            if (!isEnabled)
                return;

            textColour.A = spriteTintColour.A;
            Position.X = newXPosition;
            Position.Y = newYPosition;
            Update(theGameTime);
            
        }

        public void Update(GameTime theGameTime, KeyboardState oldKeyboard, KeyboardState newKeyboard, MouseState oldMouse, MouseState newMouse)
        {
            //TODO: perform whatever checks you need to to decide if the sprite should be turned on

            if (!isEnabled)
                return;

            updateInputDevices(oldKeyboard, newKeyboard, oldMouse, newMouse);
            Update(theGameTime);
        }

        public void setAllSpriteTextures(string newTexture)
        {
            imageWhenHoveredOver = newTexture;
            imageWhenPressedOn = newTexture;
            primarySpriteImage = newTexture;
            imageWhenClicked = newTexture;
        }

        public void Update(GameTime theGameTime)
        {
            //TODO: perform whatever checks you need to to decide if the sprite should be turned on

            if (!isEnabled)
                return;

            
            performFirstPossibleAction(); 

            //Had a couple of GOTO statements here for the lols, replaced them with a simple flag to avoid being told that its bad practice (which I already know)
            bool stillPerformAction = true;

            if (isSpriteHoveredOver() && !isSpriteClicked() && !isSpritePressed())
            {
                mSpriteTexture = mContentManager.Load<Texture2D>(imageWhenHoveredOver);
                stillPerformAction = false;
            }

            if (stillPerformAction && isSpriteClicked())
            {
                mSpriteTexture = mContentManager.Load<Texture2D>(imageWhenClicked);//the normal bahaviour
                stillPerformAction = false;
            }

            if (stillPerformAction && isSpritePressed())
            {
                mSpriteTexture = mContentManager.Load<Texture2D>(imageWhenPressedOn);
                stillPerformAction = false;
            }

            //What to do if you'r not being howeverd over or clicked at all:
            if(stillPerformAction)
                try
                {
                    mSpriteTexture = mContentManager.Load<Texture2D>(primarySpriteImage);
                }
                catch (Exception) { }

            //when here any actions that depend on the current sprite texture are not performed
            //performFirstPossibleAction(); 

            
            base.Update(theGameTime, Vector2.Zero, Vector2.Zero);
        }

        /// <summary>
        /// checks if the mouse is over the sprite in question, regardless of whether its being clicked or not
        /// </summary>
        /// <returns></returns>
        public bool isMouseOverSprite()
        {
            int x = newMouseState.X;
            int y = newMouseState.Y;

            return x >= Position.X && x <= Size.Width + Position.X && y >= Position.Y && y <= Size.Height + Position.Y;
        }

        public bool isSpriteClicked()
        {
            int x = newMouseState.X;
            int y = newMouseState.Y;

            if (oldMouseState.LeftButton == ButtonState.Pressed && newMouseState.LeftButton == ButtonState.Released &&
                x >= Position.X && x <= Size.Width + Position.X && y >= Position.Y && y <= Size.Height + Position.Y)
                return true;
            else
                return false;
        }

        public bool isSpriteHoveredOver()
        {
            int x = newMouseState.X;
            int y = newMouseState.Y;

            if (oldMouseState.LeftButton == ButtonState.Released && newMouseState.LeftButton == ButtonState.Released &&
                x >= Position.X && x <= Size.Width + Position.X && y >= Position.Y && y <= Size.Height + Position.Y)
                return true;
            else
                return false;
        }
        public bool isSpritePressed()
        {
            int x = newMouseState.X;
            int y = newMouseState.Y;

            if (oldMouseState.LeftButton == ButtonState.Pressed && newMouseState.LeftButton == ButtonState.Pressed &&
                x >= Position.X && x <= Size.Width + Position.X && y >= Position.Y && y <= Size.Height + Position.Y)
                return true;
            else
                return false;
        }

       

        float textScale = 1.0f;
        public void updateText(int xOffset, int yOffset, float scale, string newText, Color newColour)
        {
            textXOffset = xOffset;
            textYOffset = yOffset;
            spriteText = newText;
            textScale = scale;
            textColour = newColour;
        }

        public void updateText(int xOffset, int yOffset, float scale, string newText)
        {
            updateText(xOffset, yOffset, scale, newText, textColour);
        }

        public override void Draw(SpriteBatch theSpriteBatch)
        {
            //Don't draw anything if it isn't enabled
            if (!isEnabled)
                return;

            base.Draw(theSpriteBatch);
            theSpriteBatch.DrawString(font, spriteText, new Vector2(Position.X + textXOffset, Position.Y + textYOffset), textColour, 0, Vector2.Zero, textScale, SpriteEffects.None, 0.0f);
        }

        public void updateInputDevices(KeyboardState oldKeyboard, KeyboardState newKeyboard, MouseState oldMouse, MouseState newMouse)
        {
            oldMouseState = oldMouse;
            newMouseState = newMouse;
            oldKeyboardState = oldKeyboard;
            newKeyboardState = newKeyboard;
        }

        /// <summary>
        /// Changes all the images (clicked, hovered, normal, pressed) associated with the sprite to one single image
        /// </summary>
        /// <param name="newImageName"></param>
        public void changeAllImagesTo(string newImageName)
        {
            //was:
            //primarySpriteImage = imageWhenClicked;
            //imageWhenHoveredOver = imageWhenClicked;
            //imageWhenPressedOn = imageWhenClicked;
            primarySpriteImage = newImageName;
            imageWhenHoveredOver = newImageName;
            imageWhenPressedOn = newImageName;
            imageWhenClicked= newImageName;
            LoadContent(mContentManager, primarySpriteImage);
        }
        


    }
}
