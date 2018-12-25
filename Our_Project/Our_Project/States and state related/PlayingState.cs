using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using XELibrary;

namespace Our_Project
{


    public sealed class PlayingState : BaseGameState, IPlayingState
    {
        private Texture2D Tile_texture; // the square
        private Texture2D Pawn_texture; // the character of user team
        private Tile[][] tile_matrix;   // the board of the game
        private Pawn[] pawns;           // the pawns

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
            Tile_texture = Content.Load<Texture2D>(@"Tiles\Gray_Tile");

            //pawn texture.
            Pawn_texture = Content.Load<Texture2D>(@"Pawns\death");

            //creating a jagged 2d array to store tiles and the array of pawns to be user army
            tile_matrix = new Tile[gridSize][];
            pawns = new Pawn[gridSize * 3];
            int pawnsIndex = 0;

            for (int i = 0; i < gridSize; i++)
            {
                tile_matrix[i] = new Tile[gridSize];
            }

            for (int i = 0; i < gridSize; ++i)
            {
                for (int j = 0; j < gridSize; ++j)
                {
                    // the matrix:
                    Rectangle rec = new Rectangle(i * tileSize + 120, j * tileSize + 20, tileSize, tileSize);
                    tile_matrix[i][j] = new Tile(Tile_texture, rec);
                    

                    // the user army:
                    if (j > gridSize - 4)
                    {
                        tile_matrix[i][j].occupied = Tile.Occupied.yes_by_me;
                        pawns[pawnsIndex] = new Pawn(Pawn_texture, tile_matrix[i][j]);
                        pawnsIndex++;
                    }

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

            
        }
        /*
        bool checkNeighbor(int mouseX, int mouseY, Vector2 positionOfNeighbor, Tile t)
        {
            return ((mouseX >= (positionOfNeighbor.X) && (mouseX <= (positionOfNeighbor.X + t.Rec.Width))) &&
                        ((mouseY >= positionOfNeighbor.Y) && (mouseY <= positionOfNeighbor.Y + t.Rec.Height)));
        }*/

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int i=0; i<pawns.Length; i++)
                pawns[i].Update();
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

            for (int i=0; i<pawns.Length; i++)
                pawns[i].Draw(OurGame.spriteBatch);
        }





    }
}