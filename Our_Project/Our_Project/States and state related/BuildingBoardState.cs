﻿using Microsoft.Xna.Framework;
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
        Board bigEmptyBoard;
        List<Board> shapes;
        Texture2D g2d;
        Texture2D gIs;
        Texture2D emptyTile2d;
        Texture2D emptyTileIso;
        private SpriteFont font;
        private Button button;
        bool hideShape = true;
        List<List<NodeOFHidenTiles>> allHidenPoints;


        public BuildingBoardState(Game game) : base(game)
        {
            game.Services.AddService(typeof(IBuildingBoardState), this);

            allHidenPoints = setHidenTiles();
            shapes = new List<Board>();

        }

        private void createShapes()
        {
           // List<List<NodeOFHidenTiles>> allHidenPoints = setHidenTiles();

            //setShapesBoards(g2d, gIs, allHidenPoints);

        }



        private void setShapesBoards(Texture2D g2d, Texture2D gIs, List<List<NodeOFHidenTiles>> allHidenPoints)
        {
            shapes = new List<Board>();
            
            shapes.Add(new Board(allHidenPoints[0], 2, 3, 0, 0, gIs, g2d, false, this.Content));
            
            // int space_between_shapes = 50;
            /*shapes.Add(new Board(allHidenPoints[1], 1, 4, shapes[0].getEndOfXaxisOfLastTile() +
                space_between_shapes, shapes[0].getEndOfYaxisOfLastTile() + space_between_shapes,
                gIs, g2d, false, this.Content));

            shapes.Add(new Board(allHidenPoints[2], 4, 2, shapes[1].getEndOfXaxisOfLastTile() +
                space_between_shapes, shapes[1].getEndOfYaxisOfLastTile() + space_between_shapes,
                gIs, g2d, false, this.Content));

            shapes.Add(new Board(allHidenPoints[3], 3, 2, shapes[2].getEndOfXaxisOfLastTile() +
                space_between_shapes, shapes[2].getEndOfYaxisOfLastTile() + space_between_shapes,
                gIs, g2d, false, this.Content));*/
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
            bigEmptyBoard = new Board(24, emptyTileIso, emptyTile2d);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            g2d = OurGame.Content.Load<Texture2D>(@"Textures\Tiles\Gray_Tile - Copy");
            gIs = Content.Load<Texture2D>(@"Textures\Tiles\Gray_Tile(2)");
            emptyTile2d = Content.Load<Texture2D>(@"Textures\Tiles\White_2d_Tile");
            emptyTileIso = Content.Load<Texture2D>(@"Textures\Tiles\White_Isometric_Tile");
            font = Content.Load<SpriteFont>(@"Fonts\ArialSmall");

            button = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(350, 200),
                Text = "First Shape",
            };

            button.Click += clickFirstShape;
            Game.Components.Add(button);

            buildEmptyBoard();
            createShapes();
          
        }

        private void clickFirstShape(object sender, System.EventArgs e)
        {
            

            if (hideShape)
            {
                shapes.Add(new Board(allHidenPoints[0], 2, 3, 0, 0, gIs, g2d, false, this.Content));
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



            foreach (Board shape in shapes)
            {
                if (shape != null)
                {
                    int how_much_tiles_in_shape = shape.getHeight() * shape.getWidth();
                    List<bool> eachShapeHasEmptyTile = new List<bool>();

                    for (int i = 0; i < how_much_tiles_in_shape; i++)
                    {
                        eachShapeHasEmptyTile.Add(false);
                    }
                    int a = 0;
                    List<Tile> shapeTilesToMove = new List<Tile>();
                    List<Tile> emptyTilesToMove = new List<Tile>();

                    foreach (Tile[] shapeTilesLine in shape.getBoard())
                    {
                        foreach (Tile shapeTile in shapeTilesLine)
                        {
                            foreach (Tile[] emptyTilesLine in bigEmptyBoard.getBoard())
                            {
                                foreach (Tile emptyTile in emptyTilesLine)
                                {
                                    if (shapeTile.getOldRectangle().Intersects(emptyTile.getOldRectangle())
                                        && (!shape.getMove()))
                                    {
                                        if ((shapeTile.getOldRectangle().Center.X >
                                            emptyTile.getOldRectangle().Center.X) &&
                                            (shapeTile.getOldRectangle().Center.Y >
                                            emptyTile.getOldRectangle().Center.Y)
                                            )
                                        {
                                            eachShapeHasEmptyTile[a] = true;
                                            emptyTilesToMove.Add(emptyTile);
                                            shapeTilesToMove.Add(shapeTile);
                                            a++;
                                        }
                                    }
                                }
                            }
                        }
                    }


                    bool putShapeAtNewPlace = false;
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
                        for (int i = 0; i < how_much_tiles_in_shape; i++)
                        {
                            shapeTilesToMove[i].setToOldRectangle(emptyTilesToMove[i].getOldRectangle().X,
                                emptyTilesToMove[i].getOldRectangle().Y);
                        }
                    }
                }
            }
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
                                    if (shapeTile.getOldRectangle().Intersects(emptyTile.getOldRectangle()))
                                    {
                                        if ((shapeTile.getOldRectangle().Center.X >
                                            emptyTile.getOldRectangle().Center.X) &&
                                            (shapeTile.getOldRectangle().Center.Y >
                                            emptyTile.getOldRectangle().Center.Y))
                                        {
                                            emptyTile.Draw(OurGame.spriteBatch, Color.Green);
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


            button.Draw(gameTime, OurGame.spriteBatch);

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
