
/* Gil Nevo 310021654
 * Shachar Bartal 305262016
 */

using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization.Json;
using System.Net.Http;
using System.Runtime.Serialization;
using MonoGame.Shared1.States_and_state_related;

namespace MonoGame.Shared1
{
    public class Connection //the client side of connection
    {
        public NetClient client;

        public Player player;
        public Player enemy;

        public static bool local; // if this is a local connection.

        readonly Game game;

        private NetConnection server;

        public static NetOutgoingMessage outmsg;

        public string ip;
        ACIlistResponse[] aci;
        int index = 0;

        public Connection(Game game, ref Player _player, ref Player _enemy)
        {
            this.game = game;
            enemy = _enemy;
            player = _player;

            NetPeerConfiguration config = new NetPeerConfiguration("Flags");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

            client = new NetClient(config);
            client.Start(); //Binds to socket and spawns the networking thread.

            if (local)
                client.DiscoverLocalPeers(14242);
            else
            {
                try { ip = GetIP().Result; } catch (Exception e) { ip = "0.0.0.0"; }
                client.DiscoverKnownPeer(ip, 14242); 
            }

        }


        //HTTP GET request to Azure ACIList function which return Json that contains Array of ACIlistResponse[]
        //that contains IP.
        public async System.Threading.Tasks.Task<string> GetIP()
        {
            var client = new HttpClient();

            var resp = await client.GetAsync("https://herpsgodfunctions.azurewebsites.net/api/ACIList");

            var content = await resp.Content.ReadAsStreamAsync();

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ACIlistResponse[]));

            aci = (ACIlistResponse[])ser.ReadObject(content);



            for (int i = 0; i < aci.Length; i++)
            {
                if (aci[i].ActiveSessions < 10)
                {
                    index = i;
                    break;
                }
            }

