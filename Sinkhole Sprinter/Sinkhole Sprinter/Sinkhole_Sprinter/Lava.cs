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
    class Lava : Sprite
    {
        // const int RISE_DELAY = 3;
        // int timer = 0;
        public Lava(Rectangle rect, Texture2D texture) : base(rect, texture)
        {

        }
        //public void Update()
        //{
        //    timer++;
        //    if (timer % RISE_DELAY == 0)
        //    {
        //        position.Y--;
        //    }
        //}
    }
}

