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
        enum Gamestate
        {
            title, play, gameover
        }
        Gamestate currentState = Gamestate.play;
        SpriteFont titleFont, titleTextFont, testFont;
        public Color titleColor = Color.Black;
        Rectangle titleRect, multiplayerTextRect;
        MouseState mouse, oldMouse;
        Color[] titleScreenColors = { Color.Black, Color.Black, Color.Black };
        Color[] deathScreenColors = { Color.Black, Color.Black, Color.Black };

        String[] titleScreenText = { "single player", "multiplayer" };
        String[] gameoverScreenText = { "play again", "main menu" };
        Rectangle[] deathScreenText;

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
            IsMouseVisible = true;
            platforms = new List<Platform>();

            deathScreenText = new Rectangle[5];
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

            titleTextFont = Content.Load<SpriteFont>("SpriteFont2");
            titleFont = Content.Load<SpriteFont>("SpriteFont1");
            titleRect = new Rectangle((int)(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString(titleScreenText[0]).Length() / 2)), 200, 30, 30);
            multiplayerTextRect = new Rectangle((int)(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString(titleScreenText[1]).Length() / 2)), 300, 30, 30);
            deathScreenText[0] = new Rectangle((int)(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("play again").Length() / 2)), 250, 30, 30);
            deathScreenText[1] = new Rectangle((int)(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("main menu").Length() / 2)), 350, 30, 30);

            player = new Player(new Rectangle(50, 360, 75, 75), spreadsheet, running, jumping, new Rectangle(0, 0, 400, 400));

            placeholder = this.Content.Load<Texture2D>("white");
            createPlatform(new Vector2(Platform.WIDTH / 2, camera.boundingRectangle.Height * .7f));

            testFont = Content.Load<SpriteFont>("SpriteFont3");
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
             mouse = Mouse.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            switch (currentState)
            {
                case Gamestate.title:
                    if (mouse.X > titleRect.X && mouse.X < titleRect.X + (("singleplayer".Length - 1) * 20) && mouse.Y > titleRect.Y + 10 && mouse.Y < titleRect.Y + titleRect.Height)
                    {
                        titleScreenColors[0] = Color.Gold;
                        if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                        {
                            currentState = Gamestate.play;
                        }
                    }
                    else
                        titleScreenColors[0] = Color.Black;
                    if (mouse.X > multiplayerTextRect.X && mouse.X < multiplayerTextRect.X + (("multiplayer".Length - 1) * 20) && mouse.Y > multiplayerTextRect.Y + 10 && mouse.Y < multiplayerTextRect.Y + multiplayerTextRect.Height)
                    {
                        titleScreenColors[1] = Color.Gold;
                        if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                        {
                            currentState = Gamestate.play;
                        }
                    }
                    else
                        titleScreenColors[1] = Color.Black;
                    break;
                case Gamestate.play:
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
                    if (player.position.Y >= 683 ) // checks if player is dead
                    {
                        currentState = Gamestate.gameover;
                    }
                    break;
                case Gamestate.gameover:
                    KeyboardState kb = Keyboard.GetState();
                    KeyboardState oldKb = Keyboard.GetState();

                    if (kb.IsKeyDown(Keys.R) && !oldKb.IsKeyDown(Keys.R))
                    {
                        currentState = Gamestate.play;
                        player.position.Y = 400;
                        player.position.X = 100;
                    }
                    if (mouse.X > deathScreenText[0].X &&
                        mouse.X < deathScreenText[0].X + (("play again".Length - 1) * 20) && mouse.Y > deathScreenText[0].Y + 10 &&    mouse.Y < deathScreenText[0].Y + deathScreenText[0].Height)
                    {
                        deathScreenColors[0] = Color.Gold;
                        if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                        {
                            currentState = Gamestate.title;
                            player.position.Y = 400;
                            player.position.X = 100;


                        }
                    }
                    else
                        deathScreenColors[0] = Color.Black;
                    if (mouse.X > deathScreenText[1].X && mouse.X < deathScreenText[1].X + (("main menu".Length - 1) * 20) && mouse.Y > deathScreenText[1].Y + 10 && mouse.Y < deathScreenText[1].Y + deathScreenText[1].Height)
                    {
                        deathScreenColors[1] = Color.Gold;
                        if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                        {
                            currentState = Gamestate.title;
                            player.position.Y = 400;
                            player.position.X = 100;

                        }
                    }
                    else
                        deathScreenColors[1] = Color.Black;
                    oldKb = kb;
                    break;
            }

            oldMouse = mouse;
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
            switch (currentState)
            {
                case Gamestate.title:
                    spriteBatch.DrawString(titleTextFont, "SINKHOLE SPRINTER", new Vector2(GraphicsDevice.Viewport.Width / 2  - (titleTextFont.MeasureString("SINKHOLE SPRINTER").Length() / 2), 50), Color.Black);
                    spriteBatch.DrawString(titleFont, "single player", new Vector2(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("single player").Length() / 2), 200), titleScreenColors[0]);
                    spriteBatch.DrawString(titleFont, "multiplayer", new Vector2(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("multiplayer").Length() / 2), 300), titleScreenColors[1]);
                    spriteBatch.DrawString(titleFont, "high scores", new Vector2(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("high scores").Length() / 2), 400), titleScreenColors[2]);




                    break;
                case Gamestate.play:
                    foreach (Platform platform in platforms)
                    {
                        camera.Draw(gameTime, spriteBatch, platform);
                    }
                    camera.DrawPlayer(gameTime, spriteBatch, player);
                    break;
                case Gamestate.gameover:
                    spriteBatch.DrawString(titleTextFont, "you died", new Vector2(GraphicsDevice.Viewport.Width / 2 - (titleTextFont.MeasureString("you died").Length() / 2), 50), Color.DarkRed);
                    spriteBatch.DrawString(titleFont, "play again", new Vector2(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("play again").Length() / 2), 250), deathScreenColors[0]);
                    spriteBatch.DrawString(titleFont, "main menu", new Vector2(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("main menu").Length() / 2), 350), deathScreenColors[1]);
                    spriteBatch.DrawString(titleFont, "press r to play again", new Vector2(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("press r to play again").Length() / 2), 450), Color.Gold);

                    break;
            }
            
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

