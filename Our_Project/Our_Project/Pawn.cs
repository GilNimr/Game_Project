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
   public class Pawn
    {

        public Tile current_tile;  // the square that now the pawn in it
        private Tile move;          // if we will move this will get the details of new tile
        private Texture2D texture;  // pawn texture
        private int strength;
        public bool send_update;
        public bool isMouseClicked; // if mouse clicked on pawn 
        private bool isMove;           // if pawn needs to move
        private bool hasDied = false;

        public MouseState oldState; // mouse input old position 
        public Vector2 position;

        Rectangle mouseRec;
        public Team team;
        public enum Team
        {
            my_team, enemy_team
        }

        public Pawn(Texture2D _texture, Tile _tile, int _strength, Team _team)
        {
            current_tile = _tile;
            _tile.current_pawn = this;

            team = _team;
            
           _tile.occupied = Tile.Occupied.yes_by_me;
          
            strength = _strength;
            texture = _texture;
           
            isMouseClicked = false;
            isMove = false;
            position = new Vector2(_tile.Rec.X, _tile.Rec.Y);

            send_update = true; //initialize as true it tells the server to update this pawn for the second plyer.
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

            if (isMouseClicked && !hasDied)
            {
                // if we clicked, we will get the newe details of mouse position
                newState = Mouse.GetState();
                CartasianMouseLocation = Game1.Isometrix2twoD(mouseState.X, mouseState.Y);
                mouseRec.X =(int) CartasianMouseLocation.X;
                mouseRec.Y =(int) CartasianMouseLocation.Y;

                // if there is another click, that means we want to move
                if (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Pressed)
                {
                    // checking the move direction:
                    if ((current_tile.left != null) &&
                        (current_tile.left.occupied != Tile.Occupied.yes_by_me) && (mouseRec.Intersects(current_tile.left.Rec)))

                    {
                        moveORattack(current_tile.left);
                    }


                    else if ((current_tile.right != null) && 
                        (current_tile.right.occupied != Tile.Occupied.yes_by_me) && (mouseRec.Intersects(current_tile.right.Rec)))

                    {
                        moveORattack(current_tile.right);
                    }

                    else if ((current_tile.up != null) &&
                        (current_tile.up.occupied != Tile.Occupied.yes_by_me) && (mouseRec.Intersects(current_tile.up.Rec)))

                    {
                        moveORattack(current_tile.up);
                    }

                    else if ((current_tile.down != null) && 
                        (current_tile.down.occupied != Tile.Occupied.yes_by_me) && (mouseRec.Intersects(current_tile.down.Rec)))

                    {
                        moveORattack(current_tile.down);
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
           
            if (!hasDied)
            {
                Rectangle Rec = new Rectangle(Game1.TwoD2isometrix(current_tile.Rec.Center) - new Point(current_tile.tilesize / 4), new Point(current_tile.tilesize / 2));
                spriteBatch.Draw(texture, Rec, Color.White);
            }
            else
            {
                if(team==Team.my_team)
                spriteBatch.Draw(texture, new Rectangle(30*strength,20, current_tile.tilesize / 2, current_tile.tilesize / 2), Color.White);
                else
                spriteBatch.Draw(texture, new Rectangle(320+30 * strength, 20, current_tile.tilesize / 2, current_tile.tilesize / 2), Color.White);
            }

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

                if (hasDied)
                {
                    current_tile.occupied = Tile.Occupied.no;
                    current_tile.current_pawn = null;
                    send_update = true;
                }

                else
                { 
                // move the pawn
                current_tile.occupied = Tile.Occupied.no;
                current_tile.current_pawn = null;
                current_tile = move;
                current_tile.occupied = Tile.Occupied.yes_by_me;
                current_tile.current_pawn = this;
                send_update = true;
                } 
               
                isMouseClicked = false;
                isMove = false;
                move = null;
            }

            //drawing the world mouse for debug purposes.
            //spriteBatch.Draw(texture,new Rectangle(mouseRec.Location,new Point(10)) , Color.Goldenrod);
        }

       public void moveORattack(Tile direction)
        {
            isMove = true;
            move = direction;

            if (direction.occupied == Tile.Occupied.yes_by_enemy)
            {
                //if we lost the encounter with enemy
                if (direction.current_pawn.strength > strength)
                {
                    hasDied = true;
                }
                //if we won an encounter with the enemy
                else if (direction.current_pawn.strength < strength)
                {
                    direction.current_pawn.hasDied = true;
                }
                else
                {
                    hasDied = true;
                    direction.current_pawn.hasDied = true;
                }
            }
        }


    }
}