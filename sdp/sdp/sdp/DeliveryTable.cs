using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using MySql.Data.MySqlClient;
using System.IO;
using Newtonsoft.Json;

namespace sdp
{
    public partial class DeliveryOrder : Form
    {
        String status;
        bool isSelect = false;
        public static DeliveryOrder instance;
        int morningPage;
        int aftermoonPage;
        int eveningPage;
        int maxPage;
        public String selectOrder;
        int current = 1;
        String deliveryDate;
        String nowDate;
        Queue morning = new Queue();
        Queue aftermoon = new Queue();
        Queue evening = new Queue();

        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;
        private string JSONlist;

        public DeliveryOrder()
        {
            InitializeComponent();
            instance = this;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            setPermission(Main.instance.dept, Main.instance.position);
            changeLanguage();
            setStaff();
            btnNext.Visible = false;
            btnPrevious.Visible = false;
            txtCurrentPage.Visible = false;
            nowDate = DateTime.Now.ToString("yyyy-MM-dd");
            if(language == "Chinese")
                lblDate.Text = "日期:\n" + nowDate;
            else
                lblDate.Text = "Date:\n" + nowDate;
            sqlConn.ConnectionString = ConnectString.ConnectionString;
            morning.Clear();
            aftermoon.Clear();
            evening.Clear();
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select delivery_order_id , delivery_status " +
            "from delivery_order " +
            "where delivery_date = '" + nowDate + "' and " +
            "delivery_time >= 900 and delivery_time <= 1200 " +
            "and delivery_status in (1, 2)";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                numberToStatus();
                morning.Enqueue("Order id: " + sqlRd.GetString(0));
                //         morning.Enqueue("Order id: " + sqlRd.GetString(0) + "\nDelivery Status: " + status);
            }
            sqlConn.Close();

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select delivery_order_id , delivery_status " +
            "from delivery_order " +
            "where delivery_date = '" + nowDate + "' and " +
            "delivery_time >= 1300 and delivery_time <= 1700 " +
            "and delivery_status in (1, 2)";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                numberToStatus();
                //        aftermoon.Enqueue("Order id: " + sqlRd.GetString(0) + "\nDelivery Status: " + status);
                aftermoon.Enqueue("Order id: " + sqlRd.GetString(0));
            }
            sqlConn.Close();

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select delivery_order_id , delivery_status " +
            "from delivery_order " +
            "where delivery_date = '" + nowDate + "' and " +
            "delivery_time >= 1800 and delivery_time <= 2200 " +
            "and delivery_status in (1, 2)";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                numberToStatus();
                evening.Enqueue("Order id: " + sqlRd.GetString(0));
                //       evening.Enqueue("Order id: "+sqlRd.GetString(0) + "\nDelivery Status: " + status);
            }
            sqlConn.Close();

            morningPage = Convert.ToInt32(Math.Ceiling((double) morning.Count / 5));
            aftermoonPage = Convert.ToInt32(Math.Ceiling((double)aftermoon.Count / 5));
            eveningPage = Convert.ToInt32(Math.Ceiling((double)evening.Count / 5));
            maxPage = Math.Max(Math.Max(morningPage, aftermoonPage),eveningPage);
            writeMorningPage(current);
            writeAftermoonPage(current);
            writeEveningPage(current);
            txtCurrentPage.Text = "1";
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
                    case 35:
                        btnNew.Visible = true;
                        break;
                    case 70:
                        btnView.Visible = true;
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
                label2.Text = "早上\n(9:00 - 12:00)";
                label3.Text = "中午\n(9:00 - 12:00)";
                label4.Text = "晚上\n(9:00 - 12:00)";
                btnBack.Text = "返回";
                btnPrevious.Text = "上一页";
                btnNext.Text = "下一页";
                button1.Text = "所有记录";
                btnDate.Text = "选择日期";
                btnNew.Text = "新增订单";
                btnView.Text = "详情";
            }
            else
            {
                label2.Text = "Morning\n(9:00 - 12:00)";
                label3.Text = "Aftermoon\n(9:00 - 12:00)";
                label4.Text = "Evening\n(9:00 - 12:00)";
                btnBack.Text = "BACK";
                btnPrevious.Text = "PREVIOUS";
                btnNext.Text = "NEXT";
                button1.Text = "View All Order";
                btnDate.Text = "Select Date";
                btnNew.Text = "New Order";
                btnView.Text = "View Order";
            }
        }

        public void numberToStatus()
        {
            switch (sqlRd.GetInt32(1))
            {
                case 1:
                    status = "Request Delivered";
                    break;
                case 2:
                    status = "Request Confirmed";
                    break;
                case 3:
                    status = "Request Rejected";
                    break;
                case 4:
                    status = "Request Cancelled";
                    break;
            }
        }


        public void writeAftermoonPage(int selectPage)
        {
            selectPage -= 1;
            rbn01.Text = "";
            rbn11.Text = "";
            rbn21.Text = "";
            rbn31.Text = "";
            rbn41.Text = "";
            if (selectPage == aftermoonPage - 1)
            {
                switch (aftermoon.Count % 5)
                {
                    case 1:
                        rbn01.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage).ToString();
                        break;
                    case 2:
                        rbn01.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage).ToString();
                        rbn11.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 1).ToString();
                        break;
                    case 3:
                        rbn01.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 0).ToString();
                        rbn11.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 1).ToString();
                        rbn21.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 2).ToString();
                        break;
                    case 4:
                        rbn01.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 0).ToString();
                        rbn11.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 1).ToString();
                        rbn21.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 2).ToString();
                        rbn31.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 3).ToString();
                        break;
                    case 0:
                        rbn01.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 0).ToString();
                        rbn11.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 1).ToString();
                        rbn21.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 2).ToString();
                        rbn31.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 3).ToString();
                        rbn41.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 4).ToString();
                        break;
                }
            }
            else if(selectPage >= 0 && selectPage < aftermoonPage - 1)
            {
                rbn01.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 0).ToString();
                rbn11.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 1).ToString();
                rbn21.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 2).ToString();
                rbn31.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 3).ToString();
                rbn41.Text = aftermoon.ToArray().ToList().ElementAt(5 * selectPage + 4).ToString();
            }
        }

        public void writeMorningPage(int selectPage)
        {
            selectPage -= 1;
            rbn00.Text = "";
            rbn10.Text = "";
            rbn20.Text = "";
            rbn30.Text = "";
            rbn40.Text = "";
            if (selectPage == morningPage - 1)
            {
                switch (morning.Count % 5)
                {
                    case 1:
                        rbn00.Text = morning.ToArray().ToList().ElementAt(5 * selectPage).ToString();
                        break;
                    case 2:
                        rbn00.Text = morning.ToArray().ToList().ElementAt(5 * selectPage).ToString();
                        rbn10.Text = morning.ToArray().ToList().ElementAt(5 * selectPage + 1).ToString();
                        break;
                    case 3:
                        rbn00.Text = morning.ToArray().ToList().ElementAt(5 * selectPage + 0).ToString();
                        rbn10.Text = morning.ToArray().ToList().ElementAt(5 * selectPage + 1).ToString();
                        rbn20.Text = morning.ToArray().ToList().ElementAt(5 * selectPage + 2).ToString();
                        break;
                    case 4:
                        rbn00.Text = morning.ToArray().ToList().ElementAt(5 * selectPage + 0).ToString();
                        rbn10.Text = morning.ToArray().ToList().ElementAt(5 * selectPage + 1).ToString();
                        rbn20.Text = morning.ToArray().ToList().ElementAt(5 * selectPage + 2).ToString();
                        rbn30.Text = morning.ToArray().ToList().ElementAt(5 * selectPage + 3).ToString();
                        break;
                    case 0:
                        rbn00.Text = morning.ToArray().ToList().ElementAt(5 * selectPage + 0).ToString();
                        rbn10.Text = morning.ToArray().ToList().ElementAt(5 * selectPage + 1).ToString();
                        rbn20.Text = morning.ToArray().ToList().ElementAt(5 * selectPage + 2).ToString();
                        rbn30.Text = morning.ToArray().ToList().ElementAt(5 * selectPage + 3).ToString();
                        rbn40.Text = morning.ToArray().ToList().ElementAt(5 * selectPage + 4).ToString();
                        break;
                }
            }
            else if (selectPage >= 0 && selectPage < morningPage - 1)
            {
                rbn00.Text = morning.ToArray().ToList().ElementAt(5* selectPage + 0).ToString();
                rbn10.Text = morning.ToArray().ToList().ElementAt(5* selectPage + 1).ToString();
                rbn20.Text = morning.ToArray().ToList().ElementAt(5* selectPage + 2).ToString();
                rbn30.Text = morning.ToArray().ToList().ElementAt(5* selectPage + 3).ToString();
                rbn40.Text = morning.ToArray().ToList().ElementAt(5* selectPage + 4).ToString();
            }
        }

        public void writeEveningPage(int selectPage)
        {
            selectPage -= 1;
            rbn02.Text = "";
            rbn12.Text = "";
            rbn22.Text = "";
            rbn32.Text = "";
            rbn42.Text = "";
            if (selectPage == eveningPage - 1)
            {
                switch (evening.Count % 5)
                {
                    case 1:
                        rbn02.Text = evening.ToArray().ToList().ElementAt(5 * selectPage).ToString();
                        break;
                    case 2:
                        rbn02.Text = evening.ToArray().ToList().ElementAt(5 * selectPage).ToString();
                        rbn12.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 1).ToString();
                        break;
                    case 3:
                        rbn02.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 0).ToString();
                        rbn12.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 1).ToString();
                        rbn22.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 2).ToString();
                        break;
                    case 4:
                        rbn02.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 0).ToString();
                        rbn12.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 1).ToString();
                        rbn22.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 2).ToString();
                        rbn32.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 3).ToString();
                        break;
                    case 0:
                        rbn02.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 0).ToString();
                        rbn12.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 1).ToString();
                        rbn22.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 2).ToString();
                        rbn32.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 3).ToString();
                        rbn42.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 4).ToString();
                        break;
                }
            }
            else if (selectPage >= 0 && selectPage < eveningPage - 1)
            {
                rbn02.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 0).ToString();
                rbn12.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 1).ToString();
                rbn22.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 2).ToString();
                rbn32.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 3).ToString();
                rbn42.Text = evening.ToArray().ToList().ElementAt(5 * selectPage + 4).ToString();
            }
        }

        public void changeText(int selectPage)
        {
            selectPage -= 1;
            lblNo1.Text = (5 * selectPage + 1).ToString();
            lblNo2.Text = (5 * selectPage + 2).ToString();
            lblNo3.Text = (5 * selectPage + 3).ToString();
            lblNo4.Text = (5 * selectPage + 4).ToString();
            lblNo5.Text = (5 * selectPage + 5).ToString();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (current < maxPage && current >= 1)
            {
                current += 1;
                writeMorningPage(current);
                writeAftermoonPage(current);
                writeEveningPage(current);
                changeText(current);
                txtCurrentPage.Text = current.ToString();
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (current <= maxPage && current > 1)
            {
                current -= 1;
                writeMorningPage(current);
                writeAftermoonPage(current);
                writeEveningPage(current);
                changeText(current);
                txtCurrentPage.Text = current.ToString();
            }
        }

        private void txtCurrentPage_TextChanged(object sender, EventArgs e)
        {/*
            try
            {
                current = Convert.ToInt32(txtCurrentPage.Text);
                if (current <= morningPage && current >= 1)
                {
                    writeMorningPage(current);
                    txtCurrentPage.Text = current.ToString();
                }
                else
                {
                    MessageBox.Show("please input correly page");
                }
            }
            catch(FormatException)
            {
                MessageBox.Show("please input integer");
            }*/
        }

        private void btnDate_Click(object sender, EventArgs e)
        {
            selectDate date = new selectDate();
            date.ShowDialog();
            deliveryDate = date.getDate();
            if (language == "Chinese")
                lblDate.Text = "日期:\n" + deliveryDate;
            else
                lblDate.Text = "Date:\n" + deliveryDate;
            btnNext.Visible = false;
            btnPrevious.Visible = false;
            txtCurrentPage.Visible = false;
            sqlConn.ConnectionString = ConnectString.ConnectionString;
            morning.Clear();
            aftermoon.Clear();
            evening.Clear();
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select delivery_order_id , delivery_status " +
            "from delivery_order " +
            "where delivery_date = '" + deliveryDate + "' and " +
            "delivery_time >= 900 and delivery_time <= 1200 " +
            "and delivery_status in (1, 2)";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                numberToStatus();
                morning.Enqueue("Order id: " + sqlRd.GetString(0));
            //    morning.Enqueue("Order id: " + sqlRd.GetString(0) + "\nDelivery Status: " + status);
            }
            sqlConn.Close();

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select delivery_order_id , delivery_status " +
            "from delivery_order " +
            "where delivery_date = '" + deliveryDate + "' and " +
            "delivery_time >= 1300 and delivery_time <= 1700 " +
            "and delivery_status in (1, 2)";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                numberToStatus();
                aftermoon.Enqueue("Order id: " + sqlRd.GetString(0));
                //        aftermoon.Enqueue("Order id: " + sqlRd.GetString(0) + "\nDelivery Status: " + status);
            }
            sqlConn.Close();

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select delivery_order_id , delivery_status " +
            "from delivery_order " +
            "where delivery_date = '" + deliveryDate + "' and " +
            "delivery_time >= 1800 and delivery_time <= 2200 " +
            "and delivery_status in (1, 2)";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                numberToStatus();
                evening.Enqueue("Order id: " + sqlRd.GetString(0));
                //     evening.Enqueue("Order id: " + sqlRd.GetString(0) + "\nDelivery Status: " + status);
            }
            sqlConn.Close();
            morningPage = Convert.ToInt32(Math.Ceiling((double)morning.Count / 5));
            aftermoonPage = Convert.ToInt32(Math.Ceiling((double)aftermoon.Count / 5));
            eveningPage = Convert.ToInt32(Math.Ceiling((double)evening.Count / 5));
            maxPage = Math.Max(Math.Max(morningPage, aftermoonPage), eveningPage);
            writeMorningPage(current);
            writeAftermoonPage(current);
            writeEveningPage(current);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnNext.Visible = true;
            btnPrevious.Visible = true;
            txtCurrentPage.Visible = true;
            sqlConn.ConnectionString = ConnectString.ConnectionString;
            morning.Clear();
            aftermoon.Clear();
            evening.Clear();
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select delivery_order_id , delivery_status , delivery_date " +
            "from delivery_order " +
            "where " +
            "delivery_time >= 1300 and delivery_time <= 1700 " +
            "and delivery_status in (1, 2)";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                numberToStatus();
                aftermoon.Enqueue("Order id: " + sqlRd.GetString(0) + " \nDate: " + sqlRd.GetDateTime(2).ToString("yyyy-MM-dd"));
              //  aftermoon.Enqueue("Order id: " + sqlRd.GetString(0) + "\nDelivery Status: " + status);
            }
            sqlRd.Close();
            sqlConn.Close();

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select delivery_order_id , delivery_status , delivery_date " +
            "from delivery_order " +
            "where " +
            "delivery_time >= 1800 and delivery_time <= 2200 " +
            "and delivery_status in (1, 2)";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                numberToStatus();
                evening.Enqueue("Order id: " + sqlRd.GetString(0) + " \nDate: " + sqlRd.GetDateTime(2).ToString("yyyy-MM-dd"));
        //        evening.Enqueue("Order id: " + sqlRd.GetString(0) + "\nDelivery Status: " + status);
            }
            sqlConn.Close();

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select delivery_order_id , delivery_status , delivery_date " +
            "from delivery_order " +
            "where " +
            "delivery_time >= 900 and delivery_time <= 1200 "+
            "and delivery_status in (1, 2)";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                numberToStatus();
                morning.Enqueue("Order id: " + sqlRd.GetString(0) + " \nDate: " + sqlRd.GetDateTime(2).ToString("yyyy-MM-dd"));
             //   morning.Enqueue("Order id: " + sqlRd.GetString(0) + "\nDelivery Status: " + status);
            }
            sqlConn.Close();
            lblDate.Text = "";
            morningPage = Convert.ToInt32(Math.Ceiling((double)morning.Count / 5));
            aftermoonPage = Convert.ToInt32(Math.Ceiling((double)aftermoon.Count / 5));
            eveningPage = Convert.ToInt32(Math.Ceiling((double)evening.Count / 5));
            maxPage = Math.Max(Math.Max(morningPage, aftermoonPage), eveningPage);
            writeMorningPage(current);
            writeAftermoonPage(current);
            writeEveningPage(current);
        }

        private void rbn00_CheckedChanged(object sender, EventArgs e)
        {
            if (rbn00.Text != "")
            {
                selectOrder = rbn00.Text;
                isSelect = true;
            }
            else
            {
                selectOrder = "";
            }
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            if (rbn41.Text != "")
            {
                selectOrder = rbn41.Text;
                isSelect = true;
            }
            else
            {
                selectOrder = "";
            }
        }

        private void rbn10_CheckedChanged(object sender, EventArgs e)
        {
            if (rbn10.Text != "")
            {
                selectOrder = rbn10.Text;
                isSelect = true;
            }
            else
            {
                selectOrder = "";
            }
        }

        private void rbn20_CheckedChanged(object sender, EventArgs e)
        {
            if (rbn20.Text != "")
            {
                selectOrder = rbn20.Text;
                isSelect = true;
            }
            else
            {
                selectOrder = "";
            }
        }

        private void rbn30_CheckedChanged(object sender, EventArgs e)
        {
            if (rbn30.Text != "")
            {
                selectOrder = rbn30.Text;
                isSelect = true;
            }
            else
            {
                selectOrder = "";
            }
        }

        private void rbn40_CheckedChanged(object sender, EventArgs e)
        {
            if (rbn40.Text != "")
            {
                selectOrder = rbn40.Text;
                isSelect = true;
            }
            else
            {
                selectOrder = "";
            }
        }

        private void rbn01_CheckedChanged(object sender, EventArgs e)
        {
            if(rbn01.Text != "")
            {
                selectOrder = rbn01.Text;
                isSelect = true;
            }
            else
            {
                selectOrder = "";
            }
        }

        private void rbn11_CheckedChanged(object sender, EventArgs e)
        {
            if (rbn11.Text != "")
            {
                selectOrder = rbn11.Text;
                isSelect = true;
            }
            else
            {
                selectOrder = "";
            }
        }

        private void rbn21_CheckedChanged(object sender, EventArgs e)
        {
            if (rbn21.Text != "")
            {
                selectOrder = rbn21.Text;
                isSelect = true;
            }
            else
            {
                selectOrder = "";
            }
        }

        private void rbn31_CheckedChanged(object sender, EventArgs e)
        {
            if (rbn31.Text != "")
            {
                selectOrder = rbn31.Text;
                isSelect = true;
            }
            else
            {
                selectOrder = "";
            }
        }

        private void rbn02_CheckedChanged(object sender, EventArgs e)
        {
            if (rbn02.Text != "")
            {
                selectOrder = rbn02.Text;
                isSelect = true;
            }
            else
            {
                selectOrder = "";
            }
        }

        private void rbn12_CheckedChanged(object sender, EventArgs e)
        {
            if (rbn12.Text != "")
            {
                selectOrder = rbn12.Text;
                isSelect = true;
            }
            else
            {
                selectOrder = "";
            }
        }

        private void rbn22_CheckedChanged(object sender, EventArgs e)
        {
            if (rbn22.Text != "")
            {
                selectOrder = rbn22.Text;
                isSelect = true;
            }
            else
            {
                selectOrder = "";
            }
        }

        private void rbn32_CheckedChanged(object sender, EventArgs e)
        {
            if (rbn32.Text != "")
            {
                selectOrder = rbn32.Text;
                isSelect = true;
            }
            else
            {
                selectOrder = "";
            }
        }

        private void rbn42_CheckedChanged(object sender, EventArgs e)
        {
            if (rbn42.Text != "")
            {
                selectOrder = rbn42.Text;
                isSelect = true;
            }
            else
            {
                selectOrder = "";
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            foreach (var control in this.Controls)
            {
                TableLayoutPanel tb = control as TableLayoutPanel;
                if (null != tb)
                {
                    foreach (var rb in tb.Controls.OfType<RadioButton>())
                    {
                        if (rb.Checked)
                        {
                            selectOrder = rb.Text;
                        }
                    }
                }
            }
            if(selectOrder != "")
            {
                Main.instance.DeliveryDetail("table");
            }
            else
            {
                MessageBox.Show("Please at least choose one order");
            }
            /*    if (isSelect)
                {
                    if (selectOrder != "")
                    {
                        Main.instance.DeliveryDetail("table");
                    }
                    else
                    {
                        MessageBox.Show("You cannot choose null order");
                    }
                }
                else
                {
                    MessageBox.Show("Please at least choose one order");
                }*/
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Main.instance.addDeliveryOrder();
        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.DeliveryHome();
        }

        private void btnStaff_Click(object sender, EventArgs e)
        {
            if (cbxPosition.Text != "")
            {
                btnNext.Visible = true;
                btnPrevious.Visible = true;
                txtCurrentPage.Visible = true;
                sqlConn.ConnectionString = ConnectString.ConnectionString;
                morning.Clear();
                aftermoon.Clear();
                evening.Clear();
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select delivery_order_id , delivery_status , delivery_date " +
                "from delivery_order " +
                "where " +
                "delivery_time >= 1300 and delivery_time <= 1700 " +
                "and delivery_status in (1, 2)" +
                "and staff_id = '" + cbxPosition.Text + "'";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    numberToStatus();
                    aftermoon.Enqueue("Order id: " + sqlRd.GetString(0) +" \nDate: " + sqlRd.GetDateTime(2).ToString("yyyy-MM-dd"));
                    //  aftermoon.Enqueue("Order id: " + sqlRd.GetString(0) + "\nDelivery Status: " + status);
                }
                sqlRd.Close();
                sqlConn.Close();

                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select delivery_order_id , delivery_status , delivery_date " +
                "from delivery_order " +
                "where " +
                "delivery_time >= 1800 and delivery_time <= 2200 " +
                "and delivery_status in (1, 2)" +
                "and staff_id = '" + cbxPosition.Text + "'";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    numberToStatus();
                    evening.Enqueue("Order id: " + sqlRd.GetString(0) + " \nDate: " + sqlRd.GetDateTime(2).ToString("yyyy-MM-dd"));
                    //        evening.Enqueue("Order id: " + sqlRd.GetString(0) + "\nDelivery Status: " + status);
                }
                sqlConn.Close();

                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select delivery_order_id , delivery_status , delivery_date " +
                "from delivery_order " +
                "where " +
                "delivery_time >= 900 and delivery_time <= 1200 " +
                "and delivery_status in (1, 2) " +
                "and staff_id = '" + cbxPosition.Text + "'";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    numberToStatus();
                    morning.Enqueue("Order id: " + sqlRd.GetString(0) + " \nDate: " + sqlRd.GetDateTime(2).ToString("yyyy-MM-dd"));
                    //   morning.Enqueue("Order id: " + sqlRd.GetString(0) + "\nDelivery Status: " + status);
                }
                sqlConn.Close();
                lblDate.Text = "";
                morningPage = Convert.ToInt32(Math.Ceiling((double)morning.Count / 5));
                aftermoonPage = Convert.ToInt32(Math.Ceiling((double)aftermoon.Count / 5));
                eveningPage = Convert.ToInt32(Math.Ceiling((double)evening.Count / 5));
                maxPage = Math.Max(Math.Max(morningPage, aftermoonPage), eveningPage);
                writeMorningPage(current);
                writeAftermoonPage(current);
                writeEveningPage(current);
            }
            else
            {
                MessageBox.Show("Please select a staff");
            }
        }
    }
}
