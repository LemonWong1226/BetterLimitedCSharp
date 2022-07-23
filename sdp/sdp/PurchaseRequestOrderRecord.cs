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
    public partial class PurchaseRequestOrderRecord : Form
    {
        private String selectStatus;
        private String requestId;
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

        public PurchaseRequestOrderRecord()
        {
            InitializeComponent();
        }

        private void PurchaseRequestOrderRecord_Load(object sender, EventArgs e)
        {
            setPermission(Main.instance.dept, Main.instance.position);
            changeLanguage();
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            //           MessageBox.Show(rootconn.getString());
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select purchase_request_id, purchase_date, purchase_ttl_amount, purchase_request_status from purchase_request where purchase_request_status in(1,2,3,4)";
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
                    }
                }
                dataGridView1.Rows.Add(order.ElementAt(i * 4), order.ElementAt(i * 4 + 1), order.ElementAt(i * 4 + 2), status);
            }
            sqlDt.Load(sqlRd);
            sqlRd.Close();
            sqlConn.Close();
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
                    case 45:
                        btnDetail.Visible = true;
                        break;
                    case 46:
                        btnAdd.Visible = true;
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
                btnDetail.Text = "详情";
                btnBack.Text = "返回";
                btnAdd.Text = "添加";
            }
            else
            {
                btnDetail.Text = "More Detail";
                btnBack.Text = "BACK";
                btnAdd.Text = "Add New";
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Main.instance.NewPurchaseRequest();
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            Int32 selectedRowCount =
                dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                requestId = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                selectStatus = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
                Main.instance.PurchaseRequestOrderDetail(requestId, selectStatus);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.PurchaseHome();
        }
    }
}
