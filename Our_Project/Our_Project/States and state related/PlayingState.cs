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

       // private Pawn[] pawns;           // the pawns
        NodeOFHidenTiles[] hidenTiles;  // an array that include all the tiles are hiden for build the shape
        Shape[] shapes;                 // all the shapes we going to use
      
        
        public int gridSize = 50;        // size of the whole board


        public static int tileSize = Game1.screen_height / 80;
        

        public static Dictionary<int, Tile> tileDictionary;

        // public Pawn[] pawns;           // the pawns
        private bool pawnTryToMove;
        public Player player;
        public Player enemy;
        public Connection connection;

        public static bool i_am_second_player=false;

        
        
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

        //   pawns = new Pawn[gridSize * 3];     // need to change the size - pawns array

            


            for (int i = 0; i < gridSize; i++)
            {
                tile_matrix[i] = new Tile[gridSize];
            }

            // manualy build the shapes:
            hidenTiles = new NodeOFHidenTiles[1];
           hidenTiles[0] = new NodeOFHidenTiles(22, 22);


            /*
             *  for draw you board to the left, you need to put "true" at the boolean variable, and
             *  to fill the currect position to the endY and endX
             * 
             */


            shapes = new Shape[2];              
            shapes[0] = new Shape(hidenTiles, 25, 25, Tile_texture, cartasian_texture, 0, 0, false,0);

            hidenTiles[0].i = 11;
            hidenTiles[0].j = 11; // (2,3)
            
            shapes[1] = new Shape(hidenTiles, 25, 25, Tile_texture, cartasian_texture, 0+shapes[0].endX+tileSize,
                                        0, true,5000);

        /*            Rectangle rec = new Rectangle((250) + x, y-(218/4), tileSize, tileSize);
                    tile_matrix[i][j] = new Tile(Tile_texture, cartasian_texture, rec,id);
                    tileDictionary.Add(id, tile_matrix[i][j]);
                    id++;
>>>>>>> master*/

            bool skip;
            
                for (int indexOfShape = 0; indexOfShape < shapes.Length; indexOfShape++)
                {
                skip = false;

                    for (int i = 0; i < gridSize; ++i)
                    {
     
                        if (i >= shapes[indexOfShape].width)
                            skip = true;

                        for (int j = 0; j < gridSize; ++j)
                        {
                        if (skip)
                            break;
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
                                        if (!shapes[indexOfShape].addToLeft)
                                        {
                                            //int _i = i + shapes[indexOfShape - 1].width;
                                            int _j = j + shapes[indexOfShape - 1].height;

                                            /*if (i >= tile_matrix.Length || _j >= tile_matrix.Length)
                                                break;*/

                                            tile_matrix[i][_j] = shapes[indexOfShape].shapeBoard[i][j];
                                        }
                                        else
                                        {
                                            int _i = i + shapes[indexOfShape - 1].width;
                                            tile_matrix[_i][j] = shapes[indexOfShape].shapeBoard[i][j];
                                        }
                                        
                                    }
                                }
                            }



                         /*   // the user army:

                            //if ((j > gridSize - 4) && (tile_matrix[i][j] != null))
                            if (indexOfShape==1 &&(j<4)&&(i==0))  // manually putting the pawns
                            {

                              tile_matrix[i][j].occupied = Tile.Occupied.yes_by_me;
                            
                                player.pawns[pawnsIndex] = new Pawn(Pawn_texture, tile_matrix[i][j],j,Pawn.Team.my_team);
                               tile_matrix[i][j].current_pawn = player.pawns[pawnsIndex];
                               pawnsIndex++;
                          
                            }

                        // the enemy army:
                        if (indexOfShape == 0 && (j <4 ) && (i == 5))  // put manual the pawns
                        {
                           tile_matrix[i][j].occupied = Tile.Occupied.yes_by_enemy;
                            enemy.pawns[enemypawnsIndex] = new Pawn(Pawn_texture, tile_matrix[i][j],j,Pawn.Team.enemy_team);
                            tile_matrix[i][j].current_pawn = enemy.pawns[enemypawnsIndex];
                            enemypawnsIndex++;
                            
                        }*/
                        skip = false;
                        }
                    }
                }

            player.pawns = new Pawn[player.army_size];
            enemy.pawns = new Pawn[player.army_size];
            //manually putting pawns for now.
            player.pawns[0] = new Pawn(Pawn_texture, tileDictionary[0], 0, Pawn.Team.my_team);
            player.pawns[1] = new Pawn(Pawn_texture, tileDictionary[1], 1, Pawn.Team.my_team);
            player.pawns[2] = new Pawn(Pawn_texture, tileDictionary[2], 2, Pawn.Team.my_team);
            player.pawns[3] = new Pawn(Pawn_texture, tileDictionary[3], 3, Pawn.Team.my_team);

            //manually putting pawns for now.
            enemy.pawns[0] = new Pawn(Pawn_texture, tileDictionary[51], 0, Pawn.Team.my_team);
            enemy.pawns[1] = new Pawn(Pawn_texture, tileDictionary[52], 1, Pawn.Team.my_team);
            enemy.pawns[2] = new Pawn(Pawn_texture, tileDictionary[53], 2, Pawn.Team.my_team);
            enemy.pawns[3] = new Pawn(Pawn_texture, tileDictionary[54], 3, Pawn.Team.my_team);
            



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

            connection.update();
            if (i_am_second_player)
            {
                changeTilematrix();
                Pawn[] swap_pawns = new Pawn[player.army_size];
                swap_pawns = player.pawns;
                player.pawns = enemy.pawns;
                enemy.pawns = swap_pawns;
            }

        }

      

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            
            connection.update();
          

            
            for (int i = 0; i < player.pawns.Length; i++)
            {

                /*             if (pawns[i] != null)
                             {
                                 pawns[i].Update();

                                 if (pawns[i].isMouseClicked) // if this pawn was clicked before
                                 {
                                     for (int j = 0; j < pawns.Length; j++)
                                     {
                                         if (pawns[j] != null &&  i!=j) // so the other will canceled
                                             pawns[j].isMouseClicked=false;
                                     }*/
                if (player.pawns[i] != null)
                {
                    player.pawns[i].Update();

                    if (player.pawns[i].isMouseClicked) // if this pawn was clicked before
                    {
                        for (int j = 0; j < player.pawns.Length; j++)
                        {
                            if (i != j) // so the other will canceled
                                player.pawns[j].isMouseClicked = false;

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


            for (int i = 0; i < player.pawns.Length; i++)
                player.pawns[i].Draw(OurGame.spriteBatch);

            for (int i = 0; i < enemy.pawns.Length; i++)
                enemy.pawns[i].Draw(OurGame.spriteBatch);

        }

        public void changeTilematrix()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Tile tmp = tile_matrix[i][j];
                    tile_matrix[i][j] = tile_matrix[gridSize - 1 - i][gridSize - 1 - j];
                    tile_matrix[gridSize - 1 - i][gridSize - 1 - j] = tmp;


                   /* Rectangle tmpRec = new Rectangle();
                    if(tile_matrix[i][j]!=null)
                    tmpRec = tile_matrix[i][j].Rec;

                    Rectangle tmpRec2 = new Rectangle();
                    if (tile_matrix[gridSize - 1 - i][gridSize - 1 - j] != null)
                        tmpRec2 = tile_matrix[gridSize - 1 - i][gridSize - 1 - j].Rec;


                   
                    if(tile_matrix[i][j]!=null && tmpRec!=null)
                    tile_matrix[i][j].Rec = tmpRec2 ;

                  
                    if(tile_matrix[gridSize - 1 - i][gridSize - 1 - j]!=null && tmpRec2!=null)
                    tile_matrix[gridSize - 1 - i][gridSize - 1 - j].Rec = tmpRec;*/
                }
            }
        }



    }
}