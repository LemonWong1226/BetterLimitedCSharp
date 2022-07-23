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
    public partial class InstallationDetail : Form
    {
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;

        private String action;
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

        public InstallationDetail()
        {
            InitializeComponent();
        }

        private void InstallationDetail_Load(object sender, EventArgs e)
        {
            setPermission(Main.instance.dept, Main.instance.position);
            changeLanguage();
            if (action == "record")
            {
                btnEdit.Visible = false;
            }
           // dtpDate.MinDate = DateTime.Now;
            txtInstallationTime.Text = "e.g 0840, 0940, 2200";
            // dtpDate.MinDate = DateTime.Today;
            dtpDate.CustomFormat = "yyyy-MM-dd";
            dtpDate.Format = DateTimePickerFormat.Custom;
            updateValie();
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
                    case 56:
                        btnEdit.Visible = true;
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
                label3.Text = "时间段 :";
                label4.Text = "安装时间 :";
                label8.Text = "技术员编号:";
                label12.Text = "备注 :";
                btnEditBack.Text = "返回";
                label6.Text = "安装日期 :";
                label9.Text = "客户姓名 :";
                label10.Text = "客户电话 :";
                label7.Text = "送货地址 :";
                label5.Text = "状态 :";
                label16.Text = "职责内容 :";
                btnSubmit.Text = "提交";
                btnEdit.Text = "编辑";
                btnBack.Text = "返回";
            }
            else
            {
                label1.Text = "Installation Order Id :";
                label2.Text = "Delivery Order Id :";
                label3.Text = "Session :";
                label4.Text = "Installation Time :";
                label8.Text = "Workman Staff Id:";
                label12.Text = "Installation Remarks :";
                btnEditBack.Text = "back";
                label6.Text = "Installation Date :";
                label9.Text = "Customer Name :";
                label10.Text = "Customer Phone No. :";
                label7.Text = "Dlivery Address :";
                label5.Text = "Status :";
                label16.Text = "Installation Duties :";
                btnSubmit.Text = "Submit";
                btnEdit.Text = "Edit Order";
                btnBack.Text = "back";
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

        public void updateValie()
        {
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select install_order_id, install_order.delivery_order_id, install_time, install_status, worker_staff_id, " +
                "worker_duty, install_order.invoice_id, install_date, customer_name, " +
                "customer_phone , delivery_address, install_remark " +
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
            }
            sqlRd.Close();
            sqlConn.Close();

            status = numericeToStatu(statusnumber);
            session = timeToSession(installationTime);
            loadlable();
        }

        public void loadlable()
        {
            lblInstallationId.Text = installationId;
            lblDeliveryId.Text = deliveryId;
            lblSession.Text = session;
            lblInstallationTime.Text = installationTime.ToString();
            lblStatus.Text = status;
            lblWorkman.Text = workmanId;
            lblDuty.Text = duty;
            lblInstallationDate.Text = installationDate;
            lblCustomerName.Text = customerName;
            lblCustomerPhone.Text = customerPhone;
            lblAddress.Text = address;
            lblRemarks.Text = remark;
        }

        public void setInstallationId(String installationId)
        {
            this.installationId = installationId;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            btnBack.Visible = false;
            btnEditBack.Visible = true;
            btnSubmit.Visible = true;
            btnEdit.Visible = false;
            lblInstallationTime.Visible = false;
            lblWorkman.Visible = false;
            lblInstallationDate.Visible = false;
            lblDuty.Visible = false;
            lblRemarks.Visible = false;
            txtDuty.Visible = true;
            txtInstallationTime.Visible = true;
            txtRemarks.Visible = true;
            txtWorkman.Visible = true;
            dtpDate.Visible = true;
            txtDuty.Text = duty;
            txtInstallationTime.Text = installationTime.ToString();
            txtRemarks.Text = remark;
            txtWorkman.Text = workmanId;
           // dtpDate.Text = installationDate;
        }

        private void txtInstallationTime_MouseEnter(object sender, EventArgs e)
        {
            if (txtInstallationTime.Text == "e.g 0840, 0940, 2200")
            {
                txtInstallationTime.Text = "";
                txtInstallationTime.ForeColor = DefaultForeColor;
                txtInstallationTime.ReadOnly = false;
            }
        }

        private void txtInstallationTime_MouseLeave(object sender, EventArgs e)
        {
            if (txtInstallationTime.Text == "")
            {
                txtInstallationTime.Text = "e.g 0840, 0940, 2200";
                txtInstallationTime.ForeColor = Color.Gray;
                txtInstallationTime.ReadOnly = true;
            }
        }

        private void txtInstallationTime_TextChanged(object sender, EventArgs e)
        {
            checkTime();
        }

        public bool checkTime()
        {
            int i = 1;
            String first = "";
            String second = "";
            String third = "";
            String fourth = "";
            foreach (var a in txtInstallationTime.Text)
            {
                switch (i)
                {
                    case 1:
                        first = a.ToString();
                        break;
                    case 2:
                        second = a.ToString();
                        break;
                    case 3:
                        third = a.ToString();
                        break;
                    case 4:
                        fourth = a.ToString();
                        break;
                }
                i++;
            }
            if (i == 5)
            {
                try
                {
                    if (Convert.ToInt32(third) >= 0 && Convert.ToInt32(third) <= 5)
                    {
                        Console.WriteLine(first + second);
                        if (Convert.ToInt32(first + second) >= 9 && Convert.ToInt32(first + second) <= 11 || first + second == "12" && third == "0" && fourth == "0")
                        {
                            lblSession.Text = "Morning";
                            return true;
                        }
                        else if (Convert.ToInt32(first + second) >= 13 && Convert.ToInt32(first + second) <= 17 || first + second == "17" && third == "0" && fourth == "0")
                        {
                            lblSession.Text = "Aftermoon";
                            return true;
                        }
                        else if (Convert.ToInt32(first + second) >= 18 && Convert.ToInt32(first + second) <= 22 || first + second == "22" && third == "0" && fourth == "0")
                        {
                            lblSession.Text = "Evening";
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("The session was not found on this time");
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("please input correct format on the time (last 2 decimal must be 00 - 59)");
                        return false;
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("please input correct number on the time item");
                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
            {
                return false;
            }
        }

        private void btnEditBack_Click(object sender, EventArgs e)
        {
            btnBack.Visible = true;
            btnEditBack.Visible = false;
            btnSubmit.Visible = false;
            lblInstallationTime.Visible = true;
            lblWorkman.Visible = true;
            lblInstallationDate.Visible = true;
            lblDuty.Visible = true;
            lblRemarks.Visible = true;
            txtDuty.Visible = false;
            txtInstallationTime.Visible = false;
            txtRemarks.Visible = false;
            txtWorkman.Visible = false;
            dtpDate.Visible = false;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!(txtInstallationTime.Text.Length < 4))
            {
                if (checkTimeNull())
                {
                    if (checkTime())
                    {
                        if (checkWorkman())
                        {
                            ConnectString rootconn = new ConnectString("root", "123456");
                            sqlConn.ConnectionString = rootconn.getString();

                            sqlConn.Open();
                            sqlCmd.Connection = sqlConn;
                            sqlCmd.CommandText = "update install_order set install_time = '"+txtInstallationTime.Text+"', " +
                                "worker_staff_id = '"+txtWorkman.Text+"', " +
                                "install_remark = '"+txtRemarks.Text+"', install_date = '"+dtpDate.Text+"'," +
                                " worker_duty = '"+txtDuty.Text+"' " +
                                "where install_order_id = '"+installationId+"' ";
                            sqlCmd.ExecuteNonQuery();
                            sqlConn.Close();

                            MessageBox.Show("Successful to update the delivery order");
                            updateValie();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please input 4 decimal on the installation time");
            }
        }

        public bool checkTimeNull()
        {
            if(txtInstallationTime.Text != "")
            {
                return true;
            }
            else
            {
                MessageBox.Show("The installation time cannot null");
                return false;
            }
        }

        public bool checkWorkman()
        {
            try
            {
                ConnectString rootconn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = rootconn.getString();
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select staff_id from staff where staff_id = '" + txtWorkman.Text + "' and staff_id like 'T%' and position = 1";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    return true;
                }
                sqlRd.Close();
                sqlConn.Close();
                MessageBox.Show("The workman is no found");
                return false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                sqlRd.Close();
                sqlConn.Close();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (action == "table")
            {
                Main.instance.TechnicalTable();
            }
            else if (action == "record")
            {
                Main.instance.TechnicalRecord();
            }
        }

        public void setAction(String action)
        {
            this.action = action;
        }
    }
}
