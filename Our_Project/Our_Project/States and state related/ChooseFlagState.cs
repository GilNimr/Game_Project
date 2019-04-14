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
  public class ChooseFlagState : BaseGameState, IChooseFlagState
    {
        private Texture2D main;

        public string[] flags;
        private int currFlag;
        public int taken = -1;
       
        private Button left;
        private Button right;
        private Button select;
        private Color color;

        ICelAnimationManager celAnimationManager;
        private StartMenuState startMenuState;
        private Connection connection;

        public ChooseFlagState(Game game) : base(game)
        {
            game.Services.AddService(typeof(IChooseFlagState), this);
            startMenuState = (StartMenuState)game.Services.GetService(typeof(IStartMenuState));
            
            celAnimationManager = (ICelAnimationManager)game.Services.GetService(typeof(ICelAnimationManager));
            color = Color.White;
        }
        protected override void LoadContent()
        {
            base.LoadContent();
            connection = startMenuState.connection;
            flags = new string[3];
            flags[0]="jamaica";
            flags[1]="canada";
            flags[2]="israel";
            currFlag = 1;
            

            main = Content.Load<Texture2D>(@"Textures\Controls\choose_flag_menu");
            left = new Button(OurGame, OurGame.button_texture, OurGame.font30)
            {
                Text = "Left",
                Position=new Vector2(Game1.screen_width/4 , Game1.screen_height/2)
            };
            left.Click += Left_Click;
            right = new Button(OurGame, OurGame.button_texture, OurGame.font30)
            {
                Text = "Right",
                Position = new Vector2((Game1.screen_width / 4)*3 - left.Rectangle.Width , Game1.screen_height / 2)
            };
            right.Click += Right_Click;
            select = new Button(OurGame, OurGame.button_texture, OurGame.font30)
            {
                Text = "Select",
                Position = new Vector2((Game1.screen_width / 2) - left.Rectangle.Width/2, Game1.screen_height / (2 *3))
            };
            select.Click += Select_Click;
            OurGame.Components.Add(left);
            OurGame.Components.Add(right);
            OurGame.Components.Add(select);
    
        }

        private void Select_Click(object sender, EventArgs e)
        {
            if (currFlag != taken)
            {
                connection.player.flag = flags[currFlag];
                connection.SendFlagChoise(currFlag);
                OurGame.Components.Remove(left);
                OurGame.Components.Remove(right);
                OurGame.Components.Remove(select);
                StateManager.ChangeState(OurGame.BuildingBoardState.Value);
                
            }
        }

        private void Right_Click(object sender, EventArgs e)
        {
            currFlag--;
            if (currFlag == -1)
                currFlag = 0;
        }

        private void Left_Click(object sender, EventArgs e)
        {
            currFlag++;
            if (currFlag == flags.Length)
                currFlag = flags.Length-1;
        }

        public override void Update(GameTime gameTime)
        {
            
            base.Update(gameTime);
            connection.Update();

        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            for (int i = 0; i < flags.Length; i++)
            {
                if (i == taken)
                    color = Color.Red;
                else
                    color = Color.White;
                if(i==currFlag)
                celAnimationManager.Draw(gameTime, flags[i], OurGame.spriteBatch, new Rectangle((Game1.screen_width / 16) * 7, (Game1.screen_height / 8) * 3, Game1.screen_width / 8, Game1.screen_height / 4),color, SpriteEffects.None);
                else
                {
                celAnimationManager.Draw(gameTime, flags[i], OurGame.spriteBatch, new Rectangle((Game1.screen_width / 16) * 7 + (currFlag - i) * (Game1.screen_width / 8)/*x*/, (Game1.screen_height / 8) * 3 - ((int)  Math.Pow(Math.Abs( currFlag - i),2)*(Game1.screen_height/50)) /*y*/, (Game1.screen_width / 8)*(10 - Math.Abs(currFlag - i))/10 /*width*/, (Game1.screen_height / 4) *(10 - Math.Abs(currFlag - i)) / 10),color, SpriteEffects.None);
                }
            }
            celAnimationManager.Draw(gameTime, flags[currFlag], OurGame.spriteBatch, new Rectangle((Game1.screen_width/16)*7, (Game1.screen_height/8)*3, Game1.screen_width / 8, Game1.screen_height / 4), SpriteEffects.None);
          
       //     OurGame.spriteBatch.Draw(main, new Rectangle(/*Game1.screen_width / 4*/0, Game1.screen_height / 4, Game1.screen_width , Game1.screen_height / 2),Color.AliceBlue);
            left.Draw(gameTime, OurGame.spriteBatch);
            right.Draw(gameTime, OurGame.spriteBatch);
            select.Draw(gameTime, OurGame.spriteBatch);
        }
    }
}
