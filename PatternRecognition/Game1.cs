﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace PatternRecognition
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState kb, oldkb;
        MouseState mouse, oldmouse;
        Point mousePos;
        SpriteFont testFont;
        SpriteFont titleFont;
        Random rand = new Random();

        //Character player, playerCopyTopAndBottom, playerCopyLeftAndRight;
        Character playButton;
        Character[,] board;
        //Character[,] solutionBoard;
        //Character[,] tokens;
        int playerWidth = 64, playerHeight = 64;
        int sameDim = 10;

        int boardXDim = 9, boardYDim = 9;

        bool isInvertingPattern = false;
        bool isMirroringLeftRight = false;

        int patternChoice = 1;

        int screenWidth = 720;
        int screenHeight = 720;

        int gameClock = 0;
        int amountToShift = 0;
        int revealedCount = 0;
        int numChanged = 0;

        int pointsToSpend = 30;
        int startingPointsToSpend = 30;

        bool isDebugEnabled = false;
        bool startButtonSelected = false;

        enum tools
        {
            paintCan = 0, plus, xTool, single
        }

        tools selectedTool = tools.paintCan;

        bool shouldCalcRedraw = false;

        bool hasDoneOneTimeCode = false;

        Color selectedColor = Color.Blue;

        enum gameState
        {
            titleScreen, gameplay, lose,
        }

        gameState state = gameState.titleScreen;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.graphics.PreferredBackBufferHeight = screenHeight;
            this.graphics.PreferredBackBufferWidth = screenWidth;
            this.IsMouseVisible = true;

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

            testFont = Content.Load<SpriteFont>("testFont");
            titleFont = Content.Load<SpriteFont>("titleAndEndFont");

            board = new Character[boardYDim, boardXDim];
            playButton = new Character(Content.Load<Texture2D>("buttonOutline"),
                        new Rectangle(screenWidth / 2 - 100, 250,
                        200, 90));

            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    board[y, x] = new Character(Content.Load<Texture2D>("buttonOutline"),
                        new Rectangle(x * (screenWidth / boardXDim), y * (screenHeight / boardYDim),
                        (screenWidth / boardXDim), (screenHeight / boardYDim)));
                }
            }

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
            kb = Keyboard.GetState();
            mouse = Mouse.GetState();
            mousePos.X = mouse.X;
            mousePos.Y = mouse.Y;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (state)
            {
                case gameState.titleScreen:
                    titleScreen();
                    break;
                case gameState.gameplay:
                    gameplay();
                    break;
                case gameState.lose:
                    lose();
                    break;
            }

            oldmouse = mouse;
            oldkb = kb;
            gameClock++;
            base.Update(gameTime);
        }

        public void titleScreen()
        {
            if ((playButton.getRec()).Contains((mousePos)) || kb.IsKeyDown(Keys.Enter))
            {
                startButtonSelected = true;

                if (mouse.LeftButton == ButtonState.Pressed || kb.IsKeyDown(Keys.Enter))
                {
                    state = gameState.gameplay;
                }

            }
            else
            {
                startButtonSelected = false;
            }
        }

        public void startOfGameCode()
        {
            if (hasDoneOneTimeCode == false)
            {
                generateLevel();

                hasDoneOneTimeCode = true;
            }
        }

        public void generateLevel()
        {
            shouldCalcRedraw = true;
            patternChoice = rand.Next(1, 22);
            makePatterns();

            int rotateChanceInX = rand.Next(1, 4);
            if (rotateChanceInX == 1)
            {

                int rotateCount = rand.Next(1, 4);
                for (int i = 0; i < rotateCount; i++)
                {
                    rotateClockwise();
                }
            }

            int invertChanceInX = rand.Next(1, 6);
            if(invertChanceInX == 1)
            {
                invertPattern();
            }

            int otherChance = rand.Next(1, 9);
            if(otherChance == 5)
            {
                mirrorLeftAndRight();
            }
            else if(otherChance == 6)
            {
                mirrorOnDiagonal();
            }
            else if (otherChance == 7)
            {
                mirrorUpAndDown();
            }
        }

        public void clearPattern()
        {
            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    board[y, x].isHighlighted = false;
                    board[y, x].isUserHighlighted = false;
                }
            }
        }

        public void mirrorLeftAndRight()
        {
            bool[,] tempBoard = new bool[boardYDim, boardXDim];

            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    tempBoard[y, x] = board[y, x].isHighlighted;
                }
            }

            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    board[y, x].isHighlighted = tempBoard[y, boardXDim - x - 1];
                }
            }
        }

        public void mirrorUpAndDown()
        {
            bool[,] tempBoard = new bool[boardYDim, boardXDim];

            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    tempBoard[y, x] = board[y, x].isHighlighted;
                }
            }

            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    board[y, x].isHighlighted = tempBoard[boardYDim - y - 1, x];
                }
            }
        }

        public void mirrorOnDiagonal()
        {
            bool[,] tempBoard = new bool[boardYDim, boardXDim];

            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    tempBoard[y, x] = board[y, x].isHighlighted;
                }
            }

            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    board[y, x].isHighlighted = tempBoard[x, y];
                }
            }
        }

        public void rotateClockwise()
        {
            bool[,] tempBoard = new bool[boardYDim, boardXDim];

            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    tempBoard[y, x] = board[y, x].isHighlighted;
                }
            }

            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    board[x, (boardYDim - 1 - y)].isHighlighted = tempBoard[y, x];
                }
            }

        }

        public void invertPattern()
        {
            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    board[y, x].isHighlighted = !board[y, x].isHighlighted;
                }
            }
        }

        public void makePatterns()
        {
            //patternChoice = rand.Next(1, 2);
            if (shouldCalcRedraw == false)
                return;
            clearPattern();
            switch (patternChoice)
            {
                case 1:
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (x % 2 == 0 && y % 2 == 0)
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 2:
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if ((x + y) % 2 == 0)
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 3:
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (x % 2 == 0 || y % 2 == 0)
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 4:
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (Math.Abs(x - y) > (int)Math.Sqrt(boardXDim))
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 5:
                    amountToShift = rand.Next(1, (int)Math.Ceiling((double)boardXDim / 2));
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (x >= amountToShift && x <= boardXDim - 1 - amountToShift)
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 6:
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (x == y)
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 7:
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (x == (y | 1))
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 8:
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (y == (x | 1))
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 9:
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if ((x & 1) == y || (y & 1) == x)
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 10:
                    amountToShift = rand.Next(0, 2);
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (x % 2 == amountToShift)
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 11:
                    amountToShift = rand.Next(0, 3);
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if ((x + amountToShift) % 3 == 0)
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 12:
                    //amountToShift = rand.Next(3, 5);
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (x % 4 == 0)
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 13:
                    amountToShift = rand.Next(1, boardXDim / 2);
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (x == boardXDim / 2 + amountToShift || y == boardYDim / 2 + amountToShift ||
                                x == boardXDim / 2 - amountToShift || y == boardYDim / 2 - amountToShift)
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 14:
                    amountToShift = rand.Next(1, (int)Math.Ceiling((double)boardXDim / 2));
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if ((x == boardXDim / 2 + amountToShift || y == boardYDim / 2 + amountToShift ||
                                x == boardXDim / 2 - amountToShift || y == boardYDim / 2 - amountToShift) &&
                                ((x >= boardXDim / 2 - amountToShift && x <= boardXDim / 2 + amountToShift) &&
                                (y >= boardYDim / 2 - amountToShift && y <= boardYDim / 2 + amountToShift)))
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 15:
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (x > boardXDim / 2 && y > boardXDim / 2)
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 16:
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (Math.Abs(x - y) > boardXDim / 2 || x + y < boardXDim / 2 || x + y > boardXDim / 3 * 4)
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 17:
                    int xPos = rand.Next(1, boardXDim - 1);
                    int yPos = 0;
                    int xAddAmount = 1;
                    int yAddAmount = 1;
                    for (int i = 0; i < 16; i++)
                    {
                        board[yPos, xPos].isHighlighted = !board[yPos, xPos].isHighlighted;
                        xPos += xAddAmount;
                        yPos += yAddAmount;
                        if (xPos == boardXDim - 1 || xPos == 0)
                        {
                            xAddAmount *= -1;
                        }
                        if (yPos == boardYDim - 1 || yPos == 0)
                        {
                            yAddAmount *= -1;
                        }

                    }
                    break;
                case 18:
                    amountToShift = rand.Next(2, 5);
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (x % amountToShift == 0 && y % amountToShift == 0)
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 19:
                    amountToShift = rand.Next(0, boardXDim);
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (x == amountToShift || y == amountToShift)
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 20:
                    amountToShift = rand.Next(2, 6);
                    int startX = rand.Next(0, 1);
                    for (int x = startX; x < boardXDim; x += 1)
                    {
                        board[x * amountToShift % boardYDim, x].isHighlighted = !board[x * amountToShift % boardYDim, x].isHighlighted;

                    }
                    break;
                case 21:
                    amountToShift = rand.Next(2, 5);
                    startX = rand.Next(0, 1);
                    yPos = 0;
                    bool isCheckingOdds = false;
                    bool shouldFlip = false;

                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (y % amountToShift == 0)
                            {
                                if (isCheckingOdds)
                                {
                                    if (x % 2 == 1)
                                    {
                                        board[y, x].isHighlighted = !board[y, x].isHighlighted;
                                        shouldFlip = true;
                                    }
                                }
                                else
                                {
                                    if (x % 2 == 0)
                                    {
                                        board[y, x].isHighlighted = !board[y, x].isHighlighted;
                                        shouldFlip = true;

                                    }
                                }
                            }
                        }
                        if (shouldFlip)
                        {
                            isCheckingOdds = !isCheckingOdds;
                            shouldFlip = false;
                        }
                    }
                    break;
            }
            shouldCalcRedraw = false;


        }

        public void lose()
        {

        }

        public void gameplay()
        {
            startOfGameCode();


            userControls();

            makePatterns();
            if (isInvertingPattern)
            {
                invertPattern();
                isInvertingPattern = false;
            }

            if (isMirroringLeftRight)
            {
                mirrorLeftAndRight();
                isMirroringLeftRight = false;
            }

        }

        public void userControls()
        {
            for (int i = 49; i < 58; i++)
            {
                if (kb.IsKeyDown((Keys)(i)))
                {
                    patternChoice = i - 48;
                    shouldCalcRedraw = true;
                }
            }

            //if (kb.IsKeyDown(Keys.D7))
            //{
            //    selectedColor = Color.re;
            //}

            revealedCount = 0;
            numChanged = 0;
            if (mouse.LeftButton == ButtonState.Pressed && oldmouse.LeftButton == ButtonState.Released)
            {
                for (int y = 0; y < boardYDim; y++)
                {
                    for (int x = 0; x < boardXDim; x++)
                    {
                        if (board[y, x].getRec().Contains(mousePos))
                        {
                            if (selectedTool == tools.paintCan)
                            {
                                for (int y2 = y - 1; y2 < y + 2; y2++)
                                {
                                    for (int x2 = x - 1; x2 < x + 2; x2++)
                                    {
                                        if (board[y2, x2].isHighlighted && board[y2, x2].isUserHighlighted == false)
                                        {
                                            revealedCount += 1;
                                        }
                                        if(board[y2, x2].isUserHighlighted == false)
                                        {
                                            board[y2, x2].isUserHighlighted = true;
                                            numChanged += 1;
                                        }
                                    }
                                }
                                pointsToSpend -= (int)(6 * ((double)numChanged / 9));
                                pointsToSpend += revealedCount;
                                y = boardYDim;
                                x = boardXDim;
                            }
                            else if (selectedTool == tools.plus)
                            {
                                for (int y2 = y - 1; y2 < y + 2; y2++)
                                {
                                    if (board[y2, x].isHighlighted && board[y2, x].isUserHighlighted == false)
                                    {
                                        revealedCount += 1;
                                    }
                                    if (board[y2, x].isUserHighlighted == false)
                                    {
                                        board[y2, x].isUserHighlighted = true;
                                        numChanged += 1;
                                    }
                                }

                                for (int x2 = x - 1; x2 < x + 2; x2++)
                                {
                                    if (board[y, x2].isHighlighted && board[y, x2].isUserHighlighted == false)
                                    {
                                        revealedCount += 1;
                                    }
                                    if (board[y, x2].isUserHighlighted == false)
                                    {
                                        board[y, x2].isUserHighlighted = true;
                                        numChanged += 1;
                                    }

                                }

                                pointsToSpend -= (int)(4 * ((double)numChanged / 5));
                                pointsToSpend += revealedCount;
                                y = boardYDim;
                                x = boardXDim;
                            }
                            else if (selectedTool == tools.xTool)
                            {

                                int firstPoint = ((y - 1) + (x - 1)) % 2;
                                for (int y2 = y - 1; y2 < y + 2; y2++)
                                {
                                    for (int x2 = x - 1; x2 < x + 2; x2++)
                                    {
                                        if ((y2 + x2) % 2 == firstPoint)
                                        {
                                            if (board[y2, x2].isHighlighted && board[y2, x2].isUserHighlighted == false)
                                            {
                                                revealedCount += 1;
                                            }
                                            if (board[y2, x2].isUserHighlighted == false)
                                            {
                                                board[y2, x2].isUserHighlighted = true;
                                                numChanged += 1;
                                            }
                                        }
                                    }
                                }

                                pointsToSpend -= (int)(4 * ((double)numChanged / 5));
                                pointsToSpend += revealedCount;
                                y = boardYDim;
                                x = boardXDim;
                            }
                            else if (selectedTool == tools.single)
                            {
                                if (board[y, x].isHighlighted && board[y, x].isUserHighlighted == false)
                                {
                                    revealedCount += 1;
                                }
                                if (board[y, x].isUserHighlighted == false)
                                {
                                    board[y, x].isUserHighlighted = true;
                                    numChanged += 1;
                                }
                                pointsToSpend -= 1 * numChanged;
                                pointsToSpend += revealedCount;
                                y = boardYDim;
                                x = boardXDim;
                            }

                            //board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }

                    }
                }
            }
            if(pointsToSpend <= 0)
            {
                state = gameState.lose;
            }
            //pointsToSpend -= changedCount;

            if (kb.IsKeyDown(Keys.LeftShift) && kb.IsKeyDown(Keys.LeftControl)
                && kb.IsKeyDown(Keys.P) && oldkb.IsKeyUp(Keys.P))
            {
                isDebugEnabled = !isDebugEnabled;
            }

            if (kb.IsKeyDown(Keys.W) && oldkb.IsKeyUp(Keys.W))
            {
                selectedTool = tools.paintCan;
            }

            if (kb.IsKeyDown(Keys.A) && oldkb.IsKeyUp(Keys.A))
            {
                selectedTool = tools.plus;
            }

            if (kb.IsKeyDown(Keys.S) && oldkb.IsKeyUp(Keys.S))
            {
                selectedTool = tools.xTool;
            }
            if (kb.IsKeyDown(Keys.D) && oldkb.IsKeyUp(Keys.D))
            {
                selectedTool = tools.single;
            }

            if (kb.IsKeyDown(Keys.E) && oldkb.IsKeyUp(Keys.E))
            {
                selectedTool = (tools)((int)(selectedTool + 1) % (Enum.GetNames(typeof(tools)).Length));
            }

            if (kb.IsKeyDown(Keys.R) && oldkb.IsKeyUp(Keys.R))
            {

            }

            if (isDebugEnabled)
            {


                if (kb.IsKeyDown(Keys.K) && oldkb.IsKeyUp(Keys.K))
                {
                    rotateClockwise();
                }

                if (kb.IsKeyDown(Keys.N) && oldkb.IsKeyUp(Keys.N))
                {
                    mirrorUpAndDown();
                }
                if (kb.IsKeyDown(Keys.M) && oldkb.IsKeyUp(Keys.M))
                {
                    mirrorLeftAndRight();
                }
                if (kb.IsKeyDown(Keys.C) && oldkb.IsKeyUp(Keys.C))
                {
                    isInvertingPattern = !isInvertingPattern;
                }
                if (kb.IsKeyDown(Keys.X) && oldkb.IsKeyUp(Keys.X))
                {
                    patternChoice = 20;
                    shouldCalcRedraw = true;
                }
                if (kb.IsKeyDown(Keys.B) && oldkb.IsKeyUp(Keys.B))
                {
                    shouldCalcRedraw = true;
                }
                if (kb.IsKeyDown(Keys.V) && oldkb.IsKeyUp(Keys.V))
                {
                    if (kb.IsKeyDown(Keys.LeftShift))
                        patternChoice -= 1;
                    else
                        patternChoice += 1;

                    shouldCalcRedraw = true;

                }
            }

            if (kb.IsKeyDown(Keys.U) && oldkb.IsKeyUp(Keys.U))
            {

            }

        }

        public void reset()
        {

        }

        public void checkWins()
        {

        }

        public string WrapText(SpriteFont spriteFont, float maxLineWidth, string text)
        {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = spriteFont.MeasureString(word);

                if (word.Contains("\n"))
                {
                    string replace = word.Replace("\n", "\n\n");
                    lineWidth = size.X + spaceWidth;
                    sb.Append(replace + " ");
                }
                else
                {


                    if (lineWidth + size.X < maxLineWidth)
                    {
                        sb.Append(word + " ");
                        lineWidth += size.X + spaceWidth;
                    }
                    else
                    {
                        sb.Append("\n" + word + " ");
                        lineWidth = size.X + spaceWidth;
                    }

                }
            }

            return sb.ToString();
        }

        public void drawTitleScreen()
        {
            if(startButtonSelected)
            {
            playButton.drawCharacter(spriteBatch, Color.Red);

            }
            else
            {
                playButton.drawCharacter(spriteBatch);

            }
            spriteBatch.DrawString(titleFont, "Play",
                        new Vector2(screenWidth / 2, playButton.getRec().Bottom - (playButton.getRec().Height * 2 / 4))
                        - titleFont.MeasureString("Play") / 2, Color.Black);

            spriteBatch.DrawString(titleFont, "Pattern Painter",
                        new Vector2(screenWidth / 2, 100) - titleFont.MeasureString("Pattern Painter") / 2, Color.Black);

            spriteBatch.DrawString(testFont, WrapText(testFont, playButton.getRec().Width * 2,
                "Use the WASD keys to select a tool, or E to cycle. Click a tile to use a tool there.\n" +
                "The paint bucket fills in a 3 by 3 area centered on the tile " +
                "clicked (cost 6). The plus makes a plus shape of 5 tiles, " +
                "and similarly the xTool is an X shape (both cost 4)." +
                " Single is only paints the tile clicked (cost 1). \nA green tile means " +
                "that there was nothing there. A light blue tile means that there was treasure" +
                " on that tile. The treasure always spawns a in pattern of some kind."),
                        new Vector2(playButton.getRec().X - playButton.getRec().Width / 2, playButton.getRec().Bottom + 20), Color.Black);
        }

        public void drawGameplay()
        {
            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    if (board[y, x].isHighlighted)
                    {
                        if (board[y, x].isUserHighlighted)
                        {
                            board[y, x].drawCharacter(spriteBatch, Color.Cyan);
                        }
                        else
                        {
                            if (isDebugEnabled)
                                board[y, x].drawCharacter(spriteBatch, selectedColor);
                            else
                                board[y, x].drawCharacter(spriteBatch);
                        }
                    }
                    else
                    {
                        if (board[y, x].isUserHighlighted)
                        {
                            board[y, x].drawCharacter(spriteBatch, Color.Green);
                        }
                        else
                        {
                            board[y, x].drawCharacter(spriteBatch);

                        }
                    }
                    if (isDebugEnabled)
                    {

                        spriteBatch.DrawString(testFont, "Y: " + y + "\nX: " + x,
                            new Vector2(board[y, x].getRec().Center.X -
                            (board[y, x].getRec().Width / 4), board[y, x].getRec().Center.Y -
                            (board[y, x].getRec().Height / 3)), Color.Red);
                    }
                }
            }
            if (isDebugEnabled)
            {

                spriteBatch.DrawString(testFont, patternChoice.ToString(),
                            new Vector2(10, 10), Color.Red);
            }
            spriteBatch.DrawString(testFont, "Points to\nSpend: " + pointsToSpend.ToString(),
                        new Vector2(0, 15), Color.Red);
            spriteBatch.DrawString(testFont, "  Tool:\n" + selectedTool.ToString(),
                        new Vector2(5, 90), Color.Red);

        }

        public void drawLose()
        {

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();

            switch (state)
            {
                case gameState.titleScreen:
                    drawTitleScreen();
                    break;
                case gameState.gameplay:
                    drawGameplay();
                    break;
                case gameState.lose:
                    drawLose();
                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
