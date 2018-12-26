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
        public Tile current_tile;  // the square that now the pawn in it
        private Tile move;          // if we will move this will get the details of new tile
        private Texture2D texture;  // pawn texture
        public bool isMouseClicked; // if mouse clicked on pawn 
        private bool isMove;           // if pawn need to move
        public MouseState oldState; // mouse input old position 
        public Vector2 position;

        public enum team
        {
            my_team, enemy_team
        }

        public Pawn(Texture2D _texture, Tile _tile)
        {
            texture = _texture;
            current_tile = _tile;
            isMouseClicked = false;
            isMove = false;
            position = new Vector2(_tile.Rec.X, _tile.Rec.Y);
        }

        public void Update()
        {
            MouseState mouseState = Mouse.GetState(); // previous mouse position
            MouseState newState = Mouse.GetState();     // current mouse position  
            Rectangle mouseRec = new Rectangle(mouseState.X, mouseState.Y, 1, 1); //rectangle of mouse
                                                                                  // position of mouse:
            int mouseX = mouseState.X;
            int mouseY = mouseState.Y;

            // if previous left button of mouse was unclicked, and now clicked on current pawn:
            if ((newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released) &&
                        (mouseRec.Intersects(current_tile.Rec)))
            {
                

                if (!isMouseClicked) // if we want to move
                    isMouseClicked = true;
                else                 // if we want cancel moving
                    isMouseClicked = false;
            }

            oldState = newState; // get the current mpuse position as old position

            if (isMouseClicked)
            {
                // if we clicked, we will get the newe details of mouse position
                newState = Mouse.GetState();
                mouseRec.X = mouseState.X;
                mouseRec.Y = mouseState.Y;

                // if there is another click, that means we want to move
                if (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Pressed)
                {
                    // checking the move direction:
                    if ((current_tile.left != null) &&
                        (current_tile.left.occupied == Tile.Occupied.no) && (mouseRec.Intersects(current_tile.left.Rec)))
                    {
                        isMove = true;
                        move = current_tile.left;
                    }

                    else if ((current_tile.right != null) && 
                        (current_tile.right.occupied == Tile.Occupied.no) && (mouseRec.Intersects(current_tile.right.Rec)))
                    {
                        isMove = true;
                        move = current_tile.right;
                    }
                    else if ((current_tile.up != null) &&
                        (current_tile.up.occupied == Tile.Occupied.no) && (mouseRec.Intersects(current_tile.up.Rec)))
                    {
                        isMove = true;
                        move = current_tile.up;
                    }
                    else if ((current_tile.down != null) && 
                        (current_tile.down.occupied == Tile.Occupied.no) && (mouseRec.Intersects(current_tile.down.Rec)))
                    {
                        isMove = true;
                        move = current_tile.down;
                    }
                    if (isMove) // get new oldState
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
                if ((current_tile.left != null) && (current_tile.left.occupied == Tile.Occupied.no))
                    current_tile.left.Draw(spriteBatch, Color.Red);

                if ((current_tile.right != null) && (current_tile.right.occupied == Tile.Occupied.no))
                    current_tile.right.Draw(spriteBatch, Color.Red);

                if ((current_tile.up != null) && (current_tile.up.occupied == Tile.Occupied.no))
                    current_tile.up.Draw(spriteBatch, Color.Red);

                if ((current_tile.down != null) && (current_tile.down.occupied == Tile.Occupied.no))
                    current_tile.down.Draw(spriteBatch, Color.Red);


            }

            if (isMove)
            {// draw to white again


                if ((current_tile.left != null) && (current_tile.left.occupied == Tile.Occupied.no))
                    current_tile.left.Draw(spriteBatch, Color.White);

                if ((current_tile.right != null) && (current_tile.right.occupied == Tile.Occupied.no))
                    current_tile.right.Draw(spriteBatch, Color.White);

                if ((current_tile.up != null) && (current_tile.up.occupied == Tile.Occupied.no))
                    current_tile.up.Draw(spriteBatch, Color.White);

                if ((current_tile.down != null) && (current_tile.down.occupied == Tile.Occupied.no))
                    current_tile.down.Draw(spriteBatch, Color.White);

                // move the pawn
                position.X = move.Rec.X;
                position.Y = move.Rec.Y + current_tile.Rec.Height;
                current_tile.occupied = Tile.Occupied.no;
                current_tile = move;
                current_tile.occupied = Tile.Occupied.yes_by_me;

                Rectangle newRec = new Rectangle(current_tile.Rec.Location, new Point(40));
                spriteBatch.Draw(texture, newRec, Color.White);

                isMouseClicked = false;
                isMove = false;
                move = null;
            }
        }
    }
}