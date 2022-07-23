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
    public partial class DeliveryDetail : Form
    {
        public static DeliveryDetail instance;
        public String action;
        private String order;
        public String invoice;
        public String session;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

  /*      String orderId;
        String date;
        String name;
        String phone;
        String status;
        String Remarks;
        String invoice; */



        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;
        private string JSONlist;

        public DeliveryDetail()
        {
            InitializeComponent();
            instance = this;
        }

        private void DeliveryDetail_Load(object sender, EventArgs e)
        {
            setPermission(Main.instance.dept, Main.instance.position);
            changeLanguage();
            if (action == "record")
            {
                btnEdit.Visible = false;
            }
            txtTime.Text = "e.g 0840, 0940, 2200";
           // dtpDate.MinDate = DateTime.Today;
            dtpDate.CustomFormat = "yyyy-MM-dd";
            dtpDate.Format = DateTimePickerFormat.Custom;
            Console.WriteLine(order);
            update();
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
                    case 34:
                        btnEdit.Visible = true;
                        break;
                    case 32:
                        btnDeliveryNote.Visible = true;
                        break;
                    case 33:
                        btnSaleDetail.Visible = true;
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
                label1.Text = "送货单号 :";
                label2.Text = "送货日期 :";
                label3.Text = "客户姓名 :";
                label4.Text = "客户电话 :";
                label6.Text = "状态 :";
                label5.Text = "备注 :";
                btnEditBack.Text = "返回";
                btnback.Text = "返回";
                btnEdit.Text = "编辑";
                label7.Text = "发票号 :";
                label8.Text = "时间段 :";
                label10.Text = "送货时间 :";
                label9.Text = "送货地址 :";
                btnSaleDetail.Text = "销售详情";
                btnSubmit.Text = "提交";
                btnDeliveryNote.Text = "打印送货单";
            }
            else
            {
                label1.Text = "Delivery Order Id :";
                label2.Text = "Delivery Date :";
                label3.Text = "Customer Name :";
                label4.Text = "Customer Phone No. :";
                label6.Text = "Status :";
                label5.Text = "Remarks :";
                btnEditBack.Text = "BACK";
                btnback.Text = "BACK";
                btnEdit.Text = "Edit Order";
                label7.Text = "Sales Invoice Id :";
                label8.Text = "Delivery Session :";
                label10.Text = "Delivery Time :";
                label9.Text = "Delivery Address :";
                btnSaleDetail.Text = "Sale Order Detail";
                btnSubmit.Text = "Submit";
                btnDeliveryNote.Text = "Print Delivery Note";
            }
        }

        public void update()
        {
            try
            {
                ConnectString conn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = conn.getString();
                sqlConn.Open();
                Console.WriteLine(order);
                sqlQuery = "select delivery_order_id, customer_name, customer_phone, delivery_address, invoice_id, delivery_date, delivery_time, delivery_remarks, delivery_status from delivery_order " +
                    "where delivery_order_id = '" + order + "'";
                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    lblOrder.Text = sqlRd.GetString(0);
                    lblDate.Text = sqlRd.GetDateTime(5).ToString("yyyy-MM-dd");
                    lblName.Text = sqlRd.GetString(1);
                    lblPhone.Text = sqlRd.GetString(2);
                    switch (sqlRd.GetInt32(8))
                    {
                        case 1:
                            if (language == "Chinese")
                                lblStatus.Text = "请求已送到";
                            else
                                lblStatus.Text = "Request Delivered";
                            break;
                        case 2:
                            if (language == "Chinese")
                                lblStatus.Text = "请求已确认";
                            else
                                lblStatus.Text = "Request Confirmed";
                            break;
                        case 3:
                            if (language == "Chinese")
                                lblStatus.Text = "请求被拒绝";
                            else
                                lblStatus.Text = "Request Rejected";
                            break;
                        case 4:
                            if (language == "Chinese")
                                lblStatus.Text = "请求已取消";
                            else
                                lblStatus.Text = "Request Cancelled";
                            break;
                    }
                    lblRemark.Text = sqlRd.GetString(7);
                    lblInvoice.Text = sqlRd.GetString(4);
                    if (sqlRd.GetInt32(6) <= 2200 && sqlRd.GetInt32(6) >= 1800)
                    {
                        if(language == "Chinese")
                            session = "晚上";
                        else
                            session = "Evening";
                        lblSession.Text = session;
                    }
                    else if (sqlRd.GetInt32(6) <= 1700 && sqlRd.GetInt32(6) >= 1300)
                    {
                        if (language == "Chinese")
                            session = "中午";
                        else
                            session = "Aftermoon";
                        lblSession.Text = session;
                    }
                    else if (sqlRd.GetInt32(6) <= 1200 && sqlRd.GetInt32(6) >= 900)
                    {
                        if (language == "Chinese")
                            session = "早上";
                        else
                            session = "Morning";
                        lblSession.Text = session;
                    }
                    lblTime.Text = sqlRd.GetString(6);
                    lblAddress.Text = sqlRd.GetString(3);
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

        public void SetOrder(String order)
        {
            this.order = order;
        }

        public void SetInvoice(String invoice)
        {
            this.invoice = invoice;
        }

        public void setAction(String action)
        {
            this.action = action;
        }

        private void btnback_Click(object sender, EventArgs e)
        {
            if (action == "table")
            {
                Main.instance.DeliveryTable();
            }
            else if(action == "record")
            {
                Main.instance.DeliveryRecord();
            }
        }

        private void btnDeliveryNote_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int totalItem = 1;
            SolidBrush solidBrush = new SolidBrush(
            Color.DarkGray);
            Pen blackPen = new Pen(Color.Black, 1);

            e.Graphics.DrawRectangle(blackPen, 30, 320, 400, 100);

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            StringFormat sf1 = new StringFormat();
            sf1.LineAlignment = StringAlignment.Center;
            // int stringlength;
            //  stringlength = (int) Convert.ToDouble(e.Graphics.MeasureString("KOWLOON BAY STORE", new Font("Mircosoft Sans Serif", 20, FontStyle.Bold)).Width.ToString());
            e.Graphics.DrawString("Better Limited", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 70, 40);
            e.Graphics.DrawString("Unit 1301,kowloon Bay pazzy 022", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 70);
            e.Graphics.DrawString("address address address address", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 90);
            e.Graphics.DrawString("Tel: +852 1234 5678    Fax: +852 1234 5678", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 110);
            e.Graphics.DrawString("Delivery Note No.: "+ lblOrder.Text, new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Right - 200, 150);
            e.Graphics.DrawString("___________________________________________________", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 180);
            e.Graphics.DrawString("_____", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Right, 180);
            e.Graphics.DrawString("Delivery Note", new Font("Mircosoft Sans Serif", 20, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 200, 180);
            e.Graphics.DrawString("Delivery To", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 295);
            e.Graphics.DrawString("Mr. "+ lblName.Text +"            +852 "+ lblPhone.Text, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 330);
            e.Graphics.DrawString(lblAddress.Text, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 370);
        //    e.Graphics.DrawString("address address address address", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 390);
            e.Graphics.DrawRectangle(blackPen, 30, 320, 400, 100);

            e.Graphics.DrawRectangle(blackPen, 30, 450, e.MarginBounds.Right + 30, 102);
            e.Graphics.FillRectangle(solidBrush, 30, 450, 140, 102);
            e.Graphics.DrawRectangle(blackPen, 30, 450, 140, 34);
            e.Graphics.DrawRectangle(blackPen, 30, 450, 140, 68);
            e.Graphics.DrawRectangle(blackPen, 30, 450, 140, 102);

            e.Graphics.DrawRectangle(blackPen, 170, 450, 180, 34);
            e.Graphics.FillRectangle(solidBrush, 350, 450, 140, 34);
            e.Graphics.DrawRectangle(blackPen, 350, 450, 140, 34);
            e.Graphics.DrawRectangle(blackPen, 170, 450, 617, 68);
            e.Graphics.DrawRectangle(blackPen, 490, 450, 297, 34);

            e.Graphics.DrawString(lblInvoice.Text, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 178, 455);
            e.Graphics.DrawString("Remarks", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 525);
            e.Graphics.DrawString("Invoice No.", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 455);
            e.Graphics.DrawString("Delivery Time", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 490);
            e.Graphics.DrawString("Remarks", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 525);
            e.Graphics.DrawString("Delivery Date", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 354, 455);
            e.Graphics.DrawString(lblDate.Text, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 498, 455);
            e.Graphics.DrawString(lblTime.Text, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 178, 490);
            e.Graphics.DrawString(lblRemark.Text, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 178, 525);
            try
            {
                ConnectString conn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = conn.getString();
                sqlConn.Open();
                sqlQuery = "SELECT * FROM sdp.store_stock;";
                sqlQuery =
                    "select distinct inventory.item_id, inventory.item_name, order_item.order_item_qty " +
                    "from delivery_order " +
                    "inner join order_item " +
                    "on order_item.order_id = delivery_order.order_id " +
                    "inner join inventory " +
                    "on inventory.item_id = order_item.item_id " +
                  "where delivery_order_id = '" + lblOrder.Text + "'";
                // sqlQuery = "SELECT * FROM sdp.store_stock;";
                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    e.Graphics.DrawString(sqlRd.GetString(0), new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 585 + totalItem * 35);
                    e.Graphics.DrawString(sqlRd.GetString(1), new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 212, 585 + totalItem * 35);
                    //  e.Graphics.DrawString("PCS", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 592, 585 + currentItem * 35);
                    e.Graphics.DrawString(sqlRd.GetString(2), new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 692, 585 + totalItem * 35);
                    totalItem++;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConn.Close();
            }

            e.Graphics.FillRectangle(solidBrush, 30, 580, e.MarginBounds.Right + 30, 34);
            e.Graphics.DrawRectangle(blackPen, 30, 580, e.MarginBounds.Right + 30, 34);
            e.Graphics.DrawRectangle(blackPen, 30, 580, 180, 34);
            //e.Graphics.DrawRectangle(blackPen, 210, 580, 380, 34);
            //e.Graphics.DrawRectangle(blackPen, 590, 580, 100, 34);
            e.Graphics.DrawRectangle(blackPen, 30, 580, 180, 34 * totalItem);
            //e.Graphics.DrawRectangle(blackPen, 210, 580, 380, 34 * totalItem);
            //e.Graphics.DrawRectangle(blackPen, 590, 580, 100, 34 * totalItem);
            e.Graphics.DrawRectangle(blackPen, 690, 580, 97, 34 * totalItem);
            e.Graphics.DrawRectangle(blackPen, 30, 580, 757, 34 * totalItem);

            e.Graphics.DrawString("Item Id", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 585);
            e.Graphics.DrawString("Description", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 212, 585);
            //e.Graphics.DrawString("Unit", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 592, 585);
            e.Graphics.DrawString("Quantity", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 692, 585);



            e.Graphics.DrawString("Consignee's Signature", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 1050);
            e.Graphics.DrawString("___________________________________", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 1120);

        }

        private void btnSaleDetail_Click(object sender, EventArgs e)
        {
            invoice = lblInvoice.Text;
            if (action == "record")
            {
                Main.instance.SaleRecordDetail("deliveryRecord", "");

            }else if (action == "table")
            {
                Main.instance.SaleRecordDetail("table", "");
            }
        }

 /*       private void btnEdit_Click(object sender, EventArgs e)
        {
            txtAddress.Visible = true;
            txtName.Visible = true;
            txtPhone.Visible = true;
            txtTime.Visible = true;
            txtRemark.Visible = true;
            dtpDate.Visible = true;
            lblAddress.Visible = false;
            lblName.Visible = false;
            lblPhone.Visible = false;
            lblTime.Visible = false;
            lblDate.Visible = false;
            lblRemark.Visible = false;
        }*/

        private void btnEdit_Click_1(object sender, EventArgs e)
        {
            dtpDate.Text = lblDate.Text;
            txtAddress.Text = lblAddress.Text;
            txtName.Text = lblName.Text;
            txtPhone.Text = lblPhone.Text;
            txtRemark.Text = lblRemark.Text;
            txtTime.Text = lblTime.Text;
            txtAddress.Visible = true;
            txtName.Visible = true;
            txtPhone.Visible = true;
            txtTime.Visible = true;
            txtRemark.Visible = true;
            dtpDate.Visible = true;
            lblAddress.Visible = false;
            lblName.Visible = false;
            lblPhone.Visible = false;
            lblTime.Visible = false;
            lblDate.Visible = false;
            lblRemark.Visible = false;
            btnEdit.Visible = false;
            btnDeliveryNote.Visible = false;
            btnSaleDetail.Visible = false;
            btnEditBack.Visible = true;
            btnback.Visible = false;
            btnSubmit.Visible = true;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkTime())
                {
                    if (txtName.Text != "")
                    {
                        if (txtAddress.Text != "")
                        {
                            if (txtPhone.Text.Length == 8)
                            {
                                if (checkphone())
                                {
                                    ConnectString rootconn = new ConnectString("root", "123456");
                                    sqlConn.ConnectionString = rootconn.getString();

                                    sqlConn.Open();
                                    sqlCmd.Connection = sqlConn;
                                    sqlCmd.CommandText = "update delivery_order set delivery_date = '" + dtpDate.Text + "', customer_name = '" + txtName.Text +
                                        "', customer_phone = '" + txtPhone.Text + "', " + "delivery_time = '" + txtTime.Text + "', " +
                                        "delivery_address = '" + txtAddress.Text + "', delivery_remarks ='" + txtRemark.Text + "' " +
                                        "where delivery_order_id = '" + lblOrder.Text + "'";
                                    sqlCmd.ExecuteNonQuery();
                                    sqlConn.Close();

                                    MessageBox.Show("Successful to update the delivery order");
                                    update();
                                }
                            }
                            else
                            {
                                MessageBox.Show("The phone number must be 8 digital number");
                            }
                        }
                        else
                        {
                            MessageBox.Show("The address item is not allow null value");
                        }
                    }
                    else
                    {
                        MessageBox.Show("The name item is not allow null value");
                    }
                }
                else
                {
                    MessageBox.Show("Please imput correct time value");
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("The phone number is incorrect format");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEditBack_Click(object sender, EventArgs e)
        {
            txtAddress.Visible = false;
            txtName.Visible = false;
            txtPhone.Visible = false;
            txtTime.Visible = false;
            txtRemark.Visible = false;
            dtpDate.Visible = false;
            lblAddress.Visible = true;
            lblName.Visible = true;
            lblPhone.Visible = true;
            lblTime.Visible = true;
            lblDate.Visible = true;
            lblRemark.Visible = true;
            lblSession.Text = session;
            setPermission(Main.instance.dept, Main.instance.position);
            btnEditBack.Visible = false;
            btnback.Visible = true;
            btnSubmit.Visible = false;
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

        private void txtTime_MouseLeave(object sender, EventArgs e)
        {
            if (txtTime.Text == "")
            {
                txtTime.Text = "e.g 0840, 0940, 2200";
                txtTime.ForeColor = Color.Gray;
                txtTime.ReadOnly = true;
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
                                lblSession.Text = "Aftermoon";
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

        private void txtTime_TextChanged(object sender, EventArgs e)
        {
            checkTime();
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
                MessageBox.Show("Please input correct format on phone item");
                return false;
            }
        }
    }
}
