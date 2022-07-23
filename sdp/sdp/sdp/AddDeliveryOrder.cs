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
    public partial class AddDeliveryOrder : Form
    {
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;

        String order;
        private string language;

        public AddDeliveryOrder()
        {
            InitializeComponent();
        }

        private void AddDeliveryOrder_Load(object sender, EventArgs e)
        {
            setStaff();
            changeLanguage();
            txtTime.Text = "e.g 0840, 0940, 2200";
            txtTime.ForeColor = Color.Gray;
         //   dtpDate.MinDate = DateTime.Today;
            dtpDate.CustomFormat = "yyyy-MM-dd";
            dtpDate.Format = DateTimePickerFormat.Custom;
        }

        public void setStaff()
        {
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select staff_id from staff where department = 2 and position = 1";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                cbxPosition.Items.Add(sqlRd.GetString(0));
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
                label1.Text = "选择日期:";
                label2.Text = "发票号:";
                label3.Text = "客户姓名:";
                label4.Text = "客户电话 :";
                label5.Text = "员工编号:";
                label6.Text = "备注:";
                btnBack.Text = "返回";
                label7.Text = "已选时间段: ";
                label8.Text = "时间:";
                label9.Text = "送货地址:";
                btnSubmit.Text = "提交";
            }
            else
            {
                label1.Text = "Selected Date:";
                label2.Text = "Sales Invoice Id:";
                label3.Text = "Customer Name:";
                label4.Text = "Customer Phone No. :";
                label5.Text = "Staff Id:";
                label6.Text = "Remarks:";
                btnBack.Text = "BACK";
                label7.Text = "Selected Session: ";
                label8.Text = "Time:";
                label9.Text = "Delivery Address:";
                btnSubmit.Text = "SUBMIT";
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (checkTime())
            {
                try
                {
                    if (txtInvoice.Text != "")
                    {
                        if (txtName.Text != "")
                        {
                            if (cbxPosition.Text != "")
                            {
                                if (txtAddress.Text != "")
                                {
                                    if (txtPhone.Text.Length == 8)
                                    {
                                        if (checkphone())
                                        {
                                            if (checkNumberOfSession(lblSession.Text) < 5)
                                            {
                                                Console.WriteLine(checkNumberOfSession(lblSession.Text));
                                                ConnectString conn = new ConnectString("root", "123456");
                                                sqlConn.ConnectionString = conn.getString();
                                                sqlConn.Open();
                                                sqlQuery = "select order_id from sales_receipt where invoice_id = '" + txtInvoice.Text + "'";
                                                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                                                sqlRd = sqlCmd.ExecuteReader();
                                                while (sqlRd.Read())
                                                {
                                                    order = sqlRd.GetString(0);
                                                }
                                                sqlConn.Close();
                                                sqlConn.Open();
                                                sqlQuery = "insert into delivery_order values(DEFAULT,'" + txtAddress.Text + "','" + txtName.Text + "','" + txtPhone.Text + "', " +
                                                      order + "," + txtInvoice.Text + "," + txtTime.Text + ",'" + dtpDate.Text + "', '" + txtRemark.Text + "' ,'" + cbxPosition.Text + "',DEFAULT)";
                                                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                                                sqlRd = sqlCmd.ExecuteReader();

                                                sqlConn.Close();
                                                MessageBox.Show("Successful to add the delivery order");
                                            }
                                            else
                                            {
                                                MessageBox.Show("the date and the session already full, please choose another date or session");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("the phone number must be 8 digital number");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("the address cannot null");
                                }
                            }
                            else
                            {
                                MessageBox.Show("please select a staff");
                            }
                        }
                        else
                        {
                            MessageBox.Show("the name cannot null");
                        }
                    }
                    else
                    {
                        MessageBox.Show("the invoice id cannot null");
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("the phone number is incorrect format");
                }
                catch (MySqlException)
                {
                    MessageBox.Show("the invoice id or staff id may not exist");
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    sqlConn.Close();
                }
            }
            else
            {
                MessageBox.Show("please input correct time");
            }
            
        }

        private void txtTime_TextChanged(object sender, EventArgs e)
        {
            checkTime();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.DeliveryTable();
        }

        private void txtTime_MouseLeave(object sender, EventArgs e)
        {
            if (txtTime.Text == "") 
            { 
                txtTime.Text = "e.g 0840, 0940, 2200";
                txtTime.ForeColor = Color.Gray;
                txtTime.ReadOnly = true;
            }
        }

        private void txtTime_MouseEnter(object sender, EventArgs e)
        {
            if(txtTime.Text == "e.g 0840, 0940, 2200")
            {
                txtTime.Text = "";
                txtTime.ForeColor = DefaultForeColor;
                txtTime.ReadOnly = false;
            }
        }

        private void AddDeliveryOrder_TextChanged(object sender, EventArgs e)
        {
        }

        public bool checkTime()
        {
            int i = 1;
            String first = "";
            String second = "";
            String third = "";
            String fourth = "";
            foreach (var a in txtTime.Text)
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
                        if (Convert.ToInt32(first + second) >= 9 && Convert.ToInt32(first + second) <= 12 || first + second == "12" && third == "0" && fourth == "0")
                        {
                            if(language == "Chinese")
                                lblSession.Text = "早上";
                            else
                                lblSession.Text = "Morning";
                            return true;
                        }
                        else if (Convert.ToInt32(first + second) >= 13 && Convert.ToInt32(first + second) <= 17 || first + second == "17" && third == "0" && fourth == "0")
                        {
                            if (language == "Chinese")
                                lblSession.Text = "中午";
                            else
                                lblSession.Text = "Afternoon";
                            return true;
                        }
                        else if (Convert.ToInt32(first + second) >= 18 && Convert.ToInt32(first + second) <= 22 || first + second == "22" && third == "0" && fourth == "0")
                        {
                            if (language == "Chinese")
                                lblSession.Text = "晚上";
                            else
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
                        MessageBox.Show("Please input correct format on the time (last 2 decimal must be 00 - 59)");
                        return false;
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("Please input correct number on the time item");
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

        public bool checkphone()
        {
            try
            {
                foreach (var a in txtPhone.Text)
                {
                    Convert.ToInt32(a.ToString());
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int checkNumberOfSession(String session)
        {
            try
            {
                String numberSql = "";
                switch (session)
                {
                    case "Afternoon":
                        numberSql = "select count(*) " +
                        "from delivery_order " +
                        "where delivery_time <= 1700 and delivery_time >= 1300 " +
                        "group by delivery_date " +
                        "having delivery_date = '" + dtpDate.Text + "'";
                        break;
                    case "Morning":
                        numberSql = "select count(*) " +
                        "from delivery_order " +
                        "where delivery_time >= 0900 and delivery_time <= 1200 " +
                        "group by delivery_date " +
                        "having delivery_date = '" + dtpDate.Text + "'";
                        break;
                    case "Evening":
                        numberSql = "select count(*) " +
                        "from delivery_order " +
                        "where delivery_time <= 2200 and delivery_time >= 1800 " +
                        "group by delivery_date " +
                        "having delivery_date = '" + dtpDate.Text + "'";
                        break;
                    case "早上":
                        numberSql = "select count(*) " +
                        "from delivery_order " +
                       "where delivery_time >= 0900 and delivery_time <= 1200  " +
                        "group by delivery_date " +
                        "having delivery_date = '" + dtpDate.Text + "'";
                        break;
                    case "中午":
                        numberSql = "select count(*) " +
                        "from delivery_order " +
                        "where delivery_time <= 1700 and delivery_time >= 1300 " +
                        "group by delivery_date " +
                        "having delivery_date = '" + dtpDate.Text + "'";
                        break;
                    case "晚上":
                        numberSql = "select count(*) " +
                        "from delivery_order " +
                        "where delivery_time <= 2200 and delivery_time >= 1800 " +
                        "group by delivery_date " +
                        "having delivery_date = '" + dtpDate.Text + "'";
                        break;
                }
                ConnectString conn = new ConnectString("root", "123456");
                Console.WriteLine(numberSql);
                sqlConn.ConnectionString = conn.getString();
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = numberSql;
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    return sqlRd.GetInt32(0);
                }
                return 0;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
            finally
            {
                sqlConn.Close();
                sqlRd.Close();
            }
        }
    }
}
