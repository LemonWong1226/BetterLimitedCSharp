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
    public partial class ReorderDetail : Form
    {
        private String reorderId;
        private String storeId;
        private String staffId;
        private String reorderDate;
        private String remarks;
        private String status;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;

        public ReorderDetail()
        {
            InitializeComponent();
        }

        private void ReorderDetail_Load(object sender, EventArgs e)
        {
            changeLanguage();
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select store_id, staff_id, reorder_date, remarks, reorder_status " +
                "from reorder_request " +
                "where reorder_id = '" + reorderId + "'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                storeId = sqlRd.GetString(0);
                staffId = sqlRd.GetString(1);
                reorderDate = sqlRd.GetDateTime(2).ToString("yyyy-MM-dd");
                remarks = sqlRd.GetString(3);
                switch (sqlRd.GetInt32(4))
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
            }
            sqlRd.Close();
            sqlConn.Close();

            lblStoreId.Text = storeId;
            lblStaffId.Text = staffId;
            lblSubmitDate.Text = reorderDate;
            lblState.Text = status;
            txtRemark.Text = remarks;

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            if (language == "Chinese")
            {
                sqlCmd.CommandText = "select reorder_item.item_id as '补货货品号' " +
                    ", item_name as '货品名称'," +
                    " re_item_qty as '补货数量'" +
                    "from reorder_item " +
                    "inner join inventory " +
                    "on reorder_item.item_id = inventory.item_id " +
                    "where reorder_id = '" + reorderId + "'";
            }
            else
            {
                sqlCmd.CommandText = "select reorder_item.item_id as 'Reorder Item Id' " +
                    ", item_name as 'Item Name'," +
                    " re_item_qty as 'Reorder Item Quantity'" +
                    "from reorder_item " +
                    "inner join inventory " +
                    "on reorder_item.item_id = inventory.item_id " +
                    "where reorder_id = '" + reorderId + "'";
            }
            sqlRd = sqlCmd.ExecuteReader();
            sqlDt.Load(sqlRd);
            sqlRd.Close();
            sqlConn.Close();
            dataGridView1.DataSource = sqlDt;
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
                label5.Text = "备注:";
                btnBack.Text = "返回";
                label6.Text = "店铺号 :";
                label1.Text = "员工编号 :";
                label2.Text = "送出日期 :";
                label4.Text = "状态 :";
            }
            else
            {
                label5.Text = "Remark:";
                btnBack.Text = "BACK";
                label6.Text = "Store Id :";
                label1.Text = "Staff Id :";
                label2.Text = "Submit Date :";
                label4.Text = "State :";
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.ReorderRecord();
        }

        public void setReorderID(String reorderId)
        {
            this.reorderId = reorderId;
        }
    }
}
