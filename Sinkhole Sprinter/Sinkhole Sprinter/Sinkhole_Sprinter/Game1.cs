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
        Texture2D run;
        Texture2D jump;
        Texture2D idle;
        List<Rectangle> running, jumping, standing;
        List<Texture2D> textures;
        Player player;
        Camera camera;
        enum Gamestate
        {
            title, play, gameover, highscores
        }
        Gamestate currentState;
        SpriteFont titleFont, titleTextFont, scoreFont, testFont;
        public Color titleColor = Color.Black;
        Rectangle titleRect, multiplayerTextRect;
        MouseState mouse, oldMouse;
        KeyboardState oldKb;
        Color[] titleScreenColors = { Color.Black, Color.Black, Color.Black };
        Color[] deathScreenColors = { Color.Black, Color.Black, Color.Black };
        Color[] highScoreColors = { Color.Black, Color.Black, Color.Black };


        string[] titleScreenText = { "single player", "multiplayer", "highscores" };
        string[] gameoverScreenText = { "play again", "main menu" };
        Rectangle[] deathScreenText, mainScreenText, highScoreTxtRect;

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
        const int PLATFORM_MIN_HEIGHT = 100;
        // The portion of max jump distance not required
        const float PLATFORM_MIN_WIGGLE_ROOM = .1f;
        // Less portion of max jump distance required, decreases with difficulty
        const float PLATFORM_BONUS_WIGGLE_ROOM = .2f;
        const double PLATFORM_BREAKING_CHANCE = .3;
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

        // lava and fire
        Texture2D Lava, firesheet, exclamation;
        Rectangle lavaSize;
        float lavaHeight;
        const float LAVA_RISE_SPEED = .4f;
        List<Lava> lavas;
        const int LAVA_HEIGHT_SHOWN = 200;

        // rockWall
        Texture2D RockWall;
        RockWall rockWall;
        Rectangle rockWallRect;
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
            standing = new List<Rectangle>();
            textures = new List<Texture2D>();
            running.Add(new Rectangle(0, 0, 400, 400));
            running.Add(new Rectangle(400, 0, 400, 400));
            running.Add(new Rectangle(800, 0, 400, 400));
            running.Add(new Rectangle(1200, 0, 400, 400));
            jumping.Add(new Rectangle(0, 300, 300, 300));
            jumping.Add(new Rectangle(300, 300, 300, 300));
            jumping.Add(new Rectangle(600, 300, 300, 300));
            jumping.Add(new Rectangle(900, 300, 300, 300));
            jumping.Add(new Rectangle(1200, 200, 300, 400));
            jumping.Add(new Rectangle(1500, 200, 300, 400));
            standing.Add(new Rectangle(0, 0, 300, 300));
            standing.Add(new Rectangle(300, 0, 300, 300));
            standing.Add(new Rectangle(600, 0, 300, 300));
            standing.Add(new Rectangle(900, 0, 300, 300));
            IsMouseVisible = true;
            currentState = Gamestate.title;
            platforms = new List<Platform>();
            lavas = new List<Lava>();
            lavaSize = new Rectangle(0, 0, 1500, 300);
            oldKb = Keyboard.GetState();
            highScores = new List<int>();
            rockWallRect = new Rectangle(-1000, 360, 700, 720);


            highScoreTxtRect= mainScreenText = deathScreenText = new Rectangle[5];
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
            run = this.Content.Load<Texture2D>("player_running_spritesheet_25");
            jump = this.Content.Load<Texture2D>("jumping spritesheet 2");
            idle = this.Content.Load<Texture2D>("idle spritesheet");
            textures = new List<Texture2D>();
            textures.Add(run);
            textures.Add(jump);
            textures.Add(idle);

            titleTextFont = Content.Load<SpriteFont>("SpriteFont2");
            titleFont = Content.Load<SpriteFont>("SpriteFont1");
            scoreFont = Content.Load<SpriteFont>("ScoreFont");
            // rectangles to highlight text in update method
            titleRect = new Rectangle((int)(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString(titleScreenText[0]).Length() / 2)), 200, 30, 30);
            multiplayerTextRect = new Rectangle((int)(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString(titleScreenText[1]).Length() / 2)), 300, 30, 30);
            mainScreenText[2] = new Rectangle((int)(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString(titleScreenText[2]).Length() / 2)), 400, 30, 30);
            deathScreenText[0] = new Rectangle((int)(GraphicsDevice.Viewport.Width / 4 - (titleFont.MeasureString(gameoverScreenText[0]).Length() / 2)), 350, 30, 30); // rectangle to highlight text "play again"
            deathScreenText[1] = new Rectangle((int)(GraphicsDevice.Viewport.Width / 4 + (GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString(gameoverScreenText[1]).Length() / 2))), 350, 30, 30); // rectangle to highlight text "main menu"
            highScoreTxtRect[0] = new Rectangle((int)(GraphicsDevice.Viewport.Width / 4 - (titleFont.MeasureString("main menu").Length() / 2)), GraphicsDevice.Viewport.Height / 3, 30, 30);
            testFont = Content.Load<SpriteFont>("SpriteFont3");

            placeholder = this.Content.Load<Texture2D>("white");
            platform = this.Content.Load<Texture2D>("platform");

            firesheet = this.Content.Load<Texture2D>("Fire");
            Lava = this.Content.Load<Texture2D>("Lava");
            
            exclamation = this.Content.Load<Texture2D>("exclamation");

            RockWall = this.Content.Load<Texture2D>("white");
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
                    if (mouse.X > mainScreenText[2].X && mouse.X < mainScreenText[2].X + (("highscores".Length - 1) * 20) && mouse.Y > mainScreenText[2].Y + 10 && mouse.Y < mainScreenText[2].Y + mainScreenText[2].Height)
                    {
                        titleScreenColors[2] = Color.Gold;
                        if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                        {
                            currentState = Gamestate.highscores;
                        }
                    }
                    else
                        titleScreenColors[2] = Color.Black;

                    break;

                case Gamestate.play:
                    for (int x = 0; x < platforms.Count; x++)
                    {
                        //platforms[x].update(PLATFORM_SPEED);
                        platforms[x].Update();
                        if (platforms[x].touchedTimer == 0)
                        {
                            platforms.RemoveAt(x);
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
                    rockWall.Update();

                    //ensures rockwall maintains minimum distance from player
                    if (rockWall.position.X<player.position.X-950 && timer>300)
                        rockWall.position.X= MathHelper.Lerp(rockWall.position.X, player.position.X - 950, .02f);

                    lavaHeight -= LAVA_RISE_SPEED;

                    //ensures lava maintains minimum distance from player
                    if (lavaHeight > player.position.Y + 375 && timer>300)
                        lavaHeight = Math.Max(MathHelper.Lerp(lavaHeight, player.position.Y + 375, 0.02f), lavaHeight - LAVA_RISE_SPEED * 4); // Capped at additional 4x lava speed

                    tileLava();
                    foreach (Lava lava in lavas)
                    {
                        lava.position.Y = lavaHeight + lavaSize.Height / 2;
                    }
                    if (LastPlatform.position.X < camera.boundingRectangle.Right)
                    {
                        createPlatform();
                    }
                    if (platforms[0].Top - PLATFORM_MIN_HEIGHT > lavaHeight)
                        platforms.RemoveAt(0);

                    camera.position.X = Math.Max(player.position.X, camera.boundingRectangle.Width / 2);
                    camera.position.Y = Math.Min(Math.Min(player.position.Y, camera.boundingRectangle.Height / 2), lavas[0].Top + LAVA_HEIGHT_SHOWN - camera.boundingRectangle.Height / 2);
                    if (player.Bottom >= lavas[0].Top) // checks if player is dead
                    {
                        onDeath();
                    }


                    //rockWall 
                    rockWall.position.Y = camera.position.Y;
                    if (player.position.X <= rockWall.Right) // checks if player is dead
                    {
                        onDeath();
                    }
                    // Update stats
                    maxHeight = Math.Max(STARTING_PLATFORM_HEIGHT - Platform.HEIGHT / 2 - player.Bottom, maxHeight);
                    distance = Math.Max((int)player.position.X, distance);
                    if (timer % 60 == 0)
                        points += Math.Min(10 + timer / 600, 100);
                    score = points + maxHeight + distance / 10;

                    timer++;
                    break;
                    
                case Gamestate.gameover:
                    if (kb.IsKeyDown(Keys.R) && !oldKb.IsKeyDown(Keys.R))
                    {
                        startGame();
                    }
                    if (mouse.X > deathScreenText[0].X &&
                        mouse.X < deathScreenText[0].X + ((gameoverScreenText[0].Length - 1) * 20) && mouse.Y > deathScreenText[0].Y + 10 &&    mouse.Y < deathScreenText[0].Y + deathScreenText[0].Height)
                    {
                        deathScreenColors[0] = Color.Gold;
                        if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                        {
                            startGame();
                        }
                    }
                    else
                        deathScreenColors[0] = Color.Black;
                        
                    if (changeColors(mouse, "main menu", deathScreenText[1]))
                    {
                        deathScreenColors[1] = Color.Gold;
                        if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                        {
                            currentState = Gamestate.title;
                        }
                    }
                    else
                        deathScreenColors[1] = Color.Black;
                    //if (mouse.X > deathScreenText[1].X && mouse.X < deathScreenText[1].X + (("main menu".Length - 1) * 20) && mouse.Y > deathScreenText[1].Y + 10 && mouse.Y < deathScreenText[1].Y + deathScreenText[1].Height)
                    //{
                    //    deathScreenColors[1] = Color.Gold;
                    //    if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                    //    {
                    //        currentState = Gamestate.title;
                    //    }
                    //}
                    //else
                    //    deathScreenColors[1] = Color.Black;
                    
                    break;
                case Gamestate.highscores:
                    if (changeColors(mouse, "main menu", highScoreTxtRect[0]))
                    {
                        highScoreColors[0] = Color.Gold;
                        if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                        {
                            currentState = Gamestate.title;
                        }
                    }
                    else
                        highScoreColors[0] = Color.Black;
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
            createPlatform(new Vector2(Platform.MAX_WIDTH / 2, camera.boundingRectangle.Height * .7f), Platform.MAX_WIDTH, false);
            player = new Player(new Rectangle(50, STARTING_PLATFORM_HEIGHT - Platform.HEIGHT / 2 - 38, 75, 75), textures, running, jumping, standing);
            lavas.Clear();
            lavaHeight = GraphicsDevice.Viewport.Height;
            lavas.Add(new Lava(new Rectangle(lavaSize.Width / 2, (int)lavaHeight + lavaSize.Height / 2, lavaSize.Width, lavaSize.Height), Lava));
            camera.position.X = Math.Max(player.position.X, camera.boundingRectangle.Width / 2);
            camera.position.Y = Math.Min(Math.Min(player.position.Y, camera.boundingRectangle.Height / 2), lavas[0].Top + LAVA_HEIGHT_SHOWN - camera.boundingRectangle.Height / 2);
            rockWall = new RockWall(rockWallRect, RockWall);
            maxHeight = 0;
            distance = 0;
            points = 0;
            timer = 0;
        }

        private void tileLava()
        {
            if (lavas[0].Right + lavaSize.Width < camera.boundingRectangle.Left)
                lavas.RemoveAt(0);
            else
                lavas.Insert(0, new Lava(new Rectangle((int)lavas[0].position.X + lavaSize.Width, (int)lavas[0].position.Y, lavaSize.Width, lavaSize.Height), Lava));

            if (lavas[lavas.Count - 1].Left - lavaSize.Width > camera.boundingRectangle.Right)
                lavas.RemoveAt(lavas.Count - 1);
            else
                lavas.Add(new Lava(new Rectangle((int)lavas[0].position.X + lavaSize.Width, (int)lavas[0].position.Y, lavaSize.Width, lavaSize.Height), Lava));
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
            position.Y = Math.Min(LastPlatform.position.Y + dHeight, lavas[0].Top - PLATFORM_MIN_HEIGHT);

            // Platform width calculations
            int width = (int)(Platform.MIN_WIDTH + (Platform.MAX_WIDTH - Platform.MIN_WIDTH) * Math.Pow(.5, LastPlatform.position.X / PLATFORM_DIFFICULTY_DISTANCE));

            // Fraction of the max jump distance based on difficulty (max distance)
            float reverseDistanceModifier = (float)(PLATFORM_MIN_WIGGLE_ROOM + PLATFORM_BONUS_WIGGLE_ROOM * Math.Pow(.5, LastPlatform.position.X / PLATFORM_DIFFICULTY_DISTANCE));
            float distanceModifier = 1 - (float)(r.NextDouble() * reverseDistanceModifier + reverseDistanceModifier);
            float dDistance = distanceModifier * (player.GetMaxJumpDistance(dHeight) + width);
            position.X = Math.Max(LastPlatform.position.X + dDistance, LastPlatform.position.X + width * 2);

            bool isBreaking = r.NextDouble() < PLATFORM_BREAKING_CHANCE;

            createPlatform(position, width, isBreaking);
        }
        private void createPlatform(Vector2 position, int width, bool isBreaking)
        {
            platforms.Add(new Platform(new Rectangle((int)position.X, (int)position.Y, width, Platform.HEIGHT), platform, isBreaking));
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
                    spriteBatch.DrawString(titleFont, titleScreenText[0], new Vector2(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString(titleScreenText[0]).Length() / 2), 200), titleScreenColors[0]);
                    spriteBatch.DrawString(titleFont, titleScreenText[1], new Vector2(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString(titleScreenText[1]).Length() / 2), 300), titleScreenColors[1]);
                    spriteBatch.DrawString(titleFont, "high scores", new Vector2(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("high scores").Length() / 2), 400), titleScreenColors[2]);




                    break;
                case Gamestate.play:
                    // spriteBatch.Draw(placeholder, new Rectangle(0, lavas[0].rect.Bottom - 5, 1500, Math.Max(GraphicsDevice.Viewport.Height - lavas[0].rect.Bottom + 5, 0)), new Color(255, 79, 9));
                    foreach (Platform platform in platforms)
                    {
                        camera.Draw(gameTime, spriteBatch, platform);
                    }
                    camera.DrawPlayer(gameTime, spriteBatch, player);
                    camera.Draw(gameTime, spriteBatch, rockWall);
                    foreach (Lava lava in lavas)
                        camera.Draw(gameTime, spriteBatch, lava);
                    spriteBatch.Draw(placeholder, new Rectangle(00, 0, GraphicsDevice.Viewport.Width, 25), Color.Black);
                    spriteBatch.DrawString(scoreFont, "score: " + score, new Vector2(0, 00), Color.White); // points distance max height
                    spriteBatch.DrawString(scoreFont, "points: " + points, new Vector2(GraphicsDevice.Viewport.Width / 2 -  (scoreFont.MeasureString("points: " + points).Length() / 2), 00), Color.White);
                    spriteBatch.DrawString(scoreFont, "height: " + maxHeight, new Vector2(1280 - (scoreFont.MeasureString("height: " + maxHeight).Length()), 00), Color.White);
                    float heightTextX = 1280 - (scoreFont.MeasureString("height: " + maxHeight).Length());
                    float pointsTextX = (GraphicsDevice.Viewport.Width / 2 - (scoreFont.MeasureString("points: " + points).Length() / 2)); // get avg distance between both

                    spriteBatch.DrawString(scoreFont, "distance: " + distance, new Vector2((1280 - (scoreFont.MeasureString("height: " + maxHeight).Length()) + GraphicsDevice.Viewport.Width / 2 - (scoreFont.MeasureString("points: " + points).Length() / 2)) / 2,0), Color.White);

                    break;
                    
                case Gamestate.gameover:
                    spriteBatch.DrawString(titleTextFont, "you died", new Vector2(GraphicsDevice.Viewport.Width / 2 - (titleTextFont.MeasureString("you died").Length() / 2), 50), Color.DarkRed);
                    spriteBatch.DrawString(titleFont, "play again", new Vector2(GraphicsDevice.Viewport.Width / 4 - (titleFont.MeasureString("play again").Length() / 2), 350), deathScreenColors[0]); //1280
                    spriteBatch.DrawString(titleFont, "main menu", new Vector2(GraphicsDevice.Viewport.Width / 2 + GraphicsDevice.Viewport.Width / 4 - (titleFont.MeasureString("main menu").Length() / 2), 350), deathScreenColors[1]);
                    spriteBatch.DrawString(titleFont, "press r to play again", new Vector2(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("press r to play again").Length() / 2), 400), Color.Gold);

                    spriteBatch.DrawString(scoreFont, "final score: " + score, new Vector2(GraphicsDevice.Viewport.Width / 2 - (scoreFont.MeasureString("final score: " + score).Length() /2), 150), Color.Black);
                    spriteBatch.DrawString(scoreFont, "final distance: " + distance, new Vector2(GraphicsDevice.Viewport.Width / 2 - (scoreFont.MeasureString("final distance: " + distance).Length() / 2), 200), Color.Black);
                    spriteBatch.DrawString(scoreFont, "final height: " + maxHeight, new Vector2(GraphicsDevice.Viewport.Width / 2 - (scoreFont.MeasureString("final height: " + maxHeight).Length() / 2), 250), Color.Black);
                    break;
                case Gamestate.highscores:
                    spriteBatch.DrawString(titleFont, "main menu", new Vector2(GraphicsDevice.Viewport.Width / 4 - (titleFont.MeasureString("main menu").Length() / 2), GraphicsDevice.Viewport.Height / 3), highScoreColors[0]);
                    spriteBatch.DrawString(titleTextFont, "highscores", new Vector2(GraphicsDevice.Viewport.Width / 2 - (titleTextFont.MeasureString("highscores").Length() / 2), 0), Color.Black);
                    drawScores(spriteBatch, gameTime, highScores.Count);




                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        private bool changeColors(MouseState mouse, string text, Rectangle txtRect) //change colors of text when the mouse hovers over it
        {
            if (mouse.X > txtRect.X &&
                mouse.X < txtRect.X + ((text.Length - 1) * 20) &&
                mouse.Y > txtRect.Y + 10 &&
                mouse.Y < txtRect.Y + txtRect.Height)
            {
                return true;
            }
            return false;
        }


        private void drawScores(SpriteBatch spriteBatch, GameTime gameTime, int scoreNum)
        {
            float posX = 75;
            switch (scoreNum)
            {
                default:
                    if (scoreNum >= 10)
                    {
                        goto case 3;
                    }
                    spriteBatch.DrawString(scoreFont, "1. ", new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("1. ").Length(), posX), Color.Gold);
                    spriteBatch.DrawString(scoreFont, "2. ", new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("2. ").Length(), posX + 50), Color.Silver);
                    spriteBatch.DrawString(scoreFont, "3. ", new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("3. ").Length(), posX + 100), Color.Brown);
                    int x = (int)(posX + 150);
                    for (int i = 4; i < 11; i++)
                    {
                        spriteBatch.DrawString(scoreFont, i + ". ", new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString(i + ". ").Length(), x), Color.Black);
                        x += 50;

                    }
                    break;
                case 1:
                    spriteBatch.DrawString(scoreFont, "1. " + highScores[0], new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("1. " + highScores[0]).Length() / 2, posX), Color.Gold);
                    spriteBatch.DrawString(scoreFont, "2. ", new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("2. ").Length() / 2, posX + 50), Color.Silver) ;
                    spriteBatch.DrawString(scoreFont, "3. ", new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("3. ").Length() / 2, posX + 100), Color.Brown);
                    break;
                case 2:
                    spriteBatch.DrawString(scoreFont, "1. " + highScores[0], new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("1. " + highScores[0]).Length() / 2, posX), Color.Gold);
                    spriteBatch.DrawString(scoreFont, "2. " + highScores[1], new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("2. " + highScores[1]).Length() / 2, posX + 50), Color.Silver);
                    spriteBatch.DrawString(scoreFont, "3. ", new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("3. ").Length() / 2, posX + 100), Color.Brown);
                    break;
                case 3:

                    spriteBatch.DrawString(scoreFont, "1. " + highScores[0], new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("1. " + highScores[0]).Length() / 2, posX), Color.Gold);
                    spriteBatch.DrawString(scoreFont, "2. " + highScores[1], new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("2. " + highScores[1]).Length() / 2, posX + 50), Color.Silver);
                    spriteBatch.DrawString(scoreFont, "3. " + highScores[2], new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("3. " + highScores[2]).Length() / 2, posX + 100), Color.Brown);
                    break;
                case 4:
                    spriteBatch.DrawString(scoreFont, "4. " + highScores[3], new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("4. " + highScores[3]).Length() / 2, posX + 150), Color.Black);
                    goto case 3;
                case 5:
                    spriteBatch.DrawString(scoreFont, "5. " + highScores[4], new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("5. " + highScores[4]).Length() / 2, posX + 200), Color.Black);
                    goto case 4;
                case 6:
                    spriteBatch.DrawString(scoreFont, "6. " + highScores[5], new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("6. " + highScores[5]).Length()/2, posX + 250), Color.Black);
                    goto case 5;
                case 7:
                    spriteBatch.DrawString(scoreFont, "7. " + highScores[6], new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("7. " + highScores[6]).Length()/2, posX + 300), Color.Black);
                    goto case 6;
                case 8:
                    spriteBatch.DrawString(scoreFont, "8. " + highScores[7], new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("8. " + highScores[7]).Length()/2, posX + 350), Color.Black);
                    goto case 7;
                case 9:
                    spriteBatch.DrawString(scoreFont, "9. " + highScores[8], new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("9. " + highScores[8]).Length()/2, posX + 400), Color.Black);
                    goto case 8;
                case 10:
                    spriteBatch.DrawString(scoreFont, "10. " + highScores[9], new Vector2(GraphicsDevice.Viewport.Width / 2 - scoreFont.MeasureString("10. " + highScores[9]).Length()/2, posX + 450), Color.Black);
                    goto case 9;


            }
        }
    }
}

