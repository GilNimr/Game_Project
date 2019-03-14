using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Our_Project.States_and_state_related;
using XELibrary;

namespace Our_Project
{


    public sealed class PlayingState : BaseGameState, IPlayingState
    {
        public static Dictionary<int, Tile> tileDictionary;
        public static Texture2D teleport_texture;
        public static bool win = false;
        public static bool lose = false;
        public static Tile[] teleports; // array of all the teleports tiles.
        public static bool i_am_second_player = false;
        public static int tileSize = Game1.screen_height / 30;

        public Player player;
        public Player enemy;
        public Connection connection;

        private Texture2D Tile_texture;            
        private Texture2D cartasian_texture;       
        private Texture2D Pawn_texture; // the character of user team
        private Board ourBoard;
        private string flag;
        private string enemy_flag;

        SpriteFont font_small;
        PlacingSoldiersState placingSoldiersState;

        IScrollingBackgroundManager scrollingBackgroundManager;
        ICelAnimationManager celAnimationManager;
        
        public PlayingState(Game game)
           : base(game)
        {
  
            game.Services.AddService(typeof(IPlayingState), this);
            placingSoldiersState = (PlacingSoldiersState)game.Services.GetService(typeof(IPlacingSoldiersState));
            scrollingBackgroundManager = (IScrollingBackgroundManager)game.Services.GetService(typeof(IScrollingBackgroundManager));
            celAnimationManager = (ICelAnimationManager)game.Services.GetService(typeof(ICelAnimationManager));
            player = placingSoldiersState.player;
            teleports = new Tile[4];
            enemy = placingSoldiersState.enemy;
            connection = placingSoldiersState.connection;
        }

        protected override void LoadContent()
        {
            //Loading fonts.
            font_small = Content.Load<SpriteFont>(@"Fonts\KaushanScript");
            ourBoard = placingSoldiersState.ourBoard;
            tileDictionary = placingSoldiersState.ourBoard.boardDictionaryById;
            
            //Gray tile texture.
            Tile_texture = Content.Load<Texture2D>(@"Textures\Tiles\grass_tile_iso5");
            cartasian_texture = Content.Load<Texture2D>(@"Textures\Tiles\Gray_Tile");
            teleport_texture = Content.Load<Texture2D>(@"Textures\Tiles\teleport");

            foreach (var tile in ourBoard.boardDictionaryById.Values)
            {
                if (tile.GetIsHidden())
                    tile.texture = null;
                else if (tile.teleport_tile)
                    tile.texture = teleport_texture;
                else
                    tile.texture = Tile_texture;
            }         

            //pawn texture.
            Pawn_texture = Content.Load<Texture2D>(@"Textures\Pawns\death");

            //flag animations
            CelCount celCount = new CelCount(5, 25);
            celAnimationManager.AddAnimation("canada", "canada test", celCount, 20);
            celAnimationManager.ResumeAnimation("canada");

            celCount = new CelCount(30, 5);
            celAnimationManager.AddAnimation("israel", "sprite sheet israel", celCount, 20);
            celAnimationManager.ResumeAnimation("israel");

            celCount = new CelCount(30, 5);
            celAnimationManager.AddAnimation("jamaica", "sprite sheet jamaica", celCount, 20);
            celAnimationManager.ResumeAnimation("jamaica");

            flag = placingSoldiersState.flag_animation;
            enemy_flag = placingSoldiersState.enemy_flag_animation;
            
            i_am_second_player = placingSoldiersState.i_am_second_player;
            if (!i_am_second_player)
                player.myTurn = true;
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            connection.update();
            
            for (int i = 0; i < player.pawns.Length; i++)
            {
                if (player.pawns[i] != null)
                {
                    if (player.myTurn)
                    {
                        player.pawns[i].Update(gameTime);

                        if (player.pawns[i].isMouseClicked) // if this pawn was clicked before
                        {
                            for (int j = 0; j < player.pawns.Length; j++)
                            {
                                if (player.pawns[j] != null)
                                { 
                                if (i != j) // so the other will canceled
                                    player.pawns[j].isMouseClicked = false;
                                }
                            }
                        }
                    }
                    else if (player.pawns[i].attacked)
                    {
                        player.pawns[i].GettingAttacked(gameTime);
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

            ourBoard.Draw(OurGame.spriteBatch, Color.White);

            //drawing our giant flag 
            Rectangle Rec = new Rectangle(Game1.screen_width*8/10,Game1.screen_height*7/10, Game1.screen_width * 1 / 10, Game1.screen_height * 2 / 10);
            celAnimationManager.Draw(gameTime, flag, OurGame. spriteBatch, Rec, SpriteEffects.None);

            //drawing our enemys giant flag 
            Rec = new Rectangle(Game1.screen_width * 2 / 10, Game1.screen_height * 1 / 10, Game1.screen_width * 1 / 10, Game1.screen_height * 2 / 10);
            celAnimationManager.Draw(gameTime, enemy_flag, OurGame.spriteBatch, Rec, SpriteEffects.None);
            
            for (int i = 0; i < player.pawns.Length; i++)
            {
                if(player.pawns[i]!=null)
                player.pawns[i].Draw(OurGame.spriteBatch, gameTime);
            }
            
            for (int i = 0; i < enemy.pawns.Length; i++)
            {
                if (enemy.pawns[i] != null)
                    enemy.pawns[i].Draw(OurGame.spriteBatch, gameTime);
            }
            
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