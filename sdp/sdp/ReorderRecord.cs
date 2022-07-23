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
    public partial class ReorderRecord : Form
    {
        private String reorderId;
        private String Status;
        LinkedList<String> request = new LinkedList<String>();
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;

        public ReorderRecord()
        {
            InitializeComponent();
        }

        private void ReorderRecord_Load(object sender, EventArgs e)
        {
            changeLanguage();
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            //           MessageBox.Show(rootconn.getString());
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select reorder_id, staff_id, reorder_date, reorder_status from reorder_request where reorder_status in (2,3,4)";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i != 2)
                    {
                        request.AddLast(sqlRd.GetString(i));
                    }
                    else
                    {
                        request.AddLast(sqlRd.GetDateTime(i).ToString("yyyy-MM-dd"));
                    }
                }
            }
            if (language == "Chinese")
            {
                dataGridView1.Columns.Add("reorder_id", "补货号");
                dataGridView1.Columns.Add("staff_id", "店铺号");
                dataGridView1.Columns.Add("reorder_date", "日期");
                dataGridView1.Columns.Add("reorder_status", "状态");
            }
            else
            {
                dataGridView1.Columns.Add("reorder_id", "Re-order Request Id");
                dataGridView1.Columns.Add("staff_id", "Store Id");
                dataGridView1.Columns.Add("reorder_date", "Date");
                dataGridView1.Columns.Add("reorder_status", "Status");
            }

            for (int i = 0; i < request.Count / 4; i++)
            {
                if (language == "Chinese")
                {
                    switch (Convert.ToInt32(request.ElementAt(i * 4 + 3)))
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
                    switch (Convert.ToInt32(request.ElementAt(i * 4 + 3)))
                    {

                        case 1:
                            Status = "Request Delivered";
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
                dataGridView1.Rows.Add(request.ElementAt(i * 4), request.ElementAt(i * 4 + 1), request.ElementAt(i * 4 + 2), Status);
            }
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
                btnBack.Text = "返回";
            }
            else
            {
                btnDetail.Text = "More Detail";
                btnBack.Text = "BACK";
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.saleManagement();
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            Int32 selectedRowCount =
    dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                reorderId = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                Main.instance.ReorderRecordDetail(reorderId);
            }
        }
    }
}
