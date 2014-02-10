using System;
using System.Net;
using System.Net.Sockets;
using tacs.Network;
using tacs.Structures;

namespace tacs.Entities
{
    class Client
    {
        private Socket sock;
        private ClientState state;
        private String name;
        private EndPoint ep;
        private byte[] buffer;
        private String garbage;
        private System.Timers.Timer Timeout;

        private readonly object statelock = new object();

        public bool disposed = false;
        public uint Pinged = 0;
        public uint Challenged = 0;
        public String Target;
        public Game Game;

        public String Name
        {
            get { return name; }
        }

        public ClientState State
        {
            get{ return state; }
        }

        public EndPoint EndPoint
        {
            get
            {
                return ep;
            }
        }

        public Client (Socket s)
        {
            sock = s;
            ep = sock.RemoteEndPoint;
            buffer = new byte[1028];
            garbage = String.Empty;
            sock.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
            state = ClientState.Connecting;

            Timeout = new System.Timers.Timer(30000);
            Timeout.Elapsed += delegate
            {
                if (state == ClientState.Connecting)
                    Disconnect("LOGIN TIMEOUT");
            };
            Timeout.Start();
        }

        public void SetName (String name)
        {
            this.name = name;
        }

        public void SetState (ClientState state)
        {
            if (this.state == ClientState.Disconnected) return;

            this.state = state;

            if (state == ClientState.Connected)
            {
                Timeout.Stop();
                Timeout.Dispose();
            }
        }

        private void OnReceive (IAsyncResult res)
        {
            if (state == ClientState.Disconnected) return;
            try
            {
                int rcv = sock.EndReceive(res);
                if (rcv > 0)
                {
                    byte[] data = new byte[rcv];
                    Buffer.BlockCopy(buffer, 0, data, 0, rcv);
                    string pkt = Tacs.Encoding.GetString(data);
                    Array.Clear(buffer, 0, rcv);

                    if (garbage.Length > 0)
                    {
                        pkt = garbage + pkt;
                        garbage = String.Empty;
                    }

                    if (!pkt.Contains("\a"))
                    {
                        Disconnect("INVALID PACKET");
                        return;
                    }

                    string[] chunks = pkt.Split('\a');

                    foreach (var chunk in chunks)
                    {
                        if (chunk.Length > 0 && sock != null && state != ClientState.Disconnected)
                        {
                            if (chunk.EndsWith("\0"))
                                Tacs.server.Handle(this, pkt);
                            else
                                garbage += chunk;
                        }
                    }
                }
                sock.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
            }
            catch
            {
                if (state != ClientState.Disconnected)
                    Disconnect("READ ERROR");
            }
        }

        public void Send (Packet p)
        {
            try
            {
                sock.Send(p.Finalize());
            }
            catch
            {
                if (state != ClientState.Disconnected)
                    Disconnect("SEND ERROR");
            }
        }

        public void Error (string msg)
        {
            Packet p = new Packet("ERROR");
            p.AddChunk(msg);
            Send(p);
        }

        public void Disconnect (string reason)
        {
            lock (statelock)
            {
                if (state == ClientState.Disconnected) return;
                state = ClientState.Disconnected;
            }

            try
            {
                Packet p = new Packet("DISCONNECT");
                p.AddChunk(reason);
                Send(p);
                sock.Disconnect(false);
                sock.Dispose();
            }
            catch { }
            Tacs.server.RemoveClient(ep);
        }
    }
}
