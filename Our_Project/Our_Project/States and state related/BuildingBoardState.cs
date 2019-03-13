using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XELibrary;

namespace Our_Project.States_and_state_related
{
    public sealed class BuildingBoardState : BaseGameState, IBuildingBoardState
    {
        private Texture2D fullTile2d , fullTileIso , emptyTile2d , emptyTileIso;
        private SpriteFont font;   
        private bool hideShape = true;
        private Board bigEmptyBoard;
        private Board dragingShape;
        private List<List<NodeOFHidenTiles>> allHidenPoints;
        private List<Button> buttons;
        private Button firstShape, secondShape, thirdShape, forthShape, fifthShape;
        private Button save_and_start_game, saveYourShapeInBoard;
        private int remainShapesToPutOnBigEmptyBoard;
        ISoundManager soundManager, soundEffect;


        public BuildingBoardState(Game game) : base(game)
        {
            game.Services.AddService(typeof(IBuildingBoardState), this);

            allHidenPoints = setHidenTiles();
            dragingShape = null;
            remainShapesToPutOnBigEmptyBoard = 5;
            soundManager = (ISoundManager)game.Services.GetService(typeof(ISoundManager));
            soundEffect = (ISoundManager)game.Services.GetService(typeof(ISoundManager));
        }

        private static List<List<NodeOFHidenTiles>> setHidenTiles()
        {
            List<List<NodeOFHidenTiles>> allHidenPoints = new List<List<NodeOFHidenTiles>>();
            allHidenPoints.Add(new List<NodeOFHidenTiles>());
            allHidenPoints.Add(new List<NodeOFHidenTiles>());
            allHidenPoints.Add(new List<NodeOFHidenTiles>());
            allHidenPoints.Add(new List<NodeOFHidenTiles>());

            /*
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


            /*
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


            /*
             * ///
             * ///
             * ///
             * ////////
             * 
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


            /*
             *  \\          \\\\\\\\\
             *  \\          \\\\\\\\\
             *  \\     &    \\\\\\\\\
             *  \\          \\\\\\\\\
             *  \\
             */ 

            allHidenPoints[3].Add(new NodeOFHidenTiles(-1, -1));
            allHidenPoints[3].Add(new NodeOFHidenTiles(-1, -1));


            return allHidenPoints;
        }

        private void buildEmptyBoard()
        {
            // set the board
            bigEmptyBoard = new Board(24, emptyTileIso, emptyTile2d);

            // adding a 2/24 shape wich will be the middle of the board
            List<NodeOFHidenTiles> empty = new List<NodeOFHidenTiles>();
            empty.Add(new NodeOFHidenTiles(-1, -1));
            empty.Add(new NodeOFHidenTiles(-1, -1));

            setMiddleLine(new Board(empty, 2, 12, bigEmptyBoard.getBoard()[12][0].getCartasianRectangle().X,
                bigEmptyBoard.getBoard()[12][0].getCartasianRectangle().Y, fullTileIso, null, false, this.Content));
        }

        private void setMiddleLine(Board line)
        {
            Texture2D fullTexture = line.getBoard()[0][0].texture;

            for (int i=11; i<=12; i++)
            {
                for (int j=0; j<bigEmptyBoard.getBoard()[i].Length; j++)
                {
                    bigEmptyBoard.getBoard()[i][j].texture = fullTexture;
                    bigEmptyBoard.getBoard()[i][j].setIsHidden(false);
                }
            }
            setNeighbors(bigEmptyBoard);

        }

