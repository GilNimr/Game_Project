using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XELibrary;

namespace Our_Project.States_and_state_related
{
    public sealed class BoardEditorState : BaseGameState, IBoardEditorState
    {
        // fullTile - tile texture. emptyTile - free tile for put a shape
        private Texture2D fullTile2d, fullTileIso, emptyTile2d, emptyTileIso;
        private ISoundManager soundEffect;
        private Board bigEmptyBoard;    // the big board we build our area on it
        private bool isPlayBadPlaceSoundEffect;     // boolean for checking if turn on the bad place sound


        public BoardEditorState(Game game) : base(game)
        {
            game.Services.AddService(typeof(IBoardEditorState), this);
            soundEffect = (ISoundManager)game.Services.GetService(typeof(ISoundManager));

        }

        private void BuildEmptyBoard()
        {
            // set the board
            bigEmptyBoard = new Board(24, emptyTileIso, emptyTile2d);

            // adding a 2/24 shape wich will be the middle of the board, set the hiden tiles and call them at the next metod
            List<NodeOFHidenTiles> empty = new List<NodeOFHidenTiles>
            {
                new NodeOFHidenTiles(-1, -1),
                new NodeOFHidenTiles(-1, -1)
            };

            // setting the middle line of bigEmptyBoard
            SetMiddleLine(new Board(empty, 2, 12, bigEmptyBoard.GetBoard()[12][0].GetCartasianRectangle().X,
                bigEmptyBoard.GetBoard()[12][0].GetCartasianRectangle().Y, fullTileIso, null, false, this.Content));
        }

        private void SetMiddleLine(Board line)  // setting the middle line of bigEmptyBoard
        {
            Texture2D fullTexture = line.GetBoard()[0][0].texture;

            for (int i = 11; i <= 12; i++)
            {
                for (int j = 0; j < bigEmptyBoard.GetBoard()[i].Length; j++)
                {
                    bigEmptyBoard.GetBoard()[i][j].texture = fullTexture;   // set textur
                    bigEmptyBoard.GetBoard()[i][j].SetIsHidden(false);      // set boolean type isHiden
                }
            }
            SetNeighbors(bigEmptyBoard);    // set the neighbors
        }

        private void SetNeighbors(Board b)
        {
            // set the neighbors of board b. as we have a lot of change, we need to update it each change
            for (int i = 0; i < b.GetBoard().Length; ++i)
            {
                for (int j = 0; j < b.GetBoard()[i].Length; ++j)
                {
                    if (b.GetBoard()[i][j] != null)
                    {
                        //right
                        if (i < b.GetBoard().Length - 1)
                            b.GetBoard()[i][j].SetRight(b.GetBoard()[i + 1][j]); // x axis grow up

                        //left
                        if (i >= 1)
                            b.GetBoard()[i][j].SetLeft(b.GetBoard()[i - 1][j]); // x axis go down

                        //down
                        if (j < b.GetBoard()[i].Length - 1)
                            b.GetBoard()[i][j].SetDown(b.GetBoard()[i][j + 1]); // y axis grow up
                                                                                //up
                        if (j >= 1)
                            b.GetBoard()[i][j].SetUp(b.GetBoard()[i][j - 1]); // y axis go down
                    }
                }
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            BuildEmptyBoard();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            bigEmptyBoard.Draw(OurGame.spriteBatch, Color.White);
            base.Draw(gameTime);



        }
    }
}
