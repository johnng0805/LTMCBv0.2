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

namespace Đồ_án_môn_học_LTMCB
{
    public partial class UIServer : Form
    {
        private Socket serverSocket = null;
        private byte[] buffer = new byte[4096];
        private const int serverPort = 8000;
        private string logMsg = "";
        bool clientAccepted = false;

        struct ClientSocket
        {
            public string room;
            public string username;
            public Socket socket;
        }

        List<ClientSocket> clientList = new List<ClientSocket>();

        //ArrayList clientList;

        public UIServer()
        {
            //clientList = new ArrayList();
            InitializeComponent();
        }

        private void listenButton_Click(object sender, EventArgs e)
        {
            IPEndPoint serverIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort);

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(serverIP);
            serverSocket.Listen(20);
            serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);
        }

        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket socketState = serverSocket.EndAccept(ar);

                serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);
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
                Socket serverReceive = (Socket)ar.AsyncState;
                serverReceive.EndReceive(ar);

                Data receiveMsg = new Data(buffer);
                Data forwardMsg = new Data();

                forwardMsg.command = receiveMsg.command;
                forwardMsg.username = receiveMsg.username;
                forwardMsg.room = receiveMsg.room;
                forwardMsg.id = receiveMsg.id;

                switch (receiveMsg.command)
                {
                    case Command.Login:
                        int matched = 0;
                        if (clientList != null)
                        {
                            foreach (ClientSocket clientSck in clientList)
                            {
                                if (clientSck.username == receiveMsg.username)
                                {
                                    matched++;
                                    break;
                                }
                            }

                            if (matched < 1)
                            {
                                clientAccepted = true;

                                ClientSocket clientSocket = new ClientSocket();
                                clientSocket.username = receiveMsg.username;
                                clientSocket.socket = serverReceive;
                                clientSocket.room = "";
                                clientList.Add(clientSocket);

                                forwardMsg.command = Command.Accepted;
                                forwardMsg.content = "";
                                byte[] fwdAccepted = forwardMsg.ToByte();
                                serverReceive.BeginSend(fwdAccepted, 0, fwdAccepted.Length, SocketFlags.None, new AsyncCallback(OnSend), serverReceive);

                                logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} has just logged in";
                            }
                            else
                            {
                                logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: username taken. Declined connection...";
                                
                                forwardMsg.command = Command.Null;
                                forwardMsg.content = "";
                                byte[] fwdDecline = forwardMsg.ToByte();
                                serverReceive.BeginSend(fwdDecline, 0, fwdDecline.Length, SocketFlags.None, new AsyncCallback(OnSend), serverReceive);
                            }
                        }
                        else
                        {
                            ClientSocket clientSocket = new ClientSocket();
                            clientSocket.username = receiveMsg.username;
                            clientSocket.socket = serverReceive;
                            clientSocket.room = "";
                            clientList.Add(clientSocket);

                            forwardMsg.command = Command.Accepted;
                            forwardMsg.content = "";
                            byte[] fwdAccepted = forwardMsg.ToByte();
                            serverReceive.BeginSend(fwdAccepted, 0, fwdAccepted.Length, SocketFlags.None, new AsyncCallback(OnSend), serverReceive);

                            logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} has just logged in";
                        }

                        break;
                    case Command.Logout:
                        int index = 0;
                        foreach (ClientSocket clientSocket in clientList)
                        {
                            if (clientSocket.socket == serverSocket)
                            {
                                clientList.RemoveAt(index);
                            }
                            index++;
                        }
                        forwardMsg.content = $"<<<{forwardMsg.username} just logged out>>>";
                        serverSocket.Close();
                        break;
                    case Command.Join:
                        int count = 0;
                        int j = 0;

                        for (int i = 0; i < clientList.Count; i++)
                        {
                            if (clientList[i].room == receiveMsg.room)
                            {
                                count++;
                            }
                            if (clientList[i].username == receiveMsg.username)
                            {
                                j = i;
                            }
                        }
                        if (count == 1)
                        {
                            ClientSocket client = clientList[j];
                            client.room = receiveMsg.room;
                            clientList[j] = client;
                            logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} joined room {receiveMsg.room}";
                        }

                        for (int listIndex = 0; listIndex < listView1.Items.Count; listIndex++)
                        {
                            if (listView1.Items[listIndex].Text == receiveMsg.room)
                            {
                                listView1.Items[listIndex].SubItems.Add(receiveMsg.username);
                            }
                        }
                        break;
                    case Command.Create:
                        bool room_match = false;
                        for (int i = 0; i < clientList.Count; i++)
                        {
                            if (clientList[i].room == receiveMsg.room)
                            {
                                room_match = true;

                                Data roomMsg = new Data();
                                roomMsg.command = Command.RoomNo;
                                roomMsg.username = receiveMsg.username;
                                roomMsg.id = receiveMsg.id;
                                roomMsg.room = "";
                                roomMsg.content = "";

                                byte[] fwdRoom = roomMsg.ToByte();
                                serverReceive.BeginSend(fwdRoom, 0, fwdRoom.Length, SocketFlags.None, new AsyncCallback(OnSend), serverReceive);
                                logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} tried to create an existing room \"{receiveMsg.room}\"";
                                break;
                            }
                        }

                        if (room_match == false)
                        {
                            for (int i = 0; i < clientList.Count; i++)
                            {
                                if (clientList[i].username == receiveMsg.username)
                                {
                                    Data roomMsg = new Data();
                                    roomMsg.command = Command.RoomYes;
                                    roomMsg.username = receiveMsg.username;
                                    roomMsg.id = receiveMsg.id;
                                    roomMsg.room = receiveMsg.room;
                                    roomMsg.content = "";

                                    ClientSocket temp = clientList[i];
                                    temp.room = receiveMsg.room;
                                    clientList[i] = temp;

                                    byte[] fwdRoom = roomMsg.ToByte();
                                    serverReceive.BeginSend(fwdRoom, 0, fwdRoom.Length, SocketFlags.None, new AsyncCallback(OnSend), serverReceive);
                                }
                            }       

                            ListViewItem newPlayer = new ListViewItem(receiveMsg.room);
                            newPlayer.SubItems.Add(receiveMsg.username);
                            listView1.Items.Add(newPlayer);

                            logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} created room {receiveMsg.room}";
                            forwardMsg.content = $"<<<{receiveMsg.username} has just created a room {receiveMsg.room}>>>";
                        }
                        break;
                    case Command.Text:
                        forwardMsg.content = receiveMsg.content;
                        logMsg = $"{DateTime.Now.ToString("hh:mm:ss tt")}: {receiveMsg.username} says \"{receiveMsg.content}\" in room \"{receiveMsg.room}\"";
                        break;
                }

                if (forwardMsg.command != Command.Accepted && forwardMsg.command != Command.Null)
                {
                    byte[] message = forwardMsg.ToByte();
                    foreach (ClientSocket client in clientList)
                    {
                        if (client.room == receiveMsg.room && client.socket != serverReceive)
                        {
                            client.socket.BeginSend(message, 0, message.Length, SocketFlags.None,
                                new AsyncCallback(OnSend), client.socket);
                        }
                    }
                }

                //logBox.Text += $"{DateTime.Now.ToString("hh:mm:ss tt")}: command_{receiveMsg.command} username_{receiveMsg.username} room_{receiveMsg.room} ID_{receiveMsg.id} content_{receiveMsg.content}\n";
                logBox.Text += logMsg + "\n";

                if (clientAccepted)
                {
                    serverReceive.BeginReceive(buffer, 0, 4096, SocketFlags.None, new AsyncCallback(OnReceive), serverReceive);
                }
                else
                {
                    serverReceive.Close();
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

        private void btnSaveLogs_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();

        }
    }
}
