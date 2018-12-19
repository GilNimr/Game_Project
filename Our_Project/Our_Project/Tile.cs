using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our_Project
{
    class Tile
    {
        private Texture2D texture;
        public Rectangle Rec;
        public Tile left;
        public Tile right;
        public Tile down;
        public Tile up;

        enum occupied
        {
            no,yes_by_me,yes_by_enemy
        }

       public Tile(Texture2D _texture, Rectangle rec)
        {
            texture = _texture;
            Rec = rec;
        }

        protected void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(texture, Rec, color);
        }


    }
}
