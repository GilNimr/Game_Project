﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private Button save_and_go_placing_soldiers_state_button, reset_button, return_to_game_button;
        private SpriteFont font;   // font on button
        public /*static*/ SaveForm saveForm;
        private  List<string> txtBoardForForm;

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
            txtBoardForForm = null;
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

            save_and_go_placing_soldiers_state_button.Click += Save_and_go_placing_soldiers_state_button_click;
            Game.Components.Add(save_and_go_placing_soldiers_state_button);

            reset_button = new Button(Game, OurGame.button_texture, font)
            {
                Position = new Vector2(save_and_go_placing_soldiers_state_button.Rectangle.X, 
                save_and_go_placing_soldiers_state_button.Rectangle.Height),
                Text = "Reset",
            };
            reset_button.Click += Reset_button_click;
            Game.Components.Add(reset_button);

            return_to_game_button = new Button(Game, OurGame.button_texture, font)
            {
                Position = new Vector2(save_and_go_placing_soldiers_state_button.Rectangle.X, 
                reset_button.Rectangle.Height*2),
                Text = "Return to the game",
            };
            return_to_game_button.Click += Return_to_game_click;
            Game.Components.Add(return_to_game_button);
        }

        private void Return_to_game_click(object sender, EventArgs e)
        {
            soundEffect.Play("click");
            StateManager.PushState(OurGame.StartMenuState.Value);
                        //Push
          //  StateManager.ChangeState(OurGame.BuildingBoardState.Value);
        }

        private void Reset_button_click(object sender, EventArgs e)
        {
            bigEmptyBoard = null;
            BuildEmptyBoard();
        }

        // Click on the save button
        private void Save_and_go_placing_soldiers_state_button_click(object sender, System.EventArgs e)
        {
            bool flag = true, closeToMiddleLine = false ; // closeTo will be on if we connected to middle line
            int counter = -48;  // 48 tiles of middle line. counter will be responsible about legal number of tiles
            List<String> strings = new List<String>();
            strings.Add("$");

            // set each texture-tile in bigEmptyBoard that without shape as null
            int lineNumber = -1; // for draw 0 when middle line
            foreach (Tile[] tileLine in bigEmptyBoard.GetBoard())
            {
                
                String s = "";   

                lineNumber++;
                foreach (Tile t in tileLine)
                {

                    if (t.GetIsHidden() || lineNumber<=10)
                    {
                        t.texture = null;
                        s += '0';
                    }

                    else
                    {
                        s += '1';
                        if (lineNumber == 13)
                            closeToMiddleLine = true;
                    }
                }
                strings.Add(s);
            }
            strings.Add("$");
            SetNeighbors(bigEmptyBoard);
            

            System.IO.File.WriteAllLines(@"‪..\..\..\..\..\..\Content\Files\delete.txt", strings);
            
            foreach (Tile[] emptyTilesLine in bigEmptyBoard.GetBoard())
            {
                foreach (Tile t in emptyTilesLine)
                {
                    
                    if ( !closeToMiddleLine || (!t.GetIsHidden() && (t.GetId() < 264 || counter > 25 || !legalTile(t)))) 
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
                saveForm = new SaveForm();
                saveForm.Show();

                ResetBoardEditor(); // returning bigEmptyBoard
                saveForm = null;

            }
            else
            {
                soundEffect.Play("badPlace");
                ResetBoardEditor(); // returning bigEmptyBoard

            }

        }

        private void ResetBoardEditor() // returning bigEmptyBoard
        {
            bigEmptyBoard = null;
            BuildEmptyBoard();
        }

        private bool legalTile(Tile t) // check if all the tiles is with at least 2 legal neighburs.
        {
            if (t == null || t.texture == null)
                return true;

            else if (t.GetUp() != null && t.GetUp().texture != null
               && t.GetRight() != null && t.GetRight().texture == null
                  && t.GetLeft() != null && t.GetLeft().texture == null)
                return false;

            else if (t.GetDown() != null && t.GetDown().texture != null
                && t.GetRight() != null && t.GetRight().texture == null
                  && t.GetLeft() != null && t.GetLeft().texture == null)
                return false;

            else if (t.GetRight() != null && t.GetRight().texture != null
                && t.GetUp() != null && t.GetUp().texture == null
                && t.GetDown() != null && t.GetDown().texture == null)
                return false;

            else if (t.GetLeft() != null && t.GetLeft().texture != null
                && t.GetUp() != null && t.GetUp().texture == null
                && t.GetDown() != null && t.GetDown().texture == null)
                return false;

            else
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
            return_to_game_button.Draw(gameTime, OurGame.spriteBatch);

            base.Draw(gameTime);



        }
    }
}
