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
        private Button save_and_go_placing_soldiers_state_button, reset_button;
        private SpriteFont font;   // font on button
        private SaveForm saveForm;


        public BoardEditorState(Game game) : base(game)
        {
            game.Services.AddService(typeof(IBoardEditorState), this);
            soundEffect = (ISoundManager)game.Services.GetService(typeof(ISoundManager));
            saveForm = new SaveForm();
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

        private void setButtons()
        {
            save_and_go_placing_soldiers_state_button = new Button(Game, OurGame.button_texture, font)
            {
                Position = new Vector2(0,0),
                Text = "Save",

            };

            save_and_go_placing_soldiers_state_button.Click += Save_and_go_placing_soldiers_state_button_clickAsync;
            Game.Components.Add(save_and_go_placing_soldiers_state_button);

            reset_button = new Button(Game, OurGame.button_texture, font)
            {
                Position = new Vector2(save_and_go_placing_soldiers_state_button.Rectangle.X, 
                save_and_go_placing_soldiers_state_button.Rectangle.Height),
                Text = "Reset",
            };
            reset_button.Click += Reset_button_click;
            Game.Components.Add(reset_button);
        }

        private void Reset_button_click(object sender, EventArgs e)
        {
            bigEmptyBoard = null;
            BuildEmptyBoard();
        }

        // Click on the save button
        private async void Save_and_go_placing_soldiers_state_button_clickAsync(object sender, System.EventArgs e)
        {
            bool flag = true;
            int counter = -48;  // 48 tiles of middle line. counter will be responsible about legal number of tiles
            List<String> strings = new List<String>();
            strings.Add("$");

            // set each texture-tile in bigEmptyBoard that without shape as null
            foreach (Tile[] tileLine in bigEmptyBoard.GetBoard())
            {
                String s = "";
                foreach (Tile t in tileLine)
                {
                    if (t.GetIsHidden())
                    {
                        t.texture = null;
                        s += '0';
                    }

                    else
                    {
                        s += '1';
                    }
                }
                strings.Add(s);
            }
            strings.Add("$");
            SetNeighbors(bigEmptyBoard);


            foreach (Tile[] emptyTilesLine in bigEmptyBoard.GetBoard())
            {
                foreach (Tile t in emptyTilesLine)
                {
                    if ((!t.GetIsHidden() && (t.GetId() < 264 || counter > 25 || !legalTile(t))))
                    {
                        flag = false;
                    }

                    else if (!t.GetIsHidden())
                    {
                        counter++;

                    }
                }
            }

            if (flag)
            {
                soundEffect.Play("click");

                saveForm.Show();
                
                while (saveForm.getName() == null)
                {
                    await Task.Delay(25);
                }

                System.IO.File.WriteAllLines(@"‪..\..\..\..\..\..\Content\Files\" + saveForm.getName()+
                    ".txt", strings);
                
            }
            else
            {
                soundEffect.Play("badPlace");
                bigEmptyBoard = null;
                BuildEmptyBoard();

            }

        }

        private bool legalTile(Tile t) // check if all the tiles is with at least 2 legal neighburs.
        {
            if (t.GetUp() != null && t.GetDown() != null)
            {
                if (t.GetRight() != null && t.GetRight().texture != null && t.GetUp().texture == null && t.GetDown().texture == null)
                    return false;

                else if (t.GetLeft() != null && t.GetLeft().texture != null && t.GetUp().texture == null && t.GetDown().texture == null)
                    return false;
            }

            if (t.GetRight() != null && t.GetLeft() != null)
            {
                if (t.GetUp() != null && t.GetUp().texture != null && t.GetRight().texture == null && t.GetLeft().texture == null)
                    return false;

                else if (t.GetDown() != null && t.GetDown().texture != null && t.GetRight().texture == null && t.GetLeft().texture == null)
                    return false;
            }
            
            return true;
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // load all textures
            fullTile2d = OurGame.Content.Load<Texture2D>(@"Textures\Tiles\Gray_Tile");
            fullTileIso = Content.Load<Texture2D>(@"Textures\Tiles\Grey Tile");
            emptyTile2d = Content.Load<Texture2D>(@"Textures\Tiles\White_2d_Tile");
            emptyTileIso = Content.Load<Texture2D>(@"Textures\Tiles\Clear Tile");
            font = OurGame.font30;
            
            //build BigEmptyBoard
            BuildEmptyBoard();
            setButtons();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            foreach (Tile[] emptyTilesLine in bigEmptyBoard.GetBoard())
            {
                foreach (Tile t in emptyTilesLine)
                {
                    t.Update();

                    if (t.GetIsMouseClicked())
                    {
                        if (t.GetIsHidden())
                        {
                            t.texture = fullTileIso;   // set textur
                            t.SetIsHidden(false);      // set boolean type isHiden
                        }

                        else
                        {
                            t.texture = emptyTileIso;   // set textur
                            t.SetIsHidden(true);      // set boolean type isHiden
                        }
                        
                    }
                }
            }
        }



        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            bigEmptyBoard.Draw(OurGame.spriteBatch, Color.White);
            save_and_go_placing_soldiers_state_button.Draw(gameTime, OurGame.spriteBatch);
            reset_button.Draw(gameTime, OurGame.spriteBatch);


            base.Draw(gameTime);



        }
    }
}
