
/* Gil Nevo 310021654
 * Shachar Bartal 305262016
 * 
 * This class is for future fetures
 */


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
    public sealed class LevelEditorState : BaseGameState, ILevelEditorState
    {
        // This class will br the first state for Level editor. not in use now


        private SpriteFont font30;
        private Texture2D texture, button_texture;
        ISoundManager soundOfClick;
        private Button to_shape_editor, to_board_editor;

        public LevelEditorState(Game game) : base(game)
        {
            game.Services.AddService(typeof(ILevelEditorState), this);
            soundOfClick = (ISoundManager)game.Services.GetService(typeof(ISoundManager));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        private void ShapeButtonClick(object sender, System.EventArgs e)
        { // go the shape editor
            soundOfClick.Play("click");
            StateManager.ChangeState(OurGame.ShapeEditorState.Value);
        }

        private void BoardButtonClick(object sender, System.EventArgs e)
        { // will take you to board editor
            soundOfClick.Play("click");
            StateManager.ChangeState(OurGame.BoardEditorState.Value);
        }

        protected override void LoadContent()
        {
            font30 = OurGame.font30;
            button_texture = OurGame.button_texture;
            texture = Content.Load<Texture2D>(@"Textures\bg1 (2)");


            //Load buttons:
            to_shape_editor = new Button(Game, button_texture, font30)
            {
                Position = new Vector2(Game1.screen_width / 2 - button_texture.Width, Game1.screen_height / 2 - button_texture.Height / 2),
                Text = "Shape Editor",
            };
            to_shape_editor.Click += ShapeButtonClick;
            Game.Components.Add(to_shape_editor);

            to_board_editor = new Button(Game, button_texture, font30)
            {
                Position = new Vector2(to_shape_editor.Position.X, to_shape_editor.Position.Y - to_shape_editor.Rectangle.Height),
                Text = "Board Editor",
            };
            to_shape_editor.Click += ShapeButtonClick;
            Game.Components.Add(to_shape_editor);
        }

            public override void Draw(GameTime gameTime)
            {
                OurGame.spriteBatch.Draw(texture, new Rectangle(0, 0, Game1.screen_width, Game1.screen_height), Color.White);
                {
                    to_shape_editor.Draw(gameTime, OurGame.spriteBatch);
                    to_board_editor.Draw(gameTime, OurGame.spriteBatch);
                }

                base.Draw(gameTime);
        }
  



        }


    }

