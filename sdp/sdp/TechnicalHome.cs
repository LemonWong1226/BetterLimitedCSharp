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
    public partial class TechnicalHome : Form
    {
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;
        private string JSONlist;

        public TechnicalHome()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Main.instance.TechnicalTable();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Main.instance.ConfirmInstallation();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Main.instance.TechnicalRecord();
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
                button1.Text = "时间表";
                button2.Text = "确认安装订单";
                button3.Text = "安装订单记录";
            }
            else
            {
                button1.Text = "Time Table";
                button2.Text = "Confirm Installation Order";
                button3.Text = "Installation Order Records";
            }
        }

        private void TechnicalHome_Load(object sender, EventArgs e)
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
                    case 59:
                        button1.Visible = true;
                        break;
                    case 53:
                        button2.Visible = true;
                        break;
                    case 51:
                        button3.Visible = true;
                        break;
                }
            }
        }
    }
}
