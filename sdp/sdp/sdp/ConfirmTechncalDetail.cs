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
    public partial class ConfirmTechncalDetail : Form
    {
        public static ConfirmTechncalDetail instance;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;

        private String installationId;
        private String deliveryId;
        private String session;
        private int installationTime;
        private int statusnumber;
        private String status;
        private String workmanId;
        private String duty;
        private String invoice;
        private String installationDate;
        private String customerName;
        private String customerPhone;
        private String address;
        private String remark;
        private string language;
        private string JSONlist;

        public ConfirmTechncalDetail()
        {
            InitializeComponent();
            instance = this;
        }

        private void ConfirmTechncalDetail_Load(object sender, EventArgs e)
        {
            setPermission(Main.instance.dept, Main.instance.position);
            changeLanguage();
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select install_order_id, install_order.delivery_order_id, install_time, install_status, worker_staff_id, " +
                "worker_duty, install_order.invoice_id, install_date, customer_name, " +
                "customer_phone , delivery_address, install_remark, install_order.invoice_id " +
                "from install_order " +
                "inner join delivery_order " +
                "on delivery_order.delivery_order_id = install_order.delivery_order_id " +
                "where install_order_id = '" + installationId + "'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                installationId = sqlRd.GetString(0);
                deliveryId = sqlRd.GetString(1);
                installationTime = sqlRd.GetInt32(2);
                statusnumber = sqlRd.GetInt32(3);
                workmanId = sqlRd.GetString(4);
                duty = sqlRd.GetString(5);
                invoice = sqlRd.GetString(6);
                installationDate = sqlRd.GetDateTime(7).ToString("yyyy-MM-dd");
                customerName = sqlRd.GetString(8);
                customerPhone = sqlRd.GetString(9);
                address = sqlRd.GetString(10);
                remark = sqlRd.GetString(11);
                invoice = sqlRd.GetString(12);
            }
            sqlRd.Close();
            sqlConn.Close();

            status = numericeToStatu(statusnumber);
            session = timeToSession(installationTime);
            loadlable();
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
                    case 52:
                        btnSubmit.Visible = true;
                        button1.Visible = true;
                        break;
                    case 53:
                        btnCancel.Visible = true;
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
                label1.Text = "安装订单号 :";
                label2.Text = "送货单号 :";
                label11.Text = "发票号 :";
                label3.Text = "时间段 :";
                label4.Text = "安装时间 :";
                label8.Text = "技术员编号:";
                label12.Text = "备注 :";
                label6.Text = "安装日期 :";
                label9.Text = "客户姓名 :";
                label10.Text = "客户电话 :";
                label7.Text = "送货地址 :";
                label16.Text = "职责内容 :";
                btnSubmit.Text = "提交";
                btnBack.Text = "返回";
                btnSale.Text = "销售详情";
                btnCancel.Text = "取消请求";
                button1.Text = "拒绝";
            }
            else
            {
                label1.Text = "Installation Order Id :";
                label2.Text = "Delivery Order Id :";
                label11.Text = "Sale Invoice Id :";
                label3.Text = "Session :";
                label4.Text = "Installation Time :";
                label8.Text = "Workman Staff Id:";
                label12.Text = "Installation Remarks :";
                label6.Text = "Installation Date :";
                label9.Text = "Customer Name :";
                label10.Text = "Customer Phone No. :";
                label7.Text = "Dlivery Address :";
                label16.Text = "Installation Duties :";
                btnSubmit.Text = "Submit";
                btnBack.Text = "back";
                btnSale.Text = "Sale Order Detail";
                btnCancel.Text = "Request Cancel";
                button1.Text = "Reject";
            }
        }

        public String numericeToStatu(int number)
        {
            if (language == "Chinese")
            {
                switch (number)
                {
                    case 1:
                        return "请求已送到";
                    case 2:
                        return "请求已确认";
                    case 3:
                        return "请求被拒绝";
                    case 4:
                        return "请求已取消";
                    default:
                        return "";
                }
            }
            else
            {
                switch (number)
                {
                    case 1:
                        return "Request Delivered";
                    case 2:
                        return "Request Confirmed";
                    case 3:
                        return "Request Rejected";
                    case 4:
                        return "Request Cancelled";
                    default:
                        return "";
                }
            }
        }

        public String getInovice()
        {
            return invoice;
        }

        public String timeToSession(int time)
        {
            if (time <= 2200 && time >= 1800)
            {
                return "Evening";
            }
            else if (time <= 1700 && time >= 1300)
            {
                return "Aftermoon";
            }
            else if (time <= 1200 && time >= 900)
            {
                return "Morning";
            }
            else
            {
                return "";
            }
        }

        public void loadlable()
        {
            lblInstallationId.Text = installationId;
            lblDeliveryId.Text = deliveryId;
            lblSession.Text = session;
            lblInstallationTime.Text = installationTime.ToString();
            lblWorkman.Text = workmanId;
            lblDuty.Text = duty;
            lblInstallationDate.Text = installationDate;
            lblCustomerName.Text = customerName;
            lblCustomerPhone.Text = customerPhone;
            lblAddress.Text = address;
            lblRemarks.Text = remark;
            lblInvoice.Text = invoice;
        }

        public void setInstallationId(String installationId)
        {
            this.installationId = installationId;
        }

        private void btnSale_Click(object sender, EventArgs e)
        {
            Main.instance.SaleRecordDetail("confirmTechnical", "");
        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.ConfirmInstallation();

        }

        private void btnSubmit_Click(object sender, EventArgs e)
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
                    sqlQuery = "update install_order set install_status = 2 where install_order_id = " + lblInstallationId.Text;
                    sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    MessageBox.Show("Successful to confirm this order");
                    Main.instance.ConfirmInstallation();
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

        private void button1_Click(object sender, EventArgs e)
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
                    sqlQuery = "update install_order set install_status = 3 where install_order_id = " + lblInstallationId.Text;
                    sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    MessageBox.Show("Successful to confirm this order");
                    Main.instance.ConfirmInstallation();
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

        private void btnCancel_Click(object sender, EventArgs e)
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
                    sqlQuery = "update install_order set install_status = 4 where install_order_id = " + lblInstallationId.Text;
                    sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    MessageBox.Show("Successful to confirm this order");
                    Main.instance.ConfirmInstallation();
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
