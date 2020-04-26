
/* Gil Nevo 310021654
 * Shachar Bartal 305262016
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using XELibrary;

namespace MonoGame.Shared1
{
    public class Button : GameComponent
    {
        private SpriteFont _font;

        private bool _isHovering;

        private readonly Texture2D _texture;
        public Texture2D picture;
        protected IInputHandler Input;
        private double click_timer;

        public event EventHandler Click;

        public bool Clicked { get; private set; }

        public Color PenColour { get; set; }

        public Vector2 Position { get; set; }

        public Rectangle Rectangle
        {
            get
            {
                //  return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
                return new Rectangle((int)Position.X, (int)Position.Y, Game1.screen_width / 7, Game1.screen_height / 22);

            }
            // get;  set;
        }


        public string Text { get; set; }



        #region Methods

        public Button(Game game, Texture2D texture, SpriteFont font) : base(game)
        {
            Input = (IInputHandler)game.Services.GetService(typeof(
                IInputHandler));

            _texture = texture;

            _font = font;

            PenColour = Color.Black;
        }



        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var colour = Color.White;

            if (_isHovering)
                colour = Color.Gray;

            spriteBatch.Draw(_texture, Rectangle, colour);


            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

                //spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour);

                spriteBatch.DrawString(_font, Text, new Vector2(x, y), Color.Black, 0, new Vector2(0), 0.8f,
                    SpriteEffects.None, 0);

            }

            if (picture != null)
            {
                spriteBatch.Draw(picture, new Rectangle(Rectangle.X, Rectangle.Y, Rectangle.Width, Rectangle.Height), PenColour);
            }
        }

        public override void Update(GameTime gameTime)
        {
            var touchRectangle = new Rectangle();

            TouchCollection touchCollection = TouchPanel.GetState();
            if (touchCollection.Count > 0)
            {
                //Only Fire Select Once it's been released
                if (touchCollection[0].State == TouchLocationState.Moved || touchCollection[0].State == TouchLocationState.Pressed)
                {
                    touchRectangle = new Rectangle((int)touchCollection[0].Position.X,(int) touchCollection[0].Position.Y, 1, 1);
                }
            }
            if (touchRectangle.Intersects(Rectangle))
            {
                Clicked = true;
                Click?.Invoke(this, new EventArgs());
            }
            else
            {
                Clicked = false;
            }

            var mouseRectangle = new Rectangle(Input.MouseHandler.MouseState.X, Input.MouseHandler.MouseState.Y, 1, 1);

            _isHovering = false;

            if (mouseRectangle.Intersects(Rectangle))
            {
                _isHovering = true;

                if (Input.MouseHandler.WasLeftButtonClicked())
                {
                    Clicked = true;
                    Click?.Invoke(this, new EventArgs());
                }
                else
                {
                    Clicked = false;
                }
            }

            if (Clicked)
            {
                click_timer += gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (click_timer > 1.0)
            {
                Clicked = false;
                click_timer = 0;
            }

        }

        #endregion
    }
}
