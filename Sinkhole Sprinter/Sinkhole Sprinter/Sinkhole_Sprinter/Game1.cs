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
        Player player2;
        int players;
        Texture2D playerOneHeartText, playerTwoHeartText;

        // General
        Camera camera;
        Texture2D background;
        MouseState mouse, oldMouse;
        KeyboardState oldKb;
        enum Gamestate
        {
            title, play, gameover, highscores, credits
        }
        Gamestate currentState;

        // UI
        SpriteFont titleFont, titleTextFont, scoreFont, testFont;
        Color titleColor = Color.Black;
        Rectangle titleRect, multiplayerTextRect;
        Color[] titleScreenColors = { Color.Black, Color.Black, Color.Black, Color.Black };
        Color[] deathScreenColors = { Color.Black, Color.Black, Color.Black };
        Color[] highScoreColors = { Color.Black, Color.Black, Color.Black};
        Color[] highScoreCols = { Color.Gold, Color.Silver, Color.Brown, Color.Black };
        Rectangle[] deathScreenText, highScoreTxtRect, mainScreenText;
        Vector2 leaderboardPos;
        string endText;

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
        
        const int PLATFORM_GOLDEN_POINTS = 50;          // Points gained by touching a golden platform
        const int PLATFORM_POINTS = 20;                 // Points gained by touching a point platform
        const double PLATFORM_BREAKING_CHANCE = .3;     // Chance for a platform to be a breaking platform
        const double PLATFORM_POINT_CHANCE = .1;        // Chance for a platform to be a point platform
        const double PLATFORM_GOLDEN_CHANCE = .25;      // Chance for a point platform to be a golden platform
        const double PLATFORM_MOVING_CHANCE = .05;      // Chance for a platform to be a moving platform

        Texture2D platform;
        Texture2D weakPlatform;
        Texture2D goldenPlatform;
        Texture2D pointPlatform;
        Texture2D movingPlatform;
        Texture2D placeholder;
        List<Platform> platforms;
        List<Platform> extraPlatforms;

        Song music;

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

        // FallingRocks
        ExclaimRocks exclaimRocks;
        FallingRocks fallingRocks;
        Texture2D stalagsheet;
        List<bool> hazardCollisionCheck;

        // RockWall
        const int ROCK_SIZE = 40;
        RockWall rockWall;
        Rectangle rockWallRect;
        List<Texture2D> rocks;
        Rock tempRock;
        Rock[] rockArray = new Rock[200];

        //Double Jump
        Texture2D doubleJump;


        //test
        Rectangle temp;
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
            extraPlatforms = new List<Platform>();
            lavas = new List<Lava>();
            hazardCollisionCheck = new List<bool>();
            lavaSize = new Rectangle(0, -200, 1500, 300);
            rockWallRect = new Rectangle(-800, 360, 700, 720);
            rocks = new List<Texture2D>();
            doubleJump = this.Content.Load<Texture2D>("double jump copy");

            // High Score
            LoadScores();
            highScoreTxtRect = mainScreenText = deathScreenText = new Rectangle[5];
            leaderboardPos = new Vector2(600, 100);
            endText = "you died";
            base.Initialize();

        }

        private void LoadScores()
        {
            highScores = new List<int>();
            try
            {
                if (!File.Exists(fileName))
                {
                    using (StreamWriter writer = new StreamWriter(fileName)) {}
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
            playerOneHeartText= Content.Load<Texture2D>("purple hearts");
            playerTwoHeartText= Content.Load<Texture2D>("orange hearts");

            background = Content.Load<Texture2D>("cave");

            // Fonts
            titleTextFont = Content.Load<SpriteFont>("SpriteFont2");
            titleFont = Content.Load<SpriteFont>("SpriteFont1");
            scoreFont = Content.Load<SpriteFont>("ScoreFont");
            // rectangles to highlight text in update method
            mainScreenText[0]=titleRect = new Rectangle((int)(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("singleplayer").Length() / 2)), 200, 30, 30);
            mainScreenText[1]=multiplayerTextRect = new Rectangle((int)(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("multiplayer").Length() / 2)), 300, 30, 30);
            mainScreenText[2] = new Rectangle((int)(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("highscores").Length() / 2)), 400, 30, 30);
            mainScreenText[3] = new Rectangle((int)(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("credits").Length() / 2)), 500, 30, 30);
            //
            deathScreenText[0] = new Rectangle((int)(GraphicsDevice.Viewport.Width / 4 - (titleFont.MeasureString("play again").Length() / 2)), 350, 30, 30);
            deathScreenText[1] = new Rectangle((int)(GraphicsDevice.Viewport.Width / 4 + (GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("main menu").Length() / 2))), 350, 30, 30); 
            //
            highScoreTxtRect[0] = new Rectangle((int)(GraphicsDevice.Viewport.Width / 4 - (titleFont.MeasureString("main menu").Length() / 2)), GraphicsDevice.Viewport.Height / 3, 30, 30);
            testFont = Content.Load<SpriteFont>("SpriteFont3");

            // Other sprites
            placeholder = this.Content.Load<Texture2D>("white");
            goldenPlatform = this.Content.Load<Texture2D>("ylwPlatform");
            pointPlatform = this.Content.Load<Texture2D>("bluePlatform");
            platform = this.Content.Load<Texture2D>("platform");
            weakPlatform = this.Content.Load<Texture2D>("platformWeak");
            // TO CHANGE
            movingPlatform = this.Content.Load<Texture2D>("platform");

            firesheet = this.Content.Load<Texture2D>("Fire");
            lavaTexture = this.Content.Load<Texture2D>("Lava");
            exclamation = this.Content.Load<Texture2D>("exclamation");
            stalagsheet = this.Content.Load<Texture2D>("stalagmites");


            // Rocks
            rocks.Add(this.Content.Load<Texture2D>("rock1"));
            rocks.Add(this.Content.Load<Texture2D>("rock2"));
            rocks.Add(this.Content.Load<Texture2D>("rock3"));
            rocks.Add(this.Content.Load<Texture2D>("rock4"));

            //sound
            music = this.Content.Load<Song>("Music");
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
                            players = 1;
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
                            players = 2;
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

                    // Open credits menu
                    if (changeColors(mouse, "credits", mainScreenText[3]))
                    {
                        titleScreenColors[3] = Color.Gold;
                        if (mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released)
                        {
                            currentState = Gamestate.credits;
                        }
                    }
                    else
                        titleScreenColors[3] = Color.Black;

                    break;

                case Gamestate.play:
                    // Remove broken platforms and check collisions
                    for (int x = 0; x < platforms.Count; x++)
                    {
                        //platforms[x].update(PLATFORM_SPEED);
                        platforms[x].Update();
                        if (platforms[x] is BreakingPlatform && ((BreakingPlatform)platforms[x]).touchedTimer == 0)
                        {
                            platforms.RemoveAt(x);
                            x--;
                            continue;
                        }
                        if (platforms[x].rect.Intersects(player.rect))
                        {
                            if (platforms[x] is PointPlatform) {
                                PointPlatform pointPlatform = (PointPlatform)platforms[x];
                                if (!pointPlatform.used)
                                {
                                    pointPlatform.used = true;
                                    points += pointPlatform.points;
                                }
                            }
                            player.CheckCollisions(platforms[x]);
                        }
                        if (player2 != null && platforms[x].rect.Intersects(player2.rect))
                        {
                            if (platforms[x] is PointPlatform)
                            {
                                PointPlatform pointPlatform = (PointPlatform)platforms[x];
                                if (!pointPlatform.used)
                                {
                                    pointPlatform.used = true;
                                    points += pointPlatform.points;
                                }
                            }
                            player2.CheckCollisions(platforms[x]);
                        }
                        if (platforms[x].power != null && platforms[x].power.rect.Intersects(player.rect))
                        {
                            switch (platforms[x].power.type)
                            {
                                case Power.variant.speed:
                                    player.effect = Player.power.speed;
                                    break;
                                case Power.variant.jump:
                                    player.effect = Player.power.jump;
                                    break;
                            }
                            platforms[x].power = null;
                        }

                        if (player2 != null && platforms[x].power != null && platforms[x].power.rect.Intersects(player2.rect))
                        {
                            switch (platforms[x].power.type)
                            {
                                case Power.variant.speed:
                                    player2.effect = Player.power.speed;
                                    break;
                                case Power.variant.jump:
                                    player2.effect = Player.power.jump;
                                    break;
                            }
                            platforms[x].power = null;
                        }
                    }

                    //do same thing for extra platforms
                    for (int x = 0; x < extraPlatforms.Count; x++)
                    {
                        //platforms[x].update(PLATFORM_SPEED);
                        extraPlatforms[x].Update();
                        if (extraPlatforms[x] is BreakingPlatform && ((BreakingPlatform)extraPlatforms[x]).touchedTimer == 0)
                        {
                            extraPlatforms.RemoveAt(x);
                            x--;
                            continue;
                        }
                        if (extraPlatforms[x].rect.Intersects(player.rect))
                        {
                            if (extraPlatforms[x] is PointPlatform)
                            {
                                PointPlatform pointPlatform = (PointPlatform)extraPlatforms[x];
                                if (!pointPlatform.used)
                                {
                                    pointPlatform.used = true;
                                    points += pointPlatform.points;
                                }
                            }
                            player.CheckCollisions(extraPlatforms[x]);
                        }
                        if (player2 != null && extraPlatforms[x].rect.Intersects(player2.rect))
                        {
                            if (extraPlatforms[x] is PointPlatform)
                            {
                                PointPlatform pointPlatform = (PointPlatform)extraPlatforms[x];
                                if (!pointPlatform.used)
                                {
                                    pointPlatform.used = true;
                                    points += pointPlatform.points;
                                }
                            }
                            player2.CheckCollisions(extraPlatforms[x]);
                        }
                    }

                    //if (timer % 60 == 0)
                    //{
                    //    platforms.Add(new Platform(new Rectangle(1280, platforms[platforms.Count-1].rect.Y+r.Next(-150,50), 70, 10), placeholder));
                    //}

                    // Update positions
                    player.Update();
                    if (player2 != null)
                        player2.Update();
                    camera.Update();
                    rockWall.Update();
                    lavaHeight -= LAVA_RISE_SPEED;

                    //ensures lava maintains minimum distance from player
                    if (lavaHeight > camera.boundingRectangle.Bottom + 50 && timer > 300)
                        lavaHeight = Math.Max(MathHelper.Lerp(lavaHeight, camera.boundingRectangle.Bottom + 50, 0.02f), lavaHeight - LAVA_RISE_SPEED * 4); // Capped at additional 4x lava speed

                    //ensures rockwall maintains minimum distance from player
                    if (rockWall.Right < camera.boundingRectangle.Left - 300 && timer > 300)
                        rockWall.position.X = MathHelper.Lerp(rockWall.Right, camera.boundingRectangle.Left - 300, .02f) - rockWall.rect.Width / 2;

                    // Tile lava and adjust height
                    tileLava();
                    foreach (Lava lava in lavas)
                    {
                        lava.position.Y = lavaHeight + lavaSize.Height / 2;
                    }

                    //Warning system
                    fireExclaim.Update(lavaHeight, player.Right, camera.Right);
                    exclaimRocks.Update(camera.Top + exclaimRocks.rect.Height / 4, player.Right, camera.Right);

                    //Fire
                    fire.Update(fireExclaim.pastPosition);
                    if (exclaimRocks.timer % 200 == 0)
                    {
                        Vector2 p = exclaimRocks.pastPosition;
                        p.Y -= exclaimRocks.rect.Height / 4 + fallingRocks.rect.Height;
                        fallingRocks.Update(p);
                    }
                    else
                    {
                        fallingRocks.Update();
                    }

                    // Update camera position
                    camera.FollowX(player, player2);
                    camera.FollowY(player, player2);
                    camera.position.Y = Math.Min(Math.Min(camera.position.Y, camera.boundingRectangle.Height / 2), lavaHeight + LAVA_HEIGHT_SHOWN - camera.boundingRectangle.Height / 2);

                    // Create platforms, remove first platform if under lava
                    if (LastPlatform.position.X < camera.boundingRectangle.Right)
                    {
                        createPlatform();
                    }

                    if (platforms[0].Top - PLATFORM_MIN_HEIGHT > lavaHeight)
                        platforms.RemoveAt(0);

                    if (player.Bottom >= lavaHeight) // checks if player is dead
                    {
                        endText = player2 != null ? "player two wins" : "you died";
                        onDeath();
                    }
                    if (player2 != null && player2.Bottom >= lavaHeight)
                    {
                        endText = "player one wins";
                        onDeath();
                    }
                    // Kills player if they run out of hearts
                    if (player.hearts <= 0)
                    {
                        endText = player2 != null ? "player two wins" : "you died";
                        onDeath();
                    }
                    if (player2 != null && player2.hearts <= 0)
                    {
                        endText = "player one wins";
                        onDeath();
                    }

                    temp = fire.rect;
                    temp.Y += 15;
                    temp.Height -= 15;
                    temp.Width -= 60;
                    temp.X += 30;

                    // Takes a heart away from the player if they touch a fire hazard
                    if (player.rect.Intersects(temp))
                    {
                        if (!fireExclaim.collisionCheck)
                        {
                            player.TakeDamage();
                            fireExclaim.OnCollide();
                        }
                    }

                    temp = fallingRocks.rect;
                    temp.X += 25;
                    temp.Width -= 50;
                    // Takes a heart away from the player if they touch a stalagmite
                    if (player.rect.Intersects(temp))
                    {
                        if (!fallingRocks.collisionCheck)
                        {
                            player.TakeDamage();
                            fallingRocks.OnCollide();
                        }
                    }
                    if (players == 2 && player2.rect.Intersects(fire.rect)) // Takes a heart away from the player if they touch a fire hazard
                    {
                        if (!fireExclaim.collisionCheck)
                        {
                            player2.TakeDamage();
                            fireExclaim.OnCollide();
                        }
                    }

                    //if (player.rect.Intersects(fallingRocks.rect))
                    //{
                    //    onDeath();
                    //}    
                    //Update rockwall position
                    rockWall.position.Y = camera.position.Y;
                    if (player.position.X <= rockWall.Right) // checks if player is dead
                    {
                        player.TakeDamage();
                        rockWall.OnCollide();
                    }
                    if (player2 != null && player2.position.X <= rockWall.Right)
                    {
                        endText = "player one wins";
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
                    if (player2 != null)
                    {
                        maxHeight = Math.Max(STARTING_PLATFORM_HEIGHT - Platform.HEIGHT / 2 - player2.Bottom, maxHeight);
                        distance = Math.Max((int)player2.position.X, distance);
                    }
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
                case Gamestate.credits:
                    if (changeColors(mouse, "credits", highScoreTxtRect[0]))
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
            createExtraPlatform(new Vector2(Platform.MAX_WIDTH / 2, camera.boundingRectangle.Height * .7f), Platform.MAX_WIDTH, false);

            if (players == 1)
            {
                player = new Player(new Rectangle(50, STARTING_PLATFORM_HEIGHT - Platform.HEIGHT / 2 - 50, 75, 75), textures, running, jumping, standing, 0);
                player2 = null;
            }
            else
            {
                player = new Player(new Rectangle(30, STARTING_PLATFORM_HEIGHT - Platform.HEIGHT / 2 - 50, 75, 75), textures, running, jumping, standing, 1);
                player2 = new Player(new Rectangle(60, STARTING_PLATFORM_HEIGHT - Platform.HEIGHT / 2 - 50, 75, 75), textures, running, jumping, standing, 2);
            }

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
            fireExclaim = new ExclaimFire(new Rectangle(6000, 6000, 100, 400), exclamation);
            fire = new Fire(new Rectangle(6000, 6000, 100, 400), firesheet);

            //create stalagmites
            exclaimRocks = new ExclaimRocks(new Rectangle(6000, 6000, 100, 200), exclamation);
            fallingRocks = new FallingRocks(new Rectangle(6000, 6000, 75, 200), stalagsheet);
            // Reset camera
            camera.position.X = Math.Max(player.position.X, camera.boundingRectangle.Width / 2);
            camera.position.Y = Math.Min(Math.Min(player.position.Y, camera.boundingRectangle.Height / 2), lavas[0].Top + LAVA_HEIGHT_SHOWN - camera.boundingRectangle.Height / 2);
            
            // Reset stats
            currentState = Gamestate.play;
            maxHeight = 0;
            distance = 0;
            points = 0;
            timer = 0;

            //music
            MediaPlayer.Play(music);
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
            extraPlatforms.Clear();
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
            if (r.Next(100) < 5)
            {
                Rectangle rect = new Rectangle(
                    platforms[platforms.Count - 1].rect.X,
                    platforms[platforms.Count - 1].rect.Y - 40,
                    50,
                    50);

                platforms[platforms.Count - 1].power = new Power(rect, doubleJump, Power.variant.jump);
            }

            //random chance to spawn bonus platforms relative to main ones
            if(r.Next(100)<30)
                createExtraPlatform(new Vector2(position.X + r.Next(-175, 175), position.Y + r.Next(75, 400)), width, isBreaking);

            if (r.Next(100) < 60)
                createExtraPlatform(new Vector2(position.X + r.Next(-175, 175), position.Y + r.Next(-400, -75)), width, isBreaking);
                
                

        }

        // Spawn a platform (mainly to specify first platform position)
        private void createPlatform(Vector2 position, int width, bool variants)
        {
            Rectangle rect = new Rectangle((int)position.X, (int)position.Y, width, Platform.HEIGHT);
            
            // Platform Variants
            if (variants)
            {
                if (r.NextDouble() < PLATFORM_MOVING_CHANCE)
                {
                    string axis = r.Next(2) == 0 ? "x" : "y";
                    platforms.Add(new MovingPlatform(rect, movingPlatform, axis));
                    return;
                }
                if (r.NextDouble() < PLATFORM_POINT_CHANCE)
                {
                    if (r.NextDouble() < PLATFORM_GOLDEN_CHANCE)
                    {
                        platforms.Add(new PointPlatform(rect, goldenPlatform, PLATFORM_GOLDEN_POINTS));
                    }
                    else
                    {
                        platforms.Add(new PointPlatform(rect, pointPlatform, PLATFORM_POINTS));
                    }
                    return;
                }
                if (r.NextDouble() < PLATFORM_BREAKING_CHANCE)
                {
                    platforms.Add(new BreakingPlatform(rect, weakPlatform));
                    return;
                }
                platforms.Add(new Platform(rect, platform));
                return;
            }
            platforms.Add(new Platform(rect, platform));
        }

        //so that extra platforms do not interfere with the algorithm for regular ones
        private void createExtraPlatform(Vector2 position, int width, bool variants)
        {
            Rectangle rect = new Rectangle((int)position.X, (int)position.Y, width, Platform.HEIGHT);

            // Platform Variants
            if (variants)
            {
                if (r.NextDouble() < PLATFORM_MOVING_CHANCE)
                {
                    string axis = r.Next(2) == 0 ? "x" : "y";
                    extraPlatforms.Add(new MovingPlatform(rect, movingPlatform, axis));
                    return;
                }
                if (r.NextDouble() < PLATFORM_POINT_CHANCE)
                {
                    if (r.NextDouble() < PLATFORM_GOLDEN_CHANCE)
                    {
                        extraPlatforms.Add(new PointPlatform(rect, goldenPlatform, PLATFORM_GOLDEN_POINTS));
                    }
                    else
                    {
                        extraPlatforms.Add(new PointPlatform(rect, pointPlatform, PLATFORM_POINTS));
                    }
                    return;
                }
                if (r.NextDouble() < PLATFORM_BREAKING_CHANCE)
                {
                    extraPlatforms.Add(new BreakingPlatform(rect, weakPlatform));
                    return;
                }
                extraPlatforms.Add(new Platform(rect, platform));
                return;
            }
            extraPlatforms.Add(new Platform(rect, platform));
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
                    spriteBatch.DrawString(titleFont, "credits", new Vector2(GraphicsDevice.Viewport.Width / 2 - (titleFont.MeasureString("credits").Length() / 2), 500), titleScreenColors[3]);
                    break;

                case Gamestate.play:
                    spriteBatch.Draw(background, new Rectangle(0, -200, 1280, 1100), new Rectangle(0, 0, 785, 442),  Color.White);
                    // spriteBatch.Draw(placeholder, new Rectangle(0, lavas[0].rect.Bottom - 5, 1500, Math.Max(GraphicsDevice.Viewport.Height - lavas[0].rect.Bottom + 5, 0)), new Color(255, 79, 9));
                    foreach (Platform platform in platforms)
                    {
                        camera.Draw(gameTime, spriteBatch, platform);

                        if (platform.power != null)
                            camera.Draw(gameTime, spriteBatch, platform.power);
                    }

                    foreach (Platform platform in extraPlatforms)
                    {
                        camera.Draw(gameTime, spriteBatch, platform);
                    }
                    camera.DrawPlayer(gameTime, spriteBatch, player);
                    if (players == 2)
                        camera.DrawPlayer(gameTime, spriteBatch, player2);
                    foreach (Lava lava in lavas)
                        camera.Draw(gameTime, spriteBatch, lava);

                    //rocks
                    for (int a = 0; a < rockArray.Length; a++)
                        camera.Draw(gameTime, spriteBatch, rockArray[a]);
                    //fire
                    //spriteBatch.Draw(placeholder, temp, Color.Red);
                    if (timer > 100)
                    {
                        camera.Draw(gameTime, spriteBatch, fireExclaim);
                        camera.Draw(gameTime, spriteBatch, fire, fire.currentRect);
                    }
                    
                    //rocks
                    camera.Draw(gameTime, spriteBatch, exclaimRocks);
                    camera.Draw(gameTime, spriteBatch, fallingRocks,fallingRocks.currentRect);
                    // Draw stat bar at top
                    spriteBatch.Draw(placeholder, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, 25), Color.Black);
                    if (player2 != null)
                    {
                        spriteBatch.DrawString(scoreFont, "height: " + maxHeight, new Vector2(1280 - (scoreFont.MeasureString("height: " + maxHeight).Length()), 0), Color.White);
                        spriteBatch.DrawString(scoreFont, "distance: " + distance, new Vector2(0, 0), Color.White);
                    }
                    else
                    {
                        spriteBatch.DrawString(scoreFont, "score: " + score, new Vector2(0, 0), Color.White); // points distance max height
                        spriteBatch.DrawString(scoreFont, "points: " + points, new Vector2(centerText(scoreFont, "points: " + points), 0), Color.White);
                        spriteBatch.DrawString(scoreFont, "height: " + maxHeight, new Vector2(1280 - (scoreFont.MeasureString("height: " + maxHeight).Length()), 0), Color.White);
                        spriteBatch.DrawString(scoreFont, "distance: " + distance, new Vector2((1280 - (scoreFont.MeasureString("distance : " + distance).Length()) +
                            GraphicsDevice.Viewport.Width / 2 - (scoreFont.MeasureString("points: " + points).Length() / 2)) / 2, 0), Color.White);
                    }
                    // display number of hearts
                    if (players != 2)
                    {
                        float heartsVectorX = 230; // Distance between points & score
                        spriteBatch.DrawString(scoreFont, "hearts: ", new Vector2(heartsVectorX, 00), Color.White);
                        int heartsX = 230;
                        for (int i = 0; i < player.hearts; i++)
                        {
                            spriteBatch.Draw(hearts, new Rectangle((int)(heartsX + scoreFont.MeasureString("hearts: ").X), 0, 30, 30), Color.White);
                            heartsX += 30;
                        }
                    }
                    else
                    {
                        float P1heartsVectorX = 230; // Distance between points & score
                        spriteBatch.DrawString(scoreFont, "hearts: ", new Vector2(P1heartsVectorX, 00), Color.White);
                        int heartsX = 230;
                        for (int i = 0; i < player.hearts; i++)
                        {
                            spriteBatch.Draw(playerOneHeartText, new Rectangle((int)(heartsX + scoreFont.MeasureString("hearts: ").X), 0, 30, 30), Color.White);
                            heartsX += 30;
                        }

                        float P2heartsVectorX = 1280 - 500;// Distance between points & score
                        spriteBatch.DrawString(scoreFont, "hearts: ", new Vector2(P2heartsVectorX, 00), Color.White);
                        int P2heartsX = 1280 - 500;
                        for (int i = 0; i < player2.hearts; i++)
                        {
                            spriteBatch.Draw(playerTwoHeartText, new Rectangle((int)(P2heartsX + scoreFont.MeasureString("hearts: ").X), 0, 30, 30), Color.White);
                            P2heartsX += 30;
                        }


                    }
                    
                    break;

                case Gamestate.gameover:
                    spriteBatch.DrawString(titleTextFont, endText, new Vector2(centerText(titleTextFont, endText), 50), Color.DarkRed);
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

                case Gamestate.credits:
                    spriteBatch.DrawString(titleFont, "main menu", new Vector2(GraphicsDevice.Viewport.Width / 4 - (titleFont.MeasureString("main menu").Length() / 2), GraphicsDevice.Viewport.Height / 3), highScoreColors[0]);
                    spriteBatch.DrawString(titleTextFont, "credits", new Vector2(centerText(titleTextFont, "credits"), 0), Color.Black);
                    spriteBatch.DrawString(titleFont, "Many   thanks   too", new Vector2(centerText(titleFont, "Many   thanks   too"), 100), Color.Black);
                    spriteBatch.DrawString(scoreFont, "Font Monger", new Vector2(centerText(scoreFont, "Font Monger"), 150), Color.Black);
                    spriteBatch.DrawString(scoreFont, "Pizzadude", new Vector2(centerText(scoreFont, "Pizzadude"), 200), Color.Black);
                    spriteBatch.DrawString(scoreFont, "Karl Casey @ White Bat Audio", new Vector2(centerText(scoreFont, "Karl Casey @ White Bat Audio"), 250), Color.Black);
                    
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

