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
    class FallingRocks : Sprite
    {
        public bool collisionCheck; // boolean to make sure that the players hearts dont drain to 0 on collision
        private List<Rectangle> fireAnim;
        private int timer;
        public Rectangle currentRect;
        public FallingRocks(Rectangle rect, Texture2D texture) : base(rect, texture)
        {
            fireAnim = new List<Rectangle>();
            fireAnim.Add(new Rectangle(0, 0, 100, 200));
            fireAnim.Add(new Rectangle(150, 0, 150, 200));
            fireAnim.Add(new Rectangle(325, 0, 150, 200));
            fireAnim.Add(new Rectangle(550, 0, 150, 200));
            fireAnim.Add(new Rectangle(750, 0, 175, 200));
            Random random = new Random();
            currentRect = fireAnim[random.Next(fireAnim.Count-1)];
            collisionCheck = false;
        }
        public void Update()
        {
            position.Y+=7;
            if (collisionCheck)
            {
                timer++;
            }
            if (timer % 150 == 0)
            {
                collisionCheck = false;
            }
        }
        public void Update(Vector2 p)
        {
            position = p;
        }
        public void OnCollide()
        {
            collisionCheck = true;
            timer = 1;
        }
    }
}
