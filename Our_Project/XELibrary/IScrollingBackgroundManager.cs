using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XELibrary
{
    public interface IScrollingBackgroundManager 
    {
        /// <summary>
        /// Add a background to the manager. Backgrounds can be referenced by their key.
        /// </summary>
        /// <param name="backgroundKey">Unique name for the background for later referencing</param>
        /// <param name="textureName">The name of the content texture for the background</param>
        /// <param name="position">The screen location of the top-left point of the background</param>
        /// <param name="scrollRateRatio">The scroll rate of the new background in relation to the global scroll rate</param>
        /// <param name="layerDepth">Depth of the background. Used to determine the order in which backgrounds appear</param>
        void AddBackground(string backgroundKey, string textureName, Vector2 position, float scrollRateRatio, float layerDepth);

        /// <summary>
        /// Add a background to the manager. Backgrounds can be referenced by their key.
        /// </summary>
        /// <param name="backgroundKey">Unique name for the background for later referencing</param>
        /// <param name="textureName">The name of the content texture for the background</param>
        /// <param name="sourceRect">A region in the texture to be used as background</param>
        /// <param name="position">The screen location of the top-left point of the background</param>
        /// <param name="scrollRateRatio">The scroll rate of the new background in relation to the global scroll rate</param>
        /// <param name="layerDepth">Depth of the background. Used to determine the order in which backgrounds appear</param>
        /// <param name="color">An added tint to the background</param>
        void AddBackground(string backgroundKey, string textureName, Vector2 position, Rectangle sourceRect, float scrollRateRatio, float layerDepth, Color color);

        /// <summary>
        /// Draws the background by key
        /// </summary>
        /// <param name="backgroundKey">The unique name of the background to be drawn</param>
        /// <param name="batch">Sprite batch used for drawing</param>
        void Draw(string backgroundKey, SpriteBatch batch);

        /// <summary>
        /// The current scroll rate for all backgrounds.
        /// Each background has its own scroll rate ratio to determine how fast or slow it is compared to other background scrolls.
        /// This is how we obtain the parallax effect.
        /// </summary>
        float ScrollRate { get; set; }
    }
}
