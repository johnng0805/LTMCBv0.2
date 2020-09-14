using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Net.Mail;
using System.Xml.Serialization;

namespace Đồ_án_môn_học_LTMCB
{
    public partial class UIServer : Form
    {
        #region Initialize Global Variables
        //Khởi tạo 1 socket toàn cục, server chỉ dùng mỗi socket này để lắng nghe / truyền mọi dữ liệu.
        private Socket serverSocket = null;
        //Thông điệp được gửi đi và nhận dưới dạng byte. Buffer dùng để chứa thông điệp đó. 
        private byte[] buffer = new byte[4096];
        private const int serverPort = 8000;
        //logMsg dùng để thông báo các thông điệp mà server nhận được trực quan hơn.
        private string logMsg = "";
        //clientAccepted dùng để cho Server biết là nên tiếp tục lắng nghe kết nối của client hay nên ngưng. Dùng trong trường hợp từ chối kết nối do username trùng.
        bool clientAccepted = false;
        #endregion

        #region ServerManager
        //1 struct đại diện cho 1 người dùng được Server chấp nhận kết nối.
        struct ClientSocket
        {
            private string room;
            private string username;
            private string mark;
            private Socket socket;

            public ClientSocket(string _username = "", string _room = "", string _mark = "", Socket _socket = null)
            {
                username = _username;
                room = _room;
                mark = _mark;
                socket = _socket;
            }
            public string Username
            {
                get { return username; }
                set { username = value; }
            }
            public string Room
            {
                get { return room; }
                set { room = value; }
            }
            public string Mark
            {
                get { return mark; }
                set { mark = value; }
            }
            public Socket Socket
            {
                get { return socket; }
                set { socket = value; }
            }
        }
        //Do có rất nhiều người dùng kết nối tới nên cần một danh sách để liên kết các struct với nhau.
        //Dùng để quản lý, truy vấn.
        List<ClientSocket> clientList = new List<ClientSocket>();
        #endregion

        //ArrayList clientList;

        public UIServer()
        {
            //clientList = new ArrayList();
            InitializeComponent();
        }

        private static string GetHostIP()
        {
            string myIP = "";
            string hostname = Dns.GetHostName();
            IPAddress[] myIPRange = Dns.GetHostAddresses(hostname);
            for (int i = 0; i < myIPRange.Length; i++)
            {
                if (myIPRange[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    myIP = myIPRange[i].ToString();
                }
            }
            return myIP;
        }

        //Hàm event khi nhấn nút Listen trên Winform 
        private void listenButton_Click(object sender, EventArgs e)
        { 
            IPEndPoint serverIP = new IPEndPoint(IPAddress.Parse(IPAddress.Any.ToString()), serverPort);

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(serverIP); //Bind dùng để gán địa chỉ IP EndPoint vào Socket. Tức khi có kết nối tới địa chỉ IP đó thì sẽ vào Socket.
            serverSocket.Listen(20); //Số 20 tức là Socket này sẽ lắng nghe tối đa 20 kết nối tới nó.
            serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);

            //MessageBox.Show("Listening on IP: " + serverIP.Address.ToString() + " port: " + serverPort.ToString() + "...", "Server", MessageBoxButtons.OK, MessageBoxIcon.Information);
            logBox.Text += $"Listening on IP: {serverIP.Address.ToString()} port: {serverPort.ToString()}...\n";
        }

        //Hàm chức năng khi Server accept kết nối thì sẽ làm gì tiếp theo.
        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket socketState = serverSocket.EndAccept(ar); //Kết thúc trạng thái accept
                //Bắt đầu accept kết nối khác (nếu có)
                serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);
                //Bắt đầu lắng nghe thông điệp của client đã accept.
                socketState.BeginReceive(buffer, 0, 4096, SocketFlags.None, new AsyncCallback(OnReceive), socketState);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                Socket serverReceive = (Socket)ar.AsyncState; //Tạo một thực thể socket ảo để lấy trạng thái của socket thật.
                serverReceive.EndReceive(ar); //Kết thúc trạng thái lắng nghe. Chương trình bắt đầu thực hiện nhiệm vụ bên dưới sau khi nó nhận được thông điệp.

                Data receiveMsg = new Data(buffer); //Tất cả dữ liệu chứa trong buffer đã được phân chia trong receiveMsg.
                Data forwardMsg = new Data(); //Khởi tạo thông điệp chuyển tiếp. Thông điệp rỗng.

                //Gán thông tin cần thiết để chuyển tiếp
                forwardMsg.command = receiveMsg.command;
                forwardMsg.username = receiveMsg.username;
                forwardMsg.room = receiveMsg.room;
                forwardMsg.id = receiveMsg.id;

