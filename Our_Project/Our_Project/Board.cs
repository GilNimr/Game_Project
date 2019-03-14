
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
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Our_Project.States_and_state_related;

namespace Our_Project
{
   public class Board
    {
        private enum TypeOfBord { fullBoard, Shape }
        private readonly TypeOfBord typeOfBord;
        private int id;                                             // id to tile at this board
        private int height, width;                            // if full board, sizeOfBoard = height = width
        private int starterX, starterY;                             // where begin shape
        private int endOfXaxisOfLastTile, endOfYaxisOfLastTile;  // where each shape are end. (the next will begin)
        private int hidenIndex;
        private int iIndexOfTileToMove, jIndexOfTileToMove;         // maybe not supposed to be here but here its work
        private Tile[][] board;                                     // the board
        private /*public*/ Texture2D tileIsoImg;                               // the isometric image of tile
        private Texture2D tile2dImg;                                // the 2d image of tile
        private readonly Texture2D emtyIsoImg;                               // the empty isometric tile
        private readonly Texture2D empty2dImg;                               // the empty 2d tile
        private readonly int tileSize = Game1.screen_height / 30;   // size of tile
        private List<NodeOFHidenTiles> hidenTiles;                                 // hiden tiles to shape
        public Dictionary <int, Tile> boardDictionaryById;           // get tile by id
        private bool move;                                          // for knowing if move some shape at the moment
       
        public Board(int size, Texture2D isometricTileImage, Texture2D twoDtileImage)
        {/// full board

            typeOfBord = TypeOfBord.fullBoard;
            SetGeneralTypesOfBoard(isometricTileImage, twoDtileImage, size, size); // set types for board or shapes
        }


        public Board(List<NodeOFHidenTiles> _hidenTiles, int _width,
            int _height, int starterX, int starterY, Texture2D ti,
            Texture2D t2d, bool _addToLeft, ContentManager content)
        {
            // shape
            typeOfBord = TypeOfBord.Shape;
            SetShapeTypes(_hidenTiles, starterX, starterY, content); //set types only in shape
            SetGeneralTypesOfBoard(ti, t2d, _height, _width);
        }


        public void Update()
        {
            CheckAndMoveShape(); // checking if mouse drag shape and move it
        }
        
        private void CheckAndMoveShape()
        {
            // checking if mouse drag shape and move it

            MouseState mouseState = Mouse.GetState();
            Vector2 CartasianMouseLocation = Game1.Isometrix2twoD(mouseState.X, mouseState.Y); //we always need to convert to isometric before draw:
            Rectangle mouseRectangle = new Rectangle((int)CartasianMouseLocation.X,
                (int)CartasianMouseLocation.Y, 1, 1);

            for (int i = 0; i < board.Length; i++)
            {
                Vector2 difference; // save the difference between position of mouse to last position of shape (for draw the drag)
                for (int j = 0; j < board[i].Length; j++)
                {
                    if (ClickedShapeAndMove(ref mouseState, ref mouseRectangle, i, j)) // if click on shape
                    {
                        move = true; // and algorithm will set the differnce and move
                        SetIndexesOfTilesWeMoves(out iIndexOfTileToMove, out jIndexOfTileToMove, i, j);
                    }

                    if (move)
                    {
                        difference = SetDifference(iIndexOfTileToMove, jIndexOfTileToMove, ref mouseState);
                        MoveTheShape(difference); // drag the shape up to difference
                    }

                    if (mouseState.LeftButton == ButtonState.Released)
                    {   
                        // if releas the mouse, bool move is false
                        move = false;
                    }
                }
            }
        }

        private void MoveTheShape(Vector2 difference)
        { // drag the shape up to difference
            foreach (Tile[] tilesLine in board)
            {
                foreach (Tile tile in tilesLine)
                {
                    tile.addToCartasianRectangle((int)difference.X, (int)difference.Y); // += the cartisian rectangle of tile
                }
            }
        }
        /*
        private void SetNewShpae(Board shapeToDrow)
        {
            Texture isometricTextureOfTile = shapeToDrow.tileIsoImg;
        }*/

