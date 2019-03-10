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

        public void setFirstPlayer(NetConnection first)
        {
            firstPlayer = first;
            players.Add(firstPlayer);
        }
        public void setSecondPlayer(NetConnection second)
        {
            secondPlayer = second;
            players.Add(secondPlayer);
        }

        public NetConnection getFirstPlayer()
        {
            if (firstPlayer == null)
                throw new Exception("null value");

            return firstPlayer;

        }

        public NetConnection getSecondPlayer()
        {
            if (secondPlayer == null)
                throw new Exception("null value");

            return secondPlayer;

        }
    }
}
