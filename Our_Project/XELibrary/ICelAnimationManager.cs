using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XELibrary
{
    public interface ICelAnimationManager
    {
        /// <summary>
        /// Get the width in pixels of the current frame of animation
        /// </summary>
        /// <param name="animationKey">The key by which the animation is stored</param>
        /// <returns></returns>
        int  GetAnimationFrameWidth(string animationKey);

        /// <summary>
        /// Get the height in pixels of the current frame of animation
        /// </summary>
        /// <param name="animationKey">The key by which the animation is stored</param>
        /// <returns></returns>
        int GetAnimationFrameHeight(string animationKey);

        /// <summary>
        /// Create an animation sequence from a sprite sheet.
        /// </summary>
        /// <param name="animationKey">The key by which the animation is stored and later retrieved</param>
        /// <param name="textureName">Sprite sheet texture name</param>
        /// <param name="celCount">Grid-based representation of the number of cels in the sprite sheet</param>
        /// <param name="framesPerSecond">The speed of the animation</param>
        void AddAnimation(string animationKey, string textureName, CelCount celCount, int framesPerSecond);

        /// <summary>
        /// Create an animation sequence from a sprite sheet.
        /// </summary>
        /// <param name="animationKey">The key by which the animation is stored and later retrieved</param>
        /// <param name="textureName">Sprite sheet texture name</param>
        /// <param name="celRange">A pixel range-based representation of the cels in the sprite sheet</param>
        /// <param name="celWidth">The width of each cell</param>
        /// <param name="celHeight">The height of each cell</param>
        /// <param name="numberOfCels">Number of cels in the animation</param>
        /// <param name="framesPerSecond">The speed of the animation</param>
        void AddAnimation(string animationKey, string textureName, CelRange celRange, int celWidth, int celHeight, int numberOfCels, int framesPerSecond);

        /// <summary>
        /// Pauses the animation
        /// </summary>
        /// <param name="animationKey">The key by which the animation is stored</param>
        void PauseAnimation(string animationKey);

        /// <summary>
        /// Resumes playback of the animation
        /// </summary>
        /// <param name="animationKey">The key by which the animation is stored</param>
        void ResumeAnimation(string animationKey);

        /// <summary>
        /// Switches between paused and play depending on the current state of the animation
        /// </summary>
        /// <param name="animationKey">The key by which the animation is stored</param>
        void ToggleAnimation(string animationKey);

        /// <summary>
        /// Draws the animation while progressing the frames according to the speed of the animation.
        /// </summary>
        /// <param name="animationKey">The key by which the animation is stored</param>
        void Draw(GameTime gameTime, string animationKey, SpriteBatch batch, Vector2 position, SpriteEffects effects);

        /// <summary>
        /// Draws the animation while progressing the frames according to the speed of the animation.
        /// </summary>
        /// <param name="animationKey">The key by which the animation is stored</param>
        /// <param name="color">An added tint to the frame</param>
        void Draw(GameTime gameTime, string animationKey, SpriteBatch batch, Vector2 position, Color color, SpriteEffects effects);
    }
}
