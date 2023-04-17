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
    class RockWall : Sprite
    {
        const float MAX_SPEED = 6.5f;
        float SPEED = 2;
        public RockWall(Rectangle rect, Texture2D texture) : base(rect, texture)
        {

        }
        public void Update()
        {
            if (SPEED < MAX_SPEED)
                SPEED += (MAX_SPEED - SPEED) / 4000;

            position.X += SPEED;
        }
    }
}

