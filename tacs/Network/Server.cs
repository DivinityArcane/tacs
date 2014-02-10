using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Timers;
using tacs.Entities;
using tacs.Structures;

namespace tacs.Network
{
    class Server
    {
        private Listener Listen;
        private ConcurrentDictionary<EndPoint, Client> Pending;
        private ConcurrentDictionary<EndPoint, Client> Online;
        private ConcurrentDictionary<EndPoint, string> ClientNames;
        private List<Client> ClientQueue;
        private Timer PingTimer;
        private static Regex NameRegex;
        const uint timeout = 30;

        public Server (ushort port = 128)
        {
            Pending = new ConcurrentDictionary<EndPoint, Client>();
            Online = new ConcurrentDictionary<EndPoint, Client>();
            ClientNames = new ConcurrentDictionary<EndPoint, string>();
            ClientQueue = new List<Client>();
            Listen = new Listener(port, Accepted);
            NameRegex = new Regex(@"/[^a-zA-Z0-9\\-\\_]/");

            PingTimer = new Timer((timeout * 1000) / 2);
            PingTimer.Elapsed += delegate
            {
                Packet p = new Packet("PING");
                var ts = Tacs.TS;
                p.AddChunk(ts.ToString());
                foreach (var client in Online.Values)
                {
                    if (client.Pinged == 0)
                    {
                        client.Pinged = ts;
                        client.Send(p);
                    }
                    else if (ts - client.Pinged > timeout)
                    {
                        client.Disconnect("Timed out");
                    }
                }
            };
            PingTimer.Start();
        }

        private void Accepted (Socket sock)
        {
            if (Pending.ContainsKey(sock.RemoteEndPoint))
            {
                sock.Disconnect(false);
                return;
            }

            Client client = new Client(sock);
            ConIO.Write("Pending connection from " + client.EndPoint.ToString());
            client.SetState(ClientState.Connecting);
            Pending.TryAdd(sock.RemoteEndPoint, client);
            HandShake(client);
        }

        private void HandShake (Client client)
        {
            Packet p = new Packet("HANDSHAKE");
            p.AddChunk("Tacs_v" + Tacs.Version);
            client.Send(p);
        }

        private void Joined (string name)
        {
            ConIO.Write(name + " has connected.");
            Packet p = new Packet("LOBBYUSER");
            p.AddChunk("JOIN");
            p.AddChunk(name);
            SendAll(p);
        }

        private void ProcessQueue ()
        {
            if (ClientQueue.Count <= 1) return;

            lock (ClientQueue)
            {
                Client A = ClientQueue[ClientQueue.Count - 1];
                Client B = ClientQueue[0];

                if (A != B)
                {
                    ClientQueue.Remove(A);
                    ClientQueue.Remove(B);

                    NewGame(A, B);
                }
            }
        }

        private void NewGame (Client A, Client B)
        {
            Game G = new Game(A, B);

            A.Game = G;
            B.Game = G;

            A.SetState(ClientState.InGame);
            B.SetState(ClientState.InGame);

            Packet p = new Packet("STATE");
            p.AddChunk(A.Name);
            p.AddChunk("2");
            SendAll(p);

            p = new Packet("STATE");
            p.AddChunk(B.Name);
            p.AddChunk("2");
            SendAll(p);

            p = new Packet("GAME");
            p.AddChunk(B.Name);
            A.Send(p);

            p = new Packet("GAME");
            p.AddChunk(A.Name);
            B.Send(p);

            p = new Packet("TURN");
            A.Send(p);

            p = new Packet("WAIT");
            B.Send(p);
        }

        private void AnnounceOnlineUsers (Client c)
        {
            Packet p;

            foreach (var c2 in Online.Values)
            {
                if (c2 != c)
                {
                    p = new Packet("LOBBYUSER");
                    p.AddChunk("ANNOUNCE");
                    p.AddChunk(c2.Name);
                    c.Send(p);
                }
            }
        }

        private Client GetClientByName (string name)
        {
            foreach (var KVP in Online)
            {
                if (KVP.Value.Name.ToLower() == name.ToLower())
                    return KVP.Value;
            }
            return null;
        }

