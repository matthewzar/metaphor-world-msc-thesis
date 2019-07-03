using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace CSMetaphorWorld
{
    public class multiRegionClickableSprite : simpleSprite
    {
        string primarySpriteImage;
        string imageWhenClicked = "angryFace";
        string imageWhenHoveredOver = "angryFace";
        string imageWhenPressedOn = "angryFace";

        string specialBehaviourOnInteraction = "none";//this property will determine whether images being used are switched around when the the sprite is clicked/hoever/pressed etc

        SpriteFont font;
        int textXOffset = 0;
        int textYOffset = 0;
        string spriteText = "";
        Color textColour = Color.Red;

        int start_Position_X = 0;
        int start_Position_Y = 0;
        
        Vector2 mStartingPosition = Vector2.Zero;
        ContentManager mContentManager;

        private MouseState oldMouseState;
        private MouseState newMouseState;
        private KeyboardState oldKeyboardState;
        private KeyboardState newKeyboardState;

        List<Tuple<int, int, int, int>> clickableRegions;
        List<Action> actionsForRegions;

        

        public multiRegionClickableSprite(float scale, string primarySpriteFileName, SpriteFont textFont)
            : this(0, 0, scale, primarySpriteFileName, primarySpriteFileName, textFont)
        {
            
        }

        public multiRegionClickableSprite(int startingX, int startingY, float scale, string primarySpriteFileName, string imageWhenClickedFileName, SpriteFont textFont)
            : this(startingX, startingY, scale, primarySpriteFileName, imageWhenClickedFileName, new List<Tuple<int, int, int, int>>(), new List<Action>(), textFont)
        {
            
        }

        public multiRegionClickableSprite(int startingX, int startingY, float scale, string primarySpriteFileName, string imageWhenClickedFileName, List<Tuple<int, int, int, int>> listOfRegions, List<Action> listOfActionsForEachRegion, SpriteFont textFont)
            : this(startingX, startingY, scale, primarySpriteFileName, imageWhenClickedFileName, primarySpriteFileName, primarySpriteFileName, listOfRegions, listOfActionsForEachRegion, textFont, "none")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startingX"></param>
        /// <param name="startingY"></param>
        /// <param name="scale"></param>
        /// <param name="primarySpriteFileName"></param>
        /// <param name="imageWhenClickedFileName"></param>
        /// <param name="imageWhenHoveredOverFileName"></param>
        /// <param name="imageWhenPressedOnFileName"></param>
        /// <param name="listOfRegions"></param>
        /// <param name="listOfActionsForEachRegion"></param>
        /// <param name="textFont"></param>
        /// <param name="interactionBehaviour">Lets you alter the way certain actions are performed when the sprite is interated with (clicked/pressed/hovered etc). An example would be "swap primary and clicked"</param>
        public multiRegionClickableSprite(int startingX, int startingY, float scale, string primarySpriteFileName, string imageWhenClickedFileName, string imageWhenHoveredOverFileName,
                                            string imageWhenPressedOnFileName, List<Tuple<int, int, int, int>> listOfRegions, List<Action> listOfActionsForEachRegion, SpriteFont textFont, string interactionBehaviour)
        {
            specialBehaviourOnInteraction = interactionBehaviour;

            imageWhenHoveredOver = imageWhenHoveredOverFileName;
            imageWhenPressedOn = imageWhenPressedOnFileName;
            System.Diagnostics.Debug.Assert(listOfActionsForEachRegion.Count == listOfRegions.Count, string.Format(
                                            "The 2 lists in the multiRegionClickableSprite that uses image {0}, need to be the same length",primarySpriteFileName));
            start_Position_X = startingX;
            start_Position_Y = startingY;
            primarySpriteImage = primarySpriteFileName;
            imageWhenClicked = imageWhenClickedFileName;
            clickableRegions = listOfRegions;
            actionsForRegions = listOfActionsForEachRegion;
            Scale = scale;
            font = textFont;
            
        }

        public void refreshSizes()
        {
            LoadContent(mContentManager, primarySpriteImage);
        }


        /// <summary>
        /// Finds the first region  clcicked and returns its index, this is used to avoid perfoming multiple actions that might occur if
        /// overlapping regions are clicked. -1 is returned if no region is found
        /// </summary>
        /// <returns></returns>
        private int getFirstClickedRegionIndex()
        {
            System.Diagnostics.Debug.Assert(isSpriteClicked(), "We assumed that get getFirstClickedRegionIndex would only be called if the sprite had been clicked, using this method without checking that there was a click could cause problems");
            MouseState mouseState = Mouse.GetState();
            int x = mouseState.X;
            int y = mouseState.Y;
            for (int i = 0; i < clickableRegions.Count; i++)
            {
                if (clickableRegions[i].Item1 <= x - Position.X && clickableRegions[i].Item2 <= y - Position.Y && //is the clicked area is greater than the top left of the regio in question AND:
                    clickableRegions[i].Item3 + Position.X >= x && clickableRegions[i].Item4 + Position.Y >= y) // the clicked area is above and to the left of the bottom right of the region
                        return i;
            }
            return -1;
        }

        /// <summary>
        /// Takes a region that has been clicked and then executes the associated Action from the list of actions, assumes that there are an equal number of regions and actions
        /// </summary>
        /// <param name="clickedRegionIndex"></param>
        private void performActionForRegionX(int clickedRegionIndex)
        {
            //Error checking and logical assertions:
            System.Diagnostics.Debug.Assert(clickableRegions.Count == actionsForRegions.Count, "You shouldn't have unequal length lists of regions and their actions");
            if (clickedRegionIndex == -1)
                return;// getting here means the sprite was clicked, but no specific region was clicked
            System.Diagnostics.Debug.Assert(clickedRegionIndex >= 0 && clickedRegionIndex < clickableRegions.Count, "The index must be in the approriate range of the lists in question");

            //The actual performing of the action:
            actionsForRegions[clickedRegionIndex]();
        }

        public void LoadContent(ContentManager theContentManager)
        {
            
            mContentManager = theContentManager;

            Position = new Vector2(start_Position_X, start_Position_Y);
            base.LoadContent(theContentManager, primarySpriteImage);
            Source = new Rectangle(0, 0, Source.Width, Source.Height);
        }

        void performSpecialBehaviour()
        {
            switch(specialBehaviourOnInteraction.ToLower())
            {
                case ("swapprimaryandclick"):
                case ("swap primary and click"):
                case ("swapprimaryandclicked"):
                case ("swap primary and clicked"):
                case ("swapclickandprimary"):
                case ("swapclickedandprimary"):
                case ("swap clicked and primary"):
                case("swap click and primary"):
                    string temp = primarySpriteImage;
                    primarySpriteImage = imageWhenClicked;
                    imageWhenClicked = temp;
                    refreshSizes();
                    return;

                case ("alltoclicked"):
                case ("all to clicked"):
                case ("alltoclick"):
                case ("all to click"):
                    primarySpriteImage = imageWhenClicked;
                    imageWhenHoveredOver = imageWhenClicked;
                    imageWhenPressedOn = imageWhenClicked;
                    refreshSizes();
                    return;

                default:
                    return;
            }
        }

        public void Update(GameTime theGameTime, KeyboardState oldKeyboard, KeyboardState newKeyboard, MouseState oldMouse, MouseState newMouse)
        {
            updateInputDevices(oldKeyboard, newKeyboard, oldMouse, newMouse);
            Update(theGameTime);
        }

        public void Update(GameTime theGameTime)
        {
            //TODO - perhaps remove these GOTO statements, maybe replacing them with a flag
            if (isSpriteHoveredOver() && !isSpriteClicked() && !isSpritePressed())
            {
                mSpriteTexture = mContentManager.Load<Texture2D>(imageWhenHoveredOver);
                goto finishedStateChecking;
            }

            if (isSpriteClicked())
            {
                performActionForRegionX(getFirstClickedRegionIndex());
               // string temp = primarySpriteImage;
               // primarySpriteImage = imageWhenClicked;
                if (specialBehaviourOnInteraction.ToLower() == "none")
                    mSpriteTexture = mContentManager.Load<Texture2D>(imageWhenClicked);//the normal bahaviour
                else
                    performSpecialBehaviour();
               // imageWhenClicked = temp;
                
                goto finishedStateChecking;
            }
            
            if (isSpritePressed())
            {
                mSpriteTexture = mContentManager.Load<Texture2D>(imageWhenPressedOn);
                goto finishedStateChecking;
            }

            //What to do if you'r not being howeverd over or clicked at all:
            try
            {
                mSpriteTexture = mContentManager.Load<Texture2D>(primarySpriteImage);
            }
            catch (Exception){}

            finishedStateChecking:
            
            base.Update(theGameTime, Vector2.Zero, Vector2.Zero);
        }

        

        private bool isSpriteClicked()
        {
            int x = newMouseState.X;
            int y = newMouseState.Y;

            if (oldMouseState.LeftButton == ButtonState.Pressed && newMouseState.LeftButton == ButtonState.Released &&
                x >= Position.X && x <= Size.Width + Position.X && y >= Position.Y && y <= Size.Height + Position.Y)             
                return true;
            else
                return false;        
        }

        internal bool isSpriteHoveredOver()
        {
            int x = newMouseState.X;
            int y = newMouseState.Y;

            if (oldMouseState.LeftButton == ButtonState.Released && newMouseState.LeftButton == ButtonState.Released &&
                x >= Position.X && x <= Size.Width + Position.X && y >= Position.Y && y <= Size.Height + Position.Y)
                return true;
            else
                return false;
        }
        private bool isSpritePressed()
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
        public void updateText(int xOffset, int yOffset, float scale, string newText)
        {
            textXOffset = xOffset;
            textYOffset = yOffset;
            spriteText = newText;
            textScale = scale;
        }

        public override void Draw(SpriteBatch theSpriteBatch)
        {
            base.Draw(theSpriteBatch);
           //theSpriteBatch.DrawString(font, spriteText, new Vector2(start_Position_X + textXOffset, start_Position_Y + textYOffset), textColour);
            theSpriteBatch.DrawString(font, spriteText, new Vector2(start_Position_X + textXOffset, start_Position_Y + textYOffset), textColour, 0, Vector2.Zero, textScale, SpriteEffects.None, 0.0f);
        }

        public void updateInputDevices(KeyboardState oldKeyboard, KeyboardState newKeyboard, MouseState oldMouse, MouseState newMouse)
        {
            oldMouseState = oldMouse;
            newMouseState = newMouse;
            oldKeyboardState = oldKeyboard;
            newKeyboardState = newKeyboard;
        }

    }
}

