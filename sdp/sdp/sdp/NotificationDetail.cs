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
using MySql.Data.MySqlClient;

namespace sdp
{
    public partial class NotificationDetail : Form
    {
        public String NotificationID;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;

        public NotificationDetail()
        {
            InitializeComponent();
        }

        private void NotificationDetail_Load(object sender, EventArgs e)
        {
            try {
                changeLanguage();
                ConnectString rootconn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = rootconn.getString();
                sqlConn.Open();
                sqlQuery = "select staff_id, notification_date, notification_type, notification_message from notification where notification_no = " + NotificationID;
                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                    {
                        lblSender.Text = sqlRd.GetString(0);
                        lblSendDate.Text = sqlRd.GetString(1);
                        lblType.Text = sqlRd.GetString(2);
                        lblContent.Text = sqlRd.GetString(3);
                    }
                sqlConn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    sqlConn.Close();
                }
        }

        public void setNotificationID(String NotificationID)
        {
            this.NotificationID = NotificationID;
        }

        private void butback_Click(object sender, EventArgs e)
        {
            Main.instance.Notification();
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
                label7.Text = "发送者 :";
                label8.Text = "发送日期 :";
                label4.Text = "讯息类型 :";
                label3.Text = "内容 :";
                butback.Text = "返回";
            }
            else
            {
                label7.Text = "Sender :";
                label8.Text = "Send Date :";
                label4.Text = "Message Type :";
                label3.Text = "Message Content :";
                butback.Text = "BACK";
            }
        }
    }
}
