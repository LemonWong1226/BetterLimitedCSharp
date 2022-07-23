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
    public partial class NewTechnical : Form
    {
        String order;
        string invoice = "";

        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;

        public NewTechnical()
        {
            InitializeComponent();
        }

        private void NewTechnical_Load(object sender, EventArgs e)
        {
            setStaff();
            changeLanguage();
            txtTime.Text = "e.g 0840, 0940, 2200";
            txtTime.ForeColor = Color.Gray;
            dtpDate.MinDate = DateTime.Today;
            dtpDate.CustomFormat = "yyyy-MM-dd";
            dtpDate.Format = DateTimePickerFormat.Custom;
        }

        public void setStaff()
        {
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select staff_id from staff where department = 3 and position = 1";
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
                label5.Text = "技术员编号:";
                label6.Text = "备注:";
                btnBack.Text = "返回";
                label7.Text = "时间段: ";
                label8.Text = "时间:";
                label3.Text = "送货单号:";
                btnSubmit.Text = "提交";
                label4.Text = "工人职责:";
            }
            else
            {
                label1.Text = "Selected Date:";
                label2.Text = "Sales Invoice Id:";
                label5.Text = "Worker Staff Id:";
                label6.Text = "Remarks:";
                btnBack.Text = "BACK";
                label7.Text = "Selected Session: ";
                label8.Text = "Time:";
                label3.Text = "Delivery Order Id:";
                btnSubmit.Text = "SUBMIT";
                label4.Text = "Worker Duty :";
            }
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
                        if (Convert.ToInt32(first + second) >= 9 && Convert.ToInt32(first + second) <= 11 || first + second == "12" && third == "0" && fourth == "0")
                        {
                            if (language == "Chinese")
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
            if (txtTime.Text == "e.g 0840, 0940, 2200")
            {
                txtTime.Text = "";
                txtTime.ForeColor = DefaultForeColor;
                txtTime.ReadOnly = false;
            }
        }

        private void txtTime_TextChanged(object sender, EventArgs e)
        {
            checkTime();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.TechnicalTable();
        }

        public void checkBeforeTime()
        {

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Console.WriteLine(checkNumberOfSession(lblSession.Text)); 
            int status = 0;
            int delivery_time = 0;
            DateTime delivery_date = DateTime.Now;
            try
            {
                if (checkTime())
            {
                if (checkNumberOfSession(lblSession.Text) < 5)
                {

                        sqlConn.Open();
                        sqlQuery = "select invoice_id, delivery_status, delivery_time, delivery_date " +
                            "from delivery_order " +
                            "where delivery_order_id = '" + txtDelivertOrder.Text + "'";
                        Console.WriteLine(sqlQuery);
                        sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                        sqlRd = sqlCmd.ExecuteReader();
                        while (sqlRd.Read())
                        {
                            invoice = sqlRd.GetString(0);
                            status = sqlRd.GetInt32(1);
                            delivery_time = sqlRd.GetInt32(2);
                            delivery_date = sqlRd.GetDateTime(3);
                        }
                        sqlConn.Close();
                        sqlRd.Close();
                        if (invoice != "")
                        {
                            if (status == 2)
                            {
                                if (dtpDate.Value.Date > delivery_date.Date)
                                {
                                    if (cbxPosition.Text != "")
                                    {
                                        if (txtDelivertOrder.Text != "")
                                        {
                                            Console.WriteLine("run");
                                            ConnectString conn = new ConnectString("root", "123456");
                                            sqlConn.ConnectionString = conn.getString();
                                            sqlConn.Open();
                                            sqlQuery = "select order_id from delivery_order where delivery_order_id = '" + txtDelivertOrder.Text + "'";
                                            sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                                            sqlRd = sqlCmd.ExecuteReader();
                                            while (sqlRd.Read())
                                            {
                                                order = sqlRd.GetString(0);
                                            }
                                            sqlConn.Close();
                                            sqlRd.Close();

                                            sqlConn.Open();
                                            sqlQuery = "insert into install_order values(default, '" + txtDelivertOrder.Text + "', '" +
                                                order + "','" + invoice + "','" + txtTime.Text + "','" + dtpDate.Text + "','1','" + Main.instance.Staff + "', '" +
                                                cbxPosition.Text + "','" + txtDuty.Text + "', '" + txtRemark.Text + "')";
                                            Console.WriteLine(sqlQuery);
                                            sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                                            sqlRd = sqlCmd.ExecuteReader();

                                            sqlConn.Close();
                                            sqlRd.Close();
                                            MessageBox.Show("Successful to add the installation order");
                                        }
                                        else
                                        {
                                            MessageBox.Show("The delivery order id cannot null");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("please select a staff");
                                    }
                                }
                                else
                                {
                                    if (delivery_time + 200 <= Convert.ToInt32(txtTime.Text))
                                    {
                                        if (cbxPosition.Text != "")
                                        {
                                            if (txtDelivertOrder.Text != "")
                                            {
                                                ConnectString conn = new ConnectString("root", "123456");
                                                sqlConn.ConnectionString = conn.getString();
                                                sqlConn.Open();
                                                sqlQuery = "select order_id from delivery_order where delivery_order_id = '" + txtDelivertOrder.Text + "'";
                                                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                                                sqlRd = sqlCmd.ExecuteReader();
                                                while (sqlRd.Read())
                                                {
                                                    order = sqlRd.GetString(0);
                                                }
                                                sqlConn.Close();
                                                sqlRd.Close();

                                                sqlConn.Open();
                                                sqlQuery = "insert into install_order values(default, '" + txtDelivertOrder.Text + "', '" +
                                                    order + "','" + invoice + "','" + txtTime.Text + "','" + dtpDate.Text + "','1','" + Main.instance.Staff + "', '" +
                                                    cbxPosition.Text + "','" + txtDuty.Text + "', '" + txtRemark.Text + "')";
                                                Console.WriteLine(sqlQuery);
                                                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                                                sqlRd = sqlCmd.ExecuteReader();

                                                sqlConn.Close();
                                                sqlRd.Close();
                                                MessageBox.Show("Successful to add the installation order");
                                            }
                                            else
                                            {
                                                MessageBox.Show("The delivery order id cannot null");
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("please select a staff");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("The date or time must be more than 2 hour with confirm delivery order");
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("The delivery order must me confirm");
                            }
                        }
                        else
                        {
                            MessageBox.Show("The delivery id does not exist");
                        }

                }
                else
                {
                    MessageBox.Show("the date and the session already full, please choose another date or session");
                }
            }
            else
            {
                MessageBox.Show("Please input correct time");
            }
            }
            /*      catch (MySqlException)
                  {
                      MessageBox.Show("the invoice id or worker staff id may not exist");
                  }*/
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConn.Close();
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
                        "from install_order " +
                        "where install_time >= 1300 and install_time <= 1700 " +
                        "group by install_date " +
                        "having install_date = '" + dtpDate.Text + "'";
                        break;
                    case "Morning":
                        numberSql = "select count(*) " +
                        "from install_order " +
                        "where install_time >= 0900 and install_time <= 1200 " +
                        "group by install_date " +
                        "having install_date = '" + dtpDate.Text + "'";
                        break;
                    case "Evening":
                        numberSql = "select count(*) " +
                        "from install_order " +
                        "where install_time >= 1800 and install_time <= 2200 " +
                        "group by install_date " +
                        "having install_date = '" + dtpDate.Text + "'";
                        break;
                    case "早上":
                        numberSql = "select count(*) " +
                        "from install_order " +
                        "where install_time >= 0900 and install_time <= 1200 " +
                        "group by install_date " +
                        "having install_date = '" + dtpDate.Text + "'";
                        break;
                    case "中午":
                        numberSql = "select count(*) " +
                        "from install_order " +
                        "where install_time >= 1300 and install_time <= 1700 " +
                        "group by install_date " +
                        "having install_date = '" + dtpDate.Text + "'";
                        break;
                    case "晚上":
                        numberSql = "select count(*) " +
                        "from install_order " +
                        "where install_time >= 1800 and install_time <= 2200 " +
                        "group by install_date " +
                        "having install_date = '" + dtpDate.Text + "'";
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
            catch (Exception ex)
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
