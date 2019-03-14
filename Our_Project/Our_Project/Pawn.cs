using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XELibrary;

namespace Our_Project
{
   public class Pawn
    {
        public int id;

        public Tile current_tile;  // the square that now the pawn in it
        private Tile direction;          // if we will move this will get the details of new tile

        public bool the_flag;

        private Texture2D texture;  // pawn texture
        public int strength;
        public bool send_update;
        public bool isMouseClicked; // if mouse clicked on pawn 
        private bool hasMoved;           // if pawn needs to move

        public bool attacked=false;
        public Pawn attacker;
        private bool hasDied = false;

        public MouseState oldState; // mouse input old position 
        public Vector2 position;

        private SpriteFont strength_font;

        private double timer_atk_num_display=0;
        private bool draw_atk_font = false;

        private double timer_has_died = 0;

        Rectangle mouseRec;
        public Team team;
        public enum Team
        {
            my_team, enemy_team
        }
        ICelAnimationManager celAnimationManager;

        public Pawn(Game game,Texture2D _texture, Tile _tile, int _strength, Team _team, int _id, SpriteFont _strength_font)
        {
            celAnimationManager = (ICelAnimationManager)game.Services.GetService(typeof(ICelAnimationManager));

            if (_strength == 21)  
                the_flag = true;

            id = _id;

            current_tile = _tile;

            _tile.setCurrentPawn(this);

            team = _team;

            strength_font = _strength_font;
            
           _tile.occupied = Tile.Occupied.yes_by_me;
          
            strength = _strength;
            texture = _texture;
           
            isMouseClicked = false;
            hasMoved = false;
            position = new Vector2(_tile.getCartasianRectangle().X, _tile.getCartasianRectangle().Y);

            CelCount celCount = new CelCount(30, 5);
            celAnimationManager.AddAnimation("israel", "sprite sheet israel", celCount, 10);
            celAnimationManager.ResumeAnimation("israel");

             celCount = new CelCount(30, 5);
            celAnimationManager.AddAnimation("jamaica", "sprite sheet jamaica", celCount, 20);
            celAnimationManager.ResumeAnimation("jamaica");

            send_update = true; //initialize as true it tells the server to update this pawn for the second plyer.
        }

        public void Update(GameTime gametime)
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
                        (mouseRec.Intersects(current_tile.getCartasianRectangle())))
            {
                if (!isMouseClicked) // if we want to move
                    isMouseClicked = true;
                else                 // if we want cancel moving
                    isMouseClicked = false;
            }

            oldState = newState; // get the current mpuse position as old position

            if (attacked)
            {
                GettingAttacked(gametime);
            }

            if (isMouseClicked && !hasDied && !the_flag)
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
                    if ((current_tile.getLeft() != null) &&
                        (current_tile.getLeft().occupied != Tile.Occupied.yes_by_me) && (mouseRec.Intersects(current_tile.getLeft().getCartasianRectangle())))

                    {
                        moveORattack(current_tile.getLeft(), gametime);
                    }


                    else if ((current_tile.getRight() != null) && 
                        (current_tile.getRight().occupied != Tile.Occupied.yes_by_me) && (mouseRec.Intersects(current_tile.getRight().getCartasianRectangle())))

                    {
                        moveORattack(current_tile.getRight(), gametime);
                    }

                    else if ((current_tile.getUp() != null) &&
                        (current_tile.getUp().occupied != Tile.Occupied.yes_by_me) && (mouseRec.Intersects(current_tile.getUp().getCartasianRectangle())))

                    {
                        moveORattack(current_tile.getUp() , gametime);
                    }

                    else if ((current_tile.getDown() != null) && 
                        (current_tile.getDown().occupied != Tile.Occupied.yes_by_me) && (mouseRec.Intersects(current_tile.getDown().getCartasianRectangle())))

