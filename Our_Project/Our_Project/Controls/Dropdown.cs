using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our_Project.Controls
{
    class Dropdown : GameComponent
    {
        Button master;
        Texture2D master_texture;
        Texture2D texture;
        int rows;
        
        int amount;
        SpriteFont font;
        public Dropdown(Game game, int _amount ,int _rows, Texture2D _master_texture, Texture2D _texture, SpriteFont _font ):base(game)
        {
            master_texture = _master_texture;
            texture = _texture;
            rows = _rows;
            amount = _amount;
            font = _font;

            master = new Button(game, master_texture, font);
        }
       
    }
}
