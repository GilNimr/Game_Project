using System;

namespace XELibrary
{
    /// <summary>
    /// Manages game states for the entire game.
    /// States must be implemented by the game, and added to this manager.
    /// </summary>
    public interface IGameStateManager
    {
        /// <summary>
        /// An event that will fire on any change to the state, 
        /// including Push, Pop and Change.
        /// </summary>
        event EventHandler OnStateChange;

        /// <summary>
        /// Get the current state
        /// </summary>
        GameState State { get; }

        void PopState();
        void PushState(GameState newState);
        bool ContainsState(GameState state);
        void ChangeState(GameState newState);
    }
}