        private Vector2 SetDifference(int iIndexOfTileToMove, int jIndexOfTileToMove, ref MouseState mouseState)
        {
            // set the differnce between the position of shape when dragging for draw better
            Vector2 getRectangleIsometric = Game1.TwoD2isometrix(
                                            board[iIndexOfTileToMove][jIndexOfTileToMove].getCartasianRectangle().X,
                                            board[iIndexOfTileToMove][jIndexOfTileToMove].getCartasianRectangle().Y);
            Vector2 ret = new Vector2((mouseState.X - getRectangleIsometric.X), (mouseState.Y -
                                           getRectangleIsometric.Y));
            return ret;
        }

        private bool ClickedShapeAndMove(ref MouseState mouseState, ref Rectangle mouseRectangle, int i, int j)
        {
            // return true if mouse click on shape 
            return (mouseState.LeftButton == ButtonState.Pressed) &&
                                        (mouseRectangle.Intersects(board[i][j].getCartasianRectangle()));
        }

        private static void SetIndexesOfTilesWeMoves(out int iIndexOfTileToMove, out int jIndexOfTileToMove, int i, int j)
        {
            // set the specific i and j for difference
            iIndexOfTileToMove = i;
            jIndexOfTileToMove = j;
        }

        private void SetGeneralTypesOfBoard(Texture2D isometricTileImage, Texture2D twoDtileImage,
            int height, int width)
        {
            SetSizes(height, width);
            id = 0;
            boardDictionaryById = new Dictionary<int, Tile>();
            SetTexture(isometricTileImage, twoDtileImage); // set the texture of tiles
            SetBoard(); // set all tiles in board (new Tile...)
        }

        private void SetSizes(int height, int width)
        {
            if (typeOfBord == TypeOfBord.fullBoard) // if board, so it is square
                this.height = this.width = height;
            else
            {
                this.height = height;
                this.width = width;
            }
        }

        private void SetTexture(Texture2D isometricTileImage, Texture2D twoDtileImage)
        {
            // set the texture of tiles
            tileIsoImg = isometricTileImage;
            tile2dImg = twoDtileImage;
        }

        private void SetShapeTypes(List<NodeOFHidenTiles> _hidenTiles, int starterX, int starterY, ContentManager content)
        {
            move = false; // because shape start at const place without move
            SetPositionOfShape(starterX, starterY);
            SetHidenTiles(_hidenTiles); // set the hiden tiles at shape
        }

        private void SetPositionOfShape(int starterX, int starterY)
        {
            this.starterX = starterX;
            this.starterY = starterY;
        }



        private void SetHidenTiles(List<NodeOFHidenTiles> _hidenTiles)
        {
            // set hiden tiles in shape
                hidenTiles = new List<NodeOFHidenTiles>();
                for (int i = 0; i < _hidenTiles.Count; i++)
                {
                    hidenTiles.Add(_hidenTiles[i]);
                }
        }

        void SetBoard()
        {
            NewBoard(); // set the tiles as new in board
            SetTiles(); // set the tiles with there texture and position
        }


        private void NewBoard()
        {
            // set the tiles as new in board
            board = new Tile[width][];

            for (int i = 0; i < width; i++)
            {
                board[i] = new Tile[height];
            }
        }


