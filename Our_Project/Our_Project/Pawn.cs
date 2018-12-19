using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our_Project
{
    class Pawn
    {
        public Tile current_tile;
        private Texture2D texture;
        private bool isMouseClicked;
        private MouseState oldState; // mouse input old position
        private Vector2 position;

        public enum team
        {
            my_team, enemy_team
        }

        public Pawn(Texture2D _texture, Tile _tile)
        {
            texture = _texture;
            current_tile = _tile;
            isMouseClicked = false;
            position = new Vector2(_tile.Rec.X, _tile.Rec.Y);
        }

        public void Update()
        {
            MouseState mouseState = Mouse.GetState(); // previous mouse position
            MouseState newState = Mouse.GetState();     // current mouse position

            int mouseX = mouseState.X;
            int mouseY = mouseState.Y;      

            if ( (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released) &&
                    (mouseX >= (position.X) && (mouseX <= (position.X + current_tile.Rec.Width))) &&
                        ((mouseY >= position.Y) && (mouseY <= position.Y+ current_tile.Rec.Height)) )
            {
                if (!isMouseClicked)
                    isMouseClicked = true;
                else
                    isMouseClicked = false;
            }
            oldState = newState; // get the current mpuse position as old position
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle Rec = new Rectangle(current_tile.Rec.Location, new Point(40));
            spriteBatch.Draw(texture, Rec, Color.White);

            //drawing adjecant tiles if clicked
            if (isMouseClicked)
            {
                if (current_tile.left != null)
                    current_tile.left.Draw(spriteBatch, Color.Red);
                if (current_tile.right != null)
                    current_tile.right.Draw(spriteBatch, Color.Red);
                if (current_tile.up != null)
                    current_tile.up.Draw(spriteBatch, Color.Red);
                if (current_tile.down != null)
                    current_tile.down.Draw(spriteBatch, Color.Red);
                
            }
        }
    }
}
