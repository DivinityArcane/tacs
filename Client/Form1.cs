using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        private Socket sock;
        private byte[] buffer;
        private string garbage;
        private delegate void NoParamsCallback ();
        private delegate void MsgCallback (string text);
        private delegate void RTMsgCallback (string who, string text);
        private List<string> userlist;
        private string name;
        private bool InGame = false;
        private static string server;

        public static Encoding encoding = Encoding.UTF8;

        public Form1 ()
        {
            InitializeComponent();
        }

        private void UpdateUserList ()
        {
            if (UserList.InvokeRequired)
            {
                UserList.Invoke(new NoParamsCallback(UpdateUserList));
            }
            else
            {
                userlist.Sort();
                UserList.DataSource = null;
                UserList.DataSource = userlist;
                UserList.Refresh();
            }
        }

        private void ChatMsg (String text)
        {
            if (ChatBox.InvokeRequired)
            {
                ChatBox.Invoke(new MsgCallback(ChatMsg), text);
            }
            else
            {
                ChatBox.AppendText(text + Environment.NewLine);
                ChatBox.ScrollToCaret();
            }
        }

        private void RTChatMsg (String who, String text)
        {
            if (ChatBox.InvokeRequired)
            {
                ChatBox.Invoke(new RTMsgCallback(RTChatMsg), who, text);
            }
            else
            {
                lock (ChatBox)
                {
                    int start = ChatBox.Text.Length;
                    ChatBox.AppendText("<" + who + "> " + text + Environment.NewLine);
                    ChatBox.Select(start + 1, who.Length);
                    ChatBox.SelectionFont = new Font(ChatBox.Font, FontStyle.Bold);
                    ChatBox.DeselectAll();
                    ChatBox.ScrollToCaret();
                }
            }
        }

        private void RTNotice (String text)
        {
            if (ChatBox.InvokeRequired)
            {
                ChatBox.Invoke(new MsgCallback(RTNotice), text);
            }
            else
            {
                lock (ChatBox)
                {
                    int start = ChatBox.Text.Length;
                    ChatBox.AppendText("** " + text + Environment.NewLine);
                    ChatBox.Select(start, text.Length + 3);
                    ChatBox.SelectionFont = new Font(ChatBox.Font, FontStyle.Bold);
                    ChatBox.DeselectAll();
                    ChatBox.ScrollToCaret();
                }
            }
        }

        private void EnableTiles ()
        {
            if (Tile0.InvokeRequired)
            {
                Tile0.Invoke(new NoParamsCallback(EnableTiles));
            }
            else
            {
                if (Tile0.Text.Length < 1)
                    Tile0.Enabled = true;

                if (Tile1.Text.Length < 1)
                    Tile1.Enabled = true;

                if (Tile2.Text.Length < 1)
                    Tile2.Enabled = true;

                if (Tile3.Text.Length < 1)
                    Tile3.Enabled = true;

                if (Tile4.Text.Length < 1)
                    Tile4.Enabled = true;

                if (Tile5.Text.Length < 1)
                    Tile5.Enabled = true;

                if (Tile6.Text.Length < 1)
                    Tile6.Enabled = true;

                if (Tile7.Text.Length < 1)
                    Tile7.Enabled = true;

                if (Tile8.Text.Length < 1)
                    Tile8.Enabled = true;
            }
        }

        private void DisableTiles ()
        {
            if (Tile0.InvokeRequired)
            {
                Tile0.Invoke(new NoParamsCallback(DisableTiles));
            }
            else
            {
                Tile0.Enabled = false;
                Tile1.Enabled = false;
                Tile2.Enabled = false;
                Tile3.Enabled = false;
                Tile4.Enabled = false;
                Tile5.Enabled = false;
                Tile6.Enabled = false;
                Tile7.Enabled = false;
                Tile8.Enabled = false;
            }
        }

        private void ClearTiles ()
        {
            if (Tile0.InvokeRequired)
            {
                Tile0.Invoke(new NoParamsCallback(ClearTiles));
            }
            else
            {
                Tile0.Text = String.Empty;
                Tile1.Text = String.Empty;
                Tile2.Text = String.Empty;
                Tile3.Text = String.Empty;
                Tile4.Text = String.Empty;
                Tile5.Text = String.Empty;
                Tile6.Text = String.Empty;
                Tile7.Text = String.Empty;
                Tile8.Text = String.Empty;
            }
        }

        private void GetName ()
        {
            var f = new GetName();

            if (f.ShowDialog(this) == DialogResult.OK)
            {
                var name = f.Input.Text;
                if (name.Length < 3 || name.Length > 20)
                {
                    MessageBox.Show("Invalid name length! Names must be between 3 and 20 characters long.");
                    GetName();
                }
                else
                {
                    this.name = name;
                }
            }
            else
            {
                MessageBox.Show("You must input a name for use on the server!");
                GetName();
            }
        }

        private void Form1_Load (object sender, EventArgs e)
        {
            //"direct.divinearcanum.org"
            if (File.Exists(@"./server.ini"))
            {
                server = File.ReadAllText(@"./server.ini").Trim();
            }
            else
            {
                MessageBox.Show("Server configuration file is missing!");
                Environment.Exit(-1);
            }

            GetName();

            buffer = new byte[1024];
            userlist = new List<string>();
            UserList.DataSource = userlist;

            try
            {
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.BeginConnect(server, 128, new AsyncCallback(OnConnect), null);
            }
            catch (Exception E)
            {
                ChatMsg(E.Message);
            }
        }

        private void OnConnect (IAsyncResult res)
        {
            try
            {
                sock.EndConnect(res);
                sock.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
            }
            catch (Exception E)
            {
                ChatMsg(E.ToString());
            }
        }

        private void OnReceive (IAsyncResult res)
        {
            int rcv = 0;

            try
            {
                rcv = sock.EndReceive(res);

                if (rcv > 0)
                {
                    byte[] data = new byte[rcv];
                    Buffer.BlockCopy(buffer, 0, data, 0, rcv);
                    var pkt = encoding.GetString(data);

                    if (garbage != null)
                    {
                        pkt = garbage + pkt;
                        garbage = null;
                    }

                    var chunks = pkt.Split('\x07');

                    foreach (var chunk in chunks)
                    {
                        if (chunk.EndsWith("\0"))
                        {
                            var p = BreakPacket(chunk);
                            HandlePacket(p);
                        }
                        else
                        {
                            garbage = chunk;
                        }
                    }
                }
            }
            catch (Exception E)
            {
                ChatMsg(E.ToString());
            }

            Array.Clear(buffer, 0, rcv);

            try
            {
                sock.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
            }
            catch (SocketException)
            {
                // Disconnect
                RTChatMsg("SERVER", "Disconnected.");
                // Quit?
            }
        }

        private void Send (Packet p)
        {
            try
            {
                sock.Send(p.Finalize());
            }
            catch (Exception E)
            {
                ChatMsg(E.ToString());
            }
        }

        private void HandlePacket (string[] pack)
        {
            switch (pack[0])
            {
                case "HANDSHAKE":
                    {
                        if (pack.Length < 2) break;
                        RTNotice("Connected to server version " + pack[1]);
                        Packet p = new Packet("HELLO");
                        p.AddChunk(name);
                        Send(p);
                        break;
                    }

                case "LOGIN":
                    {
                        if (pack.Length < 2) break;
                        // needed?
                        break;
                    }

                case "PING":
                    {
                        if (pack.Length < 2) break;
                        Packet p = new Packet("PONG");
                        p.AddChunk(pack[1]);
                        Send(p);
                        break;
                    }

                case "MESSAGE":
                    {
                        if (pack.Length < 3) break;
                        RTChatMsg(pack[1], pack[2]);
                        break;
                    }

                case "NOTICE":
                    {
                        if (pack.Length < 2) break;
                        RTNotice(pack[1]);
                        break;
                    }

                case "ERROR":
                    {
                        if (pack.Length < 2) break;
                        RTNotice("ERROR: " + pack[1]);
                        break;
                    }

                case "CHALLENGED":
                    {
                        if (pack.Length < 2) break;

                        var who = pack[1];
                        RTNotice("!! You've been challenged by " + who + "! Accept with /accept " + who);

                        break;
                    }

                case "STATE":
                    {
                        if (pack.Length < 3) break;

                        var who = pack[1];
                        var state = pack[2];
                        var desc = (state == "0" ? "idle" : state == "1" ? "in queue" : state == "2" ? "in game" : "undefined");

                        if (who.ToLower() == name.ToLower())
                            RTNotice("You are now " + desc);
                        else RTNotice(who + " is now " + desc);

                        break;
                    }

                case "GAME":
                    {
                        if (pack.Length < 2) break;

                        var who = pack[1];
                        InGame = true;

                        RTNotice("You're now in a game with " + who);

                        ClearTiles();

                        break;
                    }

                case "WON":
                case "LOST":
                case "DRAW":
                    {
                        var state = pack[0].ToLower();
                        DisableTiles();
                        if (state == "draw")
                            RTNotice("Your game resulted in a draw!");
                        else
                            RTNotice("You " + state + " your game!");
                        break;
                    }

                case "WAIT":
                    {
                        DisableTiles();
                        break;
                    }

                case "TURN":
                    {
                        EnableTiles();
                        break;
                    }

                case "MOVE":
                    {
                        if (pack.Length < 3) break;

                        byte tile = 0;
                        byte state = 0;

                        byte.TryParse(pack[1], out tile);
                        byte.TryParse(pack[2], out state);

                        string mark = state == 1 ? "X" : "O";

                        if (tile == 0)
                            Tile0.Text = mark;

                        else if (tile == 1)
                            Tile1.Text = mark;

                        else if (tile == 2)
                            Tile2.Text = mark;

                        else if (tile == 3)
                            Tile3.Text = mark;

                        else if (tile == 4)
                            Tile4.Text = mark;

                        else if (tile == 5)
                            Tile5.Text = mark;

                        else if (tile == 6)
                            Tile6.Text = mark;

                        else if (tile == 7)
                            Tile7.Text = mark;

                        else if (tile == 8)
                            Tile8.Text = mark;

                        break;
                    }

                case "LOBBYUSER":
                    {
                        if (pack.Length < 3) break;

                        var type = pack[1];
                        var who = pack[2];

                        if (type == "LEAVE")
                        {
                            RTNotice(who + " has left.");
                            userlist.Remove(who);
                        }
                        else if (type == "JOIN")
                        {
                            RTNotice(who + " has joined.");
                            userlist.Add(who);
                        }
                        else if (type == "ANNOUNCE")
                        {
                            userlist.Add(who);
                        }

                        UpdateUserList();
                        break;
                    }

                default:
                    ChatMsg("Unhandled packet type: " + pack[0]);
                    break;
            }
        }

        private string[] BreakPacket (string data)
        {
            if (!data.Contains('\0')) return null;
            return data.Split('\0');
        }

        private void ChatInput_KeyUp (object sender, KeyEventArgs e)
        {
            var data = ChatInput.Text.Replace("\r", "").Replace("\n", "");
            if ((e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter) && data.Length > 0)
            {
                ChatInput.Clear();

                if (data.StartsWith("/"))
                {
                    string[] args;
                    string cmd = data.Substring(1);

                    if (data.Contains(' '))
                    {
                        args = data.Substring(1).Split(' ');
                        cmd = args[0];
                    }
                    else args = new string[1] { cmd };

                    switch (cmd)
                    {
                        case "accept":
                            {
                                if (args.Length < 2)
                                {
                                    RTNotice("Invalid context. Use /accept challenger_name");
                                    return;
                                }

                                Packet p = new Packet("ACCEPT");
                                p.AddChunk(args[1]);
                                Send(p);
                                break;
                            }

                        case "challenge":
                            {
                                if (args.Length < 2)
                                {
                                    RTNotice("Invalid context. Use /challenge player_name");
                                    return;
                                }

                                Packet p = new Packet("CHALLENGE");
                                p.AddChunk(args[1]);
                                Send(p);
                                break;
                            }

                        case "play":
                            {
                                Packet p = new Packet("PLAY");
                                Send(p);
                                break;
                            }

                        case "idle":
                            {
                                Packet p = new Packet("IDLE");
                                Send(p);
                                break;
                            }

                        default:
                            RTNotice("Unhandled command: " + cmd);
                            break;
                    }
                }
                else
                {
                    Packet p = new Packet("MESSAGE");
                    p.AddChunk(data);
                    Send(p);
                }
            }
            e.Handled = true;
        }

        private void ChallengeButton_Click (object sender, EventArgs e)
        {
            lock (UserList)
            {
                var item = UserList.SelectedIndex;

                if (item > -1)
                {
                    var name = UserList.Items[item].ToString();

                    if (name != this.name)
                    {
                        Packet p = new Packet("CHALLENGE");
                        p.AddChunk(name);
                        Send(p);
                        UserList.ClearSelected();
                    }
                }
            }
        }

        private void Tile_Click (object sender, EventArgs e)
        {
            var tile = sender as Button;
            var id = tile.Name.Substring(4, 1);
            var p = new Packet("MARK");
            p.AddChunk(id);
            Send(p);
        }

        private void SendButton_Click (object sender, EventArgs e)
        {
            var data = ChatInput.Text.Replace("\r", "").Replace("\n", "");
            if (data.Length > 0)
            {
                ChatInput.Clear();
                Packet p = new Packet("MESSAGE");
                p.AddChunk(data);
                Send(p);
            }
        }
    }
}
