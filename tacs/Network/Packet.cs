
namespace tacs.Network
{
    class Packet
    {
        private string type, payload;

        public Packet (string type)
        {
            this.type = type;
            this.payload = string.Empty;
        }

        public void AddChunk (string chunk)
        {
            this.payload += ("\0" + chunk);
        }

        public byte[] Finalize ()
        {
            return Tacs.Encoding.GetBytes(this.type + this.payload + "\0\a");
        }
    }
}