        private void SetTiles()
        {
            // run all the tiles:
            hidenIndex = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int axisXofNewTileRectangle, axisYofNewTileRectangle;

                    if (IsShape()) // if it is shape, we set the positions values and set the tile to shape (can be hiden)
                    {
                        SetXYaxissOfShape(i, j, out axisXofNewTileRectangle, out axisYofNewTileRectangle);
                        CheckAndSetEndOfXYaxiss(i, j, axisXofNewTileRectangle, axisYofNewTileRectangle);
                        SetNewTileOfShape(i, j, axisXofNewTileRectangle, axisYofNewTileRectangle, hidenIndex);
                    }
                    else
                    {   // if it is not shape, set the tiles of board
                        SetXYaxissOfFullBoard(i, j, out axisXofNewTileRectangle, out axisYofNewTileRectangle);
                        SetTileOfFullBoard(i, j, axisXofNewTileRectangle, axisYofNewTileRectangle, hidenIndex);
                    }
                    AddToDictionary(i, j);
                }
            }
        }

        private void AddToDictionary(int i, int j)
        {
            boardDictionaryById.Add(id, board[i][j]);
            id++;
        }

        private void SetXYaxissOfFullBoard(int i, int j, out int axisXofNewTileRectangle, out int axisYofNewTileRectangle)
        {
            axisXofNewTileRectangle = i * tileSize;
            axisYofNewTileRectangle = j * tileSize;
        }

        private void SetXYaxissOfShape(int i, int j, out int axisXofNewTileRectangle, out int axisYofNewTileRectangle)
        {
            axisXofNewTileRectangle = starterX + i * tileSize;
            axisYofNewTileRectangle = starterY + j * tileSize;
        }

        private bool IsShape()
        {
            return typeOfBord == TypeOfBord.Shape;
        }

        private void CheckAndSetEndOfXYaxiss(int i, int j, int axisXofNewTileRectangle, int axisYofNewTileRectangle)
        {
            if (i == width - 1 && j == height - 1)
            {
                endOfXaxisOfLastTile = axisXofNewTileRectangle;
                endOfYaxisOfLastTile = axisYofNewTileRectangle;
            }
        }

        private void SetHidenIndex()
        {
            // next hiden tile of specific shape
            hidenIndex++;
            if (hidenIndex >= hidenTiles.Count)
                hidenIndex = -1;
        }

        private bool HideThisTile(int hidenIndex, int i, int j)
        {
            // return if this i and j supposed to be hiden
            return hidenIndex > -1 && i == hidenTiles[hidenIndex].getI() && j == hidenTiles[hidenIndex].getJ();
        }

        private void SetTileOfFullBoard(int i, int j, int axisXofNewTileRectangle, int axisYofNewTileRectangle,
            int hidenIndex)
        {
            Rectangle rec = new Rectangle(Game1.screen_width / 3 + axisXofNewTileRectangle,
                  -Game1.screen_height / 3 + axisYofNewTileRectangle, tileSize, tileSize);
            board[i][j] = new Tile(tileIsoImg, tile2dImg, rec, id);
        }

        private void SetNewTileOfShape(int i, int j, int axisXofNewTileRectangle, int axisYofNewTileRectangle, int hidenIndex)
        {
            Rectangle rec = new Rectangle(200 + axisXofNewTileRectangle,
            10 + axisYofNewTileRectangle, tileSize, tileSize);

            if (HideThisTile(hidenIndex, i, j)) // if that shape supposed to be hident
            {
                SetNewHidenTile(i, j, rec);
            }

            else // if that shape not hiden
                SetNewShapeTile(i, j, rec);
        }

        private void SetNewShapeTile(int i, int j, Rectangle rec)
        {
            // set tile of shape (not hiden)
            board[i][j] = new Tile(tileIsoImg, tile2dImg, rec, id);
            board[i][j].setIsHidden(false);
        }

        private void SetNewHidenTile(int i, int j, Rectangle rec)
        {
            board[i][j] = new Tile(emtyIsoImg, empty2dImg, rec, id);
            board[i][j].setIsHidden(true);
            SetHidenIndex();
        }

        /*
         * getters and setters:
         */ 

        public int GetStarterX()
        {
            return starterX;
        }

        public int GetStarterY()
        {
            return starterY;
        }

        public int GetHeight()
        {
            return height;
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetEndOfXaxisOfLastTile()
        {
            return endOfXaxisOfLastTile;
        }

        public int GetEndOfYaxisOfLastTile()
        {
            return endOfYaxisOfLastTile;
        }

        public Tile[][] GetBoard()
        {
            return board;
        }

        public bool GetMove()
        {
            return move;
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            if (!BuildingBoardState.i_am_second_player || this.height<10)  //if we are second player there is different draw order
                                                                           //to big emty board only!   
           {
              for (int i = 0; i < width; i++)
              {
                for (int j = 0; j < height; j++)
                {
                    board[i][j].Draw(spriteBatch);
                    board[i][j].setColor(Color.White); //returning to default color in case it was changed.
                }
              }
           }
            else
            {
                for (int i = width-1; i >= 0 ; i--)
                {
                    for (int j = height-1; j >=0 ; j--)
                    {
                        board[i][j].Draw(spriteBatch);
                        board[i][j].setColor(Color.White); //returning to default color in case it was changed.
                    }
                }
            }
        }
   }
}