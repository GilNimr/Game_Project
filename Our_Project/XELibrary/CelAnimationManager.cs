using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XELibrary
{
    public class CelAnimationManager : Microsoft.Xna.Framework.GameComponent, ICelAnimationManager
    {
        private Dictionary<string, CelAnimation> animations = new Dictionary<string, CelAnimation>();
        private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        private string contentPath;

        public CelAnimationManager(Game game, string contentPath)
            : base(game)
        {
            this.contentPath = contentPath;
            if (this.contentPath.LastIndexOf("\\") < this.contentPath.Length - 1)
                this.contentPath += "\\";

            game.Services.AddService(typeof(ICelAnimationManager), this);
        }

        public int GetAnimationFrameWidth(string animationKey)
        {
            if (!animations.ContainsKey(animationKey))
                return -1;
            CelAnimation ca = animations[animationKey];
            return ca.CelWidth;
        }

        public int GetAnimationFrameHeight(string animationKey)
        {
            if (!animations.ContainsKey(animationKey))
                return -1;
            CelAnimation ca = animations[animationKey];
            return ca.CelHeight;
        }

        public void AddAnimation(string animationKey, string textureName, CelCount celCount, int framesPerSecond)
        {
            if (!textures.ContainsKey(textureName))
            {
                textures.Add(textureName, Game.Content.Load<Texture2D>(
                contentPath + textureName));
            }
            int celWidth = (int)(textures[textureName].Width / celCount.NumberOfColumns);
            int celHeight = (int)(textures[textureName].Height / celCount.NumberOfRows);
            int numberOfCels = celCount.NumberOfColumns * celCount.NumberOfRows;
            
            AddAnimation(animationKey, textureName, new CelRange(1, 1, celCount.NumberOfColumns, celCount.NumberOfRows), celWidth, celHeight, numberOfCels, framesPerSecond);
        }

        public void AddAnimation(string animationKey, string textureName, CelRange celRange, int celWidth, int celHeight, int numberOfCels, int framesPerSecond)
        {
            CelAnimation ca = new CelAnimation(textureName, celRange,
            framesPerSecond);
            if (!textures.ContainsKey(textureName))
            {
                textures.Add(textureName, Game.Content.Load<Texture2D>(contentPath + textureName));
            }
            ca.CelWidth = celWidth;
            ca.CelHeight = celHeight;
            ca.NumberOfCels = numberOfCels;
            ca.CelsPerRow = textures[textureName].Width / celWidth;
            if (animations.ContainsKey(animationKey))
                animations[animationKey] = ca;
            else
                animations.Add(animationKey, ca);
        }

        public void PauseAnimation(string animationKey)
        {
            if (animations.ContainsKey(animationKey))
                animations[animationKey].Paused = true;
        }

        public void ResumeAnimation(string animationKey)
        {
            if (animations.ContainsKey(animationKey))
                animations[animationKey].Paused = false;
        }

        public void ToggleAnimation(string animationKey)
        {
            if (animations.ContainsKey(animationKey))
                animations[animationKey].Paused = !animations[animationKey].Paused;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (KeyValuePair<string, CelAnimation> animation in animations)
            {
                CelAnimation ca = animation.Value;
                if (ca.Paused)
                    continue; //no need to update this animation,check next one
                ca.TotalElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (ca.TotalElapsedTime > ca.TimePerFrame)
                {
                    ca.Frame++;
                    
                    //min: 0, max: total cels
                    ca.Frame = ca.Frame % (ca.NumberOfCels);
                    
                    //reset our timer
                    ca.TotalElapsedTime -= ca.TimePerFrame;
                }
            }
            base.Update(gameTime);
        }
        
        public void Draw(GameTime gameTime, string animationKey,
        SpriteBatch batch, Vector2 position, SpriteEffects effects)
        {
            Draw(gameTime, animationKey, batch,
            position, Color.White, effects);
        }
        
        public void Draw(GameTime gameTime, string animationKey,
        SpriteBatch batch, Vector2 position, Color color, SpriteEffects effects)
        {
            if (!animations.ContainsKey(animationKey))
                return;
            CelAnimation ca = animations[animationKey];
            //first get our x increase amount
            //(add our offset-1 to our current frame)
            int xincrease = (ca.Frame + ca.CelRange.FirstCelX - 1);
            
            //now we need to wrap the value so it will loop to the next row
            int xwrapped = xincrease % ca.CelsPerRow;
            
            //finally we need to take the product of our wrapped value
            //and a cel’s width
            int x = xwrapped * ca.CelWidth;
            
            //to determine how much we should increase y, we need to look
            //at how much we increased x and do an integer divide
            int yincrease = xincrease / ca.CelsPerRow;
            
            //now we can take this increase and add it to
            //our Y offset-1 and multiply the sum by our cel height
            int y = (yincrease + ca.CelRange.FirstCelY - 1) * ca.CelHeight;
            Rectangle cel = new Rectangle(x, y, ca.CelWidth, ca.CelHeight);
            batch.Draw(textures[ca.TextureName], position, cel, color, 0.0f, Vector2.Zero, 1.0f, effects, 0.0f);
        }
    }
}
