using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProgrammingAssignment6
{
    /// <summary>
    /// A class for a menu button
    /// </summary>
    public class MenuButton
    {
        #region Fields

        // fields for button image
        Texture2D sprite;
        const int ImagesPerRow = 2;
        int buttonWidth;

        // fields for drawing
        Rectangle drawRectangle;
        Rectangle sourceRectangle;
        
        // click processing
        GameState clickState;
        bool clickStarted = false;
        bool buttonReleased = true;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sprite">the sprite for the button</param>
        /// <param name="center">the center of the button</param>
        /// <param name="clickState">the game state to change to when the button is clicked</param>
        public MenuButton(Texture2D sprite, Vector2 center, GameState clickState)
        {
            this.sprite = sprite;
            this.clickState = clickState;
            Initialize(center);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Updates the button to check for a button click
        /// </summary>
        /// <param name="gamepad">the current mouse state</param>
        public void Update(MouseState mouse)
        {
             // check for mouse over button
            if (drawRectangle.Contains(mouse.X, mouse.Y))
            {
                // highlight button
                sourceRectangle.X = buttonWidth;

                // check for click started on button
                if (mouse.LeftButton == ButtonState.Pressed &&
                    buttonReleased)
                {
                    clickStarted = true;
                    buttonReleased = false;
                }
                else if (mouse.LeftButton == ButtonState.Released)
                {
                    buttonReleased = true;

                    // if click finished on button, change game state
                    if (clickStarted)
                    {
                        Game1.ChangeState(clickState);
                        clickStarted = false;
                    }
                }
            }
            else
            {
                sourceRectangle.X = 0;

                // no clicking on this button
                clickStarted = false;
                buttonReleased = false;
            }
        }

        /// <summary>
        /// Draws the button
        /// </summary>
        /// <param name="spriteBatch">the spritebatch</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, drawRectangle, sourceRectangle, Color.White);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Initializes the button characteristics
        /// </summary>
        /// <param name="center">the center of the button</param>
         private void Initialize(Vector2 center)
        {
            // calculate button width
            buttonWidth = sprite.Width / ImagesPerRow;

            // set initial draw and source rectangles
            drawRectangle = new Rectangle(
                (int)(center.X - buttonWidth / 2),
                (int)(center.Y - sprite.Height / 2),
                buttonWidth, sprite.Height);
            sourceRectangle = new Rectangle(0, 0, buttonWidth, sprite.Height);
        }

        #endregion
    }
}
