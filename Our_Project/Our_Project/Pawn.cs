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

        Rectangle mouseRec;

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
        }

        public void Update()
        {
            MouseState mouseState = Mouse.GetState(); // previous mouse position
            MouseState newState = Mouse.GetState();     // current mouse position  

            //the location of the world mouse.
            Vector2 CartasianMouseLocation = Game1.Isometrix2twoD(mouseState.X, mouseState.Y);

            //rectangle of the world mouse.
             mouseRec = new Rectangle((int)CartasianMouseLocation.X, (int)CartasianMouseLocation.Y, 1, 1);

            //rectangle of the screen mouse.
           // mouseRec = new Rectangle(mouseState.X, mouseState.Y, 1, 1);


            if ( (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released) &&
                        mouseRec.Intersects(current_tile.Rec))

            {
                if (!isMouseClicked)
                    isMouseClicked = true;
                else
                    isMouseClicked = false;
            }

            oldState = newState; // get the current mpuse position as old position

            if (isMouseClicked)
            {
                
                newState = Mouse.GetState();


                CartasianMouseLocation = Game1.Isometrix2twoD(mouseState.X, mouseState.Y);
                mouseRec.X =(int) CartasianMouseLocation.X;
                mouseRec.Y =(int) CartasianMouseLocation.Y;

            
                if (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Pressed)
                {

                    if   (current_tile.left != null && mouseRec.Intersects(current_tile.left.Rec)
                          ) 
                    {
                         isMove = true;
                         move = current_tile.left;
                    }
              
                    else if (current_tile.right != null &&mouseRec.Intersects(current_tile.right.Rec)
                          )
                    {
                        isMove = true;
                        move = current_tile.right;
                    }
                    else if ((current_tile.up != null) && mouseRec.Intersects(current_tile.up.Rec)
                          )
                    {
                        isMove = true;
                        move = current_tile.up;
                    }
                    else if ((current_tile.down != null)&&mouseRec.Intersects(current_tile.down.Rec)
                          )
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
            //drawing the pawn in the middle of the tile screen location.
            Rectangle Rec = new Rectangle(Game1.TwoD2isometrix( current_tile.Rec.Center)-new Point(current_tile.tilesize/4), new Point(current_tile.tilesize / 2));
            spriteBatch.Draw(texture, Rec, Color.White);

            //drawing adjecant tiles if clicked
            if (isMouseClicked)
            {
                if (current_tile.left != null)
                    current_tile.left.Draw(spriteBatch, Color.Red);
                if (current_tile.right != null)
                    current_tile.right.Draw(spriteBatch, Color.Blue);
                if (current_tile.up != null)
                    current_tile.up.Draw(spriteBatch, Color.Green);
                if (current_tile.down != null)
                    current_tile.down.Draw(spriteBatch, Color.Purple);
                
                
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
             
                current_tile = move;
                
              

                
                
                isMouseClicked = false;
                isMove = false;
                move = null;
            }

            //drawing the world mouse for debug purposes.
            //spriteBatch.Draw(texture,new Rectangle(mouseRec.Location,new Point(10)) , Color.Goldenrod);
        }
    }
}