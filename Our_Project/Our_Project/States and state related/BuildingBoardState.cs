
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
        private List<List<NodeOFHidenTiles>> allHidenPoints; // all the hiden tile at each shape
        private List<Button> buttons;       // all the buttons:
        private Button firstShape, secondShape, thirdShape, forthShape, fifthShape, next, saveYourShapeInBoard;
        private int remainShapesToPutOnBigEmptyBoard; //counter of shapes on board
        private ISoundManager soundEffect;      
        private bool isPlayBadPlaceSoundEffect;     // boolean for checking if turn on the bad place sound
        private MouseState prvState;            // mouse state for the algorithm of bad place sound
        private ScrollingBackgroundManager scrollingBackgroundManager;

        public SpriteFont font;   // font on button
        public Connection connection;
        public static bool i_am_second_player = false;
        public Player player;
        public Player enemy;
        
        public BuildingBoardState(Game game) : base(game)
        {
            game.Services.AddService(typeof(IBuildingBoardState), this);
            scrollingBackgroundManager = new ScrollingBackgroundManager(game, "Textures\\");
            game.Components.Add(scrollingBackgroundManager);
            scrollingBackgroundManager.ScrollRate = -1f;
            allHidenPoints = setHidenTiles();   // set all the allHidenPoints
            player = new Player(game);
            player.myTurn = true;
            enemy = new Player(game);
            player.pawns = new Pawn[player.army_size];
            enemy.pawns = new Pawn[player.army_size];
            connection = new Connection(game,ref player, ref enemy);
            remainShapesToPutOnBigEmptyBoard = 5;   // set the number of shapes we exepted on board as 5
            soundEffect = (ISoundManager)game.Services.GetService(typeof(ISoundManager));
            isPlayBadPlaceSoundEffect = true;   // for the algorithm about activate the badPlace sound
            // we dont see any shape:
                dragingShape = null;
                hideShape = true;
        }

        private static List<List<NodeOFHidenTiles>> setHidenTiles() // set the hide tiles at each shape as list of lists
        {
            List<List<NodeOFHidenTiles>> allHidenPoints = new List<List<NodeOFHidenTiles>>();
            allHidenPoints.Add(new List<NodeOFHidenTiles>());
            allHidenPoints.Add(new List<NodeOFHidenTiles>());
            allHidenPoints.Add(new List<NodeOFHidenTiles>());
            allHidenPoints.Add(new List<NodeOFHidenTiles>());

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

        private void buildEmptyBoard()
        {
            // set the board
            bigEmptyBoard = new Board(24, emptyTileIso, emptyTile2d);

            // adding a 2/24 shape wich will be the middle of the board, set the hiden tiles and call them at the next metod
            List<NodeOFHidenTiles> empty = new List<NodeOFHidenTiles>();
            empty.Add(new NodeOFHidenTiles(-1, -1));
            empty.Add(new NodeOFHidenTiles(-1, -1));
            
            // setting the middle line of bigEmptyBoard
            setMiddleLine(new Board(empty, 2, 12, bigEmptyBoard.getBoard()[12][0].getCartasianRectangle().X,
                bigEmptyBoard.getBoard()[12][0].getCartasianRectangle().Y, fullTileIso, null, false, this.Content));
        }

        private void setMiddleLine(Board line)  // setting the middle line of bigEmptyBoard
        {
            Texture2D fullTexture = line.getBoard()[0][0].texture;

            for (int i=11; i<=12; i++)
            {
                for (int j=0; j<bigEmptyBoard.getBoard()[i].Length; j++)
                {
                    bigEmptyBoard.getBoard()[i][j].texture = fullTexture;   // set textur
                    bigEmptyBoard.getBoard()[i][j].setIsHidden(false);      // set boolean type isHiden
                }
            }
            setNeighbors(bigEmptyBoard);    // set the neighbors
        }

        // if user wants to save shape were he put it:
        private void addShapeToEmptyBoard(Board s, List<Tile> tilesFromShpae, List<Tile> tilesFromEmpty) 
        {   
            int i = 0;
            if (remainShapesToPutOnBigEmptyBoard > 0) // check if it is legal to put one shape more
            {
                //like we did on setMiddleLine():
                foreach (Tile tFromEmpty in tilesFromEmpty)
                {
                    if (!tilesFromShpae[i].getIsHidden())
                    {
                        tFromEmpty.texture = tilesFromShpae[i].texture;
                        tFromEmpty.setIsHidden(false);
                        tFromEmpty.sendUpdate = true;
                    }
                    i++;
                }
                setNeighbors(bigEmptyBoard);
                remainShapesToPutOnBigEmptyBoard--; // subtract counter


                if (remainShapesToPutOnBigEmptyBoard == 0) // if now we have max number of shapes
                {   // we will create and show the next button
                    next = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
                    {
                        
                        Position = new Vector2((int)(Game1.screen_width / 1.2), (int)(Game1.screen_height / 50)),
                        Text = "Next",
                    };
                    
                    next.Click += saveAndStartGame; // if we click on this button
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
            setAllContent();    // load textures
            setAllButtons();
            buildEmptyBoard();
            initializeConnection();
            loadBG();
        }

        private void loadBG()
        {
            scrollingBackgroundManager.AddBackground("space", "backgroundSpace", new Vector2(0, 0), new Rectangle(0, 0, 1024, 1024), 30, 0.5f, Color.White);
            scrollingBackgroundManager.AddBackground("space2", "backgroundSpace", new Vector2(0, 1023), new Rectangle(0, 0, 1024, 1024), 30, 0.5f, Color.White);
            scrollingBackgroundManager.AddBackground("space3", "backgroundSpace", new Vector2(0, 2047), new Rectangle(0, 0, 1024, 1024), 30, 0.5f, Color.White);
        }

        private void initializeConnection()
        {
            
            connection.update();
            if (i_am_second_player)
            {
                flipMatrix(bigEmptyBoard);
            }
        }

        private void flipMatrix(Board bigEmptyBoard)
        {
            Rectangle tmp;

            int length = bigEmptyBoard.getHeight() - 1;
            for (int i = 0; i < bigEmptyBoard.getHeight() / 2; i++)
            {
                for (int j = 0; j < bigEmptyBoard.getHeight(); j++)
                {
                    tmp = bigEmptyBoard.getBoard()[i][j].getCartasianRectangle();
                    bigEmptyBoard.getBoard()[i][j].setCartasianRectangle(bigEmptyBoard.getBoard()[length - i][length - j].getCartasianRectangle());
                   
                    bigEmptyBoard.getBoard()[length - i][length - j].setCartasianRectangle(tmp);
                    
                }
            }

            
        
        
     
        }

        private void setAllContent()
        {   // load all textures
            fullTile2d = OurGame.Content.Load<Texture2D>(@"Textures\Tiles\Gray_Tile");
            fullTileIso = Content.Load<Texture2D>(@"Textures\Tiles\Gray_Tile_iso");
            emptyTile2d = Content.Load<Texture2D>(@"Textures\Tiles\White_2d_Tile");
            emptyTileIso = Content.Load<Texture2D>(@"Textures\Tiles\White_Isometric_Tile");
            font = Content.Load<SpriteFont>(@"Fonts\KaushanScript");
        }

        private void setAllButtons()
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

            firstShape.Click += clickFirstShape;
            buttons.Add(firstShape);

            int heightOfButton = firstShape.Rectangle.Height;

            secondShape = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(xPositionOfShapeButton, yPositionOfShapeButton + heightOfButton),
                Text = "Second Shape",
            };

            secondShape.Click += clickSecondShape;
            buttons.Add(secondShape);

            thirdShape = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(xPositionOfShapeButton, yPositionOfShapeButton + 2*heightOfButton),
                Text = "Thirth Shape",    
            };

            thirdShape.Click += clickThirdShape;
            buttons.Add(thirdShape);

            forthShape = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(xPositionOfShapeButton, yPositionOfShapeButton + 3*heightOfButton),
                Text = "Fourth Shape",
            };

            forthShape.Click += clickFourthShape;
            buttons.Add(forthShape);

            fifthShape = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(xPositionOfShapeButton, yPositionOfShapeButton + 4*heightOfButton),
                Text = "Fifth Shape",
            };

            fifthShape.Click += clickFifthShape;
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

        private void clickFirstShape(object sender, System.EventArgs e)
        {
            soundEffect.Play("click");
            if (hideShape)
            {
                dragingShape = new Board(allHidenPoints[0], 6, 4, 0, 0, fullTileIso, fullTile2d, false, this.Content);
                setNeighbors(dragingShape);
                hideShape = false;
            }

            else
            {
                dragingShape = null;
                GC.Collect();
                hideShape = true;
            }
        }

        private void clickSecondShape(object sender, EventArgs e)
        {
            soundEffect.Play("click");
            if (hideShape)
            {
                
                dragingShape = new Board(allHidenPoints[1], 4, 6, 0, 0, fullTileIso, fullTile2d, false, this.Content);
                setNeighbors(dragingShape);
                hideShape = false;
            }

            else
            {
                dragingShape = null;
                GC.Collect();
                hideShape = true;
            }
        }

        private void clickThirdShape(object sender, EventArgs e)
        {
            soundEffect.Play("click");
            if (hideShape)
            {
                dragingShape = new Board(allHidenPoints[2], 6, 4, 0, 0, fullTileIso, fullTile2d, false, this.Content);
                setNeighbors(dragingShape);
                hideShape = false;
            }

            else
            {
                dragingShape = null;
                GC.Collect();
                hideShape = true;
            }
        }

        private void clickFourthShape(object sender, EventArgs e)
        {
            soundEffect.Play("click");
            if (hideShape)
            {

                dragingShape = new Board(allHidenPoints[3], 6, 2, 0, 0, fullTileIso, fullTile2d, false, this.Content);
                setNeighbors(dragingShape);
                hideShape = false;
            }

            else
            {
                dragingShape = null;
                GC.Collect();
                hideShape = true;
            }
        }

        private void clickFifthShape(object sender, EventArgs e)
        {
            soundEffect.Play("click");
            if (hideShape)
            {

                dragingShape = new Board(allHidenPoints[3], 5, 5, 0, 0, fullTileIso, fullTile2d, false, this.Content);
                setNeighbors(dragingShape);
                hideShape = false;
            }

            else
            {
                dragingShape = null;
                GC.Collect();
                hideShape = true;
            }
        }


        private void saveAndStartGame(object sender, EventArgs e) // if you finished build your board and click "next"
        {   
                // set each texture-tile in bigEmptyBoard that without shape as null
            foreach (Tile[] tileLine in bigEmptyBoard.getBoard()) 
            {
                foreach (Tile t in tileLine)
                {
                    if (t.getIsHidden())
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
            
            putShapeAtNewPosition(); // checking and put shapes as user wants on the big board

            connection.update();
            base.Update(gameTime);
        }


        private void putShapeAtNewPosition() // checking and put shapes as user wants on the bigBoard
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
                for (int i = 0; i < dragingShape.getBoard().Length; i++)
                {
                    for (int j = 0; j < dragingShape.getBoard()[i].Length; j++)
                        if (!dragingShape.getBoard()[i][j].getIsHidden())
                            allFullTilesAtShapeIsInsideBigEmptyBoardLimit.Add(false);
                }

                /*
                 * Now we start loop on tiles of shape, and loop of tiles of bigEmptyBoard and checking instract
                 * between them
                 */

                int a = 0; // just indext for allFullTilesAtShapeIsInsideBigEmptyBoardLimit
                foreach (Tile[] shapeTilesLine in dragingShape.getBoard()) // tiles at shape
                {
                    foreach (Tile shapeTile in shapeTilesLine)
                    {
                        foreach (Tile[] emptyTilesLine in bigEmptyBoard.getBoard()) // tiles at bigEmptyBoard
                        {
                            foreach (Tile emptyTile in emptyTilesLine)
                            {
                                if (checkingInstractOfTiles(shapeTile, emptyTile)) // if emptyTile and shapeTile instract
                                {
                                    if (checkingMatchBetweenEmptyAndShape(shapeTile, emptyTile)) // checking specific match between tiles
                                    {
                                        allFullTilesAtShapeIsInsideBigEmptyBoardLimit[a] = true; // set specific tile as true
                                        /*  Now we add emptyTile and shapeTile to a list, that if we will move at the
                                         * and the shape to other place - we use this lists.*/

                                        emptyTilesToMove.Add(emptyTile);
                                        shapeTilesToMove.Add(shapeTile);
                                        a++;
                                    }
                                }
                                if (dragingShape.getMove()) // cancel saveYourShapeInBoard button if we dragging the shapes
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

                    setShapeAtHisNewPosition(shapeTilesToMove, emptyTilesToMove);

                    if (legalPlace(dragingShape, shapeTilesToMove, emptyTilesToMove)) // check if there is 2 neighbor tiles
                    {
                        soundEffect.Play("click");
                        saveYourShapeInBoard.Click += (sender2, e2) => saveShapeAtNewPlace(sender2, e2,
                            dragingShape, shapeTilesToMove, emptyTilesToMove); // set the shape as part in bigEmptyBoard
                        setNeighbors(bigEmptyBoard); // set neighbors
                    }
                    else // if it is illegal place
                        soundEffect.Play("badPlace");
                }
            }
        }

        private static void setShapeAtHisNewPosition(List<Tile> shapeTilesToMove, List<Tile> emptyTilesToMove)
        { /* still not save, but the user will see how it will be look like 
            if he will decide to put the shape in this place*/

            for (int i = 0; i < shapeTilesToMove.Count; i++)
            {
                shapeTilesToMove[i].setToCartasianRectangle(emptyTilesToMove[i].getCartasianRectangle().X,
                    emptyTilesToMove[i].getCartasianRectangle().Y);
            }
        }

        private static bool checkingMatchBetweenEmptyAndShape(Tile shapeTile, Tile emptyTile)
        { // check match between tiles
            return (shapeTile.getCartasianRectangle().Center.X > emptyTile.getCartasianRectangle().Center.X) &&
                        (shapeTile.getCartasianRectangle().Center.Y > emptyTile.getCartasianRectangle().Center.Y) 
                        && emptyTile.getIsHidden();
        }

        private bool checkingInstractOfTiles(Tile shapeTile, Tile emptyTile)
        {
            return shapeTile.getCartasianRectangle().Intersects(emptyTile.getCartasianRectangle())
                                                    && (!dragingShape.getMove()) && !shapeTile.getIsHidden() && emptyTile.getIsHidden();
        }

        private void setNeighbors(Board b)
        { 
            // set the neighbors of board b. as we have a lot of change, we need to update it each change
            for (int i = 0; i < b.getBoard().Length; ++i)
            {
                for (int j = 0; j < b.getBoard()[i].Length; ++j)
                {
                    if (b.getBoard()[i][j] != null)
                    {
                        //right
                        if (i < b.getBoard().Length - 1)
                            b.getBoard()[i][j].setRight(b.getBoard()[i + 1][j]); // x axis grow up

                        //left
                        if (i >= 1)
                            b.getBoard()[i][j].setLeft(b.getBoard()[i - 1][j]); // x axis go down

                        //down
                        if (j < b.getBoard()[i].Length - 1)
                            b.getBoard()[i][j].setDown(b.getBoard()[i][j + 1]); // y axis grow up
                                                                                                        //up
                        if (j >= 1)
                            b.getBoard()[i][j].setUp(b.getBoard()[i][j - 1]); // y axis go down
                    }
                }
            }
        }

        private bool legalPlace(Board shape, List<Tile> shapeTiles, List<Tile> empty)
        { //checking that there is at least 2 not-hiden-tiles neighbors of shape
            for (int i=0; i<shapeTiles.Count; i++)
            {
                Tile tFromShape = shape.boardDictionaryById[shapeTiles[i].getId()];
                Tile tFromEmpty = bigEmptyBoard.boardDictionaryById[empty[i].getId()];

                if (!i_am_second_player)
                {
                    if (tFromEmpty.getId() < 286)
                        return false;
                }
                else
                {
                    if (tFromEmpty.getId() > 264) // currect place of user on big board
                        return false;
                }

                /*
                 * Now we check neighbors from each side
                 */ 

                if (checkingFromLeft(tFromEmpty))
                {
                    if (moreTileFromLeft(tFromShape, tFromEmpty))
                    {
                        return true;
                    }
                }

                if (checkingFromRight(tFromEmpty))
                {
                    if (moreTileFromRight(tFromShape, tFromEmpty))
                    {
                        return true;
                    }
                }

                if (checkingFromUp(tFromEmpty))
                {
                    if (moreTileFromUp(tFromShape, tFromEmpty))
                    {
                        return true;
                    }
                }

                if (checkingFromDown(tFromEmpty))
                {
                    if (moreTileFromDown(tFromShape, tFromEmpty))
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

        private static bool moreTileFromDown(Tile tFromShape, Tile tFromEmpty)
        {
            return tFromEmpty.getRight() != null && tFromShape.getRight() != null && !tFromShape.getRight().getIsHidden()
                                    && tFromEmpty.getRight().getDown() != null && !tFromEmpty.getRight().getDown().getIsHidden()
                                    ||
                                    tFromEmpty.getLeft() != null && tFromShape.getLeft() != null && !tFromShape.getLeft().getIsHidden()
                                    && tFromEmpty.getLeft().getDown() != null && !tFromEmpty.getLeft().getDown().getIsHidden();
        }

        private static bool checkingFromDown(Tile tFromEmpty)
        {
            return tFromEmpty.getDown() != null && !tFromEmpty.getDown().getIsHidden();
        }

        private static bool moreTileFromUp(Tile tFromShape, Tile tFromEmpty)
        {
            return tFromEmpty.getRight() != null && tFromShape.getRight() != null && !tFromShape.getRight().getIsHidden()
                                    && tFromEmpty.getRight().getUp() != null && !tFromEmpty.getRight().getUp().getIsHidden()
                                    ||
                                     tFromEmpty.getLeft() != null && tFromShape.getLeft() != null && !tFromShape.getLeft().getIsHidden()
                                    && tFromEmpty.getLeft().getUp() != null && !tFromEmpty.getLeft().getUp().getIsHidden();
        }

        private static bool checkingFromUp(Tile tFromEmpty)
        {
            return tFromEmpty.getUp() != null && !tFromEmpty.getUp().getIsHidden();
        }

        private static bool moreTileFromRight(Tile tFromShape, Tile tFromEmpty)
        {
            return tFromEmpty.getUp() != null && tFromShape.getUp() != null && !tFromShape.getUp().getIsHidden()
                                    && tFromEmpty.getUp().getRight() != null && !tFromEmpty.getUp().getRight().getIsHidden()
                                    ||
                                    tFromEmpty.getDown() != null && tFromShape.getDown() != null && !tFromShape.getDown().getIsHidden()
                                    && tFromEmpty.getDown().getRight() != null && !tFromEmpty.getDown().getRight().getIsHidden();
        }

        private static bool checkingFromRight(Tile tFromEmpty)
        {
            return tFromEmpty.getRight() != null && !tFromEmpty.getRight().getIsHidden();
        }

        private static bool moreTileFromLeft(Tile tFromShape, Tile tFromEmpty)
        {
            return tFromEmpty.getUp() != null && tFromShape.getUp() != null && !tFromShape.getUp().getIsHidden()
                                    && tFromEmpty.getUp().getLeft() != null && !tFromEmpty.getUp().getLeft().getIsHidden()
                                    ||
                                    tFromEmpty.getDown() != null && tFromShape.getDown() != null && !tFromShape.getDown().getIsHidden()
                                    && tFromEmpty.getDown().getLeft() != null && !tFromEmpty.getDown().getLeft().getIsHidden();
        }

        private static bool checkingFromLeft(Tile tFromEmpty)
        {
            return tFromEmpty.getLeft() != null && !tFromEmpty.getLeft().getIsHidden();
        }

        private void saveShapeAtNewPlace(object sender, EventArgs e, Board shape, List<Tile> shapeTiles,
            List<Tile> emptyTiles)
        {
            /*
             * if user chose to save shape, this is the function from the save button.
             * here we hear sound, adding the shape to bigEmptyBoard and delete the shape as shape
             * (now realy inside the bigEmptyBoard
             * then we remove the save button
             */ 


            soundEffect.Play("click");
            addShapeToEmptyBoard(shape, shapeTiles, emptyTiles);
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
            foreach (Tile[] emptyTilesLine in bigEmptyBoard.getBoard())
            {
                foreach (Tile emptyTile in emptyTilesLine)
                {
                        if (dragingShape != null)
                        {
                            Board shape = dragingShape;
                            foreach (Tile[] shapeTilesLine in shape.getBoard())
                            {
                                foreach (Tile shapeTile in shapeTilesLine)
                                {
                                // because it is not ecacly the same condition, we hav sub-metods for draw
                                    if (checkingInstractTilesForDraw(emptyTile, shapeTile))
                                    {
                                        if (chekingMatchTilesForDraw(emptyTile, shapeTile))
                                        {
                                            if (!shapeTile.getIsHidden())
                                            {
                                                if (!emptyTile.getIsHidden())
                                                {       
                                                        // we paint emptyBoard to red if there is shape already
                                                    emptyTile.setColor(Color.Red);

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
                                                    emptyTile.setColor(Color.Green);
                                                }



                                            //for debug purposes
                                            /* OurGame.spriteBatch.DrawString(font, emptyTile.getId().ToString(),Game1.TwoD2isometrix( emptyTile.getCartasianRectangle().X,

                                                 emptyTile.getCartasianRectangle().Y), Color.Black, 0, new Vector2(0), 0.8f, SpriteEffects.None, 0);*/
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
        private static bool checkingInstractTilesForDraw(Tile emptyTile, Tile shapeTile)
        {
            return shapeTile.getCartasianRectangle().Intersects(emptyTile.getCartasianRectangle());
        }

        // checking match between tiles at Draw metod
        private static bool chekingMatchTilesForDraw(Tile emptyTile, Tile shapeTile)
        {
            return (shapeTile.getCartasianRectangle().Center.X >
                                                        emptyTile.getCartasianRectangle().Center.X) &&
                                                        (shapeTile.getCartasianRectangle().Center.Y >
                                                        emptyTile.getCartasianRectangle().Center.Y);
        }

        public  Board getEmptyBoard()
        {
            return bigEmptyBoard;
        }
    }
}