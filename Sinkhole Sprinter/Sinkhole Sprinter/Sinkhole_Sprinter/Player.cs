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
    class Player
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Texture2D spreadsheet;
        private List<Rectangle> running, jumping;
        public Rectangle currentsource;
        public Vector2 currentdest;
        private Rectangle standing;
        private int currentInt;
        private bool isJumping;
        int speed, jump;
        int timer, timer2;
        KeyboardState oldkb = Keyboard.GetState();
        public Player(Texture2D s, List<Rectangle> r, List<Rectangle> j, Rectangle st)
        {

            spreadsheet = s;
            running = r;
            jumping = j;
            currentsource = st;
            standing = st;
            currentdest = new Vector2(100, 100);
            currentInt = 0;
            isJumping = false;
            speed = 1;
            jump = 5;
            timer = 0;
            timer2 = 0;
        }

        public void Update()
        {
            timer++;
            timer2++;
            currentdest.Y++;
            KeyboardState kb = Keyboard.GetState();
            if (timer % 10 == 0)
                currentsource = standing;
            if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left))
            {
                currentdest.X -= speed;
                if (timer % 10 == 0)
                {
                    currentInt++;
                    if (currentInt == running.Count)
                    {
                        currentInt = 0;
                    }

                    currentsource = running[currentInt];
                }

            }
            if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right))
            {
                currentdest.X += speed;
                if (timer % 10 == 0)
                {
                    currentInt++;
                    if (currentInt == running.Count)
                    {
                        currentInt = 0;
                    }

                    currentsource = running[currentInt];
                }
            }
            if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up))
            {
                if (timer2 > 25)
                {
                    isJumping = true;
                    timer2 = 0;
                }
            }
            if (timer2 > 5)
            {
                isJumping = false;

            }
            if (isJumping)
            {
                currentdest.Y -= jump;
                currentsource = jumping[0];
            }
            oldkb = kb;
        }

    }
}
