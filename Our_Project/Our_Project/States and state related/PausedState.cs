
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
    public sealed class PausedState : BaseGameState, IPausedState
    {
        private Texture2D texture;
        
        private Vector2 scale;

        public PausedState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IPausedState), this);
        }

        protected override void LoadContent()
        {
            texture = Content.Load<Texture2D>(@"Textures\pause");
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.KeyboardHandler.WasKeyPressed(Keys.Escape) ||
                Input.KeyboardHandler.WasKeyPressed(Keys.Enter) ||
                Input.KeyboardHandler.WasKeyPressed(Keys.P))
                StateManager.PopState();

            if (scale.X < Vector2.One.X)
            {
                scale.X += 2.0f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                scale.Y += 2.0f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 pos = new Vector2(Game.GraphicsDevice.Viewport.Width / 2,
                                      Game.GraphicsDevice.Viewport.Height / 2);
            Vector2 origin = new Vector2(texture.Width / 2,
                                         texture.Height / 2);
            OurGame.spriteBatch.Draw(texture, pos, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0.0f, origin, scale, SpriteEffects.None, 0.0f);

            base.Draw(gameTime);
        }

        protected override void StateChanged(object sender, EventArgs e)
        {
            base.StateChanged(sender, e);

            if (StateManager.State == this.Value)
                scale = Vector2.Zero;

        }

    }
}
