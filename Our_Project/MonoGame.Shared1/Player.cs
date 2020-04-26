
/* Gil Nevo 310021654
 * Shachar Bartal 305262016
 */

using MonoGame.Shared1.States_and_state_related;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame.Shared1
{
    public class Player
    {
        public int army_size;
        public Pawn[] pawns;
        public bool myTurn = false;
        public string flag;

        public Board Board

        {
            get
            {

                return buildingBoardState.GetEmptyBoard();

            }
        }
     
        public BuildingBoardState buildingBoardState;
        public PlacingSoldiersState placingSoldiersState;
        public ChooseFlagState chooseFlagState;
        public ParticleService particleService;

        public Player(Game game)
        {
            chooseFlagState = (ChooseFlagState)game.Services.GetService(typeof(IChooseFlagState));
            buildingBoardState = (BuildingBoardState)game.Services.GetService(typeof(IBuildingBoardState));
            placingSoldiersState = (PlacingSoldiersState)game.Services.GetService(typeof(IPlacingSoldiersState));
            particleService = (ParticleService)game.Services.GetService(typeof(ParticleService));

            army_size = 21;
        }
    }
}
