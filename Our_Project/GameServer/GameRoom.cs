
/* Gil Nevo 310021654
 * Shachar Bartal 305262016
 */

using System;
using System.Collections.Generic;
using System.Threading;
using Lidgren.Network;


namespace GameServer
{
    class GameRoom
    {
        private NetConnection firstPlayer;
        private NetConnection secondPlayer;
        public List<NetConnection> players = new List<NetConnection>();

        public GameRoom()
        {

        }

        public void SetFirstPlayer(NetConnection first)
        {
            firstPlayer = first;
            players.Add(firstPlayer);
        }
        public void SetSecondPlayer(NetConnection second)
        {
            secondPlayer = second;
            players.Add(secondPlayer);
        }

        public NetConnection GetFirstPlayer()
        {
            if (firstPlayer == null)
                throw new Exception("null value");

            return firstPlayer;

        }

        public NetConnection GetSecondPlayer()
        {
            if (secondPlayer == null)
                throw new Exception("null value");

            return secondPlayer;

        }
    }
}
