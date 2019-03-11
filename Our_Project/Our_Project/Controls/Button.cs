using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XELibrary;

namespace Our_Project
{
    public class Button : GameComponent
    {
        

       

        private SpriteFont _font;

        private bool _isHovering;

        private MouseState _previousMouse;

        private Texture2D _texture;

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
             //   return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
                return new Rectangle((int)Position.X, (int)Position.Y, Game1.screen_width/10, Game1.screen_height/40);
            }
        }

        public string Text { get; set; }

        

        #region Methods

        public Button(Game game,Texture2D texture, SpriteFont font):base(game)
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
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2 );
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2 );

                spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour);
            //    spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColour,0f,Vector2.Zero,new Vector2(Rectangle.Width/_font.MeasureString(Text).X, Rectangle.Height/_font.MeasureString(Text).Y ), SpriteEffects.None,0);
            }
        }

        public override  void Update(GameTime gameTime)
        {
          

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
            
        }

        #endregion
    }
}
