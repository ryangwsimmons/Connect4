//Author: Ryan Simmons
//File Name: Connect4.cs
//Project Name: Connect4
//Creation Date: April 27, 2017
//Modification Date: May 9, 2017
//Description: A recreation of the classic table-top game, Connect4.
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace Connect4
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Connect4 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Random number generator for determining whose turn it is at the beginning of a new game
        Random rng = new Random();

        //Size of Pieces
        const int PIECE_WIDTH = 76;
        const int PIECE_HEIGHT = 65;

        //Size of board borders
        const int BORDER_WIDTH = 4;
        const int BORDER_HEIGHT = 2;


        //Variables for Game States
        int GameState;
        const int MENU = 0;
        const int PLAY = 1;
        const int WIN1 = 2;
        const int WIN2 = 3;
        const int GAMEOVER = 4;
        const int VIEWSTATS = 5;

        //Define variables for menu images
        Texture2D logo;
        Texture2D playButton;
        Texture2D statsButton;
        Texture2D exitButton;
        Rectangle logoBox;
        Rectangle playBox;
        Rectangle statsBox;
        Rectangle exitBox;

        //Define variables for gameplay images
        Texture2D board;
        Texture2D player1;
        Texture2D player2;
        Rectangle boardBox;
        Rectangle player1Box;
        Rectangle player2Box;

        //Define variables for win screen images
        Texture2D menuButton;
        Rectangle menuBox;

        //Define variables for snap spaces
        Rectangle[] snapSpaces = new Rectangle[7];
        int currentSnapSpace;

        //Array for Board
        int[,] boardArray = new int[6, 7];

        //Variables for the screen dimensions
        int screenWidth;
        int screenHeight;

        //Variable for mouse state
        MouseState mouse;
        MouseState prevMouse;

        //Variable that determines how long it has been since the game has started
        int gameCount;

        //Variable that determines whose turn it is
        bool player1Turn;

        //Font for displaying messages to the user
        SpriteFont messageFont;

        //Font for displaying statistics
        SpriteFont statsFont;

        //Variable for message to be displayed
        string message;

        //Location of message
        Vector2 messageLoc;

        //Location of Statistics
        Vector2 statsLoc;
        Vector2 statsTitleLoc;

        //Variable for winning message location
        Vector2 winLoc;

        //Determines number of pieces used
        int piecesUsed;

        //Variables for Game Statistics
        int totalGames;
        int redWins;
        int yellowWins;
        double totalTime;
        double avgGameTime;

        //Variable for making sure saving stats only happens once
        int winScreenCount;

        //Variables for Background Music
        Song backgroundMusic;

        //Variables for Sound Effects
        SoundEffect click;
        SoundEffectInstance clickInstance;
        SoundEffect setPiece;
        SoundEffectInstance setPieceInstance;

        public Connect4()
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
            this.graphics.PreferredBackBufferWidth = 800;
            this.graphics.PreferredBackBufferHeight = 600;
            this.graphics.ApplyChanges();
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            this.IsMouseVisible = true;
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

            //Load in menu images
            logo = Content.Load<Texture2D>("Images/Backgrounds/connect4Logo");
            playButton = Content.Load<Texture2D>("Images/Sprites/PlayButton");
            statsButton = Content.Load<Texture2D>("Images/Sprites/StatsButton");
            exitButton = Content.Load<Texture2D>("Images/Sprites/ExitButton");

            //Load in gameplay images
            board = Content.Load<Texture2D>("Images/Sprites/Connect4Board");
            player1 = Content.Load<Texture2D>("Images/Sprites/Player 1");
            player2 = Content.Load<Texture2D>("Images/Sprites/Player 2");

            //Load in win screen images
            menuButton = Content.Load<Texture2D>("Images/Sprites/MenuButton");

            //Load in menu sprite boxes
            logoBox = new Rectangle(10, 10, 400, 200);
            playBox = new Rectangle(screenWidth - 200, 200, 150, 50);
            statsBox = new Rectangle(screenWidth - 200, 275, 150, 50);
            exitBox = new Rectangle(screenWidth - 200, 350, 150, 50);

            //Load in gameplay image boxes
            boardBox = new Rectangle(50, 125, 700, 450);
            player1Box = new Rectangle(boardBox.X + 15, (boardBox.Y - 75), PIECE_WIDTH, PIECE_HEIGHT);
            player2Box = new Rectangle(boardBox.X + 15, (boardBox.Y - 75), PIECE_WIDTH, PIECE_HEIGHT);

            //Load in win screen image boxes
            menuBox = new Rectangle(0, screenHeight - 50, 150, 50);

            //Define locations for snap spaces
            snapSpaces[0] = new Rectangle(boardBox.X, (boardBox.Y - 75), 76, 65);
            snapSpaces[1] = new Rectangle((boardBox.X + (boardBox.Width / 7)), (boardBox.Y - 75), 76, (boardBox.Width / 7));
            snapSpaces[2] = new Rectangle(((boardBox.X + (boardBox.Width / 7)) + (boardBox.Width / 7)), (boardBox.Y - 75), 76, (boardBox.Width / 7));
            snapSpaces[3] = new Rectangle(((boardBox.X + (boardBox.Width / 7)) + (boardBox.Width / 7) * 2), (boardBox.Y - 75), 76, (boardBox.Width / 7));
            snapSpaces[4] = new Rectangle(((boardBox.X + (boardBox.Width / 7)) + (boardBox.Width / 7) * 3), (boardBox.Y - 75), 76, (boardBox.Width / 7));
            snapSpaces[5] = new Rectangle(((boardBox.X + (boardBox.Width / 7)) + (boardBox.Width / 7) * 4), (boardBox.Y - 75), 76, (boardBox.Width / 7));
            snapSpaces[6] = new Rectangle(((boardBox.X + (boardBox.Width / 7)) + (boardBox.Width / 7) * 5), (boardBox.Y - 75), 76, (boardBox.Width / 7));

            //Variables for on-screen message
            messageFont = Content.Load<SpriteFont>("Fonts/MessageFont");
            message = "";

            //Variables for Statistics Font
            statsFont = Content.Load<SpriteFont>("Fonts/StatsFont");
            statsLoc = new Vector2(100, 200);
            statsTitleLoc = new Vector2(0, 0);

            messageLoc = new Vector2(200, 50);

            //Location of "You Won" message
            winLoc = new Vector2(50, 50);

            //Open Stats File and Read in Values
            StreamReader inStats = File.OpenText("stats.txt");
            totalGames = Convert.ToInt32(inStats.ReadLine());
            redWins = Convert.ToInt32(inStats.ReadLine());
            yellowWins = Convert.ToInt32(inStats.ReadLine());
            totalTime = Convert.ToInt32(inStats.ReadLine());
            avgGameTime = Convert.ToDouble(inStats.ReadLine());
            inStats.Close();

            //Load in background music
            backgroundMusic = Content.Load<Song>("Audio/Music/BackgroundMusic");
            MediaPlayer.Volume = 0.1f;

            //Load in Sound Effects
            click = Content.Load<SoundEffect>("Audio/SoundEffects/Click");
            clickInstance = click.CreateInstance();
            clickInstance.Volume = 0.2f;
            setPiece = Content.Load<SoundEffect>("Audio/SoundEffects/SetPiece");
            setPieceInstance = setPiece.CreateInstance();
            setPieceInstance.Volume = 0.2f;

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
            // TODO: Add your update logic here
            prevMouse = mouse;
            mouse = Mouse.GetState();
            if (MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.Play(backgroundMusic);
            }
            switch (GameState)
            {
                //Code for Menu
                case MENU:
                    MenuUpdate();
                    break;
                //Code for Gameplay
                case PLAY:
                    PlayUpdate();
                    break;
                //Code for Running out of Pieces
                case GAMEOVER:
                    GameOverUpdate();
                    break;
                //Code for Player 1 Win
                case WIN1:
                    Win1Update();
                    break;
                //Code for Player 2 Win
                case WIN2:
                    Win2Update();
                    break;
                //Code for Viewing Statistics
                case VIEWSTATS:
                    ViewStatsUpdate();
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
            // TODO: Add your drawing code here
            spriteBatch.Begin();
            switch (GameState)
            {
                //Code for Menu
                case MENU:
                    MenuDraw();
                    break;
                //Code for Gameplay
                case PLAY:
                    PlayDraw();
                    break;
                //Code for Running out of Pieces
                case GAMEOVER:
                    GameOverDraw();
                    break;
                //Code for Player 1 Win
                case WIN1:
                    Win1Draw();
                    break;
                //Code for Player 2 Win
                case WIN2:
                    Win2Draw();
                    break;
                //Code for Viewing Statistics
                case VIEWSTATS:
                    ViewStatsDraw();
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Determines if the mouse clicked the desired button
        /// </summary>
        /// <param name="button">A rectangle for a button that the user can click</param>
        /// <returns>Whether or not the user clicked the button</returns>
        private bool WasMouseClicked(Rectangle button)
        {
            if (mouse.X >= button.X && mouse.X <= (button.X + button.Width) && mouse.Y >= button.Y && mouse.Y <= (button.Y + button.Height) && mouse.LeftButton == ButtonState.Pressed && !(prevMouse.X >= button.X && prevMouse.X <= (button.X + button.Width) && prevMouse.Y >= button.Y && prevMouse.Y <= (button.Y + button.Height) && prevMouse.LeftButton == ButtonState.Pressed))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if the user hovered the mouse over the desired button
        /// </summary>
        /// <param name="button">A rectangle for an area that the user can hover over</param>
        /// <returns>Whether or not the user hovered over that area</returns>
        private bool WasMouseHovered(Rectangle button)
        {
            if (mouse.X >= button.X && mouse.X <= (button.X + button.Width) && mouse.Y >= button.Y && mouse.Y <= (button.Y + button.Height))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Code for drawing items on screen when the user is in the main menu.
        /// </summary>
        private void MenuDraw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Draw(logo, logoBox, Color.White);
            if (WasMouseHovered(playBox) == true)
            {
                spriteBatch.Draw(playButton, playBox, Color.Red);
            }
            else
            {
                spriteBatch.Draw(playButton, playBox, Color.White);
            }
            if (WasMouseHovered(statsBox) == true)
            {
                spriteBatch.Draw(statsButton, statsBox, Color.Red);
            }
            else
            {
                spriteBatch.Draw(statsButton, statsBox, Color.White);
            }
            if (WasMouseHovered(exitBox) == true)
            {
                spriteBatch.Draw(exitButton, exitBox, Color.Red);
            }
            else
            {
                spriteBatch.Draw(exitButton, exitBox, Color.White);
            }
        }

        /// <summary>
        /// Code for logic determining what happens when the user clicks the buttons in the main menu.
        /// </summary>
        private void MenuUpdate()
        {
            if (WasMouseClicked(playBox) == true)
            {
                clickInstance.Play();
                GameState = PLAY;
            }
            if (WasMouseClicked(statsBox) == true)
            {
                clickInstance.Play();
                GameState = VIEWSTATS;
            }
            if (WasMouseClicked(exitBox) == true)
            {
                clickInstance.Play();
                this.Exit();
            }
        }

        /// <summary>
        /// Determines if the mouse is on a space the piece should snap to while determining which column to drop the piece in for player 1.
        /// </summary>
        private void ShouldPieceSnapP1()
        {
            if (WasMouseHovered(snapSpaces[0]) == true)
            {
                player1Box.X = snapSpaces[0].X + 15;
                currentSnapSpace = 0;
            }
            else if (WasMouseHovered(snapSpaces[1]) == true)
            {
                player1Box.X = snapSpaces[1].X + 15;
                currentSnapSpace = 1;
            }
            else if (WasMouseHovered(snapSpaces[2]) == true)
            {
                player1Box.X = snapSpaces[2].X + 15;
                currentSnapSpace = 2;
            }
            else if (WasMouseHovered(snapSpaces[3]) == true)
            {
                player1Box.X = snapSpaces[3].X + 15;
                currentSnapSpace = 3;
            }
            else if (WasMouseHovered(snapSpaces[4]) == true)
            {
                player1Box.X = snapSpaces[4].X + 15;
                currentSnapSpace = 4;
            }
            else if (WasMouseHovered(snapSpaces[5]) == true)
            {
                player1Box.X = snapSpaces[5].X + 15;
                currentSnapSpace = 5;
            }
            else if (WasMouseHovered(snapSpaces[6]) == true)
            {
                player1Box.X = snapSpaces[6].X + 15;
                currentSnapSpace = 6;
            }
        }

        /// <summary>
        /// Determines if the mouse is on a space the piece should snap to while determining which column to drop the piece in for player 2.
        /// </summary>
        private void ShouldPieceSnapP2()
        {
            if (WasMouseHovered(snapSpaces[0]) == true)
            {
                player2Box.X = snapSpaces[0].X + 15;
                currentSnapSpace = 0;
            }
            else if (WasMouseHovered(snapSpaces[1]) == true)
            {
                player2Box.X = snapSpaces[1].X + 15;
                currentSnapSpace = 1;
            }
            else if (WasMouseHovered(snapSpaces[2]) == true)
            {
                player2Box.X = snapSpaces[2].X + 15;
                currentSnapSpace = 2;
            }
            else if (WasMouseHovered(snapSpaces[3]) == true)
            {
                player2Box.X = snapSpaces[3].X + 15;
                currentSnapSpace = 3;
            }
            else if (WasMouseHovered(snapSpaces[4]) == true)
            {
                player2Box.X = snapSpaces[4].X + 15;
                currentSnapSpace = 4;
            }
            else if (WasMouseHovered(snapSpaces[5]) == true)
            {
                player2Box.X = snapSpaces[5].X + 15;
                currentSnapSpace = 5;
            }
            else if (WasMouseHovered(snapSpaces[6]) == true)
            {
                player2Box.X = snapSpaces[6].X + 15;
                currentSnapSpace = 6;
            }
        }

        /// <summary>
        /// Moves the current player's piece based on the column they are currently in
        /// </summary>
        private void MovePiece()
        {
            if (player1Turn == true)
            {
                if (WasMouseClicked(player1Box) == true)
                {
                    setPieceInstance.Play();
                    if (boardArray[5, currentSnapSpace] == 0)
                    {
                        boardArray[5, currentSnapSpace] = 1;
                        player1Turn = false;
                        piecesUsed++;
                    }
                    else if (boardArray[4, currentSnapSpace] == 0)
                    {
                        boardArray[4, currentSnapSpace] = 1;
                        player1Turn = false;
                        piecesUsed++;
                    }
                    else if (boardArray[3, currentSnapSpace] == 0)
                    {
                        boardArray[3, currentSnapSpace] = 1;
                        player1Turn = false;
                        piecesUsed++;
                    }
                    else if (boardArray[2, currentSnapSpace] == 0)
                    {
                        boardArray[2, currentSnapSpace] = 1;
                        player1Turn = false;
                        piecesUsed++;
                    }
                    else if (boardArray[1, currentSnapSpace] == 0)
                    {
                        boardArray[1, currentSnapSpace] = 1;
                        player1Turn = false;
                        piecesUsed++;
                    }
                    else if (boardArray[0, currentSnapSpace] == 0)
                    {
                        boardArray[0, currentSnapSpace] = 1;
                        player1Turn = false;
                        piecesUsed++;
                    }
                }
            }
            else
            {
                if (WasMouseClicked(player2Box) == true)
                {
                    setPieceInstance.Play();
                    if (boardArray[5, currentSnapSpace] == 0)
                    {
                        boardArray[5, currentSnapSpace] = 2;
                        player1Turn = true;
                        piecesUsed++;
                    }
                    else if (boardArray[4, currentSnapSpace] == 0)
                    {
                        boardArray[4, currentSnapSpace] = 2;
                        player1Turn = true;
                        piecesUsed++;
                    }
                    else if (boardArray[3, currentSnapSpace] == 0)
                    {
                        boardArray[3, currentSnapSpace] = 2;
                        player1Turn = true;
                        piecesUsed++;
                    }
                    else if (boardArray[2, currentSnapSpace] == 0)
                    {
                        boardArray[2, currentSnapSpace] = 2;
                        player1Turn = true;
                        piecesUsed++;
                    }
                    else if (boardArray[1, currentSnapSpace] == 0)
                    {
                        boardArray[1, currentSnapSpace] = 2;
                        player1Turn = true;
                        piecesUsed++;
                    }
                    else if (boardArray[0, currentSnapSpace] == 0)
                    {
                        boardArray[0, currentSnapSpace] = 2;
                        player1Turn = true;
                        piecesUsed++;
                    }
                }
            }
        }

        /// <summary>
        /// Draws all pieces on the board that have been already placed
        /// </summary>
        private void DrawBoardPieces()
        {
            for (int i = 0; i <= 5; i++)
            {
                for (int j = 0; j <= 6; j++)
                {
                    if (boardArray[i, j] == 1)
                    {
                        spriteBatch.Draw(player1, new Rectangle((boardBox.Width / 7) + ((boardBox.Width / 7) * j) - (PIECE_WIDTH / 2) - (int)(1.5 * j) + BORDER_WIDTH, (boardBox.Height / 5) + ((boardBox.Height / 5) * i) - (PIECE_HEIGHT / 2) + (int)(15 * DetermineAdditionAmount(i)) - BORDER_HEIGHT, PIECE_WIDTH, PIECE_HEIGHT), Color.White);
                    }
                    else if (boardArray[i, j] == 2)
                    {
                        spriteBatch.Draw(player2, new Rectangle((boardBox.Width / 7) + ((boardBox.Width / 7) * j) - (PIECE_WIDTH / 2) - (int)(1.5 * j) + BORDER_WIDTH, (boardBox.Height / 5) + ((boardBox.Height / 5) * i) - (PIECE_HEIGHT / 2) + (int)(15 * DetermineAdditionAmount(i)) - BORDER_HEIGHT, PIECE_WIDTH, PIECE_HEIGHT), Color.White);
                    }
                }
            }
        }

        /// <summary>
        /// Determines the amount to add to each piece's vertical position based on the current row number.
        /// </summary>
        /// <param name="columnNum">The current row number the player is trying to put a piece into</param>
        /// <returns>What 15 should be multiplied by in order for the piece to be placed correctly.</returns>
        private int DetermineAdditionAmount(int columnNum)
        {
            if (columnNum == 5)
            {
                return 0;
            }
            else if (columnNum == 4)
            {
                return 1;
            }
            else if (columnNum == 3)
            {
                return 2;
            }
            else if (columnNum == 2)
            {
                return 3;
            }
            else if (columnNum == 1)
            {
                return 4;
            }
            else
            {
                return 5;
            }
        }

        /// <summary>
        /// Determines who will start at the beginning of a new game
        /// </summary>
        private void DeterminePlayerTurn()
        {
            if (gameCount == 0)
            {
                player1Turn = Convert.ToBoolean(rng.Next(0, 2));
            }
            if (gameCount <= 120 && player1Turn == true)
            {
                message = "Player 1 will start.";
            }
            else if (gameCount <= 120)
            {
                message = "Player 2 will start.";
            }
            else
            {
                message = "";
            }
            gameCount++;
        }

        /// <summary>
        /// Checks if a player has won
        /// </summary>
        /// <param name="playerNum">The number of the current player</param>
        /// <returns>Whether or not a player has won, or if the board is full</returns>
        private int CheckWinner(int playerNum)
        {
            for (int r = 0; r <= 5; r++)
            {
                for (int c = 0; c <= 6; c++)
                {
                    //r = row number, c = column number
                    //Check for horizontal win
                    if (boardArray[r, c] == playerNum && c <= 3)
                    {
                        if (boardArray[r, c + 1] == playerNum)
                        {
                            if (boardArray[r, c + 2] == playerNum)
                            {
                                if (boardArray[r, c + 3] == playerNum)
                                {
                                    return playerNum;
                                }
                            }
                        }
                    }
                    //Check for vertical win
                    if (boardArray[r, c] == playerNum && r <= 2)
                    {
                        if (boardArray[r + 1, c] == playerNum)
                        {
                            if (boardArray[r + 2, c] == playerNum)
                            {
                                if (boardArray[r + 3, c] == playerNum)
                                {
                                    return playerNum;
                                }
                            }
                        }
                    }
                    //Check for diagonal win
                    if (boardArray[r, c] == playerNum && c <= 3 && r <= 2)
                    {
                        if (boardArray[r + 1, c + 1] == playerNum)
                        {
                            if (boardArray[r + 2, c + 2] == playerNum)
                            {
                                if (boardArray[r + 3, c + 3] == playerNum)
                                {
                                    return playerNum;
                                }
                            }
                        }
                        if (r <= 2 && c >= 3)
                        {
                            if (boardArray[r + 1, c - 1] == playerNum)
                            {
                                if (boardArray[r + 2, c - 2] == playerNum)
                                {
                                    if (boardArray[r + 3, c - 3] == playerNum)
                                    {
                                        return playerNum;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //If no one has won, check if the boardArray is full and set values accordingly
            if (piecesUsed >= 42)
            {
                return -2;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Calculates the average time per game of Connect4
        /// </summary>
        /// <param name="time">The total time of all games played</param>
        /// <param name="games">The total number of games played</param>
        /// <returns>The average time per game</returns>
        private double CalcAvgTime(double time, int games)
        {
            double timeInMins = (double)((time / 60) / 60);
            return Math.Round((timeInMins / Convert.ToDouble(games)), 2);
        }

        /// <summary>
        /// Update Code for the Gameplay Screen
        /// </summary>
        private void PlayUpdate()
        {
            DeterminePlayerTurn();
            if (gameCount > 120)
            {
                if (player1Turn == true)
                {
                    ShouldPieceSnapP1();
                    MovePiece();
                    //Checks if player 1 has won the game or if the game board is full
                    if (CheckWinner(1) == 1)
                    {
                        GameState = WIN1;
                    }
                    else if (CheckWinner(1) == -2)
                    {
                        GameState = GAMEOVER;
                    }
                }
                else
                {
                    ShouldPieceSnapP2();
                    MovePiece();
                    //Checks if player 2 has won the game or if the game board is full
                    if (CheckWinner(2) == 2)
                    {
                        GameState = WIN2;
                    }
                    else if (CheckWinner(1) == -2)
                    {
                        GameState = GAMEOVER;
                    }
                }
            }
        }

        /// <summary>
        /// Draw Code for Gameplay Screen
        /// </summary>
        private void PlayDraw()
        {
            GraphicsDevice.Clear(Color.SandyBrown);
            spriteBatch.Draw(board, boardBox, Color.White);
            if (player1Turn == true)
            {
                spriteBatch.Draw(player1, player1Box, Color.White);
            }
            else
            {
                spriteBatch.Draw(player2, player2Box, Color.White);
            }
            spriteBatch.DrawString(messageFont, message, messageLoc, Color.White);
            DrawBoardPieces();
        }

        /// <summary>
        /// Running out of pieces update code
        /// </summary>
        private void GameOverUpdate()
        {
            if (winScreenCount == 0)
            {
                totalGames++;
                totalTime += gameCount;
                avgGameTime = CalcAvgTime(totalTime, totalGames);
                StreamWriter outStats = File.CreateText("stats.txt");
                outStats.WriteLine(totalGames);
                outStats.WriteLine(redWins);
                outStats.WriteLine(yellowWins);
                outStats.WriteLine(totalTime);
                outStats.WriteLine(avgGameTime);
                outStats.Close();
            }
            winScreenCount++;
            //Reset Game
            if (WasMouseClicked(menuBox) == true)
            {
                clickInstance.Play();
                for (int i = 0; i <= 5; i++)
                {
                    for (int j = 0; j <= 6; j++)
                    {
                        boardArray[i, j] = 0;
                    }
                }
                piecesUsed = 0;
                gameCount = 0;
                GameState = MENU;
                player1Box.X = boardBox.X + 15;
                player2Box.X = boardBox.X + 15;
                winScreenCount = 0;
            }
        }

        /// <summary>
        /// Running out of pieces draw code
        /// </summary>
        private void GameOverDraw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.DrawString(messageFont, "GAME OVER\nAll pieces have been used.", winLoc, Color.White);
            if (WasMouseHovered(menuBox) == true)
            {
                spriteBatch.Draw(menuButton, menuBox, Color.Red);
            }
            else
            {
                spriteBatch.Draw(menuButton, menuBox, Color.White);
            }
        }

        /// <summary>
        /// Update code for player 1 win screen
        /// </summary>
        private void Win1Update()
        {
            if (winScreenCount == 0)
            {
                totalGames++;
                redWins++;
                totalTime += gameCount;
                avgGameTime = CalcAvgTime(totalTime, totalGames);
                StreamWriter outStats = File.CreateText("stats.txt");
                outStats.WriteLine(totalGames);
                outStats.WriteLine(redWins);
                outStats.WriteLine(yellowWins);
                outStats.WriteLine(totalTime);
                outStats.WriteLine(avgGameTime);
                outStats.Close();
            }
            winScreenCount++;
            //Reset Game
            if (WasMouseClicked(menuBox) == true)
            {
                clickInstance.Play();
                for (int i = 0; i <= 5; i++)
                {
                    for (int j = 0; j <= 6; j++)
                    {
                        boardArray[i, j] = 0;
                    }
                }
                piecesUsed = 0;
                gameCount = 0;
                GameState = MENU;
                player1Box.X = boardBox.X + 15;
                player2Box.X = boardBox.X + 15;
                winScreenCount = 0;
            }
        }

        /// <summary>
        /// Draw code for Player 1 winning screen
        /// </summary>
        private void Win1Draw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.DrawString(messageFont, "Congratulations,\nPlayer 1 has won the game!", winLoc, Color.White);
            if (WasMouseHovered(menuBox) == true)
            {
                spriteBatch.Draw(menuButton, menuBox, Color.Red);
            }
            else
            {
                spriteBatch.Draw(menuButton, menuBox, Color.White);
            }
        }

        /// <summary>
        /// Update code for Player 2 winning screen
        /// </summary>
        private void Win2Update()
        {
            if (winScreenCount == 0)
            {
                totalGames++;
                yellowWins++;
                totalTime += gameCount;
                avgGameTime = CalcAvgTime(totalTime, totalGames);
                StreamWriter outStats = File.CreateText("stats.txt");
                outStats.WriteLine(totalGames);
                outStats.WriteLine(redWins);
                outStats.WriteLine(yellowWins);
                outStats.WriteLine(totalTime);
                outStats.WriteLine(avgGameTime);
                outStats.Close();
            }
            winScreenCount++;
            //Reset Game
            if (WasMouseClicked(menuBox) == true)
            {
                clickInstance.Play();
                for (int i = 0; i <= 5; i++)
                {
                    for (int j = 0; j <= 6; j++)
                    {
                        boardArray[i, j] = 0;
                    }
                }
                piecesUsed = 0;
                gameCount = 0;
                GameState = MENU;
                player1Box.X = boardBox.X + 15;
                player2Box.X = boardBox.X + 15;
                winScreenCount = 0;
            }
        }

        //Draw code for Player 2 winning screen
        private void Win2Draw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.DrawString(messageFont, "Congratulations,\nPlayer 2 has won the game!", winLoc, Color.White);
            if (WasMouseHovered(menuBox) == true)
            {
                spriteBatch.Draw(menuButton, menuBox, Color.Red);
            }
            else
            {
                spriteBatch.Draw(menuButton, menuBox, Color.White);
            }
        }

        /// <summary>
        /// Update code for statistics screen
        /// </summary>
        private void ViewStatsUpdate()
        {
            if (WasMouseClicked(menuBox) == true)
            {
                clickInstance.Play();
                GameState = MENU;
            }
        }

        /// <summary>
        /// Draw code for statistics screen
        /// </summary>
        private void ViewStatsDraw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (WasMouseHovered(menuBox) == true)
            {
                spriteBatch.Draw(menuButton, menuBox, Color.Red);
            }
            else
            {
                spriteBatch.Draw(menuButton, menuBox, Color.White);
            }
            spriteBatch.DrawString(messageFont, "Statistics:", statsTitleLoc, Color.White);
            spriteBatch.DrawString(statsFont, "Total # of Games Played: " + totalGames + "\nTotal Red Wins: " + redWins + "\nTotal Yellow Wins: " + yellowWins + "\nAverage Game Time: " + avgGameTime + " Minute(s)", statsLoc, Color.White);
        }
    }
}