                logMsg = "";
                switch (receiveMsg.command)
                {
                    #region Login
                    case Command.Login:
                        int matched = 0;
                        //Dò trong danh sách xem đã client mang username đó chưa.
                        if (clientList != null)
                        {
                            foreach (ClientSocket clientSck in clientList)
                            {
                                if (clientSck.Username == receiveMsg.username)
                                {
                                    matched++;
                                    break;
                                }
                            }
                            // Khi không có ai trùng username
                            if (matched < 1)
                            {
                                clientAccepted = true; //Chấp nhận kết nối.

                                ClientSocket clientSocket = new ClientSocket(receiveMsg.username, "", "", serverReceive); //Struct ClientSocket
                                clientList.Add(clientSocket); //Thêm vào danh sách

                                forwardMsg.command = Command.Accepted; //Chuyển ngược lại Client thông báo rằng đã chấp nhận kết nối.
                                forwardMsg.content = "";
                                
                                byte[] fwdAccepted = forwardMsg.ToByte();
                                serverReceive.BeginSend(fwdAccepted, 0, fwdAccepted.Length, SocketFlags.None, new AsyncCallback(OnSend), serverReceive);

                                logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} has just logged in";
                            }
                            else //Khi bị trùng tên 
                            {
                                logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: username taken. Declined connection...";
                                
                                forwardMsg.command = Command.Null;
                                forwardMsg.content = "";
                                
                                byte[] fwdDecline = forwardMsg.ToByte();
                                serverReceive.BeginSend(fwdDecline, 0, fwdDecline.Length, SocketFlags.None, new AsyncCallback(OnSend), serverReceive);
                            }
                        }
                        else //Khi chưa có ai kết nối tới
                        {
                            ClientSocket clientSocket = new ClientSocket(receiveMsg.username, receiveMsg.room, "", serverReceive);
                            clientList.Add(clientSocket);

                            forwardMsg.command = Command.Accepted;
                            forwardMsg.content = "";
                            
                            byte[] fwdAccepted = forwardMsg.ToByte();
                            serverReceive.BeginSend(fwdAccepted, 0, fwdAccepted.Length, SocketFlags.None, new AsyncCallback(OnSend), serverReceive);

                            logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} has just logged in";
                        }

                        break;
                    #endregion

                    #region Logout
                    case Command.Logout:
                        int index = 0;
                        foreach (ClientSocket client in clientList) //Dò trong danh sách client xem ai trùng tên thì xóa client đó khỏi danh sách.
                        {
                            if (client.Username == receiveMsg.username && client.Room == receiveMsg.room)
                            {
                                RemoveItemListView(receiveMsg.username, receiveMsg.room, receiveMsg.id);
                                clientList.RemoveAt(index);
                                break;
                            }
                            index++;
                        }
                        
                        forwardMsg.content = $"<<<{DateTime.Now.ToString("hh:mm:ss tt")}: {forwardMsg.username} just logged out>>>";
                        //serverSocket.Close();
                        clientAccepted = false; //Ngắt kết nối.
                        serverReceive.Shutdown(SocketShutdown.Both); //Ngưng socket của client.
                        logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} has just logged out of room \"{receiveMsg.room}\"";
                        //receiveMsg.room = "";
                        break;
                    #endregion

                    #region Join
                    case Command.Join:
                        int count = 0;
                        int j = 0;
                        //Dò qua danh sách để tìm xem có phòng đó chưa.
                        //Đồng thời xem coi phòng đó đã đầy người chơi chưa.
                        for (int i = 0; i < clientList.Count; i++)
                        {
                            if (clientList[i].Room == receiveMsg.room)
                            {
                                if (clientList[i].Username != receiveMsg.username)
                                {
                                    //switch (clientList[i].mark)
                                    //{
                                    //    case "X":
                                    //        joinMsg.content = "X";
                                    //        break;
                                    //    case "O":
                                    //        joinMsg.content = "O";
                                    //        break;
                                    //}
                                }
                                forwardMsg.username = clientList[i].Username;
                                count++;
                            }
                            if (clientList[i].Username == receiveMsg.username) //Do lúc Login mình đã add vào clientList nhưng room vẫn còn rỗng. Do đó cần cập nhật biến room lại 
                            {
                                j = i; //Lấy index của client đó để truy vấn. 
                            }
                        }
                        if (count == 1) //Khi phòng chưa đầy 
                        {
                            Data joinMsg = new Data(Command.JoinYes, receiveMsg);
                            joinMsg.username = forwardMsg.username;

                            ClientSocket client = clientList[j];
                            client.Room = receiveMsg.room;
                            clientList[j] = client;
                            logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} joined room {receiveMsg.room}";

                            byte[] joinByte = joinMsg.ToByte();
                            serverReceive.BeginSend(joinByte, 0, joinByte.Length, SocketFlags.None, new AsyncCallback(OnSend), serverReceive);
                            forwardMsg.username = receiveMsg.username;

                            //AddToXML(client);

                            AddItemListView(receiveMsg.username, receiveMsg.room, Command.Join, ID.Player);
                        }
                        else //Khi phòng đầy 
                        {
                            if (count < 1)
                            {
                                logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} requested for non-existing room.";
                            }
                            else
                            {
                                logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} requested room is full.";
                            }
                            Data joinMsg = new Data(Command.JoinNo, receiveMsg);
                            byte[] joinByte = joinMsg.ToByte();

                            serverReceive.BeginSend(joinByte, 0, joinByte.Length, SocketFlags.None, new AsyncCallback(OnSend), serverReceive);
                            receiveMsg.room = "";
                        }
                        
                        break;
                    #endregion

                    #region Create 
                    case Command.Create:
                        bool room_match = false;
                        //Dò xem phòng đó đã tồn tại chưa 
                        for (int i = 0; i < clientList.Count; i++)
                        {
                            if (clientList[i].Room == receiveMsg.room)
                            {
                                room_match = true;

                                Data roomMsg = new Data(Command.RoomNo, receiveMsg);

                                byte[] fwdRoom = roomMsg.ToByte();
                                //Gửi lại client đó là phòng đã tồn tại.
                                serverReceive.BeginSend(fwdRoom, 0, fwdRoom.Length, SocketFlags.None, new AsyncCallback(OnSend), serverReceive);
                                logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} tried to create an existing room \"{receiveMsg.room}\"";
                                break;
                            }
                        }
                        //Khi không bị trùng tên phòng 
                        if (room_match == false) 
                        {
                            for (int i = 0; i < clientList.Count; i++)
                            {
                                if (clientList[i].Username == receiveMsg.username) //Duyệt qua danh sách để cập nhật biến room cho client đó. 
                                {
                                    Data roomMsg = new Data(Command.RoomYes, receiveMsg);

                                    ClientSocket temp = clientList[i];
                                    temp.Room = receiveMsg.room;
                                    temp.Mark = "X";
                                    clientList[i] = temp;

                                    //AddToXML(temp);

                                    byte[] fwdRoom = roomMsg.ToByte();
                                    serverReceive.BeginSend(fwdRoom, 0, fwdRoom.Length, SocketFlags.None, new AsyncCallback(OnSend), serverReceive);
                                }
                            }

                            AddItemListView(receiveMsg.username, receiveMsg.room, Command.Create, ID.Player);

                            logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} created room {receiveMsg.room}";
                            forwardMsg.content = $"<<<{receiveMsg.username} has just created a room {receiveMsg.room}>>>";
                        }
                        break;
                    #endregion

                    #region Text
                    case Command.Text:
                        forwardMsg.content = receiveMsg.content;
                        logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} says \"{receiveMsg.content}\" in room \"{receiveMsg.room}\"";
                        break;
                    #endregion

                    #region Move
                    case Command.Move:
                        forwardMsg.content = receiveMsg.content;
                        int x = receiveMsg.horizontal;
                        int y = receiveMsg.vertical;
                        forwardMsg.horizontal = receiveMsg.horizontal;
                        forwardMsg.vertical = receiveMsg.vertical;
                        logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} moves to x: {x.ToString()} y: {y.ToString()}";

                        break;
                    #endregion

                    #region Winner
                    case Command.Winner:
                        forwardMsg.content = receiveMsg.content;
                        forwardMsg.horizontal = receiveMsg.horizontal;
                        forwardMsg.vertical = receiveMsg.vertical;
                        logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} wins.";
                        break;
                    #endregion

                    #region Timer
                    case Command.Timer:
                        forwardMsg.content = receiveMsg.content;
                        forwardMsg.horizontal = receiveMsg.horizontal;
                        forwardMsg.vertical = receiveMsg.vertical;
                        logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} has lost due to timer ran out.";
                        break;
                    #endregion

                    #region NewGame
                    case Command.NewGame:
                        logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} started a new game.";
                        forwardMsg.content = receiveMsg.content;
                        forwardMsg.horizontal = receiveMsg.horizontal;
                        forwardMsg.vertical = receiveMsg.vertical;
                        break;
                    #endregion
                }

                if (forwardMsg.command != Command.Accepted && forwardMsg.command != Command.Null)
                {
                    byte[] message = forwardMsg.ToByte();
                    foreach (ClientSocket client in clientList)
                    {
                        if (client.Room == receiveMsg.room && client.Socket != serverReceive)
                        {
                            client.Socket.BeginSend(message, 0, message.Length, SocketFlags.None,
                                new AsyncCallback(OnSend), client.Socket);
                        }
                    }
                }

                //logBox.Text += $"{DateTime.Now.ToString("hh:mm:ss tt")}: command_{receiveMsg.command} username_{receiveMsg.username} room_{receiveMsg.room} ID_{receiveMsg.id} content_{receiveMsg.content}\n";
                if (InvokeRequired)
                {
                    this.BeginInvoke((MethodInvoker)delegate ()
                    {
                        logBox.Text += logMsg + "\n";
                    });
                }

                if (clientAccepted)
                {
                    serverReceive.BeginReceive(buffer, 0, 4096, SocketFlags.None, new AsyncCallback(OnReceive), serverReceive);
                }
                else
                {
                    serverReceive.Shutdown(SocketShutdown.Both);
                    serverReceive.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnSend(IAsyncResult ar)
        {
            try
            {
                Socket serverSend = (Socket)ar.AsyncState;
                serverSend.EndSend(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Save log xuống file .rtf 
        private void btnSaveLogs_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Rich Text Format|*.rtf";
            saveFile.DefaultExt = "*.rtf";
            saveFile.AddExtension = true;
            saveFile.InitialDirectory = Application.StartupPath.ToString();
            
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                logBox.SaveFile(saveFile.FileName);
            }   
        }

        private void AddToXML(ClientSocket cs)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ClientSocket));
            using (FileStream fs = new FileStream(Application.StartupPath + "Data.xml", FileMode.Create))
            {
                xs.Serialize(fs, cs);
            }
        }

        private void RemoveItemListView(string username, string room, ID playerID)
        {
            for (int listIndex = 0; listIndex < listView1.Items.Count; listIndex++)
            {
                int count = 0;
                if (listView1.Items[listIndex].Text == room)
                {
                    for (int subIndex = 0; subIndex < listView1.Items[listIndex].SubItems.Count; subIndex++)
                    {
                        if (listView1.Items[listIndex].SubItems[subIndex].Text == username)
                        {
                            ListViewItem item = listView1.Items[listIndex];
                            item.SubItems[subIndex].Text = "";
                            listView1.Items[listIndex] = item;
                        }
                        if (listView1.Items[listIndex].SubItems[subIndex].Text == "")
                        {
                            count++;
                        }
                    }
                    if (count >= 3)
                    {
                        listView1.Items.RemoveAt(listIndex);
                        return;
                    }
                }
            }
             
        }

        private void AddItemListView(string username, string room, Command command, ID playerID)
        {
            switch (command)
            {
                case Command.Create:
                    ListViewItem newPlayer = new ListViewItem(room);
                    if (playerID == ID.Player)
                    {
                        newPlayer.SubItems.Add(username);
                        newPlayer.SubItems.Add("");
                        newPlayer.SubItems.Add("");
                    }
                    else if (playerID == ID.Spectator)
                    {
                        newPlayer.SubItems.Add("");
                        newPlayer.SubItems.Add(username);
                        newPlayer.SubItems.Add("");
                    }
                    listView1.Items.Add(newPlayer);
                    break;
                case Command.Join:
                    for (int listIndex = 0; listIndex < listView1.Items.Count; listIndex++)
                    {
                        if (listView1.Items[listIndex].Text == room)
                        {
                            for (int subIndex = 1; subIndex < listView1.Items[listIndex].SubItems.Count; subIndex++)
                            {
                                if (playerID == ID.Player)
                                {
                                    if (listView1.Items[listIndex].SubItems[subIndex].Text == "")
                                    {
                                        ListViewItem joinPlayer = listView1.Items[listIndex];
                                        joinPlayer.SubItems[subIndex].Text = username;
                                        listView1.Items[listIndex] = joinPlayer;
                                        break;
                                    }
                                }
                                else if (playerID == ID.Spectator)
                                {
                                    ListViewItem specPlayer = listView1.Items[listIndex];
                                    specPlayer.SubItems[4].Text = username;
                                    listView1.Items[listIndex] = specPlayer;
                                    break;
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void logBox_TextChanged(object sender, EventArgs e)
        {
            logBox.SelectionStart = logBox.Text.Length;
            logBox.ScrollToCaret();
        }

        private void UIServer_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }

        private void UIServer_Closing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
