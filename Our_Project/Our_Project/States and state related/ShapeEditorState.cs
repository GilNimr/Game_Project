using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XELibrary;

namespace Our_Project.States_and_state_related
{
    public sealed class ShapeEditorState : BaseGameState, IShapeEditorState
    {
        private ISoundManager soundEffect;


        public ShapeEditorState(Game game) : base(game)
        {
            game.Services.AddService(typeof(IShapeEditorState), this);

            soundEffect = (ISoundManager)game.Services.GetService(typeof(ISoundManager));

        }


    }
}
