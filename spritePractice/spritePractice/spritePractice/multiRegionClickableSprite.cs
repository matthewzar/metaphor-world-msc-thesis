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
    public class multiRegionClickableSprite : simpleSprite
    {
        string primarySpriteImage;
        string imageWhenClicked = "angryFace";
        int start_Position_X = 0;
        int start_Position_Y = 0;
        
        private MouseState oldState;
        Vector2 mStartingPosition = Vector2.Zero;
        ContentManager mContentManager;
        KeyboardState mPreviousKeyboardState;

        List<Tuple<int, int, int, int>> clickableRegions;
        List<Action> actionsForRegions;

        public multiRegionClickableSprite(float scale, string primarySpriteFileName)
            : this(0, 0, scale, primarySpriteFileName, primarySpriteFileName)
        {
            
        }

        public multiRegionClickableSprite(int startingX, int startingY, float scale, string primarySpriteFileName, string imageWhenClickedFileName)
            : this(startingX, startingY, scale, primarySpriteFileName, imageWhenClickedFileName, new List<Tuple<int, int, int, int>>(), new List<Action>())
        {
            
        }

        public multiRegionClickableSprite(int startingX, int startingY, float scale, string primarySpriteFileName, string imageWhenClickedFileName, List<Tuple<int, int, int, int>> listOfRegions, List<Action> listOfActionsForEachRegion)
        {
            System.Diagnostics.Debug.Assert(listOfActionsForEachRegion.Count == listOfRegions.Count, "The 2 lists need to be the same length");
            start_Position_X = startingX;
            start_Position_Y = startingY;
            primarySpriteImage = primarySpriteFileName;
            imageWhenClicked = imageWhenClickedFileName;
            clickableRegions = listOfRegions;
            actionsForRegions = listOfActionsForEachRegion;
            Scale = scale;
        }

        /// <summary>
        /// Finds the first region  clcicked and returns its index, this is used to avoid perfoming multiple actions that might occur if
        /// overlapping regions are clicked. -1 is returned if no region is found
        /// </summary>
        /// <returns></returns>
        private int getFirstClickedRegionIndex()
        {
            System.Diagnostics.Debug.Assert(spriteClicked(), "We assumed that get getFirstClickedRegionIndex would only be called if the sprite had been clicked, using this method without checking that there was a click could cause problems");
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

        public void Update(GameTime theGameTime)
        {
            KeyboardState aCurrentKeyboardState = Keyboard.GetState();

            mPreviousKeyboardState = aCurrentKeyboardState;
            
            if (spriteClicked())
            {
                mSpriteTexture = mContentManager.Load<Texture2D>(imageWhenClicked);
                performActionForRegionX(getFirstClickedRegionIndex());
            }

            oldState = Mouse.GetState();
            base.Update(theGameTime, Vector2.Zero, Vector2.Zero);
        }

        private bool spriteClicked()
        {
            MouseState mouseState = Mouse.GetState();
            int x = mouseState.X;
            int y = mouseState.Y;

            if (oldState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released &&
                x >= Position.X && x <= Size.Width + Position.X && y >= Position.Y && y <= Size.Height + Position.Y)             
                return true;
            else
                return false;        
        }


        public override void Draw(SpriteBatch theSpriteBatch)
        {
            base.Draw(theSpriteBatch);
        }

    }
}

