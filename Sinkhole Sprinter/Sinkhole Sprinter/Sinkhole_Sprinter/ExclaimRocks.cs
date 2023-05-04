using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sinkhole_Sprinter
{
    class ExclaimRocks : Sprite
    {
        public int timer = 100;
        private Random randomGen = new Random();
        public Vector2 pastPosition;
        public ExclaimRocks(Rectangle rect, Texture2D texture) : base(rect, texture)
        {

        }
        public void Update(float flo, int left, int right)
        {
            timer++;
            if (timer % 180 == 0)
            {
                pastPosition = position;
                position.X = randomGen.Next(left, right + 640);
            }
            position.Y = (int)flo;
        }
    }
}
