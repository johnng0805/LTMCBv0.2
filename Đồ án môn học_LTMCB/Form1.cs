using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace Đồ_án_môn_học_LTMCB
{
    public partial class Form1 : Form
    {
        private Socket clientSocket = null;
        private byte[] buffer = new byte[4096];
        private const int serverPort = 8000;
        bool connected = true;

        public Form1()
        {
            InitializeComponent();

            DrawChessBoard();
        }

        private delegate void SafeCallThread(string text);

        private void UpdateThreadSafe(string text)
        {
            if (rtbMessage.InvokeRequired)
            {
                var d = new SafeCallThread(UpdateThreadSafe);
                rtbMessage.Invoke(d, new object[] { text });
            }
            else
            {
                rtbMessage.Text += text + "\n";
            }
        }

        void DrawChessBoard()
        {
            Button oldButton = new Button() { Width = 0, Location = new Point(0, 0) };
            for (int i = 0; i < Info.CHESS_BOARD_HEIGHT; i++)
            {
                for (int j = 0; j < Info.CHESS_BOARD_WIDTH; j++)
                {
                    Button btn = new Button()
                    {
                        Width = Info.CHESS_WIDTH,
                        Height = Info.CHESS_HEIGHT,
                        Location = new Point(oldButton.Location.X + oldButton.Width, oldButton.Location.Y)
                    };

                    pnlChessBoard.Controls.Add(btn);

                    oldButton = btn;
                }
                oldButton.Location = new Point(0, oldButton.Location.Y + Info.CHESS_HEIGHT);
                oldButton.Width = 0;
                oldButton.Height = 0;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(textIPServer.Text), serverPort);

            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.BeginConnect(endPoint, new AsyncCallback(OnConnect), null);
        }

        private void OnConnect(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndConnect(ar);

                Data loginMessage = new Data();
                loginMessage.command = Command.Login;
                loginMessage.id = ID.Player;
                loginMessage.content = "";
                loginMessage.username = textPlayer1Name.Text;
                loginMessage.room = "";

                byte[] byteMessage = loginMessage.ToByte();
                clientSocket.BeginSend(byteMessage, 0, byteMessage.Length, SocketFlags.None, new AsyncCallback(OnVerify), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnVerify(IAsyncResult ar)
        {
            clientSocket.EndSend(ar);
            clientSocket.BeginReceive(buffer, 0, 4096, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            Data createMsg = new Data();
            createMsg.command = Command.Create;
            createMsg.id = ID.Player;
            createMsg.room = textPlayer2Name.Text;
            createMsg.username = textPlayer1Name.Text;

            btnSend.Enabled = true;
            byte[] createByte = createMsg.ToByte();
            clientSocket.BeginSend(createByte, 0, createByte.Length, SocketFlags.None, new AsyncCallback(OnCreateJoin), clientSocket);
        }

        private void OnCreateJoin(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            client.EndSend(ar);

            client.BeginReceive(buffer, 0, 4096, SocketFlags.None, new AsyncCallback(OnReceive), client);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                Data sendMsg = new Data();
                sendMsg.command = Command.Text;
                sendMsg.id = ID.Player;
                sendMsg.room = textPlayer2Name.Text;
                sendMsg.content = textSendMessage.Text;
                sendMsg.username = textPlayer1Name.Text;

                byte[] sendByte = sendMsg.ToByte();
                clientSocket.BeginSend(sendByte, 0, sendByte.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnSend(IAsyncResult ar)
        {
            clientSocket.EndSend(ar);
            rtbMessage.Text += $"{textPlayer1Name.Text}: {textSendMessage.Text}\n";
            textSendMessage.Clear();
        }

   

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                Socket clientRcv = (Socket)ar.AsyncState;
                clientRcv.EndReceive(ar);
                string message = "";
                Data rcvMsg = new Data(buffer);
                
                switch (rcvMsg.command)
                {
                    case Command.Login:
                        message = $"<<<{rcvMsg.username} has joined the room>>>";
                        break;
                    case Command.Logout:
                        message = $"<<<{rcvMsg.username} has left the room>>>";
                        break;
                    case Command.Join:
                        message = $"<<<{rcvMsg.username} has joined your room>>>";
                        btnJoinRoom.Enabled = false;
                        break;
                    case Command.Text:
                        message = $"{rcvMsg.username}: {rcvMsg.content}";
                        break;
                    case Command.Accepted:
                        message = $"<<<Login successful>>>";
                        connected = true;
                        btnLogin.Enabled = false;
                        btnCreate.Enabled = true;
                        btnJoinRoom.Enabled = true;
                        btnWatch.Enabled = true;
                        break;
                    case Command.Null:
                        message = $"<<<Login unsuccessful. Username taken!>>>";
                        break;
                    case Command.RoomYes:
                        message = $"<<<Room created>>>";
                        btnCreate.Enabled = false;
                        break;
                    case Command.RoomNo:
                        message = $"<<<Room already existed>>>";
                        break;
                }

                rtbMessage.Text += message + "\n";

                if (connected && rcvMsg.command != Command.Accepted)
                {
                    clientRcv.BeginReceive(buffer, 0, 4096, SocketFlags.None, new AsyncCallback(OnReceive), clientRcv);
                }
                else if (!connected)
                {
                    clientRcv.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Client_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            btnCreate.Enabled = false;
            btnSend.Enabled = false;
            btnWatch.Enabled = false;
            btnJoinRoom.Enabled = false;
        }

        private void btnJoinRoom_Click(object sender, EventArgs e)
        {
            try
            {
                Data joinMsg = new Data();
                joinMsg.command = Command.Join;
                joinMsg.id = ID.Player;
                joinMsg.room = textPlayer2Name.Text;
                joinMsg.content = "";
                joinMsg.username = textPlayer1Name.Text;

                btnSend.Enabled = true;
                byte[] joinByte = joinMsg.ToByte();
                clientSocket.BeginSend(joinByte, 0, joinByte.Length, SocketFlags.None, new AsyncCallback(OnCreateJoin), clientSocket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
