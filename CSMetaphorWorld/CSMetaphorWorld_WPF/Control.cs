using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CSMetaphorWorld;
using System.IO;

using System.Linq;

namespace CSMetaphorWorld_WPF
{
    /// <summary>
    /// A structure to keep track of all the details of a Sprite: if the sprites contents never change then this isn't necessary,
    /// but if we want to alter things from inside Control we need to have a reference to those things that might need changing.
    /// If your concerned for efficiency and space usage, just remeber that all the reference types in here have to exist elseware
    /// therefore they aren't taking up any more space than an integer (or address) each
    /// </summary>
    class Sprite
    {
        
        public string imageAddress;
        public double width;
        public double xCoord;
        public double yCoord;
        public Image imageComponent;
        public Action<object, EventArgs> mouseLeftUpAction;

        /// <summary>
        /// This label is used to show the underlying state (from the Model) that corresponds to a particular sprite
        /// </summary>
        public Label infoLabel;

        /// <summary>
        /// A tracker for as may events as a sprite might need to handle, along with the action to be taken with each event.
        /// </summary>
        public Dictionary<string, Action<object, EventArgs>> event_ActionPairs;


        public Sprite(string imageAddress, double width, double xCoord, double yCoord, Action<object, EventArgs> upAction = null, Dictionary<string, Action<object, EventArgs>> event_ActionPairs = null)
        {
            this.width = width;
            this.imageAddress = imageAddress;
            this.xCoord = xCoord;
            this.yCoord = yCoord;
            imageComponent = null;
            if (upAction != null)
                mouseLeftUpAction = upAction;
            else
                mouseLeftUpAction = (object senderx, EventArgs ex) => { };

            if (event_ActionPairs != null)
                this.event_ActionPairs = event_ActionPairs;
            else
                this.event_ActionPairs = new Dictionary<string, Action<object, EventArgs>>();
        }
    }

    /// <summary>
    /// Used to facilitate communication between the Model Classes and the View Classes...doesn't have to keep them as fields but it might make things easier
    /// </summary>
    class Control
    {
        string currentLevelDirectory = "...\\...\\...\\levels\\empty.xml";
        public bool freePlay = true;

        public bool hintsOn = false;


        LevelCodeInterface textualInterface;

        /// <summary>
        /// The MainWindow that this particular instance belongs to/with
        /// We need to be able to alter our parent window as much as it needs to be able to alter us
        /// </summary>
        public MainWindow parentWindow = null;

        /// <summary>
        /// The logic in the background - keeps track of all the non-visual state of the game
        /// </summary>
        public WorldTracker Model;

        public View theView;

        public Dictionary<string,Sprite> theMainSpritesList;

        public Dictionary<string, Sprite> methodMechanismSpriteList;

        public Dictionary<string, Sprite> frameSprites;

        public List<Sprite> stackSprites;

        public void createStackFrameSprites(double startXPos, double startYPos, double width, double height, int stackSize, bool removeOld = true)
        {
            if (removeOld && stackSprites != null) //if things do already exist we need to check if we are supposed to remove the old ones
            {
                foreach (var sprite in stackSprites)
                {
                    parentWindow.canvas_Main.Children.Remove(sprite.imageComponent);
                    parentWindow.canvas_Main.Children.Remove(sprite.infoLabel);
                }
            }
            height = 500;
            double scale = 0.9;
            double currentX = startXPos; //Whether we build up or down, the starting X position will always be the same
            double currentY = startYPos; //Shifts everything up from the bottom y coord

            stackSprites = new List<Sprite>();


            for (int i = 0; i < stackSize - 1; i++)
            {
                stackSprites.Insert(0,new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\topDownShelf.png", width, currentX, currentY));
                scale = scale * 0.98f;
                width *= scale;

                currentX += 25;

                currentY = startYPos - (70 * (i + 1) * scale);
            }


            foreach (var sprite in stackSprites)
            {
                sprite.imageComponent = theView.addInteractiveImageToCanvas(sprite.imageAddress, sprite.width, sprite.xCoord, sprite.yCoord, sprite.mouseLeftUpAction);
                sprite.imageComponent.MouseMove += shuffle;

                //this loop is just for consistency, in case we add interactive functionality to the stack sprites later
                foreach (var eventActionPair in sprite.event_ActionPairs)
                {
                    addActionToEvent(sprite.imageComponent, eventActionPair.Key, eventActionPair.Value);
                }

                sprite.imageComponent.ToolTip = "Non-Local Frames on the stack";
            }


        }

        public void createLocalFrameSprites(double startXPos, double startYPos, int width, int height, bool removeOld = true)
        {
            if (removeOld && frameSprites != null) //if things do already exist we need to check if we are supposed to remove the old ones
            {
                foreach (var pair in frameSprites)
                {
                    parentWindow.canvas_Main.Children.Remove(pair.Value.imageComponent);
                    parentWindow.canvas_Main.Children.Remove(pair.Value.infoLabel);
                }
            }

            frameSprites = new Dictionary<string, Sprite>();


            double scale = 0.9;

            // int width = (int)(image.Width * scale - 6) - 2;
            // int height = (int)(image.Height * scale - 6) - 2;

            int totalLocalVariables = Model.globalStack.getTopFrame().getVariablesList().Count;
            double currentX = startXPos; //Whether we build up or down, the starting X position will always be the same
            double currentY = startYPos - ((height * scale) * (totalLocalVariables / 4)); //Shifts everything up from the bottom y coord

            for (int i = 0; i < totalLocalVariables; i++)
            {
                frameSprites.Add("variablepanel" + i, new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\singleShelfBrickNoBorder.png", width, currentX, currentY,
                                                (object senderx, EventArgs ex) => { }));

                int temp = i;
                frameSprites.Add("variablebox" + i, new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\variableBoxVersion2.png", width * 0.6, currentX + 25, currentY + 20,
                                               (object senderx, EventArgs ex) =>
                                               {
                                                   //using 'i' here is unsafe as it's value gets changed in the outer loop, wheras temp exists uniquely for each iteration
                                                   string varName = Model.getNthLocalVariableName(temp);
                                                   Model.performOperation("assignvalue " + varName, reflectModelStateOnView, freePlay);
                                               },
                                                new Dictionary<string, Action<object, EventArgs>>() 
                                                {
                                                    {Image.MouseRightButtonUpEvent.ToString() ,(object senderx, EventArgs ex) => 
                                                    {
                                                        string varName = Model.getNthLocalVariableName(temp);
                                                        Model.performOperation("readvariable " + varName, reflectModelStateOnView, freePlay);
                                                    }},
                                                }));


                currentX = startXPos + ((width * scale) * ((i + 1) % 4));
                currentY = startYPos - ((height * scale) * ((totalLocalVariables / 4) - ((i + 1) / 4))); //work out how far from the bottom we need to draw
            }


