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
        public Vector2 position;
        public Rectangle boundingRectangle;

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

        /// <summary>
        /// Draws one sprite to the screen relative to the camera's position
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Sprite sprite)
        {
            // Set the rectangle to the correct relative position
            sprite.rect.X = (int)(sprite.position.X - sprite.rect.Width / 2 - position.X + boundingRectangle.Width / 2);
            sprite.rect.Y = (int)(sprite.position.Y - sprite.rect.Height / 2 - position.Y + boundingRectangle.Height / 2);
            Color color = Color.White;

            if (sprite is Platform)
            {
                Platform platform = (Platform)sprite;
                if (platform.isBreaking)
                {
                    // Fade colors if the platform is breaking
                    byte value = (byte)(255 * platform.touchedTimer / Platform.BREAKING_TIME);
                    color = new Color(value, value, value, value);
                }
            }

            spriteBatch.Draw(sprite.texture, sprite.rect, null, color);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Sprite sprite, Rectangle sourceRect)
        {
            // Set the rectangle to the correct relative position
            sprite.rect.X = (int)(sprite.position.X - sprite.rect.Width / 2 - position.X + boundingRectangle.Width / 2);
            sprite.rect.Y = (int)(sprite.position.Y - sprite.rect.Height / 2 - position.Y + boundingRectangle.Height / 2);
            Color color = Color.White;

            if (sprite is Platform)
            {
                Platform platform = (Platform)sprite;
                if (platform.isBreaking)
                {
                    // Fade colors if the platform is breaking
                    byte value = (byte)(255 * platform.touchedTimer / Platform.BREAKING_TIME);
                    color = new Color(value, value, value, value);
                }
            }

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

            spriteBatch.Draw(player.texture, player.rect, player.currentsource, Color.White, 0, new Vector2(0, 0), flip, 0);
        }
    }
}
