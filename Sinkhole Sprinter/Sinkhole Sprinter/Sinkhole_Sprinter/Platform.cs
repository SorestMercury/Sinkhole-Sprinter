using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sinkhole_Sprinter
{
    class Platform : Sprite
    {
        // Does the platform break on touch
        public bool isBreaking;
        public bool offScreen;

        /// <summary>
        /// Create a new permanent Platform
        /// </summary>
        public Platform(Rectangle rect, Texture2D texture) : this(rect, texture, false) { }

        /// <summary>
        /// Create a new Platform with specified type
        /// </summary>
        public Platform(Rectangle rect, Texture2D texture, bool isBreaking) : base(rect, texture) {
            this.isBreaking = isBreaking;
        }

        public void update(int speed)
        {
            if (rect.Right < 0)
                offScreen = true;

            else
                position.X -= speed;
        }

    }
}
