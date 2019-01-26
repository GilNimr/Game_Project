using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Our_Project
{
    public sealed class StartMenuState : BaseGameState, IStartMenuState
    {
        private Texture2D texture;
        
        private SpriteFont font;
        private Texture2D button_texture;
        private int selected;

        public Button Host_Button;
        private string[] entries = 
        {
            "Host",
            "join",
            "Options",
            "Exit Game"
        };

        public StartMenuState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IStartMenuState), this);

        }

        public override void Update(GameTime gameTime)
        {
            var mouseRectangle = new Rectangle(Input.MouseHandler.MouseState.X, Input.MouseHandler.MouseState.Y, 1, 1);

            if (Input.KeyboardHandler.WasKeyPressed(Keys.Escape))
            {
                // Go back to title screen
                StateManager.ChangeState(OurGame.TitleIntroState.Value);
            }


            //Host_Button.Update(gameTime);
           

            base.Update(gameTime);
        }

        private void HostButtonClick(object sender, System.EventArgs e)
        {
            
              
                        StateManager.ChangeState(OurGame.PlayingState.Value);
              
            
        }

        protected override void LoadContent()
        {
            texture = Content.Load<Texture2D>(@"Textures\startMenu");
            font = Content.Load<SpriteFont>(@"Fonts\ArialSmall");
            button_texture = Content.Load<Texture2D>(@"Textures\Controls\Button");

            Host_Button = new Button(Game, button_texture , font)
            {
                Position = new Vector2(Game1.screen_width/2 -  button_texture.Width/2, Game1.screen_height / 2 - button_texture.Height/2),
                Text = "play",
            };
            Host_Button.Click += HostButtonClick;
            Game.Components.Add(Host_Button);
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

                Host_Button.Draw(gameTime,OurGame.spriteBatch);
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
