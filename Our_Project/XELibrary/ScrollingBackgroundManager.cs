using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XELibrary
{

    public class ScrollingBackgroundManager : Microsoft.Xna.Framework.GameComponent, IScrollingBackgroundManager
    {
        private Dictionary<string, ScrollingBackground> backgrounds = new Dictionary<string, ScrollingBackground>();
        private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        private string contentPath;
        private int screenWidth;
        private int screenHeight;
        private float scrollRate;
        
        public ScrollingBackgroundManager(Game game, string contentPath)
            : base(game)
        {
            this.contentPath = contentPath;
            if (this.contentPath.LastIndexOf('\\') < this.contentPath.Length - 1)
                this.contentPath += "\\";
            game.Services.AddService(
            typeof(IScrollingBackgroundManager), this);
        }
        
        public override void Initialize()
        {
            base.Initialize();
            GraphicsDeviceManager graphics = (GraphicsDeviceManager)Game.Services.GetService(typeof(IGraphicsDeviceManager));
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;
        }
        
        public void AddBackground(string backgroundKey, string textureName,
        Vector2 position, float scrollRateRatio, float layerDepth = 0.0f)
        {
            AddBackground(backgroundKey, textureName, position, Rectangle.Empty, scrollRateRatio, layerDepth, Color.White);
        }
        
        public void AddBackground(string backgroundKey, string textureName,
        Vector2 position, Rectangle sourceRect, float scrollRateRatio, float layerDepth,
        Color color)
        {
            ScrollingBackground background = new ScrollingBackground(textureName, position, sourceRect, scrollRateRatio, layerDepth, color);
            background.ScrollRate = scrollRate * scrollRateRatio;
            if (!textures.ContainsKey(textureName))
            {
                textures.Add(textureName, Game.Content.Load<Texture2D>(contentPath + textureName));
            }
            if (backgrounds.ContainsKey(backgroundKey))
                backgrounds[backgroundKey] = background;
            else
                backgrounds.Add(backgroundKey, background);
        }
        
        public override void Update(GameTime gameTime)
        {
            foreach (KeyValuePair<string, ScrollingBackground> background in backgrounds)
            {
                ScrollingBackground sb = background.Value;
                sb.Position.X += (sb.ScrollRate * (float)gameTime.ElapsedGameTime.TotalSeconds);
                sb.Position.X = sb.Position.X % textures[sb.TextureName].Width;
            }
            base.Update(gameTime);
        }

        public void Draw(string backgroundKey,
        SpriteBatch batch)
        {
            ScrollingBackground sb = backgrounds[backgroundKey];
            Texture2D texture = textures[sb.TextureName];
            
            //Draw the main texture
            batch.Draw(texture, sb.Position, sb.SourceRect, sb.Color, 0, Vector2.Zero, 1.0f, SpriteEffects.None, sb.LayerDepth);

            // Repeat as necessary
            int offsetFromZero = (int)(sb.Position.X);

            // To the right
            int repeatRight = Game.GraphicsDevice.Viewport.Width / (texture.Width + offsetFromZero);
            for (int i = 1; i <= repeatRight; i++)
            {
                Vector2 offsetPos = sb.Position;
                offsetPos.X = offsetPos.X + (texture.Width * i);
                batch.Draw(texture, offsetPos, sb.SourceRect, sb.Color, 0, Vector2.Zero, 1.0f, SpriteEffects.None, sb.LayerDepth);
            }

            // To the left
            if (offsetFromZero > 0)
            {
                int repeatLeft = offsetFromZero / (texture.Width) + 1;
                for (int i = 1; i <= repeatLeft; i++)
                {
                    Vector2 offsetPos = sb.Position;
                    offsetPos.X = offsetPos.X - (texture.Width * i);
                    batch.Draw(texture, offsetPos, sb.SourceRect, sb.Color, 0, Vector2.Zero, 1.0f, SpriteEffects.None, sb.LayerDepth);
                }
            }

        }

        public float ScrollRate
        {
            get { return (scrollRate); }
            set
            {
                scrollRate = value;
                foreach (ScrollingBackground sb in backgrounds.Values)
                    sb.ScrollRate = scrollRate * sb.ScrollRateRatio;
            }
        }
    }
}
