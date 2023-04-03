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
        public const int HEIGHT = 20, MIN_WIDTH = 50, MAX_WIDTH = 200;
        public const int BREAKING_TIME = 40;

        // Does the platform break on touch
        public bool isBreaking;
        public int touchedTimer;

        /// <summary>
        /// Create a new permanent Platform
        /// </summary>
        public Platform(Rectangle rect, Texture2D texture) : this(rect, texture, false) { }

        /// <summary>
        /// Create a new Platform with specified type
        /// </summary>
        public Platform(Rectangle rect, Texture2D texture, bool isBreaking) : base(rect, texture) {
            this.isBreaking = isBreaking;
            touchedTimer = -1;
        }

        public void Update()
        {
            if (isBreaking && touchedTimer != -1)
            {
                touchedTimer--;
            }
        }

    }
}
