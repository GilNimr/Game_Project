using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our_Project.States_and_state_related
{
    public sealed class BuildingBoardState : BaseGameState, IBuildingBoardState
    {
        //GraphicsDeviceManager graphics;
       // SpriteBatch spriteBatch;
        Board bigEmptyBoard;
        List<Board> shapes;

        public BuildingBoardState(Game game) : base(game)
        {
            game.Services.AddService(typeof(IBuildingBoardState), this);

            //graphics = new GraphicsDeviceManager(this);
            //Content.RootDirectory = "Content";
            //IsMouseVisible = true;
 /*
            OurGame.graphics.PreferredBackBufferHeight = 2400;
            Game1.screen_height = OurGame.graphics.PreferredBackBufferHeight;
            OurGame.graphics.PreferredBackBufferWidth = 2400;
            Game1.screen_width = OurGame.graphics.PreferredBackBufferWidth;
   */         
            buildEmptyBoard();

            createShapes();

        }

        private void createShapes()
        {
            Texture2D g2d = Content.Load<Texture2D>("Gray_2d_Tile");
            Texture2D gIs = Content.Load<Texture2D>("Gray_Isometric_Tile");

            List<List<NodeOFHidenTiles>> allHidenPoints = setHidenTiles();

            setShapesBoards(g2d, gIs, allHidenPoints);
        }

        private void setShapesBoards(Texture2D g2d, Texture2D gIs, List<List<NodeOFHidenTiles>> allHidenPoints)
        {
            shapes = new List<Board>();

            int space_between_shapes = 50;

            shapes.Add(new Board(allHidenPoints[0], 2, 3, 0, 0, gIs, g2d, false, this.Content));

            shapes.Add(new Board(allHidenPoints[1], 1, 4, shapes[0].getEndOfXaxisOfLastTile() +
                space_between_shapes, shapes[0].getEndOfYaxisOfLastTile() + space_between_shapes,
                gIs, g2d, false, this.Content));

            shapes.Add(new Board(allHidenPoints[2], 4, 2, shapes[1].getEndOfXaxisOfLastTile() +
                space_between_shapes, shapes[1].getEndOfYaxisOfLastTile() + space_between_shapes,
                gIs, g2d, false, this.Content));

            shapes.Add(new Board(allHidenPoints[3], 3, 2, shapes[2].getEndOfXaxisOfLastTile() +
                space_between_shapes, shapes[2].getEndOfYaxisOfLastTile() + space_between_shapes,
                gIs, g2d, false, this.Content));
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

           Texture2D emptyTile2d =  Content.Load<Texture2D>(@"Textures\Tiles\White_2d_Tile");
            Texture2D emptyTileIso = Content.Load<Texture2D>(@"Textures\Tiles\White_Isometric_Tile");

            bigEmptyBoard = new Board(20, emptyTileIso, emptyTile2d);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);

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
                shape.Update();
            }
        }

        private void putShapeAtNewPosition()
        {
            foreach (Tile[] emptyTilesLine in bigEmptyBoard.getBoard())
            {
                foreach (Tile emptyTile in emptyTilesLine)
                {
                    foreach (Board shape in shapes)
                    {
                        foreach (Tile[] shapeTilesLine in shape.getBoard())
                        {
                            foreach (Tile shapeTile in shapeTilesLine)
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
                                        shapeTile.setToOldRectangle(emptyTile.getOldRectangle().X,
                                            emptyTile.getOldRectangle().Y);
                                    }
                                }
                            }
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

            OurGame.spriteBatch.Begin();

            bigEmptyBoard.Draw(OurGame.spriteBatch, Color.White);


            foreach (Tile[] emptyTilesLine in bigEmptyBoard.getBoard())
            {
                foreach (Tile emptyTile in emptyTilesLine)
                {
                    foreach (Board shape in shapes)
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
            foreach (Board shape in shapes)
            {
                shape.Draw(OurGame.spriteBatch, Color.White);
            }




            base.Draw(gameTime);

            OurGame.spriteBatch.End();
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
