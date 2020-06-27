using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Đồ_án_môn_học_LTMCB
{
    public class Player
    {
        private string name;    // Ctrl + R + E

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Image mark;

        public Image Mark
        {
            get { return mark; }
            set { mark = value; }
        }

        public List<Player> players;

        public Player(string name, Image mark)
        {
            this.Name = name;
            this.Mark = mark;
        }

        public Player()
        {

        }

        public void AddPlayer(string name, Image mark)
        {
            if (players == null)
            {
                players = new List<Player>();
                players.Add(new Player(name, mark));
                this.Name = name;
                this.Mark = mark;
            }
            else
            {
                players.Add(new Player(name, mark));
                this.Name = name;
                this.Mark = mark;
            }
        }
    }
}
