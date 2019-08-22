
/* Gil Nevo 310021654
 * Shachar Bartal 305262016
 */

using System;
using Microsoft.Xna.Framework;

using XELibrary;

namespace Our_Project
{
    public interface ITitleIntroState : IGameState { }
    public interface IStartMenuState : IGameState { }
    public interface IChooseFlagState : IGameState { }
    public interface IPlayingState : IGameState { }
    public interface IPlaying_Server_State : IGameState { }
    public interface IPlaying_Client_State : IGameState { }
    public interface IPausedState : IGameState { }
    public interface IOptionsMenuState : IGameState { }
    public interface IBuildingBoardState : IGameState { }
    public interface IPlacingSoldiersState : IGameState { }
    public interface ILevelEditorState : IGameState { }
    public interface IShapeEditorState : IGameState { }
    public interface IBoardEditorState : IGameState { }
}
