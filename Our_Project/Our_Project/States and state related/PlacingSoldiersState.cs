using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XELibrary;

namespace Our_Project.States_and_state_related
{
    public class PlacingSoldiersState : BaseGameState, IPlacingSoldiersState
    {
        private SpriteFont font;
        private Texture2D button_texture;
        private Button save_and_start_game;
        private Button teleport_button;
        public List<Button> buttons;
        public Board ourBoard;
        BuildingBoardState buildingBoardState;
        private bool hideFlag = true;
        IInputHandler inputHandler;
        ICelAnimationManager celAnimationManager;
        private IScrollingBackgroundManager scrollingBackgroundManager;

        private Rectangle iso_rec;
        private bool resting;

        private Rectangle Cartasian_rec
        {
            get
            {
                return new Rectangle(Game1.Isometrix2twoD(iso_rec.Location), new Point(iso_rec.Width / 2, iso_rec.Height));
            }
        }
        

        private String strength= "";
        private bool draggin;
        Point flag_size = new Point(Tile.GetTileSize());
        Rectangle MouseRec
        {
            get
            {
                return new Rectangle(inputHandler.MouseHandler.MouseState.Position.X, inputHandler.MouseHandler.MouseState.Position.Y, 1, 1);
            }
        }
        public Player player;
        public Player enemy;
        public static Tile[] teleports; // array of all the teleports tiles.
        public Texture2D teleport_texture;
        private Button save_flag_button;
        private int pawn_index = 0;
        private Tile curtile;
        public Connection connection;
        public bool i_am_second_player;
        public string flag_animation;
        public string enemy_flag_animation;
        private int teleport_index = 0 ;

        public PlacingSoldiersState(Game game) : base(game)
        {
            game.Services.AddService(typeof(IPlacingSoldiersState), this);
          buildingBoardState= (BuildingBoardState)game.Services.GetService(typeof(IBuildingBoardState));
            celAnimationManager = (ICelAnimationManager)game.Services.GetService(typeof(ICelAnimationManager));
            scrollingBackgroundManager= (IScrollingBackgroundManager)game.Services.GetService(typeof(IScrollingBackgroundManager));
            inputHandler = (IInputHandler)game.Services.GetService(typeof(IInputHandler));
            player = buildingBoardState.player;
            player.myTurn = true;
            enemy = buildingBoardState.enemy;
            connection = buildingBoardState.connection;
           
           
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            i_am_second_player = BuildingBoardState.i_am_second_player;

            if (!i_am_second_player)
            {
                flag_animation = "canada";
                player.flag = "canada";
                enemy_flag_animation = "israel";
                enemy.flag = "israel";
            }
            else
            {
                flag_animation = "israel";
                player.flag = "israel";
                enemy_flag_animation = "canada";
                enemy.flag = "canada";
            }

            ourBoard = buildingBoardState.GetEmptyBoard();
            buttons = new List<Button>();
            font = Content.Load<SpriteFont>(@"Fonts\KaushanScript");
            button_texture = Content.Load<Texture2D>(@"Textures\Controls\Button");

            save_and_start_game = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2((int)(Game1.screen_width / 1.2), (int)(Game1.screen_height / 50)),
                Text = "Next",
            };

            save_and_start_game.Click += SaveAndStartGame;
           // Game.Components.Add(save_and_start_game);

            save_flag_button = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(save_and_start_game.Rectangle.X - save_and_start_game.Rectangle.Width, (int)(Game1.screen_height / 50)),
                Text = "Save here?",
            };

            save_flag_button.Click += new EventHandler((sender, e) => SaveFlag(sender, e, curtile));

            CelCount celCount = new CelCount(5, 25);
            celAnimationManager.AddAnimation("canada", "canada test", celCount, 10);
            celAnimationManager.ResumeAnimation("canada");

            celCount = new CelCount(30, 5);
            celAnimationManager.AddAnimation("israel", "sprite sheet israel", celCount, 10);
            celAnimationManager.ResumeAnimation("israel");

            celCount = new CelCount(30, 5);
            celAnimationManager.AddAnimation("jamaica", "sprite sheet jamaica", celCount, 20);
            celAnimationManager.ResumeAnimation("jamaica");

            int xPositionOfShapeButton = Game1.screen_width / 50;
            int yPositionOfShapeButton = (int)(Game1.screen_height / 50);
            int heightOfButton = save_and_start_game.Rectangle.Height;

            teleport_button = new Button(OurGame, button_texture, font)
            {
                Position = new Vector2(xPositionOfShapeButton + save_flag_button.Rectangle.Width, heightOfButton)
                    ,
                Text = ("teleport"),
            };

           

            for (int i=0; i<= 20; i++)
            {
               
                if(i!=20)
                buttons.Add(new Button(OurGame, button_texture, font) {Position=new Vector2(xPositionOfShapeButton, /*yPositionOfShapeButton +*/ i*heightOfButton)
                    ,Text=(i+1).ToString() });
                else
                    buttons.Add(new Button(OurGame, button_texture, font) { Position = new Vector2(xPositionOfShapeButton, /*yPositionOfShapeButton +*/ i*heightOfButton),
                        Text = "flag" });
            }
            buttons.Add(teleport_button);
            foreach (Button button in buttons)
            {
                Game.Components.Add(button);
                button.Click += CreateFlag;

            }

