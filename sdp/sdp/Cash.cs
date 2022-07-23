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
    public partial class Cash : Form
    {
        public int invoice_idForPre = 400000001;
        public int pageHeight;
        private String action;
        double depositTotal;
        double originalTotal;
        double discount;
        double total;
        public Double paidDeposit;
        public Boolean deposit;
        public Double allDeposit;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        DataSet DS = new DataSet();
        Dictionary<string, string> dicStockID = new Dictionary<string, string>();
        Dictionary<string, string> dicDepositStockID = new Dictionary<string, string>();

        MySqlDataReader sqlRd;

        private String storeId;
        public String staffName;
        public String shop;
        public String address;
        public int invoice_id = 400000001;
        public int order_id = 200000001;
        public String[,] order;
        public static Cash instance;
        public Double price;
        Double cashTendered;
        Double change;
        private string language;

        public Cash()
        {
            InitializeComponent();
            instance = this;
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
                label2.Text = "现金";
                label3.Text = "找钱";
                label1.Text = "现金支付";
                btnCancel.Text = "取消";
                btnConfirm.Text = "确认";
                btnEnter.Text = "计算";
                this.Text = "现金支付";
            }
            else
            {
                label2.Text = "CASH TENDERED";
                label3.Text = "CHANGE";
                label1.Text = "CASH PAYMENT";
                btnCancel.Text = "Cancel";
                btnConfirm.Text = "Confirm";
                btnEnter.Text = "Calculate";
                this.Text = "Cash Payment";
            }
        }

        private void cash_Load(object sender, EventArgs e)
        {
            changeLanguage();
        }

        private void txtCash_TextChanged(object sender, EventArgs e)
        {

        }

        public void setPrice(Double price)
        {
            this.price = price;
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            try
            {
                cashTendered = Convert.ToDouble(String.Format("{0:0.#}", Convert.ToDouble(txtCash.Text)));
                txtCash.Text = String.Format("{0:0.#}", cashTendered);
                if (price <= cashTendered)
                {
                    change = Convert.ToDouble(String.Format("{0:0.#}", (cashTendered - price)));
                    txtChange.Text = change.ToString();
                }
                else
                {
                    MessageBox.Show("cash tendered must be larger than total amoung");
                }
            }
            catch
            {
                MessageBox.Show("Please input correct number");
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (action != "preorder")
            {
                handleCheckour();
                checkout.instance.setCashAction("confirm");
                this.Close();
            }
            else
            {
                handlePreOrderCheckour();
                PreOrderCheckout.instance.setCashAction("confirm");
                this.Close();
            }
        }

        public void handlePreOrderCheckour()
        {
            if (txtChange.Text != "")
            {
                switch (storeId)
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
                /*    Console.WriteLine(shop);
                    Console.WriteLine(address);
                    Console.WriteLine(staffName);
                    Console.WriteLine(paidDeposit);
                    Console.WriteLine(originalTotal);
                    Console.WriteLine(total);
                    Console.WriteLine(discount);
                    Console.WriteLine(total);*/


                ConnectString rootconn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = rootconn.getString();
                for (int i = 0; i < order.GetLength(0); i++)
                {
                    int beforeStockqty = 0;
                    int beforeStockAlarm = 0;
                    int aftereStockqty = 0;
                    int afterStockAlarm = 0;

                    String stockId = "";
                    Boolean before = false;
                    Boolean after = false;
                    Boolean zero = false;
                    sqlConn.Open();
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandText = "select store_stock_qty, stock_alarm from store_stock where item_id = '" + order[i, 0] + "' and store_id = '" + storeId + "'";
                    sqlRd = sqlCmd.ExecuteReader();
                    while (sqlRd.Read())
                    {
                        beforeStockqty = sqlRd.GetInt32(0);
                        beforeStockAlarm = sqlRd.GetInt32(1);

                        if (beforeStockqty > beforeStockAlarm)
                        {
                            before = false;
                        }
                        else if (beforeStockqty <= beforeStockAlarm)
                        {
                            before = true;
                        }
                    }
                    sqlRd.Close();
                    sqlConn.Close();

                    sqlConn.Open();
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandText = "update store_stock set store_stock_qty = store_stock_qty - " + order[i, 2] + "  where item_id = '" + order[i, 0] + "' and store_id = '" + storeId + "'";
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();

                    sqlConn.Open();
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandText = "select stock_alarm , store_stock_id , store_stock_qty from store_stock where item_id = '" + order[i, 0] + "' and store_id = '" + storeId + "'";
                    sqlRd = sqlCmd.ExecuteReader();
                    Console.WriteLine(sqlCmd.CommandText);
                    while (sqlRd.Read())
                    {
                        stockId = sqlRd.GetString(1);
                        aftereStockqty = sqlRd.GetInt32(2);
                        afterStockAlarm = sqlRd.GetInt32(0);

                        if (aftereStockqty == 0)
                        {
                            zero = true;
                        }
                        if (aftereStockqty > afterStockAlarm)
                        {
                            after = false;
                        }
                        else if (aftereStockqty <= afterStockAlarm)
                        {
                            after = true;
                        }
                    }
                    sqlRd.Close();
                    sqlConn.Close();

                    sqlConn.Open();
                    sqlCmd.Connection = sqlConn; //get order id
                    sqlCmd.CommandText = "select order_id from sales_receipt where invoice_id = '" + invoice_id + "'";
                    sqlRd = sqlCmd.ExecuteReader();
                    while (sqlRd.Read())
                    {
                        order_id = sqlRd.GetInt32(0);
                    }
                    sqlRd.Close();
                    sqlConn.Close();
                    if (zero == true)
                    {
                        sqlConn.Open();
                        sqlCmd.Connection = sqlConn;
                        sqlCmd.CommandText = "insert into notification values(default, default,'Alarm Message', '" + stockId + " is out of stock already, please make a re-order request', '" + Main.instance.Staff + "')";
                        sqlCmd.ExecuteNonQuery();
                        sqlConn.Close();
                        Main.instance.increaseNotification();
                        Main.instance.setButtonRedDot();
                    }
                    else if (after == true && before == false)
                    {
                        sqlConn.Open();
                        sqlCmd.Connection = sqlConn;
                        sqlCmd.CommandText = "insert into notification values(default, default,'Alarm Message', '" + stockId + " stock level is low', '" + Main.instance.Staff + "')";
                        sqlCmd.ExecuteNonQuery();
                        sqlConn.Close();
                        Main.instance.increaseNotification();
                        Main.instance.setButtonRedDot();
                    }
                }

                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "update sales_order set order_is_complete = 'y' where order_id = '" + order_id + "'";
                sqlCmd.ExecuteNonQuery();
                sqlConn.Close();

                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "update order_item set is_sale = 'y' where order_id = '" + order_id + "' and is_deposit = 'y'";
                sqlCmd.ExecuteNonQuery();
                sqlConn.Close();

                sqlConn.Open();  //get sequence invoice id number
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select invoice_id from sales_receipt order by invoice_id";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    if (sqlRd.GetInt32(0) == invoice_idForPre)
                    {
                        invoice_idForPre++;
                    }
                    else
                    {
                        break;
                    }
                }
                sqlRd.Close();
                sqlConn.Close();

                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "insert into sales_receipt values(" + invoice_idForPre + ", default, " + order_id + ", 'cash' , " + txtCash.Text + ", 3, '" + Main.instance.Staff + "')"; ;
                sqlCmd.ExecuteNonQuery();
                sqlConn.Close();

                printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("custom", 427, pageHeight);
                printPreviewDialog1.Document = printDocument1;
                printPreviewDialog1.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("confirm beford must be input the tendered");
            }
        }

        public void handleCheckour()
        {
            if (txtChange.Text != "")
            {
                switch (checkout.instance.store)
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

                ConnectString rootconn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = rootconn.getString();
                for (int i = 0; i < order.GetLength(0); i++)
                {
                    if (order[i, 6] != null)
                    {

                        int beforeStockqty = 0;
                        int beforeStockAlarm = 0;
                        int aftereStockqty = 0;
                        int afterStockAlarm = 0;
                        String stockId = "";
                        Boolean before = false;
                        Boolean after = false;
                        Boolean zero = false;
                        sqlConn.Open();
                        sqlCmd.Connection = sqlConn;
                        sqlCmd.CommandText = "select store_stock_qty, stock_alarm from store_stock where item_id = '" + order[i, 0] + "' and store_id = '" + checkout.instance.store + "'";
                        sqlRd = sqlCmd.ExecuteReader();
                        while (sqlRd.Read())
                        {
                            beforeStockqty = sqlRd.GetInt32(0);
                            beforeStockAlarm = sqlRd.GetInt32(1);

                            if (beforeStockqty > beforeStockAlarm)
                            {
                                before = false;
                            }
                            else if (beforeStockqty <= beforeStockAlarm)
                            {
                                before = true;
                            }
                        }
                        sqlRd.Close();
                        sqlConn.Close();


                        sqlConn.Open();
                        sqlCmd.Connection = sqlConn;
                        sqlCmd.CommandText = "update store_stock set store_stock_qty = " + order[i, 6] + "  where item_id = '" + order[i, 0] + "' and store_id = '" + checkout.instance.store + "'";
                        sqlCmd.ExecuteNonQuery();
                        sqlConn.Close();

                        sqlConn.Open();
                        sqlCmd.Connection = sqlConn;
                        sqlCmd.CommandText = "select stock_alarm , store_stock_id , store_stock_qty from store_stock where item_id = '" + order[i, 0] + "' and store_id = '" + checkout.instance.store + "'";
                        sqlRd = sqlCmd.ExecuteReader();
                        while (sqlRd.Read())
                        {
                            stockId = sqlRd.GetString(1);
                            aftereStockqty = sqlRd.GetInt32(2);
                            afterStockAlarm = sqlRd.GetInt32(0);

                            if (aftereStockqty == 0)
                            {
                                zero = true;
                            }
                            if (aftereStockqty > afterStockAlarm)
                            {
                                after = false;
                            }
                            else if (aftereStockqty <= afterStockAlarm)
                            {
                                after = true;
                            }
                        }
                        sqlRd.Close();
                        sqlConn.Close();

                        if (zero == true)
                        {
                            sqlConn.Open();
                            sqlCmd.Connection = sqlConn;
                            sqlCmd.CommandText = "insert into notification values(default, default,'Alarm Message', '" + stockId + " is out of stock already, please make a re-order request', '" + Main.instance.Staff + "')";
                            sqlCmd.ExecuteNonQuery();
                            sqlConn.Close();
                            Main.instance.increaseNotification();
                            Main.instance.setButtonRedDot();
                        }
                        else if (after == true && before == false)
                        {
                            sqlConn.Open();
                            sqlCmd.Connection = sqlConn;
                            sqlCmd.CommandText = "insert into notification values(default, default,'Alarm Message', '" + stockId + " stock level is low', '" + Main.instance.Staff + "')";
                            sqlCmd.ExecuteNonQuery();
                            sqlConn.Close();
                            Main.instance.increaseNotification();
                            Main.instance.setButtonRedDot();
                        }
                    }
                    else
                    {
                        //handle deposit payment
                    }
                }

                sqlConn.Open();  //get staff full name
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select staff_id from staff where staff_id = '" + Main.instance.Staff + "'";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    staffName = sqlRd.GetString(0);
                }
                sqlRd.Close();
                sqlConn.Close();

                sqlConn.Open();  //get sequence order id number
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select order_id from sales_order order by order_id";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    if (sqlRd.GetInt32(0) == order_id)
                    {
                        order_id++;
                    }
                    else
                    {
                        break;
                    }
                }
                sqlRd.Close();
                sqlConn.Close();

                calculateReceipttotal();
                if (!deposit)
                {
                    sqlConn.Open();
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandText = "insert into sales_order values(" + order_id + ", " + calculateOrderTotal() + ", " + calculateOrderQty() +
                        ", " + 0 + ", '" + "n" + "','" + "n" + "','" + checkout.instance.remark + "',default,'" + checkout.instance.store + "', '" + Main.instance.Staff + "', default)";
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                }
                else
                {
                    sqlConn.Open();
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandText = "insert into sales_order values(" + order_id + ", " + calculateOrderTotal() + ", " + calculateOrderQty() +
                        ", " + String.Format("{0:0.#}", (depositTotal - paidDeposit)).ToString() + ", '" + "n" + "','" + "y" + "','" + checkout.instance.remark + "',default,'" + checkout.instance.store + "', '" + Main.instance.Staff + "', 'n')";
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                }

                for (int i = 0; i < order.GetLength(0); i++)
                {
                    if (order[i, 6] != null)
                    {
                        sqlConn.Open();
                        sqlCmd.Connection = sqlConn;
                        sqlCmd.CommandText = "SELECT store_stock_id FROM sdp2.store_stock " +
                            "where item_id = '" + order[i, 0] + "' and store_id = '" + checkout.instance.store + "'";
                        sqlRd = sqlCmd.ExecuteReader();
                        while (sqlRd.Read())
                        {
                            dicStockID.Add(order[i, 0], sqlRd.GetString(0));
                        }
                        sqlRd.Close();
                        sqlConn.Close();
                    }
                    else
                    {
                        sqlConn.Open();
                        sqlCmd.Connection = sqlConn;
                        sqlCmd.CommandText = "SELECT store_stock_id FROM sdp2.store_stock " +
                            "where item_id = '" + order[i, 0] + "' and store_id = '" + checkout.instance.store + "'";
                        sqlRd = sqlCmd.ExecuteReader();
                        while (sqlRd.Read())
                        {
                            dicDepositStockID.Add(order[i, 0], sqlRd.GetString(0));
                        }
                        sqlRd.Close();
                        sqlConn.Close();
                    }
                }

                for (int i = 0; i < order.GetLength(0); i++)
                {
                    if (order[i, 6] != null)
                    {
                        sqlConn.Open();
                        sqlCmd.Connection = sqlConn;
                        sqlCmd.CommandText = "insert into order_item values(default, " + order_id + ", '" + dicStockID[order[i, 0]] + "', '" + order[i, 0] + "', " + order[i, 2] + ", '" + order[i, 3] + "' , " + order[i, 5] + ", 'n','y')";
                        sqlCmd.ExecuteNonQuery();
                        sqlConn.Close();
                    }
                    else
                    {
                        sqlConn.Open();
                        sqlCmd.Connection = sqlConn;
                        sqlCmd.CommandText = "insert into order_item values(default, " + order_id + ", '" + dicDepositStockID[order[i, 0]] + "', '" + order[i, 0] + "', " + order[i, 2] + ", '" + order[i, 3] + "', " +
                            (Convert.ToInt32(order[i, 2]) * Convert.ToDouble(order[i, 3]) * (1 - Convert.ToDouble(order[i, 4]) / 100)) + ", 'y' , 'n')";
                        sqlCmd.ExecuteNonQuery();
                        sqlConn.Close();
                    }
                }

                invoice_id = 400000001;
                sqlConn.Open();  //get sequence order id number
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select invoice_id from sales_receipt order by invoice_id";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    if (sqlRd.GetInt32(0) == invoice_id)
                    {
                        invoice_id++;
                    }
                    else
                    {
                        break;
                    }
                }
                sqlRd.Close();
                sqlConn.Close();

                if (deposit)
                {
                    sqlConn.Open();
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandText = "insert into sales_receipt values(" + invoice_id + ", default, " + order_id + ", 'cash' , " + txtCash.Text + ", 2, '" + Main.instance.Staff + "')";
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                }
                else
                {
                    sqlConn.Open();
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandText = "insert into sales_receipt values(" + invoice_id + ", default, " + order_id + ", 'cash' , " + txtCash.Text + ", 1, '" + Main.instance.Staff + "')";
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                }

                /*        sqlConn.Open();
                        sqlCmd.Connection = sqlConn;
                        sqlCmd.CommandText = "SELECT invoice_id FROM sales_receipt;";
                        sqlRd = sqlCmd.ExecuteReader();
                        while (sqlRd.Read())
                        {
                            if (sqlRd.GetInt32(0) == invoiceId)
                            {
                                invoiceId++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        sqlRd.Close();
                        sqlConn.Close();*/
                printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("custom", 427, checkout.instance.pageHeight);
                printPreviewDialog1.Document = printDocument1;
                printPreviewDialog1.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("confirm beford must be input the tendered");
            }
        }

        public int calculateOrderQty()
        {
            int quantity = 0;
            for (int i = 0; i < order.GetLength(0); i++)
            {
                quantity += Convert.ToInt32(order[i, 2]);
            }
            return quantity;
        }

        public Double calculateOrderTotal()
        {
            Double ordertotal = 0;
            for (int i = 0; i < order.GetLength(0); i++)
            {
                ordertotal += (Convert.ToDouble(order[i, 2]) * Convert.ToDouble(order[i, 3]) * (1 - Convert.ToDouble(order[i, 4]) / 100));
            }
            return Convert.ToDouble(String.Format("{0:0.#}", ordertotal));
        }

        public void calculateReceipttotal()
        {
            originalTotal = 0;
            total = 0;
            discount = 0;
            depositTotal = 0;
            for (int i = 0; i < order.GetLength(0); i++)
            {
                if (order[i, 6] != null)
                {
                    originalTotal += Convert.ToDouble(order[i, 2]) * Convert.ToDouble(order[i, 3]);
                    total += (Convert.ToDouble(order[i, 2]) * Convert.ToDouble(order[i, 3]) * (1 - Convert.ToDouble(order[i, 4]) / 100));
                    discount += ((Convert.ToDouble(order[i, 2]) * Convert.ToDouble(order[i, 3]))
                        - ((Convert.ToDouble(order[i, 2]) * Convert.ToDouble(order[i, 3]) * (1 - Convert.ToDouble(order[i, 4]) / 100))));
                }
                else
                {
                    depositTotal += (Convert.ToDouble(order[i, 2]) * Convert.ToDouble(order[i, 3]) * (1 - Convert.ToDouble(order[i, 4]) / 100));
                    discount += ((Convert.ToDouble(order[i, 2]) * Convert.ToDouble(order[i, 3]))
                        - ((Convert.ToDouble(order[i, 2]) * Convert.ToDouble(order[i, 3]) * (1 - Convert.ToDouble(order[i, 4]) / 100))));
                }
            }
        }

        public void setOrder(String[,] order)
        {
            this.order = order;
        }

        public void checkDeposit(Boolean deposit)
        {
            this.deposit = deposit;
        }

        public void setPaidDeposti(Double paidDeposit)
        {
            this.paidDeposit = paidDeposit;
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (action != "preorder")
            {
                calculateReceipttotal();
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
                e.Graphics.DrawString("STORE: " + checkout.instance.store, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 170, sf1);
                e.Graphics.DrawString("STAFF: " + staffName, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, 170, sf);
                e.Graphics.DrawString("INVOICE NO.: " + invoice_id, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 200, sf1);
                e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                    e.MarginBounds.Left + (e.MarginBounds.Width / 2), 220, sf);
                for (int i = 0; i < order.GetLength(0); i++)
                {
                    if (order[i, 6] != null)
                    {
                        height += 40;
                        e.Graphics.DrawString(order[i, 1], new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                        // e.Graphics.DrawString((Convert.ToDouble(order[i,2]) * Convert.ToDouble(order[i, 3])).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        e.Graphics.DrawString(Convert.ToDouble(order[i, 5]).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString(order[i, 0] + "   " +
                            order[i, 2] + "@ " +
                            order[i, 3], new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                    }
                    else
                    {
                        height += 40;
                        e.Graphics.DrawString(order[i, 1], new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                        e.Graphics.DrawString("(UNPAID)", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 75, height);
                        e.Graphics.DrawString((Convert.ToInt32(order[i, 2]) * Convert.ToDouble(order[i, 3]) * (1 - Convert.ToDouble(order[i, 4]) / 100)).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString(order[i, 0] + "   " +
                            order[i, 2] + "@ " +
                            order[i, 3], new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                    }
                }

                height += 30;
                e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 20;
                e.Graphics.DrawString("PRE-ORDER", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                if (!deposit)
                {
                    e.Graphics.DrawString("NO", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                }
                else
                {
                    e.Graphics.DrawString("Yes", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                }
                height += 20;
                e.Graphics.DrawString("PAID DEPOSIT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                if (!deposit)
                {
                    e.Graphics.DrawString("0.0", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                }
                else
                {
                    e.Graphics.DrawString(String.Format("{0:0.#}", paidDeposit).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                }
                height += 20;
                e.Graphics.DrawString("ORIGINAL TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.#}", originalTotal).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("UNPAID", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                if (!deposit)
                {
                    e.Graphics.DrawString("0.0", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                }
                else
                {
                    e.Graphics.DrawString(String.Format("{0:0.#}", (depositTotal - paidDeposit)).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                }
                height += 20;
                e.Graphics.DrawString("DISCOUNT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.#}", discount), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.#}", (total + paidDeposit)), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 30;
                e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 20;
                e.Graphics.DrawString("CASH TENDERED", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(cashTendered.ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("CHANGE", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(change.ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 30;
                e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 20;
                // e.Graphics.DrawString("Tran:155100", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(DateTime.Now.ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 100, height);
                height += 20;
                e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 30;
                e.Graphics.DrawString("Thank For Shopping", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
            }
            else
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
                e.Graphics.DrawString("STAFF: " + staffName, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, 170, sf);
                e.Graphics.DrawString("INVOICE NO.: " + invoice_idForPre, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 200, sf1);
                e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                    e.MarginBounds.Left + (e.MarginBounds.Width / 2), 220, sf);
                for (int i = 0; i < order.GetLength(0); i++)
                {
                    height += 40;
                    e.Graphics.DrawString(order[i, 1], new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                    // e.Graphics.DrawString((Convert.ToDouble(order[i,2]) * Convert.ToDouble(order[i, 3])).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                    e.Graphics.DrawString(Convert.ToDouble(order[i, 5]).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                    height += 20;
                    e.Graphics.DrawString(order[i, 0] + "   " +
                        order[i, 2] + "@ " +
                        order[i, 3], new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                }

                height += 30;
                e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 20;
                e.Graphics.DrawString("PRE-ORDER", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString("NO", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("PAID DEPOSIT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.#}", paidDeposit).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("ORIGINAL TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.#}", originalTotal).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("UNPAID", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.#}", 0), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("DISCOUNT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.#}", discount), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(String.Format("{0:0.#}", total), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 30;
                e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 20;
                e.Graphics.DrawString("CASH TENDERED", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(cashTendered.ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 20;
                e.Graphics.DrawString("CHANGE", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(change.ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                height += 30;
                e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 20;
                // e.Graphics.DrawString("Tran:155100", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                e.Graphics.DrawString(DateTime.Now.ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 100, height);
                height += 20;
                e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                height += 30;
                e.Graphics.DrawString("Thank For Shopping", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (action == "preorder")
            {
                this.Close();
                PreOrderCheckout.instance.setCashAction("cancell");
            }
            else
            {
                this.Close();
                checkout.instance.setCashAction("cancell");
            }
        }


        private void txtChange_TextChanged(object sender, EventArgs e)
        {

        }

        public void setAction(String action)
        {
            this.action = action;
        }

        public void setStaffName(String staffname)
        {
            this.staffName = staffname;
        }

        public void setStoreId(String storeId)
        {
            this.storeId = storeId;
        }

        public void setOriginalTotal(Double originalTotal)
        {
            this.originalTotal = originalTotal;
        }

        public void setpPidDeposit(Double paidDeposit)
        {
            this.paidDeposit = paidDeposit;
        }

        public void setDiscount(Double discount)
        {
            this.discount = discount;
        }

        public void setTotal(Double total)
        {
            this.total = total;
        }

        public void setPageHeight(int pageHeight)
        {
            this.pageHeight = pageHeight;
        }

        public void setInvoiceId(int invoice_id)
        {
            this.invoice_id = invoice_id;
        }
    }
}
