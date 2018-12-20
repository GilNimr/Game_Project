using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using XELibrary;

namespace Our_Project
{


    public sealed class PlayingState : BaseGameState, IPlayingState
    {
        private Texture2D Tile_texture;
        private Texture2D Pawn_texture;
        private Tile[][] tile_matrix;

        private Pawn pawn;

        public int gridSize = 14;
        public int tileSize = 40;

        public PlayingState(Game game)
           : base(game)
        {
            game.Services.AddService(typeof(IPlayingState), this);
        }
        protected override void LoadContent()
        {
            //Gray tile texture.
            Tile_texture = Content.Load<Texture2D>(@"Textures\Tiles\Gray_Tile");

            //pawn texture.
            Pawn_texture = Content.Load<Texture2D>(@"Textures\Pawns\death");

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
                    if (j < tile_matrix[i].Length - 1)
                        tile_matrix[i][j].right = tile_matrix[i][j + 1];
                    //left
                    if (j >= 1)
                        tile_matrix[i][j].left = tile_matrix[i][j - 1];
                    //down
                    if (i < tile_matrix.Length - 1)
                        tile_matrix[i][j].down = tile_matrix[i + 1][j];
                    //up
                    if (i >= 1)
                        tile_matrix[i][j].up = tile_matrix[i - 1][j];
                }
            }

            pawn = new Pawn(Pawn_texture, tile_matrix[7][7]);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            base.Draw(gameTime);

            for (int i = 0; i < tile_matrix.Length; ++i)
            {
                for (int j = 0; j < tile_matrix[i].Length; ++j)
                {
                    tile_matrix[i][j].Draw(OurGame.spriteBatch, Color.White);
                }
            }

            pawn.Draw(OurGame.spriteBatch);

        }





    }
}
