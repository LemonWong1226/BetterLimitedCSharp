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
    public partial class DeliveryRecord : Form
    {
        public String order;
        public String invoice;
        public static DeliveryRecord instance;
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

        public DeliveryRecord()
        {
            InitializeComponent();
            instance = this;
        }

        private void DeliveryRecord_Load(object sender, EventArgs e)
        {
            try
            {
                changeLanguage();
                ConnectString rootconn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = rootconn.getString();
                //           MessageBox.Show(rootconn.getString());
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "SELECT delivery_order_id, invoice_id, delivery_date, delivery_time, delivery_status from delivery_order where delivery_status in(2,3,4)";
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
                    dataGridView1.Columns.Add("OrderId", "送货单号");
                    dataGridView1.Columns.Add("Sales", "发票号");
                    dataGridView1.Columns.Add("Date", "时间");
                    dataGridView1.Columns.Add("Session", "时间段");
                    dataGridView1.Columns.Add("Status", "状态");
                }
                else
                {
                    dataGridView1.Columns.Add("OrderId", "Dlivery Order Id");
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
                            session = "晚上";
                        else
                            session = "evening";
                    }
                    if (language == "Chinese")
                    {
                        switch (Convert.ToInt32(delivery.ElementAt(i * 5 + 4)))
                        {
                            case 1:
                                Status = "请求已送到";
                                break;
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
                            case 1:
                                Status = "Request Relivered";
                                break;
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
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlRd.Close();
                sqlConn.Close();
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
                btnBack.Text = "返回";
                btnDetail.Text = "详情";
            }
            else
            {
                btnBack.Text = "BACK";
                btnDetail.Text = "More Detail";
            }
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            Int32 selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                order = (dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                invoice = (dataGridView1.SelectedRows[0].Cells[1].Value.ToString());
            }
            Main.instance.DeliveryDetail("record");
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.DeliveryHome();
        }
    }
}
