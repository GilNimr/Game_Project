using System;
using Microsoft.Xna.Framework;

namespace XELibrary
{
    public abstract partial class GameState : Microsoft.Xna.Framework.DrawableGameComponent, IGameState
    {
        protected IGameStateManager StateManager;
        protected IInputHandler Input;

        #region IGameState Members
        
        public GameState Value
        {
            get { return (this); }
        }
        
        #endregion

        public GameState(Game game)
            : base(game)
        {
            StateManager = (IGameStateManager)game.Services.GetService(typeof(
                IGameStateManager));
            Input = (IInputHandler)game.Services.GetService(typeof(
                IInputHandler));

        }

        internal protected virtual void StateChanged(object sender, EventArgs e)
        {
            if (StateManager.State == this.Value)
                Visible = Enabled = true;
            else
                Visible = Enabled = false;
        }
    }
}
