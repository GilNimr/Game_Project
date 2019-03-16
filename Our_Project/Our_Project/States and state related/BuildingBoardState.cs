
/* Gil Nevo 310021654
 * Shachar Bartal 305262016
 */ 

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XELibrary;

/*
 * On this screen you will choose 5 shapes and builld your area.
 */ 

namespace Our_Project.States_and_state_related
{
    public sealed class BuildingBoardState : BaseGameState, IBuildingBoardState
    {
            // fullTile - tile texture. emptyTile - free tile for put a shape
        private Texture2D fullTile2d , fullTileIso , emptyTile2d , emptyTileIso;
        private bool hideShape;     // true if user dont see any shape
        private Board bigEmptyBoard;    // the big board we build our area on it
        private Board dragingShape;     // get the only shape the user sees
        private readonly List<List<NodeOFHidenTiles>> allHidenPoints; // all the hiden tile at each shape
        private List<Button> buttons;       // all the buttons:
        private Button firstShape, secondShape, thirdShape, forthShape, fifthShape, next, saveYourShapeInBoard;
        private int remainShapesToPutOnBigEmptyBoard; //counter of shapes on board
        private ISoundManager soundEffect;      
        private bool isPlayBadPlaceSoundEffect;     // boolean for checking if turn on the bad place sound
        private MouseState prvState;            // mouse state for the algorithm of bad place sound
        private ScrollingBackgroundManager scrollingBackgroundManager;
        private ICelAnimationManager celAnimationManager;
        private StartMenuState startMenuState;
        public SpriteFont font;   // font on button
        public Connection connection;
        public static bool i_am_second_player = false;
        public Player player;
        public Player enemy;
        public string flag_animation;
        public string enemy_flag_animation;

        public BuildingBoardState(Game game) : base(game)
        {
            game.Services.AddService(typeof(IBuildingBoardState), this);
            scrollingBackgroundManager = new ScrollingBackgroundManager(game, "Textures\\");
            game.Components.Add(scrollingBackgroundManager);
            scrollingBackgroundManager.ScrollRate = -1f;

            celAnimationManager = (ICelAnimationManager)game.Services.GetService(typeof(ICelAnimationManager));

            startMenuState = (StartMenuState)game.Services.GetService(typeof(IStartMenuState));
       

            allHidenPoints = SetHidenTiles();   // set all the allHidenPoints
            player = new Player(game)
            {
                myTurn = true
            };

            enemy = new Player(game);
            player.pawns = new Pawn[player.army_size];
            enemy.pawns = new Pawn[player.army_size];
            

            remainShapesToPutOnBigEmptyBoard = 5;   // set the number of shapes we exepted on board as 5
            soundEffect = (ISoundManager)game.Services.GetService(typeof(ISoundManager));
            isPlayBadPlaceSoundEffect = true;   // for the algorithm about activate the badPlace sound
            // we dont see any shape:
                dragingShape = null;
                hideShape = true;
        }

