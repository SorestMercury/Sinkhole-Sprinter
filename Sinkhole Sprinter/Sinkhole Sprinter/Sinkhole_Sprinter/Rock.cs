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
    class Rock : Sprite
    {
        private int speed;
        public Rock(Rectangle rect, Texture2D texture, int s) : base(rect, texture)
        {
            speed = s;
        }
        public void Update()
        {
            position.Y+=speed;
        }
    }
}

