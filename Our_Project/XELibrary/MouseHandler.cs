using Microsoft.Xna.Framework.Input;

namespace XELibrary
{
    public class MouseHandler
    {
        private MouseState prevMouseState;
        public MouseState PrevMouseState { get { return prevMouseState;} }
        private MouseState mouseState;
        public MouseState MouseState { get { return mouseState; } }

        public MouseHandler()
        {
            prevMouseState = Mouse.GetState();
        }

        public bool IsLeftButtonDown()
        {
            return (MouseState.LeftButton == ButtonState.Pressed);
        }

        public bool IsRightButtonDown()
        {
            return (MouseState.RightButton == ButtonState.Pressed);
        }

        public bool IsMiddleButtonDown()
        {
            return (MouseState.MiddleButton == ButtonState.Pressed);
        }

        public bool IsLeftButtonUp()
        {
            return (MouseState.LeftButton == ButtonState.Released);
        }

        public bool IsRightButtonUp()
        {
            return (MouseState.RightButton == ButtonState.Released);
        }

        public bool IsMiddleButtonUp()
        {
            return (MouseState.MiddleButton == ButtonState.Released);
        }

        public bool IsHoldingLeftButton()
        {
            return (MouseState.LeftButton == ButtonState.Pressed &&
                    prevMouseState.LeftButton == ButtonState.Pressed);
        }

        public bool IsHoldingRightButton()
        {
            return (MouseState.RightButton == ButtonState.Pressed &&
                    prevMouseState.RightButton == ButtonState.Pressed);
        }

        public bool IsHoldingMiddleButton()
        {
            return (MouseState.MiddleButton == ButtonState.Pressed &&
                    prevMouseState.MiddleButton == ButtonState.Pressed);
        }

        public bool WasLeftButtonClicked()
        {
            return (MouseState.LeftButton == ButtonState.Released &&
                    prevMouseState.LeftButton == ButtonState.Pressed);
        }

        public bool WasRightButtonClicked()
        {
            return (MouseState.RightButton == ButtonState.Released &&
                    prevMouseState.RightButton == ButtonState.Pressed);
        }

        public bool WasMiddleButtonClicked()
        {
            return (MouseState.MiddleButton == ButtonState.Released &&
                    prevMouseState.MiddleButton == ButtonState.Pressed);
        }

        public void Update()
        {
            // set our previous Mouse state
            prevMouseState = MouseState;

            // get our new Mouse state
            mouseState = Mouse.GetState();
        }
    }
}
