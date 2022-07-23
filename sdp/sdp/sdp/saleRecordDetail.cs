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
    public partial class saleRecordDetail : Form
    {
        private String storeId;
        public int pageHeight;
        private String address;
        private String shop;
        private String staffId;
        private String receiptType;
        private double originalTotal;
        public String item;
        private double cashTendered;
        public double subtotal;
        public String staff;
        public String action;
        public static Shop1 instance;
        public String staffName;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        public String invoice;
        private String orderid;
        private String storeid;
        private String date;
        private String preorder;
        double total;
        private Double paidDeposit;
        private string language;

        public saleRecordDetail()
        {
            InitializeComponent();
        }

        private void saleRecordDetail_Load(object sender, EventArgs e)
        {
            changeLanguage();
            DataGridViewTextBoxColumn Col1 = new DataGridViewTextBoxColumn();
            Col1.ReadOnly = true;
            DataGridViewTextBoxColumn Col2 = new DataGridViewTextBoxColumn();
            Col2.Width = 150;
            Col2.ReadOnly = true;
            DataGridViewTextBoxColumn Col3 = new DataGridViewTextBoxColumn();
            Col3.Width = 10;
            Col3.ValueType = typeof(int);
            DataGridViewTextBoxColumn Col4 = new DataGridViewTextBoxColumn();
            Col4.ValueType = typeof(double);
            DataGridViewTextBoxColumn Col5 = new DataGridViewTextBoxColumn();
            Col5.Width = 100;
            Col5.ValueType = typeof(double);
            DataGridViewTextBoxColumn Col6 = new DataGridViewTextBoxColumn();
            Col6.ReadOnly = true;
            Col6.ValueType = typeof(String);
            if (language == "Chinese")
            {
                Col1.HeaderText = "货品号";
                Col2.HeaderText = "货品名称";
                Col3.HeaderText = "数量";
                Col4.HeaderText = "金额 ";
                Col5.HeaderText = "折扣(%)";
                Col6.HeaderText = "总共";
            }
            else
            {
                Col1.HeaderText = "Item Id";
                Col2.HeaderText = "Item Name";
                Col3.HeaderText = "Quantity";
                Col4.HeaderText = "Price";
                Col5.HeaderText = "Discount(%)";
                Col6.HeaderText = "Total";
            }
            if (action == "Sale")
            {
                btnReject.Visible = true;
            }
            dataGridView1.Columns.AddRange(new[] { Col1, Col2, Col3, Col4, Col5, Col6 });
            updateLable();
            switch (receiptType)
            {
                case "1":
                    handleType1();
                    break;
                case "2":
                    handleType2();
                    break;
                case "3":
                    handleType3();
                    break;
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
                label1.Text = "销售发票号:";
                btnback.Text = "返回";
                label3.Text = "店铺号:";
                label4.Text = "日期:";
                label5.Text = "预订:";
                label6.Text = "订金:";
                label7.Text = "付款方式:";
                label8.Text = "总共(HKD) :";
                label2.Text = "备注:";
                btnPrint.Text = "打印发票";
                btnReject.Text = "取消订单";
            }
            else
            {
                label1.Text = "Sales Invoice Id:";
                btnback.Text = "BACK";
                label3.Text = "Store Id:";
                label4.Text = "Date:";
                label5.Text = "Pre-order:";
                label6.Text = "Deposit:";
                label7.Text = "Payment Method:";
                label8.Text = "TOTAL (HKD) :";
                label2.Text = "Remark:";
                btnPrint.Text = "Print Receipt";
                btnReject.Text = "Cancel The Order";
            }
        }

        public void updateLable()
        {
            sqlConn.ConnectionString = ConnectString.ConnectionString;

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select invoice_id, store_id, Sales_datetime, IF(sales_order.order_is_preorder = 'n', 'No', 'Yes') , sales_order.order_remarks, " +
                "sales_order.order_deposit , payment_method, receipt_type, cash_tender, sales_receipt.staff_id " +
            "from sales_receipt " +
            "inner join sales_order " +
            "on sales_order.order_id = sales_receipt.order_id " +
            "where invoice_id = '" + invoice + "'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                orderid = sqlRd.GetString(0);
                storeid = sqlRd.GetString(1);
                date = sqlRd.GetString(2);
                preorder = sqlRd.GetString(3);
                lblRemarks.Text = sqlRd.GetString(4);
                lblDeposit.Text = sqlRd.GetString(5);
                lblMethod.Text = sqlRd.GetString(6).ToUpper();
                receiptType = sqlRd.GetString(7);
                cashTendered = sqlRd.GetDouble(8);
                staffId = sqlRd.GetString(9);
            }
            if (language == "Chinese")
            {
                if (preorder == "No")
                    preorder = "否";
                else
                    preorder = "是";
            }
            sqlRd.Close();
            sqlConn.Close();
        }

        public void handleType1()
        {
            sqlConn.ConnectionString = ConnectString.ConnectionString;

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select order_item.item_id, item_name, order_item_qty, sale_price, " +
                "cast(((order_item_final_amount - sale_price * order_item_qty) * -100 / sale_price / order_item_qty) as DECIMAL) as 'Discount'," +
                "order_item_final_amount " +
                "from order_item " +
                "inner join inventory " +
                "on inventory.item_id = order_item.item_id " +
                "inner join sales_receipt " +
                "on sales_receipt.order_id = order_item.order_id " +
                "where sales_receipt.invoice_id = '" + invoice + "'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                dataGridView1.Rows.Add(sqlRd.GetString(0), sqlRd.GetString(1),
                    sqlRd.GetString(2), sqlRd.GetString(3), sqlRd.GetString(4), sqlRd.GetString(5));
            }
            sqlRd.Close();
            sqlConn.Close();
            lblInvoice.Text = orderid;
            lblDate.Text = date;
            lblStore.Text = storeid;
            lblPre.Text = preorder;
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                originalTotal += (Convert.ToDouble(item.Cells[2].Value.ToString()) * Convert.ToDouble(item.Cells[3].Value.ToString()));
                total += Convert.ToDouble(item.Cells[5].Value.ToString());
            }
            lblTotal.Text = total.ToString();
        }

        public void handleType2()
        {
            sqlConn.ConnectionString = ConnectString.ConnectionString;

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select order_item.item_id, item_name, order_item_qty, sale_price, " +
                "cast(((order_item_final_amount - sale_price * order_item_qty) * -100 / sale_price / order_item_qty) as DECIMAL) as 'Discount'," +
                "order_item_final_amount, " +
                "is_deposit " +
                "from order_item " +
                "inner join inventory " +
                "on inventory.item_id = order_item.item_id " +
                "inner join sales_receipt " +
                "on sales_receipt.order_id = order_item.order_id " +
                "where sales_receipt.invoice_id = '" + invoice + "'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                if (sqlRd.GetString(6) == "y")
                {
                    dataGridView1.Rows.Add(sqlRd.GetString(0), sqlRd.GetString(1),
                        sqlRd.GetString(2), sqlRd.GetString(3), sqlRd.GetString(4), "Deposit");
                }
                else
                {
                    dataGridView1.Rows.Add(sqlRd.GetString(0), sqlRd.GetString(1),
    sqlRd.GetString(2), sqlRd.GetString(3), sqlRd.GetString(4), sqlRd.GetString(5));
                }
            }
            sqlRd.Close();
            sqlConn.Close();
            lblInvoice.Text = orderid;
            lblDate.Text = date;
            lblStore.Text = storeid;
            lblPre.Text = preorder;
            lblTotal.Text = total.ToString();
            double paidDepositTotal = 0;
            paidDeposit = 0;
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                if (item.Cells[5].Value.ToString() != "Deposit")
                {
                    originalTotal += (Convert.ToDouble(item.Cells[2].Value.ToString()) * Convert.ToDouble(item.Cells[3].Value.ToString()));
                    total += Convert.ToDouble(item.Cells[5].Value.ToString());
                }
                else
                {
                    paidDepositTotal +=
                        (Convert.ToInt32(item.Cells[2].Value.ToString()) *
                        Convert.ToDouble(item.Cells[3].Value.ToString()) *
                        (1 - Convert.ToDouble(item.Cells[4].Value.ToString()) / 100));
                }
            }
            paidDeposit = paidDepositTotal - Convert.ToDouble(lblDeposit.Text);
            lblTotal.Text = (total + paidDeposit).ToString();
        }

        public void handleType3()
        {
            sqlConn.ConnectionString = ConnectString.ConnectionString;

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select order_item.item_id, item_name, order_item_qty, sale_price, " +
                "cast(((order_item_final_amount - sale_price * order_item_qty) * -100 / sale_price / order_item_qty) as DECIMAL) as 'Discount'," +
                "order_item_final_amount, " +
                "is_deposit " +
                "from order_item " +
                "inner join inventory " +
                "on inventory.item_id = order_item.item_id " +
                "inner join sales_receipt " +
                "on sales_receipt.order_id = order_item.order_id " +
                "where sales_receipt.invoice_id = '" + invoice + "' and " +
                "is_deposit = 'y' and " +
                "is_sale = 'y'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                dataGridView1.Rows.Add(sqlRd.GetString(0), sqlRd.GetString(1),
sqlRd.GetString(2), sqlRd.GetString(3), sqlRd.GetString(4), sqlRd.GetString(5));
            }
            sqlRd.Close();
            sqlConn.Close();
            lblInvoice.Text = orderid;
            lblDate.Text = date;
            lblStore.Text = storeid;
            lblPre.Text = preorder;
            lblTotal.Text = lblDeposit.Text;

            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                if (item.Cells[5].Value.ToString() != "Deposit")
                {
                    originalTotal += (Convert.ToDouble(item.Cells[2].Value.ToString()) * Convert.ToDouble(item.Cells[3].Value.ToString()));
                    total += Convert.ToDouble(item.Cells[5].Value.ToString());
                }
            }
            lblTotal.Text = total.ToString();
        }

        public void setAction(String action)
        {
            this.action = action;
        }

        public void setShop(String storeId)
        {
            this.storeId = storeId;
        }

        public void setInvoice(String invoice)
        {
            this.invoice = invoice;
        }

        private void btnback_Click(object sender, EventArgs e)
        {
            if (action == "confirmTechnical")
            {
                Main.instance.ConfirmInstallationDetail();
            }
            if (action == "confirmDelivery")
            {
                Main.instance.ConfirmDeliveryDetail();
            }
            if (action == "Sale")
            {
                Main.instance.SaleRecord(storeId);
            }
            if (action == "deliveryRecord")
            {
                Main.instance.DeliveryDetail("record");
            }
            if (action == "table")
            {
                Main.instance.DeliveryDetail("table");
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            pageHeight = 560 + 60 * dataGridView1.Rows.Count;

            switch (storeid)
            {
                case "HK01":
                    shop = "KOWLOON BAY STORE";
                    address = "B102, LAM CHAK STREET, KOWLOON, H.K";
                    break;
                case "HK02":
                    shop = "TSUEN WAN";
                    address = "B102,TAI HO ROAD, TSUEN WAN H.K";
                    break;
            }

            sqlConn.Open();  //get staff full name
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "SELECT first_name, last_name " +
                "FROM staff " +
                "inner join user " +
                "on user.user_id = staff.user_id " +
                "where staff_id = '" + staffId + "'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                staffName = sqlRd.GetString(0) + " " + sqlRd.GetString(1);
            }
            sqlRd.Close();
            sqlConn.Close();

            printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("custom", 427, pageHeight);
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }


        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (receiptType == "1")
            {
                int height = 220;
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                StringFormat sf1 = new StringFormat();
                sf1.LineAlignment = StringAlignment.Center;
                // int stringlength;
                //  stringlength = (int) Convert.ToDouble(e.Graphics.MeasureString("KOWLOON BAY STORE", new Font("Mircosoft Sans Serif", 20, FontStyle.Bold)).Width.ToString());
                e.Graphics.DrawString(shop, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 40, sf);
                e.Graphics.DrawString(address, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 100, sf);
                e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                    e.MarginBounds.Left + (e.MarginBounds.Width / 2), 140, sf);
                e.Graphics.DrawString("STORE: " + storeId, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 170, sf1);
                e.Graphics.DrawString("STAFF: " + staffId, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, 170, sf);
                e.Graphics.DrawString("INVOICE NO.: " + invoice, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 200, sf1);
                e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                    e.MarginBounds.Left + (e.MarginBounds.Width / 2), 220, sf);
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    height += 40;
                    e.Graphics.DrawString(row.Cells[1].Value.ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                    e.Graphics.DrawString(String.Format("{0:0.0}", row.Cells[5].Value.ToString()), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                    height += 20;
                    e.Graphics.DrawString(row.Cells[0].Value.ToString() + "   " +
                        row.Cells[2].Value.ToString() + "@ " +
                        String.Format("{0:0.0}", row.Cells[3].Value.ToString()), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                }

                height += 30;
                e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 20;
                e.Graphics.DrawString("PRE-ORDER", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString("NO", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("PAID DEPOSIT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString("0.0", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("ORIGINAL TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", originalTotal).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("UNPAID", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString("0.0", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("DISCOUNT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", (originalTotal - total)), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", (total)), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 30;
                e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 20;
                e.Graphics.DrawString("CASH TENDERED", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", cashTendered), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("CHANGE", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", cashTendered - total), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 30;
                e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 20;
                // e.Graphics.DrawString("Tran:155100", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(date, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 100, height);
                height += 20;
                e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 30;
                e.Graphics.DrawString("Thank For Shopping", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
            }
            else if (receiptType == "2")
            {
                double discount = 0;
                int height = 220;
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                StringFormat sf1 = new StringFormat();
                sf1.LineAlignment = StringAlignment.Center;
                // int stringlength;
                //  stringlength = (int) Convert.ToDouble(e.Graphics.MeasureString("KOWLOON BAY STORE", new Font("Mircosoft Sans Serif", 20, FontStyle.Bold)).Width.ToString());
                e.Graphics.DrawString(shop, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 40, sf);
                e.Graphics.DrawString(address, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 100, sf);
                e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                    e.MarginBounds.Left + (e.MarginBounds.Width / 2), 140, sf);
                e.Graphics.DrawString("STORE: " + storeId, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 170, sf1);
                e.Graphics.DrawString("STAFF: " + staffId, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, 170, sf);
                e.Graphics.DrawString("INVOICE NO.: " + invoice, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 200, sf1);
                e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                    e.MarginBounds.Left + (e.MarginBounds.Width / 2), 220, sf);
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[5].Value.ToString() != "Deposit")
                    {
                        height += 40;
                        e.Graphics.DrawString(row.Cells[1].Value.ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                        e.Graphics.DrawString(String.Format("{0:0.0}", row.Cells[5].Value.ToString().ToString()), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString(row.Cells[0].Value.ToString() + "   " +
                            row.Cells[2].Value.ToString() + "@ " +
                            String.Format("{0:0.0}", row.Cells[3].Value.ToString()), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                        discount += (Convert.ToInt32(row.Cells[2].Value.ToString()) *
                        Convert.ToDouble(row.Cells[3].Value.ToString()) -
                            (Convert.ToInt32(row.Cells[2].Value.ToString()) *
                        Convert.ToDouble(row.Cells[3].Value.ToString()) *
                        (1 - Convert.ToDouble(row.Cells[4].Value.ToString()) / 100)));
                    }
                    else
                    {
                        height += 40;
                        e.Graphics.DrawString(row.Cells[1].Value.ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                        e.Graphics.DrawString("(UNPAID)", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 75, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", (Convert.ToInt32(row.Cells[2].Value.ToString()) *
                            Convert.ToDouble(row.Cells[3].Value.ToString()) *
                            (1 - Convert.ToDouble(row.Cells[4].Value.ToString()) / 100))).ToString(),
                            new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString(row.Cells[0].Value.ToString() + "   " +
                            row.Cells[2].Value.ToString() + "@ " +
                            String.Format("{0:0.0}", row.Cells[3].Value.ToString()), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                        discount += (Convert.ToInt32(row.Cells[2].Value.ToString()) *
                        Convert.ToDouble(row.Cells[3].Value.ToString()) -
                            (Convert.ToInt32(row.Cells[2].Value.ToString()) *
                        Convert.ToDouble(row.Cells[3].Value.ToString()) *
                        (1 - Convert.ToDouble(row.Cells[4].Value.ToString()) / 100)));
                    }
                }

                height += 30;
                e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 20;
                e.Graphics.DrawString("PRE-ORDER", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString("Yes", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("PAID DEPOSIT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", paidDeposit).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("ORIGINAL TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", originalTotal).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("UNPAID", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", lblDeposit.Text), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("DISCOUNT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", discount), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", (total + paidDeposit)), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 30;
                e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 20;
                e.Graphics.DrawString("CASH TENDERED", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", cashTendered), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("CHANGE", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", cashTendered - total - paidDeposit), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 30;
                e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 20;
                // e.Graphics.DrawString("Tran:155100", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(date, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 100, height);
                height += 20;
                e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 30;
                e.Graphics.DrawString("Thank For Shopping", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
            }
            else if (receiptType == "3")
            {
                int height = 220;
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                StringFormat sf1 = new StringFormat();
                sf1.LineAlignment = StringAlignment.Center;
                // int stringlength;
                //  stringlength = (int) Convert.ToDouble(e.Graphics.MeasureString("KOWLOON BAY STORE", new Font("Mircosoft Sans Serif", 20, FontStyle.Bold)).Width.ToString());
                e.Graphics.DrawString(shop, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 40, sf);
                e.Graphics.DrawString(address, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 100, sf);
                e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                    e.MarginBounds.Left + (e.MarginBounds.Width / 2), 140, sf);
                e.Graphics.DrawString("STORE: " + storeId, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 170, sf1);
                e.Graphics.DrawString("STAFF: " + staffId, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, 170, sf);
                e.Graphics.DrawString("INVOICE NO.: " + invoice, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 200, sf1);
                e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                    e.MarginBounds.Left + (e.MarginBounds.Width / 2), 220, sf);
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    height += 40;
                    e.Graphics.DrawString(row.Cells[1].Value.ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                    // e.Graphics.DrawString((Convert.ToDouble(order[i,2]) * Convert.ToDouble(order[i, 3])).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                    e.Graphics.DrawString(String.Format("{0:0.0}", Convert.ToDouble(row.Cells[5].Value.ToString())), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                    height += 20;
                    e.Graphics.DrawString(row.Cells[0].Value.ToString() + "   " +
                        row.Cells[2].Value.ToString() + "@ " +
                        String.Format("{0:0.0}", Convert.ToDouble(row.Cells[3].Value.ToString())), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                }

                height += 30;
                e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 20;
                e.Graphics.DrawString("PRE-ORDER", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString("NO", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("PAID DEPOSIT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", total - Convert.ToDouble(lblDeposit.Text)).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("ORIGINAL TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", originalTotal).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("UNPAID", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", 0), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("DISCOUNT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", (originalTotal - total)), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", lblDeposit.Text), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 30;
                e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 20;
                e.Graphics.DrawString("CASH TENDERED", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", cashTendered), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("CHANGE", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.0}", cashTendered - Convert.ToDouble(lblDeposit.Text)), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 30;
                e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 20;
                // e.Graphics.DrawString("Tran:155100", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(date, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 100, height);
                height += 20;
                e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 30;
                e.Graphics.DrawString("Thank For Shopping", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
            }
        }

        private void lblInvoice_Click(object sender, EventArgs e)
        {

        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            string orderId = "";
            string message = "Are you want to cancel the sale order?";
            string title = "cancel order";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.Yes)
            {
                try
                {
                    sqlConn.Open();
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandText = "select order_id from sales_receipt where invoice_id = '" + invoice + "'";
                    sqlRd = sqlCmd.ExecuteReader();
                    while (sqlRd.Read())
                    {
                        orderId = sqlRd.GetString(0);
                    }
                    sqlRd.Close();
                    sqlConn.Close();
                    sqlConn.Open();
                    sqlQuery = "update sales_order set order_is_cancelled = 'y' where order_id ='" + orderId + "'";
                    sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    MessageBox.Show("Successful to confirm cancel this sale order");
                    if (action == "confirmTechnical")
                    {
                        Main.instance.ConfirmInstallationDetail();
                    }
                    if (action == "confirmDelivery")
                    {
                        Main.instance.ConfirmDeliveryDetail();
                    }
                    if (action == "Sale")
                    {
                        Main.instance.SaleRecord(storeId);
                    }
                    if (action == "deliveryRecord")
                    {
                        Main.instance.DeliveryDetail("record");
                    }
                    if (action == "table")
                    {
                        Main.instance.DeliveryDetail("table");
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
    }
}
