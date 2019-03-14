using Lidgren.Network;
using Microsoft.Xna.Framework;
using Our_Project.States_and_state_related;
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

        Game game;
        Player player;
        Player enemy;
        private NetConnection server;

        public static NetOutgoingMessage outmsg;

        public Connection(Game game,ref Player _player,ref Player _enemy)
        {
            this.game = game;
            enemy = _enemy;
            player = _player;
            NetPeerConfiguration config = new NetPeerConfiguration("Flags");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

            client = new NetClient(config);
            client.Start();


            // client.DiscoverKnownPeer(" 172.27.38.20", 14242) ; //sapir
           // client.DiscoverKnownPeer("62.219.99.130", 14242); //sapir external
            client.DiscoverLocalPeers(14242);
            
        //    client.DiscoverKnownPeer("192.168.1.11", 14242); //home
          //  client.DiscoverKnownPeer("192.168.43.199", 14242); //home amit



            // client.DiscoverLocalPeers(14242);
        }

        public void update()
        {
            
            for (int i = 0; i < player.Board.getHeight()* player.Board.getWidth(); i++)
            {
                if (player.Board.boardDictionaryById[i].sendUpdate)
                {
                    NetOutgoingMessage om = client.CreateMessage();
                    om.Write("tile_added");
                    om.Write(i);
                    client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
                    player.Board.boardDictionaryById[i].sendUpdate = false;


                }
            }

            if (player.pawns != null)
            {
                NetOutgoingMessage om = client.CreateMessage();
                for (int i = 0; i < player.pawns.Length; i++)
                {
                    if (player.pawns[i] != null)
                    { 
                       if (player.pawns[i].send_update)
                       {
                        om.Write("move");
                        om.Write(player.pawns[i].current_tile.getId());
                        om.Write(i);
                         
                        
                        client.SendMessage(om,NetDeliveryMethod.ReliableOrdered);
                        player.pawns[i].send_update = false;
                        player.myTurn = false;
 
                       }
                    }
                }
            }

            if(enemy.pawns!=null)
            {
                NetOutgoingMessage om = client.CreateMessage();
                for (int i = 0; i < enemy.pawns.Length; i++)
                {
                    if (enemy.pawns[i] != null)
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
                            BuildingBoardState.i_am_second_player = true;

                        break;

                    case NetIncomingMessageType.Data:

                        string data_string = msg.ReadString();

                        switch (data_string)
                        {
                            case "tile_added":
                                {
                                    int tile_id = msg.ReadInt32();
                                    player.Board.boardDictionaryById[tile_id].texture = player.Board.boardDictionaryById[299].texture;
                                    player.Board.boardDictionaryById[tile_id].setIsHidden(false);

                                    break;
                                }
                            case "move":
                                {
                                    
                                    int id = msg.ReadInt32();
                                    int i = msg.ReadInt32();

                                    if (enemy.pawns[i] == null)
                                    {
                                        enemy.pawns[i] = new Pawn(game, null, player.buildingBoardState.getEmptyBoard().boardDictionaryById[id], i, Pawn.Team.enemy_team, i, player.buildingBoardState.font);
                                        enemy.pawns[i].current_tile = player.buildingBoardState.getEmptyBoard().boardDictionaryById[id];
                                        player.buildingBoardState.getEmptyBoard().boardDictionaryById[id].occupied = Tile.Occupied.yes_by_enemy;
                                        player.buildingBoardState.getEmptyBoard().boardDictionaryById[id].setCurrentPawn(enemy.pawns[i]);
                                        enemy.pawns[i].team = Pawn.Team.enemy_team;
                                    }
                                    else
                                    {
                                        enemy.pawns[i].current_tile.occupied = Tile.Occupied.no;
                                        enemy.pawns[i].current_tile = player.buildingBoardState.getEmptyBoard().boardDictionaryById[id];
                                        player.buildingBoardState.getEmptyBoard().boardDictionaryById[id].occupied = Tile.Occupied.yes_by_enemy;
                                        player.buildingBoardState.getEmptyBoard().boardDictionaryById[id].setCurrentPawn(enemy.pawns[i]);
                                        enemy.pawns[i].team = Pawn.Team.enemy_team;
                                        player.myTurn = true;
                                    }
                                   
                                    
                                   
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
