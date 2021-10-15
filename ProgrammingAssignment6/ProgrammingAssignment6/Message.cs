using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProgrammingAssignment6
{
    /// <summary>
    /// A message
    /// </summary>
    public class Message
    {
        #region Fields

        string text;
        SpriteFont font;
        Vector2 center;
        Vector2 position;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">the text for the message</param>
        /// <param name="font">the sprite font for the message</param>
        /// <param name="center">the center of the message</param>
        public Message(string text, SpriteFont font, Vector2 center)
        {
            this.text = text;
            this.font = font;
            this.center = center;

            // calculate position from text and center
            float textWidth = font.MeasureString(text).X;
            float textHeight = font.MeasureString(text).Y;
            position = new Vector2(center.X - textWidth / 2,
                center.Y - textHeight / 2);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets the text for the message
        /// </summary>
        public string Text
        {
            set 
            { 
                text = value; 

                // changing text could change text location
                float textWidth = font.MeasureString(text).X;
                float textHeight = font.MeasureString(text).Y;
                position.X = center.X - textWidth / 2;
                position.Y = center.Y - textHeight / 2;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Draws the message
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, text, position, Color.White);
        }

        #endregion
    }
}