        private void addShapeToEmptyBoard(Board s, List<Tile> tilesFromShpae, List<Tile> tilesFromEmpty)
        {   // set new shape in emptyBoard
            int i = 0;
            if (remainShapesToPutOnBigEmptyBoard > 0)
            {
                foreach (Tile tFromEmpty in tilesFromEmpty)
                {
                    if (!tilesFromShpae[i].getIsHidden())
                    {
                        tFromEmpty.texture = tilesFromShpae[i].texture;
                        tFromEmpty.setIsHidden(false);
                    }
                    i++;
                }
                setNeighbors(bigEmptyBoard);
                remainShapesToPutOnBigEmptyBoard--;
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
            setAllContent();
            setAllButtons();
            buildEmptyBoard();
        }

        private void setAllContent()
        {
            fullTile2d = OurGame.Content.Load<Texture2D>(@"Textures\Tiles\Gray_Tile");
            fullTileIso = Content.Load<Texture2D>(@"Textures\Tiles\Gray_Tile_iso");
            emptyTile2d = Content.Load<Texture2D>(@"Textures\Tiles\White_2d_Tile");
            emptyTileIso = Content.Load<Texture2D>(@"Textures\Tiles\White_Isometric_Tile");
            font = Content.Load<SpriteFont>(@"Fonts\KaushanScript");
        }

        protected override void StateChanged(object sender, EventArgs e)  // start music if this state is on screen
        {
            base.StateChanged(sender, e);
            
            if (StateManager.State == this.Value)
                soundManager.Play("backGroundPlayinState");
            /*else
                soundManager.StopSong();*/
        }

        private void setAllButtons()
        {
            buttons = new List<Button>();

            firstShape = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(200, 20),

                Text = "",
                picture = Content.Load<Texture2D>(@"Textures\Controls\Shape1")
                
            };
            firstShape.Click += clickFirstShape;
            buttons.Add(firstShape);


            secondShape = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(200, 40),
                Text = "Second Shape",
            };

            secondShape.Click += clickSecondShape;
            buttons.Add(secondShape);

            thirdShape = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(200, 60),
                Text = "Thirth Shape",
                
            };

            thirdShape.Click += clickThirdShape;
            buttons.Add(thirdShape);

