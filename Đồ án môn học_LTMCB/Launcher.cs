using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Đồ_án_môn_học_LTMCB
{
    public partial class Launcher : Form
    {
        public Launcher()
        {
            InitializeComponent();
        }

        private void btnServer_Click(object sender, EventArgs e)
        {
            Server server = new Server();
            server.Show();
        }

        private void btnClient_Click(object sender, EventArgs e)
        {
            Client client = new Client();
            client.Show();
        }
    }
}
