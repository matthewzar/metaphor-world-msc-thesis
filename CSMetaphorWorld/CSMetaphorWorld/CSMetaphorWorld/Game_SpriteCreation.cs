using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Net.Mail;
using System.IO;


namespace CSMetaphorWorld
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class Game1 : Microsoft.Xna.Framework.Game
    {
        //This portion of the Game1 class is dedicated to the declaration and addition of the assorted sprites, however any non-anonymous delegates that the sprites are assigned are not declared in here

        /// <summary>
        /// The interactive sprites are those that don't change quantity, and generally not position, and who don't really depend on other sprites.
        /// So while the declare variables sprite only occurs once it isn't located here as it's position on the screen changes along with the number of
        /// variable sprites.
        /// </summary>
        private void initializeInteractiveSprites()
        {
            interactiveSpriteCollection = new multiSpriteCollection(Color.Black);
           
            #region add assorted sprites

            interactiveSpriteCollection.Add("conveyor",
                        new dynamicSprite(20, -50, 0.55f, "ConveyerBelt(WithPerspective)", "ConveyerBelt(WithPerspective)", "ConveyerBelt(WithPerspective)", "ConveyerBelt(WithPerspective)", new List<interactiveRegion> { }, font),
                        null,
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);

            interactiveSpriteCollection.Add("memorybarrier",
                        new dynamicSprite(-10, 260, 0.7f, "memoryBarrier", "memoryBarrier", "memoryBarrier", "memoryBarrier", new List<interactiveRegion> { }, font),
                        null,
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);

            interactiveSpriteCollection.Add("helpbackground",
                        new dynamicSprite(0, 0, 1.0f, "division", "division", "division", "division", new List<interactiveRegion> { }, font),
                        null,
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);

            interactiveSpriteCollection.Add("robot", new dynamicSprite(440, 50, 0.2f, "Robot", "Robot", "Robot", "Robot", new List<interactiveRegion> { }, font),
                        null,
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);

            

            if (terminalDisabled == true)
            {
                interactiveSpriteCollection.Add("terminal", new dynamicSprite(460, 333, 0.2f, "Terminal", "Terminal", "Terminal", "Terminal", new List<interactiveRegion> 
                         {
                            new interactiveRegion(10,20,139,47,readValueDirectFromHeapAsAction,interactiveRegion.interactionModes.mouse_left_press),
                            new interactiveRegion(10,72,139,99,writeValueDirectToHeapAsAction,interactiveRegion.interactionModes.mouse_left_press),
                            new interactiveRegion(10,130,139,157,addressToMemManAsAction,interactiveRegion.interactionModes.mouse_left_press),
                            new interactiveRegion(10,188,139,215,offsetToMemManAsAction,interactiveRegion.interactionModes.mouse_left_press)
                         }, font),
                            null,
                            new Tuple<Color, Color, Color, Color, Color>(Color.Black, Color.Black, Color.Black, Color.Black, Color.Black),
                            new Tuple<Color, Color, Color, Color, Color>(Color.Black, Color.Black, Color.Black, Color.Black, Color.Black), null);
            }
            else
            {
                interactiveSpriteCollection.Add("terminal", new dynamicSprite(460, 333, 0.2f, "Terminal", "Terminal", "Terminal", "Terminal", new List<interactiveRegion> 
                         {
                            new interactiveRegion(10,20,139,47,readValueDirectFromHeapAsAction,interactiveRegion.interactionModes.mouse_left_press),
                            new interactiveRegion(10,72,139,99,writeValueDirectToHeapAsAction,interactiveRegion.interactionModes.mouse_left_press),
                            new interactiveRegion(10,130,139,157,addressToMemManAsAction,interactiveRegion.interactionModes.mouse_left_press),
                            new interactiveRegion(10,188,139,215,offsetToMemManAsAction,interactiveRegion.interactionModes.mouse_left_press)
                         }, font),
                            null,
                            new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                            new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);
            }

            if (calculatorDisabled == true)
            {
                interactiveSpriteCollection.Add("calculator", new dynamicSprite(190, 670, 0.075f, "Calculator", "Calculator", "Calculator", "Calculator", new List<interactiveRegion> 
                         {
                             new interactiveRegion(0,0,118,200,leftClickedCalculator,interactiveRegion.interactionModes.mouse_left_press), //evaluate if not simplified, read to hand
                             new interactiveRegion(0,0,118,200,rightClickedCalculator,interactiveRegion.interactionModes.mouse_right_press), //write new expression, insert variable value 
                             new interactiveRegion(0,0,118,200,actionTextCalculator,interactiveRegion.interactionModes.mouse_over)
                         }, font),
                        null,
                        new Tuple<Color, Color, Color, Color, Color>(Color.Black, Color.Black, Color.Black, Color.Black, Color.Black),
                        new Tuple<Color, Color, Color, Color, Color>(Color.Black, Color.Black, Color.Black, Color.Black, Color.Black), null);
            }
            else
            {
                interactiveSpriteCollection.Add("calculator", new dynamicSprite(190, 670, 0.075f, "Calculator", "Calculator", "Calculator", "Calculator", new List<interactiveRegion> 
                         {
                             new interactiveRegion(0,0,118,200,leftClickedCalculator,interactiveRegion.interactionModes.mouse_left_press), //evaluate if not simplified, read to hand
                             new interactiveRegion(0,0,118,200,rightClickedCalculator,interactiveRegion.interactionModes.mouse_right_press), //write new expression, insert variable value 
                             new interactiveRegion(0,0,118,200,actionTextCalculator,interactiveRegion.interactionModes.mouse_over)
                         }, font),
                            null,
                            new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                            new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);
            }

            Action methodReturnAction = delegate()
            {
                returnFromMethodAsAction();
            };

            interactiveSpriteCollection.Add("returnbutton", new dynamicSprite(7, 500, 0.25f, "unpresseddownarrow", "presseddownarrow", "unpresseddownarrow", "presseddownarrow", new List<interactiveRegion> 
                         {
                             new interactiveRegion(0,0,100,220,methodReturnAction,interactiveRegion.interactionModes.mouse_left_press),
                             new interactiveRegion(0,0,100,100,actionTextReturnButton,interactiveRegion.interactionModes.mouse_over)
                         }, font),
                         newAncillarySprite: new dynamicSprite(pipX, pipY, 1f, "helpSprite-Return", "helpSprite-Return", "helpSprite-Return", "helpSprite-Return", new List<interactiveRegion> { }, font),
                         newPrimarySpritesTintColours: new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                         newAncillarySpritesTintColours: new Tuple<Color, Color, Color, Color, Color>(Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White),
                         specialColourConditional: displayEitherContextSpecificHelp);

            interactiveSpriteCollection.Add("bin", new dynamicSprite(635, 330, 0.4f, "RecycleBin", "RecycleBin", "RecycleBin", "RecycleBin", new List<interactiveRegion> 
                         {
                             new interactiveRegion(0,0,100,100,garbageCollect,interactiveRegion.interactionModes.mouse_left_press),
                             new interactiveRegion(0,0,100,100,actionTextGarbageCollect,interactiveRegion.interactionModes.mouse_over)
                         }, font),
                        null,
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);


            interactiveSpriteCollection.Add("methodtable", new dynamicSprite(60, 500, 0.14f, "FunctionTable", "FunctionTable", "FunctionTable", "FunctionTable", new List<interactiveRegion> { }, font),
                        null,
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);

            interactiveSpriteCollection.Add("namingbutton", new dynamicSprite(65, 600, 0.1f, "functionNameButton", "functionNamedAndPressed", "functionNameButton", "functionNameButton", new List<interactiveRegion> 
                         {
                             new interactiveRegion(0,0,100,100,beginMethodMenuSelection,interactiveRegion.interactionModes.mouse_left_press),
                            // new interactiveRegion(0,0,100,100,functionNamingAsAction,interactiveRegion.interactionModes.mouse_left_press),
                             new interactiveRegion(0,0,100,100,actionTextMethodButton,interactiveRegion.interactionModes.mouse_over)
                         }, font),
                        null,
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);

            //this pencil is used to handle any and all kinds of value writting to the hand
            interactiveSpriteCollection.Add("pencil", new dynamicSprite(650, 650, 1.0f, "pencil", "pencil", "pencil", "pencil", new List<interactiveRegion> 
                         {
                             new interactiveRegion(0,0,500,500,directHandWriteAsAction,interactiveRegion.interactionModes.mouse_left_press),
                             new interactiveRegion(0,0,500,500,actionTextDirectHandWrite,interactiveRegion.interactionModes.mouse_over)
                         }, font),
                        new dynamicSprite(pipX, pipY, 1.0f, "helpSprite-Evaluation", "helpSprite-Evaluation", "helpSprite-Evaluation", "helpSprite-Evaluation", new List<interactiveRegion> { }, font),
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                        new Tuple<Color, Color, Color, Color, Color>(Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White), displayEitherContextSpecificHelp);

            #region int pencil sprite and associated delegate

            //The string argument will always be the key of the sprite being used
            Predicate<string> intPencilInputLockedCheck = delegate(string key)
            {
                return cursorLockMode == lockMode.intLocked;
            };

            Action lockCursorOverIntPencil = delegate()
            {
                cursorLockMode = lockMode.intLocked;
                //now not only mark as locaked, but also make the sprite change to one without a pencil:
                interactiveSpriteCollection.changeAllPrimaryImagesTo("intpencil", "notepadnopencil");
            };

            interactiveSpriteCollection.Add("intpencil", new dynamicSprite(5, 745, 0.6f, "notepadpencil", "notepadpencil", "notepadpencil", "notepadpencil", new List<interactiveRegion> 
                         {
                             new interactiveRegion(0,0,500,500,lockCursorOverIntPencil,interactiveRegion.interactionModes.mouse_left_press),
                             new interactiveRegion(0,0,500,500,actionTextDirectHandWrite,interactiveRegion.interactionModes.mouse_over)
                         }, font),
                        null,
                        new Tuple<Color, Color, Color, Color, Color>(Color.Gray, Color.LightGray, Color.White, Color.White, new Color(0,200,0)),
                        new Tuple<Color, Color, Color, Color, Color>(Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White), intPencilInputLockedCheck);

            #endregion

            #region double pencil sprite and associated delegate

            //The string argument will always be the key of the sprite being used
            Predicate<string> doublePencilInputLockedCheck = delegate(string key)
            {
                return cursorLockMode == lockMode.doubleLocked;
            };

            Action lockCursorOverDoublePencil = delegate()
            {
                cursorLockMode = lockMode.doubleLocked;
                interactiveSpriteCollection.changeAllPrimaryImagesTo("doublepencil", "notepadnopencil");
            };

            interactiveSpriteCollection.Add("doublepencil", new dynamicSprite(210, 745, 0.6f, "notepadpencil", "notepadpencil", "notepadpencil", "notepadpencil", new List<interactiveRegion> 
                         {
                             new interactiveRegion(0,0,500,500,lockCursorOverDoublePencil,interactiveRegion.interactionModes.mouse_left_press),
                             new interactiveRegion(0,0,500,500,actionTextDirectHandWrite,interactiveRegion.interactionModes.mouse_over)
                         }, font),
                        null,
                        new Tuple<Color, Color, Color, Color, Color>(Color.Gray, Color.LightGray, Color.White, Color.White, Color.Green),
                        new Tuple<Color, Color, Color, Color, Color>(Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White), doublePencilInputLockedCheck);

            #endregion

            #region bool pencil sprite and associated delegate

            //The string argument will always be the key of the sprite being used
            Predicate<string> boolPencilInputLockedCheck = delegate(string key)
            {
                return cursorLockMode == lockMode.boolLocked;
            };

            Action lockCursorOverBoolPencil = delegate()
            {
                cursorLockMode = lockMode.boolLocked;
                interactiveSpriteCollection.changeAllPrimaryImagesTo("boolpencil", "notepadnopencil");
            };

            interactiveSpriteCollection.Add("boolpencil", new dynamicSprite(420, 745, 0.6f, "notepadpencil", "notepadpencil", "notepadpencil", "notepadpencil", new List<interactiveRegion> 
                         {
                             new interactiveRegion(0,0,500,500,lockCursorOverBoolPencil,interactiveRegion.interactionModes.mouse_left_press),
                             new interactiveRegion(0,0,500,500,actionTextDirectHandWrite,interactiveRegion.interactionModes.mouse_over)
                         }, font),
                        null,
                        new Tuple<Color, Color, Color, Color, Color>(Color.Gray, Color.LightGray, Color.White, Color.White, Color.Green),
                        new Tuple<Color, Color, Color, Color, Color>(Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White), boolPencilInputLockedCheck);

            #endregion

            interactiveSpriteCollection.getPrimaryDynamicSprite("intpencil").updateText(6, 35, 2f, "int:",Color.Black);
            interactiveSpriteCollection.getPrimaryDynamicSprite("doublepencil").updateText(6, 35, 2f, "double:", Color.Black);
            interactiveSpriteCollection.getPrimaryDynamicSprite("boolpencil").updateText(6, 35, 2f, "bool:", Color.Black);

            Predicate<string> operationIsBooleanConditional = delegate(string key)
            {
                return theWorldTracker.isNextOperation("consumeBool") ||
                       theWorldTracker.isLevelComplete() ;
            };

            Action consumeBoolean = delegate()
            {
                theWorldTracker.performOperation("consumeBool", freePlay);
            };
            int x = 3 > 2 ? 1 : -1;
            interactiveSpriteCollection.Add("booleater", new dynamicSprite(480, 560, 0.6f, "boolEater", "boolEater", "boolEater", "boolEater", new List<interactiveRegion> 
                         {
                            new interactiveRegion(0,0,500,500,consumeBoolean,interactiveRegion.interactionModes.mouse_left_press),
                            //new interactiveRegion(0,0,500,500,actionTextDirectHandWrite,interactiveRegion.interactionModes.mouse_over)
                         }, font),
                        null,
                        new Tuple<Color, Color, Color, Color, Color>(Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White),
                        new Tuple<Color, Color, Color, Color, Color>(Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent), operationIsBooleanConditional);

            interactiveSpriteCollection.Add("mouse", new dynamicSprite(900, 340, 0.8f, "mouse", "mouse", "mouse", "mouse", new List<interactiveRegion> { }, font),
                        null,
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);

            interactiveSpriteCollection.Add("feedback", new dynamicSprite(1095, 365, 1.5f, "feedbackNone", "feedbackNone", "feedbackNone", "feedbackNone", new List<interactiveRegion> { }, font),
                        null,
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                        new Tuple<Color, Color, Color, Color, Color>(Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent), null);

            interactiveSpriteCollection.Add("undo", new dynamicSprite(1100, 600, 0.4f, "undoButton", "undoButton", "undoButton", "undoButton", new List<interactiveRegion> 
                        {
                            new interactiveRegion(0,0,500,500,()=>{
                                    theWorldTracker.undoLastOperation(freePlay);
                                    LoadContent();
                                    reUpdate = true; //we want an extra update cycle becuase we want to re-hide all the sprites that will be revealed upun reloadingContent
                            },interactiveRegion.interactionModes.mouse_left_press),
                        }, font),
                         null,
                         new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                         new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);

            interactiveSpriteCollection.Add("help", new dynamicSprite(-1, -1, 1.0f, "help", "help", "help", "help", new List<interactiveRegion> { }, font),
                        null,
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.Transparent),
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.Transparent), 
                        (string k) => { return displayGeneralHelp; }); //we use a lamda function here becuase its a SIMPLE delegate needed that isn't used elseware

 
            interactiveSpriteCollection.Add("intro", new dynamicSprite(-1, -1, 1.0f, "introscreen", "introscreen", "introscreen", "introscreen", new List<interactiveRegion> { }, font),
                        null,
                        new Tuple<Color, Color, Color, Color, Color>(Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White),
                        new Tuple<Color, Color, Color, Color, Color>(Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White),
                        (string k) => { return displayIntro; }); //we use a lamda function here becuase its a SIMPLE delegate needed that isn't used elseware


            interactiveSpriteCollection.Add("border", new dynamicSprite(-1, -1, 1.05f, "border", "border", "border", "border", new List<interactiveRegion> { }, font),
                        null,
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                        new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);
            #endregion
        }

        /// <summary>
        /// The sprites that represent lower level frames are assigned/declred/initialized in here. Currently they are represented as shelves 
        /// </summary>
        /// <param name="bottomLeftX"></param>
        /// <param name="bottomLeftY"></param>
        /// <param name="stackSize"></param>
        private void assignBackgroundStackSprites(int bottomLeftX, int bottomLeftY, int stackSize)
        {
            float scale = 0.29f;

            Texture2D image = Content.Load<Texture2D>("topDownShelf");

            int width = (int)(image.Width * scale - 6);
            int height = (int)(image.Height * scale - 6);
            int currentX = bottomLeftX;
            int currentY = bottomLeftY;

            backgroundStackSprites = new List<dynamicSprite>();

            for (int i = 0; i < stackSize - 1; i++)
            {
                backgroundStackSprites.Insert(0, new dynamicSprite(currentX, currentY, scale, "topDownShelf", "topDownShelf", "topDownShelf", "topDownShelf", new List<interactiveRegion>(), font));

                scale = scale * 0.95f;
                currentX = currentX + 10;
                currentY = bottomLeftY - (int)((1 - scale) * height * ((i + 1)));
            }

            foreach (var sprt in backgroundStackSprites)
                sprt.LoadContent(Content);
        }

        private void assignFunctionCallingSprites(int bottomLeftX, int bottomLeftY, MethodMechanism theMethodHandler)
        {

            //TODO: consider putting an unloadContent call for the current currentFrameSprites so that the content manager isn't overloaded (might not be neccessary if content manager does garbage collection)

            float scale = 0.3f;

            Texture2D image = Content.Load<Texture2D>("singleShelfBrick");

            int width = (int)(image.Width * scale - 6) - 2;
            int height = (int)(image.Height * scale - 6) - 2;
            int currentX = bottomLeftX; //Whether we build up or down, the starting X position will always be the same
            int currentY = bottomLeftY - (height * (theMethodHandler.parameterValues.Count / 4)); //Shifts everything up from the bottom y coord
            //int currentY = bottomLeftY; //This would allow for buidling up, but not down


            functionCallingSpritesCollection = new multiSpriteCollection();
            
            for (int i = 0; i < theMethodHandler.parameterValues.Count; i++)
            {
                functionCallingSpritesCollection.Add("backbrick" + i, new dynamicSprite(currentX, currentY, scale, "singleShelfBrick", "singleShelfBrick", "singleShelfBrick", "singleShelfBrick", new List<interactiveRegion>(), font),
                                                     null,
                                                     new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                                                     new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);

                functionCallingSpritesCollection.Add("parameterbox" + i, new dynamicSprite(currentX + 25, currentY + 20, 0.06f, "variableBoxVersion2", "variableBoxVersion2", "variableBoxVersion2", "variableBoxVersion2", new List<interactiveRegion>() 
                                                        { 
                                                            new interactiveRegion(0, 0, 1000, 1000, clickedParameter, interactiveRegion.interactionModes.mouse_left_press),
                                                            new interactiveRegion(0, 0, 1000, 1000, actionTextWrite, interactiveRegion.interactionModes.mouse_over)
                                                        }, font), null,
                                                   new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                                                   new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);

                //functionCallingSpritesCollection.Add("parameterbox" + i, new dynamicSprite(currentX + 28, currentY + 40, 0.015f, "VariableBoxSideOn", "VariableBoxTopDown", "VariableBoxTopDown", "VariableBoxTopDown", new List<interactiveRegion>() 
                //                                        { 
                //                                            new interactiveRegion(0, 0, 1000, 1000, clickedParameter, interactiveRegion.interactionModes.mouse_left_press),
                //                                            new interactiveRegion(0, 0, 1000, 1000, actionTextWrite, interactiveRegion.interactionModes.mouse_over)
                //                                        }, font), null,
                //                                   new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                //                                   new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);


                currentX = bottomLeftX + (width * ((i + 1) % 4));
                currentY = bottomLeftY - (height * ((theMethodHandler.parameterValues.Count / 4) - ((i + 1) / 4))); //work out how far from the bottom we need to draw
            }

            functionCallingSpritesCollection.Add("finalbackbrick", new dynamicSprite(currentX, currentY, scale, "singleShelfBrick", "singleShelfBrick", "singleShelfBrick", "singleShelfBrick", new List<interactiveRegion>(), font),
                                                     null,
                                                     new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                                                     new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);


            //check if we are allowing the user to add arbitary parameters (might want to tie this in with freeplay mode flag)
            if (haveParameterAddingSprite)
            {
                functionCallingSpritesCollection.Add("paramadd", new dynamicSprite(currentX + 35, currentY + 30, scale, "addSign", "addSign", "addSign", "addSign", new List<interactiveRegion>() 
                                                        { 
                                                            new interactiveRegion(0, 0, 40, 40, declareParameterAsAction, interactiveRegion.interactionModes.mouse_left_press), 
                                                            new interactiveRegion(0, 0, 1000, 1000, actionTextDeclareParameter, interactiveRegion.interactionModes.mouse_over)
                                                        }, font), null,
                                                    new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                                                    new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);
            }
            else //if we don't have a parameter "+" sprite we can neaten things up by moving the call button to be inside the method calling mechanism 
            {
                functionCallingSpritesCollection.Add("call", new dynamicSprite(currentX + 35, currentY + 20, 0.14f, "unpresseduparrow", "presseduparrow", "unpresseduparrow", "presseduparrow", new List<interactiveRegion> 
                                                            {
                                                                new interactiveRegion(0,0,100,220,callSelectedMethodAsAction,interactiveRegion.interactionModes.mouse_left_press),
                                                                new interactiveRegion(0,0,100,100,actionTextCallButton,interactiveRegion.interactionModes.mouse_over)
                                                            }, font),
                                                             new dynamicSprite(pipX, pipY, 1f, "helpSprite-Call", "helpSprite-Call", "helpSprite-Call", "helpSprite-Call", new List<interactiveRegion> { }, font),
                                                    new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                                                    new Tuple<Color, Color, Color, Color, Color>(Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White), displayEitherContextSpecificHelp);
            }

            functionCallingSpritesCollection.LoadContent(Content);
        }

        private void assignCurrentFrameSprites(int bottomLeftX, int bottomLeftY, List<Tuple<string, string>> logicalFrameVariables)
        {
            //TODO: consider putting an unloadContent call for the current currentFrameSprites so that the content manager isn't overloaded (might not be neccessary if content manager does garbage collection)

            float scale = 0.3f;

            Texture2D image = Content.Load<Texture2D>("singleShelfBrick");

            int width = (int)(image.Width * scale - 6) - 2;
            int height = (int)(image.Height * scale - 6) - 2;
            int currentX = bottomLeftX; //Whether we build up or down, the starting X position will always be the same
            int currentY = bottomLeftY - (height * (logicalFrameVariables.Count / 4)); //Shifts everything up from the bottom y coord
            //int currentY = bottomLeftY; //This would allow for buidling up, but not down

            currentFrameSpritesCollection = new multiSpriteCollection();

            for (int i = 0; i < logicalFrameVariables.Count; i++)
            {
                currentFrameSpritesCollection.Add("frameshelfbrick" + i, new dynamicSprite(currentX, currentY, scale, "singleShelfBrick", "singleShelfBrick", "singleShelfBrick", "singleShelfBrick", new List<interactiveRegion>(), font),
                                                 null,
                                                 new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                                                 new Tuple<Color, Color, Color, Color, Color>(Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White), displayEitherContextSpecificHelp);

                currentFrameSpritesCollection.Add("variablebox" + i, new dynamicSprite(currentX + 25, currentY + 20, 0.06f, "variableBoxVersion2", "variableBoxVersion2", "variableBoxVersion2", "variableBoxVersion2", new List<interactiveRegion>()  
                                                {
                                                    new interactiveRegion(0,0,100,100,leftClickedVariable,interactiveRegion.interactionModes.mouse_left_press),
                                                    new interactiveRegion(0,0,100,100,rightClickedVariable,interactiveRegion.interactionModes.mouse_right_press),
                                                    new interactiveRegion(0,0,100,100,actionTextWriteRead,interactiveRegion.interactionModes.mouse_over)
                                                }, font),
                                                 new dynamicSprite(pipX, pipY, 1.0f, "helpSprite-Assignment", "helpSprite-Assignment", "helpSprite-Assignment", "helpSprite-Assignment", new List<interactiveRegion>(), font),
                                                new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                                                new Tuple<Color, Color, Color, Color, Color>(Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White), displayEitherContextSpecificHelp);

                //currentFrameSpritesCollection.Add("variablebox" + i, new dynamicSprite(currentX + 28, currentY + 40, 0.015f, "VariableBoxSideOn", "VariableBoxTopDown", "VariableBoxTopDown", "VariableBoxTopDown", new List<interactiveRegion>()  
                //                                {
                //                                    new interactiveRegion(0,0,50,30,leftClickedVariable,interactiveRegion.interactionModes.mouse_left_press),
                //                                    new interactiveRegion(0,0,50,30,rightClickedVariable,interactiveRegion.interactionModes.mouse_right_press),
                //                                    new interactiveRegion(0,0,100,100,actionTextWriteRead,interactiveRegion.interactionModes.mouse_over)
                //                                }, font),
                //                                 new dynamicSprite(pipX, pipY, 1.0f, "helpSprite-Assignment", "helpSprite-Assignment", "helpSprite-Assignment", "helpSprite-Assignment", new List<interactiveRegion>(), font),
                //                                new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                //                                new Tuple<Color, Color, Color, Color, Color>(Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White), displayEitherContextSpecificHelp);



                //a check to determine if we are on the final variable
                if (i == logicalFrameVariables.Count - 1)
                {
                    //currentFrameSpritesCollection.Add("deleteicon",
                    //                                 new dynamicSprite(currentX + 5, currentY + 15, 0.08f, "deleteIcon", "deleteIcon", "deleteIcon", "deleteIcon", new List<interactiveRegion>() 
                    //                                        { 
                    //                                            new interactiveRegion(0, 0, 40, 40, deleteNewestVariableAsAction, interactiveRegion.interactionModes.mouse_left_press),
                    //                                            new interactiveRegion(0, 0, 40, 40, actionTextDeleteVariable, interactiveRegion.interactionModes.mouse_over)
                    //                                        }, font), null,
                    //                                 new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                    //                                 new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);
                }

                currentX = bottomLeftX + (width * ((i + 1) % 4));
                currentY = bottomLeftY - (height * ((logicalFrameVariables.Count / 4) - ((i + 1) / 4))); //work out how far from the bottom we need to draw

            }

            currentFrameSpritesCollection.Add("lastframebrick",
                                             new dynamicSprite(currentX, currentY, scale, "singleShelfBrick", "singleShelfBrick", "singleShelfBrick", "singleShelfBrick", new List<interactiveRegion>(), font),
                                             null,
                                             new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                                             new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White), null);


            currentFrameSpritesCollection.Add("declarevariable",
                                                new dynamicSprite(currentX + 35, currentY + 30, scale, "addSign", "addSign", "addSign", "addSign",
                                                    new List<interactiveRegion>()
                                                { 
                                                    new interactiveRegion(0, 0, 40, 40, declareVariableAsAction, interactiveRegion.interactionModes.mouse_left_press), 
                                                    new interactiveRegion(0, 0, 40, 40, actionTextDeclare, interactiveRegion.interactionModes.mouse_over),
                                                }, font),
                                              new dynamicSprite(pipX, pipY, 1f, "helpSprite-Declaration", "helpSprite-Declaration", "helpSprite-Declaration", "helpSprite-Declaration", new List<interactiveRegion> { }, font),
                                              new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White),
                                              new Tuple<Color, Color, Color, Color, Color>(Color.Transparent, Color.Transparent, Color.Transparent, Color.Transparent, Color.White), displayEitherContextSpecificHelp);



            currentFrameSpritesCollection.LoadContent(Content);
        }



    }
}
