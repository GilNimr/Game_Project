using Microsoft.Xna.Framework.Input;

namespace XELibrary
{
    public class KeyboardHandler
    {
        private KeyboardState prevKeyboardState;
        private KeyboardState keyboardState;

        public KeyboardHandler()
        {
            prevKeyboardState = Keyboard.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return (keyboardState.IsKeyDown(key));
        }

        public bool IsKeyUp(Keys key)
        {
            return (keyboardState.IsKeyUp(key));
        }

        public bool IsHoldingKey(Keys key)
        {
            return (keyboardState.IsKeyDown(key) && prevKeyboardState.IsKeyDown(key));
        }

        public bool WasKeyPressed(Keys key)
        {
            return (keyboardState.IsKeyUp(key) && prevKeyboardState.IsKeyDown(key));
        }

        public void Update()
        {
            // set our previous keyboard state
            prevKeyboardState = keyboardState;

            // get our new keyboard state
            keyboardState = Keyboard.GetState();
        }
    }
}
