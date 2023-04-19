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
        public const int HEIGHT = 20, MIN_WIDTH = 60, MAX_WIDTH = 200;
        

        /// <summary>
        /// Create a new Platform
        /// </summary>
        public Platform(Rectangle rect, Texture2D texture) : base(rect, texture) {
            
        }

        public virtual void Update()
        {
            
        }

    }
}