        private static List<List<NodeOFHidenTiles>> SetHidenTiles() // set the hide tiles at each shape as list of lists
        {
            List<List<NodeOFHidenTiles>> allHidenPoints = new List<List<NodeOFHidenTiles>>
            {
                new List<NodeOFHidenTiles>(),
                new List<NodeOFHidenTiles>(),
                new List<NodeOFHidenTiles>(),
                new List<NodeOFHidenTiles>()
            };

            /*  for shape:
             *  
             *  //
             *  ////
             *    //
             */

            allHidenPoints[0].Add(new NodeOFHidenTiles(0, 2));
            allHidenPoints[0].Add(new NodeOFHidenTiles(0, 3));
            allHidenPoints[0].Add(new NodeOFHidenTiles(1, 2));
            allHidenPoints[0].Add(new NodeOFHidenTiles(1, 3));
            allHidenPoints[0].Add(new NodeOFHidenTiles(4, 0));
            allHidenPoints[0].Add(new NodeOFHidenTiles(4, 1));
            allHidenPoints[0].Add(new NodeOFHidenTiles(5, 0));
            allHidenPoints[0].Add(new NodeOFHidenTiles(5, 1));


            /* for shape:
             * 
             * \\\\\\\\\
             *    \\\
             */

            allHidenPoints[1].Add(new NodeOFHidenTiles(2, 0));
            allHidenPoints[1].Add(new NodeOFHidenTiles(2, 1));
            allHidenPoints[1].Add(new NodeOFHidenTiles(2, 4));
            allHidenPoints[1].Add(new NodeOFHidenTiles(2, 5));
            allHidenPoints[1].Add(new NodeOFHidenTiles(3, 0));
            allHidenPoints[1].Add(new NodeOFHidenTiles(3, 1));
            allHidenPoints[1].Add(new NodeOFHidenTiles(3, 4));
            allHidenPoints[1].Add(new NodeOFHidenTiles(3, 5));


            /* for shape:
             * 
             * ///
             * ///
             * ///
             * ////////
             * 
             */ 

            allHidenPoints[2].Add(new NodeOFHidenTiles(0, 2));
            allHidenPoints[2].Add(new NodeOFHidenTiles(0, 3));
            allHidenPoints[2].Add(new NodeOFHidenTiles(1, 2));
            allHidenPoints[2].Add(new NodeOFHidenTiles(1, 3));
            allHidenPoints[2].Add(new NodeOFHidenTiles(2, 2));
            allHidenPoints[2].Add(new NodeOFHidenTiles(2, 3));
            allHidenPoints[2].Add(new NodeOFHidenTiles(3, 2));
            allHidenPoints[2].Add(new NodeOFHidenTiles(3, 3));


            /*  for shapes:
             *  
             *  \\          \\\\\\\\\
             *  \\          \\\\\\\\\
             *  \\     &    \\\\\\\\\
             *  \\          \\\\\\\\\
             *  
             *  (with no hiden tiles)
             */ 

            allHidenPoints[3].Add(new NodeOFHidenTiles(-1, -1));
            allHidenPoints[3].Add(new NodeOFHidenTiles(-1, -1));
            
            return allHidenPoints;
        }

        private void BuildEmptyBoard()
        {
            // set the board
            bigEmptyBoard = new Board(24, emptyTileIso, emptyTile2d);

            // adding a 2/24 shape wich will be the middle of the board, set the hiden tiles and call them at the next metod
            List<NodeOFHidenTiles> empty = new List<NodeOFHidenTiles>
            {
                new NodeOFHidenTiles(-1, -1),
                new NodeOFHidenTiles(-1, -1)
            };

            // setting the middle line of bigEmptyBoard
            SetMiddleLine(new Board(empty, 2, 12, bigEmptyBoard.GetBoard()[12][0].GetCartasianRectangle().X,
                bigEmptyBoard.GetBoard()[12][0].GetCartasianRectangle().Y, fullTileIso, null, false, this.Content));
        }

        private void SetMiddleLine(Board line)  // setting the middle line of bigEmptyBoard
        {
            Texture2D fullTexture = line.GetBoard()[0][0].texture;

            for (int i=11; i<=12; i++)
            {
                for (int j=0; j<bigEmptyBoard.GetBoard()[i].Length; j++)
                {
                    bigEmptyBoard.GetBoard()[i][j].texture = fullTexture;   // set textur
                    bigEmptyBoard.GetBoard()[i][j].SetIsHidden(false);      // set boolean type isHiden
                }
            }
            SetNeighbors(bigEmptyBoard);    // set the neighbors
        }

