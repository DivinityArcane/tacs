using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace tacs.Network
{
    class Listener
    {
        public delegate void Callback (Socket s);

        private Socket sock;
        private IPEndPoint ep;
        private Callback onaccepted;
        private ConcurrentDictionary<string, uint> AntiFlood;

        public Listener (ushort port, Callback callback)
        {
            try
            {
                AntiFlood = new ConcurrentDictionary<string, uint>();
                ep = new IPEndPoint(IPAddress.Any, port);
                onaccepted = callback;
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Bind(ep);
                sock.Listen(32);
                sock.BeginAccept(new AsyncCallback(OnAccept), null);
                ConIO.Write("Listening for new connections on port " + port);
            }
            catch (Exception E)
            {
                Console.WriteLine("Unable to bind listen socket: {0}", E.Message);
                Environment.Exit(-1);
            }
        }

        private void OnAccept (IAsyncResult res)
        {
            try
            {
                Socket client = sock.EndAccept(res);
                var ip = client.RemoteEndPoint.ToString().Split(':')[0];

                if (AntiFlood.ContainsKey(ip))
                {
                    var time = AntiFlood[ip];

                    if (Tacs.TS - time < 30)
                    {
                        Packet p = new Packet("ERROR");
                        p.AddChunk("CANNOT CONNECT SO SOON AFTER LAST CONNECT");
                        client.Send(p.Finalize());
                        client.Disconnect(false);
                    }
                    else
                    {
                        AntiFlood.TryRemove(ip, out time);
                        onaccepted(client);
                    }
                }
                else
                {
                    AntiFlood.TryAdd(ip, Tacs.TS);
                    onaccepted(client);
                }
            }
            catch { }
            sock.BeginAccept(new AsyncCallback(OnAccept), null);
        }
    }
}
