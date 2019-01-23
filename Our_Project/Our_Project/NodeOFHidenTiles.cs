using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our_Project
{
    public class NodeOFHidenTiles
    {
        int i, j;

        public NodeOFHidenTiles(int _i, int _j)
        {
            i = _i;
            j = _j;
        }

        public int getI()
        {
            return i;
        }

        public int getJ()
        {
            return j;
        }
    }
}
