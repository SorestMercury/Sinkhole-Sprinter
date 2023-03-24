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
        Gamestate currentState;
        SpriteFont titleFont, titleTextFont, testFont;
        public Color titleColor = Color.Black;
        Rectangle titleRect, multiplayerTextRect;
        MouseState mouse, oldMouse;
        KeyboardState oldKb;
        Color[] titleScreenColors = { Color.Black, Color.Black, Color.Black };
        Color[] deathScreenColors = { Color.Black, Color.Black, Color.Black };

        string[] titleScreenText = { "single player", "multiplayer" };
        string[] gameoverScreenText = { "play again", "main menu" };
        Rectangle[] deathScreenText;

        //const int PLATFORM_SPEED = 3;
        const int STARTING_PLATFORM_HEIGHT = 500;
        // How much the platform can spawn up or down
        const int PLATFORM_HEIGHT_VARIANCE = 100;
        // Minimum average increase in height
        const int PLATFORM_HEIGHT_GAIN = 10;
        // Bonus height increase that lowers with increasing difficulty
        const int PLATFORM_EXTRA_HEIGHT_GAIN = 40;
        // How far to travel to half the bonus height gain and jump wiggle room
        const int PLATFORM_DIFFICULTY_DISTANCE = 20000;
        // The minimum height the platform can spawn at above the lava
        const int PLATFORM_MIN_HEIGHT = 30;
        // The portion of max jump distance not required
        const float PLATFORM_MIN_WIGGLE_ROOM = .1f;
        // Less portion of max jump distance required, decreases with difficulty
        const float PLATFORM_BONUS_WIGGLE_ROOM = .4f;
        List<Platform> platforms;
        Platform LastPlatform
        {
            get => platforms[platforms.Count - 1];
        }

        // Stats
        int maxHeight; // Highest height (flipped to positive, greater is higher)
        int points; // Gained based on time, used to by items
        int distance; // Furthest distance
        int score; // Calculated based on previous stats
        List<int> highScores;

        Texture2D placeholder;
        Texture2D platform;
        // Time survived
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
            currentState = Gamestate.title;
            platforms = new List<Platform>();
            oldKb = Keyboard.GetState();
            highScores = new List<int>();

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

            testFont = Content.Load<SpriteFont>("SpriteFont3");

            placeholder = this.Content.Load<Texture2D>("white");
            platform = this.Content.Load<Texture2D>("platform");
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
            KeyboardState kb = Keyboard.GetState();
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
                            startGame();
                        }
                    }
                    else
                        titleScreenColors[0] = Color.Black;
                    if (mouse.X > multiplayerTextRect.X && mouse.X < multiplayerTextRect.X + (("multiplayer".Length - 1) * 20) && mouse.Y > multiplayerTextRect.Y + 10 && mouse.Y < multiplayerTextRect.Y + multiplayerTextRect.Height)
                    {
                        titleScreenColors[1] = Color.Gold;
                        if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                        {
                            startGame();
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
                        onDeath();
                    }
                    camera.position.Y = Math.Min(player.position.Y, camera.boundingRectangle.Height / 2); // Add lava to minimum

                    // Update stats
                    maxHeight = Math.Max(STARTING_PLATFORM_HEIGHT - Platform.HEIGHT / 2 - player.Bottom, maxHeight);
                    distance = Math.Max((int)player.position.X, distance);
                    if (timer % 60 == 0)
                        points += Math.Min(10 + timer / 600, 100);
                    score = points + timer / 12 + maxHeight + distance / 20;

                    timer++;
                    break;
                    
                case Gamestate.gameover:
                    if (kb.IsKeyDown(Keys.R) && !oldKb.IsKeyDown(Keys.R))
                    {
                        currentState = Gamestate.play;
                        startGame();
                    }
                    if (mouse.X > deathScreenText[0].X &&
                        mouse.X < deathScreenText[0].X + (("play again".Length - 1) * 20) && mouse.Y > deathScreenText[0].Y + 10 &&    mouse.Y < deathScreenText[0].Y + deathScreenText[0].Height)
                    {
                        deathScreenColors[0] = Color.Gold;
                        if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                        {
                            currentState = Gamestate.title;
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
                        }
                    }
                    else
                        deathScreenColors[1] = Color.Black;
                    
                    break;
            }

            oldMouse = mouse;
            oldKb = kb;
            base.Update(gameTime);
        }

        // Resets stats and starts the game (multiplayer parameter in future)
        private void startGame()
        {
            currentState = Gamestate.play;
            createPlatform(new Vector2(Platform.WIDTH / 2, STARTING_PLATFORM_HEIGHT));
            player = new Player(new Rectangle(50, STARTING_PLATFORM_HEIGHT - Platform.HEIGHT / 2 - 38, 75, 75), spreadsheet, running, jumping, new Rectangle(0, 0, 400, 400));
            maxHeight = 0;
            distance = 0;
            points = 0;
            timer = 0;
        }

        // Ran when the player dies
        private void onDeath()
        {
            currentState = Gamestate.gameover;
            highScores.Add(score);
            highScores.Sort();
            highScores.Reverse();
            platforms.Clear();
        }

        // Create a new platform in calculated position
        private void createPlatform()
        {
            // Average height gain based on difficulty (max distance)
            int avgGain = PLATFORM_HEIGHT_GAIN + (int)(PLATFORM_EXTRA_HEIGHT_GAIN * Math.Pow(.5, LastPlatform.position.X / PLATFORM_DIFFICULTY_DISTANCE));
            int dHeight = r.Next(-(avgGain + PLATFORM_HEIGHT_VARIANCE), -(avgGain - PLATFORM_HEIGHT_VARIANCE));
            Vector2 position = new Vector2();
            // TODO: Change viewport height to max lava height + PLATFORM_MIN_HEIGHT
            position.Y = Math.Min(LastPlatform.position.Y + dHeight, STARTING_PLATFORM_HEIGHT);

            // Fraction of the max jump distance based on difficulty (max distance)
            float reverseDistanceModifier = (float)(PLATFORM_MIN_WIGGLE_ROOM + PLATFORM_BONUS_WIGGLE_ROOM * Math.Pow(.5, LastPlatform.position.X / PLATFORM_DIFFICULTY_DISTANCE));
            float distanceModifier = 1 - (float)(r.NextDouble() * reverseDistanceModifier / 2 + reverseDistanceModifier);
            position.X = Math.Max(LastPlatform.position.X + distanceModifier * player.GetMaxJumpDistance(dHeight), LastPlatform.position.X + Platform.WIDTH * 2);
            createPlatform(position);
        }
        private void createPlatform(Vector2 position)
        {
            platforms.Add(new Platform(new Rectangle((int)position.X, (int)position.Y, Platform.WIDTH, Platform.HEIGHT), platform));
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