        public void Handle (Client client, string pkt)
        {
            string[] chunks = pkt.Split('\0');
            string type = chunks[0];

            if (type.Length < 3) return;

            switch (type.ToUpper())
            {
                #region Handshake
                case "HELLO":
                    {
                        if (client.State != ClientState.Connecting) return;

                        if (chunks.Length < 2 || chunks[1].Length < 3 || chunks[1].Length > 20)
                        {
                            client.Error("Invalid name length! Name must be between 3 and 20 characters long!");
                            return;
                        }

                        var safename = NameRegex.Replace(chunks[1], "");
                        var name = chunks[1];
                        var key = name.ToLower();

                        if (name != safename)
                        {
                            client.Error("Invalid character(s) in name. Names can only contain a-z, A-Z, 0-9, - and _");
                            return;
                        }

                        Packet p;

                        if (key.Contains("admin") || key.Contains("server") || key.Contains("staff") || key.Contains("moderator"))
                        {
                            client.Disconnect("That name is reserved!");
                            return;
                        }

                        if (ClientNames.Values.Contains(key))
                        {
                            client.Error("That name is already in use at the moment.");
                            return;
                        }

                        ConIO.Write("Connection [" + client.EndPoint.ToString() + "] identified as " + name);
                        ClientNames.TryAdd(client.EndPoint, key);
                        Online.TryAdd(client.EndPoint, client);
                        client.SetName(name);
                        client.SetState(ClientState.Connected);

                        p = new Packet("LOGIN");
                        p.AddChunk(name);
                        client.Send(p);

                        var ts = Tacs.TS;
                        client.Pinged = ts;
                        p = new Packet("PING");
                        p.AddChunk(ts.ToString());
                        client.Send(p);

                        client.SetState(ClientState.LobbyIdle);

                        Joined(name);
                        AnnounceOnlineUsers(client);

                        break;
                    }
                #endregion Handshake

                #region Ping Response
                case "PONG":
                    {
                        if (client.Pinged == 0 || chunks.Length < 2 || chunks[1] != client.Pinged.ToString())
                        {
                            client.Disconnect("Invalid pong payload!");
                            return;
                        }

                        client.Pinged = 0;
                        break;
                    }
                #endregion Ping Response

                #region Chat Message
                case "MESSAGE":
                    {
                        if (chunks.Length < 2)
                        {
                            client.Error("Invalid message payload!");
                            return;
                        }

                        var msg = chunks[1];
                        Packet p = new Packet("MESSAGE");
                        p.AddChunk(client.Name);
                        p.AddChunk(msg);
                        SendAll(p);

                        break;
                    }
                #endregion Chat Message

                #region Quit
                case "QUIT":
                    {
                        client.Disconnect("QUIT");
                        break;
                    }
                #endregion Quit

                #region Play (random)
                case "PLAY":
                    {
                        if (client.State != ClientState.LobbyIdle) break;
                        if (ClientQueue.Contains(client)) break;

                        if (client.QueueTime != 0 && Tacs.TS - client.QueueTime < 30)
                        {
                            client.Error("Cannot enter the play queue so quickly after you last entered!");
                            return;
                        }

                        lock (ClientQueue)
                        {
                            ClientQueue.Add(client);
                        }

                        client.SetState(ClientState.LobbyReady);
                        Packet p = new Packet("STATE");
                        p.AddChunk(client.Name);
                        p.AddChunk("1");
                        SendAll(p);
                        client.QueueTime = Tacs.TS;

                        ProcessQueue();

                        break;
                    }
                #endregion Play (random)

                #region Idle (leave queue)
                case "IDLE":
                    {
                        if (client.State != ClientState.LobbyReady) break;
                        if (!ClientQueue.Contains(client)) break;

                        lock (ClientQueue)
                        {
                            ClientQueue.Remove(client);
                        }

                        client.SetState(ClientState.LobbyIdle);
                        Packet p = new Packet("STATE");
                        p.AddChunk(client.Name);
                        p.AddChunk("0");
                        SendAll(p);

                        break;
                    }
                #endregion Idle (leave queue)

                #region Challenge
                case "CHALLENGE":
                    {
                        if (client.State != ClientState.LobbyIdle) break;
                        if (chunks.Length < 2)
                        {
                            client.Error("Invalid challenge payload!");
                            return;
                        }

                        uint delay;
                        if ((delay = Tacs.TS - client.Challenged) < 30)
                        {
                            delay = 30 - delay;
                            client.Error(string.Format("Cannot challenge again for {0} second{1}!", delay, delay == 1 ? "" : "s"));
                            return;
                        }

                        var name = chunks[1].ToLower();

                        if (name == client.Name.ToLower())
                        {
                            client.Error("Cannot challenge yourself!");
                            return;
                        }

                        if (!ClientNames.Values.Contains(name))
                        {
                            client.Error("Invalid name, or that user is offline!");
                            return;
                        }

                        Client other = GetClientByName(name);

                        if (other == null)
                        {
                            client.Error("Invalid client/challenge target!");
                            return;
                        }

                        client.Challenged = Tacs.TS;
                        client.Target = other.Name;
                        Notice(client, "Challenge request sent to " + other.Name);
                        Packet p = new Packet("CHALLENGED");
                        p.AddChunk(client.Name);
                        other.Send(p);

                        break;
                    }
                #endregion Challenge

                #region Accept
                case "ACCEPT":
                    {
                        if (client.State != ClientState.LobbyIdle) break;
                        if (chunks.Length < 2)
                        {
                            client.Error("Invalid accept payload!");
                            return;
                        }

                        var name = chunks[1].ToLower();

                        if (name == client.Name.ToLower())
                        {
                            client.Error("Cannot challenge yourself!");
                            return;
                        }

                        if (!ClientNames.Values.Contains(name))
                        {
                            client.Error("Invalid name, or that user is offline!");
                            return;
                        }

                        Client other = GetClientByName(name);

                        if (other == null)
                        {
                            client.Error("Invalid client/challenge target!");
                            return;
                        }

                        if (other.Target != client.Name)
                        {
                            client.Error("Target has not challenged you, or has challenged someone else since then!");
                            return;
                        }

                        other.Target = null;
                        client.Target = null;

                        Notice(other, "Challenge accepted by " + client.Name);

                        NewGame(other, client);

                        break;
                    }
                #endregion Accept

                #region Mark
                case "MARK":
                    {
                        if (client.State != ClientState.InGame) break;
                        if (chunks.Length < 2)
                        {
                            client.Error("Invalid mark payload!");
                            return;
                        }

                        if (client.Game == null)
                        {
                            client.Error("Invalid game state!");
                            return;
                        }

                        if (!client.Game.IsTurn(client))
                        {
                            Notice(client, "It's not your turn!");
                            return;
                        }

                        byte tile = 0;
                        byte.TryParse(chunks[1], out tile);
                        bool success = client.Game.MakeMove(client, tile);
                        var state = client.Game.GetState(client);

                        if (success && state != TileState.UnMarked)
                        {
                            Packet p = new Packet("MOVE");
                            p.AddChunk(tile.ToString());
                            p.AddChunk(((byte)state).ToString());

                            client.Game.SendAll(p);

                            if (client.Game.HasWon(client))
                            {
                                client.Game.Winner(client);
                            }
                            else if (client.Game.IsBoardFull())
                            {
                                client.Game.Draw();
                            }
                            else
                            {
                                client.Game.NegotiateTurns(client);
                            }
                        }
                        else
                        {
                            Notice(client, "Invalid move! Try again.");
                        }

                        break;
                    }
                #endregion Mark

                default:
                    client.Disconnect("Unknown packet type: " + type);
                    break;
            }
        }

