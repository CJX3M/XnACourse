using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ProjectAssigment5
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Mines collection
        List<TeddyMineExplosion.Mine> mines = new List<TeddyMineExplosion.Mine>();
        List<TeddyMineExplosion.TeddyBear> bears = new List<TeddyMineExplosion.TeddyBear>();
        List<TeddyMineExplosion.Explosion> explosions = new List<TeddyMineExplosion.Explosion>();

        // Game sprites;
        Texture2D mineSprite;
        Texture2D teddySprite;
        Texture2D explosionSprite;
        // Button previsouState
        bool previousStatePressed;

        // milisecond from last teddy;
        int millisecondsFromLastTeddy = 0;

        // Randomizer
        Random rand = new Random(); 

        // Window constants
        private const int WINDOW_WIDTH = 800;
        private const int WINDOW_HEIGHT = 600;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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

            base.Initialize();
            IsMouseVisible = true;

            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            mineSprite = Content.Load<Texture2D>(@"graphics\mine");
            teddySprite = Content.Load<Texture2D>(@"graphics\teddybear");
            explosionSprite = Content.Load<Texture2D>(@"graphics\explosion");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            MouseState mouse = Mouse.GetState();

            if(previousStatePressed && mouse.LeftButton == ButtonState.Released)
            {
                TeddyMineExplosion.Mine mine = new TeddyMineExplosion.Mine(mineSprite, mouse.X, mouse.Y);
                mines.Add(mine);
            }
            previousStatePressed = mouse.LeftButton == ButtonState.Pressed;

            millisecondsFromLastTeddy += gameTime.ElapsedGameTime.Milliseconds;
            if(millisecondsFromLastTeddy > rand.Next(1000, 3000))
            {
                millisecondsFromLastTeddy = 0;
                TeddyMineExplosion.TeddyBear newBear = new TeddyMineExplosion.TeddyBear(teddySprite, new Vector2(RandFloat(), RandFloat()), 
                    graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight - teddySprite.Height);
                bears.Add(newBear);
            }
            foreach (TeddyMineExplosion.TeddyBear bear in bears)
            {
                bear.Update(gameTime);
                foreach (TeddyMineExplosion.Mine mine in mines)
                {
                    if(bear.CollisionRectangle.Intersects(mine.CollisionRectangle))
                    {
                        bear.Active = false;
                        mine.Active = false;
                        TeddyMineExplosion.Explosion explosion = new TeddyMineExplosion.Explosion(explosionSprite, 
                            mine.CollisionRectangle.Center.X, mine.CollisionRectangle.Center.Y);
                        explosions.Add(explosion);
                    }
                }
            }
            foreach(TeddyMineExplosion.Explosion explosion in explosions)
            {
                explosion.Update(gameTime);
            }
            for(int i = bears.Count - 1; i >= 0; i--)
            {
                if (!bears[i].Active)
                    bears.RemoveAt(i);
            }
            for (int i = mines.Count - 1; i >= 0; i--)
            {
                if (!mines[i].Active)
                    mines.RemoveAt(i);
            }
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                if (!explosions[i].Playing)
                    explosions.RemoveAt(i);
            }
            base.Update(gameTime);
        }

        private float RandFloat()
        {
            double val = rand.NextDouble();
            val -= 0.5;
            return (float)val;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            foreach(TeddyMineExplosion.Mine mine in mines)
            {
                mine.Draw(spriteBatch);
            }
            foreach(TeddyMineExplosion.TeddyBear bear in bears)
            {
                bear.Draw(spriteBatch);
            }
            foreach (TeddyMineExplosion.Explosion explosion in explosions)
            {
                explosion.Draw(spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