            return aci[index].PublicIP;

        }

        public async void SetSessions()
        {
            //HTTP POST request to ACISetSessions function on Azure
            var client = new HttpClient();
            var response = await client.PostAsJsonAsync("https://herpsgodfunctions.azurewebsites.net/api/ACISetSessions?code=X7Nza24K5nr2HlyCkjG5ax/TXVUt9zbJzNPf8UW8VUfY3aRKEjiRSA==",
            new ACISetSessionsPost[]
            {
                            new ACISetSessionsPost
                            {
                               resourceGroup = aci[index].ResourceGroup,
                               containerGroupName = aci[index].ContainerGroupName,
                               activeSessions= aci[index].ActiveSessions + 1
                            }
            });

        }

        public void SendFlagChoise(int i)
        {
            NetOutgoingMessage om = client.CreateMessage();
            om.Write("flag");
            om.Write(i);
            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
        }
        public void SendWin()
        {
            NetOutgoingMessage om = client.CreateMessage();
            om.Write("win");
            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendTelParticle(int tile_id)
        {
            NetOutgoingMessage om = client.CreateMessage();
            om.Write("trigger");
            om.Write(tile_id);
            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
        }

        public void Update()
        {
            if (player != null)
            {
                if (player.pawns != null) //updates on pawns 
                {
                    NetOutgoingMessage om = client.CreateMessage();
                    for (int i = 0; i < player.pawns.Length; i++)
                    {
                        if (player.pawns[i] != null)
                        {
                            if (player.pawns[i].send_update)
                            {
                                om.Write("move");
                                om.Write(player.pawns[i].current_tile.GetId());
                                om.Write(i);


                                client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
                                player.pawns[i].send_update = false;
                                player.myTurn = false;

                            }
                        }
                    }
                }
            }
            if (enemy != null)
            {


                if (enemy.pawns != null) //updates on enemys being attacked.
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
            }

            NetIncomingMessage msg;
            while ((msg = client.ReadMessage()) != null) //reading all the messages from server
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse: //message that approves our connection

                        server = client.Connect(msg.SenderEndPoint);

                        int num_of_players = msg.ReadInt32(); //reading if we are player 1 or player2

                        if (num_of_players == 1)
                            BuildingBoardState.i_am_second_player = true;
                        else
                            BuildingBoardState.wait_for_other_player = true;
                        if (!local)
                        {
                            //sending Azure info to server
                            NetOutgoingMessage om = client.CreateMessage();
                            om.Write("azure");
                            om.Write(aci[index].ResourceGroup);
                            om.Write(aci[index].ContainerGroupName);
                            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
                        }
                        break;

                    case NetIncomingMessageType.Data: //all game related messages

                        string data_string = msg.ReadString(); //reading message header

                        switch (data_string)
                        {
                            case "win":
                                {
                                    PlayingState.win = true;
                                    break;
                                }
                            case "trigger":
                                {
                                    int tile_id = msg.ReadInt32();
                                    player.particleService.Trigger(Game1.TwoD2isometrix(player.Board.boardDictionaryById[tile_id].GetCartasianRectangle().Center.X, player.Board.boardDictionaryById[tile_id].GetCartasianRectangle().Center.Y));
                                    break;
                                }
                            case "flag":
                                {
                                    int taken = msg.ReadInt32();
                                    player.chooseFlagState.taken = taken;
                                    enemy.flag = player.chooseFlagState.flags[taken];
                                    break;
                                }
                            case "second":
                                {
                                    BuildingBoardState.wait_for_other_player = false;
                                    break;
                                }
                            case "disconnect":
                                {
                                    PlayingState.win = true;
                                    break;
                                }

                            case "tile_added":
                                {
                                    int tile_id = msg.ReadInt32(); //reading message
                                    player.Board.boardDictionaryById[tile_id].texture = player.Board.boardDictionaryById[299].texture;
                                    player.Board.boardDictionaryById[tile_id].SetIsHidden(false);

                                    break;
                                }
                            case "teleport":
                                {
                                    int tile_id = msg.ReadInt32(); //reading message
                                    player.Board.boardDictionaryById[tile_id].texture = PlayingState.teleport_texture;
                                    player.Board.boardDictionaryById[tile_id].SetIsHidden(false);
                                    player.Board.boardDictionaryById[tile_id].teleport_tile = true;

                                    for (int i = 0; i < PlayingState.teleports.Length; i++)
                                    {
                                        if (PlayingState.teleports[i] == null)
                                        {
                                            PlayingState.teleports[i] = player.Board.boardDictionaryById[tile_id];
                                            break;
                                        }

                                    }

                                    break;
                                }
                            case "move":
                                {

                                    int id = msg.ReadInt32();//reading message
                                    int i = msg.ReadInt32();//reading message

                                    if (enemy.pawns[i] == null) //registering new enemy's pawm
                                    {
                                        enemy.pawns[i] = new Pawn(game, enemy.flag, player.buildingBoardState.GetEmptyBoard().boardDictionaryById[id], i + 1, Pawn.Team.enemy_team, i, player.buildingBoardState.font)
                                        {
                                            current_tile = player.buildingBoardState.GetEmptyBoard().boardDictionaryById[id]
                                        };
                                        player.buildingBoardState.GetEmptyBoard().boardDictionaryById[id].occupied = Tile.Occupied.yes_by_enemy;
                                        player.buildingBoardState.GetEmptyBoard().boardDictionaryById[id].SetCurrentPawn(enemy.pawns[i]);
                                        enemy.pawns[i].team = Pawn.Team.enemy_team;
                                    }
                                    else //updating enemy's pawn position
                                    {
                                        enemy.pawns[i].current_tile.occupied = Tile.Occupied.no;
                                        enemy.pawns[i].current_tile = player.buildingBoardState.GetEmptyBoard().boardDictionaryById[id];
                                        player.buildingBoardState.GetEmptyBoard().boardDictionaryById[id].occupied = Tile.Occupied.yes_by_enemy;
                                        player.buildingBoardState.GetEmptyBoard().boardDictionaryById[id].SetCurrentPawn(enemy.pawns[i]);
                                        enemy.pawns[i].team = Pawn.Team.enemy_team;
                                        player.myTurn = true;

                                        if (enemy.pawns[i].current_tile.teleport_tile)
                                            enemy.pawns[i].trigger_teleport_particle = true;
                                    }

                                    break;
                                }
                            case "attacked": //if we are being attacked
                                {
                                    int id = msg.ReadInt32();//reading message
                                    int i = msg.ReadInt32();//reading message

                                    player.pawns[i].attacked = true;
                                    player.pawns[i].attacker = enemy.pawns[id];

                                    break;
                                }
                        }
                        break;
                }
            }
        }

        //checking to see for updates to write to server.
        public void SendTileUpdate(int i)
        {
            if (player.Board.boardDictionaryById[i].sendUpdate)
            {
                if (player.Board.boardDictionaryById[i].teleport_tile) //updates on teleports
                {
                    NetOutgoingMessage om = client.CreateMessage();
                    om.Write("teleport");
                    om.Write(i);
                    client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
                    player.Board.boardDictionaryById[i].sendUpdate = false;
                }
                else                                                  //updates on other tiles
                {
                    NetOutgoingMessage om = client.CreateMessage();
                    om.Write("tile_added");
                    om.Write(i);
                    client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
                    player.Board.boardDictionaryById[i].sendUpdate = false;
                }

            }
        }

    }


    //the Json we recieve from HTTP function ACIList that in our Azure project.
    [DataContract]
    internal class ACIlistResponse
    {
        [DataMember]
        internal string ResourceGroup;

        [DataMember]
        internal string ContainerGroupName;

        [DataMember]
        internal string PublicIP;

        [DataMember]
        internal int ActiveSessions;

        [DataMember]
        internal string Locations;
    }


    [DataContract]
    internal class ACISetSessionsPost
    {

        [DataMember]
        internal string resourceGroup;

        [DataMember]
        internal string containerGroupName;

        [DataMember]
        internal int activeSessions;

    }
}
