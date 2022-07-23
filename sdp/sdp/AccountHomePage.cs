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
    public partial class AccountHomePage : Form
    {
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        MySqlDataReader sqlRd;
        private string JSONlist;
        private string language;

        public AccountHomePage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            setPermission(Main.instance.dept, Main.instance.position);
            Main.instance.ExportPaymentReceipt();
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
                    case 62:
                        button1.Visible = true;
                        break;
                    case 61:
                        button2.Visible = true;
                        break;
                }
            }
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
                button1.Text = "输出付款收据";
                button2.Text = "輸出采购订单";
            }
            else
            {
                button1.Text = "Export Payment Receipt";
                button2.Text = "Export Purchase Order";
            }
        }

        private void AccountHomePage_Load(object sender, EventArgs e)
        {
            changeLanguage();
            setPermission(Main.instance.dept, Main.instance.position);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Main.instance.ExportPurchaseOrder();
        }
    }
}
