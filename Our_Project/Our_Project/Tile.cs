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
        private Texture2D realTile;              //the isometric tile texture
        private Texture2D oldTile;              //the 2D tile for debugging.
        private  static  int tileSize;                   // the tile size
        private Rectangle oldRectangle;
        private int id;                         // ID of tile
        private Rectangle isoprojection_rectangle; //the rectangle in which we draw the isometric projection
        private Pawn current_pawn;                  // current pawn on tile
        private Tile left, right, down, up;        //   Tile neighbors

        public Occupied occupied;

        public enum Occupied
        {
            no, yes_by_me, yes_by_enemy
        }

        public Tile(Texture2D _realTile, Texture2D _oldTile, Rectangle _oldRectangle, int _id)
        {
            id = _id;
            tileSize = _oldRectangle.Width;
            realTile = _realTile;
            oldTile = _oldTile;
            oldRectangle = _oldRectangle;
            occupied = Occupied.no;
        }


        protected void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {

            //dubug draw of the cartasian 2d-real world tiles.
            //spriteBatch.Draw(oldTile, oldRectangle, null, color, MathHelper.ToRadians(0f), new Vector2(0), SpriteEffects.None, 0f);

            //creating the isometric-screen rectangle where we will draw our tile.
            Vector2 iso_location = Game1.TwoD2isometrix(oldRectangle.X, oldRectangle.Y);
            isoprojection_rectangle = new Rectangle((int)iso_location.X - tileSize, (int)iso_location.Y, oldRectangle.Width * 2, oldRectangle.Height);

            if (realTile!= null && isoprojection_rectangle != null)
                spriteBatch.Draw(realTile, isoprojection_rectangle, null, color, MathHelper.ToRadians(0f), new Vector2(0), SpriteEffects.None, 0f);
        }

        public int getId()
        {
            return id;
        }

        public static int getTileSize()
        {
            return tileSize;
        }

        public Rectangle getOldRectangle()
        {
            return oldRectangle;
        }

        /*
        public Pawn getCurrentPawn()
        {
            return current_pawn;
        }*/

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

        /*
        public void setCurrentPawn(Pawn p)
        {
            current_pawn = p;
        }*/

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

        public void addToOldRectangle(int x, int y)
        {
            oldRectangle.X += x;
            oldRectangle.Y += y;
        }

        public void setToOldRectangle(int x, int y)
        {
            oldRectangle.X = x;
            oldRectangle.Y = y;
        }

        public Pawn getCurrentPawn()
        {
            return current_pawn;
        }

        public void setCurrentPawn (Pawn p)
        {
            current_pawn = p;
        }



    }
}