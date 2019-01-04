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
        public int endX, endY, height, width;

        public Shape(NodeOFHidenTiles[] _hidenTiles, int _width, int _height, Texture2D Tile_texture, 
            Texture2D cartasian_texture, int starterX, int starterY )
        {
            height = _height;
            width = _width;

            /*hidenTiles = new NodeOFHidenTiles[_hidenTiles.Length] ;
            for (int i = 0; i < hidenTiles.Length; i++)
                hidenTiles[i] = _hidenTiles[i];
                */

            shapeBoard = new Tile[_width][];
            for (int i = 0; i < _width; i++)
                shapeBoard[i] = new Tile[_height];

            int tileSize = 30;
            int hidenIndex = 0;

            
            for (int i = 0; i < _width; ++i)
            {
                for (int j = 0; j < _height; ++j)
                {

                 int x = starterX + i * tileSize;
                 int y = starterY + j * tileSize;
                    
                    if (hidenIndex>-1  && i == _hidenTiles[hidenIndex].i && j == _hidenTiles[hidenIndex].j  )
                    {

                        hidenIndex++;

                        if (hidenIndex >= _hidenTiles.Length)
                            hidenIndex = -1;

                        continue;
                    }


                    Rectangle rec = new Rectangle(350 + x, y, tileSize, tileSize);
                    shapeBoard[i][j] = new Tile(Tile_texture, cartasian_texture, rec);

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
