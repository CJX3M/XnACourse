using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProgrammingAssignment2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        const int WindowWidth = 800;
        const int WindowHeight = 600;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // STUDENTS: declare variables for three sprites
        Texture2D nut1, nut2, nut3, current;

        // STUDENTS: declare variables for x and y speeds
        int speedX, speedY;

        // used to handle generating random values
        Random rand = new Random();
        const int ChangeDelayTime = 1000;
        int elapsedTime = 0;

        // used to keep track of current sprite and location
        Texture2D currentSprite;
        Rectangle drawRectangle = new Rectangle();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
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
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // STUDENTS: load the sprite images here
            nut1 = Content.Load<Texture2D>(@"graphics\tank_nut_DEATH");
            nut2 = Content.Load<Texture2D>(@"graphics\tank_nut_HIT");
            nut3 = Content.Load<Texture2D>(@"graphics\tank_nut_IDLE");
            // STUDENTS: set the currentSprite variable to one of your sprite variables
            current = nut1;
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

            elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsedTime > ChangeDelayTime)
            {
                elapsedTime = 0;

                // STUDENTS: uncomment the code below and make it generate a random number 
                // between 0 and 2 inclusive using the rand field I provided
                int spriteNumber = rand.Next(0, 3);

                // sets current sprite
                // STUDENTS: uncomment the lines below and change sprite0, sprite1, and sprite2
                //      to the three different names of your sprite variables
                if (spriteNumber == 0)
                {
                    current = nut1;
                }
                else if (spriteNumber == 1)
                {
                    current = nut2;
                }
                else if (spriteNumber == 2)
                {
                    current = nut3;
                }

                // STUDENTS: set the drawRectangle.Width and drawRectangle.Height to match the width and height of currentSprite

                // Selected images that where quite way to big, so I'm scaling them, hence why I'm dividing by 9
                drawRectangle.Width = current.Width / 9;
                drawRectangle.Height = current.Height / 9;

                // STUDENTS: center the draw rectangle in the window. Note that the X and Y properties of the rectangle
                // are for the upper left corner of the rectangle, not the center of the rectangle
                drawRectangle.X = graphics.PreferredBackBufferWidth / 2;
                drawRectangle.Y = graphics.PreferredBackBufferHeight / 2;

                // STUDENTS: write code below to generate random numbers  between -4 and 4 inclusive for the x and y speed 
                // using the rand field I provided
                // CAUTION: Don't redeclare the x speed and y speed variables here!
                speedX = rand.Next(-4, 4);
                speedY = rand.Next(-4, 4);
            }

            // STUDENTS: move the drawRectangle by the x speed and the y speed
            drawRectangle.X += speedX;
            drawRectangle.Y += speedY;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // STUDENTS: draw current sprite here
            spriteBatch.Begin();
            spriteBatch.Draw(current, drawRectangle, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
