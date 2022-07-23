using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sdp
{
    public partial class selectShop : Form
    {
        public String select;
        public selectShop()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                select = "HK01";
            }
            if (radioButton2.Checked)
            {
                select = "HK02";
            }
            this.Close();
        }

        public String getSelect()
        {
            return select;
        }
    }
}
