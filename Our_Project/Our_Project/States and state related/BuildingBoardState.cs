using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our_Project.States_and_state_related
{
    public sealed class BuildingBoardState : BaseGameState, IBuildingBoardState
    {
        public BuildingBoardState(Game game) : base(game)
        {
            game.Services.AddService(typeof(IBuildingBoardState), this);
        }

    }
}
