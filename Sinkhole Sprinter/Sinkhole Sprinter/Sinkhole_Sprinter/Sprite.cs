using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sinkhole_Sprinter
{
    abstract class Sprite
    {
        // Actual position of the sprite, centered
        public Vector2 position;
        // Rectangle for size, X and Y updated by camera to draw
        public Rectangle rect;
        // Texture of the sprite
        public Texture2D texture;

        public int Top
        {
            get => (int)(position.Y - rect.Height / 2);
        }
        public int Left
        {
            get => (int)(position.X - rect.Width / 2);
        }
        public int Bottom
        {
            get => (int)(position.Y + rect.Height / 2);
        }
        public int Right
        {
            get => (int)(position.X + rect.Width / 2);
        }

        /// <summary>
        /// Create a new Sprite object
        /// </summary>
        /// <param name="rect">Rectangle centered (only for constructor) at X and Y</param>
        /// <param name="texture">Texture to draw</param>
        public Sprite(Rectangle rect, Texture2D texture)
        {
            this.rect = rect;
            this.position = new Vector2(rect.X, rect.Y);
            this.texture = texture;
        }

    }
}
