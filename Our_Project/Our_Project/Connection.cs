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

        public Connection(ref Player _player,ref Player _enemy)
        {
            enemy = _enemy;
            player = _player;
            NetPeerConfiguration config = new NetPeerConfiguration("Flags");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

            client = new NetClient(config);
            client.Start();


           // client.DiscoverKnownPeer("172.27.38.8", 14242); //sapir
            client.DiscoverKnownPeer("192.168.1.11", 14242); //home



            // client.DiscoverLocalPeers(14242);
        }

        public void update()
        {
            if (player.pawns != null)
            {
                NetOutgoingMessage om = client.CreateMessage();
                for (int i = 0; i < player.pawns.Length; i++)
                {
                    if (player.pawns[i].send_update)
                    {
                        om.Write("move");
                        om.Write(player.pawns[i].current_tile.id);
                        om.Write(i);
                         
                        
                        client.SendMessage(om,NetDeliveryMethod.ReliableOrdered);
                        player.pawns[i].send_update = false;
                        player.myTurn = false;
 
                    }

                }
            }
            if(enemy.pawns!=null)
            {
                NetOutgoingMessage om = client.CreateMessage();
                for (int i = 0; i < enemy.pawns.Length; i++)
                {
                    if (enemy.pawns[i].attacked)
                    {
                        om.Write("attacked");
                        om.Write(enemy.pawns[i].attacker.id);
                        om.Write(i);


                        client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
                        enemy.pawns[i].attacked = false;
                    }

                }

            }
            NetIncomingMessage msg;
            while ((msg = client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:

                        server = client.Connect(msg.SenderEndPoint);
                        int num_of_players = msg.ReadInt32();
                        if (num_of_players == 1)
                            PlayingState.i_am_second_player = true;

                        break;

                    case NetIncomingMessageType.Data:

                        string data_string = msg.ReadString();

                        switch (data_string)
                        {
                            case "move":
                                {
                                    
                                    int id = msg.ReadInt32();
                                    int i = msg.ReadInt32();

                                    enemy.pawns[i].current_tile.occupied = Tile.Occupied.no;
                                    enemy.pawns[i].current_tile = PlayingState.tileDictionary[id];
                                    PlayingState.tileDictionary[id].occupied = Tile.Occupied.yes_by_enemy;
                                    PlayingState.tileDictionary[id].current_pawn = enemy.pawns[i];
                                    enemy.pawns[i].team = Pawn.Team.enemy_team;
                                    player.myTurn = true;

                                   
                                    break;
                                }
                            case "attacked":
                                {
                                    int id = msg.ReadInt32();
                                    int i = msg.ReadInt32();

                                    player.pawns[i].attacked = true;
                                    player.pawns[i].attacker = enemy.pawns[id];

                                    break;
                                }
                        }
                        break;
                }
            }
        }
    }
}
