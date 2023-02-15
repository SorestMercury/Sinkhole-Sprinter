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

        /// <summary>
        /// Create a new Sprite object
        /// </summary>
        /// <param name="position">Actual position of the sprite</param>
        /// <param name="rect">Only for size</param>
        /// <param name="texture">Texture to draw</param>
        public Sprite(Vector2 position, Rectangle rect, Texture2D texture)
        {
            this.position = position;
            this.rect = rect;
            this.texture = texture;
        }
    }
}
