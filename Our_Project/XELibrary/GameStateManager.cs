using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace XELibrary
{
    public class GameStateManager : Microsoft.Xna.Framework.GameComponent, IGameStateManager
    {
        private Stack<GameState> states = new Stack<GameState>();

        public event EventHandler OnStateChange;

        private readonly int initialDrawOrder = 1000;
        private int drawOrder;

        public GameStateManager(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IGameStateManager), this);
            drawOrder = initialDrawOrder;
        }

        #region IGameStateManager Members

        public void PopState()
        {
            RemoveState();
            drawOrder -= 100;

            // Let everyone know we changed states
            if (OnStateChange != null)
                OnStateChange(this, null);
        }

        public void PushState(GameState newState)
        {
            drawOrder += 100;
            newState.DrawOrder = drawOrder;

            AddState(newState);

            // Let everyone know we changed states
            if (OnStateChange != null)
                OnStateChange(this, null);
        }

        private void RemoveState()
        {
            GameState oldState = (GameState)states.Peek();

            // Unregister the event for this state
            OnStateChange -= oldState.StateChanged;

            // Remove the state from our game components
            Game.Components.Remove(oldState.Value);

            states.Pop();
        }
        
        private void AddState(GameState state)
        {
            states.Push(state);

            Game.Components.Add(state);

            // Register the event for this state
            OnStateChange += state.StateChanged;
        }

        public bool ContainsState(GameState state)
        {
            return (states.Contains(state));
        }

        /// <summary>
        /// Changing states, pops everything.
        /// If you do not wish to really change states but just modify, you
        /// should call PushState and PopState
        /// </summary>
        /// <param name="newState">The new state</param>
        public void ChangeState(GameState newState)
        {
            while (states.Count > 0)
                RemoveState();

            // Changing state, reset our draw order
            newState.DrawOrder = drawOrder = initialDrawOrder;
            AddState(newState);

            // Let everyone know we changed states
            if (OnStateChange != null)
                OnStateChange(this, null);
        }

        public GameState State
        {
            get { return (states.Peek()); }
        }

        #endregion
    }
}
