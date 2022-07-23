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
    public partial class DefectItemDetails : Form
    {
        private String goodReturnItemId;
        private String itemId;
        private String itemName;
        private String saleInvoiceId;
        private String quantity;
        private String staffId;
        private String date;
        private String customerName;
        private String customerPhone;
        private String remarks;
        private int numbericstatus;
        private String status;
        private String selectStatus;
        private String itemPrice;

        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;
        private string JSONlist;

        public DefectItemDetails()
        {
            InitializeComponent();
        }

        private void DefectItemDetails_Load(object sender, EventArgs e)
        {
            setprintPermission(Main.instance.dept, Main.instance.position);
            changeLanguage();
            if (selectStatus == "Request Delivered" || selectStatus == "请求已送到")
            {
                setConfirmPermission(Main.instance.dept, Main.instance.position);
            }
            updateValue();
            updateLable();
        }

        public void setConfirmPermission(int dept, int position)
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
                    case 22:
                        btnConfirm.Visible = true;
                        btnReject.Visible = true;
                        break;
                }
            }
        }

        public void setprintPermission(int dept, int position)
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
                    case 21:
                        btnSubmit.Visible = true;
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
                label1.Text = "退货单号 :";
                label2.Text = "货品号 :";
                label3.Text = "货品名称 :";
                label4.Text = "发票号 :";
                label8.Text = "退回数量 :";
                label12.Text = "员工编号 :";
                label14.Text = "日期 :";
                btnBack.Text = "返回";
                label9.Text = "客户姓名 :";
                label10.Text = "客户电话 :";
                label7.Text = "备注 :";
                label5.Text = "状态 :";
                btnReject.Text = "拒绝";
                btnConfirm.Text = "确认";
                btnSubmit.Text = "打印";
            }
            else
            {
                label1.Text = "Good Return Note Id :";
                label2.Text = "Item Id :";
                label3.Text = "Item Name :";
                label4.Text = "Sales Invoice Id :";
                label8.Text = "Return Quantity :";
                label12.Text = "Staff Id :";
                label14.Text = "Date :";
                btnBack.Text = "back";
                label9.Text = "Customer Name :";
                label10.Text = "Customer Phone No. :";
                label7.Text = "Remarks :";
                label5.Text = "Status :";
                btnReject.Text = "Reject";
                btnConfirm.Text = "Confirm";
                btnSubmit.Text = "Print";
            }
        }

        public void setGoodReturnItemId(String goodReturnItemId)
        {
            this.goodReturnItemId = goodReturnItemId;
        }

        public void setSelectStatus(String selectStatus)
        {
            this.selectStatus = selectStatus;
        }

        public void updateValue()
        {
            try
            {
                ConnectString rootconn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = rootconn.getString();
                sqlConn.Open();
                sqlQuery = "select good_return_note.item_id, item_name, invoice_id,good_return_qty, " +
                    "staff_id,good_return_date,customer_name,customer_phone,good_return_remarks,good_return_status " +
                    "from good_return_note " +
                    "inner join inventory " +
                    "on inventory.item_id = good_return_note.item_id " +
                    "where good_return_id = '" + goodReturnItemId + "'";
                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    itemId = sqlRd.GetString(0);
                    itemName = sqlRd.GetString(1);
                    saleInvoiceId = sqlRd.GetString(2);
                    quantity = sqlRd.GetString(3);
                    staffId = sqlRd.GetString(4);
                    date = sqlRd.GetString(5);
                    customerName = sqlRd.GetString(6);
                    customerPhone = sqlRd.GetString(7);
                    remarks = sqlRd.GetString(8);
                    numbericstatus = sqlRd.GetInt32(9);
                    numbericToStatus(numbericstatus);
                }
                sqlRd.Close();
                sqlConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlRd.Close();
                sqlConn.Close();
            }
        }

        public void updateLable()
        {
            lblGoodReturnNoteId.Text = goodReturnItemId;
            lblItemId.Text = itemId;
            lblItemName.Text = itemName;
            lblInvoiceId.Text = saleInvoiceId;
            lblQuantity.Text = quantity;
            lblStaffId.Text = staffId;
            lblDate.Text = date;
            lblCustomerName.Text = customerName;
            lblCustomerPhone.Text = customerPhone;
            lblRemarks.Text = remarks;
            lblStatus.Text = status;
        }

        public void numbericToStatus(int numbericStatus)
        {
            switch (numbericStatus)
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
                default:
                    if (language == "Chinese")
                        status = "请求已送到";
                    else
                        status = "Request Delivered";
                    break;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        public void getItemDetail()
        {
            try
            {
                ConnectString rootconn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = rootconn.getString();
                sqlConn.Open();
                sqlQuery = "select item_sales_price " +
                    "from good_return_note " +
                    "inner join inventory " +
                    "on good_return_note.item_id = inventory.item_id " +
                    "where good_return_note.item_id = '" + itemId + "' and " +
                    "good_return_id = '" + goodReturnItemId + "'";
                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    itemPrice = sqlRd.GetString(0);
                }
                sqlRd.Close();
                sqlConn.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlRd.Close();
                sqlConn.Close();
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            getItemDetail();
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
            e.Graphics.DrawString("Good Return Note Id : "+ goodReturnItemId, new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Right - 280, 150);
            e.Graphics.DrawString("___________________________________________", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 180);
            e.Graphics.DrawString("__________", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Right - 30, 180);
            e.Graphics.DrawString("Good Return Note", new Font("Mircosoft Sans Serif", 20, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 280, 180);
            e.Graphics.DrawString("Responsible Staff Id : "+staffId, new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 250);
            e.Graphics.DrawString("Customer Information :", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 295);
            e.Graphics.DrawString("Customer Name :", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 340);
            e.Graphics.DrawString(customerName, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + 90, 340);
            e.Graphics.DrawString("Customer Phone Number :", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 390);
            e.Graphics.DrawString("+852 "+customerPhone.Insert(4," "), new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + 165, 390);
            e.Graphics.DrawRectangle(blackPen, 30, 320, 400, 100);

            e.Graphics.DrawRectangle(blackPen, 30, 450, e.MarginBounds.Right + 30, 102);
            e.Graphics.FillRectangle(solidBrush, 30, 450, 160, 102);
            e.Graphics.DrawRectangle(blackPen, 30, 450, 160, 34);
            e.Graphics.DrawRectangle(blackPen, 30, 450, 160, 68);
            e.Graphics.DrawRectangle(blackPen, 30, 450, 160, 102);

            e.Graphics.DrawRectangle(blackPen, 190, 450, 160, 34);
            e.Graphics.FillRectangle(solidBrush, 350, 450, 140, 34);
            e.Graphics.DrawRectangle(blackPen, 350, 450, 140, 34);
            e.Graphics.DrawRectangle(blackPen, 190, 450, 597, 68);
            e.Graphics.DrawRectangle(blackPen, 490, 450, 297, 34);

            e.Graphics.DrawString("Invoice No.", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 455);
            e.Graphics.DrawString(saleInvoiceId, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 198, 455);
            e.Graphics.DrawString("Remarks", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 525);
            e.Graphics.DrawString(remarks, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 198, 525);
            e.Graphics.DrawString("Return Quantity", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 490);
            e.Graphics.DrawString(quantity, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 198, 490);
            e.Graphics.DrawString("Date", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 354, 455);
            e.Graphics.DrawString(date, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 498, 455);

            e.Graphics.FillRectangle(solidBrush, 30, 580, e.MarginBounds.Right + 30, 34);
            e.Graphics.DrawRectangle(blackPen, 30, 580, e.MarginBounds.Right + 30, 34);
            e.Graphics.DrawRectangle(blackPen, 30, 580, 180, 34);
            e.Graphics.DrawRectangle(blackPen, 210, 580, 380, 34);
            e.Graphics.DrawRectangle(blackPen, 590, 580, 100, 34);
            e.Graphics.DrawRectangle(blackPen, 30, 580, 180, 34 + 34 * totalItem);
            e.Graphics.DrawRectangle(blackPen, 210, 580, 380, 34 + 34 * totalItem);
            e.Graphics.DrawRectangle(blackPen, 590, 580, 100, 34 + 34 * totalItem);
            e.Graphics.DrawRectangle(blackPen, 690, 580, 97, 34 + 34 * totalItem);
            e.Graphics.DrawRectangle(blackPen, 30, 580, 757, 34 + 34 * totalItem);

            e.Graphics.DrawString("Item Id", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 585);
            e.Graphics.DrawString("Description", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 212, 585);
            e.Graphics.DrawString("Quantity", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 592, 585);
            e.Graphics.DrawString("Price(HKD)", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 690, 585);
            for (int i = 1; i <= totalItem; i++)
            {
                e.Graphics.DrawString(itemId, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 585 + i * 35);
                e.Graphics.DrawString(itemName, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 212, 585 + i * 35);
                e.Graphics.DrawString(quantity, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 592, 585 + i * 35);
                e.Graphics.DrawString(itemPrice, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 692, 585 + i * 35);
            }
        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.DefectItem();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string message = "Are you want to confirm the defect item?";
            string title = "defect item";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.Yes)
            {
                try
                {
                    sqlConn.Open();
                    sqlQuery = "update good_return_note set good_return_status = 2 where good_return_id = '"+goodReturnItemId+"'";
                    sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    MessageBox.Show("Successful to confirm the defect item");
                    Main.instance.DefectItem();
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
            string message = "Are you want to reject the defect item?";
            string title = "reject item";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.Yes)
            {
                try
                {
                    sqlConn.Open();
                    sqlQuery = "update good_return_note set good_return_status = 3 where good_return_id = '" + goodReturnItemId + "'";
                    sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    MessageBox.Show("Successful to reject the defect item");
                    Main.instance.DefectItem();
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
