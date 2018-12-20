using Microsoft.Xna.Framework;

namespace XELibrary
{
    public class InputHandler 
        : Microsoft.Xna.Framework.GameComponent, IInputHandler
    {
        private KeyboardHandler keyboard;

#if !XBOX360
        private MouseHandler mouse;
#endif

        public InputHandler(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IInputHandler), this);
            keyboard = new KeyboardHandler();

#if !XBOX360
            mouse = new MouseHandler();
            Game.IsMouseVisible = true;
#endif
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            keyboard.Update();
            mouse.Update();

            base.Update(gameTime);
        }

        #region IInputHandler Members

        public KeyboardHandler KeyboardHandler
        {
            get { return (keyboard); }
        }

        public MouseHandler MouseHandler
        {
            get { return (mouse); }
        }

        #endregion
    }
}