        public void Notice (Client c, string msg)
        {
            Packet p = new Packet("NOTICE");
            p.AddChunk(msg);
            c.Send(p);
        }

        public void ChatMessage (string from, string msg)
        {
            Packet p = new Packet("MESSAGE");
            p.AddChunk(from);
            p.AddChunk(msg);
            SendAll(p);
        }

        public void Announce (string msg)
        {
            ChatMessage("SERVER", msg);
        }

        public void SendAll (Packet p)
        {
            foreach (var client in Online.Values)
            {
                client.Send(p);
            }
        }

        public void RemoveClient (EndPoint ep)
        {
            Client c;

            lock (ClientNames)
            {
                if (ClientNames.ContainsKey(ep))
                {
                    c = GetClientByName(ClientNames[ep]);
                    var name = ClientNames[ep];

                    if (c != null)
                    {
                        if (c.disposed) return;

                        name = c.Name;
                        lock (ClientQueue)
                        {
                            if (ClientQueue.Contains(c))
                            {
                                ClientQueue.Remove(c);
                            }
                        }

                        if (c.Game != null)
                        {
                            c.Game.Forfeit(c);
                        }

                        c.disposed = true;
                    }

                    ConIO.Write(name + " has disconnected.");
                    Packet p = new Packet("LOBBYUSER");
                    p.AddChunk("LEAVE");
                    p.AddChunk(name);
                    SendAll(p);
                    ClientNames.TryRemove(ep, out name);
                }

                if (Online.ContainsKey(ep))
                    Online.TryRemove(ep, out c);
            }
        }
    }
}
