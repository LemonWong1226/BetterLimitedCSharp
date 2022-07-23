using MySql.Data.MySqlClient;
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
    public partial class GoodReceivedNote : Form
    {
        private String selectStatus;
        private String orderId;
        private String status;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        LinkedList<String> order = new LinkedList<String>();
        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;
        private string JSONlist;

        public GoodReceivedNote()
        {
            InitializeComponent();
        }

        private void GoodReceivedNote_Load(object sender, EventArgs e)
        {
            changeLanguage();
            loadDate();
        }

        public void loadDate()
        {
            dataGridView1.Rows.Clear();
            order.Clear();
            dataGridView1.Columns.Clear();
            try
            {
                ConnectString rootconn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = rootconn.getString();
                //           MessageBox.Show(rootconn.getString());
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select purchase_order_id, purchase_order.purchase_date, purchase_ttl_amount, purchase_order_status " +
                    "from purchase_order " +
                    "inner join purchase_request " +
                    "on purchase_order.purchase_request_id = purchase_request.purchase_request_id " +
                    "where purchase_order_status in (2,5)";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (i != 1)
                        {
                            order.AddLast(sqlRd.GetString(i));
                        }
                        else
                        {
                            order.AddLast(sqlRd.GetDateTime(i).ToString("yyyy-MM-dd"));
                        }
                    }
                }
                if (language == "Chinese")
                {
                    dataGridView1.Columns.Add("OrderId", "采购订单号");
                    dataGridView1.Columns.Add("Sales", "下单日期");
                    dataGridView1.Columns.Add("Date", "采购金额");
                    dataGridView1.Columns.Add("Status", "状态 ");
                }
                else
                {
                    dataGridView1.Columns.Add("OrderId", "Purchase Order Id");
                    dataGridView1.Columns.Add("Sales", "Send Purchase Date");
                    dataGridView1.Columns.Add("Date", "Purchase Total Amount");
                    dataGridView1.Columns.Add("Status", "Status");
                }

                for (int i = 0; i < order.Count / 4; i++)
                {
                    if (language == "Chinese")
                    {
                        switch (Convert.ToInt32(order.ElementAt(i * 4 + 3)))
                        {
                            case 1:
                                status = "请求已送到";
                                break;
                            case 2:
                                status = "请求已确认";
                                break;
                            case 3:
                                status = "请求被拒绝";
                                break;
                            case 4:
                                status = "请求已取消";
                                break;
                            case 5:
                                status = "请求已发送至会计部";
                                break;
                        }
                    }
                    else
                    {
                        switch (Convert.ToInt32(order.ElementAt(i * 4 + 3)))
                        {
                            case 1:
                                status = "Request Delivered";
                                break;
                            case 2:
                                status = "Request Confirmed";
                                break;
                            case 3:
                                status = "Request Rejected";
                                break;
                            case 4:
                                status = "Request Cancelled";
                                break;
                            case 5:
                                status = "Sent To Account";
                                break;
                        }
                    }
                    dataGridView1.Rows.Add(order.ElementAt(i * 4), order.ElementAt(i * 4 + 1), order.ElementAt(i * 4 + 2), status);
                }
                sqlDt.Load(sqlRd);
            }
            catch (Exception ex)
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
                btnSend.Text = "发送";
            }
            else
            {
                btnBack.Text = "BACK";
                btnSend.Text = "Send";
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            String selectID = "";
            Int32 selectedRowCount =
             dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                try
                {
                    if (dataGridView1.SelectedRows[0].Cells[3].Value.ToString() != "Sent To Accounting" ||
                       dataGridView1.SelectedRows[0].Cells[3].Value.ToString() != "请求已发送至会计部")
                    {
                        selectID = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                        sqlConn.Open();
                        sqlQuery = "update purchase_order set purchase_order_status = 5 where purchase_order_id = '" + selectID + "'";
                        sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                        sqlCmd.ExecuteNonQuery();
                        sqlConn.Close();
                        MessageBox.Show("Successful to send to the accounting department");

                        sqlConn.Open();
                        sqlCmd.Connection = sqlConn;
                        sqlCmd.CommandText = "insert into notification values(default, default,'Send Message', '" + selectID + " already sent to accounting department', '" + Main.instance.Staff + "')";
                        sqlCmd.ExecuteNonQuery();
                        sqlConn.Close();
                        Main.instance.increaseNotification();
                        Main.instance.setButtonRedDot();

                        loadDate();
                    }
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
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.PurchaseHome();
        }
    }
}
