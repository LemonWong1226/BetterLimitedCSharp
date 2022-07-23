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
    public partial class PurchaseRequestOrderDetail : Form
    {
        private String status;
        private String requestId;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;
        private string JSONlist;

        public PurchaseRequestOrderDetail()
        {
            InitializeComponent();
        }

        private void PurchaseRequestOrderDetail_Load(object sender, EventArgs e)
        {
            setPermission(Main.instance.dept, Main.instance.position);
            changeLanguage();
            if (status == "Request Confirmed" || status == "Request Rejected" || status == "Request Cancelled" ||
                status == "请求已确认" || status == "请求被拒绝" || status == "请求已取消")
            {
                btnConfirm.Visible = false;
                btnReject.Visible = false;
                btnCancel.Visible = false;
            }
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select purchase_remarks, purchase_ttl_amount from purchase_request where purchase_request_id = '" + requestId+"'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                txtRemark.Text = sqlRd.GetString(0);
                lblTotal.Text = sqlRd.GetString(1);
            }
            sqlRd.Close();
            sqlConn.Close();

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            if (language == "Chinese")
            {
                sqlCmd.CommandText = "select purchase_item.item_id as '货品号', " +
                    "item_name as '货品名称', " +
                    "p_item_qty as '数量', " +
                    "p_unix_cost as '成本价', " +
                    "p_item_ttl_amount as '货品金额(HKD)' " +
                    "from purchase_item " +
                    "inner join inventory " +
                    "on inventory.item_id = purchase_item.item_id " +
                    "where purchase_request_id = '" + requestId + "'";
            }
            else
            {
                sqlCmd.CommandText = "select purchase_item.item_id as 'Purchase Item ID', " +
                    "item_name as 'Item Name', " +
                    "p_item_qty as 'Purchase Item Quantity', " +
                    "p_unix_cost as 'Purchase Unix Cost', " +
                    "p_item_ttl_amount as 'Purchase Item Total Amount(HKD)' " +
                    "from purchase_item " +
                    "inner join inventory " +
                    "on inventory.item_id = purchase_item.item_id " +
                    "where purchase_request_id = '" + requestId + "'";
            }
            sqlRd = sqlCmd.ExecuteReader();
            sqlDt.Load(sqlRd);
            sqlRd.Close();
            sqlConn.Close();
            dataGridView1.DataSource = sqlDt;
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
                    case 43:
                        btnReject.Visible = true;
                        btnConfirm.Visible = true;
                        break;
                    case 44:
                        btnCancel.Visible = true;
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
                label5.Text = "备注:";
                btnBack.Text = "返回";
                btnCancel.Text = "取消请求";
                label6.Text = "总共金额(HKD):";
                btnReject.Text = "拒绝";
                btnConfirm.Text = "确认请求订单";
            }
            else
            {
                label5.Text = "Remark:";
                btnBack.Text = "BACK";
                btnCancel.Text = "Cancel Request";
                label6.Text = "Total Cost(HKD):";
                btnReject.Text = "Reject";
                btnConfirm.Text = "Confirm Request Order";
            }
        }

        public void setRequestId(String requestId)
        {
            this.requestId = requestId;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            UpdateStatus("confirm", 2);
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            UpdateStatus("reject", 3);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateStatus("cancel", 4);
        }

        public void UpdateStatus(String action, int state)
        {
            string message = "Are you want to " + action + " the order?";
            string title = action + " order";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.Yes)
            {
                try
                {
                    ConnectString rootconn = new ConnectString("root", "123456");
                    sqlConn.ConnectionString = rootconn.getString();
                    switch(state){
                        case 2:
                            sqlConn.Open();
                            sqlCmd.Connection = sqlConn;
                            sqlCmd.CommandText = "update purchase_request set purchase_request_status = " + state + " where purchase_request_id = '" +
                                requestId + "'";
                            sqlCmd.ExecuteNonQuery();
                            sqlConn.Close();

                            sqlConn.Open();
                            sqlCmd.Connection = sqlConn;
                            sqlCmd.CommandText = "insert into purchase_order values(default, '"+ requestId + "',1, default)";
                            sqlCmd.ExecuteNonQuery();
                            sqlConn.Close();
                            MessageBox.Show("Successful to " + action + " this order");
                            Main.instance.PurchaseRequestOrderRecord();
                            break;
                        case 3:
                            sqlConn.Open();
                            sqlCmd.Connection = sqlConn;
                            sqlCmd.CommandText = "update purchase_request set purchase_request_status = " + state + " where purchase_request_id = '" +
                                requestId + "'";
                            sqlCmd.ExecuteNonQuery();
                            sqlConn.Close();
                            MessageBox.Show("Successful to " + action + " this order");
                            Main.instance.PurchaseRequestOrderRecord();
                            break;
                        case 4:
                            sqlConn.Open();
                            sqlCmd.Connection = sqlConn;
                            sqlCmd.CommandText = "update purchase_request set purchase_request_status = " + state + " where purchase_request_id = '" +
                                requestId + "'";
                            sqlCmd.ExecuteNonQuery();
                            sqlConn.Close();
                            MessageBox.Show("Successful to " + action + " this order");
                            Main.instance.PurchaseRequestOrderRecord();
                            break;
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
            Main.instance.PurchaseRequestOrderRecord();
        }
        
        public void setStatus(String status)
        {
            this.status = status;
        }
    }
}
