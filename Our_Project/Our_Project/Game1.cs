using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Our_Project
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Texture2D Tile_texture;
        private Texture2D Pawn_texture;
        private Tile[][] tile_matrix;

        private Pawn pawn;

        public int gridSize = 14;
        public int tileSize = 40;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
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

            //Gray tile texture.
            Tile_texture = Content.Load<Texture2D>(@"Tiles\Gray_Tile");

            //pawn texture.
            Pawn_texture = Content.Load<Texture2D>(@"Pawns\death");

            //creating a jagged 2d array to store tiles
            tile_matrix = new Tile[gridSize][];
            for (int i = 0; i < gridSize; i++)
            {
                tile_matrix[i] = new Tile[gridSize];
            }

            for (int i = 0; i < gridSize; ++i)
            {
                for (int j = 0; j < gridSize; ++j)
                {
                    Rectangle rec = new Rectangle(120 + i * tileSize, 20 + j * tileSize, tileSize, tileSize);
                    tile_matrix[i][j] = new Tile(Tile_texture, rec);
                }
            }
            
            //initializing Tiles neighbors.
            for (int i = 0; i < tile_matrix.Length; ++i)
            {
                for (int j = 0; j < tile_matrix[i].Length; ++j)
                {
                    
                    //right
                    if (i < tile_matrix.Length - 1)
                        tile_matrix[i][j].right = tile_matrix[i + 1][j]; // x axis grow up
                    //left
                    if (i >= 1)
                        tile_matrix[i][j].left = tile_matrix[i - 1][j]; // x axis go down
                    //down
                    if (j < tile_matrix[i].Length - 1)
                        tile_matrix[i][j].down = tile_matrix[i][j + 1]; // y axis grow up
                    //up
                    if (j >= 1)
                        tile_matrix[i][j].up = tile_matrix[i][j - 1]; // y axis go down
                }
            }

            pawn = new Pawn(Pawn_texture, tile_matrix[7][7]);

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            pawn.Update();
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

            for (int i = 0; i < tile_matrix.Length; ++i)
            {
                for (int j = 0; j < tile_matrix[i].Length; ++j)
                {
                    tile_matrix[i][j].Draw(spriteBatch, Color.White);
                }
            }

            pawn.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
