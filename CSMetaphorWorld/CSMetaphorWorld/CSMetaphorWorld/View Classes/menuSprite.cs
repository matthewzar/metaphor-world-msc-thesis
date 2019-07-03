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
    public class menuSprite
    {
        /// <summary>
        /// The various text options that the menu is to display on its buttons
        /// </summary>
        List<string> clickTextOption;

        /// <summary>
        /// The actual sprites that will be drawn, they will derive their text from the clickTextOptions
        /// </summary>
        List<dynamicSprite> clickableButtons;

        dynamicSprite background;
        SpriteFont font;
        ContentManager storedContentManager; //this is just so that we don't have to pass the content manager into the updatetext mechanism

        bool hidden = true;


        string dataToBeUsed = null;

        /// <summary>
        /// creates a menu Sprite collection that contains no options
        /// </summary>
        public menuSprite(SpriteFont font) : this(new List<string>(), font)
        {
        }

        public menuSprite(string[] theTextOptions, SpriteFont font)
            : this(new List<string>(theTextOptions), font)
        {
        }

        /// <summary>
        /// creates a menu Sprite collection based on a list of options.
        /// </summary>
        /// <param name="theTextOptions">The of options to display. The provided list will be CLONED and thus any changes made to either the orinaigal or the clone will not afect each other</param>
        public menuSprite(List<string> theTextOptions, SpriteFont font)
        {
            this.font = font;
            populateAttributes(theTextOptions);
        }

        int menuTopLeftX = 80;
        int menuTopLeftY = 50;

        private void populateAttributes(List<string> theTextOptions)
        {
            

            //clone the list of options
            clickTextOption = new List<string>(theTextOptions);

            background = new dynamicSprite(menuTopLeftX, menuTopLeftY, 1.0f, "menubackground", "menubackground", "menubackground", "menubackground", new List<interactiveRegion>(), font);

            //make a new set of clickable sprites 
            clickableButtons = new List<dynamicSprite>();

            //now populate the list witht he approriate text:
            for (int i = 0; i < clickTextOption.Count; i++)
            {
                clickableButtons.Add(new dynamicSprite(menuTopLeftX + 11, menuTopLeftY + i * 50 + 20, 1.0f, "menubutton", "menubutton", "menubutton", "menubutton", new List<interactiveRegion>()
                {
                    new interactiveRegion(0,3,1000,1000,storeClickedButtonsText,interactiveRegion.interactionModes.mouse_left_press)
                }, font));

                clickableButtons[i].updateText(10, 1, 1.6f, clickTextOption[i]);
            }

            closingOperation = noAction;
        }

        private void storeClickedButtonsText()
        {
            //do something to determine what button was pressed
            //store the appropriate string for use later
            int yCoord = Mouse.GetState().Y;
            int correspondingIndex = (yCoord - 70) / 50;
            dataToBeUsed = clickTextOption[correspondingIndex];
        }

        public string Update(GameTime gameTime, KeyboardState oldKeyboard, KeyboardState newKeyboard, MouseState oldMouse, MouseState newMouse)
        {
            //by changing update to return strings rather than being void we are able to return either null or the selected string from the users click if there was one
            //by returning the value to the user we are able to make them decide what action needs to be taken based on the returned string rather than trying to feed in
            //user defined methods/actions/delegates that we then have to handle explcicitly

            //sequence of events:
            //we have no data and return null
            //user clicks one of the dynamic sprites
                //the clicked sprite then stores its repective data into a private global string
            //the next time update is called return the selected string, and make the global blank again
            //repeat

            //if it's hidden (ie not showing then there is no reason to update anything), also as it still exists even if its not drawn we dont want the user able to click on it
            if (hidden)
                return null;
            
            //getting here means that the menu was not hidden, but that doesn't mean that the user has clicked on any of the button yet...we need to re-hide it when and only when a button is clicked

            background.Update(gameTime);

            foreach (dynamicSprite sprt in clickableButtons)
            {
                sprt.Update(gameTime, oldKeyboard, newKeyboard, oldMouse, newMouse);
            }

            string temp = dataToBeUsed; //if the user had selected data it stored temporarily
            dataToBeUsed = null; //regardless of whether the user stored data or not, redeclare the data 

            if (temp != null) //if the user clicked on a button that has a return value we need to close/hide the menu and perform the closing operation
            {
                hidden = true;
                closingOperation(temp);
            }

            return temp; //return the temp value, it will either be null or zero, but responsibility of what happens with the data belongs to whomever called this method
        }

        public void hide()
        {
            dataToBeUsed = null;
            hidden = true;
        }

        public void LoadContent(ContentManager Content)
        {
            //regardless of whether or not the menu is currently hidden we should always load its content so as to prevent null pointer exceptions
            storedContentManager = Content;
            background.LoadContent(Content);

            foreach (dynamicSprite sprt in clickableButtons)
            {
                sprt.LoadContent(Content);
            }
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            //if the menu is supposed to be hidden then just don't draw it, that way we don't have to re-create it every time the user wants to open or close it
            if (hidden)
                return;

            background.Draw(theSpriteBatch);

            foreach (dynamicSprite sprt in clickableButtons)
            {
                sprt.Draw(theSpriteBatch);
            }
        }


        private void noAction(string selection)
        {
        }

        public delegate void methodOnSelection(string theSelectionToOperateOn);

        /// <summary>
        /// This is the "void method(string selection)" method that will performed when the user finally clicks on a button
        /// </summary>
        methodOnSelection closingOperation;

        /// <summary>
        /// un-hides the menu so that the user can interact with it again
        /// </summary>
        public void displayMenu()
        {
            displayMenu(noAction);
        }

        /// <summary>
        /// un-hides the menu so that the user can interact with it again
        /// </summary>
        /// <param name="actionToPerformOnSelection">a method of type: "void methodName(string selection)"</param>
        public void displayMenu(methodOnSelection actionToPerformOnSelection)
        {
            closingOperation = actionToPerformOnSelection;
            hidden = false;
        }

        public void displayMenu(string[] newTextToDisplay)
        {
            displayMenu(newTextToDisplay, noAction);
        }

        public void displayMenu(string[] newTextToDisplay, methodOnSelection actionToPerformOnSelection)
        {
            hidden = false;

            //update the various texts:
            populateAttributes(new List<string>(newTextToDisplay));
            closingOperation = actionToPerformOnSelection;
            LoadContent(storedContentManager);
        }

        public bool isHidden()
        {
            return hidden;
        }
 

    }
}
