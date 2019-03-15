
/* Gil Nevo 310021654
 * Shachar Bartal 305262016
 */

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
        
        public int strength;
        public bool send_update;
        public bool isMouseClicked; // if mouse clicked on pawn 
        private bool hasMoved;           // if pawn needs to move

        public bool attacked=false;
        public Pawn attacker;
        private bool hasDied = false;

        public MouseState oldState; // mouse input old position 
        public Vector2 position;

        private readonly SpriteFont strength_font;

        public String flag_animation;

        private double timer_atk_num_display=0;
        private bool draw_atk_font = false;
        private float scaleOfFont = Game1.screen_height * 0.00055f;
        private double timer_has_died = 0;

        Rectangle mouseRec;
        public Team team;
        public enum Team
        {
            my_team, enemy_team
        }
        ICelAnimationManager celAnimationManager;

        public Pawn(Game game,String _flag_animation, Tile _tile, int _strength, Team _team, int _id, SpriteFont _strength_font)
        {
            celAnimationManager = (ICelAnimationManager)game.Services.GetService(typeof(ICelAnimationManager));

            flag_animation = _flag_animation;

            if (_strength == 21)  
                the_flag = true;

            id = _id;

            current_tile = _tile;

            _tile.SetCurrentPawn(this);

            team = _team;

            strength_font = _strength_font;
            
           _tile.occupied = Tile.Occupied.yes_by_me;
          
            strength = _strength;
           // texture = _texture;
           
            isMouseClicked = false;
            hasMoved = false;
            position = new Vector2(_tile.GetCartasianRectangle().X, _tile.GetCartasianRectangle().Y);

            CelCount celCount = new CelCount(5, 25);
            celAnimationManager.AddAnimation("canada", "canada test", celCount, 10);
            celAnimationManager.ResumeAnimation("canada");

             celCount = new CelCount(30, 5);
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
                        (mouseRec.Intersects(current_tile.GetCartasianRectangle())))
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
                    if ((current_tile.GetLeft() != null) && (!current_tile.GetLeft().GetIsHidden()) &&
                        (current_tile.GetLeft().occupied != Tile.Occupied.yes_by_me) &&  (mouseRec.Intersects(current_tile.GetLeft().GetCartasianRectangle())))

                    {
                        MoveORattack(current_tile.GetLeft(), gametime);
                    }


                    else if ((current_tile.GetRight() != null) && (!current_tile.GetRight().GetIsHidden()) &&
                        (current_tile.GetRight().occupied != Tile.Occupied.yes_by_me) && (mouseRec.Intersects(current_tile.GetRight().GetCartasianRectangle())))

                    {
                        MoveORattack(current_tile.GetRight(), gametime);
                    }

                    else if ((current_tile.GetUp() != null) && (!current_tile.GetUp().GetIsHidden()) &&
                        (current_tile.GetUp().occupied != Tile.Occupied.yes_by_me) && (mouseRec.Intersects(current_tile.GetUp().GetCartasianRectangle())))

                    {
                        MoveORattack(current_tile.GetUp() , gametime);
                    }

                    else if ((current_tile.GetDown() != null) && (!current_tile.GetDown().GetIsHidden()) &&
                        (current_tile.GetDown().occupied != Tile.Occupied.yes_by_me) && (mouseRec.Intersects(current_tile.GetDown().GetCartasianRectangle())))

                    {
                        MoveORattack(current_tile.GetDown() , gametime);
                    }
                    if (hasMoved && !hasDied) // get new oldState
                    {
                        oldState = newState;

                       
                        {
                            // move the pawn
                            current_tile.occupied = Tile.Occupied.no;
                            current_tile.SetCurrentPawn(null);
                            current_tile = direction;
                            current_tile.occupied = Tile.Occupied.yes_by_me;
                            current_tile.SetCurrentPawn(this);
                            send_update = true;
                        }
                    }
                    else if (hasDied)
                        {

                            current_tile.occupied = Tile.Occupied.no;
                            current_tile.SetCurrentPawn(null);
                            isMouseClicked = false;

                        }
                }

            }
            else if (hasDied)
            {

                current_tile.occupied = Tile.Occupied.no;
                current_tile.SetCurrentPawn(null);
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
                    Rectangle Rec = new Rectangle(Game1.TwoD2isometrix(current_tile.GetCartasianRectangle().Center) - new Point(Tile.GetTileSize() / 2), new Point(Tile.GetTileSize()));
                    celAnimationManager.Draw(gameTime, flag_animation, spriteBatch, Rec, SpriteEffects.None);

                    if (the_flag)
                        spriteBatch.DrawString(strength_font, "flag", Game1.TwoD2isometrix(current_tile.GetCartasianRectangle().Center.X, current_tile.GetCartasianRectangle().Center.Y), Color.White);
                    else
                    spriteBatch.DrawString(strength_font, strength.ToString(), Game1.TwoD2isometrix(current_tile.GetCartasianRectangle().Center.X, current_tile.GetCartasianRectangle().Center.Y), Color.White, 0, Vector2.Zero, scaleOfFont, SpriteEffects.None, 0);

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
                    Rectangle Rec = new Rectangle(Game1.TwoD2isometrix(current_tile.GetCartasianRectangle().Center) - new Point(Tile.GetTileSize() / 2), new Point(Tile.GetTileSize()));
                    celAnimationManager.Draw(gameTime, flag_animation, spriteBatch, Rec, SpriteEffects.None);

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
                    celAnimationManager.Draw(gameTime, flag_animation, spriteBatch, new Rectangle(30 * strength, 20, Tile.GetTileSize() / 2, Tile.GetTileSize() / 2), SpriteEffects.None);
                    spriteBatch.DrawString(strength_font, strength.ToString(), new Vector2(30 * strength, 20), Color.White);
                }
                else
                {
                    //  spriteBatch.Draw(texture, new Rectangle(320 + 30 * strength, 20, Tile.getTileSize() / 2, Tile.getTileSize() / 2), Color.White);
                    celAnimationManager.Draw(gameTime, flag_animation, spriteBatch, new Rectangle(320 + 30 * strength, 20, Tile.GetTileSize() / 2, Tile.GetTileSize() / 2), SpriteEffects.None);
                    spriteBatch.DrawString(strength_font, strength.ToString(), new Vector2(320+30 * strength, 20), Color.White);
                }
                

            }

            //drawing adjecant tiles if clicked
            if (isMouseClicked)
            {
                if ((current_tile.GetLeft() != null) && (current_tile.GetLeft().occupied == Tile.Occupied.no))
                    current_tile.GetLeft().SetColor(Color.Red);


                if ((current_tile.GetRight() != null) && (current_tile.GetRight().occupied == Tile.Occupied.no))
                    current_tile.GetRight().SetColor(Color.Red);

                if ((current_tile.GetUp() != null) && (current_tile.GetUp().occupied == Tile.Occupied.no))
                    current_tile.GetUp().SetColor(Color.Red);

                if ((current_tile.GetDown() != null) && (current_tile.GetDown().occupied == Tile.Occupied.no))
                    current_tile.GetDown().SetColor(Color.Red);

            }

            if (hasMoved)
            {// draw to white again


                if ((current_tile.GetLeft() != null) && (current_tile.GetLeft().occupied == Tile.Occupied.no))
                    current_tile.GetLeft().SetColor(Color.White);

                if ((current_tile.GetRight() != null) && (current_tile.GetRight().occupied == Tile.Occupied.no))
                    current_tile.GetRight().SetColor(Color.White);

                if ((current_tile.GetUp() != null) && (current_tile.GetUp().occupied == Tile.Occupied.no))
                    current_tile.GetUp().SetColor(Color.White);

                if ((current_tile.GetDown() != null) && (current_tile.GetDown().occupied == Tile.Occupied.no))
                    current_tile.GetDown().SetColor(Color.White);



                isMouseClicked = false;
                hasMoved = false;
                direction = null;
            }

            //drawing the world mouse for debug purposes.
            //spriteBatch.Draw(texture,new Rectangle(mouseRec.Location,new Point(10)) , Color.Goldenrod);

            
        }

       private void MoveORattack(Tile _direction, GameTime gameTime)
        {
            hasMoved = true;

            direction = _direction;

            if (direction.occupied == Tile.Occupied.yes_by_enemy)
            {
                direction.GetCurrentPawn().attacked = true;
                direction.GetCurrentPawn().attacker = this;

                
                draw_atk_font = true;
                direction.GetCurrentPawn().draw_atk_font = true;

                if (direction.GetCurrentPawn().the_flag == true)
                {
                    PlayingState.win = true;
                }
                    //if we lost the encounter with enemy
                else if (direction.GetCurrentPawn().strength > strength)

                {
                    timer_has_died = gameTime.ElapsedGameTime.TotalSeconds;
                    //hasDied = true;
                }
                else //if we draw the encounter with enemy
                if (direction.GetCurrentPawn().strength == strength)
                {
                    timer_has_died = gameTime.ElapsedGameTime.TotalSeconds;
                   // hasDied = true;
                   // direction.getCurrentPawn().hasDied = true;
                    direction.GetCurrentPawn().timer_has_died = gameTime.ElapsedGameTime.TotalSeconds;
                }
                else //if we won the encounter with enemy
                {
                    // direction.getCurrentPawn().hasDied = true;
                    direction.GetCurrentPawn().timer_has_died = gameTime.ElapsedGameTime.TotalSeconds;
                }

            }

            //checking to see if encounterd a teleport.
            if (direction.teleport_tile)
                direction = direction.Teleport_to_rand();

            send_update = true;
        }


    }
}