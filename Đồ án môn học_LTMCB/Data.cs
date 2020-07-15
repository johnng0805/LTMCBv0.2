using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Đồ_án_môn_học_LTMCB
{

    public enum Command
    {
        Login,  //Client sẽ gửi command này khi bấm nút Login
        Logout, //Client sẽ gửi command này khi bấm nút Logout
        Text,   // Client sẽ gửi command này khi nhắn tin
        Move,   //Client sẽ gửi command này khi có nước đi
        Null, 
        Join,   //Client sẽ gửi command này khi bấm nút Join
        JoinYes,//Server sẽ gửi command này khi chấp nhận cho vô phòng
        JoinNo, //Server sẽ gửi command này khi không chấp nhận vô phòng
        Create, //Client sẽ gửi command này khi ấn nút create
        Accepted, //Server sẽ gửi command này khi chấp nhận Login (không bị trùng username)
        RoomYes, //Server sẽ gửi command này khi chấp nhận cho tạo phòng
        RoomNo,  //Server sẽ gửi command này khi không chấp nhận cho tạo phòng 
        Winner,
        Timer,
        NewGame,
    }

    public enum ID
    {
        Player,
        Spectator,
        Null,
    }

    class Data
    {
        public string username;
        public string content;
        public string room;
        public ID id;
        public Command command;
        public int horizontal; //Tọa độ x
        public int vertical; //Tọa độ y

        public Data()
        {
            username = content = "";
            id = ID.Null;
            command = Command.Null;
        }

        public Data(Command _command, Data _recv)
        {
            switch (_command)
            {
                case Command.JoinYes:
                    this.command = _command;
                    this.username = _recv.username;
                    this.room = _recv.room;
                    this.horizontal = 0;
                    this.vertical = 0;
                    this.id = _recv.id;
                    this.content = "";
                    break;
                case Command.JoinNo:
                    this.command = _command;
                    this.username = _recv.username;
                    this.room = _recv.room;
                    this.horizontal = 0;
                    this.vertical = 0;
                    this.id = _recv.id;
                    this.content = "";
                    break;
                case Command.RoomYes:
                    this.command = _command;
                    this.username = _recv.username;
                    this.room = _recv.room;
                    this.horizontal = 0;
                    this.vertical = 0;
                    this.id = _recv.id;
                    this.content = "";
                    break;
                case Command.RoomNo:
                    this.command = _command;
                    this.username = _recv.username;
                    this.room = "";
                    this.horizontal = 0;
                    this.vertical = 0;
                    this.id = _recv.id;
                    this.content = "";
                    break;
                case Command.Winner:
                    this.command = _command;
                    this.username = _recv.username;
                    this.horizontal = 0;
                    this.vertical = 0;
                    this.id = _recv.id;
                    this.room = _recv.room;
                    this.content = "";
                    break;
            }
        }

        public Data(byte[] buffer) //Hàm phân chia buffer nhận được vào các thuộc tính tương ứng của class 
        {
            string _command = Encoding.UTF8.GetString(buffer, 0, 256).Replace("\0", "");
            command = (Command)Enum.Parse(typeof(Command), _command);

            username = Encoding.UTF8.GetString(buffer, 256, 256).Replace("\0", "");
            room = Encoding.UTF8.GetString(buffer, 512, 256).Replace("\0", "");

            string _id = Encoding.UTF8.GetString(buffer, 768, 256).Replace("\0", "");
            id = (ID)Enum.Parse(typeof(ID), _id);

            if (command == Command.Text)
            {
                content = Encoding.UTF8.GetString(buffer, 1024, 3072).Replace("\0", "");
            }
            else if (command == Command.Move)
            {
                horizontal = Int32.Parse(Encoding.UTF8.GetString(buffer, 1024, 1536).Replace("\0", ""));
                vertical = Int32.Parse(Encoding.UTF8.GetString(buffer, 2560, 1536).Replace("\0", ""));
                content = "";
            }
            else
            {
                content = "";
            }
        }

        public byte[] ToByte() //Hàm padding, chuyển đổi các thuộc tính của class vô một mảng byte để chuyển đi 
        {
            byte[] result = new byte[4096];

            string _command = command.ToString();
            for (int i = _command.Length; i < 256; i++)
            {
                _command += "\0";
            }

            string _username = username;
            for (int i = username.Length; i < 256; i++)
            {
                _username += "\0";
            }

            string _room = room;
            for (int i = room.Length; i < 256; i++)
            {
                _room += "\0";
            }

            string _id = id.ToString();
            for (int i = _id.Length; i < 256; i++)
            {
                _id += "\0";
            }

            string _content = content;
            string _horizontal = horizontal.ToString();
            string _vertical = vertical.ToString();

            if (command == Command.Text)
            {
                
                for (int i = _content.Length; i < 3072; i++)
                {
                    _content += "\0";
                }
            }
            else if (command == Command.Move)
            {
                for (int i = _horizontal.Length; i < 1536; i++)
                {
                    _horizontal += "\0";
                }
                for (int i = _vertical.Length; i < 1536; i++)
                {
                    _vertical += "\0";
                }
                _content = _horizontal + _vertical;
            }

            string _result = $"{_command}{_username}{_room}{_id}{_content}";
            result = Encoding.UTF8.GetBytes(_result);

            return result;
        }
    }
}
