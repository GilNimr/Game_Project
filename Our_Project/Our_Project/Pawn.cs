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
        public bool isMouseClicked;
        public MouseState oldState; // mouse input old position 
        public Vector2 position;
        private bool isMove;
        private Tile move;

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
            isMove = false;
        }

        public void Update()
        {
            MouseState mouseState = Mouse.GetState(); // previous mouse position
            MouseState newState = Mouse.GetState();     // current mouse position  
            Rectangle mouseRec = new Rectangle(mouseState.X, mouseState.Y, 1, 1);
            int mouseX = mouseState.X;
            int mouseY = mouseState.Y;      

            if ( (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released) &&
                        (mouseRec.Intersects(current_tile.Rec)))
                                                                   
            {
                if (!isMouseClicked)
                    isMouseClicked = true;
                else
                    isMouseClicked = false;
            }

            oldState = newState; // get the current mpuse position as old position

            if (isMouseClicked)
            {
                //MouseState ms = Mouse.GetState();
                newState = Mouse.GetState();
                mouseRec.X = mouseState.X;
                mouseRec.Y = mouseState.Y;

                if (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Pressed)
                {

                    if   ((current_tile.left != null) && (mouseRec.Intersects(current_tile.left.Rec))) 
                    {
                         isMove = true;
                         move = current_tile.left;
                    }
              
                    else if ((current_tile.right != null) &&(mouseRec.Intersects(current_tile.right.Rec)))
                    {
                        isMove = true;
                        move = current_tile.right;
                    }
                    else if ((current_tile.up != null) && (mouseRec.Intersects(current_tile.up.Rec)))
                    {
                        isMove = true;
                        move = current_tile.up;
                    }
                    else if ((current_tile.down != null)&&(mouseRec.Intersects(current_tile.down.Rec)))
                    {
                        isMove = true;
                        move = current_tile.down;
                    }
                    if (isMove)
                    {
                        oldState = newState;
                    }
                }
                
            }
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

            if (isMove) 
            {// draw to white again


                if (current_tile.left != null)
                    current_tile.left.Draw(spriteBatch, Color.White);
                if (current_tile.right != null)
                    current_tile.right.Draw(spriteBatch, Color.White);
                if (current_tile.up != null)
                    current_tile.up.Draw(spriteBatch, Color.White);
                if (current_tile.down != null)
                    current_tile.down.Draw(spriteBatch, Color.White);

                // move the pawn
                position.X = move.Rec.X;
                position.Y = move.Rec.Y + current_tile.Rec.Height;
                current_tile = move;
                
                Rectangle newRec = new Rectangle(current_tile.Rec.Location, new Point(40));
                spriteBatch.Draw(texture, newRec, Color.White);
                
                isMouseClicked = false;
                isMove = false;
                move = null;
            }
        }
    }
}