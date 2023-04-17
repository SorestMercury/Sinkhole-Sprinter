using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sinkhole_Sprinter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        // Player
        Texture2D run;
        Texture2D jump;
        Texture2D idle;
        Texture2D hearts;
        List<Rectangle> running, jumping, standing;
        List<Texture2D> textures;
        Player player;

        // Info
        Camera camera;
        MouseState mouse, oldMouse;
        KeyboardState oldKb;
        enum Gamestate
        {
            title, play, gameover, highscores
        }
        Gamestate currentState;

        // UI
        SpriteFont titleFont, titleTextFont, scoreFont, testFont;
        Color titleColor = Color.Black;
        Rectangle titleRect, multiplayerTextRect;
        Color[] titleScreenColors = { Color.Black, Color.Black, Color.Black };
        Color[] deathScreenColors = { Color.Black, Color.Black, Color.Black };
        Color[] highScoreColors = { Color.Black, Color.Black, Color.Black};
        Color[] highScoreCols = { Color.Gold, Color.Silver, Color.Brown, Color.Black };
        Rectangle[] deathScreenText, highScoreTxtRect, mainScreenText;
        Vector2 leaderboardPos;

        // Platforms
        const int STARTING_PLATFORM_HEIGHT = 500;       // Height of first platform
        const int PLATFORM_HEIGHT_VARIANCE = 100;       // Randomness to platform height
        const int PLATFORM_HEIGHT_GAIN = 10;            // Minimum average height gain
        const int PLATFORM_EXTRA_HEIGHT_GAIN = 40;      // Additional height gain, lowers with time
        const int PLATFORM_DIFFICULTY_DISTANCE = 20000; // Distance to half bonus gains
        const int PLATFORM_MIN_HEIGHT = 100;            // Minimum height above lava
        const float PLATFORM_BONUS_WIGGLE_ROOM = .3f;   // Randomness to platform distance at inf distance
        const float PLATFORM_AVERAGE_DIFFICULTY = .6f;  // Average difficulty, based on max jump distance
        const int PLATFORM_WIDTH_VARIANCE = 30;         // Randomness to platform width
        const double PLATFORM_BREAKING_CHANCE = .3;     // Chance for platform to be a breaking platform
        const double PLATFORM_GOLDEN_CHANCE = .05;     // Chance for platform to be a breaking platform

        Texture2D platform;
        Texture2D platformWeak;
        Texture2D goldenPlatform;
        Texture2D placeholder;
        List<Platform> platforms;
        Platform LastPlatform
        {
            get => platforms[platforms.Count - 1];
        }

        // Stats
        string fileName = "..\\..\\..\\..\\Sinkhole_SprinterContent/scores.txt";
        int maxHeight;  // Highest height (flipped to positive, greater is higher)
        int points;     // Gained based on time, used to by items
        int distance;   // Furthest distance
        int score;      // Calculated based on previous stats
        List<int> highScores;

        // Time survived
        int timer = 0;
        Random r = new Random();

        // Lava and Fire
        const float LAVA_RISE_SPEED = .4f;
        const int LAVA_HEIGHT_SHOWN = 200;
        Texture2D lavaTexture, firesheet, exclamation;
        Rectangle lavaSize; // Base lava sprite rect
        float lavaHeight;   // Top of lava sprite
        List<Lava> lavas;
        ExclaimFire fireExclaim;
        Fire fire;
        // RockWall
        const int ROCK_SIZE = 40;
        RockWall rockWall;
        Rectangle rockWallRect;
        List<Texture2D> rocks;
        Rock tempRock;
        Rock[] rockArray = new Rock[200];

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
            // Main Info
            IsMouseVisible = true;
            currentState = Gamestate.title;
            camera = new Camera(new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
            oldKb = Keyboard.GetState();

            // Player Sprites
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

            // Other Sprites
            platforms = new List<Platform>();
            lavas = new List<Lava>();
            lavaSize = new Rectangle(0, 0, 1500, 300);
            rockWallRect = new Rectangle(-800, 360, 700, 720);
            rocks = new List<Texture2D>();

            // High Score
            LoadScores();
            highScoreTxtRect = mainScreenText = deathScreenText = new Rectangle[5];
            leaderboardPos = new Vector2(600, 100);
            base.Initialize();
        }

        private void LoadScores()
        {
            highScores = new List<int>();
            try
            {
                if (!File.Exists(fileName))
                {
                    Console.WriteLine("Score data doesn't exist");
                    return;
                }
                using (StreamReader reader = new StreamReader(fileName))
                {
                    while (!reader.EndOfStream)
                    {
                        int score = int.Parse(reader.ReadLine());
                        highScores.Add(score);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            highScores.Sort();
            highScores.Reverse();
        }

        private void SaveScores()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    foreach (int score in highScores)
                    {
                        writer.WriteLine(score);
                    }
                    Console.WriteLine("Written");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Player Sprite Sheets
            run = this.Content.Load<Texture2D>("player_running_spritesheet_25");
            jump = this.Content.Load<Texture2D>("jumping spritesheet 2");
            idle = this.Content.Load<Texture2D>("idle spritesheet");
            textures = new List<Texture2D>();
            textures.Add(run);
            textures.Add(jump);
            textures.Add(idle);
            hearts = Content.Load<Texture2D>("hearts");

            // Fonts
            titleTextFont = Content.Load<SpriteFont>("SpriteFont2");
            titleFont = Content.Load<SpriteFont>("SpriteFont1");
            scoreFont = Content.Load<SpriteFont>("ScoreFont");
            // rectangles to highlight text in update method
            mainScreenText[0]=titleRect = new Rectangle((int)(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("singleplayer").Length() / 2)), 200, 30, 30);
            mainScreenText[1]=multiplayerTextRect = new Rectangle((int)(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("multiplayer").Length() / 2)), 300, 30, 30);
            mainScreenText[2] = new Rectangle((int)(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("highscores").Length() / 2)), 400, 30, 30);
            //
            deathScreenText[0] = new Rectangle((int)(GraphicsDevice.Viewport.Width / 4 - (titleFont.MeasureString("play again").Length() / 2)), 350, 30, 30);
            deathScreenText[1] = new Rectangle((int)(GraphicsDevice.Viewport.Width / 4 + (GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("main menu").Length() / 2))), 350, 30, 30); 
            //
            highScoreTxtRect[0] = new Rectangle((int)(GraphicsDevice.Viewport.Width / 4 - (titleFont.MeasureString("main menu").Length() / 2)), GraphicsDevice.Viewport.Height / 3, 30, 30);
            testFont = Content.Load<SpriteFont>("SpriteFont3");

            // Other sprites
            placeholder = this.Content.Load<Texture2D>("white");
            goldenPlatform = this.Content.Load<Texture2D>("ylwPlatform");
            platform = this.Content.Load<Texture2D>("platform");
            platformWeak = this.Content.Load<Texture2D>("platformWeak");

            firesheet = this.Content.Load<Texture2D>("Fire");
            lavaTexture = this.Content.Load<Texture2D>("Lava");
            exclamation = this.Content.Load<Texture2D>("exclamation");
           


            // Rocks
            rocks.Add(this.Content.Load<Texture2D>("rock1"));
            rocks.Add(this.Content.Load<Texture2D>("rock2"));
            rocks.Add(this.Content.Load<Texture2D>("rock3"));
            rocks.Add(this.Content.Load<Texture2D>("rock4"));
            
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
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            switch (currentState)
            {
                case Gamestate.title:
                    // Start singleplayer
                    if (changeColors(mouse, "singleplayer", titleRect))
                    {
                        titleScreenColors[0] = Color.Gold;
                        if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                        {
                            startGame();
                        }
                    }
                    else
                        titleScreenColors[0] = Color.Black;

                    // Start multiplayer
                    if (changeColors(mouse, "multiplayer", multiplayerTextRect))
                    {
                        titleScreenColors[1] = Color.Gold;
                        if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                        {
                            startGame();
                        }
                    }
                    else
                        titleScreenColors[1] = Color.Black;

                    // Open highscore menu
                    if (changeColors(mouse, "highscores", mainScreenText[2]))
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
                    // Remove broken platforms and check collisions
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
                        {
                            if (platforms[x].texture.Equals(goldenPlatform) && platforms[x].isBreaking)
                            {
                                platforms[x].isBreaking = false;
                                points += 25;
                            }
                            player.CheckCollisions(platforms[x]);
                        }
                    }

                    //if (timer % 60 == 0)
                    //{
                    //    platforms.Add(new Platform(new Rectangle(1280, platforms[platforms.Count-1].rect.Y+r.Next(-150,50), 70, 10), placeholder));
                    //}

                    // Update positions
                    player.Update();
                    camera.Update();
                    rockWall.Update();
                    lavaHeight -= LAVA_RISE_SPEED;

                    //ensures lava maintains minimum distance from player
                    if (lavaHeight > camera.position.Y + camera.boundingRectangle.Height / 2 && timer > 300)
                        lavaHeight = Math.Max(MathHelper.Lerp(lavaHeight, player.position.Y + 375, 0.02f), lavaHeight - LAVA_RISE_SPEED * 4); // Capped at additional 4x lava speed

                    //ensures rockwall maintains minimum distance from player
                    if (rockWall.position.X < player.position.X - 950 && timer > 300)
                        rockWall.position.X = MathHelper.Lerp(rockWall.position.X, player.position.X - 950, .02f);
                    
                    // Tile lava and adjust height
                    tileLava();
                    foreach (Lava lava in lavas)
                    {
                        lava.position.Y = lavaHeight + lavaSize.Height / 2;
                    }

                    //Warning system
                    fireExclaim.Update(lavaHeight, player.Right,camera.Right);

                    //Fire
                    fire.Update(fireExclaim.pastPosition);

                    // Update camera position
                    camera.position.X = Math.Max(player.position.X, camera.boundingRectangle.Width / 2);
                    camera.FollowY(player);
                    camera.position.Y = Math.Min(Math.Min(camera.position.Y, camera.boundingRectangle.Height / 2), lavaHeight + LAVA_HEIGHT_SHOWN - camera.boundingRectangle.Height / 2);

                    // Create platforms, remove first platform if under lava
                    if (LastPlatform.position.X < camera.boundingRectangle.Right)
                    {
                        createPlatform();
                    }
                    if (platforms[0].Top - PLATFORM_MIN_HEIGHT > lavaHeight)
                        platforms.RemoveAt(0);

                    if (player.Bottom >= lavas[0].Top) // checks if player is dead
                    {
                        onDeath();
                    }

                    if (player.rect.Intersects(fire.rect))
                    {
                        onDeath();
                    }
                    //Update rockwall position
                    rockWall.position.Y = camera.position.Y;
                    if (player.position.X <= rockWall.Right) // checks if player is dead
                    {
                        onDeath();
                    }
                    for (int a = 0; a < rockArray.Length; a++)
                    {
                        rockArray[a].Update();
                    }
                    // Create new rocks if out of screen
                    for (int a = 0; a < rockArray.Length; a++)
                    {
                        if (rockArray[a] != null && rockArray[a].rect.Top > 720)
                        {
                            rockArray[a] = new Rock(new Rectangle(r.Next(rockWall.rect.Width) + rockWall.Left + 20, camera.boundingRectangle.Top, ROCK_SIZE, ROCK_SIZE), rocks[r.Next(rocks.Count)], r.Next(20)+10);
                        }
                    }

                    // Update stats
                    maxHeight = Math.Max(STARTING_PLATFORM_HEIGHT - Platform.HEIGHT / 2 - player.Bottom, maxHeight);
                    distance = Math.Max((int)player.position.X, distance);
                    if (timer % 60 == 0)
                        points += Math.Min(10 + timer / 600, 100);
                    score = 3 * points + maxHeight + distance / 10;

                    timer++;
                    break;

                case Gamestate.gameover:
                    // Check for restart keybind
                    if (kb.IsKeyDown(Keys.R) && !oldKb.IsKeyDown(Keys.R) || gamePad.Buttons.Start == ButtonState.Pressed)
                    {
                        startGame();
                    }

                    // Play Again
                    if (changeColors(mouse, "play again", new Rectangle((int)(GraphicsDevice.Viewport.Width / 4 - (titleFont.MeasureString("play again").Length() / 2)), 350, 30, 30)))
                    {
                        deathScreenColors[0] = Color.Gold;
                        if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                        {
                            startGame();
                        }
                    }
                    else
                    {
                        deathScreenColors[0] = Color.Black;
                    }
                    
                    // Go to Main Menu
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
                    break;

                case Gamestate.highscores:
                    // Return to Main Menu
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
            // Create sprites
            createPlatform(new Vector2(Platform.MAX_WIDTH / 2, camera.boundingRectangle.Height * .7f), Platform.MAX_WIDTH, false);
            player = new Player(new Rectangle(50, STARTING_PLATFORM_HEIGHT - Platform.HEIGHT / 2 - 38, 75, 75), textures, running, jumping, standing);
            
            // Create lava
            lavaHeight = GraphicsDevice.Viewport.Height;
            lavas.Clear();
            lavas.Add(new Lava(new Rectangle(lavaSize.Width / 2, (int)lavaHeight + lavaSize.Height / 2, lavaSize.Width, lavaSize.Height), lavaTexture));

            // Create rockwall
            rockWall = new RockWall(rockWallRect, null);
            for (int a = 0; a < rockArray.Length; a++)
            {
                tempRock = new Rock(new Rectangle(r.Next(rockWallRect.Width) + rockWallRect.Left, 0, ROCK_SIZE, ROCK_SIZE), rocks[r.Next(rocks.Count)], r.Next(20) + 10);
                rockArray[a] = tempRock;
            }

            //create fire
            fireExclaim = new ExclaimFire(new Rectangle(6000, 6000, 100, 200), exclamation);
            fire = new Fire(new Rectangle(6000, 6000, 100, 200), firesheet);

            // Reset camera
            camera.position.X = Math.Max(player.position.X, camera.boundingRectangle.Width / 2);
            camera.position.Y = Math.Min(Math.Min(player.position.Y, camera.boundingRectangle.Height / 2), lavas[0].Top + LAVA_HEIGHT_SHOWN - camera.boundingRectangle.Height / 2);
            
            // Reset stats
            currentState = Gamestate.play;
            maxHeight = 0;
            distance = 0;
            points = 0;
            timer = 0;
        }

        // Create and remove lava objects to keep necessary ones on screen
        private void tileLava()
        {
            // Manage left edge
            if (lavas[0].Right + lavaSize.Width < camera.boundingRectangle.Left)
                lavas.RemoveAt(0);
            else
                lavas.Insert(0, new Lava(new Rectangle((int)lavas[0].position.X + lavaSize.Width, (int)lavas[0].position.Y, lavaSize.Width, lavaSize.Height), lavaTexture));

            // Manage right edge
            if (lavas[lavas.Count - 1].Left - lavaSize.Width > camera.boundingRectangle.Right)
                lavas.RemoveAt(lavas.Count - 1);
            else
                lavas.Add(new Lava(new Rectangle((int)lavas[0].position.X + lavaSize.Width, (int)lavas[0].position.Y, lavaSize.Width, lavaSize.Height), lavaTexture));
        }

        // Ran when the player dies
        private void onDeath()
        {
            // Change state
            currentState = Gamestate.gameover;

            // Update highscores
            highScores.Add(score);
            highScores.Sort();
            highScores.Reverse();
            while (highScores.Count > 10)
                highScores.RemoveAt(10);
            SaveScores();

            // Clear platforms
            platforms.Clear();
        }

        // Create a new platform in calculated position automatically
        private void createPlatform()
        {
            // Average height gain based on difficulty (max distance)
            int avgGain = PLATFORM_HEIGHT_GAIN + (int)(PLATFORM_EXTRA_HEIGHT_GAIN * Math.Pow(.5, LastPlatform.position.X / PLATFORM_DIFFICULTY_DISTANCE));
            int dHeight = r.Next(-(avgGain + PLATFORM_HEIGHT_VARIANCE), -(avgGain - PLATFORM_HEIGHT_VARIANCE));
            Vector2 position = new Vector2();
            position.Y = Math.Min(LastPlatform.position.Y + dHeight, lavas[0].Top - PLATFORM_MIN_HEIGHT);

            // Platform width calculations
            int width = (int)(Platform.MIN_WIDTH + (Platform.MAX_WIDTH - Platform.MIN_WIDTH) * Math.Pow(.5, LastPlatform.position.X / PLATFORM_DIFFICULTY_DISTANCE));
            width = r.Next(Math.Max(Platform.MIN_WIDTH, width - PLATFORM_WIDTH_VARIANCE), Math.Min(Platform.MAX_WIDTH, width + PLATFORM_WIDTH_VARIANCE));

            // Fraction of the max jump distance based on difficulty (max distance)
            float difficultyVariance = PLATFORM_BONUS_WIGGLE_ROOM * (1 - (float)Math.Pow(.5, LastPlatform.position.X / PLATFORM_DIFFICULTY_DISTANCE));
            float distanceModifier = (float)(PLATFORM_AVERAGE_DIFFICULTY + (r.NextDouble() * difficultyVariance * 2 - difficultyVariance));
            float dDistance = distanceModifier * (player.GetMaxJumpDistance(dHeight) + width);
            position.X = Math.Max(LastPlatform.position.X + dDistance, LastPlatform.position.X + width * 2);

            // Platform modifiers
            bool isBreaking = r.NextDouble() < PLATFORM_BREAKING_CHANCE;

            createPlatform(position, width, isBreaking);
        }

        // Spawn a platform (mainly to specify first platform position)
        private void createPlatform(Vector2 position, int width, bool isBreaking)
        {
            Texture2D texture = platform;
            if (isBreaking)
                texture = platformWeak;
            if (r.NextDouble() < PLATFORM_GOLDEN_CHANCE)
            {
                isBreaking = true;
                texture = goldenPlatform;
            }

            platforms.Add(new Platform(new Rectangle((int)position.X, (int)position.Y, width, Platform.HEIGHT), texture, isBreaking));
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
                    spriteBatch.DrawString(titleTextFont, "SINKHOLE SPRINTER", new Vector2(centerText(titleTextFont, "SINKHOLE SPRINTER"), 50), Color.Black);
                    spriteBatch.DrawString(titleFont, "single player", new Vector2(centerText(titleFont, "single player"), 200), titleScreenColors[0]);
                    spriteBatch.DrawString(titleFont, "multiplayer", new Vector2(centerText(titleFont, "multiplayer"), 300), titleScreenColors[1]);
                    spriteBatch.DrawString(titleFont, "high scores", new Vector2(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("high scores").Length() / 2), 400), titleScreenColors[2]);

                    break;
                case Gamestate.play:
                    // spriteBatch.Draw(placeholder, new Rectangle(0, lavas[0].rect.Bottom - 5, 1500, Math.Max(GraphicsDevice.Viewport.Height - lavas[0].rect.Bottom + 5, 0)), new Color(255, 79, 9));
                    foreach (Platform platform in platforms)
                    {
                            camera.Draw(gameTime, spriteBatch, platform);
                    }
                    camera.DrawPlayer(gameTime, spriteBatch, player);
                    foreach (Lava lava in lavas)
                        camera.Draw(gameTime, spriteBatch, lava);
                    
                    //rocks
                    for (int a = 0; a < rockArray.Length; a++)
                        camera.Draw(gameTime, spriteBatch, rockArray[a]);
                    //fire
                    camera.Draw(gameTime, spriteBatch, fireExclaim);
                    camera.Draw(gameTime, spriteBatch, fire, fire.currentRect);

                    // Draw stat bar at top
                    spriteBatch.Draw(placeholder, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, 25), Color.Black);
                    spriteBatch.DrawString(scoreFont, "score: " + score, new Vector2(0, 00), Color.White); // points distance max height
                    spriteBatch.DrawString(scoreFont, "points: " + points, new Vector2(centerText(scoreFont, "points: " + points), 00), Color.White);
                    spriteBatch.DrawString(scoreFont, "height: " + maxHeight, new Vector2(1280 - (scoreFont.MeasureString("height: " + maxHeight).Length()), 00), Color.White);
                    spriteBatch.DrawString(scoreFont, "distance: " + distance, new Vector2((1280 - (scoreFont.MeasureString("distance : " + distance).Length()) + 
                        GraphicsDevice.Viewport.Width / 2 - (scoreFont.MeasureString("points: " + points).Length() / 2)) / 2, 0), Color.White);
                    // display number of hearts
                    float heartsVectorX = 230; // Distance between points & score
                    spriteBatch.DrawString(scoreFont, "hearts: ", new Vector2(heartsVectorX,00), Color.White);
                    int heartsX = 230;
                    for (int i = 0; i < player.hearts; i++)
                    {
                        spriteBatch.Draw(hearts, new Rectangle((int)(heartsX + scoreFont.MeasureString("hearts: ").X), 0, 30, 30), Color.White);
                        heartsX += 30; 

                    }

                    break;

                case Gamestate.gameover:
                    spriteBatch.DrawString(titleTextFont, "you died", new Vector2(centerText(titleTextFont, "you died"), 50), Color.DarkRed);
                    spriteBatch.DrawString(titleFont, "play again", new Vector2(GraphicsDevice.Viewport.Width / 4 - (titleFont.MeasureString("play again").Length() / 2), 350), deathScreenColors[0]); //1280
                    spriteBatch.DrawString(titleFont, "main menu", new Vector2(GraphicsDevice.Viewport.Width / 2 + GraphicsDevice.Viewport.Width / 4 - (titleFont.MeasureString("main menu").Length() / 2), 350), deathScreenColors[1]);
                    spriteBatch.DrawString(titleFont, "press r to play again", new Vector2(centerText(titleFont, "press r to play again"), 400), Color.Gold);

                    spriteBatch.DrawString(scoreFont, "final score: " + score, new Vector2(centerText(scoreFont, "final score" + score), 150), Color.Black);
                    spriteBatch.DrawString(scoreFont, "final distance: " + distance, new Vector2(centerText(scoreFont, "final distance: " + distance), 200), Color.Black);
                    spriteBatch.DrawString(scoreFont, "final height: " + maxHeight, new Vector2(centerText(scoreFont,"final height: " + maxHeight), 250), Color.Black);
                    break;

                case Gamestate.highscores:
                    spriteBatch.DrawString(titleFont, "main menu", new Vector2(GraphicsDevice.Viewport.Width / 4 - (titleFont.MeasureString("main menu").Length() / 2), GraphicsDevice.Viewport.Height / 3), highScoreColors[0]);
                    spriteBatch.DrawString(titleTextFont, "highscores", new Vector2(centerText(titleTextFont,"highscores"),0), Color.Black);

                    for (int i = 0; i < 10; i++) // Loop to draw numbers from 1-10
                    {
                        Color color = highScoreCols[Math.Min(i, Math.Max(highScoreCols.Length - 1, 0))];
                        if (i < highScores.Count && highScores[i] != 0) // if available highscore, draws highscore with correlating color
                        {
                            spriteBatch.DrawString(scoreFont, i+1+". " + highScores[i], new Vector2(leaderboardPos.X, leaderboardPos.Y +( i * 50)), color);
                        }
                        else // Draws a blank number
                        {
                            spriteBatch.DrawString(scoreFont, i +1+ ". ", new Vector2(leaderboardPos.X, leaderboardPos.Y + (i * 50)), color);
                        }
                    }
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        private float centerText(SpriteFont font, string txt) // calculate the x position for the text and center it 
        {
            return GraphicsDevice.Viewport.Width / 2 - (font.MeasureString(txt).Length() / 2);
        }
        private bool changeColors(MouseState mouse, string text, Rectangle txtRect) //change colors of text when the mouse hovers over it
        {
            return mouse.X > txtRect.X &&
                mouse.X < txtRect.X + ((text.Length - 1) * 20) &&
                mouse.Y > txtRect.Y + 10 &&
                mouse.Y < txtRect.Y + txtRect.Height;
        }
    }
}

