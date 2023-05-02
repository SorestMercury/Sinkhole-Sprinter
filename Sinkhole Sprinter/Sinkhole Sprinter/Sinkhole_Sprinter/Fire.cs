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
            fireAnim.Add(new Rectangle(70, 70, 80, 314));
            fireAnim.Add(new Rectangle(160, 50, 80, 314));
            fireAnim.Add(new Rectangle(255, 65, 80, 314));
            fireAnim.Add(new Rectangle(355, 70, 80, 314));
            fireAnim.Add(new Rectangle(455, 65, 80, 314));
            fireAnim.Add(new Rectangle(555, 75, 80, 314));
            fireAnim.Add(new Rectangle(640, 75, 80, 314));
            currentRect = new Rectangle(70, 70, 80, 314);
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
