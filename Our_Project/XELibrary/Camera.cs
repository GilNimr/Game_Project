using Microsoft.Xna.Framework;

namespace XELibrary
{
    /// <summary>
    /// Camera abstract class
    /// </summary>
    public abstract class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public Matrix Projection      { get; private set; }
        public Matrix View            { get; private set; }
        public Vector3 CameraPosition { get; set; } = Vector3.UnitZ;
        public Vector3 CameraTarget   { get; set; } = Vector3.Zero;
        public Vector3 CameraUp       { get; }      = Vector3.Up;
        public float   NearZ          { get; }      = 1.0f;
        public float   FarZ           { get; }      = 1000.0f;

        public Camera(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            Projection = InitializeProjection();

            base.Initialize();
        }

        protected abstract Matrix InitializeProjection();

        public override void Update(GameTime gameTime)
        {
            View = Matrix.CreateLookAt(CameraPosition, CameraTarget, CameraUp);

            base.Update(gameTime);
        }
    }
}
