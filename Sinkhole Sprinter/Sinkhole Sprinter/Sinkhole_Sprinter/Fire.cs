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
    class Fire
    {
        private Texture spreadsheet;
        public List<Rectangle> fireAnim;
        public int currentRectangle = 0;
        private int timer = 0, speed, timer2 = 0;
        public Rectangle destRectangle, exclaimRectangle;
        private bool exclaim = false, start = false;
        public Fire(Texture s, int sp)
        {
            spreadsheet = s;
            speed = sp;
            fireAnim = new List<Rectangle>();
            fireAnim.Add(new Rectangle(70, 70, 80, 314));
            fireAnim.Add(new Rectangle(160,50, 80, 314));
            fireAnim.Add(new Rectangle(255, 65, 80, 314));
            fireAnim.Add(new Rectangle(355, 70, 80, 314));
            fireAnim.Add(new Rectangle(455, 65, 80, 314));
            fireAnim.Add(new Rectangle(555, 75, 80, 314));
            fireAnim.Add(new Rectangle(640, 75, 80, 314));
            exclaimRectangle = new Rectangle(2000, 2000, 75, 75);
            destRectangle = new Rectangle(2000, 1000, 50, 150);
        }
        public void Update()
        {
            timer++;

            if (timer > 100)
                start = true;
            if (start)
            {
                if (timer % speed == 0)
                {
                    currentRectangle++;
                    if (currentRectangle == fireAnim.Count)
                        currentRectangle = 0;
                }
                if (timer % 200 == 0)
                {
                    exclaim = true;
                    Random randomGen = new Random();
                    exclaimRectangle = new Rectangle(randomGen.Next(1280), 550, 75, 75);
                }

                if (exclaim)
                {
                    timer2++;
                }

                if (timer2 == 200)
                {
                    exclaim = false;
                    timer2 = 0;
                    destRectangle = new Rectangle(exclaimRectangle.X, exclaimRectangle.Y - 35, 50, 150);
                }
            }    
            
        }
    }
}