        // if user wants to save shape were he put it:
        private void AddShapeToEmptyBoard(Board s, List<Tile> tilesFromShpae, List<Tile> tilesFromEmpty) 
        {   
            int i = 0;
            if (remainShapesToPutOnBigEmptyBoard > 0) // check if it is legal to put one shape more
            {
                //like we did on setMiddleLine():
                foreach (Tile tFromEmpty in tilesFromEmpty)
                {
                    if (!tilesFromShpae[i].GetIsHidden())
                    {
                        tFromEmpty.texture = tilesFromShpae[i].texture;
                        tFromEmpty.SetIsHidden(false);
                        tFromEmpty.sendUpdate = true;
                    }
                    i++;
                }
                SetNeighbors(bigEmptyBoard);
                remainShapesToPutOnBigEmptyBoard--; // subtract counter


                if (remainShapesToPutOnBigEmptyBoard == 0) // if now we have max number of shapes
                {   // we will create and show the next button
                    next = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
                    {
                        
                        Position = new Vector2((int)(Game1.screen_width / 1.2), (int)(Game1.screen_height / 50)),
                        Text = "Next",
                    };
                    
                    next.Click += SaveAndStartGame; // if we click on this button
                    buttons.Add(next);      // we add the button to buttons list
                    Game.Components.Add(next);  // we add the button to Compopnents 
                }
            }
            else
            {
                soundEffect.Play("badPlace"); // if there is no more shapes to put and user wanted to
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            SetAllContent();    // load textures
            SetAllButtons();
            BuildEmptyBoard();
            InitializeConnection();
            LoadBG();

        }

        private void LoadBG()
        {
            scrollingBackgroundManager.AddBackground("space", "backgroundSpace", new Vector2(0, 0), new Rectangle(0, 0, 1024, 1024), 30, 0.5f, Color.White);
            scrollingBackgroundManager.AddBackground("space2", "backgroundSpace", new Vector2(0, 1023), new Rectangle(0, 0, 1024, 1024), 30, 0.5f, Color.White);
            scrollingBackgroundManager.AddBackground("space3", "backgroundSpace", new Vector2(0, 2047), new Rectangle(0, 0, 1024, 1024), 30, 0.5f, Color.White);
        }

        private void InitializeConnection()
        {

            connection = startMenuState.connection;
            connection.player = player;
            connection.enemy = enemy;
            connection.Update();
            if (!i_am_second_player) //for now assigning flags depending on player arrival.
            {
                flag_animation = "canada";
                player.flag = "canada";
                enemy_flag_animation = "israel";
                enemy.flag = "israel";
            }
            else
            {
                flag_animation = "israel";
                player.flag = "israel";
                enemy_flag_animation = "canada";
                enemy.flag = "canada";
            }

            /*if we are the second player we have to flip the
             *board so we can view it from the same angle as first player.*/
            if (i_am_second_player)
            {
                FlipMatrix(bigEmptyBoard);
            }
        }

        private void FlipMatrix(Board bigEmptyBoard)
        {
            Rectangle tmp;

            int length = bigEmptyBoard.GetHeight() - 1;
            for (int i = 0; i < bigEmptyBoard.GetHeight() / 2; i++)
            {
                for (int j = 0; j < bigEmptyBoard.GetHeight(); j++)
                {
                    tmp = bigEmptyBoard.GetBoard()[i][j].GetCartasianRectangle();
                    bigEmptyBoard.GetBoard()[i][j].SetCartasianRectangle(bigEmptyBoard.GetBoard()[length - i][length - j].GetCartasianRectangle());
                   
                    bigEmptyBoard.GetBoard()[length - i][length - j].SetCartasianRectangle(tmp);
                    
                }
            }

            
        
        
     
        }

        private void SetAllContent()
        {   // load all textures
            fullTile2d = OurGame.Content.Load<Texture2D>(@"Textures\Tiles\Gray_Tile");
            fullTileIso = Content.Load<Texture2D>(@"Textures\Tiles\Gray_Tile_iso");
            emptyTile2d = Content.Load<Texture2D>(@"Textures\Tiles\White_2d_Tile");
            emptyTileIso = Content.Load<Texture2D>(@"Textures\Tiles\White_Isometric_Tile");
            font = Content.Load<SpriteFont>(@"Fonts\KaushanScript");
        }

        private void SetAllButtons()
        {
            /*
             *  for each button:
             *  we placement it on Game1.ScreenHeight/width as we want, add the button to buttons lists and
             *  to Components
             *  each button will show create his shape, if user will click again , the shape will disapear and 
             *  will be deleted.
             */

            buttons = new List<Button>();
            int xPositionOfShapeButton = Game1.screen_width / 4;
            int yPositionOfShapeButton = (int)(Game1.screen_height / 50);

            firstShape = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(xPositionOfShapeButton, yPositionOfShapeButton),
                Text = "First shape",
            };

            firstShape.Click += ClickFirstShape;
            buttons.Add(firstShape);

            int heightOfButton = firstShape.Rectangle.Height;

            secondShape = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(xPositionOfShapeButton, yPositionOfShapeButton + heightOfButton),
                Text = "Second Shape",
            };

