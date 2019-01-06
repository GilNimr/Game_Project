using System;
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

                // create and start server
                NetServer server = new NetServer(config);
                server.Start();

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
                                msg_num_of_players.Write(0);

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

                                // randomize his position and store in connection tag
                                msg.SenderConnection.Tag = new int[2] {
                                -10,-10
                                };
                                }

                                break;
                            case NetIncomingMessageType.Data:
                            //
                            // The client sent input to the server
                            //
                            int id = msg.ReadInt32();
                            int indexinput=msg.ReadInt32();
                                

                                int[] pos = msg.SenderConnection.Tag as int[];

                            // fancy movement logic goes here; we just append input to position
                            
                            pos[0] = id;
                            pos[1] = indexinput;

                            break;
                        }

                        //
                        // send position updates 30 times per second
                        //
                        double now = NetTime.Now;
                        if (now > nextSendUpdates)
                        {
                            // Yes, it's time to send position updates

                            // for each player...
                            foreach (NetConnection player in server.Connections)
                            {
                                // ... send information about every other player (actually including self)
                                foreach (NetConnection otherPlayer in server.Connections)
                                {
                                if (player != otherPlayer)
                                {
                                    // send position update about 'otherPlayer' to 'player'
                                    NetOutgoingMessage om = server.CreateMessage();

                                    // write who this position is for
                                    //  om.Write(otherPlayer.RemoteUniqueIdentifier);

                                    if (otherPlayer.Tag == null)
                                        otherPlayer.Tag = new int[2];

                                    int[] pos = otherPlayer.Tag as int[];
                                    if (pos[0] != -10 && pos[1] != -10)
                                    {
                                        om.Write(pos[0]);
                                        om.Write(pos[1]);
                                        // send message
                                        server.SendMessage(om, player, NetDeliveryMethod.ReliableOrdered, 0);
                                    }
                                }
                                
                                }
                            }

                            // schedule next update
                            nextSendUpdates += (1.0 / 30.0);
                        }
                    }

                    // sleep to allow other processes to run smoothly
                    Thread.Sleep(1);
                }

                server.Shutdown("app exiting");
            }
        }
    }
