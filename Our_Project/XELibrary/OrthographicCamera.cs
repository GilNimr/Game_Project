using Microsoft.Xna.Framework;

namespace XELibrary
{
    /// <summary>
    /// Simple orthographic camera. You can use this camera to create simple 2D scenes.
    /// </summary>
    public class OrthographicCamera : Camera
    {
        public OrthographicCamera(Game game)
            : base(game)
        {
        }

        protected override Matrix InitializeProjection()
        {
            //Projection
            float aspectRatio =
                (float)Game.GraphicsDevice.Viewport.Width /
                (float)Game.GraphicsDevice.Viewport.Height;

            return Matrix.CreateOrthographic(2, aspectRatio * 2, NearZ, FarZ);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public Vector2 ConvertScreenToWorld(Vector2 location)
        {
            Vector3 unprojectedVec = new Vector3(location, 0.0f);

            unprojectedVec = Game.GraphicsDevice.Viewport.Unproject(unprojectedVec, Projection, View, Matrix.Identity);

            return new Vector2(unprojectedVec.X, unprojectedVec.Y);
        }
    }
}