            secondShape.Click += ClickSecondShape;
            buttons.Add(secondShape);

            thirdShape = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(xPositionOfShapeButton, yPositionOfShapeButton + 2*heightOfButton),
                Text = "Thirth Shape",    
            };

            thirdShape.Click += ClickThirdShape;
            buttons.Add(thirdShape);

            forthShape = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(xPositionOfShapeButton, yPositionOfShapeButton + 3*heightOfButton),
                Text = "Fourth Shape",
            };

            forthShape.Click += ClickFourthShape;
            buttons.Add(forthShape);

            fifthShape = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(xPositionOfShapeButton, yPositionOfShapeButton + 4*heightOfButton),
                Text = "Fifth Shape",
            };

            fifthShape.Click += ClickFifthShape;
            buttons.Add(fifthShape);
            
            foreach(Button b in buttons)
                Game.Components.Add(b);
        }

        /*
         * Now we have function for each button.
         * when you click - you hear sound, and the algorithm will check if you already see some shape.
         * if you do - it will disapear (and will be deleted), if you dont - the button will create his shape, set
         * the neighbors of it .
         * the shapes are drawed in commits in setHidenPoints metod
         */ 

        private void ClickFirstShape(object sender, System.EventArgs e)
        {
            soundEffect.Play("click");
            if (hideShape)
            {
                dragingShape = new Board(allHidenPoints[0], 6, 4, 0, 0, fullTileIso, fullTile2d, false, this.Content);
                SetNeighbors(dragingShape);
                hideShape = false;
            }

            else
            {
                dragingShape = null;
                GC.Collect();
                hideShape = true;
            }
        }

        private void ClickSecondShape(object sender, EventArgs e)
        {
            soundEffect.Play("click");
            if (hideShape)
            {
                
                dragingShape = new Board(allHidenPoints[1], 4, 6, 0, 0, fullTileIso, fullTile2d, false, this.Content);
                SetNeighbors(dragingShape);
                hideShape = false;
            }

            else
            {
                dragingShape = null;
                GC.Collect();
                hideShape = true;
            }
        }

        private void ClickThirdShape(object sender, EventArgs e)
        {
            soundEffect.Play("click");
            if (hideShape)
            {
                dragingShape = new Board(allHidenPoints[2], 6, 4, 0, 0, fullTileIso, fullTile2d, false, this.Content);
                SetNeighbors(dragingShape);
                hideShape = false;
            }

            else
            {
                dragingShape = null;
                GC.Collect();
                hideShape = true;
            }
        }

        private void ClickFourthShape(object sender, EventArgs e)
        {
            soundEffect.Play("click");
            if (hideShape)
            {

                dragingShape = new Board(allHidenPoints[3], 6, 2, 0, 0, fullTileIso, fullTile2d, false, this.Content);
                SetNeighbors(dragingShape);
                hideShape = false;
            }

            else
            {
                dragingShape = null;
                GC.Collect();
                hideShape = true;
            }
        }

        private void ClickFifthShape(object sender, EventArgs e)
        {
            soundEffect.Play("click");
            if (hideShape)
            {

                dragingShape = new Board(allHidenPoints[3], 5, 5, 0, 0, fullTileIso, fullTile2d, false, this.Content);
                SetNeighbors(dragingShape);
                hideShape = false;
            }

            else
            {
                dragingShape = null;
                GC.Collect();
                hideShape = true;
            }
        }


        private void SaveAndStartGame(object sender, EventArgs e) // if you finished build your board and click "next"
        {   
                // set each texture-tile in bigEmptyBoard that without shape as null
            foreach (Tile[] tileLine in bigEmptyBoard.GetBoard()) 
            {
                foreach (Tile t in tileLine)
                {
                    if (t.GetIsHidden())
                        t.texture = null;
                }
            }

            soundEffect.Play("click");
            buttons.Clear();    //destroid buttons
            StateManager.ChangeState(OurGame.PlacingSoldiersState.Value); // change state
        }

        


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // update shape if not null
            if (dragingShape != null)
                dragingShape.Update();
            
            PutShapeAtNewPosition(); // checking and put shapes as user wants on the big board

            connection.Update();
            base.Update(gameTime);
        }


        private void PutShapeAtNewPosition() // checking and put shapes as user wants on the bigBoard
        {
            /* lists that will save each tile from shape that we want to move,
             * and each tile of empty that we want to put on it*/
            List<Tile> shapeTilesToMove = new List<Tile>();
            List<Tile> emptyTilesToMove = new List<Tile>();

            if (dragingShape != null)
            {

                /*
                 * allFullTilesAtShapeIsInsideBigEmptyBoardLimit will be list with all the tiles that not hiden in shape, and we
                 * will check that all of them is in the limit of bigEmptyBoard.
                 */

                List<bool> allFullTilesAtShapeIsInsideBigEmptyBoardLimit = new List<bool>();
                for (int i = 0; i < dragingShape.GetBoard().Length; i++)
                {
                    for (int j = 0; j < dragingShape.GetBoard()[i].Length; j++)
                        if (!dragingShape.GetBoard()[i][j].GetIsHidden())
                            allFullTilesAtShapeIsInsideBigEmptyBoardLimit.Add(false);
                }

                /*
                 * Now we start loop on tiles of shape, and loop of tiles of bigEmptyBoard and checking instract
                 * between them
                 */

                int a = 0; // just indext for allFullTilesAtShapeIsInsideBigEmptyBoardLimit
                foreach (Tile[] shapeTilesLine in dragingShape.GetBoard()) // tiles at shape
                {
                    foreach (Tile shapeTile in shapeTilesLine)
                    {
                        foreach (Tile[] emptyTilesLine in bigEmptyBoard.GetBoard()) // tiles at bigEmptyBoard
                        {
                            foreach (Tile emptyTile in emptyTilesLine)
                            {
                                if (CheckingInstractOfTiles(shapeTile, emptyTile)) // if emptyTile and shapeTile instract
                                {
                                    if (CheckingMatchBetweenEmptyAndShape(shapeTile, emptyTile)) // checking specific match between tiles
                                    {
                                        allFullTilesAtShapeIsInsideBigEmptyBoardLimit[a] = true; // set specific tile as true
                                        /*  Now we add emptyTile and shapeTile to a list, that if we will move at the
                                         * and the shape to other place - we use this lists.*/

                                        emptyTilesToMove.Add(emptyTile);
                                        shapeTilesToMove.Add(shapeTile);
                                        a++;
                                    }
                                }
                                if (dragingShape.GetMove()) // cancel saveYourShapeInBoard button if we dragging the shapes
                                {
                                    buttons.Remove(saveYourShapeInBoard);
                                    Game.Components.Remove(saveYourShapeInBoard);
                                }
                            }
                        }
                    }
                }

                /*
                 * Now we check allFullTilesAtShapeIsInsideBigEmptyBoardLimit list, if it will be all true - 
                 * so we can move this shape.
                 */

                bool isAllShapeOnBigEmptyBoard = false; // will be true if user wants to check new place of shape

                for (int i = 0; i < allFullTilesAtShapeIsInsideBigEmptyBoardLimit.Count; i++)
                {
                    if (!allFullTilesAtShapeIsInsideBigEmptyBoardLimit[i])
                    {
                        isAllShapeOnBigEmptyBoard = false;
                        break;
                    }
                    isAllShapeOnBigEmptyBoard = true;
                }

                if (isAllShapeOnBigEmptyBoard)
                {
                    // if shape is in limit, we create and show the saveYourShapeButton, for save the new place
                    int xPositionOfShapeButton = Game1.screen_width / 5;
                    int yPositionOfShapeButton = (int)(Game1.screen_height / 50);

                    saveYourShapeInBoard = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
                    {
                        Position = new Vector2(Game1.screen_width / 2, (int)(Game1.screen_height / 50)),
                        Text = "Save Shape",
                    };
                    // add the button the same as we did with the shapes buttons
                    buttons.Add(saveYourShapeInBoard);
                    Game.Components.Add(saveYourShapeInBoard);

                    SetShapeAtHisNewPosition(shapeTilesToMove, emptyTilesToMove);

                    if (LegalPlace(dragingShape, shapeTilesToMove, emptyTilesToMove)) // check if there is 2 neighbor tiles
                    {
                        soundEffect.Play("click");
                        saveYourShapeInBoard.Click += (sender2, e2) => SaveShapeAtNewPlace(sender2, e2,
                            dragingShape, shapeTilesToMove, emptyTilesToMove); // set the shape as part in bigEmptyBoard
                        SetNeighbors(bigEmptyBoard); // set neighbors
                    }
                    else // if it is illegal place
                        soundEffect.Play("badPlace");
                }
            }
        }

        private static void SetShapeAtHisNewPosition(List<Tile> shapeTilesToMove, List<Tile> emptyTilesToMove)
        { /* still not save, but the user will see how it will be look like 
            if he will decide to put the shape in this place*/

            for (int i = 0; i < shapeTilesToMove.Count; i++)
            {
                shapeTilesToMove[i].SetToCartasianRectangle(emptyTilesToMove[i].GetCartasianRectangle().X,
                    emptyTilesToMove[i].GetCartasianRectangle().Y);
            }
        }

        private static bool CheckingMatchBetweenEmptyAndShape(Tile shapeTile, Tile emptyTile)
        { // check match between tiles
            return (shapeTile.GetCartasianRectangle().Center.X > emptyTile.GetCartasianRectangle().Center.X) &&
                        (shapeTile.GetCartasianRectangle().Center.Y > emptyTile.GetCartasianRectangle().Center.Y) 
                        && emptyTile.GetIsHidden();
        }

        private bool CheckingInstractOfTiles(Tile shapeTile, Tile emptyTile)
        {
            return shapeTile.GetCartasianRectangle().Intersects(emptyTile.GetCartasianRectangle())
                                                    && (!dragingShape.GetMove()) && !shapeTile.GetIsHidden() && emptyTile.GetIsHidden();
        }

        private void SetNeighbors(Board b)
        { 
            // set the neighbors of board b. as we have a lot of change, we need to update it each change
            for (int i = 0; i < b.GetBoard().Length; ++i)
            {
                for (int j = 0; j < b.GetBoard()[i].Length; ++j)
                {
                    if (b.GetBoard()[i][j] != null)
                    {
                        //right
                        if (i < b.GetBoard().Length - 1)
                            b.GetBoard()[i][j].SetRight(b.GetBoard()[i + 1][j]); // x axis grow up

                        //left
                        if (i >= 1)
                            b.GetBoard()[i][j].SetLeft(b.GetBoard()[i - 1][j]); // x axis go down

                        //down
                        if (j < b.GetBoard()[i].Length - 1)
                            b.GetBoard()[i][j].SetDown(b.GetBoard()[i][j + 1]); // y axis grow up
                                                                                                        //up
                        if (j >= 1)
                            b.GetBoard()[i][j].SetUp(b.GetBoard()[i][j - 1]); // y axis go down
                    }
                }
            }
        }

        private bool LegalPlace(Board shape, List<Tile> shapeTiles, List<Tile> empty)
        { //checking that there is at least 2 not-hiden-tiles neighbors of shape
            for (int i=0; i<shapeTiles.Count; i++)
            {
                Tile tFromShape = shape.boardDictionaryById[shapeTiles[i].GetId()];
                Tile tFromEmpty = bigEmptyBoard.boardDictionaryById[empty[i].GetId()];

                if (!i_am_second_player)
                {
                    if (tFromEmpty.GetId() < 286)
                        return false;
                }
                else
                {
                    if (tFromEmpty.GetId() > 264) // currect place of user on big board
                        return false;
                }

                /*
                 * Now we check neighbors from each side
                 */ 

                if (CheckingFromLeft(tFromEmpty))
                {
                    if (MoreTileFromLeft(tFromShape, tFromEmpty))
                    {
                        return true;
                    }
                }

                if (CheckingFromRight(tFromEmpty))
                {
                    if (MoreTileFromRight(tFromShape, tFromEmpty))
                    {
                        return true;
                    }
                }

                if (CheckingFromUp(tFromEmpty))
                {
                    if (MoreTileFromUp(tFromShape, tFromEmpty))
                    {
                        return true;
                    }
                }

                if (CheckingFromDown(tFromEmpty))
                {
                    if (MoreTileFromDown(tFromShape, tFromEmpty))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /*
         * The next 8 function is sub-function of legaPlace metod, and they check neighbors
         * check from ____ -> checking the first neighbor tile
         * moreTileFrom____ -> checking the second neighbor from same side
         */ 

        private static bool MoreTileFromDown(Tile tFromShape, Tile tFromEmpty)
        {
            return tFromEmpty.GetRight() != null && tFromShape.GetRight() != null && !tFromShape.GetRight().GetIsHidden()
                                    && tFromEmpty.GetRight().GetDown() != null && !tFromEmpty.GetRight().GetDown().GetIsHidden()
                                    ||
                                    tFromEmpty.GetLeft() != null && tFromShape.GetLeft() != null && !tFromShape.GetLeft().GetIsHidden()
                                    && tFromEmpty.GetLeft().GetDown() != null && !tFromEmpty.GetLeft().GetDown().GetIsHidden();
        }

        private static bool CheckingFromDown(Tile tFromEmpty)
        {
            return tFromEmpty.GetDown() != null && !tFromEmpty.GetDown().GetIsHidden();
        }

        private static bool MoreTileFromUp(Tile tFromShape, Tile tFromEmpty)
        {
            return tFromEmpty.GetRight() != null && tFromShape.GetRight() != null && !tFromShape.GetRight().GetIsHidden()
                                    && tFromEmpty.GetRight().GetUp() != null && !tFromEmpty.GetRight().GetUp().GetIsHidden()
                                    ||
                                     tFromEmpty.GetLeft() != null && tFromShape.GetLeft() != null && !tFromShape.GetLeft().GetIsHidden()
                                    && tFromEmpty.GetLeft().GetUp() != null && !tFromEmpty.GetLeft().GetUp().GetIsHidden();
        }

        private static bool CheckingFromUp(Tile tFromEmpty)
        {
            return tFromEmpty.GetUp() != null && !tFromEmpty.GetUp().GetIsHidden();
        }

        private static bool MoreTileFromRight(Tile tFromShape, Tile tFromEmpty)
        {
            return tFromEmpty.GetUp() != null && tFromShape.GetUp() != null && !tFromShape.GetUp().GetIsHidden()
                                    && tFromEmpty.GetUp().GetRight() != null && !tFromEmpty.GetUp().GetRight().GetIsHidden()
                                    ||
                                    tFromEmpty.GetDown() != null && tFromShape.GetDown() != null && !tFromShape.GetDown().GetIsHidden()
                                    && tFromEmpty.GetDown().GetRight() != null && !tFromEmpty.GetDown().GetRight().GetIsHidden();
        }

        private static bool CheckingFromRight(Tile tFromEmpty)
        {
            return tFromEmpty.GetRight() != null && !tFromEmpty.GetRight().GetIsHidden();
        }

        private static bool MoreTileFromLeft(Tile tFromShape, Tile tFromEmpty)
        {
            return tFromEmpty.GetUp() != null && tFromShape.GetUp() != null && !tFromShape.GetUp().GetIsHidden()
                                    && tFromEmpty.GetUp().GetLeft() != null && !tFromEmpty.GetUp().GetLeft().GetIsHidden()
                                    ||
                                    tFromEmpty.GetDown() != null && tFromShape.GetDown() != null && !tFromShape.GetDown().GetIsHidden()
                                    && tFromEmpty.GetDown().GetLeft() != null && !tFromEmpty.GetDown().GetLeft().GetIsHidden();
        }

        private static bool CheckingFromLeft(Tile tFromEmpty)
        {
            return tFromEmpty.GetLeft() != null && !tFromEmpty.GetLeft().GetIsHidden();
        }

        private void SaveShapeAtNewPlace(object sender, EventArgs e, Board shape, List<Tile> shapeTiles,
            List<Tile> emptyTiles)
        {
            /*
             * if user chose to save shape, this is the function from the save button.
             * here we hear sound, adding the shape to bigEmptyBoard and delete the shape as shape
             * (now realy inside the bigEmptyBoard
             * then we remove the save button
             */ 


            soundEffect.Play("click");
            AddShapeToEmptyBoard(shape, shapeTiles, emptyTiles);
            dragingShape = null;
            GC.Collect();
            hideShape = true;
            buttons.Remove(saveYourShapeInBoard);
            Game.Components.Remove(saveYourShapeInBoard);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            MouseState currentMouse = Mouse.GetState();
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //drawing space bg
            scrollingBackgroundManager.Draw("space", OurGame.spriteBatch);
            scrollingBackgroundManager.Draw("space2", OurGame.spriteBatch);
            scrollingBackgroundManager.Draw("space3", OurGame.spriteBatch);

            // draw bigEmptyBoard
            bigEmptyBoard.Draw(OurGame.spriteBatch, Color.White);
            // just as we did at the update, we want to draw in accordance to situation
            foreach (Tile[] emptyTilesLine in bigEmptyBoard.GetBoard())
            {
                foreach (Tile emptyTile in emptyTilesLine)
                {
                        if (dragingShape != null)
                        {
                            Board shape = dragingShape;
                            foreach (Tile[] shapeTilesLine in shape.GetBoard())
                            {
                                foreach (Tile shapeTile in shapeTilesLine)
                                {
                                // because it is not ecacly the same condition, we hav sub-metods for draw
                                    if (CheckingInstractTilesForDraw(emptyTile, shapeTile))
                                    {
                                        if (ChekingMatchTilesForDraw(emptyTile, shapeTile))
                                        {
                                            if (!shapeTile.GetIsHidden())
                                            {
                                                if (!emptyTile.GetIsHidden())
                                                {       
                                                        // we paint emptyBoard to red if there is shape already
                                                    emptyTile.SetColor(Color.Red);

                                                        /* we use the boolean isPlayBad... for sound just once 
                                                            each illegal shape position*/
                                                    if (Mouse.GetState().LeftButton == ButtonState.Released &&
                                                        prvState.LeftButton == ButtonState.Pressed)
                                                        isPlayBadPlaceSoundEffect = false;

                                                    if (!isPlayBadPlaceSoundEffect)
                                                    {
                                                        soundEffect.Play("badPlace");
                                                        isPlayBadPlaceSoundEffect = true;
                                                    }
                                                }

                                                else // if it is legal place
                                                {
                                                    emptyTile.SetColor(Color.Green);
                                                }
                                        }
                                    }
                                }
                            }
                        }
                        }
                }
            }
            prvState = currentMouse; // for badPlace sound algorithm

                if (dragingShape != null)
                    dragingShape.Draw(OurGame.spriteBatch, Color.White);

            foreach(Button b in buttons)
                b.Draw(gameTime, OurGame.spriteBatch);
            
            base.Draw(gameTime);
        }

        // checking instract between tiles at Draw metod
        private static bool CheckingInstractTilesForDraw(Tile emptyTile, Tile shapeTile)
        {
            return shapeTile.GetCartasianRectangle().Intersects(emptyTile.GetCartasianRectangle());
        }

        // checking match between tiles at Draw metod
        private static bool ChekingMatchTilesForDraw(Tile emptyTile, Tile shapeTile)
        {
            return (shapeTile.GetCartasianRectangle().Center.X >
                                                        emptyTile.GetCartasianRectangle().Center.X) &&
                                                        (shapeTile.GetCartasianRectangle().Center.Y >
                                                        emptyTile.GetCartasianRectangle().Center.Y);
        }

        public  Board GetEmptyBoard()
        {
            return bigEmptyBoard;
        }
    }
}