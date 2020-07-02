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
using System.Runtime.CompilerServices;
using System.Drawing.Drawing2D;
using System.Net.Security;

namespace Đồ_án_môn_học_LTMCB
{
    public partial class Form1 : Form
    {
        #region Global Variables
        private Socket clientSocket = null;     //Socket của client 
        private byte[] buffer = new byte[4096]; //buffer nhận dữ liệu
        private const int serverPort = 8000;
        bool connected = false;                 //Khi kết nối thành công sẽ chuyển thành true
        private bool createRoom = false;        //Khi tạo phòng thành công sẽ chuyển thành true
        private ChessBoardManager chessBoard;           //Bàn cờ
        private Point point;
        private string oponent = "";

        private List<List<Button>> Matrix;      //Matrix lưu tọa độ ô đã đánh 
        #endregion

        public Form1()
        {
            InitializeComponent();
            //DrawChessBoard();
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

        //Khi một ô cờ được đánh, Client sẽ gửi một thông điệp chứa tọa độ của ô đó đến Server
        private void Player_Marked(object sender, ButtonClickEvent e)
        {
            Data chessBtn = new Data();
            chessBtn.command = Command.Move;
            chessBtn.content = "";
            chessBtn.horizontal = e.ClickedPoint.X;
            chessBtn.vertical = e.ClickedPoint.Y;
            chessBtn.id = ID.Player;
            chessBtn.room = textPlayer2Name.Text;
            chessBtn.username = textPlayer1Name.Text;

            byte[] chessByte = chessBtn.ToByte();
            clientSocket.BeginSend(chessByte, 0, chessByte.Length, SocketFlags.None, new AsyncCallback(OnCheck), clientSocket);

            rtbMessage.Text += $"<<<{textPlayer1Name.Text} moved to ({e.ClickedPoint.X},{e.ClickedPoint.Y})>>>\n";
        }

        private void OnCheck(IAsyncResult ar)
        {
            Socket clientCheck = (Socket)ar.AsyncState;
            clientCheck.EndSend(ar);

            if (chessBoard.isEnd)
            {
                Data winnerMsg = new Data();
                winnerMsg.command = Command.Winner;
                winnerMsg.content = "";
                winnerMsg.horizontal = 0;
                winnerMsg.vertical = 0;
                winnerMsg.id = ID.Player;
                winnerMsg.room = textPlayer2Name.Text;
                winnerMsg.username = textPlayer1Name.Text;

                byte[] winnerByte = winnerMsg.ToByte();
                clientSocket.Send(winnerByte);
                rtbMessage.Text += $"<<<You've won!>>>\n";
            }

            pnlChessBoard.Enabled = false;
            clientCheck.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
        }
        private void EndGame()
        {
            pnlChessBoard.Enabled = false;
            string Winner = chessBoard.Winner;
            //MessageBox.Show(Winner);
        }

        private void ChessBoard_Endgame(object sender, EventArgs e)
        {
            EndGame();
        }

        //Khi nhất nút Login, client bắt đầu kết nối tới Server bằng socket của mình
        private void btnLogin_Click(object sender, EventArgs e)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(textIPServer.Text), serverPort);

            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.BeginConnect(endPoint, new AsyncCallback(OnConnect), null);
        }

        //Trong trạng thái connect thì Client sẽ gửi 1 thông điệp chứa username để Server kiểm tra 
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

        //Khi được Server xác nhận cho kết nối
        private void OnVerify(IAsyncResult ar)
        {
            clientSocket.EndSend(ar);
            connected = true;
            clientSocket.BeginReceive(buffer, 0, 4096, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);
        }

        //Khi ấn nút Create, tạo phòng 
        private void btnCreate_Click(object sender, EventArgs e)
        {
            Data createMsg = new Data();
            createMsg.command = Command.Create;
            createMsg.id = ID.Player;
            createMsg.room = textPlayer2Name.Text;
            createMsg.username = textPlayer1Name.Text;

            createRoom = true;

            btnSend.Enabled = true;
            byte[] createByte = createMsg.ToByte();
            clientSocket.BeginSend(createByte, 0, createByte.Length, SocketFlags.None, new AsyncCallback(OnCreateJoin), clientSocket);
        }

        //Sau khi tạo/vào phòng, client bắt đầu lắng nghe 
        private void OnCreateJoin(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            client.EndSend(ar);

            client.BeginReceive(buffer, 0, 4096, SocketFlags.None, new AsyncCallback(OnReceive), client);
        }

        //Khi ấn nút Send, gửi tin nhắn 
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

