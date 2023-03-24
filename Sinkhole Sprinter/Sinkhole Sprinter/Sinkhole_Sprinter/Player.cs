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
        const int MAX_SPEED = 8, JUMP = 20, MAX_FALL_SPEED = 25;
        const float ACCELERATION = .8f, GRAVITY = 1, DRAG_FACTOR = .8f, AIR_RESISTANCE = .95f;

        //in the instance that multiple sprite sheets are necessary
        List<Texture2D> textures;

        // Source rects
        private List<Rectangle> running, jumping, standing;
        // Current source rectangle
        public Rectangle currentsource;
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

        
        public Player(Rectangle rect, Texture2D s, List<Rectangle> r, List<Rectangle> j, List<Rectangle> st) : base(rect, s)
        {
            oldkb = Keyboard.GetState();
            running = r;
            jumping = j;
            currentsource = st[0];
            standing = st;
            playerState = movement.idle;
            currentInt = 0;
            canJump = true;
            timer = 0;
            velocity = new Vector2(0, 0);
            acceleration = new Vector2(0, GRAVITY);
        }

        public Player(Rectangle rect, List<Texture2D> s, List<Rectangle> r, List<Rectangle> j, List<Rectangle> st) : base(rect, s[0])
        {
            texture = s[0];
            textures = s;
            oldkb = Keyboard.GetState();
            running = r;
            jumping = j;
            currentsource = st[0];
            standing = st;
            // playerState = movement.idle;
            playerState = movement.right;
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

        // Get the maximum distance the player can jump, factoring in player and platform width
        public float GetMaxJumpDistance(float dHeight)
        {
            return MAX_SPEED * (JUMP + (float)Math.Sqrt(Math.Pow(JUMP, 2) + 2 * dHeight)) + Platform.WIDTH + rect.Width;
        }

        public void Update()
        {
            
            KeyboardState kb = Keyboard.GetState();

            // Movement
            if (kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.Left))
            {
                acceleration.X = -ACCELERATION;
                if (timer % 8 == 0 && canJump)
                {
                    ChangeRunningFrame();

                    currentsource = running[currentInt];
                }
                playerState = movement.left;
            }
            else if (kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.Right))
            {
                acceleration.X = ACCELERATION;
                if (timer % 8 == 0 && canJump)
                {
                    ChangeRunningFrame();

                    currentsource = running[currentInt];
                }
                playerState = movement.right;
            }
            else
            {
                acceleration.X = 0;
                velocity.X *= DRAG_FACTOR;
                acceleration.X = 0;
                if (canJump)
                {
                    if (timer % 8 == 0)
                    {
                        currentsource = standing[0];
                        playerState = movement.idle;
                    }
                    velocity.X *= DRAG_FACTOR;
                }
                else
                {
                    velocity.X *= AIR_RESISTANCE;
                    
                }
                if (Math.Abs(velocity.X) < MAX_SPEED / 20f)
                    velocity.X = 0;
            }

            if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.Up) || kb.IsKeyDown(Keys.Space))
            {
                if (canJump)
                {
                    canJump = false;
                    velocity.Y = -JUMP;
                }
            }

            if (velocity.Y > 0)
            {
                canJump = false;
            }

            //check if player is in the air, if they are, switch to jumping spritesheet and animate accordingly
            if (!canJump)
            {
                if (timer % 8 == 0)
                {
                    texture = textures[1];
                    currentInt = (currentInt + 1) % jumping.Count;
                    currentsource = jumping[currentInt];
                }
            }

            //check if player is standing still, if so, switch to idle spritesheet
            else if (playerState == movement.idle)
            {
                if(timer%8==0)
                {
                    texture = textures[2];
                    currentInt = (currentInt + 1) % standing.Count;
                    currentsource = standing[currentInt];
                }
            }

            else
            {
                if(timer%8==0)
                    texture = textures[0];
            }

            velocity.X += acceleration.X;
            velocity.Y += acceleration.Y;

            // Keep velocity in bounds
            velocity.X = Math.Min(Math.Max(-MAX_SPEED, velocity.X), MAX_SPEED);
            velocity.Y = Math.Min(MAX_FALL_SPEED, velocity.Y);

            position.X += velocity.X;
            position.Y += velocity.Y;

            position.X = Math.Max(position.X, rect.Width / 2);

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
            if (Bottom - MAX_FALL_SPEED < platform.Top && velocity.Y > 0)
            {
                canJump = true;
                position.Y = platform.Top - rect.Height / 2;
                velocity.Y = 0;
            }
        }

    }
}
