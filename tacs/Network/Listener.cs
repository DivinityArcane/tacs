using System;
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

        public Listener (ushort port, Callback callback)
        {
            try
            {
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
                onaccepted(client);
            }
            catch { }
            sock.BeginAccept(new AsyncCallback(OnAccept), null);
        }
    }
}
