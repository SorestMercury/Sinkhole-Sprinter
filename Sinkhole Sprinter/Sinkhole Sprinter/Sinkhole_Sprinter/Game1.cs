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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D spreadsheet;
        private List<Rectangle> running, jumping;
        Player player;
        Camera camera;

        //const int PLATFORM_SPEED = 3;
        List<Platform> platforms;
        Platform LastPlatform
        {
            get => platforms[platforms.Count - 1];
        }

        Texture2D placeholder;
        int timer = 0;
        Random r = new Random();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            camera = new Camera(new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
            running = new List<Rectangle>();
            jumping = new List<Rectangle>();
            running.Add(new Rectangle(0, 0, 400, 400));
            running.Add(new Rectangle(400, 0, 400, 400));
            running.Add(new Rectangle(800, 0, 400, 400));
            running.Add(new Rectangle(1200, 0, 400, 400));
            jumping.Add(new Rectangle(0, 0, 400, 400));

            platforms = new List<Platform>();
            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            spreadsheet = this.Content.Load<Texture2D>("player_running_spritesheet_25");
            player = new Player(new Rectangle(50, 0, 75, 75), spreadsheet, running, jumping, new Rectangle(0, 0, 400, 400));

            placeholder = this.Content.Load<Texture2D>("white");
            createPlatform(new Vector2(Platform.WIDTH / 2, camera.boundingRectangle.Height * .7f));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            

            for (int x = 0; x < platforms.Count; x++)
            {
                //platforms[x].update(PLATFORM_SPEED);

                if (platforms[x].offScreen)
                {
                    platforms.Remove(platforms[x]);
                    x--;
                    continue;
                }

                if (platforms[x].rect.Intersects(player.rect))
                    player.CheckCollisions(platforms[x]);
            }

            //if (timer % 60 == 0)
            //{
            //    platforms.Add(new Platform(new Rectangle(1280, platforms[platforms.Count-1].rect.Y+r.Next(-150,50), 70, 10), placeholder));
            //}

            player.Update();
            camera.Update();
            if (LastPlatform.position.X < camera.boundingRectangle.Right)
            {
                createPlatform();
            }
            camera.position.X = Math.Max(player.position.X, camera.boundingRectangle.Width / 2);
            camera.position.Y = Math.Min(player.position.Y, camera.boundingRectangle.Height / 2);

            timer++;
            base.Update(gameTime);
        }

        // Create a new platform in calculated position
        private void createPlatform()
        {
            int dHeight = r.Next(-150, 50);
            Vector2 position = new Vector2();
            // Make reasonable X and Y
            position.Y = LastPlatform.position.Y + dHeight;
            position.X = Math.Max(LastPlatform.position.X + (float)(r.NextDouble() * .5 + .3) * player.GetMaxJumpDistance(dHeight), LastPlatform.position.X + Platform.WIDTH * 2);

            createPlatform(position);
        }
        private void createPlatform(Vector2 position)
        {
            platforms.Add(new Platform(new Rectangle((int)position.X, (int)position.Y, Platform.WIDTH, Platform.HEIGHT), placeholder));
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            foreach (Platform platform in platforms)
            {
                camera.Draw(gameTime, spriteBatch, platform);
            }

            camera.DrawPlayer(gameTime, spriteBatch, player);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
