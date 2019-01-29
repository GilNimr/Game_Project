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

       // private Pawn[] pawns;           // the pawns

        SpriteFont font_small;

         public Pawn[] pawns;           // the pawns

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
            connection = new Connection(ref player, ref enemy);


        }

        protected override void LoadContent()
        {

            //Loading fonts.
            font_small = Content.Load<SpriteFont>(@"Fonts\ArialSmall");

            //pawn texture.
            Pawn_texture = Content.Load<Texture2D>(@"Textures\Pawns\death");


        //   pawns = new Pawn[gridSize * 3];     // need to change the size - pawns array

            
            player.pawns = new Pawn[player.army_size];
            enemy.pawns = new Pawn[player.army_size];
            //manually putting pawns for now.


            /// -------------------------------------------------!!!!!!!!!!!!!!!!!!!!!

            /*
                                                                        player.pawns[0] = new Pawn(Pawn_texture, tileDictionary[0], 0, Pawn.Team.my_team,0,font_small);
                                                                        player.pawns[1] = new Pawn(Pawn_texture, tileDictionary[1], 1, Pawn.Team.my_team,1,font_small);
                                                                        player.pawns[2] = new Pawn(Pawn_texture, tileDictionary[2], 2, Pawn.Team.my_team,2,font_small);
                                                                        player.pawns[3] = new Pawn(Pawn_texture, tileDictionary[3], 3, Pawn.Team.my_team,3,font_small);

                                                                        //manually putting pawns for now.
                                                                        enemy.pawns[0] = new Pawn(Pawn_texture, tileDictionary[51], 0, Pawn.Team.my_team,0,font_small);
                                                                        enemy.pawns[1] = new Pawn(Pawn_texture, tileDictionary[52], 1, Pawn.Team.my_team,1,font_small);
                                                                        enemy.pawns[2] = new Pawn(Pawn_texture, tileDictionary[53], 2, Pawn.Team.my_team,2,font_small);
                                                                        enemy.pawns[3] = new Pawn(Pawn_texture, tileDictionary[54], 3, Pawn.Team.my_team,3,font_small);
                                                                 */       




            connection.update();
            if (i_am_second_player)
            {
                /////////----------!!!!!!!changeTilematrix();
                Pawn[] swap_pawns = new Pawn[player.army_size];
                swap_pawns = player.pawns;
                player.pawns = enemy.pawns;
                enemy.pawns = swap_pawns;
                player.myTurn = false;
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
            /*
            for (int i = 0; i < player.pawns.Length; i++)
                player.pawns[i].Draw(OurGame.spriteBatch);
                
            for (int i = 0; i < enemy.pawns.Length; i++)
                enemy.pawns[i].Draw(OurGame.spriteBatch);
                */
            if (player.myTurn)
            {
                OurGame.spriteBatch.DrawString(font_small, "your turn", new Vector2(Game1.screen_width / 3, Game1.screen_height / 80), Color.White);
            }
            else
                OurGame.spriteBatch.DrawString(font_small, "opponent's turn", new Vector2(Game1.screen_width / 3, Game1.screen_height / 80), Color.White);


        }
        
    }
}