            frameSprites.Add("finallegobrick", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\singleShelfBrickNoBorder.png", width, currentX, currentY,
                                                (object senderx, EventArgs ex) => { }));

            //New variable declaration
            frameSprites.Add("addlocal", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\addsign.png", width * 0.4, currentX + 35, currentY + 27,
                            (object senderx, EventArgs ex) => 
                                {
                                    performOperationThroughTextBox("declarevariable ", 100, currentX + 40, currentY + 40);
                                }));




            foreach (var pair in frameSprites)
            {
                pair.Value.imageComponent = theView.addInteractiveImageToCanvas(pair.Value.imageAddress, pair.Value.width, pair.Value.xCoord, pair.Value.yCoord, pair.Value.mouseLeftUpAction);
                pair.Value.imageComponent.MouseMove += shuffle;


                foreach (var eventActionPair in pair.Value.event_ActionPairs)
                {
                    addActionToEvent(pair.Value.imageComponent, eventActionPair.Key, eventActionPair.Value);
                }

                if (pair.Key.Contains("variablebox"))
                {
                    int index = Convert.ToInt32(pair.Key.Substring(11));
                    pair.Value.infoLabel = theView.addLabelToImage(pair.Value.imageComponent, Model.getNthLocalVariableAsCompleteString(index), 10, 10);
                }
                pair.Value.imageComponent.ToolTip = pair.Key;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="removeOld">Determines whether we remove the old sprites UIcomponents from the canvas they are on...
        /// I'm not sure when you wouldn't want to remove them but you have that option</param>
        public void createMethodMechanismSprites(double startXPos, double startYPos, int width, int height, bool removeOld = true)
        {
            if (removeOld && methodMechanismSpriteList != null) //if things do already exist we need to check if we are supposed to remove the old ones
            {
                foreach (var pair in methodMechanismSpriteList)
                {
                    parentWindow.canvas_Main.Children.Remove(pair.Value.imageComponent);
                    parentWindow.canvas_Main.Children.Remove(pair.Value.infoLabel);
                }
            }

            methodMechanismSpriteList = new Dictionary<string, Sprite>();


            double scale = 0.9;

           // int width = (int)(image.Width * scale - 6) - 2;
           // int height = (int)(image.Height * scale - 6) - 2;
            
            double currentX = startXPos; //Whether we build up or down, the starting X position will always be the same
            double currentY = startYPos - ((height * scale) * (Model.methodMech.parameterValues.Count / 4)); //Shifts everything up from the bottom y coord

            for (int i = 0; i < Model.methodMech.parameterValues.Count; i++)
            {
                methodMechanismSpriteList.Add("methodpanel"+i, new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\singleShelfBrickNoBorder.png", width, currentX, currentY,
                                                (object senderx, EventArgs ex) => { }));

                int temp = i;
                methodMechanismSpriteList.Add("parameterbox" + i, new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\variableBoxVersion2.png", width * 0.6, currentX + 25, currentY + 20,
                                               (object senderx, EventArgs ex) => {
                                                                                    //using 'i' here is unsafe as it's value gets changed in the outer loop
                                                                                    string paramName = Model.methodMech.getParameterXName(temp);
                                                                                    Model.performOperation("assignparameter "+paramName, reflectModelStateOnView, freePlay); 
                                                                                 }));
                

                currentX = startXPos + ((width * scale) * ((i + 1) % 4));
                currentY = startYPos - ((height * scale) * ((Model.methodMech.parameterValues.Count / 4) - ((i + 1) / 4))); //work out how far from the bottom we need to draw
            }

            if (Model.isMethodNamed())
            {
                methodMechanismSpriteList.Add("finalbackbrick", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\singleShelfBrickNoBorder.png", width, currentX, currentY,
                                                    (object senderx, EventArgs ex) => { }));

                //Method Mechanism Sprites
                methodMechanismSpriteList.Add("callbutton", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\unpresseduparrow.png", width*0.4, currentX + 35, currentY + 20,
                    (object senderx, EventArgs ex) => { Model.performOperation("callfunction", reflectModelStateOnView, freePlay); }));
            }


            
            foreach (var pair in methodMechanismSpriteList)
            {
                pair.Value.imageComponent = theView.addInteractiveImageToCanvas(pair.Value.imageAddress, pair.Value.width, pair.Value.xCoord, pair.Value.yCoord, pair.Value.mouseLeftUpAction);
                pair.Value.imageComponent.MouseMove += shuffle;

                if (pair.Key.Contains("parameterbox"))
                {
                    int index = Convert.ToInt32(pair.Key.Substring(12));
                    pair.Value.infoLabel = theView.addLabelToImage(pair.Value.imageComponent, Model.methodMech.parameterValues[index].read(), 10, 10);
                }
                pair.Value.imageComponent.ToolTip = pair.Key;
            }
        }

        public void initaliseMainSprites()
        {
            //certain 'imbedded' objects (like the value in a box on a shelf on a conveyor) could be nested inside one another, and then use relative positioning to make changes even easier...think about it

            //this is a suitable address if the main file isn't located relative to the orignal XNA content folder - using it requires copying the files into the Content folder of the CSmwWPF directory
            //new Sprite(@"...\...\Content\addSign.png", 50, 160, 100, (object senderx, EventArgs ex) => { Console.WriteLine("5");}),

            Dictionary<string, string> x = new Dictionary<string, string>() { {"",""} , {"d","D"}};

            theMainSpritesList = new Dictionary<string, Sprite>();

            #region no Actions
            theMainSpritesList.Add("conveyor", 
                                    new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\ConveyerBelt(WithPerspective).png", 400, 409, -5.31165311653115,
                                    (object senderx, EventArgs ex) => { },
                                    new Dictionary<string, Action<object, EventArgs>>() 
                                        {
                                            {Image.KeyDownEvent.ToString() ,(object senderx, EventArgs ex) => 
                                            {
                                                //This method (for some unkown reason) won't execute...
                                                Console.WriteLine("RUN PLEASE RUN!");
                                            }},
                                            {Image.MouseEnterEvent.ToString() ,(object senderx, EventArgs ex) => { Console.WriteLine("ENTER!"); }},
                                            {Image.MouseRightButtonUpEvent.ToString() ,(object senderx, EventArgs ex) => { Console.WriteLine("Mouse Right Up!"); }},
                                            {Image.ToolTipOpeningEvent.ToString() ,(object senderx, EventArgs ex) => { Console.WriteLine("SHOW THE HELP!"); }}
                                        }));


            
            theMainSpritesList.Add("barrier", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\memoryBarrier.png", 1400, -76, 298, 
                (object senderx, EventArgs ex) => { }));
            theMainSpritesList.Add("robot", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\Robot.png", 120, 301, 0, 
                (object senderx, EventArgs ex) => {/*robot is never clicked...so no click action*/}));

            theMainSpritesList.Add("table", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\functionTable.png", 400, 811, 367.605494892568, 
                (object senderx, EventArgs ex) => { }));

            theMainSpritesList.Add("feedback", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\feedbackNone.png", 200, 1342, 696, 
                (object senderx, EventArgs ex) => { }));
            #endregion

            
            theMainSpritesList.Add("terminal", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\Terminal.png", 120, 287, 370.355670103093, 
                (object senderx, EventArgs ex) => { performTerminalClick();}));

            theMainSpritesList.Add("methodbutton", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\functionNameButton.png", 50, 829, 463.759109311741, 
                (object senderx, EventArgs ex) =>
                {
                    int longestMenuOption = 0;
                    foreach (var item in Model.getTheCurrentLevelsMethods())
                    {
                        longestMenuOption = longestMenuOption < item.Length ? item.Length : longestMenuOption;
                    }
                    performOperationThroughMenu("preparemethod ", longestMenuOption * 6, theMainSpritesList["methodbutton"].xCoord + 5, theMainSpritesList["methodbutton"].yCoord + 30, Model.getTheCurrentLevelsMethods());
                }));

            theMainSpritesList.Add("intpad", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\notepadpencil.png", 150, 10, 389.56880733945,
                (object senderx, EventArgs ex) => 
                    {
                        performOperationThroughTextBox("directhandwrite int ", 120, theMainSpritesList["intpad"].xCoord + 5, theMainSpritesList["intpad"].yCoord + 30); 
                    }));

            theMainSpritesList.Add("doublepad", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\notepadpencil.png", 150, 10, 524.56880733945,
                (object senderx, EventArgs ex) =>
                {
                    performOperationThroughTextBox("directhandwrite double ", 120, theMainSpritesList["doublepad"].xCoord + 5, theMainSpritesList["doublepad"].yCoord + 30);
                }));

            theMainSpritesList.Add("boolpad", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\notepadpencil.png", 150, 11, 661.56880733945,
                (object senderx, EventArgs ex) =>
                {
                    performOperationThroughTextBox("directhandwrite bool ", 120, theMainSpritesList["boolpad"].xCoord + 5, theMainSpritesList["boolpad"].yCoord + 30);
                }));


            theMainSpritesList.Add("calculator", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\Calculator.png", 150, 569, 564.557438794727, 
                (object senderx, EventArgs ex) => { performCalulatorClick(); }));

            theMainSpritesList.Add("return", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\unpresseddownarrow.png", 100, 722, 528.818181818182, 
                (object senderx, EventArgs ex) => { Model.performOperation("return",reflectModelStateOnView, freePlay); }));
            theMainSpritesList.Add("bin", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\RecycleBin.png", 100, 176, 378.227467811159, 
                (object senderx, EventArgs ex) => { Model.performOperation("garbagecollect", reflectModelStateOnView, freePlay); }));
            theMainSpritesList.Add("booleater", new Sprite(@"F:\Dropbox\msc\CSMetaphorWorld\CSMetaphorWorld\CSMetaphorWorldContent\boolEater.png", 180, -1, 782.601226993865,
                (object senderx, EventArgs ex) => { Model.performOperation("consumeBool", reflectModelStateOnView, freePlay); }));

            theMainSpritesList.Add("undo", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\undoButton.png", 150, 189, 777,
                (object senderx, EventArgs ex) => { Model.undoLastOperation(freePlay, reflectModelStateOnView); }));

            theMainSpritesList.Add("hand", new Sprite(@"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\handpointing.png", 250, 1318, 585, event_ActionPairs: new Dictionary<string, Action<object, EventArgs>>() 
                                        {
                                            {Image.MouseMoveEvent.ToString() ,(object senderx, EventArgs ex) => { Console.WriteLine("MOVE!"); }}
                                        }));

        }

        private void performCalulatorClick()
        {
            /* A function that depends on the state of the calculator and hand - evaluate/read/sub etc */
            //  throw new NotImplementedException();
        }

        private void performTerminalClick()
        {
            /*requires specialist behaviour that is determined by the position of the mouse eg read/write/value/address etc*/
            // throw new NotImplementedException();
        }

        

        /// <summary>
        /// creates a temporary textbox that (when the user raises the enter key) will send the contents of the text box on to the Model, and then dispose of itself
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        private void performOperationThroughTextBox(string operation,int width, double xPos, double yPos)
        {
            TextBox temp = theView.addTextBox(width, xPos, yPos, (object o, KeyEventArgs s) => 
                                                                    {
                                                                        if (s.Key == Key.Return)
                                                                        {
                                                                            string finalContent = ((TextBox)o).Text;
                                                                            //o is the actual textbox...which means we might be able to associate it with something
                                                                            parentWindow.canvas_Main.Children.Remove(((TextBox)o));
                                                                            Model.performOperation(operation + finalContent, reflectModelStateOnView, freePlay);
                                                                           
                                                                        }
                                                                    });
            temp.Background = new SolidColorBrush(Color.FromArgb(10, 10, 10, 10));
            temp.FontSize = 20;
            temp.Focus();
        }

        private void performOperationThroughMenu(string operation, int width, double xPos, double yPos, string[] menuOptions)
        {
                                                                            //operation to perform on click of one menu option
            Menu tempMenu = theView.addMenu(width, xPos, yPos, menuOptions, (object o, RoutedEventArgs s) =>
                                                                                {
                                                                                    string finalContent = ((MenuItem)o).Header.ToString();
                                                                                    //o is the actual textbox...which means we might be able to associate it with something
                                                                                    parentWindow.canvas_Main.Children.Remove((Menu)((MenuItem)o).Parent);
                                                                                    Model.performOperation(operation + finalContent, reflectModelStateOnView, freePlay);
                                                                                });

            tempMenu.Background = new SolidColorBrush(Color.FromArgb(200, 230, 230, 230));

        }

        /// <summary>
        /// To be called whenever an interactive action completes. 
        /// This method refreshed the appearance of everything which depends on the state of the underslying model
        /// </summary>
        public void reflectModelStateOnView()
        {
            //this method is also where anything that doesn't belong to the current levels complexity can be removed...it would be more efficient (from a performance perspective) to simply 
            //not create the unused objects at all, however from an understandability perspective its easier to create everything and then exlude certain objects from populating the canvas.
            //THese exclusion are done at the end of this method by the excludeSpritesFromCanvas() method

            //the notepads
            theMainSpritesList["intpad"].infoLabel.Content = "int:";
            theMainSpritesList["intpad"].infoLabel.FontSize = 30;
            Canvas.SetLeft(theMainSpritesList["intpad"].infoLabel, Canvas.GetLeft(theMainSpritesList["intpad"].imageComponent)+10);
            Canvas.SetTop(theMainSpritesList["intpad"].infoLabel, Canvas.GetTop(theMainSpritesList["intpad"].imageComponent)+10);

            theMainSpritesList["doublepad"].infoLabel.Content = "double:";
            theMainSpritesList["doublepad"].infoLabel.FontSize = 30;
            Canvas.SetLeft(theMainSpritesList["doublepad"].infoLabel, Canvas.GetLeft(theMainSpritesList["doublepad"].imageComponent) + 10);
            Canvas.SetTop(theMainSpritesList["doublepad"].infoLabel, Canvas.GetTop(theMainSpritesList["doublepad"].imageComponent) + 10);

            theMainSpritesList["boolpad"].infoLabel.Content = "bool:";
            theMainSpritesList["boolpad"].infoLabel.FontSize = 30;
            Canvas.SetLeft(theMainSpritesList["boolpad"].infoLabel, Canvas.GetLeft(theMainSpritesList["boolpad"].imageComponent) + 10);
            Canvas.SetTop(theMainSpritesList["boolpad"].infoLabel, Canvas.GetTop(theMainSpritesList["boolpad"].imageComponent) + 10);

            //the stack frames
            createStackFrameSprites(420, 248, 400, 100,Model.getStackSize());

            //the frame and it's variables
            createLocalFrameSprites(420, 430, 100, 88);

            //the method mechanism
            createMethodMechanismSprites(830, 370, 100, 88 );

            //the method name button
            if (Model.isMethodNamed())
            {
                //Model.getTheCurrentLevelsMethods();
                theMainSpritesList["methodbutton"].infoLabel.Content = Model.methodMech.methodName;
                theView.setSpriteImageTo(theMainSpritesList["methodbutton"], @"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\functionNamedAndPressed.png", true);
            }
            else
            {
                theMainSpritesList["methodbutton"].infoLabel.Content = "";
                theView.setSpriteImageTo(theMainSpritesList["methodbutton"], @"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\functionNameButton.png", true);
            }

            //the heap

            //the robot and terminal

            //the feedback sprite

            //the hand
            if (Model.thePlayer.isHandEmpty())
            {
                theMainSpritesList["hand"].infoLabel.Content = "";
                theView.setSpriteImageTo(theMainSpritesList["hand"], @"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\handpointing.png");
                //cursorSprite.setAllSpriteTextures("handpointing");
            }
            else
            {
                theMainSpritesList["hand"].infoLabel.Content = Model.getHandValueAsString();
                theView.setSpriteImageTo(theMainSpritesList["hand"], @"...\...\...\CSMetaphorWorld\CSMetaphorWorldContent\handvalue.png");
                //cursorSprite.setAllSpriteTextures("handvalue");
            }


            if (hintsOn && !freePlay)
                applyHintTransparencies();
            else
                applyNormalTransparancies();


            //changes to the interactive code form:
            textualInterface.changeBasedOnLevelState(Model.currentLevel);
           
            //Exlcuding certain sprites from the canvas becuase they don't fit in with the level code
            excludeSpritesFromCanvas();  
        }

        public string getSpriteKeyForOperation(string operation)
        {
            
            List<Tuple<string, string>> keyOperationPairs = new List<Tuple<string, string>>() { 
                new Tuple<string, string>("addlocal", "declarevariable"),
                new Tuple<string, string>("intpad", "directhandwrite int"),
                new Tuple<string, string>("doublepad", "directhandwrite double"),
                new Tuple<string, string>("boolpad", "directhandwrite bool"),
                 new Tuple<string, string>("booleater", "consumeBool"),
                 new Tuple<string, string>("methodbutton", "preparemethod"),
                 new Tuple<string, string>("callbutton", "callfunction"),
                 new Tuple<string, string>("return", "return"),
                 
            };


            foreach (var tuple in keyOperationPairs)
            {
                if(operation.Contains("readvariable"))
                    return "variablebox" + Model.getVariableNumber(operation.Substring(13));
                if (operation.Contains("assignvalue"))
                    return "variablebox" + Model.getVariableNumber(operation.Substring(12));
                if (operation.Contains("assignparameter"))
                    return "parameterbox" + Model.getParameterNumber(operation.Substring(16));
                if (operation.Contains(tuple.Item2))
                    return tuple.Item1;
            }

            //we shouldn't get here but if we do we should return a string that can't be matched to anything
            return "☺  Ä  ☺";
        }

        private void applyNormalTransparancies()
        {
            bool passed = false;
            int counter = 0;
            foreach (var spritePair in methodMechanismSpriteList)
            {
                //count the number of elements in the current neverFaded list that are contained inside the current sprites key value
                //if (spritePair.Key == getSpriteKeyForOperation(Model.currentLevel.getNextOperationCode()) ||
               //     getSpriteKeyForOperation(Model.currentLevel.getNextOperationCode()) == "callbutton" || passed)
               // {
                if (spritePair.Key.Contains("parameterbox") || spritePair.Key.Contains("callbutton"))
                {

                    if (spritePair.Key.Contains("callbutton"))
                    {
                        if(Model.methodMech.nextParameterToAssignTo() == "")
                            spritePair.Value.imageComponent.Opacity = 1;
                        else
                            spritePair.Value.imageComponent.Opacity = 0.6;
                        break;
                    }
                    

                    if (Model.methodMech.nextParameterToAssignTo() == Model.methodMech.getParameterXName(counter))
                        spritePair.Value.imageComponent.Opacity = 1;
                    else
                        spritePair.Value.imageComponent.Opacity = 0.6;

                    counter++;
                }                
            }

            if (!Model.thePlayer.isHandEmpty() && Model.thePlayer.examineHeld().readType().ToLower() == "bool")
                theMainSpritesList["booleater"].imageComponent.Opacity = 1;
            else
                theMainSpritesList["booleater"].imageComponent.Opacity = 0;
        }

        private void applyHintTransparencies()
        {
            List<string> neverFaded = new List<string>() { "conveyor", "hand", "variablepanel", "final", "barrier", "table", "feedback", "methodpanel" };

            foreach (var collection in new Dictionary<string, Sprite>[] { theMainSpritesList, methodMechanismSpriteList, frameSprites })
                foreach (var spritePair in collection)
                {
                    //count the number of elements in the current neverFaded list that are contained inside the current sprites key value
                    if (neverFaded.Count<string>((nfElem) => { return spritePair.Key.Contains(nfElem); }) == 0 &&
                       !spritePair.Key.Contains(getSpriteKeyForOperation(Model.currentLevel.getNextOperationCode())))
                        spritePair.Value.imageComponent.Opacity = 0.6;
                    else
                        spritePair.Value.imageComponent.Opacity = 1;

                    if(spritePair.Key == "booleater")
                        if (!Model.thePlayer.isHandEmpty() && Model.thePlayer.examineHeld().readType().ToLower() == "bool")
                            spritePair.Value.imageComponent.Opacity = 1;
                        else
                            spritePair.Value.imageComponent.Opacity = 0;
                }
        }

        private void excludeSpecificSpriteFromCanvas(Dictionary<string,Sprite> collection, string spriteKey, string actualMode, params string[] inclusionModes)
        {
            if (actualMode == "all") return;

            foreach (var im in inclusionModes)
            {
                if(actualMode.Contains(im))
                    return;
            }

            //getting here means that the actual mode did not contain any of the assorted inclusion modes and so we can remove the mathcing element from the canvas
            parentWindow.canvas_Main.Children.Remove(collection[spriteKey].imageComponent);
            parentWindow.canvas_Main.Children.Remove(collection[spriteKey].infoLabel);
        }

        private void excludeSpritesFromCanvas()
        {
            string mode = Model.currentLevel.componentEnabledMode;

            //all these things are requied every time
            //List<string> enabledKeys = new List<string>() { "helpbackground", "mouse", "feedback", "help", "intro", "border", "memorybarrier", "conveyor" };

            excludeSpecificSpriteFromCanvas(theMainSpritesList,"calculator", mode);

            excludeSpecificSpriteFromCanvas(theMainSpritesList, "intpad", mode,"int");
            excludeSpecificSpriteFromCanvas(theMainSpritesList, "doublepad", mode, "double");

            excludeSpecificSpriteFromCanvas(theMainSpritesList, "boolpad", mode, "bool");
            excludeSpecificSpriteFromCanvas(theMainSpritesList, "booleater", mode, "bool");

            excludeSpecificSpriteFromCanvas(theMainSpritesList, "return", mode, "function", "method", "procedure");
            excludeSpecificSpriteFromCanvas(theMainSpritesList, "table", mode, "function", "method", "procedure");
            excludeSpecificSpriteFromCanvas(theMainSpritesList, "methodbutton", mode, "function", "method", "procedure");

            excludeSpecificSpriteFromCanvas(theMainSpritesList, "robot", mode, "heap", "object", "reference");
            excludeSpecificSpriteFromCanvas(theMainSpritesList, "terminal", mode, "heap", "object", "reference");
            excludeSpecificSpriteFromCanvas(theMainSpritesList, "bin", mode, "heap", "object", "reference");

            //this is for the pencil that we no longer include 
            //if (mode.Contains("value") || mode.Contains("other") || mode.Contains("pencil") || mode == "all" ||
            //    (mode.Contains("minimal") && !legalMode))//check if we have 'minimal' mode going, and whether we have added any pencil sprites yet...if not then we need to add the generic pencil even if it isn't mentioned
            //{
            //    enabledKeys.Add("pencil");
            //    legalMode = true;
            //}
        }

        public Control(MainWindow parentWindow)
        {
            initaliseMainSprites();
            this.parentWindow = parentWindow;
            theView = new View(parentWindow.canvas_Main);
            LoadNewLevel("empty.xml", ref this.Model);

            textualInterface = new LevelCodeInterface();
            textualInterface.Show();
            textualInterface.openLevelToolStripMenuItem.Click += new EventHandler(
                (object sender, EventArgs e) => { clearCanvas(); LoadNewLevel(textualInterface.levelToOpen, ref Model); reflectModelStateOnView(); });

            parentWindow.LocationChanged += new EventHandler(
                (object sender, EventArgs e) => { textualInterface.changePosition((int)parentWindow.Left - textualInterface.Width, (int)parentWindow.Top); });

            reflectModelStateOnView();
        }

        private void clearCanvas()
        {
            while (parentWindow.canvas_Main.Children.Count > 0)
                parentWindow.canvas_Main.Children.RemoveAt(0);
        }

        public void InitialiseVisualComponents()
        {
            Button b = new Button();
            b.Width = 1;
            b.Height = 1;
            b.Content = "WOOT!";
            //ImageSource x;
            
            //b.Foreground = new System.Windows.Media.ImageBrush(x);
            //@"F:\Dropbox\registry change.png");
            
            parentWindow.canvas_Main.Children.Add(b);
            
            Canvas.SetLeft(b, 100);
            Canvas.SetTop(b, 100);

            foreach (KeyValuePair<string,Sprite> pair in theMainSpritesList)
            {
                try
                {
                    pair.Value.imageComponent = theView.addInteractiveImageToCanvas(pair.Value.imageAddress, pair.Value.width, pair.Value.xCoord, pair.Value.yCoord, pair.Value.mouseLeftUpAction);
                    pair.Value.infoLabel = theView.addLabelToImage(pair.Value.imageComponent, "", 0, 0);
                    
                    foreach (var eventActionPair in pair.Value.event_ActionPairs)
                    {
                        addActionToEvent(pair.Value.imageComponent, eventActionPair.Key, eventActionPair.Value);
                    }

                    pair.Value.imageComponent.MouseMove += shuffle;
                    pair.Value.imageComponent.ToolTip = pair.Key;
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                }
            }          
        }

        //private void addActionToEvent(Image imageCompontent, string eventName, Action<object, EventArgs> eventAction)
        private void addActionToEvent(Image imageCompontent, string eventName, Action<object, EventArgs> eventAction)
        {
            Console.WriteLine(eventName);
            switch (eventName.ToLower())
            {
                case("keyboard.keydown"):
                case("keydown"):
                    imageCompontent.KeyDown += new KeyEventHandler(eventAction);
                    break;
                case("mouse.mouseenter"):
                case ("mouseenter"):
                    imageCompontent.MouseEnter += new MouseEventHandler(eventAction);
                    break;
                case("uielement.mouserightbuttonup"):
                case("mouserightbuttonip"):
                    imageCompontent.MouseRightButtonUp += new MouseButtonEventHandler(eventAction);
                    break;
                case("tooltipservice.tooltipopening"):
                case("tooltipopening"):
                    imageCompontent.ToolTipOpening += new ToolTipEventHandler(eventAction);
                    break;
                case("mouse.mousemove"):
                case ("mousemove"):
                    imageCompontent.MouseMove += new MouseEventHandler(eventAction);
                    break;
                default:
                    throw new ArgumentException(eventName + " is not a recognised eventName");
            }
        }

        private void shuffle(object o, MouseEventArgs s) 
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Canvas.SetLeft((Image)o, Mouse.GetPosition(parentWindow.canvas_Main).X - ((Image)o).Width / 2);
                Canvas.SetTop((Image)o, Mouse.GetPosition(parentWindow.canvas_Main).Y - ((Image)o).Height / 2);
                foreach (KeyValuePair<string, Sprite> pair in theMainSpritesList)
                {
                    if ((Image)o == pair.Value.imageComponent)
                    {
                        pair.Value.xCoord = Canvas.GetLeft((Image)o);
                        pair.Value.yCoord = Canvas.GetTop((Image)o);

                        theView.maintainRelativeLabelPosition((Image)o, pair.Value.infoLabel);

                        parentWindow.tb_SpriteName.Text = pair.Key;
                        parentWindow.tb_FileName.Text = pair.Value.imageAddress;
                        parentWindow.tb_width.Text = pair.Value.width.ToString();
                        parentWindow.tb_XPos.Text = pair.Value.xCoord.ToString();
                        parentWindow.tb_YPos.Text = pair.Value.yCoord.ToString();

                        break;
                    }
                }

                
            }
            if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                parentWindow.displaySpriteDetailsToConsole();
            }
        }

        /// <summary>
        /// Re-Creates the Model variable with data taken from the given file.
        /// Also Re-Initialises all the View components.
        /// </summary>
        /// <param name="levelName"></param>
        /// <param name="Model"></param>
        public void LoadNewLevel(string levelName, ref WorldTracker Model)
        {
            //is the levelName parameter an absolute address rather than just a level name?
            if (!levelName.Contains(":\\"))
            {
                string baseDirectory = "...\\...\\...\\CSMetaphorWorld\\CSMetaphorWorld\\levels\\";
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
                    Model = new WorldTracker(baseDirectory + "empty.xml");
                    Model.setTheCurrentLevelsMethods(new string[] {"noParams()", "isPrime(int num)", "getArea(int width,int height)", "nthTriangleNum(int n)", "hasPattern(double a, double b, double c, double d, double e)", "convertToBinary(int num)" });
                }
                else
                {
                    freePlay = false;
                    Model = new WorldTracker(currentLevelDirectory);
                }
            }
            else
            {
                if (!levelName.Contains(".xml"))
                    levelName += ".xml";
                
                //check if the level exists
                if (File.Exists(levelName))
                {
                    currentLevelDirectory = levelName;
                    Model = new WorldTracker(currentLevelDirectory);
                    if (levelName.Contains("empty.xml") || levelName.Contains("sandbox.xml"))
                    {
                        freePlay = true;
                        Model.setTheCurrentLevelsMethods(new string[] { "isPrime(int num)", "getArea(int width,int height)", "nthTriangleNum(int n)", "hasPattern(double a, double b, double c, double d, double e)", "convertToBinary(int num)" });
                    }
                    else
                        freePlay = false;
                }
                else
                    throw new FileLoadException("You tried to read from a file \"{0}\" that does not exist", levelName);
            }

            //set it up so that nothing special happens when we complete an operation
            Model.setOperationCompletionActions(() => { }, () => { });

            //We need to reset the state of all the 'sprites'
            InitialiseVisualComponents();

            //now that all the sprites have associated images we can make changes to their properties (changes that only need to be done once)
            theMainSpritesList["hand"].imageComponent.IsHitTestVisible = false;
            //the following block ensures that the hand always follows the cursor
            Random myRandom = new Random();
            parentWindow.canvas_Main.MouseMove += new MouseEventHandler(
                                                    (object o, MouseEventArgs s) => 
                                                            {
                                                                if (parentWindow.canvas_Main.Children[parentWindow.canvas_Main.Children.Count - 2] != theMainSpritesList["hand"].imageComponent)
                                                                {
                                                                    parentWindow.canvas_Main.Children.Remove(theMainSpritesList["hand"].imageComponent);
                                                                    parentWindow.canvas_Main.Children.Add(theMainSpritesList["hand"].imageComponent);
                                                                    parentWindow.canvas_Main.Children.Remove(theMainSpritesList["hand"].infoLabel);
                                                                    parentWindow.canvas_Main.Children.Add(theMainSpritesList["hand"].infoLabel);
                                                                }

                                                                if (Mouse.GetPosition(parentWindow.canvas_Main).Y > theMainSpritesList["barrier"].yCoord + 50)
                                                                    theMainSpritesList["hand"].yCoord = Mouse.GetPosition(parentWindow.canvas_Main).Y;


                                                                theMainSpritesList["hand"].xCoord = Mouse.GetPosition(parentWindow.canvas_Main).X-48;
                                                                Canvas.SetLeft(theMainSpritesList["hand"].imageComponent,theMainSpritesList["hand"].xCoord);
                                                                Canvas.SetTop(theMainSpritesList["hand"].imageComponent,theMainSpritesList["hand"].yCoord);
                                                                theMainSpritesList["hand"].infoLabel.FontSize = 30;
                                                                Canvas.SetLeft(theMainSpritesList["hand"].infoLabel, theMainSpritesList["hand"].xCoord + 50);
                                                                Canvas.SetTop(theMainSpritesList["hand"].infoLabel, theMainSpritesList["hand"].yCoord + 105);

                                                            });
        }
    }
}
