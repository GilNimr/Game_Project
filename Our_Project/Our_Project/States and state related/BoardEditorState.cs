using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XELibrary;

namespace Our_Project.States_and_state_related
{
    public sealed class BoardEditorState : BaseGameState , IBoardEditorState
    {
        // fullTile - tile texture. emptyTile - free tile for put a shape
        private Texture2D fullTile2d, fullTileIso, emptyTile2d, emptyTileIso;
        private ISoundManager soundEffect;
        private Board bigEmptyBoard;    // the big board we build our area on it
        private bool isPlayBadPlaceSoundEffect;     // boolean for checking if turn on the bad place sound


        public BoardEditorState(Game game) : base(game)
        {
            game.Services.AddService(typeof(IBoardEditorState), this);
            soundEffect = (ISoundManager)game.Services.GetService(typeof(ISoundManager));
        }

    }
}
