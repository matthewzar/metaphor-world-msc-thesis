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
        SpriteFont font;

        /// <summary>
        /// This extended WINDOWS FORM will display the code that needs to be interpretted by the user, along with hints and pogress so far
        /// </summary>
        LevelCodeInterface textualInterface;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState keyboardOldState, keyboardNewState;
        MouseState mouseOldState, mouseNewState;

        DateTime lastFailure = DateTime.MinValue;
        DateTime lastSuccess = DateTime.MinValue;

        /// <summary>
        /// if for some reason you want to update twice in a row (effectively skipping the draw step one) then set this to true
        /// </summary>
        bool reUpdate = false;

        lockMode cursorLockMode = lockMode.unlocked;
        enum lockMode
        {
            intLocked,
            boolLocked,
            doubleLocked,
            otherLocked,
            unlocked
        }


        TextDisplay debugScreen; //This is responsible for keeping track of any text on the screen associated with debuging (i.e. non-graphical info) -- hasen't been updated for several builds now, so info might not be as usefull

        
        const int BYTES_IN_THE_HEAP = 1024;
        string currentLevelDirectory = "...\\...\\...\\levels\\empty.xml";

        bool freePlay = true;
        bool loadLevelOnMenuReturn = false;


        /// <summary>
        /// When declaring parameters that take a default value rather than one from the users hand, this flag lets us know whether to show the default values or make it appear as if 
        /// the parameters are temporarily empty...this is visual only
        /// </summary>
        bool parametersCanAppearBlank = true;

        /// <summary>
        /// this flag lets us know if we can decalare arbitrary parameters for the method we going to call...if we only allow the user to select pre-defined method then it
        /// doesn't make sense that they should be able to add parameters (or appear to be able to add parameters). Might want to tie this in with freeplay mode flag.
        /// </summary>
        bool haveParameterAddingSprite = false;

        bool calculatorDisabled = true;
     
        bool terminalDisabled = true;

        /// <summary>
        /// Checks whether or not the intro screen needs to be displayed
        /// </summary>
        bool displayIntro = true;

        /// <summary>
        /// keeps track of whether we want to display help for each item being hovered over
        /// </summary>
        bool displayComponentSpecificHelp = false;

        /// <summary>
        /// based on the next desired operation, we want to display help for the component that needs to be interacted with
        /// </summary>
        bool displayNextMoveHelp = false;
        

        /// <summary>
        /// Keeps track of whether the general help overlay needs to be displayed
        /// </summary>
        bool displayGeneralHelp = true;

       

        
        const string constDefaultExpression = "20 / 10";

        /// <summary>
        /// Keeps track of all the background logic...the main API class
        /// </summary>
        WorldTracker theWorldTracker;//= new WorldTracker(currentLevelDIrectory);
        

        bool gettingInputFlag = false;

        /// <summary>
        /// This is used for asyncronously communicating with the XNA 'Guide'
        /// </summary>
        IAsyncResult keyboardAsyncResult;
        string userInputedText = "";
        
        
        
        /// <summary>
        /// These sprites display the 'bookshelves' that make up the stack (except for the current frame),
        /// they probably could just be simpleSprites, however in order to allow for easy extending later I chose to make them more dynamic/powerful
        /// </summary>
        List<dynamicSprite> backgroundStackSprites;

        /// <summary>
        /// contains all the sprites neccesarry for diplaying the shelf and variables of the current stack frame,
        /// as the variables are interactive and have help components, they need to use the powerful sprite collection  
        /// </summary>
        multiSpriteCollection currentFrameSpritesCollection;

        /// <summary>
        /// contains all the sprites used by the function calling mechanism: boxes and variables for parameters, an add parameter button, a 'call' button
        /// </summary>
        multiSpriteCollection functionCallingSpritesCollection;

        dynamicSprite cursorSprite;

        /// <summary>
        /// These are all the sprites that only appear once, such as the calculator, method table, robot, conveyor belt, barbed wire etc...
        /// They have all been made as dynamicSprites to simplify later extensions.
        /// This was originally a list, but changes to the order of sprites, along with adding and removing new/old sprites, updates all over the code became difficult.
        /// Was a dictionary for a while, but was extended to include further functionality
        /// </summary>
        multiSpriteCollection interactiveSpriteCollection;
        
        menuSprite theMenu;

        spDirectDraw heapGridDrawer;
        textSprite topInstructions = new textSprite("F1 - Load Level    F2 - General Help   F3 - Debug Data    F4 - Display Intro Screen\nF5 - Toggle Component Help    F6 - Toggle Current Instruction Help", 0, -20, 20, 100);

        //FOR PIPing
        RenderTarget2D renderTarget;

        const int height = 900;
        const int width = 1300;

        const int pipX = 890;
        const int pipY = 10;

        public Game1()
        {
            
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = height;
            graphics.PreferredBackBufferWidth = width;
            Content.RootDirectory = "Content";
            keyboardOldState = Keyboard.GetState();
            keyboardNewState = Keyboard.GetState();

            Components.Add(new GamerServicesComponent(this));
            // Components[0].ShowSignIn(1, false);


            theWorldTracker = new WorldTracker(currentLevelDirectory);


            #region declaration of the debug menu....it a long contructor

            //THIS LOOKS LIKE THE START OF A MACRO EXTENSION:
            //TODO wonder about whether the constructor for debugDisplay should takes arguments of any type
            //ALSO TODO include the menu operations for the other 2 menu types
            debugScreen = new TextDisplay(font, new List<Tuple<Keys, Action, string>>() 
            {
                    new Tuple<Keys, Action, string>(Keys.D,declareVariableAsAction,"variables"),
                    new Tuple<Keys, Action, string>(Keys.A,assignVariableValueAsAction,"variables"),
                    new Tuple<Keys, Action, string>(Keys.E,evaluateCalculatorIntoHeld,"variables"),
                    new Tuple<Keys, Action, string>(Keys.W,enterCalculatorExpressionAsAction,"variables"),
                    new Tuple<Keys, Action, string>(Keys.S,substituteCalculatorValueAsAction,"variables"), 
                    new Tuple<Keys, Action, string>(Keys.R,readValueFromVariableAsAction,"variables"), 

                    new Tuple<Keys, Action, string>(Keys.L,garbageCollect,"memory"), 
                    new Tuple<Keys, Action, string>(Keys.A,arrayToHeapAsAction,"memory"), 
                    new Tuple<Keys, Action, string>(Keys.S,valueToMemManAsAction,"memory"), 
                    new Tuple<Keys, Action, string>(Keys.G,addressToMemManAsAction,"memory"), 
                    new Tuple<Keys, Action, string>(Keys.C,readValueFromHeapToMemManAsAction,"memory"),
                    new Tuple<Keys, Action, string>(Keys.O,readValueFromStackToMemManAsAction,"memory"),
                    new Tuple<Keys, Action, string>(Keys.W,writeValueFromMemManToHeapAsAction,"memory"),
                    new Tuple<Keys, Action, string>(Keys.I,writeValueToStackReferenceAsAction,"memory"),
                    new Tuple<Keys, Action, string>(Keys.T,readValueFromMemoryManagerAsAction,"memory"), 
                    new Tuple<Keys, Action, string>(Keys.V,convertAddressAndOffsetToAbsoluteAsAction,"memory"), 

                    new Tuple<Keys, Action, string>(Keys.D,declareParameterAsAction,"methods"), 
                    new Tuple<Keys, Action, string>(Keys.C,callSelectedMethodAsAction,"methods"),
                    new Tuple<Keys, Action, string>(Keys.N,functionNamingAsAction,"methods"), 
                    new Tuple<Keys, Action, string>(Keys.R,returnFromMethodAsAction,"methods"), 
                    new Tuple<Keys, Action, string>(Keys.A,assignParameterAsValueAsAction,"methods")
            });
            debugScreen.disable();



            #endregion


            //height and width are Math.Sqrt(BYTES_IN_THE_HEAP/4) because there are usually 4 bytes in a chunk and we are simply making the heap square
            //heapGridDrawer = new spDirectDraw(new Vector2(1000, 700), (int)Math.Sqrt(BYTES_IN_THE_HEAP / 4), (int)Math.Sqrt(BYTES_IN_THE_HEAP / 4));
            heapGridDrawer = new spDirectDraw(new Vector2(560, 130), (int)Math.Sqrt(BYTES_IN_THE_HEAP / 4), (int)Math.Sqrt(BYTES_IN_THE_HEAP / 4));

            
            textualInterface = new LevelCodeInterface();
            textualInterface.Show();
        }

        private void enableLevelsSprites()
        {
            string mode = theWorldTracker.currentLevel.componentEnabledMode;
            
            //all these things are requied every time...the pencil is not included as an 'essential' seeing as the base pencil is now for less common types like floats
            List<string> enabledKeys = new List<string>() { "helpbackground", "mouse", "feedback", "help", "intro", "border", "memorybarrier", "conveyor","undo"};
            bool legalMode = false;

            //some auto assumptions to make before we assign things
            interactiveSpriteCollection.getPrimaryDynamicSprite("calculator").isEnabled = false;
            heapGridDrawer.enabled = false; //the heap will be re-enabled IF the mode contains reference type word/s

            if (mode.Contains("int") || mode == "all")
            {
                enabledKeys.Add("intpencil");
                legalMode = true;
            }

            if (mode.Contains("double") || mode == "all")
            {
                enabledKeys.Add("doublepencil");
                legalMode = true;
            }

            if (mode.Contains("bool") || mode == "all")
            {
                enabledKeys.Add("boolpencil");
                enabledKeys.Add("booleater");
                legalMode = true;
            }

            if (mode.Contains("value") || mode.Contains("other") || mode.Contains("pencil") || mode == "all" ||
                (mode.Contains("minimal") && !legalMode))//check if we have 'minimal' mode going, and whether we have added any pencil sprites yet...if not then we need to add the generic pencil even if it isn't mentioned
            {
                enabledKeys.Add("pencil");
                legalMode = true;
            }

            if (mode.Contains("method") || mode.Contains("function") || mode.Contains("procedure") || mode == "all")
            {
                enabledKeys.AddRange(new string[] { "returnbutton", "methodtable", "namingbutton" });
                legalMode = true;
            }

            if (mode.Contains("heap") || mode.Contains("object") || mode.Contains("reference") || mode == "all")
            {
                enabledKeys.AddRange(new string[] {"robot", "terminal", "bin"});
                legalMode = true;
                heapGridDrawer.enabled = true; //make sure the heap draws
            }

            if(!legalMode)
                throw new Exception(string.Format("\"{0}\" is an unrecognised sprite enabling mode format", enabledKeys));

            foreach (var sprite in interactiveSpriteCollection.getEnumerator())
            {
                if (!enabledKeys.Contains(sprite.Item2))
                {
                    sprite.Item3.isEnabled = false;
                    if (sprite.Item4 != null)
                        sprite.Item4.isEnabled = true;
                }
            }
        }

        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //FOR PIPing:
            renderTarget = new RenderTarget2D(GraphicsDevice,
               // GraphicsDevice.PresentationParameters.BackBufferWidth * 4,
                // GraphicsDevice.PresentationParameters.BackBufferHeight * 4,
                4096, 4096,
                 false,
                  GraphicsDevice.PresentationParameters.BackBufferFormat,
                  DepthFormat.Depth24);

            exampleStackStart();
            base.Initialize();
        }

        #region various draw methods
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.CornflowerBlue);

            try
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                    SamplerState.LinearClamp, DepthStencilState.Default,
                    RasterizerState.CullNone);
            }
            catch(Exception drawError)
            {
                Console.WriteLine("An exception occured at the spritebatch.begin() of Draw, with error message {0}", drawError.Message);
                Console.WriteLine("As we can't be certain whether the Begin() call succeeded we have to return and assume that it did not");
                return;
            }
            
            //draw the entire screen into the 2dtexture "renderTarget" which is then rendered to the screen later
            DrawSceneToTexture(renderTarget);
            
            //draw the fullsize screen image
            spriteBatch.Draw(renderTarget, new Rectangle(0, 0, width, height), Color.White);

            
            
            cursorSprite.Draw(spriteBatch);



            //prepare some parameters for the zoomed in version of the screen
           // Vector2 Position = new Vector2(100, 100);
            float scaleX =  4096.0f / width;
            float scaleY =   4096.0f/ height;
            Rectangle areaToMagnify = new Rectangle((int)((Mouse.GetState().X - 50)*scaleX), (int)((Mouse.GetState().Y - 50)*scaleY),320,440);
            float rotation = 0.0f;
            Vector2 origin = new Vector2(-197,-122);
            
            //draw the zoomed in screen image
            //spriteBatch.Draw(renderTarget, new Rectangle(780, 480, 400, 400), areaToMagnify, Color.Gray, rotation, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(renderTarget, new Rectangle(pipX, pipY, 400, 400), areaToMagnify, Color.Gray, rotation, Vector2.Zero, SpriteEffects.None, 0);
            

            spriteBatch.DrawString(font, topInstructions.getTextToDisplay(), topInstructions.getPosition(), Color.White);
            DrawText();

            interactiveSpriteCollection.drawAllAncillaries(spriteBatch);
            currentFrameSpritesCollection.drawAllAncillaries(spriteBatch);
            if (theWorldTracker.isMethodNamed())
            {
                functionCallingSpritesCollection.drawAllAncillaries(spriteBatch);
            }

            interactiveSpriteCollection.DrawSpecificSpriteByKey("help", spriteBatch);
            interactiveSpriteCollection.DrawSpecificSpriteByKey("intro", spriteBatch);



            theMenu.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawSceneToTexture(RenderTarget2D theRenderTarget)
        {
            GraphicsDevice.SetRenderTarget(theRenderTarget);
            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            // Draw the scene
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //do the various drawing things in here
            DrawScene();

            // Drop the render target
            GraphicsDevice.SetRenderTarget(null);

        }

        private void DrawScene()
        {
            interactiveSpriteCollection.drawPrimariesWithExclusions(new List<string>{"intro","help"}, spriteBatch);


            foreach (dynamicSprite sprt in backgroundStackSprites)
            {
                sprt.Draw(spriteBatch);
            }

            currentFrameSpritesCollection.drawAllPrimaries(spriteBatch);


            //check if we need to draw the function mechanisms content (aside from it's button and table)
            if (theWorldTracker.isMethodNamed())
            {
                functionCallingSpritesCollection.drawAllPrimaries(spriteBatch);
            }

            heapGridDrawer.DrawGrid(spriteBatch);

        }

        private void DrawText()
        {
            debugScreen.Draw(spriteBatch, height);
        }

        #endregion

       

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>("myFont");

            #region cursor clicking action
            Action clickedCursor = delegate()
            {
                //gets called whenever the mouse clicked (and the cursor is matching it's position)
                if (cursorLockMode == lockMode.unlocked)
                {
                    //cursorSprite.Scale = 0.71f;
                    if (theWorldTracker.thePlayer.isHandEmpty())
                        cursorSprite.setAllSpriteTextures("handpointing");
                    else
                        cursorSprite.setAllSpriteTextures("handvalue");
                    //and then to be certain that everything updates correctly:
                    
                    return;
                }
                else
                {
                    cursorSprite.Position.Y = 670;
                    cursorSprite.setAllSpriteTextures("handpencil");
                    //cursorSprite.Scale = 0.7f;
                    if (cursorLockMode == lockMode.intLocked)
                        cursorSprite.Position.X = 180;
                    if (cursorLockMode == lockMode.doubleLocked)
                        cursorSprite.Position.X = 390;
                    if (cursorLockMode == lockMode.boolLocked)
                        cursorSprite.Position.X = 600;
                }
                return;
            };
            #endregion

            theMenu = new menuSprite(new List<string>(), font);
            theMenu.LoadContent(Content);

            cursorSprite = new dynamicSprite(0, 0, 0.7f, "handpointing", "handpointing", "handpointing", "handpointing", new List<interactiveRegion> 
                                            { 
                                                   new interactiveRegion(0,0,1000,1000,clickedCursor,interactiveRegion.interactionModes.mouse_left_press)
                                            }, font, Color.Black);

            initializeInteractiveSprites();
            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);          
           
            //TODO: This needs to be spread around to whenever the stack state changes
            assignCurrentFrameSpritesAsAction();

            //TODO: This needs to be spread around to whenever the state of the method mechanism changes
            assignFunctionCallingSpritesAsAction();

            heapGridDrawer.LoadContent(Content, "miniBox", "miniBox", "myFont");
            interactiveSpriteCollection.LoadContent(Content);

            currentFrameSpritesCollection.LoadContent(this.Content);

            cursorSprite.LoadContent(this.Content);
            debugScreen.LoadContent(this.Content);

            

            // TODO: use this.Content to load your game content here
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        #region desicion making actions (more than one possible action for a click)

        void clickedParameter()
        {
            //will behave much like 'clickedVariable', except you can only set the values...and even that might only be once...it's a design desision that still needs to be finalized
            int pixelXValue = mouseNewState.X;
            int pixelYValue = 590 - mouseNewState.Y;
            pixelXValue -= 50;
            pixelYValue /= 90;
            pixelXValue /= 100;

            int parameterClicked = (theWorldTracker.getParameterCount() / 4 - pixelYValue) * 4 + pixelXValue;

            //TODO check if the player is already holding something and decide what happens to that which is being held (it might get thrown away, placed in a pocket or left on the notepad)
            if (theWorldTracker.isHandEmpty()) //if the players hand empty?
            {
                Console.WriteLine("You can't enter an empty value as a parameter...you also can't read the value of a parameter");
                return;
            }
            else //if he's holding something then place said somethign into the clicked parameter
            {
                string paramName = theWorldTracker.methodMech.getParameterXName(parameterClicked);
                assignParameterAValue(paramName);
            }
            updateParameterValueTexts();
        }

        /// <summary>
        /// Not currently being used anywhere
        /// </summary>
        void clickedVariable()
        {
            int pixelXValue = mouseNewState.X;
            int pixelYValue = 470 - mouseNewState.Y;
            pixelXValue -= 50;
            pixelYValue /= 90;
            pixelXValue /= 100;
            int variableClicked = (theWorldTracker.getTotalLocalFrameVariables() / 4 - pixelYValue) * 4 + pixelXValue;

            //TODO check if the player is already holding something and decide what happens to that which is being held (it might get thrown away, places in a pocket or left on the notepad)
            if (theWorldTracker.isHandEmpty()) //is the players hand empty?
            {
                bool success = theWorldTracker.performOperation("readvariable " + theWorldTracker.globalStack.getTopFrame().getVariableXName(variableClicked), assignCurrentFrameSpritesAsAction, freePlay);
                if(success)
                    updateHandTextValue(theWorldTracker.getHandValueAsString());
            }
            else //if he's holding something then place said somethign into the clicked variable
            {
                theWorldTracker.performOperation("assignvalue " + theWorldTracker.globalStack.getTopFrame().getVariableXName(variableClicked),assignCurrentFrameSpritesAsAction,freePlay);
                updateHandTextValue("");
            }
        }

        /// <summary>
        /// read the hand value to the hand
        /// </summary>
        void rightClickedVariable()
        {
            int pixelXValue = mouseNewState.X;
            int pixelYValue = 470 - mouseNewState.Y;
            pixelXValue -= 50;
            pixelYValue /= 90;
            pixelXValue /= 100;
            int variableClicked = (theWorldTracker.getTotalLocalFrameVariables() / 4 - pixelYValue) * 4 + pixelXValue;

            bool success = theWorldTracker.performOperation("readvariable " + theWorldTracker.globalStack.getTopFrame().getVariableXName(variableClicked), assignCurrentFrameSpritesAsAction, freePlay);
            if (success)
                updateHandTextValue(theWorldTracker.getHandValueAsString());

            mouseOldState = mouseNewState; //if a variable is clicked we make the mouse appear as if it is no longer being clicked...this is to prevent more than one action occuring at a time by mistake
       
        }

        /// <summary>
        /// write the value to the variable
        /// </summary>
        void leftClickedVariable()
        {
            int pixelXValue = mouseNewState.X;
            int pixelYValue = 470 - mouseNewState.Y;
            pixelXValue -= 50;
            pixelYValue /= 90;
            pixelXValue /= 100;
            int variableClicked = (theWorldTracker.getTotalLocalFrameVariables() / 4 - pixelYValue) * 4 + pixelXValue;

            bool success = theWorldTracker.performOperation("assignvalue " + theWorldTracker.globalStack.getTopFrame().getVariableXName(variableClicked), assignCurrentFrameSpritesAsAction, freePlay);

            mouseOldState = mouseNewState; //if a variable is clicked we make the mouse appear as if it is no longer being clicked...this is to prevent more than one action occuring at a time by mistake
        }

        private void leftClickedCalculator()
        {
            //if the expression is in it's simplest form (i.e. no subtitution or evaluations possible)
            if (theWorldTracker.theCalculator.isSimplest())
            {
                calculatorExpToHand();
               // updateCalculatorText(theWorldTracker.theCalculator.getCurrentExpression());

                return;
            }

            if (theWorldTracker.theCalculator.isUnsimplifiable())
            {
                //TODO ensure that a failed substitution does not remove the value from the players hand
                substituteCalculatorValueAsAction();
                // updateCalculatorText(theWorldTracker.theCalculator.getCurrentExpression());
 
                return;
            }

            if (theWorldTracker.theCalculator.isNewExp())
            {
                evaluateCalculatorInPlace();
                // updateCalculatorText(theWorldTracker.theCalculator.getCurrentExpression());

                return;
            }

            //we should in theory never get here...if we do something is wrong;
            throw new Exception("You reached a location of code that should be unreachable");

        }

        private void rightClickedCalculator()
        {
            //if the expression is in it's simplest form (i.e. no subtitution or evaluations possible)
            if (theWorldTracker.theCalculator.isSimplest())
            {
                //let the user enter a new expression
                enterCalculatorExpressionAsAction();
                // updateCalculatorText(theWorldTracker.theCalculator.getCurrentExpression());

                return;
            }

            if (theWorldTracker.theCalculator.isUnsimplifiable())
            {
                //right clicking on an unsimplified expression doesn't do anything, so the user should simply be told that they made a mistake
                Console.WriteLine("There is no right click action for the calculator when it needs a substitution to be made");
                return;
            }

            if (theWorldTracker.theCalculator.isNewExp())
            {
                substituteCalculatorValueAsAction();
                // updateCalculatorText(theWorldTracker.theCalculator.getCurrentExpression());

                return;
            }

            //we should in theory never get here...if we do something is wrong
            throw new Exception("You reached a location of code that should be unreachable...panic");


            ////This check becomes invalid when using copy mode as nothing is ever empty
            if (theWorldTracker.isCalculatorEmpty()) //is the calculator blank?
            {
                //insert into the default, that which is in the players hand IF it is listed as an expression
                string defaultExpression = constDefaultExpression;
                if (theWorldTracker.thePlayer.examineHeld() != null && theWorldTracker.thePlayer.examineHeld().readType() == "Expression")
                {
                    theWorldTracker.performOperation("handexpressiontocalculator", freePlay);
                    updateHandTextValue("");
                }

                else
                    keyboardAsyncResult = Guide.BeginShowKeyboardInput(PlayerIndex.One, "Write expression into calculator", "Enter the expression you want entered into the calculator", defaultExpression, enterCalculatorExpression, null);
            }
            else //if it contains something please perform further checks to determine if we should evaluate or substitute
            {
                if (theWorldTracker.isHandEmpty()) //if the player is holding nothing, evaluate the expression
                {
                    evaluateCalculatorIntoHeld();
                    updateHandTextValue(theWorldTracker.getNoneNullHandValue());
                }
                else //if the player IS holding something, then try and perform a substituion
                {
                    //TODO ensure that a failed substitution does not remove the value from the players hand
                    substituteCalculatorValueAsAction();
                }
            }

            updateCalculatorText(theWorldTracker.theCalculator.ToString());
        }
        #endregion


        ///////////////////////IAsyncResult taking Functions
        #region IAsyncResult taking funtions
        void readValFromVar(IAsyncResult varNameInput)
        {
            string varName = Guide.EndShowKeyboardInput(varNameInput);
            theWorldTracker.performOperation("readvariable " + varName, freePlay);
        }


        /// <summary>
        /// This operation is a set of many smaller operation and basically tells the memory manager to hand the players value directly to a supplied address
        /// </summary>
        /// <param name="addressInput"></param>
        void sendHandToHeap(IAsyncResult addressInput)
        {
            string details = Guide.EndShowKeyboardInput(addressInput);
            int memAddress = int.Parse(details.Substring(0, details.IndexOf(' ')));
            int offset = int.Parse(details.Substring(details.IndexOf(' ') + 1));

            //TODO add checks for validity of address and other checks relating to this set of operations

            theWorldTracker.performOperation("inputaddresstomemman " + memAddress, freePlay);//give address to memman
            theWorldTracker.performOperation("inputoffsettomemman " + offset, freePlay);//give offset to memmanager
            theWorldTracker.performOperation("getabsoluteaddress", freePlay);//convert to absolute address
            theWorldTracker.performOperation("handtomemman", freePlay);//give value to memman
            theWorldTracker.performOperation("writevaluetoheap", freePlay);//commit memmans new value to the heap
        }
       
        void nameMethod(IAsyncResult userInput)
        {
            //TODO add checks for validity of user input
            string methName = Guide.EndShowKeyboardInput(userInput);

            //this particular check might only be temporary if we iplement an alternative way of lettign the console know it needs writting
            if (methName.Contains("Console.") || methName.Contains(".Write"))
            {
                theWorldTracker.performOperation("consolewrite", freePlay);
                return;
            }
            

            if (theWorldTracker.performOperation("namemethod " + methName, assignFunctionCallingSpritesAsAction, freePlay))
                interactiveSpriteCollection.getPrimaryDynamicSprite("namingbutton").changeAllImagesTo("functionNamedAndPressed");
        }

        /// <summary>
        /// Sends a message to the world tracker that doesn't just name the method, but also prepares all it parameters (with default values)
        /// </summary>
        /// <param name="methodNameAndSignature">"void someFunction(int age, double height)"</param>
        void prepareMethod(string methodNameAndSignature)
        {
            if (methodNameAndSignature.Contains("Console.") || methodNameAndSignature.Contains(".Write"))
            {
                theWorldTracker.performOperation("consolewrite", freePlay);
                return;
            }

            if (theWorldTracker.performOperation("preparemethod " + methodNameAndSignature, assignFunctionCallingSpritesAsAction,freePlay))
                interactiveSpriteCollection.getPrimaryDynamicSprite("namingbutton").changeAllImagesTo("functionNamedAndPressed");
        }

        void inputAddressToMemMan(IAsyncResult addressInput)
        {
            //TODO add checks for validity of address
            string input = Guide.EndShowKeyboardInput(addressInput);
            theWorldTracker.performOperation("addresstomemman " + input, freePlay);
        }

        void inputOffsetToMemMan(IAsyncResult offsetInput)
        {
            //TODO add checks for validity of address

            string input = Guide.EndShowKeyboardInput(offsetInput);
            theWorldTracker.performOperation("inputoffsettomemman " + input, freePlay);
        }

        void inputAddressAndOffsetToMemMan(IAsyncResult addressInput)
        {
            //TODO add checks for validity of address
            string details = Guide.EndShowKeyboardInput(addressInput);
            int memAddress = int.Parse(details.Substring(0, details.IndexOf(' ')));
            int offset = int.Parse(details.Substring(details.IndexOf(' ') + 1));

            theWorldTracker.performOperation("inputaddresstomemman " + memAddress, freePlay);
            theWorldTracker.performOperation("inputoffsettomemman " + offset, freePlay);
        }

        void substituteCalculatorValue(IAsyncResult addressInput)
        {
            string variableName = Guide.EndShowKeyboardInput(addressInput);
            theWorldTracker.performOperation("substituteexpression " + variableName, freePlay);
        }

        /// <summary>
        /// This operation is a set of many smaller operation and basically tells the memory manager to give the players a value directly from a supplied address
        /// </summary>
        /// <param name="addressInput"></param>
        void heapToHand(IAsyncResult addressInput)
        {
            //TODO add checks for validity of address
            //add defaulting option for singletons (a chunk with 1 item in its list)
            
            string details = Guide.EndShowKeyboardInput(addressInput);
            int memAddress = int.Parse(details.Substring(0, details.IndexOf(' ')));
            int offset = int.Parse(details.Substring(details.IndexOf(' ') + 1));

            theWorldTracker.performOperation("inputaddresstomemman " + memAddress, freePlay);//give address to memman
            theWorldTracker.performOperation("inputoffsettomemman " + offset, freePlay);//give offset to memmanager
            theWorldTracker.performOperation("getabsoluteaddress", freePlay);//convert to absolute address
            theWorldTracker.performOperation("readvaluefromheap", freePlay);//give value to memman from heap
            theWorldTracker.performOperation("getvaluefrommemman", freePlay);//take the value form the memMan and give it to the player
        }

        void assignVariableValueFromHand(IAsyncResult varDetails)
        {
            string details = Guide.EndShowKeyboardInput(varDetails);
            theWorldTracker.performOperation("assignvalue " + details, freePlay);
        }

        /// <summary>
        /// Used to assign the hands value to a named parameter
        /// </summary>
        /// <param name="parameterName"></param>
        void assignParameterAValue(IAsyncResult parameterName)
        {
            string paramName = Guide.EndShowKeyboardInput(parameterName);
            assignParameterAValue(paramName);
        }

        void assignParameterAValue(string parameterName)
        {
            

            Action refresh = delegate()
            {
                updateHandTextValue("");
                assignFunctionCallingSpritesAsAction();
                updateParameterValueTexts();
            };
            theWorldTracker.performOperation("assignparameter " + parameterName, refresh, freePlay);
        }

        void directHandWrite(IAsyncResult varDetails)
        {
            string details = Guide.EndShowKeyboardInput(varDetails);
            theWorldTracker.performOperation("directhandwrite " + details, freePlay);
        }

        void directHandWrite(string details)
        {
            theWorldTracker.performOperation("directhandwrite " + details, freePlay);
        }

        void declareVariable(IAsyncResult varDetails)
        {
            string details = Guide.EndShowKeyboardInput(varDetails);
            theWorldTracker.performOperation("declarevariable " + details, assignCurrentFrameSpritesAsAction, freePlay);
        }

        void declareParameter(IAsyncResult userInput)
        {
            string paramDetails = Guide.EndShowKeyboardInput(userInput);
            theWorldTracker.performOperation("declareparameter " + paramDetails, assignFunctionCallingSpritesAsAction, freePlay);
        }

        void arrayToHeap(IAsyncResult arrDetails)
        {
            string details = Guide.EndShowKeyboardInput(arrDetails);
            theWorldTracker.performOperation("declarearray " + details, freePlay);
        }

        void enterCalculatorExpression(IAsyncResult expressionInput)
        {
            string exp = Guide.EndShowKeyboardInput(expressionInput);
           
            Action refresh = delegate()
            {
                updateCalculatorText(exp);
            };

            theWorldTracker.performOperation("calculatorexpression " + exp, refresh, freePlay);
            
        }

        //assign the calue from the hand to a value whos name you specify
        void placeIntoVariable(IAsyncResult varNameInput)
        {
            string varName = Guide.EndShowKeyboardInput(varNameInput);
            theWorldTracker.performOperation("assignvalue " + varName, freePlay);
        }

        //not currently used anywhere, was originally intended to store input before calling an action so that the action could still use the input
        void StoreKeyboardResult(IAsyncResult result)
        {
            userInputedText = Guide.EndShowKeyboardInput(result);

            if (userInputedText == null)
            {
                //TODO add null event handling code for when the user cancels input
            }
        }


        #endregion

        #region IASync calling action, that GET user input and call IASync users

        void readValueFromVariableAsAction()
        {
            if (!Guide.IsVisible)
                keyboardAsyncResult = Guide.BeginShowKeyboardInput(PlayerIndex.One, "Read From Variable to hand", "Enter the name of the variable you want to read", "x", readValFromVar, null);
        }

        void assignParameterAsValueAsAction()
        {
            if (!Guide.IsVisible)
                keyboardAsyncResult = Guide.BeginShowKeyboardInput(PlayerIndex.One, "Name the parameter", "Enter the name of the parameter you want your hands value to be assigned to", "x", assignParameterAValue, null);
        }

        void assignVariableValueAsAction()
        {
            if (!Guide.IsVisible)
                keyboardAsyncResult = Guide.BeginShowKeyboardInput(PlayerIndex.One, "Place Value", "Enter the name of the variable to put a value into", "x", assignVariableValueFromHand, null);
        }

        void enterCalculatorExpressionAsAction()
        {
            if (!Guide.IsVisible)
                keyboardAsyncResult = Guide.BeginShowKeyboardInput(PlayerIndex.One, "Write expression into calculator", "Enter the expression you want entered into the calculator", constDefaultExpression, enterCalculatorExpression, null);
        }

        void arrayToHeapAsAction()
        {
            if (!Guide.IsVisible)
                keyboardAsyncResult = Guide.BeginShowKeyboardInput(PlayerIndex.One, "Allocate array space on heap", "Enter the type, name, and size of the desired array", "int xs 5", arrayToHeap, null);
        }

        //TODO consider making all the hints like this into one easy to use, portable, dictionary
        private string guideHintText_PencilClick = "<type> <value>";
        void directHandWriteAsAction()
        {
            if (!Guide.IsVisible)
                keyboardAsyncResult = Guide.BeginShowKeyboardInput(PlayerIndex.One, "Write new value to hand", "Enter the new values type and then its actual value, Examples: int 5, double 2.0, float 1.2f, bool true, char 'c'", guideHintText_PencilClick, directHandWrite, null);
            guideHintText_PencilClick = "";
        }
    

        void declareVariableAsAction()
        {
            if(!Guide.IsVisible)
                keyboardAsyncResult = Guide.BeginShowKeyboardInput(PlayerIndex.One, "Declare variable", "Enter the type and name of a variable you want to declare", "int x", declareVariable, null);
        }

        void declareParameterAsAction()
        {
            if (!Guide.IsVisible)
                keyboardAsyncResult = Guide.BeginShowKeyboardInput(PlayerIndex.One, "Declare A Parameter", "Enter the type and name of a parameter you want to declare", "int x", declareParameter, null);
        }

        void substituteCalculatorValueAsAction()
        {
            if (!Guide.IsVisible)
                keyboardAsyncResult = Guide.BeginShowKeyboardInput(PlayerIndex.One, "Substitute Expression Value", "Enter the name of the variable you would like to substitute a value into", "x", substituteCalculatorValue, null);
        }

        void beginMethodMenuSelection()
        {
            //TODO - Add method signatures to the Level class, so that we can store all the relevant method details there
            //get an array of all the various method signatures that the user can use

            //display the menu so that the user can select something
            //pass in the approriate methods that can be selected
            if(freePlay)
                theMenu.displayMenu(new string[] {"noArgs()","simpleMethod()","mergeSort()", "Console.Write(string outputText)", "recursiveFactorial(int n)", "getNthTriangle(int n)", "quadtraticSolver(double a, double b, double c)"}, prepareMethod);
            else
                theMenu.displayMenu(theWorldTracker.getTheCurrentLevelsMethods(), prepareMethod);
        }

        void functionNamingAsAction()
        {
            //TODO - ensure that all of these things are done:
            //must do several things:
            //first - check that the button is enabled
            //make the button un-clickable
            //ask the user to enter the name of a function
            //make the "add new parameter variable box & button appear
            //consider disabling the addition of new variables to the current frame (maybe even remove the frames 'add variable' button)
            if (theWorldTracker.methodMech.isNotHandlingAnything() && !Guide.IsVisible)
            {
                keyboardAsyncResult = Guide.BeginShowKeyboardInput(PlayerIndex.One, "Name the method", "Enter the name of the method you want to call", "exampleMethod", nameMethod, null);
            }
            //note: becuase of IA sync being asynchronous, any code after this line will likely not function correctly if it relies on changes based on the users input
        }

        #endregion

        #region non-input interaction methods

        /// <summary>
        /// Evaluates the calculators expression and immediately move the result to the users hand, should probably not use this except in special circumstances
        /// </summary>
        private void evaluateCalculatorIntoHeld()
        {
            theWorldTracker.performOperation("evaluatecalculatorhand", freePlay);
        }

        private void evaluateCalculatorInPlace()
        {
            theWorldTracker.performOperation("evaluatecalculator", freePlay);
        }

        /// <summary>
        /// moves the expression in the calculator to the players hand, this should only be done when the expression is in it's simplest form
        /// </summary>
        private void calculatorExpToHand()
        {
             theWorldTracker.performOperation("calculatortohand", freePlay);
        }

        private void garbageCollect()
        {
            theWorldTracker.performOperation("garbagecollect", freePlay);
        }

        //reads from the the memory manager and give it to the player (value must already be held by the mem manager)
        void readValueFromMemoryManagerAsAction()
        {
            theWorldTracker.performOperation("getvaluefrommemman", freePlay);
        }

        //reads a value from the heap at the held address and stores said value in the memory manager
        void readValueFromHeapToMemManAsAction()
        {
            theWorldTracker.performOperation("readvaluefromheap", freePlay);
        }

        //read the value at an address on the heap to the memory manager, and pass the value from the memory manager on to the player
        void readValueDirectFromHeapAsAction()
        {
            theWorldTracker.performOperation("readvaluefromheap", freePlay);
            theWorldTracker.performOperation("getvaluefrommemman", freePlay);
        }

        //makes the memory manager write the value it is holding to the address on the heap that its also holding
        void writeValueFromMemManToHeapAsAction()
        {
            theWorldTracker.performOperation("writevaluetoheap", freePlay);
        }

        //Sends value to the memory manager and then *immediatly* attempts to write said value to the heap (thus assuming that the memory manager has an address, and bypassing the intermediate step of asking the memmanager to send the value)
        void writeValueDirectToHeapAsAction()
        {
            theWorldTracker.performOperation("handtomemman", freePlay);
            theWorldTracker.performOperation("writevaluetoheap", freePlay);
        }

        //reads a value from the stack using its address, and stores the result in the memory manager
        void readValueFromStackToMemManAsAction()
        {
            theWorldTracker.performOperation("readvaluefromstack", freePlay);
        }

        void writeValueToStackReferenceAsAction()
        {
            theWorldTracker.performOperation("writevaluetostack", freePlay);
        }

        //Converts the offset and the address being held by the memory manager into an absolute address which can then be written to/read from
        void convertAddressAndOffsetToAbsoluteAsAction()
        {
            theWorldTracker.performOperation("getabsoluteaddress", freePlay);
        }

        void valueToMemManAsAction()
        {
            theWorldTracker.performOperation("handtomemman", freePlay);
        }

        void offsetToMemManAsAction()
        {
            theWorldTracker.performOperation("handoffsettomemman", freePlay);
        }

        void returnFromMethodAsAction()
        {
            theWorldTracker.performOperation("return", assignCurrentFrameSpritesAsAction, freePlay);
        }

        void callSelectedMethodAsAction()
        {
            //Could put something here to check if there are unassinged parameters, the responsisiblty usually belongs soley with the API:
            //however this is a somewhat special case where the call mechanims is still in place and the user could therefore get negative feedback by mistake
            if(theWorldTracker.isMethodNamed())
                theWorldTracker.performOperation("callfunction", resetCurrentMethodMechanism, freePlay);
        }

        void deleteNewestVariableAsAction()
        {
            theWorldTracker.performOperation("deletenewestvariable", assignCurrentFrameSpritesAsAction, freePlay);
        }

        //sends a HELD address from the player to the memory manager
        void addressToMemManAsAction()
        {
            theWorldTracker.performOperation("handaddresstomemman", freePlay);
        }

        #endregion


        /// <summary>
        /// returns true when a component either passes the test of being hovered over or when it's likely to be the next interaction component, or both.
        /// The relevant help flags have to be true for either of these cases to also be true.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool displayEitherContextSpecificHelp(string key)
        {
            return displayHoveredComponenentSpecificHelp(key) || displayNextMoveHelpMethod(key);
        }

        /// <summary>
        /// This method ties the value of an entries key to the current level
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool displayHoveredComponenentSpecificHelp(string key)
        {
            return displayComponentSpecificHelp && 
                   ((interactiveSpriteCollection.ContainsKey(key) && interactiveSpriteCollection.getPrimaryDynamicSprite(key).isMouseOverSprite()) ||
                    (currentFrameSpritesCollection.ContainsKey(key) && currentFrameSpritesCollection.getPrimaryDynamicSprite(key).isMouseOverSprite()) ||
                    (functionCallingSpritesCollection.ContainsKey(key) && functionCallingSpritesCollection.getPrimaryDynamicSprite(key).isMouseOverSprite()));
        }

        private bool displayNextMoveHelpMethod(string key)
        {
            if (!displayNextMoveHelp)
                return false;

            //This is the next opCode that the user has to try and make the world tracker perform...we use it for comparing
            string currentInstruction = theWorldTracker.currentLevel.getNextOperationCode();//current instruction would have to be read from the worldTrackers level information

            //for efficiency we can make the most common operation types get checked before the less common ones
            //"declarevariable"
            //

            if (currentInstruction.Contains("directhandwrite") && key == "pencil")
                return true;
            if (currentInstruction.Contains("assignvalue") && key.Contains("variablebox"))
                return true;
            //keys with unkown comparers do not yet have associated multiSpriteCollections
            if (currentInstruction.Contains("declarevariable") && key == "declarevariable")
                return true;
            if (currentInstruction.Contains("assignparameter") && key == "Unkown")
                return true;
            if (currentInstruction.Contains("preparemethod") && key == "namingbutton")
                return true;
            if (currentInstruction == "callfunction" && key == "namingbutton")
                return true;
            if (currentInstruction == "return" && key == "returnbutton")
                return true;
            
            if (currentInstruction == "xxxx" && key == "bin")
                return true;
            if (currentInstruction == "xxxx" && key == "calculator")
                return true;
            

            if (currentInstruction == "xxxx" && key == "conveyor")
                return true;
            if (currentInstruction == "xxxx" && key == "memorybarrier")
                return true;
            if (currentInstruction == "xxxx" && key == "robot")
                return true;
            if (currentInstruction == "xxxx" && key == "terminal")
                return true;
            if (currentInstruction == "nameMethod" && key == "methodtable")
                return true;

            
            if (currentInstruction == "xxxx" && key == "mouse")
                return true;
            if (currentInstruction == "xxxx" && key == "feedback")
                return true;
            if (currentInstruction == "xxxx" && key == "help")
                return true;
            if (currentInstruction == "nameMethod" && key == "intro")
                return true;
            if (currentInstruction == "nameMethod" && key == "border")
                return true;

            return false;
        }


        

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            keyboardNewState = Keyboard.GetState();
            mouseNewState = Mouse.GetState();

            //reset the mouse so that once we leave a sprite the old 'action text' is replaced by blank strings
            displayMouseTextAction("", "");

            
            //convert the game window into a form and then use it's position to alter that of the interaction form...this is posibly an overly complicated way of doing it...not really sure
            System.Windows.Forms.Form gameWindowForm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(this.Window.Handle);
            textualInterface.Location = new System.Drawing.Point(gameWindowForm.Location.X - textualInterface.Width, gameWindowForm.Location.Y-5);


            //textualInterface.Location = new System.Drawing.Point(0, 0);



            enableLevelsSprites();
            updateFeedbackSpriteTexture();

            #region input for closing the program -- highest priority
            // Allows the game to exit
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) && !Guide.IsVisible
                || textualInterface.IsDisposed)
            {
                //To prevent accadental closure we have removed this check for now
                if (theMenu.isHidden() || textualInterface.IsDisposed)
                    this.Exit();
                else
                //this will close the menu without actually doing anything with the returned string
                theMenu.hide();
            }
            #endregion

            #region Check for intro screen toggling input (F4)
            //Turn the intro screen off if any key is pressed, turn it on when F4 is pressed
            if (displayIntro && keyboardOldState != keyboardNewState && !keyboardOldState.IsKeyDown(Keys.F4))
            {
                interactiveSpriteCollection.getPrimaryDynamicSprite("intro").spriteTintColour = Color.Transparent;

                //When closing the intro the first thing a player should do is open a level
                diplayLevelLoadMenu();
                displayIntro = false;
                loadLevelOnMenuReturn = true;
            }
            else
                if (isKeyPressed(Keys.F4) && !displayIntro)
                    displayIntro = true;
            #endregion

            #region Load new level input and checks (F1)
            //TODO - wherever the guide shows up (as in not just here), we need to add in checks that will stop in from -re-opening when it's already open as this leads to crashes
            if (isKeyPressed(Keys.F1) && !Guide.IsVisible && theMenu.isHidden())
            {
                emailErrorCountDetails();

                //prepare the menu for diplay, both by displaying it as well as by giving it the various level names
                diplayLevelLoadMenu();

                loadLevelOnMenuReturn = true; //use this to remember that the next output of the menu is to be used for loading a level

                //Guide.BeginShowKeyboardInput(PlayerIndex.One, "Load a new level", "Enter the name of the level you want to load (Leave it blank to enter free play mode)", "", loadNewLevel, null);
            }
            #endregion

            #region Simple help overlay toggle (F2)
            if (isKeyPressed(Keys.F2))
            {
                if (displayGeneralHelp)
                {
                    //fadeSprites(Color.White);
                    displayGeneralHelp = false;
                }
                else
                {
                    //testers.getPrimaryDynamicSprite("help").spriteTintColour = Color.Transparent;
                    displayGeneralHelp = true;
                    //unfadeSprites();
                }
            }
            #endregion

            #region Toggle debug display input (F3)
            if (isKeyPressed(Keys.F3))
            {
                debugScreen.drawTextFlag = !debugScreen.drawTextFlag;
                if (debugScreen.drawTextFlag)
                    debugScreen.enable();
                else
                    debugScreen.disable();
            }
            #endregion

            #region Toggle more specific help sprite modes (F5 & F6)
            //Toggle the componenet specific help (help when hovered over)
            if (isKeyPressed(Keys.F5))
                displayComponentSpecificHelp = !displayComponentSpecificHelp;

            //Toggle the next moves help (highlight the sprite to interact with next)
            if (isKeyPressed(Keys.F6))
                displayNextMoveHelp = !displayNextMoveHelp;
            #endregion

            #region reload content/textures (F12)
            //F12 to re-load all content (this is useful when the data for something has changed but the content it's matching represention is using has not been updated (either on purpose or by mistake)
            //It's only really here for debuggin purposes
            if (isKeyPressed(Keys.F12))
            {
                LoadContent();
            }
            #endregion

            if (textualInterface.isThereALevelToLoad())
            {
                string levelDirectory = textualInterface.openNewLevel();
                throw new NotImplementedException();
            }

            
            //if the side form contains focus, make it so that the XNA window doesn't do anything based on user interactions that are meant for the form.
            if (textualInterface.ContainsFocus)
            {
                mouseNewState = new MouseState();
                mouseOldState = new MouseState();

                keyboardNewState = new KeyboardState();
                keyboardOldState = new KeyboardState();
            }
            else
            {
                //TODO - make it so that the interface is only updated when an actual operation is completed
                textualInterface.changeBasedOnLevelState(theWorldTracker.currentLevel);
            }

            if (gettingInputFlag == false && !textualInterface.Focused)
            {
                //Pass references to the world Tracker so that it can have key information read from it
                debugScreen.Update(theWorldTracker, keyboardOldState, keyboardNewState);
            }

            //this is where we update the menu, and if it gets clicked its also where we will store the clicked text
            string menuSelection = theMenu.Update(gameTime, keyboardOldState, keyboardNewState, mouseOldState, mouseNewState);

            //if the menu has returned a value, and the menu that was open was one asking about a level to load, then load the level in question:
            if (menuSelection != null && loadLevelOnMenuReturn)
            {
                Console.WriteLine("Loading level {0}.", menuSelection);
                loadNewLevel(menuSelection);
                loadLevelOnMenuReturn = false;
            }

            //everything in this if block is to do with updating the state of various sprites, HOWEVER if the menu sprite is being diplayed we DON'T want anythign else to be interactive
            //therefore we just don't update the more interactive sprites...that way they esentially pause exactly where they were before the menu was opened.
            //The same sort of logic is being applied to the cursor lock mode - if the user has clicked on one of the input pens nothing needs to update (aside from the hands apparant text) until the user presses enter
            if (theMenu.isHidden() && cursorLockMode == lockMode.unlocked)
            {
                //The menu is hidden so we only want the hand cursor to show
                this.IsMouseVisible = false;

                //Check if we are on the base frame (usually main), if we are then change the colour of the return button
                if (theWorldTracker.areOnBaseFrame())
                    interactiveSpriteCollection.getPrimaryDynamicSprite("returnbutton").spriteTintColour = Color.Black;
                else
                    interactiveSpriteCollection.getPrimaryDynamicSprite("returnbutton").spriteTintColour = Color.White;


                //Having the following line meant that the stack frame was reset on every tick, leading to some strange behaviour. Now it is only set when the stack state changes (push/pop)
                //assignCurrentFrameSprites(30, 390, globalStack.getTopFrame().getVariablesList());


                updateCurrentFrameSpritesText(gameTime);

                currentFrameSpritesCollection.Update(gameTime, keyboardOldState, keyboardNewState, mouseOldState, mouseNewState);

                updateParameterValueTexts();


                //TODO - put this next line in a more sensible location, so that we don't re-declare the background sprites for each update
                assignBackgroundStackSprites(10, 200, theWorldTracker.getStackSize());

                foreach (dynamicSprite sprt in backgroundStackSprites)
                {
                    sprt.Update(gameTime, keyboardOldState, keyboardNewState, mouseOldState, mouseNewState);
                }

                interactiveSpriteCollection.Update(gameTime,keyboardOldState,keyboardNewState,mouseOldState,mouseNewState);

                updateMethodSprites(gameTime);

                updateCursorSprite(gameTime);

                #region sprite text updates
                //update the text of the calculator
                updateCalculatorText(theWorldTracker.theCalculator.ToString());

                
                updateHandTextValue(theWorldTracker.getNoneNullHandValue());

                updateRobotText();

                //This used to update the text of the single method controlling lever...now it's here just as a reminder of our history ;)
                //updateFunctionCallLeverText();

                updateFunctionNameButtonText();
                #endregion



                //height and width are Math.Sqrt(BYTES_IN_THE_HEAP/4) because there are usually 4 bytes in a chunk and we are simply making the heap square
                heapGridDrawer.Update(gameTime, ConversionHelper.convertHeapToSPDrawFormat(theWorldTracker.globalHeap, mouseNewState, heapGridDrawer,
                                        (int)Math.Sqrt(BYTES_IN_THE_HEAP / 4), (int)Math.Sqrt(BYTES_IN_THE_HEAP / 4)));
            }
            else
            {
                //Getting here either we have the menu open or we have locked the cursor while waiting for user input

                //Check if we need to wait for user input
                if (cursorLockMode != lockMode.unlocked)
                {
                    //todo - consider disabling the magnification sprite here
                    
                    //pressing enter means the user has finsished with their input so we can 
                    if (isKeyPressed(Keys.Enter))
                    {
                        performValueInputOperation();
                        cursorLockMode = lockMode.unlocked;
                        this.IsMouseVisible = true;
                        //change all the notepad sprites back to having their pencils:
                        interactiveSpriteCollection.changeAllPrimaryImagesTo("intpencil", "notepadpencil");
                        interactiveSpriteCollection.changeAllPrimaryImagesTo("doublepencil", "notepadpencil");
                        interactiveSpriteCollection.changeAllPrimaryImagesTo("boolpencil", "notepadpencil");
                    }
                    else
                    {
                        //getting here meanst he user has not yet pressed enter, thus this is where we need to start the process of input storage
                        this.IsMouseVisible = false;

                        buildUpValueInputString();



                         if (DateTime.Now.Second % 2 == 0)
                            cursorSprite.updateText(-170, 150, 2.0f, builtUpUserInput + "I");
                         else
                             cursorSprite.updateText(-170, 150, 2.0f, builtUpUserInput);
                         
                    }
                }
                else
                {
                    //The menu is showing so we want to stop drawing the hand and let the user pick from the menu
                    this.IsMouseVisible = true;
                }
            }



            Window.Title = string.Format("X coord = {0}, Y cooord = {1}", mouseNewState.X, mouseNewState.Y);

            keyboardOldState = keyboardNewState;
            mouseOldState = mouseNewState;


            base.Update(gameTime);

            if (reUpdate)
            {
                reUpdate = false;
                Update(gameTime);
            }
        }

        /// <summary>
        /// This string keeps track of the keys that the user has pressed while in a lockedcursor input mode
        /// </summary>
        string builtUpUserInput = "";

        /// <summary>
        /// This function be called whenever the user finishes writting on their hand, and will decide what operation to perform based on the cursorLock state.
        /// For example if the cursorLockstate is waiting for an int and the user input 123 this method will attempt to perform the operation: directHandWrite int 123
        /// </summary>
        private void performValueInputOperation()
        {
            switch (cursorLockMode)
            {
                case(lockMode.intLocked):
                    directHandWrite("int " + builtUpUserInput);
                    break;
                case (lockMode.doubleLocked):
                    directHandWrite("double " + builtUpUserInput);
                    break;
                case (lockMode.boolLocked):
                    directHandWrite("bool " + builtUpUserInput);
                    break;
                default:
                    directHandWrite(builtUpUserInput);
                    break;

            }

            builtUpUserInput = "";
        }
        
        private void buildUpValueInputString()
        {
            //Backspace functionality
            if (builtUpUserInput.Length > 0 && isKeyPressed(Keys.Back))
            {
                builtUpUserInput = builtUpUserInput.Remove(builtUpUserInput.Length - 1); //remove the last charachter
                return;
            }

            //Charachter case checking via Shift-key dependant delegates
            #region decide how to handle case conversion
            Func<string, string> caseConverter;
            if (keyboardNewState.IsKeyDown(Keys.LeftShift) || keyboardNewState.IsKeyDown(Keys.RightShift))
            {
                caseConverter = (string x) => { return x.ToUpper(); };
                    
                //    delegate(string keyAsString)
                //{
                //    return keyAsString.ToUpper();
                //};
            }
            else
            {
                caseConverter = delegate(string keyAsString)
                {
                    return keyAsString.ToLower();
                };
            }
            #endregion

            var pressedKeys = keyboardNewState.GetPressedKeys();
            foreach (Keys key in pressedKeys)
            {
                if (!isKeyPressed(key)) //if the key isn't pressed then don't use it
                    continue;


                string asString = key.ToString();

                //is it a normal letter?
                if (asString.Length == 1)
                {
                    builtUpUserInput += caseConverter(asString);
                    continue;
                }
                
                //is it a number off the normal number keys?
                if (asString.Length == 2 && asString[0] == 'D')
                {
                    builtUpUserInput += asString[1].ToString(); //no need to change the case if its a number
                    continue;
                }

                //is it a number off the numpad?
                if (asString.Contains("NumPad"))
                {
                    builtUpUserInput += asString[6]; //no need to change the case if its a number
                    continue;
                }

                //Any extra charachters desired can be included in this dictionary...if you want charachters like '&' which would derive from keys.D5 (when cap or shift os on) then this whole arrangement needs to be shuffled around a bit
                Dictionary<string, string> remainingConversions = new Dictionary<string, string>() { { "Space", " " }, { "OemPeriod", "." }, { "Decimal", "." } };
                if(remainingConversions.ContainsKey(asString))
                    builtUpUserInput += remainingConversions[asString];

            }
            
            
        }


        /// <summary>
        /// Updates the position and apperance of both the cursor sprite and the actual mouse cursor, so there is a clear division between user and memory space
        /// (is not responsible for the cusrorsprites text, which is updated using cursorSprite.updateText())
        /// </summary>
        /// <param name="gameTime"></param>
        private void updateCursorSprite(GameTime gameTime)
        {
            //the the usual defaults
            
            int newX = mouseNewState.X;
            int newY = mouseNewState.Y;
            
            IsMouseVisible = false;

            string currentSprite = cursorSprite.getPrimarySpriteImageName();

            
            //now check if the defaults need changing
            //checks for if the mouser has gone into the computer space:
            if (newX > 880)
            {
                newX = 880;
                this.IsMouseVisible = true;
            }
            if (newY < 300)
            {
                newY = 300;
                this.IsMouseVisible = true;
            }

            //checks for if the mouse has left the screen:
            if (newX < 1)
                newX = 1;
            //for a limite at the very bottom of the screen use 890-900...for a limit that forces the content of the hand to always be visible use 733-770
            if (newY > 750)
                newY = 750;

            if (currentSprite == "handpencil")
            {
                newY = newY - 180;
                newX = newX + 45;
                this.IsMouseVisible = true;
            }
            if (currentSprite == "handvalue")
            {
                newY = newY - 0;
                newX = newX - 20;
             //   this.IsMouseVisible = true;
            }

            if (!theWorldTracker.thePlayer.isHandEmpty() && currentSprite != "handvalue")
                cursorSprite.setAllSpriteTextures("handvalue");
            

            //"-50" to accomodate the difference between point(0,0) of the sprite and the position of the finger tip 
            cursorSprite.Update(gameTime, newX-28, newY, keyboardOldState, keyboardNewState, mouseOldState, mouseNewState);
        }

        private void updateMethodSprites(GameTime gameTime)
        {
            int firstUnassignedIndex = theWorldTracker.methodMech.getIndexOfFirstUnasignedParameter();

            foreach (var superTuple in functionCallingSpritesCollection.getEnumerator())
            {
                //check if it's an odd numbered member of the collection (i.e. a parameter and not a background box)
                if (superTuple.Item1 % 2 == 1)
                {
                    //decide if we need to highlight all white or all gray
                    if (superTuple.Item1 / 2 <= firstUnassignedIndex || firstUnassignedIndex == -1)
                    {
                        functionCallingSpritesCollection.setCustomPrimarySpriteTint(superTuple.Item2,
                             new Tuple<Color, Color, Color, Color, Color>(Color.White, Color.White, Color.White, Color.White, Color.White));
                       
                    }
                    else
                    {
                        functionCallingSpritesCollection.setCustomPrimarySpriteTint(superTuple.Item2,
                             new Tuple<Color, Color, Color, Color, Color>(Color.Gray, Color.Gray, Color.Gray, Color.Gray, Color.Gray));
                    }
                }
            }
            functionCallingSpritesCollection.Update(gameTime, keyboardOldState, keyboardNewState, mouseOldState, mouseNewState);

        }

        //////////////

        #region view refreshing methods (*usually* passed as finishingActions for calls to the worldtracker)

        /// <summary>
        /// this method is so that when you want to change the details of how frame variables are assigned you can make one change rather 
        /// than changing alll the many calls to the same function that essentially use the same values throughout the code
        /// </summary>
        private void assignCurrentFrameSpritesAsAction()
        {
            assignCurrentFrameSprites(30, 390, theWorldTracker.globalStack.getTopFrame().getVariablesList());
        }

        private void assignFunctionCallingSpritesAsAction()
        {
            assignFunctionCallingSprites(50, 500, theWorldTracker.methodMech);
        }

        private void resetCurrentMethodMechanism()
        {
            //resetCurrentMethodMechanism only ever gets called when a method is called (as the parameters need to be cleared away), at which point the current frame sprites need to be reset anyway to reflect the new frame
             assignCurrentFrameSpritesAsAction();

             interactiveSpriteCollection.setCustomPrimarySprite("namingbutton", new dynamicSprite(65, 600, 0.1f, "functionNameButton", "functionNamedAndPressed", "functionNameButton", "functionNameButton", new List<interactiveRegion> 
                     {
                         new interactiveRegion(0,0,100,100,beginMethodMenuSelection,interactiveRegion.interactionModes.mouse_left_press),
                     }, font));
             interactiveSpriteCollection.LoadSingleSpriteContent("namingbutton", Content);
            
        }
             

        private void updateHandTextValue(string newValue)
        {
            //TODO: remove the 'magic numbers' used here for size and position...if possible. And elseware also          
            
            //cursorSprite.updateText(135, 90, 2.0f, newValue);
            if (!theWorldTracker.thePlayer.isHandEmpty() && showHandValueType)
                cursorSprite.updateText(30, 120, 2.0f, theWorldTracker.thePlayer.examineHeld().readType() + ":\n" + newValue);
            else
                cursorSprite.updateText(30, 150, 2.0f, newValue);
        }


        bool showHandValueType = true;
        private void updateCalculatorText(string expression)
        {
            //interactiveSprites[4].updateText(0, 0, 0.8f, expression);
            interactiveSpriteCollection.getPrimaryDynamicSprite("calculator").updateText(0, 0, 0.8f, expression);
        }

        private void updateCurrentFrameSpritesText(GameTime gameTime)
        {
            int i;
            foreach (var superTuple in currentFrameSpritesCollection.getEnumerator())
            {
                i = superTuple.Item1;
                if (i % 2 == 1 && i / 2 < theWorldTracker.getTotalLocalFrameVariables())
                {
                    
                    //get the variables VALUE
                    string text = theWorldTracker.getNthLocalVariableValue(i / 2);
                    //check if it's a default value
                    if (theWorldTracker.localVariableHasDefaultValue(i / 2))
                        text = "???"; //if it is then display questionmarks

                    //at this point we have the VALUE but need a name
                    text = theWorldTracker.getNthLocalVariableName(i / 2) + "\n" + text;
                    superTuple.Item3.updateText(10, 0, 1.0f, text);

                    //this is what it was for when shifting between top and bottom views of the variable boxes:
                    //if (superTuple.Item3.isSpriteHoveredOver())
                    //{
                    //    string text = theWorldTracker.getNthLocalVariableValue(i / 2);
                    //    //check if the variable being hovered over has its default value from when it was declared...if it does then we need to mark it as unkown
                    //    if (theWorldTracker.localVariableHasDefaultValue(i / 2))
                    //        superTuple.Item3.updateText(20, 10, convertStringToScale("???", 25), "???");
                    //    else
                    //        superTuple.Item3.updateText(20, 10, convertStringToScale(text, 25), text);
                    //}
                    //else
                    //{
                    //    string text = theWorldTracker.getNthLocalVariableType(i / 2);
                    //    float scale = convertStringToScale(text, 40);
                    //    superTuple.Item3.updateText(5, 30 - (int)(30 * scale), scale, text);
                    //}
                }
            }
            currentFrameSpritesCollection.Update(gameTime, keyboardOldState, keyboardNewState, mouseOldState, mouseNewState);
        }

        private void updateFunctionNameButtonText()
        {
            interactiveSpriteCollection.getPrimaryDynamicSprite("namingbutton").updateText(50, 20, 0.8f, theWorldTracker.methodMech.methodName);
        }
        
        private void updateParameterValueTexts()
        {
            int endBoundary = functionCallingSpritesCollection.Count;

            if(haveParameterAddingSprite)
                endBoundary = functionCallingSpritesCollection.Count - 2;

            foreach (var superTuple in functionCallingSpritesCollection.getEnumerator())
            {
                int i = superTuple.Item1;
                if (i % 2 == 1 && i / 2 < theWorldTracker.getParameterCount())
                {
                    //get the VALUE
                    string text = theWorldTracker.methodMech.parameterValues[(i / 2)].read();
                    //check if it's a default value
                    if (theWorldTracker.methodMech.parameterValues[i / 2].readOrigin() == "defaultParameter") 
                        text = "???"; //if it is then display questionmarks

                    //at this point we have the VALUE but need a name
                    text = theWorldTracker.methodMech.paramNames[i / 2] + "\n" + text;
                    superTuple.Item3.updateText(10, 0, 1.0f, text);

                    //if (superTuple.Item3.isSpriteHoveredOver())
                    //{
                    //    string text = theWorldTracker.methodMech.parameterValues[(i / 2)].read();
                    //    if (parametersCanAppearBlank && 
                    //        theWorldTracker.methodMech.parameterValues[i / 2].readOrigin() == "defaultParameter") //check if it has had a value assigned to it...if so then display blank
                    //        text = "???";
                    //    superTuple.Item3.updateText(20, 10, convertStringToScale(text, 25), text);
                    //}
                    //else
                    //{
                    //    string text = theWorldTracker.methodMech.paramNames[i / 2];
                    //    float scale = convertStringToScale(text, 40);
                    //    superTuple.Item3.updateText(5, 30 - (int)(30 * scale), scale, text);
                    //}
                }
            }
        }


        private void updateRobotText()
        {
            string newText = "\n";
            if (theWorldTracker.theMemManager.heldValue != null)
                newText = "X" + newText;

            if (theWorldTracker.theMemManager.heldMemoryAddress != -1)
                newText += "X";

            //TODO: remove the 'magic numbers' used here for size and position...if possible
            //interactiveSprites[3].updateText(57, 132, 0.46f, newText);
            interactiveSpriteCollection.getPrimaryDynamicSprite("robot").updateText(57, 132, 0.46f, newText);
        }

        #endregion

        private void displayMouseTextAction(string lmbAction, string rmbAction, float scale = 1f, bool includeWhiteSpace = true, int xDisplacement = 0, int yDisplacement = 0)
        {
            //TODO: add calculations based on the size of the instructions  to determine position
            //needs to make use of scal or else things might go ...weird


            for (int i = lmbAction.Length; i < 10 && includeWhiteSpace; i++)
            {
                lmbAction += " ";
            }

            interactiveSpriteCollection.getPrimaryDynamicSprite("mouse").updateText(16 + xDisplacement, 180 + yDisplacement, scale, lmbAction + " " + rmbAction);
        }

        #region action text methods (for assigning text to the instruction/hint mouse)

        private void actionTextWriteRead()
        {
            displayMouseTextAction("Write","Read\n\n  Variable\n    <-->\n   Hand\n     IO\n",1.4f,false,3,-15);
        }

        private void actionTextWrite()
        {
            displayMouseTextAction("\n\n   Assign\n Parameter", "",1.5f,true,-5,-15);
        }

        private void actionTextDeclareParameter()
        {
            displayMouseTextAction("\n\nDeclare\nParameter", "");
        }

        private void actionTextDeclare()
        {
            displayMouseTextAction("\n\n Declare\n Variable", "",1.5f);
        }

        private void actionTextDeleteVariable()
        {
            displayMouseTextAction("\n\n Remove\n Variable \n (scope)", "");
        }

        private void actionTextCalculator()
        {
            /*need to tell the user that they can do these 4 things:
             - Write a new expression to the calculator --- only when the calclator doesn't contain a simplifiable thing (you shouldn't go half
                                                            way through an expression and then start a different one, unless your going into a method)
                                                            (though perhaps even at any time)
             - Evaluate the current expression  ---when the current expression contains no variables or is in its simplest form (tough to guess/evaluate)
             - Read the current value to the hand ---only when the calculator expression, when executed, doesn't simplify 
             - Write the current hand value to a variable in the calculator --only when the calculator expression, when executed, comes back as type expression
             */

            //if the expression is in it's simplest form (i.e. no subtitution or evaluations possible)
            if (theWorldTracker.theCalculator.isSimplest())
            {
                //left - read
                //right - new expression
                displayMouseTextAction("Read ", "New\n       Expr.\n\nRead value\nto the hand,\nOr write new\n expression", 1.2f, false,8,-32);
                return;
            }

            if (theWorldTracker.theCalculator.isUnsimplifiable())
            {
                //left - substitute
                //right - nothing
                displayMouseTextAction("\n\n Substitute in\n Hand Value", "",1.2f);
                return;
            }

            if (theWorldTracker.theCalculator.isNewExp())
            {
                // left - evaluate
                // right - substitute
                displayMouseTextAction("Evaluate", "Sub Var", 0.8f);
                return;
            }

            //we should in theory never get here...if we do something is wrong;
            throw new Exception("You reached a location of code that should be unreachable");

        }



        private void actionTextGarbageCollect()
        {
            displayMouseTextAction("\n\n   Clean\n   Heap", "", 1.5f);
        }

        private void actionTextCallButton()
        {
            //Only display the call text if the call button is usable
            if(theWorldTracker.isMethodNamed())
                displayMouseTextAction("\n\n   Call", "", 1.7f);
        }

        private void actionTextDirectHandWrite()
        {
            displayMouseTextAction("\n New Value:\n  Result of\n   current\n expression", "",1.3f);
        }

        private void actionTextReturnButton()
        {
            displayMouseTextAction("\n\n  Return", "", 1.5f);
        }

        private void actionTextMethodButton()
        {
            displayMouseTextAction("\nName new\nmethod to\n   call", "", 1.5f);
        }

        #endregion

        #region key and mouse checks (not for passing anywhere)
        private bool actionIsSafe(Keys theKey)
        {
            if (Guide.IsVisible == false && isKeyPressed(theKey))
                return true;
            return false;
        }

        private bool isKeyPressed(Keys keyPressed)
        {
            return keyboardOldState.IsKeyUp(keyPressed) && keyboardNewState.IsKeyDown(keyPressed);
        }

        private bool isLMBPressed()
        {
            return mouseOldState.LeftButton == ButtonState.Pressed && mouseNewState.LeftButton == ButtonState.Released;
        }

        private bool isKeyReleased(Keys keyPressed)
        {
            return keyboardOldState.IsKeyDown(keyPressed) && keyboardNewState.IsKeyUp(keyPressed);
        }

        #endregion

        private float convertStringToScale(string theText, float maxPixelWidth)
        {
            if (theText.Length < 5)
                return 1.0f;
            float avg = 8.5f;
            return (maxPixelWidth / (theText.Length * avg));
        }

        private void unfadeSprites()
        {
            fadeSprites(Color.White, "unfade");
        }

        /// <summary>
        /// Gives the requested sprites a tinting of some colour, and if said colour has an opaque alpha in will be lowered to 200
        /// </summary>
        /// <param name="fadeShade">the shading mode to use, or which sprites will be faded/tinted "all", "allbuthelp","unfade"</param>
        /// <param name="mode"></param>
        private void fadeSprites(Color fadeShade, string mode = "allbuthelp")
        {
            //if the user wants to define a fade level then he can, but 255 defaults to an alpha value of 200 (or partially tranparant)
            if(fadeShade.A == 255 && mode != "unfade")
                fadeShade.A = 200;
            

            switch (mode.ToLower())
            {

                case("all"):
                    foreach (var superTuple in functionCallingSpritesCollection.getEnumerator())
                        superTuple.Item3.spriteTintColour = fadeShade;
                    
                    foreach(var sprt in backgroundStackSprites)
                        sprt.spriteTintColour = fadeShade;

                    foreach (var superTuple in currentFrameSpritesCollection.getEnumerator())
                        superTuple.Item3.spriteTintColour = fadeShade;
                                
                    //foreach(var sprt in interactiveSprites)
                    //        sprt.Value.spriteTintColour = fadeShade;
            
                    cursorSprite.spriteTintColour = fadeShade;
                    break;

                case ("unfade"):
                case("allbuthelp"):
                    foreach (var superTuple in functionCallingSpritesCollection.getEnumerator())
                        superTuple.Item3.spriteTintColour = fadeShade;
                    
                    foreach(var sprt in backgroundStackSprites)
                        sprt.spriteTintColour = fadeShade;

                    foreach (var superTuple in currentFrameSpritesCollection.getEnumerator())
                        superTuple.Item3.spriteTintColour = fadeShade;
                                
                    //foreach(var sprt in interactiveSprites)
                    //    if (sprt.Key != "help" && sprt.Key != "intro")
                    //       sprt.Value.spriteTintColour = fadeShade;
            
                    cursorSprite.spriteTintColour = fadeShade;
            
                    //TODO consider how we would fade out the heap...
                    //heapGridDrawer
                    break;
            }
            

            

        }
        
        private void emailErrorCountDetails()
        {
            //main resource here: http://msdn.microsoft.com/en-us/library/swas0fwc(v=vs.100).aspx
            //got help from here:https://stackoverflow.com/questions/449887/sending-e-mail-using-c-sharp

            return; //TODO -> remove this return so that the method will actually work. Also change the password to be user defined (and possibly the from address also), so that we don't save password ANYWHERE
            string host = "mail.ru.ac.za";
            string to = "g08f0016@campus.ru.ac.za";
            string from = "g08f0016@campus.ru.ac.za";

            MailMessage message = new MailMessage(from, to);
            message.Subject = "Error report for user X";
            message.Body = string.Format("User had {0} errors on level {1}",theWorldTracker.totalErrors,theWorldTracker.currentLevel.name);
            SmtpClient client = new SmtpClient(host);
            // Credentials are necessary if the server requires the client 
            // to authenticate before it will send e-mail on the client's behalf.
            
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential basicAuthenticationInfo = new
                    System.Net.NetworkCredential("g08f0016", "shhh its a secret");
            client.Credentials = basicAuthenticationInfo;

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                      ex.ToString());
            }
        }

        private void diplayLevelLoadMenu()
        {
            string[] levelList;
            string directory = @"...\...\...\levels\";

            if (!Directory.Exists(directory))
            {
                directory = @"levels\";
                if(!Directory.Exists(directory))
                {
                    levelList = new string[0];
                    theMenu.displayMenu(levelList);
                    return;
                }
            }

            //search through the levels diretory for all the .xml files and turn them into an array of level names (as strings)
            levelList = Directory.GetFiles(directory,"*.xml");

            //remove all the file extensions and location data as the levels are to be displayed as simple names, rather than literal file directories
            for (int i = 0; i < levelList.Length; i++)
            {
                 levelList[i] = levelList[i].Replace(directory, "");
                 levelList[i] = levelList[i].Replace(".xml", "");
            }


            //diplay the menu, and make its buttons the levels that we found above
            theMenu.displayMenu(levelList);
        }

        private void loadNewLevel(string levelName)
        {
            string baseDirectory = "...\\...\\...\\levels\\";
            if (Directory.Exists(baseDirectory))
                currentLevelDirectory = baseDirectory + levelName;
            else
            {
                baseDirectory = "levels\\";
                if (Directory.Exists(baseDirectory))
                    currentLevelDirectory = baseDirectory + levelName;
            }

            if (!currentLevelDirectory.Contains(".xml"))
                currentLevelDirectory += ".xml";

            if (currentLevelDirectory == baseDirectory + ".xml" || currentLevelDirectory == baseDirectory + "empty.xml" || currentLevelDirectory == baseDirectory + "sandbox.xml")
            {
                freePlay = true;
                theWorldTracker = new WorldTracker(baseDirectory+"empty.xml");
            }
            else
            {
                freePlay = false;
                theWorldTracker = new WorldTracker(currentLevelDirectory);
            }

            theWorldTracker.setOperationCompletionActions(setSuccessTime, setFailTime);
            LoadContent();
            //This line is inlcuded so that the intro screen is not displayed every time a new level is opened:
            interactiveSpriteCollection.getPrimaryDynamicSprite("help").spriteTintColour = Color.Transparent;

            guideHintText_PencilClick = "<type> <value>";
        }


        /// <summary>
        /// used to set the lastSucess time to the current time. Should be used when operations are completed sucessfully
        /// </summary>
        private void setSuccessTime()
        {
            lastSuccess = DateTime.Now;
        }

        /// <summary>
        /// used to set the lastFail time to the current time. Should be used when operations are failed
        /// </summary>
        private void setFailTime()
        {
            lastFailure = DateTime.Now;
        }

        /// <summary>
        /// Based on the last failure and sucessful operation the approriate texture for the feedback sprite is selected using this method
        /// </summary>
        private void updateFeedbackSpriteTexture()
        {
            //if both the last successful and failed operation was 3 or more seconds ago then we don't need to change anything becuase the texture should be back to feedbackblank
            //Console.WriteLine("{0}, {1}", lastSuccess.CompareTo(DateTime.Now.AddSeconds(3.0)), lastFailure.CompareTo(DateTime.Now.AddSeconds(3.0)));
            if (lastSuccess.AddSeconds(3.0).CompareTo(DateTime.Now) == -1 && lastFailure.AddSeconds(3.0).CompareTo(DateTime.Now) == -1)
                return;

            if (theWorldTracker.currentLevel.isComplete)
            {
                interactiveSpriteCollection.getPrimaryDynamicSprite("feedback").setAllSpriteTextures("feedbackDone");
                return;
            }


            //check if BOTH are within the 2 second display period...whicher one is older can be made to look much older, that way the newest will be displayed
            if (lastSuccess.AddSeconds(3.0).CompareTo(DateTime.Now) == 1 && lastFailure.AddSeconds(3.0).CompareTo(DateTime.Now) == 1)
            {
                if (lastSuccess.CompareTo(lastFailure) == -1)
                    lastSuccess = DateTime.MinValue; //lastsuccess was older was lastfailure so reset it
                else
                    lastFailure = DateTime.MinValue; //lastfailure was older was lastsuccess so reset it
            }

            if (lastSuccess.AddSeconds(2.0).CompareTo(DateTime.Now) == -1 && lastFailure.AddSeconds(2.0).CompareTo(DateTime.Now) == 1)
            {
                interactiveSpriteCollection.getPrimaryDynamicSprite("feedback").setAllSpriteTextures("feedbackCross");
                return;
            }

            if (lastSuccess.AddSeconds(2.0).CompareTo(DateTime.Now) == 1 && lastFailure.AddSeconds(2.0).CompareTo(DateTime.Now) == -1)
            {
                interactiveSpriteCollection.getPrimaryDynamicSprite("feedback").setAllSpriteTextures("feedbackTick");
                return;
            }

            //getting here means that the last failure or success was less than 3 seconds ago, but more than 2 seconds ago...this line gives a 1 second interval where we can reset the feedback sprite
            interactiveSpriteCollection.getPrimaryDynamicSprite("feedback").setAllSpriteTextures("feedbackNone");
        }

        private void loadNewLevel(IAsyncResult levelName)
        {
            currentLevelDirectory = "...\\...\\...\\levels\\" + Guide.EndShowKeyboardInput(levelName);
            if (!currentLevelDirectory.Contains(".xml"))
                currentLevelDirectory += ".xml";

            if (currentLevelDirectory == "...\\...\\...\\levels\\.xml")
            {
                freePlay = true;
                theWorldTracker = new WorldTracker("...\\...\\...\\levels\\empty.xml");
            }
            else
            {
                freePlay = false;
                theWorldTracker = new WorldTracker(currentLevelDirectory);
            }
            LoadContent();
        }

        /// <summary>
        /// A function that simply sets up some starting state so that we don't have to set everything up from scratch during testing
        /// </summary>
        private void exampleStackStart()
        {

        }

        

    }
}
