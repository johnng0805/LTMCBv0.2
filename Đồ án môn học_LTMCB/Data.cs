using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Đồ_án_môn_học_LTMCB
{

    public enum Command
    {
        Login,
        Logout,
        Text,
        Move,
        Null,
        Join,
        Create,
        Accepted,
        RoomYes,
        RoomNo,
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

        public Data()
        {
            username = content = "";
            id = ID.Null;
            command = Command.Null;
        }

        public Data(byte[] buffer)
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
            else
            {
                content = "";
            }
        }

        public byte[] ToByte()
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
            if (command == Command.Text)
            {
                
                for (int i = _content.Length; i < 3072; i++)
                {
                    _content += "\0";
                }
            }

            string _result = $"{_command}{_username}{_room}{_id}{_content}";
            result = Encoding.UTF8.GetBytes(_result);

            return result;
        }
    }
}
