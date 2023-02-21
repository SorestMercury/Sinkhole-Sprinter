using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sinkhole_Sprinter
{
    class Sprite
    {
        public Vector2 position;
        public Rectangle rect;
        public Texture2D texture;
        public Vector2 origin;


        /// <summary>
        /// Create a new Sprite object
        /// </summary>
        /// <param name="rect">Rectangle centered at X and Y</param>
        /// <param name="texture">Texture to draw</param>
        public Sprite(Rectangle rect, Texture2D texture)
        {
            this.rect = rect;
            this.position = new Vector2(rect.X, rect.Y);
            this.texture = texture;
            this.origin = new Vector2(texture.Width / 2, texture.Height / 2);
        }
    }
}
