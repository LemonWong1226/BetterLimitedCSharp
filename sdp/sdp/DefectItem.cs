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
    public partial class DefectItem : Form
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

        public DefectItem()
        {
            InitializeComponent();
        }

        private void DefectItem_Load(object sender, EventArgs e)
        {
            setPermission(Main.instance.dept, Main.instance.position);
            changeLanguage();
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select good_return_id, invoice_id, good_return_date, good_return_qty, good_return_status " +
                "from good_return_note " +
                "where good_return_status in (1,2,3,4)";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                for (int i = 0; i < 5; i++)
                {
                    if (i != 2)
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
                dataGridView1.Columns.Add("OrderId", "退货号");
                dataGridView1.Columns.Add("InvoiceId", "发票号");
                dataGridView1.Columns.Add("Date", "退货日期");
                dataGridView1.Columns.Add("Quantity", "退回数量");
                dataGridView1.Columns.Add("Status", "状态");
            }
            else
            {
                dataGridView1.Columns.Add("OrderId", "Good Return Id");
                dataGridView1.Columns.Add("InvoiceId", "Invoice Id");
                dataGridView1.Columns.Add("Date", "Good Return Date");
                dataGridView1.Columns.Add("Quantity", "Return Quantity");
                dataGridView1.Columns.Add("Status", "Status");
            }
            for (int i = 0; i < order.Count / 5; i++)
            {
                switch (Convert.ToInt32(order.ElementAt(i * 5 + 4)))
                {
                    case 1:
                        if (language == "Chinese")
                            status = "请求已送到";
                        else
                            status = "Request Delivered";
                        break;
                    case 2:
                        if (language == "Chinese")
                            status = "请求已确认";
                        else
                            status = "Request Confirmed";
                        break;
                    case 3:
                        if (language == "Chinese")
                            status = "请求被拒绝";
                        else
                            status = "Request Rejected";
                        break;
                    case 4:
                        if (language == "Chinese")
                            status = "请求已取消";
                        else
                            status = "Request Cancelled";
                        break;
                }
                dataGridView1.Rows.Add(order.ElementAt(i * 5), order.ElementAt(i * 5 + 1), order.ElementAt(i * 5 + 2), order.ElementAt(i * 5 + 3), status);
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
                    case 24:
                        btnAdd.Visible = true;
                        break;
                    case 23:
                        btnDetail.Visible = true;
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
                btnAdd.Text = "新增";
                btnDetail.Text = "详情";
                btnBack.Text = "返回";
            }
            else
            {
                btnAdd.Text = "Add New";
                btnDetail.Text = "More Detail";
                btnBack.Text = "BACK";
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Main.instance.AddDefectItem();
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            Int32 selectedRowCount =
                 dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                orderId = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                selectStatus = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
                Main.instance.DefectItemDetails(orderId, selectStatus);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.Inventory();
        }
    }
}
