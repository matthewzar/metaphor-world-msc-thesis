using System;
using System.Collections.Generic;
// using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MyGameLibrary; 

namespace libraryTester
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;

        Effect effect;

        BasicEffect basicEffect;

        VertexPositionColor[] vertices;

        Sprite myGrid;

        spDirectDraw gridDrawer;

        SquaresGrid myT;


        List<List<int>> heapsData; 

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            populatePseudoHeap();
        }

        const int globalwidth = 40;
        const int globalheight = 40;

        private void populatePseudoHeap()
        {

            //get heap as list of lists
            heapsData = new List<List<int>>();

            //populate pseudo heap with 'random' lists
            Random myRan = new Random();
            int totalLength = 0;
            int currHeapItem = 0;
            while (totalLength < globalwidth * globalheight)
            {

                heapsData.Add(new List<int>());

                int currLen = myRan.Next(1, globalwidth * globalheight - totalLength + 1);
                if (currLen > globalwidth)
                    currLen = myRan.Next(1, globalwidth);

                totalLength += currLen;
                for (int i = 0; i < currLen; i++)
                    heapsData[currHeapItem].Add(myRan.Next(100));
                currHeapItem++;
            }
        }
        
        //should consider making this into a function that can be passed into the spDirectDraw update class: it would take a colour array (defined inside the spDirectDraw class) and return a string, 
        //the function would alter the internal colour array, and return a string that would then be used in update to display the selected cells contents...
        //downside to this: the update needs to take an object that can represent the heap
        private Tuple<Color[,],string> convertHeapToColourArray()
        {
            int ii = 0;
            int alphaVal = 255;

            string displayContent = "";

            Color[,] returnColours = new Color[globalwidth, globalheight];
        

           // go through each object stored on the heap (an object is made up of a list of values)
            foreach (List<int> itemAsList in heapsData)
            {
                alphaVal = 155; //reset the transparancy value... to the default of "no this list is not highlighted" 
                
                //determine if one of the cells in the current object is being hovered over, so we know later to chnage it's alpha value
                for(int localIndex = 0; localIndex < itemAsList.Count; localIndex++)
                    if (gridDrawer.isCellHoveredOver(Mouse.GetState(), (ii + localIndex) % globalwidth, (ii + localIndex) / globalwidth))
                    {
                        //getting here we know to highlight the whole section in the next iteration
                        alphaVal = 255;
                        break;
                    }

            

                //now we now know whether the whole group needs highlighting or not (and have already set the alpha value to reflect what must be done), now go and change colours
                foreach (int singleCell in itemAsList)
                {
                 
                    if (alphaVal == 255)
                    {
                        displayContent += singleCell + " ";
                    }

                    //TODO: replace this if else with a switch to allow for additional 
                    if (itemAsList[0] % 4 == 0)
                        returnColours[ii % globalwidth, ii / globalwidth] = new Color(100, 100, 100, alphaVal);
                    else if (itemAsList[0] % 4 == 1)
                        returnColours[ii % globalwidth, ii / globalwidth] = new Color(150, 150, 10, alphaVal);
                    else if (itemAsList[0] % 4 == 2)
                        returnColours[ii % globalwidth, ii / globalwidth] = new Color(10, 150, 150, alphaVal);
                    else if (itemAsList[0] % 4 == 3)
                        returnColours[ii % globalwidth, ii / globalwidth] = new Color(200, 50, 100, alphaVal);
                    else  
                        Console.WriteLine("colour failure!"); 

                    ii++;//we can use this to decide what x and y values belong where
                }
            }
            return new Tuple<Color[,],string>(returnColours,displayContent);
        }

        protected override void Initialize()
        {
            
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "XNA Tutorial";
            basicEffect = new BasicEffect(GraphicsDevice);

            myT = new SquaresGrid(6, 6, -1, 0, 1, 1.0f);
            myGrid = new Sprite();
            gridDrawer = new spDirectDraw(new Vector2(300, 300), 30, 40);
            IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            device = graphics.GraphicsDevice;

            //effect = Content.Load<Effect>("effects");

            

            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.World = Matrix.Identity;
            basicEffect.View = Matrix.CreateLookAt(new Vector3(0, 0, 3),
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0));
            basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                GraphicsDevice.Viewport.AspectRatio, 0.1f, 100.0f);
            basicEffect.VertexColorEnabled = true;
            
            myT.LoadContent(Content);
            colouredGrid x = new colouredGrid(device,100,100);
            myGrid.LoadContent(Content, x.image);

            gridDrawer.LoadContent(Content, "miniBox", "miniBox","myFont");
        }

        protected override void UnloadContent()
        {
        }

       

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            //if (Keyboard.GetState().IsKeyDown(Keys.C))

           
            myGrid.Update(gameTime, Vector2.Zero, Vector2.Zero);

            //TODO surround this update with something to check if the mouse in roughly the right place to warrant an update (an if mouse in position sort of thing):
            gridDrawer.Update(gameTime, convertHeapToColourArray());
            base.Update(gameTime);
        }

        
        protected override void Draw(GameTime gameTime)
        {
            device.Clear(Color.DarkSlateBlue);

          //  basicEffect.CurrentTechnique.Passes[0].Apply();
          //  myT.Draw(device, basicEffect);



           

            spriteBatch.Begin();
          //  myGrid.Draw(this.spriteBatch);

            gridDrawer.DrawGrid(spriteBatch);


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
