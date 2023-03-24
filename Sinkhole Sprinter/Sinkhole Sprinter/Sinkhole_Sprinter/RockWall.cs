﻿using Microsoft.Xna.Framework;
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
    class RockWall : Sprite
    {
        int timer = 0, speed = 1;
        public RockWall(Rectangle rect, Texture2D texture) : base(rect, texture)
        {

        }
        public void Update()
        {
            timer++;
            if (timer % 5 == 0)
            {
                position.X+=speed;
            }
        }
    }
}