        //Hàm lắng nghe của Client, tương tự như Server nhưng bớt phức tạp hơn 
        private void OnReceive(IAsyncResult ar)
        {
            if (connected)
            {
                clientSocket.EndReceive(ar);

                string message = "";
                Data rcvMsg = new Data(buffer);

                try
                {
                    switch (rcvMsg.command)
                    {
                        #region Login
                        case Command.Login:
                            message = $"<<<{rcvMsg.username} has joined the room>>>";
                            break;
                        #endregion

                        #region Logout
                        case Command.Logout:
                            message = $"<<<{rcvMsg.username} has left the room>>>\n<<<You've won!>>>";
                            chessBoard.PlayerLogout(rcvMsg.username);
                            pnlChessBoard.Enabled = false;
                            break;
                        #endregion

                        #region Join
                        case Command.Join:
                            message = $"<<<{rcvMsg.username} has joined your room>>>";
                            //Player newPlayer = new Player(rcvMsg.username, pictureBox1.Image);
                            pictureBox1.Image = Image.FromFile(Application.StartupPath + "\\Resources\\Omark.png");
                            chessBoard.Add(rcvMsg.username, pictureBox1);
                            btnJoinRoom.Enabled = false;

                            oponent = rcvMsg.username;
                            break;
                        #endregion

                        #region JoinYes
                        case Command.JoinYes:
                            pictureBox1.Image = Image.FromFile(Application.StartupPath + "\\Resources\\Omark.png");
                            PictureBox temp = new PictureBox();
                            temp.Image = Image.FromFile(Application.StartupPath + "\\Resources\\Xmark.png");
                            chessBoard = new ChessBoardManager(pnlChessBoard, textPlayer1Name, temp);
                            chessBoard.Add(rcvMsg.username, pictureBox1);

                            if (InvokeRequired)
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    chessBoard.DrawChessBoard();
                                    chessBoard.PlayerMarked += Player_Marked;
                                    chessBoard.EndedGame += ChessBoard_Endgame;
                                });
                            }
                            message = $"<<<You have joined {rcvMsg.username}'s room>>>";
                            oponent = rcvMsg.username;
                            break;
                        #endregion

                        #region JoinNo
                        case Command.JoinNo:
                            message = $"<<<Room {rcvMsg.room} is full>>>";
                            btnSend.Enabled = false;
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
                        #endregion

                        #region Null
                        case Command.Null:
                            message = $"<<<Login unsuccessful. Username taken!>>>";
                            connected = false;
                            break;
                        #endregion

                        #region RoomYes
                        case Command.RoomYes:
                            message = $"<<<Room created>>>";
                            pictureBox1.Image = Image.FromFile(Application.StartupPath + "\\Resources\\Xmark.png");
                            chessBoard = new ChessBoardManager(pnlChessBoard, textPlayer1Name, pictureBox1);
                            if (InvokeRequired)
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    chessBoard.DrawChessBoard();
                                    chessBoard.PlayerMarked += Player_Marked;
                                    chessBoard.EndedGame += ChessBoard_Endgame;
                                });
                            }
                            btnCreate.Enabled = false;
                            break;
                        #endregion

                        #region RoomNo
                        case Command.RoomNo:
                            message = $"<<<Room already existed>>>";
                            break;
                        case Command.Move:
                            int vertical = rcvMsg.vertical;
                            int horizontal = rcvMsg.horizontal;
                            Point point = new Point(vertical, horizontal);

                            chessBoard.OtherPlayerMark(point);
                            pnlChessBoard.Enabled = true;

                            message = $"<<<{rcvMsg.username} moved to ({horizontal},{vertical})>>>";
                            break;
                        #endregion

                        #region Winner
                        case Command.Winner:
                            if (rcvMsg.username == textPlayer1Name.Text)
                            {
                                MessageBox.Show("Winner");
                            }
                            else
                            {
                                //MessageBox.Show("Loser");
                                message = $"<<<You've lost to {rcvMsg.username}>>>";
                                pnlChessBoard.Enabled = false;
                            }
                            break;
                            #endregion
                    }

                    clientSocket.BeginReceive(buffer, 0, 4096, SocketFlags.None, new AsyncCallback(OnReceive), null);
                    rtbMessage.Text += message + "\n";
                }
                catch (ObjectDisposedException obj)
                { }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Client Receive", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

        //Khi ấn nút Join, vào phòng 
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

        //private void btnChessClick(object sender, EventArgs e)
        //{
        //	MessageBox.Show("button" + pnlChessBoard.ToString());
        //} //Hàm này không chạy 

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //pnlChessBoard.Controls.Clear();
            chessBoard.DrawChessBoard();
            chessBoard.isEnd = false;
            pnlChessBoard.Enabled = true;
        }

        //Hàm xử lý tình trạng client ngắt kết nối 
        private void Client_Closing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to leave?", "Client", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
            else
            {
                try
                {
                    Data disconnectMsg = new Data();
                    disconnectMsg.command = Command.Logout;
                    disconnectMsg.id = ID.Player;
                    disconnectMsg.room = textPlayer2Name.Text;
                    disconnectMsg.username = textPlayer1Name.Text;
                    disconnectMsg.content = "";

                    byte[] disconnectByte = disconnectMsg.ToByte();
                    clientSocket.Send(disconnectByte, 0, disconnectByte.Length, SocketFlags.None);

                    connected = false;
                    rtbMessage.Dispose();
                    clientSocket.Close();
                }
                catch (ObjectDisposedException obj)
                { }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Client Closing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