            forthShape = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(200, 80),
                Text = "Fourth Shape",
            };

            forthShape.Click += clickFourthShape;
            buttons.Add(forthShape);

            fifthShape = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(200, 100),
                Text = "Fifth Shape",
            };

            fifthShape.Click += clickFifthShape;
            buttons.Add(fifthShape);


            save_and_start_game = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(Game1.screen_width - 500, 20),
                Text = "Save and start game",
            };

            save_and_start_game.Click += saveAndStartGame; 
            buttons.Add(save_and_start_game);

            foreach(Button b in buttons)
                Game.Components.Add(b);
        }

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

        private void saveAndStartGame(object sender, EventArgs e)
        {
            soundEffect.Play("click");
            buttons.Clear();
            StateManager.ChangeState(OurGame.PlacingSoldiersState.Value);
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
        public override void Update(GameTime gameTime)
        {
            updateAllShapes();

            putShapeAtNewPosition();

            base.Update(gameTime);
        }

        private void updateAllShapes()
        {
            if (dragingShape != null)
                dragingShape.Update();
        }

        private void putShapeAtNewPosition()
        {
            bool putShapeAtNewPlace;
            List<Tile> shapeTilesToMove = new List<Tile>();
            List<Tile> emptyTilesToMove = new List<Tile>();
            Board shape=null;

                if (dragingShape != null)
                {
                    shape = dragingShape;
                    List<bool> eachShapeHasEmptyTile = new List<bool>();

                    for (int i = 0; i < shape.getBoard().Length; i++)
                    {
                        for (int j=0; j< shape.getBoard()[i].Length; j++)
                            if (!shape.getBoard()[i][j].getIsHidden())
                                eachShapeHasEmptyTile.Add(false);
                    }

                    int a = 0;

                    foreach (Tile[] shapeTilesLine in shape.getBoard())
                    {
                        foreach (Tile shapeTile in shapeTilesLine)
                        {
                            foreach (Tile[] emptyTilesLine in bigEmptyBoard.getBoard())
                            {
                                foreach (Tile emptyTile in emptyTilesLine)
                                {
                                    if (shapeTile.getCartasianRectangle().Intersects(emptyTile.getCartasianRectangle())
                                        && (!shape.getMove()) && !shapeTile.getIsHidden() && emptyTile.getIsHidden())
                                    {
                                        if ((shapeTile.getCartasianRectangle().Center.X >
                                            emptyTile.getCartasianRectangle().Center.X) &&
                                            (shapeTile.getCartasianRectangle().Center.Y >
                                            emptyTile.getCartasianRectangle().Center.Y) && emptyTile.getIsHidden()
                                            )
                                        {
                                            eachShapeHasEmptyTile[a] = true;
                                            emptyTilesToMove.Add(emptyTile);
                                            shapeTilesToMove.Add(shapeTile);
                                            a++;
                                        }
                                    }
                                    if (shape.getMove())
                                    {
                                        buttons.Remove(saveYourShapeInBoard);
                                        Game.Components.Remove(saveYourShapeInBoard);
                                    }
                                }
                            }
                        }
                    }

                    
                    putShapeAtNewPlace = false;
                    for (int i = 0; i < eachShapeHasEmptyTile.Count; i++)
                    {
                        if (!eachShapeHasEmptyTile[i])
                        {
                            putShapeAtNewPlace = false;
                            break;
                        }
                        putShapeAtNewPlace = true;
                    }

                    if (putShapeAtNewPlace)
                    {

                        saveYourShapeInBoard = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
                        {
                            Position = new Vector2(Game1.screen_width - 1000, 20),
                            Text = "Save your shape in board",
                        };
                        buttons.Add(saveYourShapeInBoard);
                        Game.Components.Add(saveYourShapeInBoard);

                        for (int i = 0; i < shapeTilesToMove.Count; i++)
                        {
                            shapeTilesToMove[i].setToCartasianRectangle(emptyTilesToMove[i].getCartasianRectangle().X,
                                emptyTilesToMove[i].getCartasianRectangle().Y);
                        }


                        if (legalPlace(shape, shapeTilesToMove, emptyTilesToMove))
                        {
                            soundEffect.Play("click");
                            saveYourShapeInBoard.Click += (sender2, e2) => saveShapeAtNewPlace(sender2, e2,
                                shape, shapeTilesToMove, emptyTilesToMove);
                            setNeighbors(bigEmptyBoard);
                        }
                        else
                            soundEffect.Play("badPlace");
                    }
                }
        }

        private void setNeighbors(Board b)
        {
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
        {
            for (int i=0; i<shapeTiles.Count; i++)
            {
                Tile tFromShape = shape.boardDictionaryById[shapeTiles[i].getId()];
                Tile tFromEmpty = bigEmptyBoard.boardDictionaryById[empty[i].getId()];

                if (tFromEmpty.getId() < 286)
                    return false;

                if (tFromEmpty.getLeft() != null && !tFromEmpty.getLeft().getIsHidden())
                {
                    if (tFromEmpty.getUp() != null && tFromShape.getUp() != null && !tFromShape.getUp().getIsHidden()
                        && tFromEmpty.getUp().getLeft() != null && !tFromEmpty.getUp().getLeft().getIsHidden()
                        ||
                        tFromEmpty.getDown() != null && tFromShape.getDown() != null && !tFromShape.getDown().getIsHidden()
                        && tFromEmpty.getDown().getLeft() != null && !tFromEmpty.getDown().getLeft().getIsHidden())
                    {
                        return true;
                    }
                }
                
                if (tFromEmpty.getRight() != null && !tFromEmpty.getRight().getIsHidden())
                {
                    if (tFromEmpty.getUp() != null && tFromShape.getUp() != null && !tFromShape.getUp().getIsHidden()
                        && tFromEmpty.getUp().getRight() != null && !tFromEmpty.getUp().getRight().getIsHidden()
                        ||
                        tFromEmpty.getDown() != null && tFromShape.getDown() != null && !tFromShape.getDown().getIsHidden()
                        && tFromEmpty.getDown().getRight() != null && !tFromEmpty.getDown().getRight().getIsHidden())
                    {
                        return true;
                    }
                }

                if (tFromEmpty.getUp() != null && !tFromEmpty.getUp().getIsHidden())
                {
                    if (tFromEmpty.getRight() != null && tFromShape.getRight() != null && !tFromShape.getRight().getIsHidden()
                        && tFromEmpty.getRight().getUp() != null && !tFromEmpty.getRight().getUp().getIsHidden()
                        ||
                         tFromEmpty.getLeft() != null && tFromShape.getLeft() != null && !tFromShape.getLeft().getIsHidden()
                        && tFromEmpty.getLeft().getUp() != null && !tFromEmpty.getLeft().getUp().getIsHidden())
                    {
                        return true;
                    }
                }

                if (tFromEmpty.getDown() != null && !tFromEmpty.getDown().getIsHidden())
                {
                    if ( tFromEmpty.getRight() != null && tFromShape.getRight() != null && !tFromShape.getRight().getIsHidden()
                        && tFromEmpty.getRight().getDown() != null && !tFromEmpty.getRight().getDown().getIsHidden()
                        ||
                        tFromEmpty.getLeft() != null && tFromShape.getLeft() != null && !tFromShape.getLeft().getIsHidden()
                        && tFromEmpty.getLeft().getDown() != null && !tFromEmpty.getLeft().getDown().getIsHidden())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        private void saveShapeAtNewPlace(object sender, EventArgs e, Board shape, List<Tile> shapeTiles,
            List<Tile> emptyTiles)
        {
            soundEffect.Play("click");
            addShapeToEmptyBoard(shape, shapeTiles, emptyTiles);
            dragingShape = null;
            GC.Collect();
            hideShape = true;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            bigEmptyBoard.Draw(OurGame.spriteBatch, Color.White);
            
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
                                    if (shapeTile.getCartasianRectangle().Intersects(emptyTile.getCartasianRectangle()))
                                    {
                                        if ((shapeTile.getCartasianRectangle().Center.X >
                                            emptyTile.getCartasianRectangle().Center.X) &&
                                            (shapeTile.getCartasianRectangle().Center.Y >
                                            emptyTile.getCartasianRectangle().Center.Y))
                                        {

                                            if (!shapeTile.getIsHidden())
                                            {
                                                if (!emptyTile.getIsHidden())
                                                {
                                                    emptyTile.setColor(Color.Red);
                                                }
                                                    
                                                else
                                                    emptyTile.setColor(Color.Green);

                                            //for debug purposes
                                            OurGame.spriteBatch.DrawString(font, emptyTile.getId().ToString(),new Vector2( emptyTile.getCartasianRectangle().X,
                                                    emptyTile.getCartasianRectangle().Y), Color.Black, 0, new Vector2(0), 0.8f, SpriteEffects.None, 0);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                }
            }

                if (dragingShape != null)
                    dragingShape.Draw(OurGame.spriteBatch, Color.White);

            foreach(Button b in buttons)
                b.Draw(gameTime, OurGame.spriteBatch);
            
            base.Draw(gameTime);
        }
        
        //translates 2d world coordinates to isometric screen coordinates.
        public static Vector2 TwoD2isometrix(int x, int y)
        {
            int tmpx = x - y;
            int tmpy = (x + y) / 2;
            return new Vector2(tmpx, tmpy);
        }

        //translates isometric screen coordinates to 2d World.
        public static Vector2 Isometrix2twoD(int x, int y)
        {
            int tmpx = (2 * y + x) / 2;
            int tmpy = (2 * y - x) / 2;
            return new Vector2(tmpx, tmpy);
        }
        

        public Board getEmptyBoard()
        {
            return bigEmptyBoard;
        }
    }
}