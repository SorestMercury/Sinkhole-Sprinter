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
    class Player : Sprite
    {
        const int MAX_SPEED = 7, JUMP = 20, MAX_FALL_SPEED = 20;
        const float ACCELERATION = .8f, GRAVITY = 1, DRAG_FACTOR = .8f;

        // Source rects
        private List<Rectangle> running, jumping;
        // Current source rectangle
        public Rectangle currentsource;
        // Standing animation
        private Rectangle standing;
        // If player is moving
        public movement playerState;
        // Frame of animation
        private int currentInt;
        // Timer for animation
        int timer;
        // If player is off ground
        private bool canJump;
        // For keyboard debounce
        KeyboardState oldkb;

        // Physics
        Vector2 velocity;
        Vector2 acceleration;

        public enum movement
        {
            idle,
            left,
            right
        }

        
        public Player(Texture2D s, List<Rectangle> r, List<Rectangle> j, Rectangle st) : base(new Rectangle(50, 360, 75, 75), s)
        {
            oldkb = Keyboard.GetState();
            running = r;
            jumping = j;
            currentsource = st;
            standing = st;
            playerState = movement.idle;
            currentInt = 0;
            canJump = true;
            timer = 0;
            velocity = new Vector2(0, 0);
            acceleration = new Vector2(0, GRAVITY);
        }

        // Increment one frame in running animation
        public void ChangeRunningFrame()
        {
            currentInt = (currentInt + 1) % running.Count;
        }

        public float GetMaxJumpDistance(float dHeight)
        {
            return MAX_SPEED * (20 + (float)Math.Sqrt(400 - 2 * dHeight));
        }

        public void Update()
        {
            
            KeyboardState kb = Keyboard.GetState();

            // Movement
            if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left))
            {
                acceleration.X = -ACCELERATION;
                if (timer % 8 == 0)
                {
                    ChangeRunningFrame();

                    currentsource = running[currentInt];
                    playerState = movement.left;
                }

            }
            else if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right))
            {
                acceleration.X = ACCELERATION;
                if (timer % 8 == 0)
                {
                    ChangeRunningFrame();

                    currentsource = running[currentInt];
                    playerState = movement.right;
                }
            }
            else
            {
                if (timer % 8 == 0)
                {
                    currentsource = standing;
                    // Uncomment when 
                    //playerState = movement.idle;
                }
                acceleration.X = 0;
                velocity.X *= DRAG_FACTOR;
                if (Math.Abs(velocity.X) < MAX_SPEED / 20f)
                    velocity.X = 0;
            }

            if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up) || kb.IsKeyDown(Keys.Space))
            {
                if (canJump)
                {
                    canJump = false;
                    velocity.Y = -JUMP;
                    currentsource = jumping[0];
                }
            }

            if (velocity.Y > 0)
                canJump = false;

            velocity.X += acceleration.X;
            velocity.Y += acceleration.Y;

            // Keep velocity in bounds
            velocity.X = Math.Min(Math.Max(-MAX_SPEED, velocity.X), MAX_SPEED);
            velocity.Y = Math.Min(MAX_FALL_SPEED, velocity.Y);

            position.X += velocity.X;
            position.Y += velocity.Y;

            if (Bottom > 720)
            {
                canJump = true;
                position.Y = 720 - rect.Height / 2;
                velocity.Y = 0;
            }

            timer++;
            oldkb = kb;
        }


        // Check if the player is on top of a platform and make functionality
        public void CheckCollisions(Platform platform)
        {
            // DEBUG THIS
            if (Bottom - MAX_FALL_SPEED < platform.Top)
            {
                canJump = true;
                position.Y = platform.Top - rect.Height / 2;
                velocity.Y = 0;
            }
        }

    }
}
