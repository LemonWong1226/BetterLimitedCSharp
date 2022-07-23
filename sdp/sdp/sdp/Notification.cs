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
using Newtonsoft.Json;

namespace sdp
{
    public partial class Notification : Form
    {
        Dictionary<String, String> dicNotiId = new Dictionary<String, String>();
        public String notificationId;
        public static Notification instance;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;
        private string JSONlist;

        public Notification()
        {
            InitializeComponent();
            instance = this;
        }

        private void Notification_Load(object sender, EventArgs e)
        {
            try
            {
                setPermission(Main.instance.dept, Main.instance.position);
                changeLanguage();
                int i = 0;
                ConnectString rootconn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = rootconn.getString();
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select * from notification order by notification_date desc";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    dicNotiId.Add(i.ToString(), sqlRd.GetString(0));
                    i++;
                }
                sqlRd.Close();
                if (language == "Chinese")
                    sqlCmd.CommandText = "select notification_type as '讯息类型', notification_date as '日期', notification_message as '内容', staff_id as '发送者'from notification order by notification_date desc";
                else
                    sqlCmd.CommandText = "select notification_type as 'Message Type', notification_date as 'Date', notification_message as 'Content', staff_id as 'Sender'from notification order by notification_date desc";
                sqlRd = sqlCmd.ExecuteReader();
                sqlDt.Load(sqlRd);
                dataGridView1.DataSource = sqlDt;
                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlRd.Close();
                sqlConn.Close();
            }
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
                    case 1:
                        btnDetail.Visible = true;
                        break;
                }
            }
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            Int32 selectedRowCount =
                dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                notificationId = dicNotiId[dataGridView1.SelectedRows[0].Index.ToString()];
            }
            Main.instance.NotificationDetail();
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
                label4.Text = "通知表:";
                btnDetail.Text = "详情";
            }
            else
            {
                label4.Text = "Notification List:";
                btnDetail.Text = "More Detail";
            }
        }
    }
}
