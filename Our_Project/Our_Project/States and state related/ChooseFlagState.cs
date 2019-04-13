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

        private string[] flags;
        private int currFlag;
       
        private Button left;
        private Button right;
        ICelAnimationManager celAnimationManager;

        public ChooseFlagState(Game game) : base(game)
        {
            celAnimationManager = (ICelAnimationManager)game.Services.GetService(typeof(ICelAnimationManager));
        }
        protected override void LoadContent()
        {
            base.LoadContent();
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
            OurGame.Components.Add(left);
            OurGame.Components.Add(right);
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
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            for (int i = 0; i < flags.Length; i++)
            {
                if(i==currFlag)
                celAnimationManager.Draw(gameTime, flags[i], OurGame.spriteBatch, new Rectangle((Game1.screen_width / 16) * 7, (Game1.screen_height / 8) * 3, Game1.screen_width / 8, Game1.screen_height / 4), SpriteEffects.None);
                else
                {
                    celAnimationManager.Draw(gameTime, flags[i], OurGame.spriteBatch, new Rectangle((Game1.screen_width / 16) * 7 + (currFlag - i) * (Game1.screen_width / 8)/*x*/, (Game1.screen_height / 8) * 3 - ((int)  Math.Pow(Math.Abs( currFlag - i),2)*(Game1.screen_height/50)) /*y*/, (Game1.screen_width / 8)*(10 - Math.Abs(currFlag - i))/10 /*width*/, (Game1.screen_height / 4) *(10 - Math.Abs(currFlag - i)) / 10), SpriteEffects.None);

                }
            }
            celAnimationManager.Draw(gameTime, flags[currFlag], OurGame.spriteBatch, new Rectangle((Game1.screen_width/16)*7, (Game1.screen_height/8)*3, Game1.screen_width / 8, Game1.screen_height / 4), SpriteEffects.None);
          
       //     OurGame.spriteBatch.Draw(main, new Rectangle(/*Game1.screen_width / 4*/0, Game1.screen_height / 4, Game1.screen_width , Game1.screen_height / 2),Color.AliceBlue);
            left.Draw(gameTime, OurGame.spriteBatch);
            right.Draw(gameTime, OurGame.spriteBatch);
        }
    }
}
