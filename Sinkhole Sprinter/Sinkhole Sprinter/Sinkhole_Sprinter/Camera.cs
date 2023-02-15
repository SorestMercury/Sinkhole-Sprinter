using System;
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

        public Camera(int screenWidth, int screenHeight)
        {
            position = new Vector2(screenWidth / 2, screenHeight / 2);
            boundingRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
        }

        /// <summary>
        /// Draws one sprite to the screen relative to the camera's position
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Sprite sprite)
        {
            // Set the rectangle to the correct relative position
            sprite.rect.X = (int)(position.X - sprite.position.X + boundingRectangle.Width / 2);
            sprite.rect.Y = (int)(position.Y - sprite.position.Y + boundingRectangle.Height / 2);
            spriteBatch.Draw(sprite.texture, sprite.rect, null, Color.White, 0, new Vector2(sprite.texture.Width / 2, sprite.texture.Height / 2), SpriteEffects.None, 0);
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
    }
}
