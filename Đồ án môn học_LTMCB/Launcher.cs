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
            UIServer server = new UIServer();
            server.Show();
        }

        private void btnClient_Click(object sender, EventArgs e)
        {
            Form1 client = new Form1();
            client.Show();
        }
    }
}
