
/* Gil Nevo 310021654
 * Shachar Bartal 305262016
 */

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using Lidgren.Network;

namespace GameServer
{
    class Program //here we run our server.
    {
        // private static bool no_more_tiles =false; //if we dont need to read updates on tiles anymore, to save work.

        static void Main(string[] args)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("Flags");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = 14242; //our port
            config.EnableUPnP = true; //enabling port forwarding using upnp.

            // create and start server
            NetServer server = new NetServer(config);
            server.Start();

            //gamerooms that will contain 2 players each.
            GameRoom currentGameRoom = new GameRoom();
            List<GameRoom> gamerooms = new List<GameRoom>(); //list of all gamerooms.

            List<int> tile_list = new List<int>(); //list of tiles id's

            server.UPnP.ForwardPort(14242, "Flags game for school project");

            // schedule initial sending of position updates
            double nextSendUpdates = NetTime.Now;

            //Azure service info
            String resourcegroup = "";
            String containergroupname = "";


            // run until escape is pressed
            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {

                NetIncomingMessage msg;
                while ((msg = server.ReadMessage()) != null) //reading messages from clients.
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:

                            // Server received a discovery request from a client; send a discovery response
                            // with data about if its the first or second player.


                            NetOutgoingMessage msg_num_of_players = server.CreateMessage();
                            //  if (server.ConnectionsCount % 2 != 0) //second player
                            if (currentGameRoom.GetFirstPlayer() != null && currentGameRoom.GetSecondPlayer() == null)
                            {
                                msg_num_of_players.Write(1);

                                NetOutgoingMessage msg_sec_player_connected = server.CreateMessage();
                                msg_sec_player_connected.Write("second");
                                server.SendMessage(msg_sec_player_connected, currentGameRoom.GetFirstPlayer(), NetDeliveryMethod.ReliableOrdered);

                            }
                            else //first player
                            {
                                msg_num_of_players.Write(0);
                            }

                            server.SendDiscoveryResponse(msg_num_of_players, msg.SenderEndPoint); //sending response.
                            break;


                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            //
                            // Just print diagnostic messages to console
                            //
                            Console.WriteLine(msg.ReadString());
                            break;

                        case NetIncomingMessageType.StatusChanged:
                            if (resourcegroup != "")
                                try { SetSessions(server.Connections.Count, resourcegroup, containergroupname); } catch (Exception e) { }

                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                            if (status == NetConnectionStatus.Connected)
                            {
                                //
                                // A new player just connected!
                                //

                                //writing to console.
                                Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");

                                //in each connection we use it's TAG to keep game data of that client.
                                msg.SenderConnection.Tag = new int[602];

                                //resetting to -10 (meaning no game data to send to other clients)
                                int[] pos = msg.SenderConnection.Tag as int[];
                                for (int i = 0; i < pos.Length; i++)
                                {
                                    pos[i] = -10;
                                }

                                //creating new game room if you are the first player.
                                if (server.ConnectionsCount % 2 == 1)
                                {
                                    Console.WriteLine("new GameRoom");
                                    GameRoom tmp = new GameRoom();
                                    currentGameRoom = tmp;
                                    currentGameRoom.SetFirstPlayer(msg.SenderConnection);
                                    gamerooms.Add(currentGameRoom);
                                }
                                else //joining an existing room if you are the second player.
                                {
                                    currentGameRoom.SetSecondPlayer(msg.SenderConnection);
                                }
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                //writing to console.
                                Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " disconnected!");

                                foreach (var gameroom in gamerooms.ToArray())
                                {
                                    foreach (var player in gameroom.players)
                                    {
                                        if (player == msg.SenderConnection)
                                        {
                                            if (player == gameroom.GetFirstPlayer())
                                            {

                                                NetOutgoingMessage om = server.CreateMessage();
                                                om.Write("disconnect");
                                                server.SendMessage(om, gameroom.GetSecondPlayer(), NetDeliveryMethod.ReliableOrdered);
                                                gamerooms.Remove(gameroom);
                                            }
                                            else if (player == gameroom.GetSecondPlayer())
                                            {
                                                NetOutgoingMessage om = server.CreateMessage();
                                                om.Write("disconnect");
                                                server.SendMessage(om, gameroom.GetFirstPlayer(), NetDeliveryMethod.ReliableOrdered);
                                                gamerooms.Remove(gameroom);
                                            }


                                        }
                                    }
                                }
                            }

                            break;

                        case NetIncomingMessageType.Data: //reading game related data.
                            string data_string = msg.ReadString(); //reading message header
                            switch (data_string)
                            {

                                case "azure":
                                    {
                                        resourcegroup = msg.ReadString();
                                        containergroupname = msg.ReadString();
                                        break;
                                    }

                                case "win":
                                    {
                                        int[] pos = msg.SenderConnection.Tag as int[];
                                        pos[597] = 1;
                                        break;
                                    }
                                case "trigger":
                                    {
                                        int tile_id = msg.ReadInt32();//reading message
                                        int[] pos = msg.SenderConnection.Tag as int[]; //using connection TAG to save data about that client
                                        pos[598] = tile_id;
                                        break;
                                    }
                                case "flag":
                                    {
                                        int taken = msg.ReadInt32();

                                        int[] pos = msg.SenderConnection.Tag as int[]; //using connection TAG to save data about that client
                                        pos[599] = taken;
                                        break;
                                    }

                                case "teleport":
                                    {
                                        int tile_id = msg.ReadInt32();//reading message
                                        int[] pos = msg.SenderConnection.Tag as int[]; //using connection TAG to save data about that client
                                        if (pos[600] == -10)
                                            pos[600] = tile_id;
                                        else pos[601] = tile_id;
                                        break;
                                    }
                                case "tile_added":
                                    {
                                        int tile_id = msg.ReadInt32();//reading message
                                        int[] pos = msg.SenderConnection.Tag as int[];//using connection TAG to save data about that client
                                        int i = 0;
                                        while (pos[4 + i] != -10)
                                            i++;
                                        pos[4 + i] = tile_id;

                                        break;
                                    }
                                case "move":
                                    {
                                        int id = msg.ReadInt32();
                                        int indexinput = msg.ReadInt32();//reading message
                                        int[] pos = msg.SenderConnection.Tag as int[]; //using connection TAG to save data about that client
                                        pos[0] = id;
                                        pos[1] = indexinput;
                                        break;
                                    }

                                case "attacked":
                                    {
                                        int id = msg.ReadInt32();
                                        int indexinput = msg.ReadInt32();//reading message
                                        int[] pos = msg.SenderConnection.Tag as int[]; //using connection TAG to save data about that client
                                        pos[2] = id;
                                        pos[3] = indexinput;
                                        break;
                                    }
                            }

