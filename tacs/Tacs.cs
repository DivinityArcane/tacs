using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tacs.Network;

namespace tacs
{
    class Tacs
    {
        public const string Version = "0.219";
        public static Encoding Encoding = System.Text.Encoding.UTF8;
        public static Server server;
        public static ManualResetEvent Wait = new ManualResetEvent(false);

        private static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0);

        public static uint TS
        {
            get
            {
                return (uint)(DateTime.UtcNow - epoch).TotalSeconds;
            }
        }

        static void Main (string[] args)
        {
            ConIO.Write("Tacs v" + Version);
            ConIO.Write("Starting listen server");
            new Thread(() => Start()).Start();
            Wait.WaitOne();
        }

        private static void Start ()
        {
            server = new Server();
        }

        public static void Close()
        {
            Wait.Set();
        }
    }
}
