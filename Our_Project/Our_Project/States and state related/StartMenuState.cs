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
        Player player;
        Player enemy;
        public Button local_Button;
        public Button remote_Button;

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
            //local_Button.Update(gameTime);

            base.Update(gameTime);
        }

        private void LocalButtonClick(object sender, System.EventArgs e)
        {
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

        protected override void LoadContent()
        {
            texture = Content.Load<Texture2D>(@"Textures\startMenu");
            font30 = OurGame.font30;
            button_texture = OurGame.button_texture;

            local_Button = new Button(Game, button_texture , font30)
            {
                Position = new Vector2(Game1.screen_width/2  -  button_texture.Width, Game1.screen_height / 2 - button_texture.Height/2),
                Text = "play on local server",
            };
            local_Button.Click += LocalButtonClick;
            Game.Components.Add(local_Button);

            remote_Button = new Button(Game, button_texture, font30)
            {
                Position = new Vector2(local_Button.Position.X, local_Button.Position.Y-local_Button.Rectangle.Height),
                Text = "play on remote server",
            };
            remote_Button.Click += RemoteButtonClick;
            Game.Components.Add(remote_Button);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 pos = new Vector2(Game.GraphicsDevice.Viewport.Width / 2,
                          Game.GraphicsDevice.Viewport.Height / 2);
            Vector2 origin = new Vector2(texture.Width / 2,
                                         texture.Height / 2);
            Vector2 currPos = new Vector2(100, pos.Y / 2);
            
            OurGame.spriteBatch.Draw(texture, pos, new Rectangle(0, 0, Game1.screen_width, Game1.screen_height), Color.White, 0.0f, origin, new Vector2(5.0f, 5.0f), SpriteEffects.None, 0.0f);
            {
                local_Button.Draw(gameTime,OurGame.spriteBatch);
                remote_Button.Draw(gameTime, OurGame.spriteBatch);
            }

            base.Draw(gameTime);
        }

        protected override void StateChanged(object sender, EventArgs e)
        {
            base.StateChanged(sender, e);

            // Change to visible if not at the top of the stack
            // This way, sub menus will appear on top of this menu
            if (StateManager.State != this.Value)
                Visible = true;
        }
    }
}