                            break;
                    }

                    //
                    // send position updates 30 times per second
                    //
                }

                double now = NetTime.Now;
                if (now > nextSendUpdates)
                {
                    // Yes, it's time to send position updates

                    // for each gameroom...
                    foreach (GameRoom gameroom in gamerooms)
                    {

                        // for each player...
                        foreach (NetConnection player in gameroom.players)
                        {
                            // ... send information to the other player
                            foreach (NetConnection otherPlayer in gameroom.players)
                            {
                                if (player != otherPlayer)
                                {


                                    NetOutgoingMessage om = server.CreateMessage();

                                    if (otherPlayer.Tag == null)
                                        otherPlayer.Tag = new int[602];

                                    int[] pos = otherPlayer.Tag as int[];

                                    if (pos[0] != -10 && pos[1] != -10) //if there is new data to write.
                                    {
                                        om.Write("move");
                                        om.Write(pos[0]);
                                        om.Write(pos[1]);

                                        // send message
                                        server.SendMessage(om, player, NetDeliveryMethod.ReliableOrdered, 0);
                                        pos[0] = -10;
                                        pos[1] = -10;

                                    }
                                    if (pos[597] == 1)
                                    {
                                        om = server.CreateMessage();
                                        om.Write("win");
                                        // send message
                                        server.SendMessage(om, player, NetDeliveryMethod.ReliableOrdered, 0);
                                        pos[597] = -10;
                                    }
                                    if (pos[598] != -10)
                                    {
                                        om = server.CreateMessage();
                                        om.Write("trigger");
                                        om.Write(pos[598]);
                                        // send message
                                        server.SendMessage(om, player, NetDeliveryMethod.ReliableOrdered, 0);
                                        pos[598] = -10;
                                    }
                                    if (pos[599] != -10) //if there is new data to write.
                                    {
                                        om.Write("flag");
                                        om.Write(pos[599]);


                                        // send message
                                        server.SendMessage(om, player, NetDeliveryMethod.ReliableOrdered, 0);
                                        pos[599] = -10;

                                    }

                                    int i = 0;
                                    while (pos[4 + i] == -10 && 4 + i < 599)
                                    {

                                        i++;
                                    }

                                    if (pos[4 + i] != -10) //if there is new data to write.
                                    {
                                        om = server.CreateMessage();
                                        om.Write("tile_added");
                                        om.Write(pos[4 + i]);
                                        server.SendMessage(om, player, NetDeliveryMethod.ReliableOrdered, 0);
                                        pos[4 + i] = -10;
                                    }


                                    if (pos[2] != -10 && pos[3] != -10) //if there is new data to write.
                                    {
                                        om = server.CreateMessage();
                                        om.Write("attacked");
                                        om.Write(pos[2]);
                                        om.Write(pos[3]);

                                        // send message
                                        server.SendMessage(om, player, NetDeliveryMethod.ReliableOrdered, 0);
                                        pos[2] = -10;
                                        pos[3] = -10;

                                    }
                                    if (pos[600] != -10 || pos[601] != -10) //if there is new data to write.
                                    {
                                        om = server.CreateMessage();
                                        om.Write("teleport");
                                        if (pos[600] != -10)
                                        {
                                            om.Write(pos[600]);
                                            // send message
                                            server.SendMessage(om, player, NetDeliveryMethod.ReliableOrdered, 0);
                                            pos[600] = -10;
                                        }
                                        else if (pos[601] != -10)
                                        {
                                            om.Write(pos[601]);
                                            // send message
                                            server.SendMessage(om, player, NetDeliveryMethod.ReliableOrdered, 0);
                                            pos[601] = -10;
                                        }
                                    }


                                }

                            }
                        }
                    }


                    // schedule next update
                    nextSendUpdates += (1.0 / 30.0);

                }

                // sleep to allow other processes to run smoothly
                Thread.Sleep(1);
            }
            server.Shutdown("app exiting");
        }


        public static async void SetSessions(int activesessions, String resourcegroup, String containergroupname)
        {
            //HTTP POST request to ACISetSessions function on Azure
            var client = new HttpClient();
            var response = await client.PostAsJsonAsync("https://herpsgodfunctions.azurewebsites.net/api/ACISetSessions?code=X7Nza24K5nr2HlyCkjG5ax/TXVUt9zbJzNPf8UW8VUfY3aRKEjiRSA==",
            new ACISetSessionsPost[]
            {
                            new ACISetSessionsPost
                            {
                               resourceGroup = resourcegroup,
                               containerGroupName = containergroupname,
                               activeSessions= activesessions
                            }
            });


        }


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

