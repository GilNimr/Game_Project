
/* Gil Nevo 310021654
 * Shachar Bartal 305262016
 */

using Our_Project.States_and_state_related;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our_Project
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

        public Player(Game game)
        {
            buildingBoardState = (BuildingBoardState)game.Services.GetService(typeof(IBuildingBoardState));
            placingSoldiersState = (PlacingSoldiersState)game.Services.GetService(typeof(IPlacingSoldiersState));

            army_size = 21;
        }
    }
}
