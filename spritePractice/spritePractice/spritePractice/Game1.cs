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

namespace spritePractice
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Vector2 speed;
        Vector2 direction;

        const int height = 900;
        const int width = 1200;

        simpleSprite aSprite;
        clickableSprite clickerSprite;
        multiRegionClickableSprite multiSprite;

        List<multiRegionClickableSprite> interactiveSprites;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = height;
            graphics.PreferredBackBufferWidth = width;
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Action anonAction = delegate() 
            {
                Console.WriteLine("Anonymous Action"); 
            };

            // TODO: Add your initialization logic here
            aSprite = new simpleSprite();
            speed = new Vector2(10, 10);
            direction = new Vector2(1, 1);

            clickerSprite = new clickableSprite();

            multiSprite = new multiRegionClickableSprite(300, 300,1.0f, "angryFace", "pngImage",
                new List<Tuple<int,int,int,int>>(){new Tuple<int,int,int,int>(0,0,50,50)},
                new List<Action>() { anonAction });

            interactiveSprites = new List<multiRegionClickableSprite>()
            {
                new multiRegionClickableSprite(20,0,0.5f,"ConveyerBelt(WithPerspective)","ConveyerBelt(WithPerspective)", new List<Tuple<int,int,int,int>>(){}, new List<Action>(){}),
                new multiRegionClickableSprite(-180,260,0.7f,"barbedWireBarrier","barbedWireBarrier", new List<Tuple<int,int,int,int>>(){}, new List<Action>(){}),
               // new multiRegionClickableSprite(200,200,0.025f,"variableBoxWithShadow","variableBoxWithShadow", new List<Tuple<int,int,int,int>>(){}, new List<Action>(){}),
                new multiRegionClickableSprite(380,670,0.3f,"Terminal","Terminal", new List<Tuple<int,int,int,int>>(){}, new List<Action>(){}),
                new multiRegionClickableSprite(550,670,0.2f,"Robot","Robot", new List<Tuple<int,int,int,int>>(){}, new List<Action>(){}),
                new multiRegionClickableSprite(200,670,0.075f,"Calculator","Calculator", new List<Tuple<int,int,int,int>>(){}, new List<Action>(){}),
                new multiRegionClickableSprite(390,420,0.4f,"RecycleBin","RecycleBin", new List<Tuple<int,int,int,int>>(){}, new List<Action>(){}),
                new multiRegionClickableSprite(750,20,0.38f,"heapBookShelf","heapBookShelf", new List<Tuple<int,int,int,int>>(){}, new List<Action>(){}),
                new multiRegionClickableSprite(770,420,0.4f,"heapBookShelf","heapBookShelf", new List<Tuple<int,int,int,int>>(){}, new List<Action>(){}),
                new multiRegionClickableSprite(0,770,0.15f,"HandHoldingValue","HandHoldingValue", new List<Tuple<int,int,int,int>>(){}, new List<Action>(){})
            };
            
            /*
             interactiveSprites = new List<multiRegionClickableSprite>()
            {
                new multiRegionClickableSprite(20,0,0.5f,"ConveyerBelt(WithPerspective)","ConveyerBelt(WithPerspective)", new List<Tuple<int,int,int,int>>(){}, new List<Action>(){}),
                new multiRegionClickableSprite(0,260,0.7f,"barbedWireBarrier","barbedWireBarrier", new List<Tuple<int,int,int,int>>(){}, new List<Action>(){}),
               // new multiRegionClickableSprite(200,200,0.025f,"variableBoxWithShadow","variableBoxWithShadow", new List<Tuple<int,int,int,int>>(){}, new List<Action>(){}),
                new multiRegionClickableSprite(530,600,0.4f,"Terminal","Terminal", new List<Tuple<int,int,int,int>>(){}, new List<Action>(){}),
                new multiRegionClickableSprite(750,600,0.25f,"Robot","Robot", new List<Tuple<int,int,int,int>>(){}, new List<Action>(){}),
                new multiRegionClickableSprite(300,600,0.1f,"Calculator","Calculator", new List<Tuple<int,int,int,int>>(){}, new List<Action>(){}),
                new multiRegionClickableSprite(0,728,0.2f,"HandHoldingValue","HandHoldingValue", new List<Tuple<int,int,int,int>>(){}, new List<Action>(){})
            };
             */

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
            aSprite.LoadContent(this.Content, "pngImage");
            clickerSprite.LoadContent(this.Content);
            multiSprite.LoadContent(this.Content);

            foreach (multiRegionClickableSprite sprt in interactiveSprites)
            {
                sprt.LoadContent(this.Content);
            }
            
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            aSprite.Update(gameTime, speed, direction);
            clickerSprite.Update(gameTime);
            multiSprite.Update(gameTime);

            foreach (multiRegionClickableSprite sprt in interactiveSprites)
            {
                sprt.Update(gameTime);
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            aSprite.Draw(spriteBatch);
            clickerSprite.Draw(spriteBatch);
            multiSprite.Draw(spriteBatch);

            foreach (multiRegionClickableSprite sprt in interactiveSprites)
            {
                sprt.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
