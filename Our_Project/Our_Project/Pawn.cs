using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public enum team
        {
            my_team,enemy_team
        }

        public Pawn(Texture2D _texture, Tile _tile)
        {
            texture = _texture;
            current_tile = _tile;
        }

        protected void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle Rec = new Rectangle(current_tile.Rec.Location, new Point(40));
            spriteBatch.Draw(texture, Rec, Color.White);

            //drawing adjecant tiles.
            if (current_tile.left != null)
                current_tile.left.Draw(spriteBatch, Color.Red);
            if (current_tile.right != null)
                current_tile.right.Draw(spriteBatch, Color.Red);
            if (current_tile.up != null)
                current_tile.up.Draw(spriteBatch, Color.Red);
            if (current_tile.down != null)
                current_tile.down.Draw(spriteBatch, Color.Red);


        }
    }
}
