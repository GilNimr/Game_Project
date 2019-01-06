using System;
using System.Collections.Generic;
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

        public static Dictionary<int, Tile> tileDictionary;

        // public Pawn[] pawns;           // the pawns
        private bool pawnTryToMove;
        public Player player;
        public Player enemy;
        public Connection connection;

        public static bool i_am_second_player=false;

        public int gridSize = 8;
        public static int tileSize = 20;

        public PlayingState(Game game)
           : base(game)
        {
            game.Services.AddService(typeof(IPlayingState), this);
            pawnTryToMove = false;

            player = new Player();
            player.myTurn = true;
            enemy = new Player();
            connection = new Connection(player, enemy);
            tileDictionary = new Dictionary<int, Tile>();

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
            player.pawns = new Pawn[gridSize * 3];
            enemy.pawns = new Pawn[gridSize * 3];

          

            int pawnsIndex = 0;
            int enemypawnsIndex = 0;
            int id = 100;

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

                    Rectangle rec = new Rectangle((250) + x, y-(218/4), tileSize, tileSize);
                    tile_matrix[i][j] = new Tile(Tile_texture, cartasian_texture, rec,id);
                    tileDictionary.Add(id, tile_matrix[i][j]);
                    id++;


                    // the user army:
                    if (j > gridSize - 4)
                    {
                        tile_matrix[i][j].occupied = Tile.Occupied.yes_by_me;
                       player. pawns[pawnsIndex] = new Pawn(Pawn_texture, tile_matrix[i][j]);
                        pawnsIndex++;
                    }

                    // the enemy army:
                    if (j < 3)
                    {
                        tile_matrix[i][j].occupied = Tile.Occupied.yes_by_enemy;
                        enemy.pawns[enemypawnsIndex] = new Pawn(Pawn_texture, tile_matrix[i][j]);
                        enemypawnsIndex++;
                    }
                   

                }

               
            }
            connection.update();
            if (i_am_second_player)
            {
                Pawn[] swap_pawns = new Pawn[gridSize * 3];
                swap_pawns = player.pawns;
                player.pawns = enemy.pawns;
                enemy.pawns = swap_pawns;
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

      

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            
            connection.update();
          

            
            for (int i = 0; i < player.pawns.Length; i++)
            {
               player. pawns[i].Update();

                if (player.pawns[i].isMouseClicked) // if this pawn was clicked before
                {
                    for (int j = 0; j < player.pawns.Length; j++)
                    {
                        if (i!=j) // so the other will canceled
                            player.pawns[j].isMouseClicked=false;
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
                    tile_matrix[i][j].Draw(OurGame.spriteBatch, Color.White);
                }
            }

            for (int i = 0; i < player.pawns.Length; i++)
                player.pawns[i].Draw(OurGame.spriteBatch);

            for (int i = 0; i < enemy.pawns.Length; i++)
                enemy.pawns[i].Draw(OurGame.spriteBatch);
        }





    }
}