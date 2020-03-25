using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Linq;
using System;

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
        Random rand = new Random();

        //Character player, playerCopyTopAndBottom, playerCopyLeftAndRight;
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
        int changedCount = 0;

        int pointsToSpend = 30;

        enum tools
        {
            paintCan = 1, plus, cross, single 
        }

        tools selectedTool = tools.paintCan;

        bool shouldCalcRedraw = true;

        bool hasDoneOneTimeCode = false;

        Color selectedColor = Color.Blue;

        enum gameState
        {
            titleScreen, gameplay, lose,
        }

        gameState state = gameState.gameplay;

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

            board = new Character[boardYDim, boardXDim];

            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    board[y, x] = new Character(Content.Load<Texture2D>("buttonOutline"),
                        new Rectangle(x * (screenWidth / boardXDim), y * (screenHeight / boardYDim), (screenWidth / boardXDim), (screenHeight / boardYDim)));
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

        }

        public void startOfGameCode()
        {
            //if (hasDoneOneTimeCode == false)
            //{
            //    makePatterns();
            //    hasDoneOneTimeCode = true;
            //}
        }

        public void clearPattern()
        {
            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    board[y, x].isHighlighted = false;
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
                    board[y, x].isHighlighted = tempBoard[y , boardXDim - x -1 ];
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
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (Math.Abs(x - boardXDim / 2) <= boardXDim / 3)
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
                            if (((boardXDim - x) & 1) == (boardYDim - y - 1) || ((boardYDim - y) & 1) == (boardXDim - x - 1))
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 10:
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if ((x & 1) == y || (y & 1) == x)
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 11:
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (x == (x | 1))
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 12:
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
                case 13:
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
                case 16:
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (x > boardXDim / 2 && y > boardXDim / 2)
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 14:
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (Math.Abs(x - y) > boardXDim / 2 || x + y < boardXDim / 2 || x + y > boardXDim / 3 * 4)
                                board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                    }
                    break;
                case 15:
                    int xPos = rand.Next(1, boardXDim - 1);
                    int yPos = 0;
                    int xAddAmount = 1;
                    int yAddAmount = 1;
                    for (int i = 0; i < 16; i++)
                    {
                        board[yPos, xPos].isHighlighted = !board[yPos, xPos].isHighlighted;
                        xPos+= xAddAmount;
                        yPos+= yAddAmount;
                        if(xPos == boardXDim - 1 || xPos == 0)
                        {
                            xAddAmount *= -1;
                        }
                        if (yPos == boardYDim - 1 || yPos == 0)
                        {
                            yAddAmount *= -1;
                        }

                    }
                    break;
                case 17:
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
                case 18:
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
                case 19:
                    amountToShift = rand.Next(2, 6);
                    int startX = rand.Next(0, 1);
                    for (int x = startX; x < boardXDim; x+= 1)
                    {
                        board[x * amountToShift % boardYDim, x].isHighlighted = !board[x * amountToShift % boardYDim, x].isHighlighted;

                    }
                    break;
                case 20:
                    amountToShift = rand.Next(2, 5);
                    startX = rand.Next(0, 1);
                    yPos = 0;
                    bool isCheckingOdds = false;
                    bool shouldFlip = false;

                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if(y % amountToShift == 0)
                            {
                                if(isCheckingOdds)
                                {
                                    if(x % 2 == 1)
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
                            //else
                            //{

                            //}
                            //if (y % (amountToShift) == 0 && (x + y) % amountToShift == 0)
                            //    board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }
                        if(shouldFlip)
                        {
                            isCheckingOdds = !isCheckingOdds;
                            shouldFlip = false;
                        }
                    }

                    //while (startX < boardXDim)
                    //{
                    //    for (int x = startX; x < boardXDim && yPos < boardYDim; x += 1)
                    //    {
                    //        board[yPos, x].isHighlighted = !board[yPos, x].isHighlighted;
                    //        yPos += amountToShift;

                    //    }
                    //    startX += 2;
                    //    yPos = 0;
                    //}
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
            if(isInvertingPattern)
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
                if(kb.IsKeyDown((Keys)(i)))
                {
                    patternChoice = i - 48;
                    shouldCalcRedraw = true;
                }
            }

            //if (kb.IsKeyDown(Keys.D7))
            //{
            //    selectedColor = Color.re;
            //}

            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    if (board[y, x].getRec().Contains(mousePos))
                    {
                        if (mouse.LeftButton == ButtonState.Pressed && oldmouse.LeftButton == ButtonState.Released)
                        {
                            if(selectedTool == tools.paintCan)
                            {
                                for (int y2 = y - 1; y2 < y +  2; y2++)
                                {
                                    for (int x2 = x - 1; x2 < x + 2; x2++)
                                    {
                                        if (board[y2, x2].isHighlighted && board[y2, x2].isUserHighlighted == false)
                                        {
                                            board[y2, x2].isUserHighlighted = true;
                                            changedCount += 1;
                                        }
                                    }
                                }
                            }

                            //board[y, x].isHighlighted = !board[y, x].isHighlighted;
                        }

                    }
                }
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
                selectedTool = tools.cross;
            }
            if (kb.IsKeyDown(Keys.D) && oldkb.IsKeyUp(Keys.D))
            {
                selectedTool = tools.single;
            }

            if (kb.IsKeyDown(Keys.E) && oldkb.IsKeyUp(Keys.E))
            {
                selectedTool = selectedTool + 1;
            }

            if (kb.IsKeyDown(Keys.R) && oldkb.IsKeyUp(Keys.R))
            {

            }
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

        public void drawTitleScreen()
        {

        }

        public void drawGameplay()
        {
            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    if (board[y, x].isHighlighted)
                    {
                        board[y, x].drawCharacter(spriteBatch, selectedColor);
                    }
                    else
                    {
                        board[y, x].drawCharacter(spriteBatch);
                    }
                    spriteBatch.DrawString(testFont, "Y: " + y + "\nX: " + x,
                        new Vector2(board[y, x].getRec().Center.X -
                        (board[y, x].getRec().Width / 4), board[y, x].getRec().Center.Y -
                        (board[y, x].getRec().Height / 3)), Color.Red);
                }
            }

            spriteBatch.DrawString(testFont, patternChoice.ToString(),
                        new Vector2(10, 10), Color.Red);

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
            GraphicsDevice.Clear(Color.CornflowerBlue);
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
