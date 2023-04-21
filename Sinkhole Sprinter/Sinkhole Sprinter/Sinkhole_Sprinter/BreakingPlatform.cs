using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sinkhole_Sprinter
{
    class BreakingPlatform : Platform
    {
        public const int BREAKING_TIME = 40;

        public int touchedTimer;

        public BreakingPlatform(Rectangle rect, Texture2D texture) : base(rect, texture)
        {
            touchedTimer = -1;
        }

        public override void Update()
        {
            if (touchedTimer != -1)
            {
                touchedTimer--;
            }
        }
    }
}
