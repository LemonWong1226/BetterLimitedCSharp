using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sdp
{
    public partial class StoreHomePage : Form
    {
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        DataSet DS = new DataSet();


        MySqlDataReader sqlRd;
        public String shop;
        bool isclick = false;
        public static StoreHomePage instance;
        private String language;
        private string JSONlist;

        public StoreHomePage()
        {
            InitializeComponent();
            instance = this;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Main.instance.checkout();
            /*      checkout check = new checkout();
                  this.Hide();
                  check.Show();
                  this.Close(); */
        }

        public void changeLanguage()
        {
            try
            {
                StreamReader text = new StreamReader(@"Language.txt");
                language = text.ReadLine();
                text.Close();
            }
            catch
            {

            }
            if (language == "Chinese")
            {
                button4.Text = "补货请求";
                button5.Text = "补货请求记录";
            }
            else
            {
                button4.Text = "Re-order Request";
                button5.Text = "Re-order Record";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Main.instance.shop1("HK01");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Main.instance.ReorderRequest();
        }

        public Boolean isClick()
        {
            return isclick;
        }

        private void StoreHomePage_Load(object sender, EventArgs e)
        {
            setPermission(Main.instance.dept, Main.instance.position);
            changeLanguage();
        }

        public void setPermission(int dept, int position)
        {
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select permission " +
                                    "from group_permission " +
                                    "where department = " + dept +
                                    " and position = " + position;
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                JSONlist = sqlRd.GetString(0);
            }
            sqlRd.Close();
            sqlConn.Close();
            var permissionList = JsonConvert.DeserializeObject<List<int>>(JSONlist);
            foreach (var permissionID in permissionList)
            {
                switch (permissionID)
                {
                    case 14:
                        button2.Visible = true;
                        break;
                    case 13:
                        button3.Visible = true;
                        break;
                    case 12:
                        button4.Visible = true;
                        break;
                    case 11:
                        button5.Visible = true;
                        break;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Main.instance.shop1("HK02");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Main.instance.ReorderRecord();
        }
    }
}
