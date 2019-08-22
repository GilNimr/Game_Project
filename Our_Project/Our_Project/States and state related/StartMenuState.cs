
/* Gil Nevo 310021654
 * Shachar Bartal 305262016
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XELibrary;

namespace Our_Project
{
    public sealed class StartMenuState : BaseGameState, IStartMenuState
    {
        private Texture2D texture;
        private SpriteFont font30;
        private Texture2D button_texture;
        ISoundManager soundOfClick;

        public Connection connection;
        public Player player;
        public Player enemy;
        //button for decided if we goes to levelEditor or starting game by choosing kind of connection
        public Button local_Button;
        public Button remote_Button;

        private int reRunCounter = 0;

        public Button board_editor_button;


        public StartMenuState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IStartMenuState), this);
            soundOfClick = (ISoundManager)game.Services.GetService(typeof(ISoundManager));

        }


        public override void Update(GameTime gameTime)
        {
            var mouseRectangle = new Rectangle(Input.MouseHandler.MouseState.X, Input.MouseHandler.MouseState.Y, 1, 1);

            if (Input.KeyboardHandler.WasKeyPressed(Keys.Escape))
            {
                // Go back to title screen
                StateManager.ChangeState(OurGame.TitleIntroState.Value);
            }


            base.Update(gameTime);
        }

        private void LocalButtonClick(object sender, System.EventArgs e)
        {// local connection 
            Connection.local = true;

            connection = new Connection(OurGame, ref player, ref enemy);
            soundOfClick.Play("click");
            Game.Components.Remove(local_Button);
            Game.Components.Remove(remote_Button);
            StateManager.ChangeState(OurGame.BuildingBoardState.Value);

        }
        private void RemoteButtonClick(object sender, System.EventArgs e)
        {
            Connection.local = false;
            connection = new Connection(OurGame, ref player, ref enemy);
            soundOfClick.Play("click");
            Game.Components.Remove(remote_Button);
            Game.Components.Remove(local_Button);
            StateManager.ChangeState(OurGame.BuildingBoardState.Value);

        }

        private void BoardEditorButtonClick(object sender, System.EventArgs e)
        {
            soundOfClick.Play("click");
            StateManager.ChangeState(OurGame.BoardEditorState.Value);
        }

        protected override void LoadContent()
        {
            player = new Player(OurGame)
            {
                myTurn = true
            };

            enemy = new Player(OurGame);
            player.pawns = new Pawn[player.army_size];
            enemy.pawns = new Pawn[player.army_size];
            texture = Content.Load<Texture2D>(@"Textures\bg1 (2)");
            font30 = OurGame.font30;

            //buttons:
            button_texture = OurGame.button_texture;

            local_Button = new Button(Game, button_texture, font30)
            {
                Position = new Vector2(Game1.screen_width / 2 - button_texture.Width, Game1.screen_height / 2 - button_texture.Height / 2),
                Text = "play on local server",
            };
            local_Button.Click += LocalButtonClick;
            Game.Components.Add(local_Button);

            remote_Button = new Button(Game, button_texture, font30)
            {
                Position = new Vector2(local_Button.Position.X, local_Button.Position.Y - local_Button.Rectangle.Height),
                Text = "play on remote server",
            };
            remote_Button.Click += RemoteButtonClick;
            Game.Components.Add(remote_Button);

            board_editor_button = new Button(Game, button_texture, font30)
            {
                Position = new Vector2(remote_Button.Position.X, remote_Button.Position.Y - remote_Button.Rectangle.Height),
                Text = "Go to Level-Editor",
            };
            board_editor_button.Click += BoardEditorButtonClick;
            Game.Components.Add(board_editor_button);

        }

        public override void Draw(GameTime gameTime)
        {
            /* Vector2 pos = new Vector2(Game.GraphicsDevice.Viewport.Width / 2,
                           Game.GraphicsDevice.Viewport.Height / 2);
             Vector2 origin = new Vector2(texture.Width / 2,
                                          texture.Height / 2);
             Vector2 currPos = new Vector2(100, pos.Y / 2);*/

            OurGame.spriteBatch.Draw(texture, new Rectangle(0, 0, Game1.screen_width, Game1.screen_height), Color.White);
            {
                local_Button.Draw(gameTime, OurGame.spriteBatch);
                remote_Button.Draw(gameTime, OurGame.spriteBatch);
                board_editor_button.Draw(gameTime, OurGame.spriteBatch);
            }

            base.Draw(gameTime);
        }

        protected override void StateChanged(object sender, EventArgs e)
        {
            base.StateChanged(sender, e);

            if (StateManager.State == this.Value)
            {
                /* if (local_Button != null && !Game.Components.Contains(local_Button))
                 {
                     Game.Components.Add(local_Button);
                     Game.Components.Add(remote_Button);
                 }*/
                reRunCounter++;
                if (reRunCounter > 1)
                {
                    LoadContent();
                }

                // Change to visible if not at the top of the stack
                // This way, sub menus will appear on top of this menu
                if (StateManager.State != this.Value)
                    Visible = true;
            }
        }
    }
}
