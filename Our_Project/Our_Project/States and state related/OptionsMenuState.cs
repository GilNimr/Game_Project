
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
    public sealed class OptionsMenuState : BaseGameState, IOptionsMenuState
    {
        private Texture2D texture;

        private SpriteFont font;
        private int selected;
        private string[] entries = 
        {
            "Sound FX",
            "Music",
            "Back"
        };

        private string[] values = 
        {
            "ON",
            "ON",
            ""
        };

        public OptionsMenuState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IOptionsMenuState), this);

            selected = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.KeyboardHandler.WasKeyPressed(Keys.Escape))
                StateManager.PopState();

            if (Input.KeyboardHandler.WasKeyPressed(Keys.Up))
                selected--;
            if (Input.KeyboardHandler.WasKeyPressed(Keys.Down))
                selected++;

            if (selected < 0)
                selected = entries.Length - 1;
            if (selected > entries.Length - 1)
                selected = 0;

            if (Input.KeyboardHandler.WasKeyPressed(Keys.Enter))
            {
                switch (selected)
                {
                    case 0:
                        OurGame.EnableSoundFx = !OurGame.EnableSoundFx;
                        values[0] = OurGame.EnableSoundFx ? "ON" : "OFF";
                        break;
                    case 1:
                        OurGame.EnableMusic = !OurGame.EnableMusic;
                        values[1] = OurGame.EnableMusic ? "ON" : "OFF";
                        break;
                    case 2:
                        StateManager.PopState();
                        break;
                }
            }

            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            texture = Content.Load<Texture2D>(@"Textures\optionsMenu");
            font = Content.Load<SpriteFont>(@"Fonts\ArialSmall");
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 pos = new Vector2(Game.GraphicsDevice.Viewport.Width / 2,
                          Game.GraphicsDevice.Viewport.Height / 2);
            Vector2 origin = new Vector2(texture.Width / 2,
                                         texture.Height / 2);
            Vector2 optionPos = new Vector2(pos.X - texture.Width / 2 + 20, pos.Y / 2 + 50);
            Vector2 valuePos = new Vector2(pos.X + texture.Width / 2 - 400, optionPos.Y);

            OurGame.spriteBatch.Draw(texture, pos, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0.0f, origin, new Vector2(1.0f, 1.0f), SpriteEffects.None, 0.0f);
            for (int i = 0; i < entries.Length; i++)
            {
                Color color;
                float scale = 1;

                if (i == selected)
                    color = Color.White;
                else
                    color = Color.Blue;

                Vector2 fontOrigin = new Vector2(0, font.LineSpacing / 2);
                Vector2 optionShadowPos = new Vector2(optionPos.X - 2, optionPos.Y - 2);
                Vector2 valueShadowPos = new Vector2(valuePos.X - 2, valuePos.Y - 2);

                // Draw Shadow
                OurGame.spriteBatch.DrawString(font, entries[i], optionShadowPos, Color.Black, 0.0f, fontOrigin, scale, SpriteEffects.None, 0);
                OurGame.spriteBatch.DrawString(font, values[i], valueShadowPos, Color.Black, 0.0f, fontOrigin, scale, SpriteEffects.None, 0);

                // Draw Text
                OurGame.spriteBatch.DrawString(font, entries[i], optionPos, color, 0.0f, fontOrigin, scale, SpriteEffects.None, 0);
                OurGame.spriteBatch.DrawString(font, values[i], valueShadowPos, color, 0.0f, fontOrigin, scale, SpriteEffects.None, 0);

                optionPos.Y += font.LineSpacing;
                valuePos.Y += font.LineSpacing;
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
            else
                selected = 0;
        }

    }
}
