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
        public static bool win = false;
        public static bool lose = false;
        private Texture2D Tile_texture; // the square
        private Texture2D cartasian_texture;
        private Texture2D teleport_texture;
        private Texture2D Pawn_texture; // the character of user team
        private Tile[][] tile_matrix;   // the board of the game

        // private Pawn[] pawns;           // the pawns

        NodeOFHidenTiles[] hidenTiles;  // an array that include all the tiles are hiden for build the shape
        Shape[] shapes;                 // all the shapes we going to use

        public static Tile[] teleports; // array of all the teleports tiles.

        public int gridSize = 200;        // size of the whole board


        SpriteFont font_small;
     //   private Texture2D israel_spritesheet;
        public static int tileSize = Game1.screen_height / 30;


        ScrollingBackgroundManager scrollingBackgroundManager;

        public static Dictionary<int, Tile> tileDictionary;



        public Player player;
        public Player enemy;
        public Connection connection;

        public static bool i_am_second_player=false;
        

        public PlayingState(Game game)
           : base(game)
        {
            scrollingBackgroundManager = new ScrollingBackgroundManager(game, "Textures\\");
            game.Components.Add(scrollingBackgroundManager);
            game.Services.AddService(typeof(IPlayingState), this);
          // game.Services.AddService(typeof(IScrollingBackgroundManager), this);
            teleports = new Tile[2];

            player = new Player();
            player.myTurn = true;
            enemy = new Player();
            connection = new Connection(ref player, ref enemy);

            tileDictionary = new Dictionary<int, Tile>();
        }

        protected override void LoadContent()
        {
            scrollingBackgroundManager.AddBackground("space", "backgroundSpace", new Vector2(0, 0), new Rectangle(0, 0, 1024, 1024), 30, 0.5f, Color.White);
            scrollingBackgroundManager.AddBackground("space2", "backgroundSpace", new Vector2(0, 1023), new Rectangle(0, 0, 1024, 1024), 30, 0.5f, Color.White);
            scrollingBackgroundManager.AddBackground("space3", "backgroundSpace", new Vector2(0, 2047), new Rectangle(0, 0, 1024, 1024), 30, 0.5f, Color.White);
            //Loading fonts.
            font_small = Content.Load<SpriteFont>(@"Fonts\KaushanScript");


            

            //Gray tile texture.
            Tile_texture = Content.Load<Texture2D>(@"Textures\Tiles\grass_tile_iso5");
            cartasian_texture = Content.Load<Texture2D>(@"Textures\Tiles\Gray_Tile");
            teleport_texture = Content.Load<Texture2D>(@"Textures\Tiles\teleport");


            //pawn texture.
            Pawn_texture = Content.Load<Texture2D>(@"Textures\Pawns\death");

            //creating a jagged 2d array to store tiles and the array of pawns to be user army
            tile_matrix = new Tile[gridSize][];



            for (int i = 0; i < gridSize; i++)
            {
                tile_matrix[i] = new Tile[gridSize];
            }

            // manualy build the shapes:
            hidenTiles = new NodeOFHidenTiles[1];
           hidenTiles[0] = new NodeOFHidenTiles(20, 20);


            /*
             *  for draw you board to the left, you need to put "true" at the boolean variable, and
             *  to fill the currect position to the endY and endX
             * 
             */


            shapes = new Shape[2];              
            shapes[0] = new Shape(hidenTiles, 10, 10, Tile_texture, cartasian_texture, 0, 0, false,0);

           
            
            shapes[1] = new Shape(hidenTiles, 10, 10, Tile_texture, cartasian_texture, 0+shapes[0].endX+tileSize,
                                        0, true,5000);



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



                        skip = false;
                        }
                    }
                }

                //manually putting teleports for now
            tileDictionary[75].teleport_tile = true;
            tileDictionary[75].texture = teleport_texture;
            teleports[0] = tileDictionary[75];
            tileDictionary[5025].teleport_tile = true;
            tileDictionary[5025].texture = teleport_texture;
            teleports[1] = tileDictionary[5025];

            player.pawns = new Pawn[player.army_size];
            enemy.pawns = new Pawn[player.army_size];
            //manually putting pawns for now.
            player.pawns[0] = new Pawn(OurGame, Pawn_texture, tileDictionary[0], 0, Pawn.Team.my_team,0,font_small);
            player.pawns[1] = new Pawn(OurGame, Pawn_texture, tileDictionary[1], 1, Pawn.Team.my_team,1,font_small);
            player.pawns[2] = new Pawn(OurGame, Pawn_texture, tileDictionary[2], 2, Pawn.Team.my_team,2,font_small);
            player.pawns[3] = new Pawn(OurGame, Pawn_texture, tileDictionary[3], 3, Pawn.Team.my_team,3,font_small);
            player.pawns[4] = new Pawn(OurGame, Pawn_texture, tileDictionary[4], 21, Pawn.Team.my_team, 4, font_small);
            player.pawns[5] = new Pawn(OurGame, Pawn_texture, tileDictionary[5], 4, Pawn.Team.my_team, 5, font_small);
            player.pawns[6] = new Pawn(OurGame, Pawn_texture, tileDictionary[6], 5, Pawn.Team.my_team, 6, font_small);
            player.pawns[7] = new Pawn(OurGame, Pawn_texture, tileDictionary[7], 6, Pawn.Team.my_team, 7, font_small);
            player.pawns[8] = new Pawn(OurGame, Pawn_texture, tileDictionary[8], 7, Pawn.Team.my_team, 8, font_small);
            player.pawns[9] = new Pawn(OurGame, Pawn_texture, tileDictionary[9], 8, Pawn.Team.my_team, 9, font_small);
            player.pawns[10] = new Pawn(OurGame, Pawn_texture, tileDictionary[10], 9, Pawn.Team.my_team, 10, font_small);
            player.pawns[11] = new Pawn(OurGame, Pawn_texture, tileDictionary[11], 10, Pawn.Team.my_team, 11, font_small);
            player.pawns[12] = new Pawn(OurGame, Pawn_texture, tileDictionary[12], 11, Pawn.Team.my_team, 12, font_small);
            player.pawns[13] = new Pawn(OurGame, Pawn_texture, tileDictionary[13], 12, Pawn.Team.my_team, 13, font_small);
            player.pawns[14] = new Pawn(OurGame, Pawn_texture, tileDictionary[14], 13, Pawn.Team.my_team, 14, font_small);
            player.pawns[15] = new Pawn(OurGame, Pawn_texture, tileDictionary[15], 14, Pawn.Team.my_team, 15, font_small);
            player.pawns[16] = new Pawn(OurGame, Pawn_texture, tileDictionary[16], 15, Pawn.Team.my_team, 16, font_small);
            player.pawns[17] = new Pawn(OurGame, Pawn_texture, tileDictionary[17], 16, Pawn.Team.my_team, 17, font_small);
            player.pawns[18] = new Pawn(OurGame, Pawn_texture, tileDictionary[18], 17, Pawn.Team.my_team, 18, font_small);
            player.pawns[19] = new Pawn(OurGame, Pawn_texture, tileDictionary[19], 18, Pawn.Team.my_team, 19, font_small);
           

            //manually putting pawns for now.
            enemy.pawns[0] = new Pawn(OurGame, Pawn_texture, tileDictionary[5090], 0, Pawn.Team.enemy_team,0,font_small);
            enemy.pawns[0].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[1] = new Pawn(OurGame, Pawn_texture, tileDictionary[5091], 1, Pawn.Team.enemy_team,1,font_small);
            enemy.pawns[1].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[2] = new Pawn(OurGame, Pawn_texture, tileDictionary[5092], 2, Pawn.Team.enemy_team,2,font_small);
            enemy.pawns[2].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[3] = new Pawn(OurGame, Pawn_texture, tileDictionary[5093], 3, Pawn.Team.enemy_team,3,font_small);
            enemy.pawns[3].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[4] = new Pawn(OurGame, Pawn_texture, tileDictionary[5094], 21, Pawn.Team.enemy_team,4, font_small);
            enemy.pawns[4].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[5] = new Pawn(OurGame, Pawn_texture, tileDictionary[5095], 4, Pawn.Team.enemy_team, 5, font_small);
            enemy.pawns[5].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[6] = new Pawn(OurGame, Pawn_texture, tileDictionary[5096], 5, Pawn.Team.enemy_team, 6, font_small);
            enemy.pawns[6].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[7] = new Pawn(OurGame, Pawn_texture, tileDictionary[5097], 6, Pawn.Team.enemy_team, 7, font_small);
            enemy.pawns[7].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[8] = new Pawn(OurGame, Pawn_texture, tileDictionary[5098], 7, Pawn.Team.enemy_team, 8, font_small);
            enemy.pawns[8].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[9] = new Pawn(OurGame, Pawn_texture, tileDictionary[5099], 8, Pawn.Team.enemy_team, 9, font_small);
            enemy.pawns[9].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[10] = new Pawn(OurGame, Pawn_texture, tileDictionary[5080], 9, Pawn.Team.enemy_team, 10, font_small);
            enemy.pawns[10].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[11] = new Pawn(OurGame, Pawn_texture, tileDictionary[5081], 10, Pawn.Team.enemy_team, 11, font_small);
            enemy.pawns[11].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[12] = new Pawn(OurGame, Pawn_texture, tileDictionary[5082], 11, Pawn.Team.enemy_team, 12, font_small);
            enemy.pawns[12].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[13] = new Pawn(OurGame, Pawn_texture, tileDictionary[5083], 12, Pawn.Team.enemy_team, 13, font_small);
            enemy.pawns[13].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[14] = new Pawn(OurGame, Pawn_texture, tileDictionary[5084], 13, Pawn.Team.enemy_team, 14, font_small);
            enemy.pawns[14].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[15] = new Pawn(OurGame, Pawn_texture, tileDictionary[5085], 14, Pawn.Team.enemy_team, 15, font_small);
            enemy.pawns[15].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[16] = new Pawn(OurGame, Pawn_texture, tileDictionary[5086], 15, Pawn.Team.enemy_team, 16, font_small);
            enemy.pawns[16].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[17] = new Pawn(OurGame, Pawn_texture, tileDictionary[5087], 16, Pawn.Team.enemy_team, 17, font_small);
            enemy.pawns[17].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[18] = new Pawn(OurGame, Pawn_texture, tileDictionary[5088], 17, Pawn.Team.enemy_team, 18, font_small);
            enemy.pawns[18].current_tile.occupied = Tile.Occupied.yes_by_enemy;
            enemy.pawns[19] = new Pawn(OurGame, Pawn_texture, tileDictionary[5089], 18, Pawn.Team.enemy_team, 19, font_small);
            enemy.pawns[19].current_tile.occupied = Tile.Occupied.yes_by_enemy;




            //initializing Tiles neighbors.
            for (int i = 0; i < tile_matrix.Length; ++i)
            {
                for (int j = 0; j < tile_matrix[i].Length; ++j)
                {
                    if (tile_matrix[i][j] != null)
                    {
                        //right
                        if (i < tile_matrix.Length - 1)
                            tile_matrix[i][j].setRight(tile_matrix[i + 1][j]); // x axis grow up

                        //left
                        if (i >= 1)
                            tile_matrix[i][j].setLeft(tile_matrix[i - 1][j]); // x axis go down

                        //down
                        if (j < tile_matrix[i].Length - 1)
                            tile_matrix[i][j].setDown(tile_matrix[i][j + 1]); // y axis grow up
                         //up
                        if (j >= 1)
                            tile_matrix[i][j].setUp(tile_matrix[i][j - 1]); // y axis go down
                    }
                    
                }
            }


            connection.update();
            if (i_am_second_player)
            {

            //    changeTilematrix();

                Pawn[] swap_pawns = new Pawn[player.army_size];
                swap_pawns = player.pawns;
                player.pawns = enemy.pawns;
                enemy.pawns = swap_pawns;
                player.myTurn = false;

                for (int i = 0; i < player.pawns.Length; i++)
                {
                    player.pawns[i].team = Pawn.Team.my_team;
                    player.pawns[i].current_tile.occupied = Tile.Occupied.yes_by_me;
                    enemy.pawns[i].team = Pawn.Team.enemy_team;
                    enemy.pawns[i].current_tile.occupied = Tile.Occupied.yes_by_enemy;
                }
            }
            

        }

      

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            scrollingBackgroundManager.ScrollRate = -1f;

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
                    if (player.myTurn)
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
                    else if (player.pawns[i].attacked)
                    {
                        player.pawns[i].GettingAttacked();
                    }
                        
                }
            }

        }
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);


            scrollingBackgroundManager.Draw("space", OurGame.spriteBatch);
            scrollingBackgroundManager.Draw("space2", OurGame.spriteBatch);
            scrollingBackgroundManager.Draw("space3", OurGame.spriteBatch);

            for (int i = 0; i < tile_matrix.Length; ++i)
            {
                for (int j = 0; j < tile_matrix[i].Length; ++j)
                {
                    if (tile_matrix[i][j] != null)
                    {
                       
                        tile_matrix[i][j].Draw(OurGame.spriteBatch);
                        tile_matrix[i][j].setColor(Color.White); //returning to default color in case it was changed.
                    }
                }
            }


            for (int i = 0; i < player.pawns.Length; i++)
                player.pawns[i].Draw(OurGame.spriteBatch,gameTime);

            for (int i = 0; i < enemy.pawns.Length; i++)
                enemy.pawns[i].Draw(OurGame.spriteBatch,gameTime);


            if (player.myTurn)
            {
                OurGame.spriteBatch.DrawString(font_small, "your turn", new Vector2(Game1.screen_width / 3, Game1.screen_height / 80), Color.White);
            }
            else
                OurGame.spriteBatch.DrawString(font_small, "opponent's turn", new Vector2(Game1.screen_width / 3, Game1.screen_height / 80), Color.White);

            if(win)
                OurGame.spriteBatch.DrawString(font_small, "You win", new Vector2(Game1.screen_width / 3, Game1.screen_height / 10), Color.White);
            if(lose)
                OurGame.spriteBatch.DrawString(font_small, "You lose", new Vector2(Game1.screen_width / 3, Game1.screen_height / 10), Color.White);
        }
        
    }
}