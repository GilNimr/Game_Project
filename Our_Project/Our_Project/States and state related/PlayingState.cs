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
        private Texture2D cartasian_texture;
        private Texture2D Pawn_texture;
        private Tile[][] tile_matrix;

        private Pawn pawn;

        public int gridSize = 14;
        public static int tileSize = 30;

        public PlayingState(Game game)
           : base(game)
        {
            game.Services.AddService(typeof(IPlayingState), this);
        }
        protected override void LoadContent()
        {
            //Gray tile texture.
            Tile_texture = Content.Load<Texture2D>(@"Textures\Tiles\Gray_Tile(2)");
           cartasian_texture = Content.Load<Texture2D>(@"Textures\Tiles\Gray_Tile - Copy");

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
                    int x = i * tileSize;
                    int y = j * tileSize;

                    

                    Rectangle rec = new Rectangle(100+x, y, tileSize , tileSize);
                    tile_matrix[i][j] = new Tile(Tile_texture,cartasian_texture, rec);
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

       

        bool checkNeighbor(int mouseX, int mouseY, Vector2 positionOfNeighbor, Tile t)
        {
            return ((mouseX >= (positionOfNeighbor.X) && (mouseX <= (positionOfNeighbor.X + t.Rec.Width))) &&
                        ((mouseY >= positionOfNeighbor.Y) && (mouseY <= positionOfNeighbor.Y + t.Rec.Height)));
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            pawn.Update();
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
