
/* Gil Nevo 310021654
 * Shachar Bartal 305262016
 */

 /*
  *  this class will be part of list, each node (=this) will show an i & j index of tile at Board
  *  that we want to define as shape, that's indexes will be hiden at this shape
  */
  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGame.Shared1
{
    public class NodeOFHidenTiles
    {
        private readonly int i, j;
        
        public NodeOFHidenTiles(int _i, int _j)
        {
            i = _i;
            j = _j;
        }
        
        public int GetI()
        {
            return i;
        }

        public int GetJ()
        {
            return j;
        }
    }
}
