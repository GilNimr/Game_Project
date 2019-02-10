using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our_Project
{
    class Shape
    {
        public Tile[][] shapeBoard;   // the board of the game
                                      //NodeOFHidenTiles[] hidenTiles;
                     
        public int endX, endY, height, width,id;   // where x and y axis are ending, and height/width of shape
        public bool addToLeft = false;
        // build the shape:
        public Shape(NodeOFHidenTiles[] _hidenTiles, int _width, int _height, Texture2D Tile_texture, 
            Texture2D cartasian_texture, int starterX, int starterY , bool _addToLeft,int _id)
        {   // starter x and yo we buid from endX and endY from the other shapes
            id = _id;
            height = _height;
            width = _width;
            addToLeft = _addToLeft;
            shapeBoard = new Tile[_width][];

            for (int i = 0; i < _width; i++)
                shapeBoard[i] = new Tile[_height];

            int tileSize = Game1.screen_height / 30;  // const
            int hidenIndex = 0; // where is the indexes we want to hide (hiden tiles)

            
            for (int i = 0; i < _width; ++i)
            {
                for (int j = 0; j < _height; ++j)
                {
                 int x = starterX + i * tileSize;
                 int y = starterY + j * tileSize;
                    
                    if (hidenIndex>-1  && i == _hidenTiles[hidenIndex].getI() && j == _hidenTiles[hidenIndex].getJ()  )
                    {   // skip hiden tile
                        hidenIndex++;

                        if (hidenIndex >= _hidenTiles.Length)
                            hidenIndex = -1;

                        continue;
                    }

                    // build tiles at the shape
                    Rectangle rec = new Rectangle(Game1.screen_width/3 + x, y, tileSize, tileSize);
                    shapeBoard[i][j] = new Tile(Tile_texture, cartasian_texture, rec,id);
                   PlayingState. tileDictionary.Add(id, shapeBoard[i][j]);
                    id++;
                    if (i == _width-1 && j == _height - 1)
                    {
                        endX = x;
                        endY = y;
                    }
                }
            }
            hidenIndex = 0;
        }
    }
}
