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
    public partial class ReorderRequestDetail : Form
    {
        private String ReorderId;
        private String storeId;
        private String staffId;
        private String reorderDate;
        private String remarks;
        private String inventoryQty;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;
        private string JSONlist;

        public ReorderRequestDetail()
        {
            InitializeComponent();
        }

        private void ReorderRequestDetail_Load(object sender, EventArgs e)
        {
            setPermission(Main.instance.dept, Main.instance.position);
            changeLanguage();
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select store_id, staff_id, reorder_date, remarks " +
                "from reorder_request " +
                "where reorder_id = '"+ReorderId+"'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                storeId = sqlRd.GetString(0);
                staffId = sqlRd.GetString(1);
                reorderDate = sqlRd.GetDateTime(2).ToString("yyyy-MM-dd");
                remarks = sqlRd.GetString(3);
            }
            sqlRd.Close();
            sqlConn.Close();

            lblStoreId.Text = storeId;
            lblStaffId.Text = staffId;
            lblSubmitDate.Text = reorderDate;
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
                    "where reorder_id = '" + ReorderId + "'";
            }
            else
            {
                sqlCmd.CommandText = "select reorder_item.item_id as 'Reorder Item Id' " +
                    ", item_name as 'Item Name'," +
                    " re_item_qty as 'Reorder Item Quantity'" +
                    "from reorder_item " +
                    "inner join inventory " +
                    "on reorder_item.item_id = inventory.item_id " +
                    "where reorder_id = '" + ReorderId + "'";
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
                    case 4:
                        btnCancel.Visible = true;
                        break;
                    case 3:
                        btnConfirm.Visible = true;
                        btnReject.Visible = true;
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
                label6.Text = "店铺号 :";
                label1.Text = "员工编号 :";
                label2.Text = "送出日期 :";
                btnReject.Text = "拒绝";
                btnConfirm.Text = "确认请求订单";
            }
            else
            {
                label5.Text = "Remark:";
                btnBack.Text = "BACK";
                btnCancel.Text = "Cancel Request";
                label6.Text = "Store Id :";
                label1.Text = "Staff Id :";
                label2.Text = "Submit Date :";
                btnReject.Text = "Reject";
                btnConfirm.Text = "Confirm Request Order";
            }
        }

        public void setReorderID(String ReorderId)
        {
            this.ReorderId = ReorderId;
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
            string message = "Are you want to "+action+" the order?";
            string title = action + " order";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.Yes)
            {
                try
                {
                    ConnectString rootconn = new ConnectString("root", "123456");
                    sqlConn.ConnectionString = rootconn.getString();
                    if(state == 2)
                    {
                        if (checkQty())
                        {
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                Boolean before = false;
                                Boolean after = false;
                                Boolean zero = false;
                                Boolean islow = false;

                                sqlConn.Open();
                                sqlQuery = "update reorder_request set reorder_status = " + state +
                                    " , remarks = '" + txtRemark.Text + "' where reorder_id = " + ReorderId;
                                Console.WriteLine(sqlQuery);
                                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                                sqlCmd.ExecuteNonQuery();
                                sqlConn.Close();

                                sqlConn.Open();
                                sqlCmd.Connection = sqlConn;
                                sqlCmd.CommandText = "update store_stock set store_stock_qty = store_stock_qty + " + row.Cells[2].Value + " where store_id = '" +
                                    lblStoreId.Text + "' and item_id = '" + row.Cells[0].Value + "'";
                                sqlCmd.ExecuteNonQuery();
                                sqlConn.Close();

                                sqlConn.Open();
                                sqlCmd.Connection = sqlConn;
                                sqlCmd.CommandText = "update inventory set inventory_qty = inventory_qty - " +
                                    row.Cells[2].Value + " where item_id = '" + row.Cells[0].Value + "'";
                                sqlCmd.ExecuteNonQuery();
                                sqlConn.Close();

                                sqlConn.Open();
                                sqlCmd.Connection = sqlConn;
                                sqlCmd.CommandText = "select inventory_qty, item_alarm from inventory where item_id = '"+ row.Cells[0].Value + "'";
                                sqlRd = sqlCmd.ExecuteReader();
                                Console.WriteLine(sqlCmd.CommandText);
                                while (sqlRd.Read())
                                {
                                    if (sqlRd.GetInt32(0) == 0)
                                    {
                                        zero = true;
                                    }
                                    if (sqlRd.GetInt32(0) < sqlRd.GetInt32(1))
                                    {
                                        islow = true;
                                    }
                                }
                                sqlRd.Close();
                                sqlConn.Close();

                                if (zero == true)
                                {
                                    sqlConn.Open();
                                    sqlCmd.Connection = sqlConn;
                                    sqlCmd.CommandText = "insert into notification values(default, default,'Alarm Message', '" + row.Cells[0].Value + " is out of stock already, please purchase the item', '" + Main.instance.Staff + "')";
                                    sqlCmd.ExecuteNonQuery();
                                    sqlConn.Close();
                                    Main.instance.increaseNotification();
                                    Main.instance.setButtonRedDot();
                                }
                                else if (islow == true)
                                {
                                    sqlConn.Open();
                                    sqlCmd.Connection = sqlConn;
                                    sqlCmd.CommandText = "insert into notification values(default, default,'Alarm Message', '" + row.Cells[0].Value + " stock level is low', '" + Main.instance.Staff + "')";
                                    sqlCmd.ExecuteNonQuery();
                                    sqlConn.Close();
                                    Main.instance.increaseNotification();
                                    Main.instance.setButtonRedDot();
                                }

                                MessageBox.Show("Successfully to " + action + " this order");
                                Main.instance.ReorderRequest();
                            }
                        }
                        else
                        {
                            MessageBox.Show("the inventory have not enough quantity to process re-order");
                        }
                    }
                    else
                    {
                        sqlConn.Open();
                        sqlQuery = "update reorder_request set reorder_status = " + state +
                            " , remarks = '" + txtRemark.Text + "' where reorder_id = " + ReorderId;
                        Console.WriteLine(sqlQuery);
                        sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                        sqlCmd.ExecuteNonQuery();
                        sqlConn.Close();

                        MessageBox.Show("Successfully to " + action + " this order");
                        Main.instance.ReorderRequest();
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

        public Boolean checkQty()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select inventory_qty from inventory where item_id = '" + row.Cells[0].Value + "'";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    inventoryQty = sqlRd.GetString(0);
                }
                sqlRd.Close();
                sqlConn.Close();
                if(Convert.ToInt32(inventoryQty) - Convert.ToInt32(row.Cells[2].Value) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.ReorderRequest();
        }
    }
}
