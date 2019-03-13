using System;
using System.Collections.Generic;
using System.Threading;
using Lidgren.Network;

namespace GameServer
{
    class Program
    {

        static void Main(string[] args)
        {
            NetPeerConfiguration config = new NetPeerConfiguration("Flags");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = 14242;
            // config.EnableUPnP = true;

            // create and start server
            NetServer server = new NetServer(config);
            server.Start();

            GameRoom currentGameRoom = new GameRoom();
            List<GameRoom> gamerooms = new List<GameRoom>();

            List<int> tile_list=new List<int>();

            //  server.UPnP.ForwardPort(14242, "Flags game for school project");


            // schedule initial sending of position updates
            double nextSendUpdates = NetTime.Now;

            // run until escape is pressed
            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {

                NetIncomingMessage msg;
                while ((msg = server.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:
                            //
                            // Server received a discovery request from a client; send a discovery response (with no extra data attached)
                            //

                            NetOutgoingMessage msg_num_of_players = server.CreateMessage();
                            if (server.ConnectionsCount % 2 != 0)
                            {
                               
                                msg_num_of_players.Write(1);
                            }
                            else
                            {
                                
                                msg_num_of_players.Write(0);

                            }

                            server.SendDiscoveryResponse(msg_num_of_players, msg.SenderEndPoint);
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
                            NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                            if (status == NetConnectionStatus.Connected)
                            {

                                //
                                // A new player just connected!
                                //
                                Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");


                                msg.SenderConnection.Tag = new int[1000];//{
                                                                         //       -10,-10,-10,-10,-10
                                                                         //       };
                                int[] pos = msg.SenderConnection.Tag as int[];
                                for (int i = 0; i < pos.Length; i++)
                                {
                                    pos[i] = -10;
                                }

                                if(server.ConnectionsCount % 2 == 1)
                                {
                                    GameRoom tmp = new GameRoom();
                                    currentGameRoom = tmp;
                                    currentGameRoom.setFirstPlayer(msg.SenderConnection);
                                    gamerooms.Add(currentGameRoom);
                                }
                                else
                                {
                                    currentGameRoom.setSecondPlayer(msg.SenderConnection);
                                }
                               
                            }

                            break;
                        case NetIncomingMessageType.Data:
                            string data_string = msg.ReadString();
                            switch (data_string)
                            {
                                case "tile_added":
                                    {
                                        int tile_id= msg.ReadInt32();
                                        int[] pos = msg.SenderConnection.Tag as int[];
                                        int i = 0;
                                        while (pos[4 + i] != -10)
                                            i++;
                                        pos[4+i] = tile_id;
                                        
                                        break;
                                    }
                                case "move":
                                    {
                                        int id = msg.ReadInt32();
                                        int indexinput = msg.ReadInt32();
                                        int[] pos = msg.SenderConnection.Tag as int[];
                                        pos[0] = id;
                                        pos[1] = indexinput;
                                        break;
                                    }

                                case "attacked":
                                    {
                                        int id = msg.ReadInt32();
                                        int indexinput = msg.ReadInt32();
                                        int[] pos = msg.SenderConnection.Tag as int[];
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

                    // for each player...
                    foreach (GameRoom gameroom in gamerooms)
                    {

                        foreach (NetConnection player in gameroom.players)
                        {
                            // ... send information about every other player (actually including self)
                            foreach (NetConnection otherPlayer in gameroom.players)
                            {
                                if (player != otherPlayer)
                                {

                                    // send position update about 'otherPlayer' to 'player'
                                    NetOutgoingMessage om = server.CreateMessage();

                                    // write who this position is for
                                    //  om.Write(otherPlayer.RemoteUniqueIdentifier);

                                    if (otherPlayer.Tag == null)
                                        otherPlayer.Tag = new int[1000];

                                    int[] pos = otherPlayer.Tag as int[];
                                    if (pos[0] != -10 && pos[1] != -10)
                                    {
                                        om.Write("move");
                                        om.Write(pos[0]);
                                        om.Write(pos[1]);
                                        // send message
                                        server.SendMessage(om, player, NetDeliveryMethod.ReliableOrdered, 0);
                                        pos[0] = -10;
                                        pos[1] = -10;
                                    }

                                    if (pos[2] != -10 && pos[3] != -10)
                                    {
                                        om.Write("attacked");
                                        om.Write(pos[2]);
                                        om.Write(pos[3]);
                                        // send message
                                        server.SendMessage(om, player, NetDeliveryMethod.ReliableOrdered, 0);
                                        pos[2] = -10;
                                        pos[3] = -10;
                                    }
                                    int i = 0;
                                    while (pos[4 + i] == -10 && 4 + i < 999)
                                    {
                                        
                                        i++;
                                    }
                                        
                                    if(pos[4+i]!=-10)
                                    {
                                        
                                            om.Write("tile_added");
                                            om.Write(pos[4 + i]);
                                            server.SendMessage(om, player, NetDeliveryMethod.ReliableOrdered, 0);
                                        pos[4 + i] = -10;
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
    }
}
   