            teleports = new Tile[4];
            teleport_texture = Content.Load<Texture2D>(@"Textures\Tiles\teleport");

           
            
        
        }

        private void CreateFlag(object sender, System.EventArgs e)
        {
            foreach(Button button in buttons)
            {
                if (button.Clicked)
                {
                    strength = button.Text;
                    
                }

            }
           
            if (hideFlag)
            {
                iso_rec = new Rectangle(new Point(500), new Point(Tile.GetTileSize()));
                
                
                hideFlag = false;
            }

            else
            {
                iso_rec = new Rectangle(-100,-100,0,0);
                hideFlag = true;
            }


        }

        private void DragFlag(Point difference)
        {
            iso_rec.X = difference.X - iso_rec.Width / 2;
            iso_rec.Y = difference.Y - iso_rec.Height / 2;
            
        }
        private void PlaceFlag()
        {
            
            foreach (Tile tile in ourBoard.boardDictionaryById.Values)
            {
                if (tile.GetCartasianRectangle().Intersects(Cartasian_rec))
                {
                    if(Cartasian_rec.Center.X>tile.GetCartasianRectangle().Center.X &&
                        Cartasian_rec.Center.Y > tile.GetCartasianRectangle().Center.Y)
                    {
                        if (((!i_am_second_player&& tile.GetId() >= 288 ) || (i_am_second_player && tile.GetId() < 288 )) && !tile.GetIsHidden())
                        {
                            if(!resting)
                            tile.SetColor(Color.Green);

                            if(!draggin && !resting)
                            {
                                resting = true;

                                iso_rec = new Rectangle( Game1.TwoD2isometrix(tile.GetCartasianRectangle().Center) - new Point(Tile.GetTileSize() / 2), new Point(Tile.GetTileSize()));

                                curtile = tile;
                                Game.Components.Add(save_flag_button);

                            }
                        }
                        else if(draggin) tile.SetColor(Color.Red);
                    }
                }
            }
            
        }

        private void SaveAndStartGame(object sender, EventArgs e)
        {
            StateManager.ChangeState(OurGame.PlayingState.Value);
        }

        private void SaveFlag(object sender, EventArgs e, Tile tile)
        {
            foreach (Button button in buttons.ToArray())
            {
                if (button.Text == strength)
                    if (button.Text != "teleport")
                    {
                        buttons.Remove(button);
                        Game.Components.Remove(button);
                    }
                       
            }
            
            if (strength == "flag")
            {
                player.pawns[20] = new Pawn(OurGame, flag_animation, tile, 21, Pawn.Team.my_team, 20, font);
                pawn_index++;


            }
            else if (strength == "teleport")
            {
                if (teleport_index < 2)
                {
                    tile.texture = teleport_texture;
                    tile.SetIsHidden(false);
                    tile.teleport_tile = true;
                    tile.sendUpdate = true;
                    for (int i = 0; i < PlayingState.teleports.Length; i++)
                    {
                        if (PlayingState.teleports[i] == null)
                        {
                            PlayingState.teleports[i] = tile;
                            break;
                        }
                            
                    }

                    teleport_index++;
                }
            }
            //if regular pawn.
            else
            {
                    player.pawns[int.Parse(strength) - 1] = new Pawn(OurGame, flag_animation, tile, int.Parse(strength), Pawn.Team.my_team, int.Parse(strength) - 1, font);
                    pawn_index++;

            }



            hideFlag = true;
            resting = false;
            iso_rec = new Rectangle(-100, -100, 0, 0);

            if (teleport_index >= 2)
            {
                buttons.Remove(teleport_button);
                Game.Components.Remove(teleport_button);
            }
        }

        

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            connection.Update();
            resting = !draggin;

            if (inputHandler.MouseHandler.IsHoldingLeftButton() && (MouseRec.Intersects(iso_rec) || draggin))
            {
                DragFlag(new Point(MouseRec.X, MouseRec.Y));
                draggin = true;
            }
            else draggin = false;

            PlaceFlag();

            if (draggin)
                Game.Components.Remove(save_flag_button);
            if (pawn_index == 3 && teleport_index==2 && !Game.Components.Contains(save_and_start_game))
                Game.Components.Add(save_and_start_game);
        }

        public override void Draw(GameTime gameTime)
        {
            float scaleOfFone = Pawn.scaleOfFont;

            //drawing space bg
            scrollingBackgroundManager.Draw("space", OurGame.spriteBatch);
            scrollingBackgroundManager.Draw("space2", OurGame.spriteBatch);
            scrollingBackgroundManager.Draw("space3", OurGame.spriteBatch);

            ourBoard.Draw(OurGame.spriteBatch, Color.White);

            foreach (Button button in buttons)
            {
                button.Draw(gameTime, OurGame.spriteBatch);
            }
            if (strength != "teleport")
                celAnimationManager.Draw(gameTime, flag_animation, OurGame.spriteBatch, iso_rec, SpriteEffects.None);
            else
                OurGame.spriteBatch.Draw(teleport_texture, iso_rec, Color.White);

            //debug view of flag
           // celAnimationManager.Draw(gameTime, "canada", OurGame.spriteBatch, new Rectangle(200,200,500,500), SpriteEffects.None); 

            OurGame.spriteBatch.DrawString(font, strength, new Vector2(iso_rec.X, iso_rec.Y), Color.Black, 0, new Vector2(0), scaleOfFone, SpriteEffects.None, 0);
            
                save_and_start_game.Draw(gameTime, OurGame.spriteBatch);

            if(Game.Components.Contains(save_flag_button))
                save_flag_button.Draw(gameTime, OurGame.spriteBatch);

            for (int i = 0; i < player.army_size; i++)
            {
                if (player.pawns[i] != null)
                    player.pawns[i].Draw(OurGame.spriteBatch, gameTime);
            }
            for (int i = 0; i < enemy.army_size; i++)
            {
                if (enemy.pawns[i] != null)
                    enemy.pawns[i].Draw(OurGame.spriteBatch, gameTime);
            }


            //debug view.
            // celAnimationManager.Draw(gameTime, "jamaica", OurGame.spriteBatch, cartasian_rec, SpriteEffects.None);

            base.Draw(gameTime);
        }

    }
}
