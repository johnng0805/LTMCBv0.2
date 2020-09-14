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
    public partial class Client : Form
    {
        #region Global Variables
        private Socket clientSocket = null;     
        private byte[] buffer = new byte[4096]; 
        private const int serverPort = 8000;
        bool connected = false;                 
        private bool createRoom = false;        
        private ChessBoardManager chessBoard;           
        private Point point;
        private string oponent = "";

        private List<List<Button>> Matrix;      //Matrix lưu tọa độ ô đã đánh 
        #endregion

        public Client()
        {
            InitializeComponent();
            //DrawChessBoard();
        }

        private string serverIP = "";

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

        //This function will get the coordinates of player's move and send it to the Server. (I think)
        private void Player_Marked(object sender, ButtonClickEvent e)
        {
            timer1.Stop();
            timer1.Enabled = false;
            progressBCoolDown.Value = 0;

            Data chessBtn = new Data();
            chessBtn.command = Command.Move;
            chessBtn.content = "";
            chessBtn.horizontal = e.ClickedPoint.X;
            chessBtn.vertical = e.ClickedPoint.Y;
            chessBtn.id = ID.Player;
            chessBtn.room = roomName.Text;
            chessBtn.username = userName.Text;

            byte[] chessByte = chessBtn.ToByte();
            clientSocket.BeginSend(chessByte, 0, chessByte.Length, SocketFlags.None, new AsyncCallback(OnCheck), clientSocket);

            rtbMessage.Text += $"<<<{userName.Text} moved to ({e.ClickedPoint.X},{e.ClickedPoint.Y})>>>\n";
        }

        //Check for winner function.
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
                winnerMsg.room = roomName.Text;
                winnerMsg.username = userName.Text;

                byte[] winnerByte = winnerMsg.ToByte();
                clientSocket.Send(winnerByte);
                rtbMessage.Text += $"<<<You've won!>>>\n";
            }

            pnlChessBoard.Enabled = false;
            clientCheck.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
        }
        private void EndGame()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate ()
               {
                   timer1.Stop();
                   timer1.Enabled = false;
                   progressBCoolDown.Value = 0;
               });
            }
         
            pnlChessBoard.Enabled = false;
            string Winner = chessBoard.Winner;
            //MessageBox.Show(Winner);
        }

        private void ChessBoard_Endgame(object sender, EventArgs e)
        {
            EndGame();
        }

        //I don't think I need to explain here xd.
        private void btnLogin_Click(object sender, EventArgs e)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(textIPServer.Text), serverPort);

            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.BeginConnect(endPoint, new AsyncCallback(OnConnect), null);
        }

        //On "connection request" state, the Client will send a message contains only the username.
        private void OnConnect(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndConnect(ar);

                Data loginMessage = new Data();
                loginMessage.command = Command.Login;
                loginMessage.id = ID.Player;
                loginMessage.content = "";
                loginMessage.username = userName.Text;
                loginMessage.room = "";

                byte[] byteMessage = loginMessage.ToByte();
                clientSocket.BeginSend(byteMessage, 0, byteMessage.Length, SocketFlags.None, new AsyncCallback(OnVerify), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //When approved by the Server.
        private void OnVerify(IAsyncResult ar)
        {
            clientSocket.EndSend(ar);
            connected = true;
            clientSocket.BeginReceive(buffer, 0, 4096, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);
        }

        //Just read the name lol
        private void btnCreate_Click(object sender, EventArgs e)
        {
            Data createMsg = new Data();
            createMsg.command = Command.Create;
            createMsg.id = ID.Player;
            createMsg.room = roomName.Text;
            createMsg.username = userName.Text;

            createRoom = true;

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
                sendMsg.room = roomName.Text;
                sendMsg.content = textSendMessage.Text;
                sendMsg.username = userName.Text;

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
            if (InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate ()
                {
                    rtbMessage.Text += $"{userName.Text}: {textSendMessage.Text}\n";
                    textSendMessage.Clear();
                });
            }  
        }

        //Mostly the same on Server's OnReceive with a few additional cases. I'm too lazy to translate all this again lmao.
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
                            if (chessBoard != null)
                            {
                                chessBoard.PlayerLogout(rcvMsg.username);
                            }
                            pnlChessBoard.Enabled = false;
                            if (InvokeRequired)
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    timer1.Stop();
                                    progressBCoolDown.Value = 0;
                                    pnlChessBoard.Enabled = false;
                                });
                            }
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
                            chessBoard = new ChessBoardManager(pnlChessBoard, userName, temp);
                            chessBoard.Add(rcvMsg.username, pictureBox1);

                            if (InvokeRequired)
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    chessBoard.DrawChessBoard();
                                    chessBoard.PlayerMarked += Player_Marked;
                                    chessBoard.EndedGame += ChessBoard_Endgame;

                                    progressBCoolDown.Step = Info.COOL_DOWN_STEP;
                                    progressBCoolDown.Maximum = Info.COOL_DOWN_TIME;
                                    progressBCoolDown.Value = 0;

                                    timer1.Interval = Info.COOL_DOWN_INTERVAL;
                                });
                            }
                            message = $"<<<You have joined {rcvMsg.username}'s room>>>";
                            oponent = rcvMsg.username;
                            break;
                        #endregion

                        #region JoinNo
                        case Command.JoinNo:
                            message = $"<<<Room {rcvMsg.room} is full or not found>>>";
                            btnSend.Enabled = false;
                            break;
                        #endregion

                        #region Text
                        case Command.Text:
                            message = $"{rcvMsg.username}: {rcvMsg.content}";
                            break;
                        #endregion

                        #region Accepted
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
                            chessBoard = new ChessBoardManager(pnlChessBoard, userName, pictureBox1);
                            if (InvokeRequired)
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    chessBoard.DrawChessBoard();
                                    chessBoard.PlayerMarked += Player_Marked;
                                    chessBoard.EndedGame += ChessBoard_Endgame;

                                    progressBCoolDown.Step = Info.COOL_DOWN_STEP;
                                    progressBCoolDown.Maximum = Info.COOL_DOWN_TIME;
                                    progressBCoolDown.Value = 0;

                                    timer1.Interval = Info.COOL_DOWN_INTERVAL;
                                });
                            }
                            btnCreate.Enabled = false;
                            break;
                        #endregion

                        #region RoomNo
                        case Command.RoomNo:
                            message = $"<<<Room already existed>>>";
                            break;
                        #endregion

                        #region Move
                        case Command.Move:
                            int vertical = rcvMsg.vertical;
                            int horizontal = rcvMsg.horizontal;
                            Point point = new Point(vertical, horizontal);

                            chessBoard.OtherPlayerMark(point);
                            pnlChessBoard.Enabled = true;

                            if (InvokeRequired)
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    progressBCoolDown.Value = 0;
                                    timer1.Enabled = true;
                                    timer1.Start();
                                });
                            }

                            message = $"<<<{rcvMsg.username} moved to ({horizontal},{vertical})>>>";
                            break;
                        #endregion

                        #region Winner
                        case Command.Winner:
                            if (rcvMsg.username == userName.Text)
                            {
                                MessageBox.Show("Winner");
                            }
                            else
                            {
                                //MessageBox.Show("Loser");
                                message = $"<<<You've lost to {rcvMsg.username}>>>";
                                pnlChessBoard.Enabled = false;

                                timer1.Stop();
                                timer1.Enabled = false;
                                progressBCoolDown.Value = 0;
                            }
                            break;
                        #endregion

                        #region NewGame
                        case Command.NewGame:
                            message = $"<<<{rcvMsg.username} started a new game>>>";

                            if (InvokeRequired)
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    chessBoard.DrawChessBoard();
                                    chessBoard.isEnd = false;
                                    pnlChessBoard.Enabled = true;
                                    timer1.Enabled = false;
                                    progressBCoolDown.Value = 0;
                                });
                            }

                            break;
                        #endregion

                        #region Timer
                        case Command.Timer:
                            if (rcvMsg.username != userName.Text)
                            {
                                message += $"<<<{rcvMsg.username} ran out of time. You've won!>>>";
                                if (InvokeRequired)
                                {
                                    this.BeginInvoke((MethodInvoker)delegate ()
                                    {
                                        timer1.Stop();
                                        progressBCoolDown.Value = 0;
                                        pnlChessBoard.Enabled = false;
                                    });
                                }
                            }
                            break;
                        #endregion
                    }

                    clientSocket.BeginReceive(buffer, 0, 4096, SocketFlags.None, new AsyncCallback(OnReceive), null);
                    if (InvokeRequired)
                    {
                        this.BeginInvoke((MethodInvoker)delegate ()
                        {
                            rtbMessage.Text += message + "\n";
                        });
                    }
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

        private void btnJoinRoom_Click(object sender, EventArgs e)
        {
            try
            {
                Data joinMsg = new Data();
                joinMsg.command = Command.Join;
                joinMsg.id = ID.Player;
                joinMsg.room = roomName.Text;
                joinMsg.content = "";
                joinMsg.username = userName.Text;

                btnSend.Enabled = true;
                byte[] joinByte = joinMsg.ToByte();

                clientSocket.BeginSend(joinByte, 0, joinByte.Length, SocketFlags.None, new AsyncCallback(OnCreateJoin), clientSocket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Client", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } 

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //pnlChessBoard.Controls.Clear();
            Data newGame = new Data();
            newGame.username = userName.Text;
            newGame.command = Command.NewGame;
            newGame.id = ID.Player;
            newGame.room = roomName.Text;
            newGame.content = "";
            newGame.horizontal = 0;
            newGame.vertical = 0;

            byte[] newGameByte = newGame.ToByte();
            clientSocket.Send(newGameByte);

            chessBoard.DrawChessBoard();
            chessBoard.isEnd = false;
            pnlChessBoard.Enabled = true;
            timer1.Enabled = false;
            progressBCoolDown.Value = 0;

            rtbMessage.Text += $"<<<You started a new game>>>\n";
        }

        private void Client_Closing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to leave?", "Client", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
            else
            {
                if (connected)
                {
                    try
                    {
                        Data disconnectMsg = new Data();
                        disconnectMsg.command = Command.Logout;
                        disconnectMsg.id = ID.Player;
                        disconnectMsg.room = roomName.Text;
                        disconnectMsg.username = userName.Text;
                        disconnectMsg.content = "";

                        byte[] disconnectByte = disconnectMsg.ToByte();
                        clientSocket.Send(disconnectByte, 0, disconnectByte.Length, SocketFlags.None);

                        this.

                        connected = false;
                        clientSocket.Close();
                        clientSocket = null;
                        e.Cancel = true;
                        this.Hide();
                        return;
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

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            progressBCoolDown.PerformStep();

            if (progressBCoolDown.Value >= progressBCoolDown.Maximum)
            {
                EndGame();
                Data timerEnd = new Data();
                timerEnd.command = Command.Timer;
                timerEnd.username = userName.Text;
                timerEnd.id = ID.Player;
                timerEnd.content = "";
                timerEnd.horizontal = 0;
                timerEnd.vertical = 0;
                timerEnd.room = roomName.Text;

                byte[] timerByte = timerEnd.ToByte();

                clientSocket.Send(timerByte);

                timer1.Stop();
                rtbMessage.Text += $"<<<Timer ran out. You've lost!>>>\n";
            }
        }

        private void rtbMessage_TextChanged(object sender, EventArgs e)
        {
            rtbMessage.SelectionStart = rtbMessage.Text.Length;
            rtbMessage.ScrollToCaret();
        }

        private void btnWatch_Click(object sender, EventArgs e)
        {
            Data watchData = new Data();
            watchData.command = Command.Spectate;
            watchData.id = ID.Spectator;
            watchData.username = userName.Text;
            watchData.room = roomName.Text;
            watchData.vertical = 0;
            watchData.horizontal = 0;

            byte[] watchByte = watchData.ToByte();
            clientSocket.Send(watchByte);
            connected = true;
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);
        }
    }
}
