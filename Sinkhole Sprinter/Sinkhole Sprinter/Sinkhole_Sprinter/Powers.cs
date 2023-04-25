using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sinkhole_Sprinter
{
    class Power : Sprite
    {
        public enum variant
        {
            speed,
            jump,
        }

        variant type; 
        public Power(Rectangle rect, Texture2D texture, variant type) : base(rect, texture) 
        {
            this.type = type;
        }


    }
}
