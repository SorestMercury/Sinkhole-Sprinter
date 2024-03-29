﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sinkhole_Sprinter
{
    class Camera
    {
        const int VERTICAL_SPEED = 3;
        public Vector2 position;
        public Rectangle boundingRectangle;
        float lastHeight;

        public int Top
        {
            get => (int)(position.Y - boundingRectangle.Height / 2);
        }
        public int Left
        {
            get => (int)(position.X - boundingRectangle.Width / 2);
        }
        public int Bottom
        {
            get => (int)(position.Y + boundingRectangle.Height / 2);
        }
        public int Right
        {
            get => (int)(position.X + boundingRectangle.Width / 2);
        }

        /// <summary>
        /// Create a new Camera located at the starting position
        /// </summary>
        public Camera(Vector2 screenSize) : this(screenSize, new Vector2(screenSize.X / 2, screenSize.Y / 2)) { }

        /// <summary>
        /// Create a new Camera with the same size as the screen centered at position
        /// </summary>
        public Camera(Vector2 screenSize, Vector2 position)
        {
            this.position = position;
            boundingRectangle = new Rectangle((int)(position.X - screenSize.X / 2), (int)(position.Y - screenSize.Y / 2), (int)screenSize.X, (int)screenSize.Y);
            lastHeight = position.Y;
        }

        /// <summary>
        /// Update the camera
        /// </summary>
        public void Update()
        {
            UpdateBoundingRectangle();
        }

        /// <summary>
        /// Update the boundingRectangle based on the position
        /// </summary>
        public void UpdateBoundingRectangle()
        {
            boundingRectangle.X = (int)(position.X - boundingRectangle.Width / 2);
            boundingRectangle.Y = (int)(position.Y - boundingRectangle.Height / 2);
        }
        
        // Follow the players' x coordinates
        public void FollowX(Player player, Player player2)
        {
            float x = player.position.X;
            if (player2 != null)
            {
                // Avg position, at most .2x screen width left of the rightmost player
                x = Math.Max((x + player2.position.X) / 2, Math.Max(player.position.X, player2.position.X) - .2f * boundingRectangle.Width);
            }
            position.X = Math.Max(x, boundingRectangle.Width / 2);
        }

        // Follow the players' y coordinates
        public void FollowY(Player player, Player player2)
        {
            // Avg y position needs to be 60% line
            float neededPlayerPos = position.Y + boundingRectangle.Height * .1f;
            float avgHeight = player.lastHeight;
            if (player2 != null)
                avgHeight += (player2.lastHeight - avgHeight) / 2;

            // If close, set equal
            if (Math.Abs(avgHeight - neededPlayerPos) < 1)
            {
                position.Y = avgHeight - boundingRectangle.Height * .1f;
                lastHeight = position.Y;
            }
            else
                //position.Y += Math.Min(Math.Max((player.lastHeight - neededPlayerPos) * .05f, -VERTICAL_SPEED), VERTICAL_SPEED);
                // Slowly move camera to position
                position.Y += Math.Min(Math.Max((avgHeight - neededPlayerPos) * (float)Math.Sqrt(Math.Abs(position.Y - lastHeight) + 1) * .005f, -VERTICAL_SPEED), VERTICAL_SPEED);
        }

        // Draws one sprite to the screen relative to the camera's position
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Sprite sprite)
        {
            // Set the rectangle to the correct relative position
            sprite.rect.X = (int)(sprite.position.X - sprite.rect.Width / 2 - position.X + boundingRectangle.Width / 2);
            sprite.rect.Y = (int)(sprite.position.Y - sprite.rect.Height / 2 - position.Y + boundingRectangle.Height / 2);
            Color color = Color.White;
            
            // Breaking platform disappearing
            if (sprite is BreakingPlatform)
            {
                BreakingPlatform platform = (BreakingPlatform)sprite;
                // Fade colors if the platform is breaking
                byte value = (byte)(255 * platform.touchedTimer / BreakingPlatform.BREAKING_TIME);
                color = new Color(value, value, value, value);
            }

            spriteBatch.Draw(sprite.texture, sprite.rect, color);
        }

        // Draw a sprite that requires a source rectangle
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Sprite sprite, Rectangle sourceRect)
        {
            // Set the rectangle to the correct relative position
            sprite.rect.X = (int)(sprite.position.X - sprite.rect.Width / 2 - position.X + boundingRectangle.Width / 2);
            sprite.rect.Y = (int)(sprite.position.Y - sprite.rect.Height / 2 - position.Y + boundingRectangle.Height / 2);
            Color color = Color.White;

            spriteBatch.Draw(sprite.texture, sprite.rect, sourceRect, color);
        }

        /// <summary>
        /// Draws a list of sprites to the screen relative to the camera's position
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, List<Sprite> sprites)
        {
            foreach (Sprite sprite in sprites)
            {
                Draw(gameTime, spriteBatch, sprite);
            }
        }

        /// <summary>
        /// Draw the player, flipping horizontally as necessary
        /// </summary>
        public void DrawPlayer(GameTime gameTime, SpriteBatch spriteBatch, Player player)
        {
            SpriteEffects flip = SpriteEffects.None;

            if (player.moveDirection == Player.direction.right && player.playerState != Player.movement.idle)
            {
                flip = SpriteEffects.FlipHorizontally;
            }
            else if (player.moveDirection == Player.direction.left && player.playerState == Player.movement.idle)
            {
                flip = SpriteEffects.FlipHorizontally;
            }

            player.rect.X = (int)(player.position.X - player.rect.Width / 2 - position.X + boundingRectangle.Width / 2);
            player.rect.Y = (int)(player.position.Y - player.rect.Height / 2 - position.Y + boundingRectangle.Height / 2);

            spriteBatch.Draw(player.texture, player.rect, player.currentsource, player.GetColor(), 0, new Vector2(0, 0), flip, 0);
        }
    }
}
