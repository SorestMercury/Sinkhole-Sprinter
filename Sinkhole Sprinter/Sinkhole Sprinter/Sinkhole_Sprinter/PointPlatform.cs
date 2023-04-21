using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sinkhole_Sprinter
{
    class PointPlatform : Platform
    {
        public int points;
        public bool used;

        public PointPlatform(Rectangle rect, Texture2D texture, int points) : base(rect, texture)
        {
            this.points = points;
            used = false;
        }
    }
}
