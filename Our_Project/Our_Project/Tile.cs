using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our_Project
{
   public class Tile
    {

        public Texture2D texture;              //the isometric tile texture
        private Texture2D cartasian_texture;              //the 2D tile for debugging.
        private  static  int tileSize;                   // the tile size
        private Rectangle cartasianRectangle;
        private int id;                         // ID of tile
        private Rectangle isoprojection_rectangle; //the rectangle in which we draw the isometric projection
        private Pawn current_pawn;                  // current pawn on tile
        private Tile left, right, down, up;        //   Tile neighbors
        private Color color;
        private bool isHidden;

        public Occupied occupied;

        public enum Occupied
        {
            no, yes_by_me, yes_by_enemy
        }


        public bool teleport_tile;


        public Tile(Texture2D _texture, Texture2D _cartasian_texture, Rectangle _cartasianRectangle, int _id)

        {
            teleport_tile = false ;
            id = _id;
            tileSize = _cartasianRectangle.Width;
            texture = _texture;
            cartasian_texture = _cartasian_texture;
            cartasianRectangle = _cartasianRectangle;
            occupied = Occupied.no;
            color = Color.White;
            isHidden = true;
        }


        protected void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            //dubug draw of the cartasian 2d-real world tiles.
            //spriteBatch.Draw(oldTile, oldRectangle, null, color, MathHelper.ToRadians(0f), new Vector2(0), SpriteEffects.None, 0f);

            //creating the isometric-screen rectangle where we will draw our tile.
            Vector2 iso_location = Game1.TwoD2isometrix(cartasianRectangle.X, cartasianRectangle.Y);
            isoprojection_rectangle = new Rectangle((int)iso_location.X - tileSize, (int)iso_location.Y, cartasianRectangle.Width *2, cartasianRectangle.Height*2);

            if (texture!= null && isoprojection_rectangle != null)
                spriteBatch.Draw(texture, isoprojection_rectangle, null, color, MathHelper.ToRadians(0f), new Vector2(0), SpriteEffects.None, 0f);
        }

        public int getId()
        {
            return id;
        }

        public static int getTileSize()
        {
            return tileSize;
        }

        public Rectangle getCartasianRectangle()
        {
            return cartasianRectangle;
        }

       
        public Pawn getCurrentPawn()
        {
            return current_pawn;
        }

        public Tile getRight()
        {
            return right;
        }

        public Tile getLeft()
        {
            return left;
        }

        public Tile getUp()
        {
            return up;
        }

        public Tile getDown()
        {
            return down;
        }

        
       

        public void setRight(Tile r)
        {
            right = r;
        }

        public void setLeft(Tile l)
        {
            left = l;
        }

        public void setUp(Tile u)
        {
            up = u;
        }

        public void setDown(Tile d)
        {
            down = d;
        }

        public void addToCartasianRectangle(int x, int y)
        {
            cartasianRectangle.X += x;
            cartasianRectangle.Y += y;
        }

        public void setToCartasianRectangle(int x, int y)
        {
            cartasianRectangle.X = x;
            cartasianRectangle.Y = y;
        }

        public void setColor(Color _color)
        {
            color = _color;
        }
    


        public Tile Teleport_to_rand()
        {
            Random rand = new Random();
            int rand_number = rand.Next(0, 2);
            while (PlayingState.teleports[rand_number] == this)
            {
                rand_number = rand.Next(0, 2);
            }
            return PlayingState.teleports[rand_number];
            
            
        }


        public void setCurrentPawn (Pawn p)
        {
            current_pawn = p;
        }

        public void setIsHidden(bool b)
        {
            isHidden = b;
        }

        public bool getIsHidden()
        {
            return isHidden;
        }

        




    }
}