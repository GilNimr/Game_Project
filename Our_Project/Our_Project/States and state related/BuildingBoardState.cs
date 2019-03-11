using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our_Project.States_and_state_related
{
    public sealed class BuildingBoardState : BaseGameState, IBuildingBoardState
    {
        private Texture2D fullTile2d , fullTileIso , emptyTile2d , emptyTileIso;
        private SpriteFont font;   
        private bool hideShape = true;
        private Board bigEmptyBoard;
        private List<Board> shapes;
        private List<List<NodeOFHidenTiles>> allHidenPoints;
        private List<Button> buttons;
        private Button firstShape, secondShape, thirdShape, forthShape, fifthShape;
        private Button save_and_start_game, saveYourShapeInBoard;




        public BuildingBoardState(Game game) : base(game)
        {
            game.Services.AddService(typeof(IBuildingBoardState), this);

            allHidenPoints = setHidenTiles();
            shapes = new List<Board>();
        }
        
        private void setShapesBoards(Texture2D fullTile2d, Texture2D fullTileIso, List<List<NodeOFHidenTiles>> allHidenPoints)
        {
            shapes.Add(new Board(allHidenPoints[0], 2, 3, 0, 0, fullTileIso, fullTile2d, false, this.Content));

            // int space_between_shapes = 50;
            /*shapes.Add(new Board(allHidenPoints[1], 1, 4, shapes[0].getEndOfXaxisOfLastTile() +
                space_between_shapes, shapes[0].getEndOfYaxisOfLastTile() + space_between_shapes,
                fullTileIso, fullTile2d, false, this.Content));
            shapes.Add(new Board(allHidenPoints[2], 4, 2, shapes[1].getEndOfXaxisOfLastTile() +
                space_between_shapes, shapes[1].getEndOfYaxisOfLastTile() + space_between_shapes,
                fullTileIso, fullTile2d, false, this.Content));
            shapes.Add(new Board(allHidenPoints[3], 3, 2, shapes[2].getEndOfXaxisOfLastTile() +
                space_between_shapes, shapes[2].getEndOfYaxisOfLastTile() + space_between_shapes,
                fullTileIso, fullTile2d, false, this.Content));*/
        }

        private static List<List<NodeOFHidenTiles>> setHidenTiles()
        {
            List<List<NodeOFHidenTiles>> allHidenPoints = new List<List<NodeOFHidenTiles>>();
            allHidenPoints.Add(new List<NodeOFHidenTiles>());
            allHidenPoints.Add(new List<NodeOFHidenTiles>());
            allHidenPoints.Add(new List<NodeOFHidenTiles>());
            allHidenPoints.Add(new List<NodeOFHidenTiles>());

            allHidenPoints[0].Add(new NodeOFHidenTiles(0, 1));
            allHidenPoints[0].Add(new NodeOFHidenTiles(0, 2));

            allHidenPoints[1].Add(new NodeOFHidenTiles(-1, -1));

            allHidenPoints[2].Add(new NodeOFHidenTiles(-1, -1));

            allHidenPoints[3].Add(new NodeOFHidenTiles(0, 1));
            allHidenPoints[3].Add(new NodeOFHidenTiles(2, 1));

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
                }
            }

        }

        private void addShapeToEmptyBoard(Board s, List<Tile> tilesFromShpae, List<Tile> tilesFromEmpty /*int firstI, int endI, int firstJ, int endJ*/)
        {   // set new shape in emptyBoard
            //Texture2D fullTexture = s.getBoard()[0][0].texture;
            int i = 0;
            foreach (Tile tFromEmpty in tilesFromEmpty)
            {
                if (!tilesFromShpae[i].getIsHidden())
                {
                    tFromEmpty.texture = tilesFromShpae[i].texture;
                }
                i++;
               
            }


            /*.setToCartasianRectangle(emptyTilesToMove[i].getCartasianRectangle().X,
                emptyTilesToMove[i].getCartasianRectangle().Y);
            currentShapeToPutAtNewPlace.Add(shapeTilesToMove[i]);*/



            /*
            for (int i=firstI; i<=endI; i++)
            {
                for (int j=firstJ; j<endJ; j++)
                {
                    bigEmptyBoard.getBoard()[i][j].texture = fullTexture;
                }
            } */
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

        private void setAllButtons()
        {
            buttons = new List<Button>();

            firstShape = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(200, 20),
                Text = "First Shape",
            };
            
            firstShape.Click += clickFirstShape;
            buttons.Add(firstShape);
          //  Game.Components.Add(firstShape);
            

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

        private void saveAndStartGame(object sender, EventArgs e)
        {
            StateManager.ChangeState(OurGame.PlayingState.Value);
        }

        private void clickFirstShape(object sender, System.EventArgs e)
        {
            if (hideShape)
            {
                shapes.Add(new Board(allHidenPoints[0], 2, 3, 0, 0, fullTileIso, fullTile2d, false, this.Content));
                hideShape = false;



            }

            else
            {
                shapes.Remove(shapes[0]);
                //                shapes[0] = null;
                hideShape = true;
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
        public override void Update(GameTime gameTime)
        {
            updateAllShapes();

            putShapeAtNewPosition();

            base.Update(gameTime);
        }

        private void updateAllShapes()
        {
            foreach (Board shape in shapes)
            {
                if (shape != null)
                    shape.Update();
            }
        }

        private void putShapeAtNewPosition()
        {
            bool putShapeAtNewPlace;
            List<Tile> shapeTilesToMove = new List<Tile>();
            List<Tile> emptyTilesToMove = new List<Tile>();

            Board shape=null;

                if (shapes.Count>0)
                {

                    shape = shapes[0];
                    int how_much_tiles_in_shape = shape.getHeight() * shape.getWidth();
                    List<bool> eachShapeHasEmptyTile = new List<bool>();

                    for (int i = 0; i < how_much_tiles_in_shape; i++)
                    {
                        eachShapeHasEmptyTile.Add(false);
                    }

                    int a = 0;
                    //List<Tile> shapeTilesToMove = new List<Tile>();
                    //List<Tile> emptyTilesToMove = new List<Tile>();

                    foreach (Tile[] shapeTilesLine in shape.getBoard())
                    {
                        foreach (Tile shapeTile in shapeTilesLine)
                        {
                            foreach (Tile[] emptyTilesLine in bigEmptyBoard.getBoard())
                            {
                                foreach (Tile emptyTile in emptyTilesLine)
                                {
                                    if (shapeTile.getCartasianRectangle().Intersects(emptyTile.getCartasianRectangle())
                                        && (!shape.getMove()))
                                    {
                                        //putShapeAtNewPlace = false;


                                        if ((shapeTile.getCartasianRectangle().Center.X >
                                            emptyTile.getCartasianRectangle().Center.X) &&
                                            (shapeTile.getCartasianRectangle().Center.Y >
                                            emptyTile.getCartasianRectangle().Center.Y)
                                            )
                                        {
                                            eachShapeHasEmptyTile[a] = true;
                                            emptyTilesToMove.Add(emptyTile);
                                            shapeTilesToMove.Add(shapeTile);
                                            a++;


                                            buttons.Remove(saveYourShapeInBoard);
                                            Game.Components.Remove(saveYourShapeInBoard);
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


                    //bool putShapeAtNewPlace = false;


                    putShapeAtNewPlace = false;
                    for (int i = 0; i < how_much_tiles_in_shape; i++)
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

                        for (int i = 0; i < how_much_tiles_in_shape; i++)
                        {
                            shapeTilesToMove[i].setToCartasianRectangle(emptyTilesToMove[i].getCartasianRectangle().X,
                                emptyTilesToMove[i].getCartasianRectangle().Y);

                        }

                    saveYourShapeInBoard.Click += (sender2, e2) => saveShapeAtNewPlace(sender2, e2,
                       shape, shapeTilesToMove, emptyTilesToMove);

                    }

                }
        }
        

        private void saveShapeAtNewPlace(object sender, EventArgs e, Board shape, List<Tile> shapeTiles,
            List<Tile> emptyTiles)
        {
                addShapeToEmptyBoard(shape, shapeTiles, emptyTiles);
            shapes.Remove(shape);
            hideShape = true;
          //  buttons.Remove(firstShape);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //OurGame.spriteBatch.Begin();

            bigEmptyBoard.Draw(OurGame.spriteBatch, Color.White);


            foreach (Tile[] emptyTilesLine in bigEmptyBoard.getBoard())
            {
                foreach (Tile emptyTile in emptyTilesLine)
                {
                    foreach (Board shape in shapes)
                    {
                        if (shape != null)
                        {
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
                                           // emptyTile.Draw(OurGame.spriteBatch, Color.Green);
                                            emptyTile.setColor(Color.Green);
                                        }
                                    }
                                }
                            }
                        }


                    }
                }
            }
            foreach (Board shape in shapes)
            {
                if (shape != null)
                    shape.Draw(OurGame.spriteBatch, Color.White);
            }

            foreach(Button b in buttons)
                b.Draw(gameTime, OurGame.spriteBatch);
            
            base.Draw(gameTime);

            //  OurGame.spriteBatch.End();
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


    }
}