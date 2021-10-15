using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject
{
    /// <summary>
    /// A burger
    /// </summary>
    public class Burger
    {
        #region Fields

        // graphic and drawing info
        Texture2D sprite;
        Rectangle drawRectangle;

        // burger stats
        int health = 100;

        // shooting support
        bool canShoot = true;
        bool useKeyboard = true;
        int elapsedCooldownMilliseconds = 0;

        // sound effect
        SoundEffect shootSound;

        bool buttonPressed = false;
        #endregion

        #region Constructors

        /// <summary>
        ///  Constructs a burger
        /// </summary>
        /// <param name="contentManager">the content manager for loading content</param>
        /// <param name="spriteName">the sprite name</param>
        /// <param name="x">the x location of the center of the burger</param>
        /// <param name="y">the y location of the center of the burger</param>
        /// <param name="shootSound">the sound the burger plays when shooting</param>
        public Burger(ContentManager contentManager, string spriteName, int x, int y,
            SoundEffect shootSound)
        {
            LoadContent(contentManager, spriteName, x, y);
            this.shootSound = shootSound;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collision rectangle for the burger
        /// </summary>
        public Rectangle CollisionRectangle
        {
            get { return drawRectangle; }
        }

        public int Health
        {
            get { return health; }
            set { health = value < 0 ? 0 : value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Updates the burger's location based on mouse. Also fires 
        /// french fries as appropriate
        /// </summary>
        /// <param name="gameTime">game time</param>
        /// <param name="mouse">the current state of the mouse</param>
        public void Update(GameTime gameTime, MouseState mouse, KeyboardState keyboard)
        {
            // burger should only respond to input if it still has health
            if (health > 0)
            {
                if (!useKeyboard)
                {
                    // move burger using mouse
                    // clamp burger in window
                    if (mouse.X >= 0 && mouse.X < GameConstants.WindowWidth - sprite.Width)
                        drawRectangle.X = mouse.X;
                    if (mouse.Y >= 0 && mouse.Y < GameConstants.WindowHeight - sprite.Height)
                        drawRectangle.Y = mouse.Y;
                }
                else
                {
                    // move burger using keyboard
                    // Up
                    if ((keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up)) && drawRectangle.Y > 0)
                        drawRectangle.Y -= GameConstants.BurgerMovementAmount;
                    // Down
                    if ((keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down)) && drawRectangle.Y < GameConstants.WindowHeight - sprite.Height)
                        drawRectangle.Y += GameConstants.BurgerMovementAmount;
                    // left
                    if ((keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left)) && drawRectangle.X > 0)
                        drawRectangle.X -= GameConstants.BurgerMovementAmount;
                    // Right
                    if ((keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right)) && drawRectangle.X < GameConstants.WindowWidth - sprite.Width)
                        drawRectangle.X += GameConstants.BurgerMovementAmount;
                }
                // update shooting allowed
                if (elapsedCooldownMilliseconds >= GameConstants.BurgerTotalCooldownMilliseconds)
                {
                    canShoot = true;
                    elapsedCooldownMilliseconds = 0;
                }
                // timer concept (for animations) introduced in Chapter 7
                if (!canShoot)
                {
                    elapsedCooldownMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
                }
                // shoot if appropriate
                if ((mouse.LeftButton == ButtonState.Released || keyboard.IsKeyUp(Keys.Space)) && buttonPressed)
                {
                    if(canShoot)
                    {
                        Projectile proj = new Projectile(ProjectileType.FrenchFries, Game1.GetProjectileSprite(ProjectileType.FrenchFries), 
                            drawRectangle.Center.X, drawRectangle.Center.Y + GameConstants.FrenchFriesProjectileOffset, GameConstants.FrenchFriesProjectileSpeed);
                        Game1.AddProjectile(proj);
                        shootSound.Play();
                        canShoot = false;
                    }
                }
                buttonPressed = mouse.LeftButton == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Space);
            }
        }

        /// <summary>
        /// Draws the burger
        /// </summary>
        /// <param name="spriteBatch">the sprite batch to use</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.sprite, this.drawRectangle, Color.White);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Loads the content for the burger
        /// </summary>
        /// <param name="contentManager">the content manager to use</param>
        /// <param name="spriteName">the name of the sprite for the burger</param>
        /// <param name="x">the x location of the center of the burger</param>
        /// <param name="y">the y location of the center of the burger</param>
        private void LoadContent(ContentManager contentManager, string spriteName,
            int x, int y)
        {
            // load content and set remainder of draw rectangle
            sprite = contentManager.Load<Texture2D>(spriteName);
            drawRectangle = new Rectangle(x - sprite.Width / 2,
                y - sprite.Height / 2, sprite.Width,
                sprite.Height);
        }

        #endregion
    }
}
