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
    public partial class ConfirmDeliveryDetail : Form
    {
        public static ConfirmDeliveryDetail instance;
        public String invoice;
        private String order;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();



        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;
        private string JSONlist;

        public ConfirmDeliveryDetail()
        {
            InitializeComponent();
            instance = this;
        }

        private void ConfirmDeliveryDetail_Load(object sender, EventArgs e)
        {
            try
            {
                setPermission(Main.instance.dept, Main.instance.position);
                changeLanguage();
                ConnectString conn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = conn.getString();
                sqlConn.Open();
                sqlQuery = "select delivery_date, invoice_id, customer_name, customer_phone, delivery_remarks, delivery_time, delivery_address, staff_id from delivery_order " +
                    "where delivery_order_id = " + order;
                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    lblDate.Text = sqlRd.GetDateTime(0).ToString();
                    lblInvoice.Text = sqlRd.GetString(1);

                    lblName.Text = sqlRd.GetString(2);
                    lblPhone.Text = sqlRd.GetString(3);
                    lblRemark.Text = sqlRd.GetString(4);
                    if (sqlRd.GetInt32(5) <= 2200 && sqlRd.GetInt32(5) >= 1800)
                    {
                        if (language == "Chinese")
                            lblSession.Text = "晚上";
                        else
                            lblSession.Text = "Evening";
                    }
                    else if (sqlRd.GetInt32(5) <= 1700 && sqlRd.GetInt32(5) >= 1300)
                    {
                        if (language == "Chinese")
                            lblSession.Text = "中午";
                        else
                            lblSession.Text = "Aftermoon";
                    }
                    else if (sqlRd.GetInt32(5) <= 1200 && sqlRd.GetInt32(5) >= 900)
                    {
                        if (language == "Chinese")
                            lblSession.Text = "早上";
                        else
                            lblSession.Text = "Morning";
                    }
                    lblTime.Text = sqlRd.GetString(5);
                    lblAddress.Text = sqlRd.GetString(6);
                    lblStaff.Text = sqlRd.GetString(7);
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
                    case 31:
                        btnCancel.Visible = true;
                        break;
                    case 30:
                        btnConfirm.Visible = true;
                        btnReject.Visible = true;
                        break;
                    case 33:
                        btnSale.Visible = true;
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
                label2.Text = "送货日期:";
                label7.Text = "发票号:";
                label3.Text = "客户姓名:";
                label4.Text = "客户电话:";
                label5.Text = "备注:";
                btnSale.Text = "销售详情";
                btnBack.Text = "返回";
                label8.Text = "时间段:";
                label10.Text = "送货时间:";
                label9.Text = "送货地址:";
                label6.Text = "员工编号:";
                btnCancel.Text = "取消";
                btnReject.Text = "拒绝";
                btnConfirm.Text = "确认";
            }
            else
            {
                label2.Text = "Delivery Date:";
                label7.Text = "Sales Invoice Id:";
                label3.Text = "Customer Name:";
                label4.Text = "Customer Phone No.:";
                label5.Text = "Remarks:";
                btnSale.Text = "Sale Order Detail";
                btnBack.Text = "BACK";
                label8.Text = "Delivery Session:";
                label10.Text = "Delivery Time:";
                label9.Text = "Delivery Address:";
                label6.Text = "Sales Staff Id:";
                btnCancel.Text = "Cancel";
                btnReject.Text = "Reject";
                btnConfirm.Text = "Confirm";
            }
        }

        public void setOrder(String order)
        {
            this.order = order;
        }

        private void btnSale_Click(object sender, EventArgs e)
        {
            invoice = lblInvoice.Text;
            Main.instance.SaleRecordDetail("confirmDelivery", "");
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.ConfirmDelivery();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            string message = "Are you want to cancel the order?";
            string title = "cancel order";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.Yes)
            {
                try
                {
                    sqlConn.Open();
                    sqlQuery = "update delivery_order set delivery_status = 4 where invoice_id = " + lblInvoice.Text;
                    sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    MessageBox.Show("Successful to cancel this order");
                    Main.instance.ConfirmDelivery();
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

        private void btnReject_Click(object sender, EventArgs e)
        {
            string message = "Are you want to reject the order?";
            string title = "cancel order";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.Yes)
            {
                try
                {
                    sqlConn.Open();
                    sqlQuery = "update delivery_order set delivery_status = 3 where invoice_id = " + lblInvoice.Text;
                    sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    MessageBox.Show("Successful to reject this order");
                    Main.instance.ConfirmDelivery();
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

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string message = "Are you want to confirm the order?";
            string title = "cancel order";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.Yes)
            {
                try
                {
                    sqlConn.Open();
                    sqlQuery = "update delivery_order set delivery_status = 2 where invoice_id = " + lblInvoice.Text;
                    sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    MessageBox.Show("Successful to confirm this order");
                    Main.instance.ConfirmDelivery();
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
    }
}
