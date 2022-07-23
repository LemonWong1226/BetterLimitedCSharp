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
    public partial class InstallationRecord : Form
    {
        private String installationDate;
        public String order;
        public String invoice;
        public static InstallationRecord instance;
        String session;
        String Status;

        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        LinkedList<String> delivery = new LinkedList<String>();
        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;

        public InstallationRecord()
        {
            InitializeComponent();
            instance = this;
        }

        private void InstallationRecord_Load(object sender, EventArgs e)
        {
            changeLanguage();
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            //           MessageBox.Show(rootconn.getString());
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select install_order_id, invoice_id, install_date, install_time, install_status " +
                "from install_order where install_status in(2,3,4)";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                for (int i = 0; i < 5; i++)
                {
                    if (i != 2)
                    {
                        delivery.AddLast(sqlRd.GetString(i));
                    }
                    else
                    {
                        delivery.AddLast(sqlRd.GetDateTime(i).ToString("yyyy-MM-dd"));
                    }
                }
            }
            if (language == "Chinese")
            {
                dataGridView1.Columns.Add("DeliveryOrderId", "安装订单号");
                dataGridView1.Columns.Add("Sales", "发票号");
                dataGridView1.Columns.Add("Date", "日期");
                dataGridView1.Columns.Add("Session", "状态");
                dataGridView1.Columns.Add("Status", "状态");
            }
            else
            {
                dataGridView1.Columns.Add("DeliveryOrderId", "Installation Order Id");
                dataGridView1.Columns.Add("Sales", "Sales Invoice Id");
                dataGridView1.Columns.Add("Date", "Date");
                dataGridView1.Columns.Add("Session", "Session");
                dataGridView1.Columns.Add("Status", "Status");
            }

            for (int i = 0; i < delivery.Count / 5; i++)
            {
                if (Convert.ToInt32(delivery.ElementAt(i * 5 + 3)) >= 900 && Convert.ToInt32(delivery.ElementAt(i * 5 + 3)) <= 1200)
                {
                    if (language == "Chinese")
                        session = "早上";
                    else
                        session = "Morning";
                }
                else if (Convert.ToInt32(delivery.ElementAt(i * 5 + 3)) >= 1300 && Convert.ToInt32(delivery.ElementAt(i * 5 + 3)) <= 1700)
                {
                    if (language == "Chinese")
                        session = "中午";
                    else
                        session = "Aftermoon";
                }
                else if (Convert.ToInt32(delivery.ElementAt(i * 5 + 3)) >= 1800 && Convert.ToInt32(delivery.ElementAt(i * 5 + 3)) <= 2200)
                {
                    if (language == "Chinese")
                        session = "下午";
                    else
                        session = "Evening";
                }
                if (language == "Chinese")
                {
                    switch (Convert.ToInt32(delivery.ElementAt(i * 5 + 4)))
                    {
                        case 2:
                            Status = "请求已确认";
                            break;
                        case 3:
                            Status = "请求被拒绝";
                            break;
                        case 4:
                            Status = "请求已取消";
                            break;
                    }
                }
                else
                {
                    switch (Convert.ToInt32(delivery.ElementAt(i * 5 + 4)))
                    {
                        case 2:
                            Status = "Request Confirmed";
                            break;
                        case 3:
                            Status = "Request Rejected";
                            break;
                        case 4:
                            Status = "Request Cancelled";
                            break;
                    }
                }
                dataGridView1.Rows.Add(delivery.ElementAt(i * 5), delivery.ElementAt(i * 5 + 1), delivery.ElementAt(i * 5 + 2), session, Status);
            }
            sqlDt.Load(sqlRd);
            sqlRd.Close();
            sqlConn.Close();
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
                btnDetail.Text = "详情";
                butDate.Text = "选择日期";
                btnBack.Text = "返回";
            }
            else
            {
                btnDetail.Text = "More Detail";
                butDate.Text = "Select Date";
                btnBack.Text = "back";
            }
        }

        private void butDate_Click(object sender, EventArgs e)
        {
            selectDate date = new selectDate();
            date.ShowDialog();
            installationDate = date.getDate();
            updateDate(installationDate);
        }

        public void updateDate(String Date)
        {
            delivery.Clear();
            dataGridView1.Rows.Clear();
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            //           MessageBox.Show(rootconn.getString());
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select install_order_id, invoice_id, install_date, install_time, install_status " +
                "from install_order where install_status in(2,3,4) " +
                "and install_date >= '" + Date + " 0:0:0' and install_date <= '" + Date + " 23:59:59'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                for (int i = 0; i < 5; i++)
                {
                    if (i != 2)
                    {
                        delivery.AddLast(sqlRd.GetString(i));
                    }
                    else
                    {
                        delivery.AddLast(sqlRd.GetDateTime(i).ToString("yyyy-MM-dd"));
                    }
                }
            }


            for (int i = 0; i < delivery.Count / 5; i++)
            {
                if (Convert.ToInt32(delivery.ElementAt(i * 5 + 3)) >= 900 && Convert.ToInt32(delivery.ElementAt(i * 5 + 3)) <= 1200)
                {
                    if (language == "Chinese")
                        session = "早上";
                    else
                        session = "Morning";
                }
                else if (Convert.ToInt32(delivery.ElementAt(i * 5 + 3)) >= 1300 && Convert.ToInt32(delivery.ElementAt(i * 5 + 3)) <= 1700)
                {
                    if (language == "Chinese")
                        session = "中午";
                    else
                        session = "Aftermoon";
                }
                else if (Convert.ToInt32(delivery.ElementAt(i * 5 + 3)) >= 1800 && Convert.ToInt32(delivery.ElementAt(i * 5 + 3)) <= 2200)
                {
                    if (language == "Chinese")
                        session = "下午";
                    else
                        session = "Evening";
                }
                if (language == "Chinese")
                {
                    switch (Convert.ToInt32(delivery.ElementAt(i * 5 + 4)))
                    {
                        case 2:
                            Status = "请求已确认";
                            break;
                        case 3:
                            Status = "请求被拒绝";
                            break;
                        case 4:
                            Status = "请求已取消";
                            break;
                    }
                }
                else
                {
                    switch (Convert.ToInt32(delivery.ElementAt(i * 5 + 4)))
                    {
                        case 2:
                            Status = "Request Confirmed";
                            break;
                        case 3:
                            Status = "Request Rejected";
                            break;
                        case 4:
                            Status = "Request Cancelled";
                            break;
                    }
                }
                dataGridView1.Rows.Add(delivery.ElementAt(i * 5), delivery.ElementAt(i * 5 + 1), delivery.ElementAt(i * 5 + 2), session, Status);
            }
            sqlDt.Load(sqlRd);
            sqlRd.Close();
            sqlConn.Close();
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            Int32 selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                order = (dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
            }
            Main.instance.InstallationDetail("record");
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.TechnicalHome();
        }
    }
}
