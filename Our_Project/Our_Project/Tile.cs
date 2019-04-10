
/* Gil Nevo 310021654
 * Shachar Bartal 305262016
 */


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

        
        private readonly Texture2D cartasian_texture;              //the 2D tile for debugging.
        private static  int tileSize;                               // the tile size
        private Rectangle cartasianRectangle;
        private Rectangle isoprojection_rectangle;        //the rectangle in which we draw the isometric projection of tile.
        private readonly int id;                                              // ID of tile
        private Pawn current_pawn;                           // current pawn on tile
        private Tile left, right, down, up;                  //  the tile's neighbors
        private Color color;
        private bool isHidden;                              // if hidden tile

        public Texture2D texture;                          //the isometric tile texture
        public float Depth=0f;  //need to delete.
        public bool sendUpdate = false; //send update to server.
        public bool teleport_tile; //is this tile a teleport tile.
        public Occupied occupied;
        public enum Occupied
        {
            no, yes_by_me, yes_by_enemy
        }

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
            //creating the isometric-screen rectangle where we will draw our tile.
            Vector2 iso_location = Game1.TwoD2isometrix(cartasianRectangle.X, cartasianRectangle.Y);
            isoprojection_rectangle = new Rectangle((int)iso_location.X - tileSize, (int)iso_location.Y, cartasianRectangle.Width *2, cartasianRectangle.Height*2);

            if (texture!= null && isoprojection_rectangle != null) //draw not null textures
                spriteBatch.Draw(texture, isoprojection_rectangle, null, color, MathHelper.ToRadians(0f), new Vector2(0), SpriteEffects.None, Depth);
        }

        public int GetId()
        {
            return id;
        }

        public static int GetTileSize()
        {
            return tileSize;
        }

        public Rectangle GetCartasianRectangle()
        {
            return cartasianRectangle;
        }

        public void SetCartasianRectangle(Rectangle rec)
        {
            cartasianRectangle = rec;
        }


        public Pawn GetCurrentPawn()
        {
            return current_pawn;
        }

        public Tile GetRight()
        {
            return right;
        }

        public Tile GetLeft()
        {
            return left;
        }

        public Tile GetUp()
        {
            return up;
        }

        public Tile GetDown()
        {
            return down;
        }
        
        public void SetRight(Tile r)
        {
            right = r;
        }

        public void SetLeft(Tile l)
        {
            left = l;
        }

        public void SetUp(Tile u)
        {
            up = u;
        }

        public void SetDown(Tile d)
        {
            down = d;
        }

        //in case we are draggin the tile in build mode.
        public void AddToCartasianRectangle(int x, int y)
        {
            cartasianRectangle.X += x;
            cartasianRectangle.Y += y;
        }

        public void SetToCartasianRectangle(int x, int y)
        {
            cartasianRectangle.X = x;
            cartasianRectangle.Y = y;
        }

        public void SetColor(Color _color)
        {
            color = _color;
        }
    
        //method to find next random teleport
        public Tile Teleport_to_rand()
        {
            Random rand = new Random();
            int rand_number = rand.Next(0, 2);
            int index_to_prevent_loop=0;
            while (PlayingState.teleports[rand_number] == this || PlayingState.teleports[rand_number].occupied!=Occupied.no)
            {
                rand_number = rand.Next(0, 4);
                index_to_prevent_loop++;

                if (index_to_prevent_loop == 10)
                    break;
            }
            if (index_to_prevent_loop == 10)
                return this;

            return PlayingState.teleports[rand_number];
        }


        public void SetCurrentPawn (Pawn p)
        {
            current_pawn = p;
        }

        public void SetIsHidden(bool b)
        {
            isHidden = b;
        }

        public bool GetIsHidden()
        {
            return isHidden;
        }

        




    }
}