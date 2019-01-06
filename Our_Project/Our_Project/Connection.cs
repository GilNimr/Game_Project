using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our_Project
{
    public class Connection
    {
        NetClient client;
        Player player;
        Player enemy;
        private NetConnection server;

        public static NetOutgoingMessage outmsg;

        public Connection(Player _player, Player _enemy)
        {
            enemy = _enemy;
            player = _player;
            NetPeerConfiguration config = new NetPeerConfiguration("Flags");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

            client = new NetClient(config);
            client.Start();

            
            client.DiscoverKnownPeer("127.0.0.1", 14242);

           // client.DiscoverLocalPeers(14242);
        }

        public void update()
        {
            if (player.pawns != null && player.myTurn)
            {
                NetOutgoingMessage om = client.CreateMessage();
                for (int i = 0; i < player.pawns.Length; i++)
                {
                    if (player.pawns[i].moved)
                    {
                        om.Write(player.pawns[i].current_tile.id);
                        om.Write(i);
                         // very inefficient to send a full Int32 (4 bytes) but we'll use this for simplicity
                        
                        client.SendMessage(om,NetDeliveryMethod.ReliableOrdered);
                        player.pawns[i].moved = false;
                    }

                }
              //  player.myTurn = false;
            }
            NetIncomingMessage msg;
            while ((msg = client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:
                        // just connect to first server discovered
                      server=  client.Connect(msg.SenderEndPoint);
                        int num_of_players= msg.ReadInt32();
                        if (num_of_players == 1)
                            PlayingState.i_am_second_player = true;

                        
                        break;
                    case NetIncomingMessageType.Data:
                        // server sent a position update
                        int id = msg.ReadInt32();
                        int i= msg.ReadInt32();
                       
                        enemy.pawns[i].current_tile = PlayingState.tileDictionary[id];

                        player.myTurn = true;
                        break;
                }
            }
        }
    }
}
