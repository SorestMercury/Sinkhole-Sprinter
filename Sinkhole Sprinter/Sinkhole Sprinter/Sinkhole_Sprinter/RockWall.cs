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
        public const float MAX_SPEED = 7;
        private float speed;
        private int timer;
        public bool collisionCheck;
        public RockWall(Rectangle rect, Texture2D texture) : base(rect, texture)
        {
            speed = 2;
        }
        public void Update()
        {
            if (speed < MAX_SPEED)
                speed += (MAX_SPEED - speed) / 3000;
            position.X += speed;
            if (collisionCheck)
            {
                timer++;
            }
            if (timer % 60 == 0)
            {
                collisionCheck = false;
            }
        }
        public void OnCollide()
        {
            collisionCheck = true;
            timer = 1;
        }
    }
}

