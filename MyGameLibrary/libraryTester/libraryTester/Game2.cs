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
using MyGameLibrary; 

namespace libraryTester
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game2 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        //my added globals
        Wizard mWizardSprite;

        Circle triangle;
        Circle triangle2;
        Circle triangle3;
        Circle triangle4;
        Circle triangle5;
        Circle triangle6;

        Circle[,] thePieces;

        KeyboardState previousState;

        Sprite pointer;
        const int WIDTH = 40; //ratio x:2x:1/x    width:maxworldSpace:scale (for drawing circles in a grid for game of life)
        const int HEIGHT = 40;

        CA_Crowd theCrowd;

        BasicEffect basicEffect;

       

        public Game2()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferHeight = 1000;
            graphics.PreferredBackBufferWidth = 1800;
            Content.RootDirectory = "Content";
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
            mWizardSprite = new Wizard();

            short[,] theStartBoard = new short[WIDTH, HEIGHT];
            

            basicEffect = new BasicEffect(GraphicsDevice);

            triangle = new Circle();
            triangle2 = new Circle();
            triangle3 = new Circle();
            triangle4 = new Circle();
            triangle5 = new Circle();
            triangle6 = new Circle();
            previousState = Keyboard.GetState();

            theCrowd = new CA_Crowd();
            /*single row test
            theCrowd.cellsOfTheCrowd.Add(new CA_Cell(0, 0, 39, 0, 40, 1));
            theCrowd.cellsOfTheCrowd.Add(new CA_Cell(1, 0, 39, 0, 40, 1));
            theCrowd.cellsOfTheCrowd.Add(new CA_Cell(2, 0, 39, 0, 40, 1));*/


           /*head on 2 row colission test
            theCrowd.cellsOfTheCrowd.Add(new CA_Cell(1, 0, 39, 0, 40, 2));
            theCrowd.cellsOfTheCrowd.Add(new CA_Cell(1, 1, 39, 1, 40, 2));
            theCrowd.cellsOfTheCrowd.Add(new CA_Cell(2, 0, 39, 0, 40, 2));
            theCrowd.cellsOfTheCrowd.Add(new CA_Cell(2, 1, 39, 1, 40, 2));
            theCrowd.cellsOfTheCrowd.Add(new CA_Cell(3, 0, 39, 0, 40, 2));
            theCrowd.cellsOfTheCrowd.Add(new CA_Cell(3, 1, 39, 1, 40, 2));

            theCrowd.cellsOfTheCrowd.Add(new CA_Cell(36, 0, 0, 0, 40, 2));
            theCrowd.cellsOfTheCrowd.Add(new CA_Cell(36, 1, 0, 1, 40, 2));
            theCrowd.cellsOfTheCrowd.Add(new CA_Cell(37, 0, 0, 0, 40, 2));
            theCrowd.cellsOfTheCrowd.Add(new CA_Cell(37, 1, 0, 1, 40, 2));
            theCrowd.cellsOfTheCrowd.Add(new CA_Cell(38, 0, 0, 0, 40, 2));
            theCrowd.cellsOfTheCrowd.Add(new CA_Cell(38, 1, 0, 1, 40, 2));*/


            theCrowd = new CA_Crowd(300, WIDTH, HEIGHT, 0.3f);

            thePieces = new Circle[WIDTH, HEIGHT];

            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                {
                    thePieces[x, y] = new Circle();
                }

            pointer = new Sprite();
            pointer.Scale = 0.1f;

          

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //my addition:
            mWizardSprite.LoadContent(this.Content);

            
            triangle.LoadContent(this.Content,  new Vector3(-2f, 1, 0f), 0.5f, "solidcolour", Color.Red);
            triangle2.LoadContent(this.Content, new Vector3(0, 1, 0f), 0.5f, "randomBorder", Color.Red);
            triangle3.LoadContent(this.Content, new Vector3(2f, 1, 0f), 0.5f, "randomcentresingleborder", Color.Red);
            triangle4.LoadContent(this.Content, new Vector3(-2f, -1, 0f), 0.5f, "singleBorder", Color.Red, Color.Blue);
            triangle5.LoadContent(this.Content, new Vector3(0, -1, 0f), 0.5f, "randomcentredoubleborder", Color.Red,Color.Black);
            triangle6.LoadContent(this.Content, new Vector3(2f, -1, 0f), 0.5f, "doubleborder", Color.Red, Color.Blue, Color.Black);

            Vector3 myCoords;
            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                {

                    myCoords = vertexAndMatricManipulation.gridToWorldConverter(x, y, WIDTH, HEIGHT, WIDTH*2);
                    thePieces[x, y].LoadContent(this.Content, myCoords, 1f / WIDTH, "singleborder", Color.Red, Color.FromNonPremultiplied(0,0,255,0));
                    //thePieces[x, y].LoadContent(this.Content, new Vector3((x - 2) * 2, (y - 2) * 2, 0), 0.2f, "singleborder", Color.Red, Color.Blue);
                }

          
            pointer.LoadContent(this.Content, "simpleSquare");

            // Create new basic effect and properites
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.World = Matrix.Identity;
            basicEffect.View = Matrix.CreateLookAt(new Vector3(0, 0, 3),
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0));
            basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                GraphicsDevice.Viewport.AspectRatio, 0.1f, 100.0f);
            basicEffect.VertexColorEnabled = true;
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

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();


            if (Keyboard.GetState().IsKeyDown(Keys.N) && previousState.IsKeyUp(Keys.N))
            {
                Console.WriteLine("next gen?");
                theCrowd.moveThoseWithNoColissionsAndNoSharedDestination();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.RightAlt) && Keyboard.GetState().IsKeyDown(Keys.Enter) && previousState.IsKeyUp(Keys.Enter))
            {
                graphics.ToggleFullScreen();
            }

            previousState = Keyboard.GetState();

            if(Mouse.GetState().LeftButton == ButtonState.Pressed)
                Console.WriteLine("X:" + (((float)(Mouse.GetState().X - 400)) / 200) + "  Y:" + ((float)(Mouse.GetState().Y - 235) / -200));
            pointer.Position.X = Mouse.GetState().X;
            pointer.Position.Y = Mouse.GetState().Y;



           

            Vector2 aDirection = new Vector2(-1, 0);
            Vector2 aSpeed = new Vector2(160, 0);


            mWizardSprite.Update(gameTime);


            triangle.Update(gameTime);
            triangle2.Update(gameTime);
            triangle3.Update(gameTime);
            triangle4.Update(gameTime);
            triangle5.Update(gameTime);
            triangle6.Update(gameTime);

            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                {
                    thePieces[x, y].userPrimitives[0].Color = Color.Red;
                }

            int counter = 0;
            foreach (CA_Cell cell in theCrowd.cellsOfTheCrowd)
            {
                thePieces[cell.xCoord_Current, cell.yCoord_Current].userPrimitives[0].Color = Color.FromNonPremultiplied((counter * counter) % 255, (counter + counter) % 255, (counter * 4) % 255, 255);
                thePieces[cell.xCoord_Current, cell.yCoord_Current].userPrimitives[(counter * 2) % 100].Color = Color.FromNonPremultiplied((counter * counter) % 255, (counter + counter)%255, (counter * 4)%255, 255);
                counter += 10;
            }

            foreach (CA_Cell cell in theCrowd.cellsOfTheCrowd)
            {
                thePieces[cell.xCoord_finalDestination, cell.yCoord_finalDestination].userPrimitives[0].Color = Color.Black;
            }
                    
            if (triangle4.userPrimitives[0].Color.R == 255)
                triangle4.userPrimitives[0].Color.R = 0;
            else
                triangle4.userPrimitives[0].Color.R++;



            base.Update(gameTime);
            return;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            //my additions:

           /* triangle.Draw(GraphicsDevice, basicEffect);
            triangle2.Draw(GraphicsDevice, basicEffect);
            triangle3.Draw(GraphicsDevice, basicEffect);
            triangle4.Draw(GraphicsDevice, basicEffect);
            triangle5.Draw(GraphicsDevice, basicEffect);
            triangle6.Draw(GraphicsDevice, basicEffect);*/

            spriteBatch.Begin();
            
            mWizardSprite.Draw(this.spriteBatch);
            pointer.Draw(this.spriteBatch);

            spriteBatch.End();

            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                {
                    thePieces[x, y].Draw(GraphicsDevice, basicEffect);
                }

           

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
