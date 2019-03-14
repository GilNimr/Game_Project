
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
        enum TypeOfBord { fullBoard, Shape }

        TypeOfBord typeOfBord;
        Tile[][] board;                                     // the board
       public Texture2D tileIsoImg;                               // the isometric image of tile
        Texture2D tile2dImg;                                // the 2d image of tile
        Texture2D emtyIsoImg;                               // the empty isometric tile
        Texture2D empty2dImg;                               // the empty 2d tile
        readonly int tileSize = Game1.screen_height / 30;   // size of tile
        int id;                                             // id to tile at this board
        int height, width;                            // if full board, sizeOfBoard = height = width
        int starterX, starterY;                             // where begin shape
        int endOfXaxisOfLastTile, endOfYaxisOfLastTile;  // where each shape are end. (the next will begin)
        public /*static*/ Dictionary<int, Tile> boardDictionaryById;   // get tile by id
        List<NodeOFHidenTiles> hidenTiles;                                 // hiden tiles to shape
        int hidenIndex;
        bool move; // for knowing if move some shape
        int iIndexOfTileToMove, jIndexOfTileToMove;         // maybe not supposed to be here but here its work


        //private SpriteFont font;
        string printDebug = "just for debug";


        public Board(int size, Texture2D isometricTileImage, Texture2D twoDtileImage)
        {/// full board

            typeOfBord = TypeOfBord.fullBoard;

            setGeneralTypesOfBoard(isometricTileImage, twoDtileImage, size, size);
        }


        public Board(List<NodeOFHidenTiles> _hidenTiles, int _width,
            int _height, int starterX, int starterY, Texture2D ti,
            Texture2D t2d, bool _addToLeft, ContentManager content)
        {// shape
            typeOfBord = TypeOfBord.Shape;
            setShapeTypes(_hidenTiles, starterX, starterY, content);
            setGeneralTypesOfBoard(ti, t2d, _height, _width);

        }


        public void Update()
        {

            checkAndMoveShape();

        }



        private void checkAndMoveShape()
        {

            MouseState mouseState = Mouse.GetState();

            Vector2 CartasianMouseLocation = Game1.Isometrix2twoD(mouseState.X, mouseState.Y);

            Rectangle mouseRectangle = new Rectangle((int)CartasianMouseLocation.X,
                (int)CartasianMouseLocation.Y, 1, 1);

            for (int i = 0; i < board.Length; i++)
            {
                Vector2 difference;
                for (int j = 0; j < board[i].Length; j++)
                {
                    if (clickedShapeAndMove(ref mouseState, ref mouseRectangle, i, j))
                    {
                        move = true;
                        setIndexesOfTilesWeMoves(out iIndexOfTileToMove, out jIndexOfTileToMove, i, j);
                    }

                    if (move)
                    {
                        difference = setDifference(iIndexOfTileToMove, jIndexOfTileToMove, ref mouseState);

                        moveTheShape(difference);
                    }

                    if (mouseState.LeftButton == ButtonState.Released)
                    {
                        move = false;
                    }
                }
            }
        }

        private void moveTheShape(Vector2 difference)
        {
            foreach (Tile[] tilesLine in board)
            {
                foreach (Tile tile in tilesLine)
                {
                    tile.addToCartasianRectangle((int)difference.X, (int)difference.Y);
                }
            }
        }

        private void setNewShpae(Board shapeToDrow)
        {
            Texture isometricTextureOfTile = shapeToDrow.tileIsoImg;

            
        }

        private Vector2 setDifference(int iIndexOfTileToMove, int jIndexOfTileToMove, ref MouseState mouseState)
        {
            Vector2 getRectangleIsometric = Game1.TwoD2isometrix(
                board[iIndexOfTileToMove][jIndexOfTileToMove].getCartasianRectangle().X,
                board[iIndexOfTileToMove][jIndexOfTileToMove].getCartasianRectangle().Y);


            Vector2 ret = new Vector2((mouseState.X - getRectangleIsometric.X), (mouseState.Y -
                                           getRectangleIsometric.Y));

            return ret;
        }

        private bool clickedShapeAndMove(ref MouseState mouseState, ref Rectangle mouseRectangle, int i, int j)
        {
            return (mouseState.LeftButton == ButtonState.Pressed) &&
                                        (mouseRectangle.Intersects(board[i][j].getCartasianRectangle()));
        }

        private static void setIndexesOfTilesWeMoves(out int iIndexOfTileToMove, out int jIndexOfTileToMove, int i, int j)
        {
            iIndexOfTileToMove = i;
            jIndexOfTileToMove = j;
        }

        private void setGeneralTypesOfBoard(Texture2D isometricTileImage, Texture2D twoDtileImage,
            int height, int width)
        {
            setSizes(height, width);

            id = 0;

            boardDictionaryById = new Dictionary<int, Tile>();

            setTexture(isometricTileImage, twoDtileImage);
            setBoard();
        }

        private void setSizes(int height, int width)
        {
            if (typeOfBord == TypeOfBord.fullBoard)
                this.height = this.width = height;
            else
            {
                this.height = height;
                this.width = width;
            }
        }

        private void setTexture(Texture2D isometricTileImage, Texture2D twoDtileImage)
        {
            tileIsoImg = isometricTileImage;
            tile2dImg = twoDtileImage;
        }

        private void setShapeTypes(List<NodeOFHidenTiles> _hidenTiles, int starterX, int starterY, ContentManager content)
        {
            //font = content.Load<SpriteFont>("font");
            move = false;
            setPositionOfShape(starterX, starterY);
            setHidenTiles(_hidenTiles);
            setEmptyTilesImg(content);
        }

        private void setPositionOfShape(int starterX, int starterY)
        {
            this.starterX = starterX;
            this.starterY = starterY;
        }



        private void setHidenTiles(List<NodeOFHidenTiles> _hidenTiles)
        {
                hidenTiles = new List<NodeOFHidenTiles>();
                for (int i = 0; i < _hidenTiles.Count; i++)
                {
                    hidenTiles.Add(_hidenTiles[i]);
                }
        }

        private void setEmptyTilesImg(ContentManager content)
        {
            /* empty2dImg = content.Load<Texture2D>("White_2d_Tile");
             emtyIsoImg = content.Load<Texture2D>("White_Isometric_Tile");
         */
        }

        void setBoard()
        {
            newBoard();

            setTiles();

        }


        private void newBoard()
        {
            board = new Tile[width][];

            for (int i = 0; i < width; i++)
            {
                board[i] = new Tile[height];
            }
        }


        private void setTiles()
        {

            hidenIndex = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int axisXofNewTileRectangle, axisYofNewTileRectangle;

                    if (isShape())
                    {
                        setXYaxissOfShape(i, j, out axisXofNewTileRectangle, out axisYofNewTileRectangle);
                        checkAndSetEndOfXYaxiss(i, j, axisXofNewTileRectangle, axisYofNewTileRectangle);
                        setNewTileOfShape(i, j, axisXofNewTileRectangle, axisYofNewTileRectangle, hidenIndex);
                    }
                    else
                    {
                        setXYaxissOfFullBoard(i, j, out axisXofNewTileRectangle, out axisYofNewTileRectangle);

                        setTileOfFullBoard(i, j, axisXofNewTileRectangle, axisYofNewTileRectangle, hidenIndex);
                    }
                    addToDictionary(i, j);
                }
            }
        }

        private void addToDictionary(int i, int j)
        {
            boardDictionaryById.Add(id, board[i][j]);
            id++;
        }

        private void setXYaxissOfFullBoard(int i, int j, out int axisXofNewTileRectangle, out int axisYofNewTileRectangle)
        {
            axisXofNewTileRectangle = i * tileSize;
            axisYofNewTileRectangle = j * tileSize;
        }

        private void setXYaxissOfShape(int i, int j, out int axisXofNewTileRectangle, out int axisYofNewTileRectangle)
        {
            axisXofNewTileRectangle = starterX + i * tileSize;
            axisYofNewTileRectangle = starterY + j * tileSize;
        }

        private bool isShape()
        {
            return typeOfBord == TypeOfBord.Shape;
        }

        private void checkAndSetEndOfXYaxiss(int i, int j, int axisXofNewTileRectangle, int axisYofNewTileRectangle)
        {
            if (i == width - 1 && j == height - 1)
            {
                endOfXaxisOfLastTile = axisXofNewTileRectangle;
                endOfYaxisOfLastTile = axisYofNewTileRectangle;
            }
        }

        private void setHidenIndex()
        {
            hidenIndex++;
            if (hidenIndex >= hidenTiles.Count)
                hidenIndex = -1;

        }

        private bool hideThisTile(int hidenIndex, int i, int j)
        {
            return hidenIndex > -1 && i == hidenTiles[hidenIndex].getI() && j == hidenTiles[hidenIndex].getJ();
        }

        private void setTileOfFullBoard(int i, int j, int axisXofNewTileRectangle, int axisYofNewTileRectangle,
            int hidenIndex)
        {
            Rectangle rec = new Rectangle(Game1.screen_width / 3 + axisXofNewTileRectangle,
                  -Game1.screen_height / 3 + axisYofNewTileRectangle, tileSize, tileSize);
            board[i][j] = new Tile(tileIsoImg, tile2dImg, rec, id);
        }

        private void setNewTileOfShape(int i, int j, int axisXofNewTileRectangle, int axisYofNewTileRectangle, int hidenIndex)
        {
            Rectangle rec = new Rectangle(200 + axisXofNewTileRectangle,
            10 + axisYofNewTileRectangle, tileSize, tileSize);

            if (hideThisTile(hidenIndex, i, j))
            {
                setNewHidenTile(i, j, rec);
            }

            else
                setNewShapeTile(i, j, rec);
        }

        private void setNewShapeTile(int i, int j, Rectangle rec)
        {
            board[i][j] = new Tile(tileIsoImg, tile2dImg, rec, id);
            board[i][j].setIsHidden(false);
        }

        private void setNewHidenTile(int i, int j, Rectangle rec)
        {
            board[i][j] = new Tile(emtyIsoImg, empty2dImg, rec, id);
            //board[i][j].setIsHidden(true);
            setHidenIndex();
        }

        public int getStarterX()
        {
            return starterX;
        }

        public int getStarterY()
        {
            return starterY;
        }

        public int getHeight()
        {
            return height;
        }

        public int getWidth()
        {
            return width;
        }

        public int getEndOfXaxisOfLastTile()
        {
            return endOfXaxisOfLastTile;
        }

        public int getEndOfYaxisOfLastTile()
        {
            return endOfYaxisOfLastTile;
        }

        public Tile[][] getBoard()
        {
            return board;
        }

        public bool getMove()
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