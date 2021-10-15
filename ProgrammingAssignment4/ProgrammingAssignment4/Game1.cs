using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProgrammingAssignment4
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int WindowWidth = 800;
        const int WindowHeight = 600;

        // teddy support
        Texture2D teddySprite;
        TeddyBear teddy;

        // pickup support
        Texture2D pickupSprite;
        List<Pickup> pickups = new List<Pickup>();

        // click processing
        bool rightClickStarted = false;
        bool rightButtonReleased = true;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // STUDENTS: set resolution and make mouse visible

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
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // STUDENTS: load teddy and pickup sprites
            teddySprite = Content.Load<Texture2D>(@"graphics\teddybear");
            pickupSprite = Content.Load<Texture2D>(@"graphics\pickup");

            // STUDENTS: create teddy object centered in window
            teddy = new TeddyBear(teddySprite, new Vector2(WindowWidth / 2, WindowHeight / 2));
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

            // STUDENTS: get current mouse state and update teddy
            MouseState mouse = Mouse.GetState(); 

            // check for right click started
            if (mouse.RightButton == ButtonState.Pressed &&
                rightButtonReleased)
            {
                rightClickStarted = true;
                rightButtonReleased = false;
            }
            else if (mouse.RightButton == ButtonState.Released)
            {
                rightButtonReleased = true;

                // if right click finished, add new pickup to list
                if (rightClickStarted)
                {
                    rightClickStarted = false;

                    // STUDENTS: add a new pickup to the end of the list of pickups
                    Pickup pickup = new Pickup(pickupSprite, new Vector2(mouse.X, mouse.Y));
                    pickups.Add(pickup);

                    // STUDENTS: if this is the first pickup in the list, set teddy target
                    if (pickups.Count == 1)
                        teddy.SetTarget(new Vector2(pickup.CollisionRectangle.X, pickup.CollisionRectangle.Y));
                }
            }

            teddy.Update(gameTime, mouse);

            // check for collision between collecting teddy and targeted pickup
            if (teddy.Collecting &&
                teddy.CollisionRectangle.Intersects(pickups[0].CollisionRectangle))
            {
                // STUDENTS: remove targeted pickup from list (it's always at location 0)
                pickups.RemoveAt(0);

                // STUDENTS: if there's another pickup to collect, set teddy target
                // If not, clear teddy target and stop the teddy from collecting
                if(pickups.Any())
                {
                    teddy.SetTarget(new Vector2(pickups[0].CollisionRectangle.Center.X, pickups[0].CollisionRectangle.Center.Y));
                }
                else
                {
                    teddy.ClearTarget();                    
                }
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

            // draw game objects
            spriteBatch.Begin();
            teddy.Draw(spriteBatch);
            foreach (Pickup pickup in pickups)
            {
                pickup.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

