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
        private Texture2D cartasian_texture;
        private Texture2D Pawn_texture; // the character of user team
        private Tile[][] tile_matrix;   // the board of the game
        private Pawn[] pawns;           // the pawns
        private bool pawnTryToMove;
        NodeOFHidenTiles[] hidenTiles;
        Shape[] shapes;
        Pawn pawnExample;
        
        public int gridSize = 4;
        public static int tileSize = 30;

        public PlayingState(Game game)
           : base(game)
        {
            game.Services.AddService(typeof(IPlayingState), this);
            pawnTryToMove = false;
        }

        protected override void LoadContent()
        {

            //Gray tile texture.
            Tile_texture = Content.Load<Texture2D>(@"Textures\Tiles\Gray_Tile(2)");
            cartasian_texture = Content.Load<Texture2D>(@"Textures\Tiles\Gray_Tile - Copy");

            //pawn texture.
            Pawn_texture = Content.Load<Texture2D>(@"Textures\Pawns\death");

            //creating a jagged 2d array to store tiles and the array of pawns to be user army
            tile_matrix = new Tile[gridSize][];
            pawns = new Pawn[gridSize * 3];
            int pawnsIndex = 0;

            for (int i = 0; i < gridSize; i++)
            {
                tile_matrix[i] = new Tile[gridSize];
            }


            hidenTiles = new NodeOFHidenTiles[1];
            hidenTiles[0] = new NodeOFHidenTiles(0, 1);
            //hidenTiles[1] = new NodeOFHidenTiles (4,2);

            shapes = new Shape[2];
            shapes[0] = new Shape(hidenTiles, 4, 2, Tile_texture, cartasian_texture, 0, 0);

            hidenTiles[0].i = 2;
            hidenTiles[0].j = 1; // (2,3)
          //  hidenTiles[1].i = 3;
            //hidenTiles[1].j = 3;    // (3,3) -> (8, 8)



            shapes[1] = new Shape(hidenTiles, 4, 2, Tile_texture, cartasian_texture, 0,
                shapes[0].endY+tileSize);

            bool skip;
            
                for (int indexOfShape = 0; indexOfShape < shapes.Length; indexOfShape++)
                {

                if (indexOfShape == 1)
                {
                    bool stop = true;
                }

                skip = false;

                    for (int i = 0; i < gridSize; ++i)
                    {
                    
                        if (i >= shapes[indexOfShape].width)
                            skip = true;

                        for (int j = 0; j < gridSize; ++j)
                        {

                            if (j >= shapes[indexOfShape].height)
                                skip = true;

                            if (!skip)
                            {
                                if (shapes[indexOfShape].shapeBoard[i][j] != null)
                                {
                                    if (indexOfShape == 0)
                                        tile_matrix[i][j] = shapes[indexOfShape].shapeBoard[i][j];
                                    else
                                    {
                                        //int _i = i + shapes[indexOfShape - 1].width;
                                        int _j = j + shapes[indexOfShape - 1].height;

                                        if (i >= tile_matrix.Length || _j >= tile_matrix.Length)
                                            break;

                                        tile_matrix[i][_j] = shapes[indexOfShape].shapeBoard[i][j];
                                    }
                                }
                            }



                            // the user army:

                            //if ((j > gridSize - 4) && (tile_matrix[i][j] != null))
                            if (indexOfShape==1 &&(j==3)&&(i==0))
                            {

                                tile_matrix[i][j].occupied = Tile.Occupied.yes_by_me;
                                pawns[pawnsIndex] = new Pawn(Pawn_texture, tile_matrix[i][j]);
                                pawnsIndex++;
                            }

                        skip = false;
                    }
                        

                    }
            }
                

            //initializing Tiles neighbors.
            for (int i = 0; i < tile_matrix.Length; ++i)
            {
                for (int j = 0; j < tile_matrix[i].Length; ++j)
                {
                    if (tile_matrix[i][j] != null)
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



            
            for (int i = 0; i < pawns.Length; i++)
            {
                if (pawns[i] != null)
                {
                    pawns[i].Update();

                    if (pawns[i].isMouseClicked) // if this pawn was clicked before
                    {
                        for (int j = 0; j < pawns.Length; j++)
                        {
                            if (pawns[j] != null &&  i!=j) // so the other will canceled
                                pawns[j].isMouseClicked=false;
                        }
                    }
                }
            }

        }
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

            for (int i = 0; i < tile_matrix.Length; ++i)
            {
                for (int j = 0; j < tile_matrix[i].Length; ++j)
                {
                    if (tile_matrix[i][j] != null)
                        tile_matrix[i][j].Draw(OurGame.spriteBatch, Color.White);
                }
            }

            for (int i = 0; i < pawns.Length; i++)
                if (pawns[i] != null)
                    pawns[i].Draw(OurGame.spriteBatch);
        }





    }
}