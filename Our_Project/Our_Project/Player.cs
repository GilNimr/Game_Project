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
        internal bool myTurn=false;

        public Player()
        {
            army_size = 4;
        }
    }
}