                    {
                        moveORattack(current_tile.getDown() , gametime);
                    }
                    if (hasMoved && !hasDied) // get new oldState
                    {
                        oldState = newState;

                       
                        {
                            // move the pawn
                            current_tile.occupied = Tile.Occupied.no;
                            current_tile.setCurrentPawn(null);
                            current_tile = direction;
                            current_tile.occupied = Tile.Occupied.yes_by_me;
                            current_tile.setCurrentPawn(this);
                            send_update = true;
                        }
                    }
                    else if (hasDied)
                        {

                            current_tile.occupied = Tile.Occupied.no;
                            current_tile.setCurrentPawn(null);
                            isMouseClicked = false;

                        }
                }

            }
            else if (hasDied)
            {

                current_tile.occupied = Tile.Occupied.no;
                current_tile.setCurrentPawn(null);
                isMouseClicked = false;
                //  current_tile = null;

            }
        }

        public void GettingAttacked(GameTime gametime)
        {
            timer_atk_num_display += gametime.ElapsedGameTime.TotalSeconds;
            draw_atk_font = true;
            attacker.draw_atk_font = true;

            if (the_flag)
            {
                PlayingState.lose = true;
            }
            //if we lost the encounter with enemy
            if (attacker.strength > strength)
            {

                hasDied = true;

            }
            else if (attacker.strength < strength)
            {

                attacker.hasDied = true;

            }
            else if (attacker.strength == strength)
            {
                hasDied = true;
                attacker.hasDied = true;


            }
            attacked = false;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //timers
            if(draw_atk_font)
            timer_atk_num_display += gameTime.ElapsedGameTime.TotalSeconds;

            if (timer_has_died > 0 && !hasDied)
                timer_has_died += gameTime.ElapsedGameTime.TotalSeconds;
            if (timer_has_died > 3.0)
                hasDied = true;

            if (!hasDied)
            {

                
                //   spriteBatch.Draw(texture, Rec, Color.White);

                if (team == Team.my_team)
                {
                    Rectangle Rec = new Rectangle(Game1.TwoD2isometrix(current_tile.getCartasianRectangle().Center) - new Point(Tile.getTileSize() / 2), new Point(Tile.getTileSize()));
                    celAnimationManager.Draw(gameTime, "jamaica", spriteBatch, Rec, SpriteEffects.None);

                    if (the_flag)
                        spriteBatch.DrawString(strength_font, "flag", Game1.TwoD2isometrix(current_tile.getCartasianRectangle().Center.X, current_tile.getCartasianRectangle().Center.Y), Color.White);
                    else
                    spriteBatch.DrawString(strength_font, strength.ToString(), Game1.TwoD2isometrix(current_tile.getCartasianRectangle().Center.X, current_tile.getCartasianRectangle().Center.Y), Color.White);

                    //timer
                    if (timer_atk_num_display<2.0 && draw_atk_font)
                    
                        
                        spriteBatch.DrawString(strength_font, strength.ToString(), Game1.TwoD2isometrix((int)position.X,(int)position.Y)+new Vector2(-Game1.screen_width/10,-Game1.screen_height/10 - 20 * (float)timer_atk_num_display), Color.Green, 0, Vector2.Zero, 5f, SpriteEffects.None, 0);
                    
                    else
                    {
                        draw_atk_font = false;
                        timer_atk_num_display = 0;
                    }
                }
                else //pawn of enemy team
                {
                    Rectangle Rec = new Rectangle(Game1.TwoD2isometrix(current_tile.getCartasianRectangle().Center) - new Point(Tile.getTileSize() / 2), new Point(Tile.getTileSize()));
                    celAnimationManager.Draw(gameTime, "israel", spriteBatch, Rec, SpriteEffects.None);

                    if (timer_atk_num_display < 2.0 && draw_atk_font)
                        spriteBatch.DrawString(strength_font, strength.ToString(), Game1.TwoD2isometrix((int)position.X, (int)position.Y) + new Vector2(+Game1.screen_width / 10, -Game1.screen_height / 10 - 20 * (float)timer_atk_num_display), Color.Red, 0, Vector2.Zero, 5f, SpriteEffects.None, 0);
                    else
                    {
                        draw_atk_font = false;
                        timer_atk_num_display = 0;
                    }
                }
            }
            //if you have died.
            else
            {

                if (team == Team.my_team)
                {
                    //   spriteBatch.Draw(texture, new Rectangle(30 * strength, 20, Tile.getTileSize() / 2, Tile.getTileSize() / 2), Color.White);
                    celAnimationManager.Draw(gameTime, "jamaica", spriteBatch, new Rectangle(30 * strength, 20, Tile.getTileSize() / 2, Tile.getTileSize() / 2), SpriteEffects.None);
                    spriteBatch.DrawString(strength_font, strength.ToString(), new Vector2(30 * strength, 20), Color.White);
                }
                else
                {
                    //  spriteBatch.Draw(texture, new Rectangle(320 + 30 * strength, 20, Tile.getTileSize() / 2, Tile.getTileSize() / 2), Color.White);
                    celAnimationManager.Draw(gameTime, "jamaica", spriteBatch, new Rectangle(320 + 30 * strength, 20, Tile.getTileSize() / 2, Tile.getTileSize() / 2), SpriteEffects.None);
                    spriteBatch.DrawString(strength_font, strength.ToString(), new Vector2(320+30 * strength, 20), Color.White);
                }
                

            }

            //drawing adjecant tiles if clicked
            if (isMouseClicked)
            {
                if ((current_tile.getLeft() != null) && (current_tile.getLeft().occupied == Tile.Occupied.no))
                    current_tile.getLeft().setColor(Color.Red);


                if ((current_tile.getRight() != null) && (current_tile.getRight().occupied == Tile.Occupied.no))
                    current_tile.getRight().setColor(Color.Red);

                if ((current_tile.getUp() != null) && (current_tile.getUp().occupied == Tile.Occupied.no))
                    current_tile.getUp().setColor(Color.Red);

                if ((current_tile.getDown() != null) && (current_tile.getDown().occupied == Tile.Occupied.no))
                    current_tile.getDown().setColor(Color.Red);

            }

            if (hasMoved)
            {// draw to white again


                if ((current_tile.getLeft() != null) && (current_tile.getLeft().occupied == Tile.Occupied.no))
                    current_tile.getLeft().setColor(Color.White);

                if ((current_tile.getRight() != null) && (current_tile.getRight().occupied == Tile.Occupied.no))
                    current_tile.getRight().setColor(Color.White);

                if ((current_tile.getUp() != null) && (current_tile.getUp().occupied == Tile.Occupied.no))
                    current_tile.getUp().setColor(Color.White);

                if ((current_tile.getDown() != null) && (current_tile.getDown().occupied == Tile.Occupied.no))
                    current_tile.getDown().setColor(Color.White);



                isMouseClicked = false;
                hasMoved = false;
                direction = null;
            }

            //drawing the world mouse for debug purposes.
            //spriteBatch.Draw(texture,new Rectangle(mouseRec.Location,new Point(10)) , Color.Goldenrod);

            
        }

       private void moveORattack(Tile _direction, GameTime gameTime)
        {
            hasMoved = true;

            direction = _direction;

            if (direction.occupied == Tile.Occupied.yes_by_enemy)
            {
                direction.getCurrentPawn().attacked = true;
                direction.getCurrentPawn().attacker = this;

                
                draw_atk_font = true;
                direction.getCurrentPawn().draw_atk_font = true;

                if (direction.getCurrentPawn().the_flag == true)
                {
                    PlayingState.win = true;
                }
                    //if we lost the encounter with enemy
                else if (direction.getCurrentPawn().strength > strength)

                {
                    timer_has_died = gameTime.ElapsedGameTime.TotalSeconds;
                    //hasDied = true;
                }
                else //if we draw the encounter with enemy
                if (direction.getCurrentPawn().strength == strength)
                {
                    timer_has_died = gameTime.ElapsedGameTime.TotalSeconds;
                   // hasDied = true;
                   // direction.getCurrentPawn().hasDied = true;
                    direction.getCurrentPawn().timer_has_died = gameTime.ElapsedGameTime.TotalSeconds;
                }
                else //if we won the encounter with enemy
                {
                    // direction.getCurrentPawn().hasDied = true;
                    direction.getCurrentPawn().timer_has_died = gameTime.ElapsedGameTime.TotalSeconds;
                }

            }

            //checking to see if encounterd a teleport.
            if (direction.teleport_tile)
                direction = direction.Teleport_to_rand();

            send_update = true;
        }


    }
}