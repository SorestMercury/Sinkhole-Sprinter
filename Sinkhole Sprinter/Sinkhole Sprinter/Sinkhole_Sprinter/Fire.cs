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
    class Fire : Sprite
    {
        private List<Rectangle> fireAnim;
        private int timer = 0, current = 0;
        public Rectangle currentRect;
        public Fire(Rectangle rect, Texture2D texture) : base(rect, texture)
        {
            fireAnim = new List<Rectangle>();
            fireAnim.Add(new Rectangle(0, 20, 80, 216));
            fireAnim.Add(new Rectangle(90, 0, 80, 216));
            fireAnim.Add(new Rectangle(185, 15, 80, 216));
            fireAnim.Add(new Rectangle(285, 20, 80, 216));
            fireAnim.Add(new Rectangle(385, 15, 80, 216));
            fireAnim.Add(new Rectangle(485, 25, 80, 216));
            fireAnim.Add(new Rectangle(580, 25, 80, 216));
            currentRect = fireAnim[0];
        }
        public void Update(Vector2 p)
        {
            if (timer % 10 == 0)
            {
                current++;
                if (current == 7)
                    current = 0;
                currentRect = fireAnim[current];
            }
            timer++;
            position = p;
        }
    }
}
