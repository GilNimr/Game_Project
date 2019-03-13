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
    class PlacingSoldiersState : BaseGameState, IPlacingSoldiersState
    {
        private SpriteFont font;
        private Texture2D button_texture;
        private Button save_and_start_game;
        public List<Button> buttons;
        public Board ourBoard;
        BuildingBoardState buildingBoardState;
        private bool hideFlag = true;
        IInputHandler inputHandler;
        ICelAnimationManager celAnimationManager;

        private Rectangle iso_rec;
        private bool resting;

        private Rectangle cartasian_rec
        {
            get
            {
                return new Rectangle(Game1.Isometrix2twoD(iso_rec.Location), new Point(iso_rec.Width / 2, iso_rec.Height));
            }
        }
        

        private String strength= "";
        private bool draggin;
        Point flag_size = new Point(Tile.getTileSize());
        Rectangle mouseRec
        {
            get
            {
                return new Rectangle(inputHandler.MouseHandler.MouseState.Position.X, inputHandler.MouseHandler.MouseState.Position.Y, 1, 1);
            }
        }
        public Player player;
        public Player enemy;
        public static Tile[] teleports; // array of all the teleports tiles.
        private Texture2D teleport_texture;
        private Button save_flag_button;
        private int pawn_index = 0;
        private Tile curtile;
        private Connection connection;
        private bool i_am_second_player;

        public PlacingSoldiersState(Game game) : base(game)
        {
            game.Services.AddService(typeof(IPlacingSoldiersState), this);
          buildingBoardState= (BuildingBoardState)game.Services.GetService(typeof(IBuildingBoardState));
            celAnimationManager = (ICelAnimationManager)game.Services.GetService(typeof(ICelAnimationManager));
            inputHandler = (IInputHandler)game.Services.GetService(typeof(IInputHandler));
            connection = buildingBoardState.connection;
            i_am_second_player = BuildingBoardState.i_am_second_player;
           
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            ourBoard = buildingBoardState.getEmptyBoard();
            buttons = new List<Button>();
            font = Content.Load<SpriteFont>(@"Fonts\KaushanScript");
            button_texture = Content.Load<Texture2D>(@"Textures\Controls\Button");

            save_and_start_game = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(Game1.screen_width - 500, 20),
                Text = "Save and start game",
            };

            save_and_start_game.Click += SaveAndStartGame;
           // Game.Components.Add(save_and_start_game);

            save_flag_button = new Button(Game, Content.Load<Texture2D>(@"Textures\Controls\Button"), font)
            {
                Position = new Vector2(Game1.screen_width - 1000, 20),
                Text = "Save here?",
            };

            save_flag_button.Click += new EventHandler((sender, e) => SaveFlag(sender, e, curtile));

            CelCount celCount = new CelCount(30, 5);
            celAnimationManager.AddAnimation("israel", "sprite sheet israel", celCount, 10);
            celAnimationManager.ResumeAnimation("israel");

            celCount = new CelCount(30, 5);
            celAnimationManager.AddAnimation("jamaica", "sprite sheet jamaica", celCount, 20);
            celAnimationManager.ResumeAnimation("jamaica");

            for (int i=1; i<= 21; i++)
            {
                if(i!=21)
                buttons.Add(new Button(OurGame, button_texture, font) {Position=new Vector2(Game1.screen_width/40,Game1.screen_height/40*i),Text=i.ToString() });
                else
                    buttons.Add(new Button(OurGame, button_texture, font) { Position = new Vector2(Game1.screen_width / 40, Game1.screen_height / 40 * i), Text = "flag" });
            }

            foreach (Button button in buttons)
            {
                Game.Components.Add(button);
                button.Click += CreateFlag;

            }

            teleports = new Tile[2];
            teleport_texture = Content.Load<Texture2D>(@"Textures\Tiles\teleport");

            player = buildingBoardState.player;
            player.myTurn = true;
            enemy = buildingBoardState.enemy;
            
        
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
                iso_rec = new Rectangle(new Point(500), new Point(Tile.getTileSize()));
                
                
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
                if (tile.getCartasianRectangle().Intersects(cartasian_rec))
                {
                    if(cartasian_rec.Center.X>tile.getCartasianRectangle().Center.X &&
                        cartasian_rec.Center.Y > tile.getCartasianRectangle().Center.Y)
                    {
                        if (tile.getId() >= 288 && !tile.getIsHidden())
                        {
                            if(!resting)
                            tile.setColor(Color.Green);

                            if(!draggin && !resting)
                            {
                                resting = true;

                                iso_rec = new Rectangle( Game1.TwoD2isometrix(tile.getCartasianRectangle().Center) - new Point(Tile.getTileSize() / 2), new Point(Tile.getTileSize()));

                                curtile = tile;
                                Game.Components.Add(save_flag_button);

                            }
                        }
                        else if(draggin) tile.setColor(Color.Red);
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
                    buttons.Remove(button);
            }

            if (strength != "flag")
            {
        
                player.pawns[pawn_index] = new Pawn(OurGame, teleport_texture, tile, int.Parse(strength), Pawn.Team.my_team, pawn_index, font);
            }
            else
            {
               
                player.pawns[pawn_index] = new Pawn(OurGame, teleport_texture, tile, 21, Pawn.Team.my_team, pawn_index, font);
            }
            
            pawn_index++;
            hideFlag = true;
            resting = false;
            iso_rec = new Rectangle(-100, -100, 0, 0);
            

        }

        

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            connection.update();
            resting = !draggin;

            if (inputHandler.MouseHandler.IsHoldingLeftButton() && (mouseRec.Intersects(iso_rec) || draggin))
            {
                DragFlag(new Point(mouseRec.X, mouseRec.Y));
                draggin = true;
            }
            else draggin = false;

            PlaceFlag();

            if (draggin)
                Game.Components.Remove(save_flag_button);
            if (pawn_index == 21 && !Game.Components.Contains(save_and_start_game))
                Game.Components.Add(save_and_start_game);
        }

        public override void Draw(GameTime gameTime)
        {

            ourBoard.Draw(OurGame.spriteBatch, Color.White);

            foreach (Button button in buttons)
            {
                button.Draw(gameTime, OurGame.spriteBatch);
            }
                celAnimationManager.Draw(gameTime, "jamaica", OurGame.spriteBatch, iso_rec, SpriteEffects.None);
                OurGame.spriteBatch.DrawString(font, strength, new Vector2(iso_rec.X, iso_rec.Y), Color.Black, 0, new Vector2(0), 0.5f, SpriteEffects.None, 0);
            
                save_and_start_game.Draw(gameTime, OurGame.spriteBatch);

            if(Game.Components.Contains(save_flag_button))
                save_flag_button.Draw(gameTime, OurGame.spriteBatch);

            for (int i = 0; i < player.army_size; i++)
            {
                if (player.pawns[i] != null)
                    player.pawns[i].Draw(OurGame.spriteBatch, gameTime);
            }
            

            //debug view.
            // celAnimationManager.Draw(gameTime, "jamaica", OurGame.spriteBatch, cartasian_rec, SpriteEffects.None);

            base.Draw(gameTime);
        }
    }
}
