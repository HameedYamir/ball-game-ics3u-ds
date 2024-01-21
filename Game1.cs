//Author: Daniel Shomali
//File Name: Game1.cs
//Project Name: ballDropGame
//Creation Date: Jan. 19, 2024
//Modified Date: Jam. 19, 2024
//Description: Game where you combine and drop balls to get the biggest ball
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using GameUtility;

namespace GameStateDemo
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        //Game States - Add/Remove/Modify as needed
        //These are the most common game states, but modify as needed
        //You will ALSO need to modify the two switch statements in Update and Draw
        const int MENU = 0;
        const int SETTINGS = 1;
        const int INSTRUCTIONS = 2;
        const int GAMEPLAY = 3;
        const int PAUSE = 4;
        const int ENDGAME = 5;

        Random random = new Random();

        int screenWidth;
        int screenHeight;
        int targetFPS = 60;

        int randBall;
        int activeBalls = 0;
        int nextBallIndex = 0;
        int activeBallIndex = -1;
        float timeSinceLastCollision = 4.0f;


        bool ballFalling = false;
        bool ballCanDrag = true;
        Vector2 ballDragging = new Vector2(0f, 0f);

        //BASE STUFF
        Texture2D gameBGImg;
        Rectangle gameBGRec;
        Song windAmbience;

        //MENU STUFF
        Texture2D menuTitleImg;
        Rectangle menuTitleRec;

        Texture2D playBtnImg;
        Texture2D instructionsBtnImg;
        Texture2D exitBtnImg;

        Rectangle playBtnRec;
        Rectangle instructionsBtnRec;
        Rectangle exitBtnRec;

        SoundEffect dingSound;
        SoundEffect ballHitSound;

        //GAMEPLAY STUFF
        Texture2D mainBoxImg;
        Rectangle mainBoxRec;
        Texture2D optionsBoxImg;
        Rectangle optionsBoxRec;
        Texture2D ballscoreBoxImg;
        Rectangle ballscoreBoxRec;

        Texture2D pauseBtnImg;

        Texture2D resetBtnImg;
        Texture2D exitBtnGameStateImg;

        Rectangle pauseBtnRec;
        Rectangle resetBtnRec;
        Rectangle exitBtnGameStateRec;

        Texture2D[] ballsImg = new Texture2D[11];
        Ball[] balls = new Ball[100];
        private Texture2D[] textures;

        MouseState mouse;
        MouseState prevMouse;
        KeyboardState kb;
        KeyboardState prevKb;

        //Store and set the initial game state, typically MENU to start
        int gameState = MENU;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 750;

            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / targetFPS);

            graphics.ApplyChanges();

            IsMouseVisible = true;

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


            //BASE STUFF
            gameBGImg = Content.Load<Texture2D>("Backgrounds/gameBG");
            gameBGRec = new Rectangle(0, 0, gameBGImg.Width, gameBGImg.Height);
            windAmbience = Content.Load<Song>("Audio/Ambience/WindAmbience");

            MediaPlayer.Play(windAmbience);
            MediaPlayer.Volume = 1.0f; // Max volume
            MediaPlayer.IsRepeating = true;

            //MENU STUFF
            menuTitleImg = Content.Load<Texture2D>("Sprites/gameTitle");
            menuTitleRec = new Rectangle(0, 0, menuTitleImg.Width, menuTitleImg.Height);
            playBtnImg = Content.Load<Texture2D>("Sprites/PlayButton");
            playBtnRec = new Rectangle(gameBGRec.Width / 2 - playBtnImg.Width / 2, gameBGRec.Height / 2 - playBtnImg.Height / 2, playBtnImg.Width, playBtnImg.Height);
            instructionsBtnImg = Content.Load<Texture2D>("Sprites/InstructionsButton");
            instructionsBtnRec = new Rectangle(playBtnRec.X + playBtnRec.Width / 2 - instructionsBtnImg.Width / 2, playBtnRec.Bottom + 20, instructionsBtnImg.Width, instructionsBtnImg.Height);
            exitBtnImg = Content.Load<Texture2D>("Sprites/ExitButton");
            exitBtnRec = new Rectangle(playBtnRec.X + playBtnRec.Width / 2 - exitBtnImg.Width / 2, instructionsBtnRec.Bottom + 20, exitBtnImg.Width, exitBtnImg.Height);

            //GAME STUFF
            mainBoxImg = Content.Load<Texture2D>("Sprites/MainBox");
            mainBoxRec = new Rectangle(gameBGRec.Width / 2 - mainBoxImg.Width / 2, gameBGRec.Height / 2 - mainBoxImg.Height / 2, mainBoxImg.Width, mainBoxImg.Height);

            pauseBtnImg = Content.Load<Texture2D>("Sprites/PauseButton");
            resetBtnImg = Content.Load<Texture2D>("Sprites/ResetButton");
            exitBtnGameStateImg = Content.Load<Texture2D>("Sprites/ExitButtonGameState");

            pauseBtnRec = new Rectangle(69, 525, pauseBtnImg.Width, pauseBtnImg.Height);
            resetBtnRec = new Rectangle(69, 580, resetBtnImg.Width, resetBtnImg.Height);
            exitBtnGameStateRec = new Rectangle(69, 634, exitBtnGameStateImg.Width, exitBtnGameStateImg.Height);

            optionsBoxImg = Content.Load<Texture2D>("Sprites/OptionsBox");
            optionsBoxRec = new Rectangle(46, 495, optionsBoxImg.Width, optionsBoxImg.Height);

            ballscoreBoxImg = Content.Load<Texture2D>("Sprites/BallScoreBox");
            ballscoreBoxRec = new Rectangle(816, 38, ballscoreBoxImg.Width, ballscoreBoxImg.Height);

            dingSound = Content.Load<SoundEffect>("Audio/Sounds/ClickSound");
            ballHitSound = Content.Load<SoundEffect>("Audio/Sounds/BallHitSound");

            ballsImg[0] = Content.Load<Texture2D>("Sprites/Marble");
            ballsImg[1] = Content.Load<Texture2D>("Sprites/PingPongBall");
            ballsImg[2] = Content.Load<Texture2D>("Sprites/GolfBall");
            ballsImg[3] = Content.Load<Texture2D>("Sprites/BocceBall");
            ballsImg[4] = Content.Load<Texture2D>("Sprites/TennisBall");
            ballsImg[5] = Content.Load<Texture2D>("Sprites/BowlingBall");
            ballsImg[6] = Content.Load<Texture2D>("Sprites/VolleyBall");
            ballsImg[7] = Content.Load<Texture2D>("Sprites/BasketBall");
            ballsImg[8] = Content.Load<Texture2D>("Sprites/BigBouncyBall");
            ballsImg[9] = Content.Load<Texture2D>("Sprites/YogaBall");
            ballsImg[10] = Content.Load<Texture2D>("Sprites/GalacticBall");

          textures = new Texture2D[]
        {
            ballsImg[0], ballsImg[1], ballsImg[2], ballsImg[3], ballsImg[4],
            ballsImg[5], ballsImg[6], ballsImg[7], ballsImg[8], ballsImg[9], ballsImg[10]
        };

            SpawnNewBall(); //start with the marble spawned

            for (int i = 0; i < 11; i++) //new ball and ballimg
            {
                balls[i] = new Ball(ballsImg[i]);
            }

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            timeSinceLastCollision += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            prevMouse = mouse;
            mouse = Mouse.GetState();
            kb = Keyboard.GetState();

            switch (gameState)
            {
                case MENU:
                    //Get and implement menu interactions, e.g. when the user clicks a Play button set gameState = GAMEPLAY
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        if (playBtnRec.Contains(mouse.Position))
                        {
                            gameState = GAMEPLAY;
                            dingSound.Play();
                        }
                        else if (instructionsBtnRec.Contains(mouse.Position))
                        {
                            gameState = INSTRUCTIONS;
                            dingSound.Play();
                        }
                        else if (exitBtnRec.Contains(mouse.Position))
                        {
                            Exit();
                            dingSound.Play();
                        }
                    }
                    break;
                case SETTINGS:
                    //Get and apply changes to game settings
                    break;
                case INSTRUCTIONS:
                    //Get user input to return to MENU
                    break;
                case GAMEPLAY:
                    //Implement standared game logic (input, update game objects, apply physics, collision detection)
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        if (pauseBtnRec.Contains(mouse.Position))
                        {
                            gameState = PAUSE;
                        }
                        else if (exitBtnGameStateRec.Contains(mouse.Position))
                        {
                            Exit();
                        }

                    }

                    if (ballFalling == false)
                    {
                        HandleBallMovement();
                        if (kb.IsKeyDown(Keys.Space) && !prevKb.IsKeyDown(Keys.Space))
                        {
                            ballFalling = true;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < activeBalls; i++)
                        {
                            balls[i].DropBall();

                            if (balls[i].GetPos().X < mainBoxRec.Left) //collisions
                            {
                                balls[i].SetPosX(mainBoxRec.Left);
                                balls[i].BounceHorizontal();
                            }

                            if (balls[i].GetPos().X + balls[i].texture.Width > mainBoxRec.Right) //collisions
                            {
                                balls[i].SetPosX(mainBoxRec.Right - balls[i].texture.Width);
                                balls[i].BounceHorizontal();
                            }

                            if (balls[i].GetPos().Y < mainBoxRec.Top) //collisions
                            {
                                balls[i].SetPosY(mainBoxRec.Top);
                                balls[i].BounceVertical();
                            }

                            if (balls[i].GetPos().Y + balls[i].texture.Height > mainBoxRec.Bottom) //collisions
                            {
                                balls[i].SetPosY(mainBoxRec.Bottom - balls[i].texture.Height);
                                balls[i].BounceVertical();


                                if (timeSinceLastCollision >= 5f)
                                {
                                    timeSinceLastCollision = 0.0f;
                                    ballFalling = false;
                                    SpawnNewBall();
                                }

                                for (int j = i + 1; j < activeBalls; j++)
                                {
                                    // checks if ball 'i' is colliding with other ball 'j'. Handles collisions.

                                    if (balls[i].IsColliding(balls[j]))
                                    {
                                        balls[i].HandleCollision(balls[j], textures);
                                    }
                                }
                            }
                        }
                    }

                    break;
                case PAUSE:
                    //Get user input to resume the game
                    break;
                case ENDGAME:
                    //Wait for final input based on end of game options (end, restart, etc.)
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            switch (gameState)
            {
                case MENU:
                    //Draw the possible menu options
                    spriteBatch.Draw(gameBGImg, gameBGRec, Color.White);
                    spriteBatch.Draw(menuTitleImg, menuTitleRec, Color.White);
                    spriteBatch.Draw(playBtnImg, playBtnRec, Color.White);
                    spriteBatch.Draw(instructionsBtnImg, instructionsBtnRec, Color.White);
                    spriteBatch.Draw(exitBtnImg, exitBtnRec, Color.White);
                    break;
                case SETTINGS:
                    //Draw the settings with prompts
                    break;
                case INSTRUCTIONS:
                    //Draw the game instructions including prompt to return to MENU
                    spriteBatch.Draw(gameBGImg, gameBGRec, Color.White);
                    break;
                case GAMEPLAY:
                    //Draw all game objects on each layers (background, middleground, foreground and user interface)
                    spriteBatch.Draw(gameBGImg, gameBGRec, Color.White);
                    spriteBatch.Draw(mainBoxImg, mainBoxRec, Color.White);
                    spriteBatch.Draw(optionsBoxImg, optionsBoxRec, Color.White);
                    spriteBatch.Draw(ballscoreBoxImg, ballscoreBoxRec, Color.White);
                    spriteBatch.Draw(pauseBtnImg, pauseBtnRec, Color.White);
                    spriteBatch.Draw(resetBtnImg, resetBtnRec, Color.White);
                    spriteBatch.Draw(exitBtnGameStateImg, exitBtnGameStateRec, Color.White);

                    for (int i = 0; i < activeBalls; i++)
                    {
                        balls[i].DrawBall(spriteBatch);
                    }
                    break;
                case PAUSE:
                    //Draw the pause screen, this may include the full game drawing behind
                    spriteBatch.Draw(gameBGImg, gameBGRec, Color.White);
                    break;
                case ENDGAME:
                    //Draw the final feedback and prompt for available options (exit,restart, etc.)
                    spriteBatch.Draw(gameBGImg, gameBGRec, Color.White);
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
        private void SpawnNewBall()
        {
            if (nextBallIndex <= balls.Length)
            {
                int ballIndex = random.Next(0, 4);
                balls[nextBallIndex] = new Ball(ballsImg[ballIndex]);
                balls[nextBallIndex].SetVel(Vector2.Zero);
                balls[nextBallIndex].SetPosX(screenWidth / 2 - balls[nextBallIndex].texture.Width / 2);
                balls[nextBallIndex].SetPosY(0);
                activeBallIndex = nextBallIndex;
                nextBallIndex++;
                activeBalls++;
                balls[nextBallIndex - 1].CollisionCallback = HandleCollisionCallback;
                Console.WriteLine($"New Ball Spawned at X: {balls[nextBallIndex - 1].GetPos().X}, Y: {balls[nextBallIndex - 1].GetPos().Y}");
            }
        }

        private void HandleBallMovement() //put comments for each code block
        {
            KeyboardState kb = Keyboard.GetState();

            if (ballCanDrag && activeBallIndex != -1 && activeBallIndex < activeBalls)
            {
                if (kb.IsKeyDown(Keys.Left) || (kb.IsKeyDown(Keys.A)))
                {
                    balls[activeBallIndex].SetPosX(Math.Max(mainBoxRec.Left, balls[activeBallIndex].GetPos().X - 5f));
                }
                else if (kb.IsKeyDown(Keys.Right) || (kb.IsKeyDown(Keys.D)))
                {
                    balls[activeBallIndex].SetPosX(Math.Min(mainBoxRec.Right - balls[activeBallIndex].texture.Width, balls[activeBallIndex].GetPos().X + 5f));
                }
            }
        }
        private void HandleCollisionCallback(Ball ball1, Ball ball2)
        {
            if (ball1.texture == ball2.texture)
            {
                SpawnMergedBall(textures, ball1.GetPos());
                RemoveBall(ball1);
                RemoveBall(ball2);
                Console.WriteLine("The balls has been removed");
            }
        }

        private void RemoveBall(Ball ball)
        {
            int index = Array.IndexOf(balls, ball);
            if (index != -1)
            {
                for (int i = index; i < activeBalls - 1; i++)
                {
                    balls[i] = balls[i + 1];
                }
                activeBalls--;
                Console.WriteLine("Ball Removed");
            }
        }

        private void SpawnMergedBall(Texture2D[] textures, Vector2 position)
        {
            if (nextBallIndex <= balls.Length)
            {
                int currentIndex = Array.IndexOf(textures, balls[activeBallIndex].texture);
                int nextIndex = (currentIndex + 1) % textures.Length;

                balls[nextBallIndex] = new Ball(textures[nextIndex]);
                balls[nextBallIndex].SetVel(Vector2.Zero);
                balls[nextBallIndex].SetPosX(position.X);
                balls[nextBallIndex].SetPosY(position.Y);
                activeBallIndex = nextBallIndex;
                nextBallIndex++;
                activeBalls++;

                balls[nextBallIndex - 1].CollisionCallback = HandleCollisionCallback;
                nextBallIndex++;
                Console.WriteLine("Ball Merged");
            }
        }





    }
}