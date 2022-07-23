using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace sdp
{
    public partial class Home : Form
    {
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        MySqlDataReader sqlRd;
        String[] notification = new String[3];
        Queue<String> noti = new Queue<string>();
        public String staffid;
        public Home()
        {
            InitializeComponent();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            int i = 0;
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            timer1.Enabled = true;
            lblStaffId.Text = staffid;

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select notification_message from notification order by notification_date desc LIMIT 3";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                notification[i] = sqlRd.GetString(0);
                i++;
            }
            sqlConn.Close();
            lbllNoti1.Text = notification[0];
            lblNoti2.Text = notification[1];
            lblNoti3.Text = notification[2];

        }

        public void setstaffID(String staffid)
        {
            this.staffid = staffid;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public void viewImage1()
        {
            pictureBox1.Visible = true;
        }

        public void viewImage2()
        {
            pictureBox2.Visible = true;
        }

        public void viewImage3()
        {
            pictureBox3.Visible = true;
        }
        public void hideImage1()
        {
            pictureBox1.Visible = false;
        }
        public void hideImage2()
        {
            pictureBox2.Visible = false;
        }
        public void hideImage3()
        {
            pictureBox3.Visible = false;
        }

        private void Home_Leave(object sender, EventArgs e)
        {
            Console.WriteLine("run");
        }
    }
}
