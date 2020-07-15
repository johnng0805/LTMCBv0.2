using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Đồ_án_môn_học_LTMCB
{
    class XMLClass
    {
        private string player1;
        private string player2;
        private string room;
        private string winner;

        public XMLClass(string username = "", string _room = "")
        {
            if (player1 == "")
            {
                player1 = username;
            }
            else
            {
                player2 = username;
            }
            room = _room;
        }

        public void XMLToFile()
        {
            XmlSerializer xs = new XmlSerializer(typeof(Player));

        }
    }
}
