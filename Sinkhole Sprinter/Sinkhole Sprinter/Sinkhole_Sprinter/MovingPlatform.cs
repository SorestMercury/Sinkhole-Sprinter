using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sinkhole_Sprinter
{
    class MovingPlatform : Platform
    {
        public const int MAXH = 100, MAXV = 50, PERIOD = 180;

        public Vector2 basePos;
        public string axis;
        public int timer;

        public MovingPlatform(Rectangle rect, Texture2D texture, string axis) : base(rect, texture)
        {
            basePos = new Vector2(rect.X, rect.Y);
            this.axis = axis;
            timer = 0;
        }

        public override void Update()
        {
            if (axis == "x")
            {
                position.X = basePos.X + MAXH * (float)Math.Sin(2 * Math.PI * timer / PERIOD);
            }
            else if (axis == "y")
            {
                position.Y = basePos.Y + MAXV * (float)Math.Sin(2 * Math.PI * timer / PERIOD);
            }
            timer = (timer + 1) % PERIOD;
        }
    }
}
