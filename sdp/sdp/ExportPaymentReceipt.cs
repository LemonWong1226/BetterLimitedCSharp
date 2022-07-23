using MySql.Data.MySqlClient;
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

namespace sdp
{
    public partial class ExportPaymentReceipt : Form
    {
        private int height;
        public int pageHeight;
        private int pagecount;
        private int count = 0;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private List<invoice> invoiceitemslist = new List<invoice>();
        //   private String[] itemDetail = new String[6];
        private int page = 0;
        private Dictionary<String, int> dicIncoiceList = new Dictionary<String, int>();
        private Dictionary<String, double> dicDeposit = new Dictionary<String, double>();
        private Dictionary<String, double> dicTender = new Dictionary<String, double>();
        private Dictionary<String, String> dicShop = new Dictionary<String, String>();
        private Dictionary<String, String> dicStaff = new Dictionary<String, String>();
        private Dictionary<String, String> dicDate = new Dictionary<String, String>();
        private string language;

        public ExportPaymentReceipt()
        {
            InitializeComponent();
        }

        public class invoice
        {
            public string invoicId { get; set; }
            public string itemId { get; set; }
            public string itemName { get; set; }
            public int quantity { get; set; }
            public double price { get; set; }
            public int discount { get; set; }
            public string total { get; set; }
        }

        private void ExportPaymentReceipt_Load(object sender, EventArgs e)
        {
            changeLanguage();
            dtp1.MinDate = new DateTime(1985, 6, 20);
            dtp1.CustomFormat = "yyyy-MM-dd";
            dtp1.Format = DateTimePickerFormat.Custom;
            dtp2.MinDate = new DateTime(1985, 6, 20);
            dtp2.CustomFormat = "yyyy-MM-dd";
            dtp2.Format = DateTimePickerFormat.Custom;
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
                label1.Text = "发票号: ";
                label2.Text = "日期:";
                label4.Text = "至";
                label3.Text = "商店编号:";
                btnSubmit.Text = "输出";
                btnBack.Text = "返回";
                cbxinvoice.Text = "一张发票导出输出";
            }
            else
            {
                label1.Text = "Sale Invoice ID: ";
                label2.Text = "Date:";
                label4.Text = "To";
                label3.Text = "Store Id:";
                btnSubmit.Text = "Export";
                btnBack.Text = "back";
                cbxinvoice.Text = "Export for one invoice";
            }
        }

        public void updateDictionary(String date1, String date2, String store, String action, String invoice)
        {
            if (action == "more")
            {
                invoiceitemslist.Clear();
                count = 0;
                dicIncoiceList.Clear();
                dicDeposit.Clear();
                dicShop.Clear();
                dicStaff.Clear();
                dicTender.Clear();
                dicDate.Clear();
                sqlConn.ConnectionString = ConnectString.ConnectionString;

                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select invoice_id, receipt_type, cash_tender,order_deposit, store_id, sales_receipt.staff_id,sales_datetime " +
                                        "from sales_receipt " +
                                        "inner join sales_order " +
                                        "on sales_order.order_id = sales_receipt.order_id " +
                                        "WHERE sales_datetime >= '" + date1 + " 0:0:0' and sales_datetime <= '" + date2 + " 23:59:59' " +
                                        "and store_id like '%" + store + "%' " +
                                        //                      "and order_is_complete = 'y'" +
                                        "order by sales_datetime";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    dicIncoiceList.Add(sqlRd.GetString(0), sqlRd.GetInt32(1));
                    dicTender.Add(sqlRd.GetString(0), sqlRd.GetInt32(2));
                    dicDeposit.Add(sqlRd.GetString(0), sqlRd.GetInt32(3));
                    dicShop.Add(sqlRd.GetString(0), sqlRd.GetString(4));
                    dicStaff.Add(sqlRd.GetString(0), sqlRd.GetString(5));
                    dicDate.Add(sqlRd.GetString(0), sqlRd.GetString(6));
                }
                sqlRd.Close();
                sqlConn.Close();
                invoiceToitem();
            }
            else if (action == "one")
            {
                invoiceitemslist.Clear();
                count = 0;
                dicIncoiceList.Clear();
                dicDeposit.Clear();
                dicShop.Clear();
                dicStaff.Clear();
                dicTender.Clear();
                dicDate.Clear();
                sqlConn.ConnectionString = ConnectString.ConnectionString;

                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select invoice_id, receipt_type, cash_tender,order_deposit, store_id, sales_receipt.staff_id,sales_datetime " +
                                        "from sales_receipt " +
                                        "inner join sales_order " +
                                        "on sales_order.order_id = sales_receipt.order_id " +
                                        "WHERE invoice_id = '" + invoice + "'";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    dicIncoiceList.Add(sqlRd.GetString(0), sqlRd.GetInt32(1));
                    dicTender.Add(sqlRd.GetString(0), sqlRd.GetInt32(2));
                    dicDeposit.Add(sqlRd.GetString(0), sqlRd.GetInt32(3));
                    dicShop.Add(sqlRd.GetString(0), sqlRd.GetString(4));
                    dicStaff.Add(sqlRd.GetString(0), sqlRd.GetString(5));
                    dicDate.Add(sqlRd.GetString(0), sqlRd.GetString(6));
                }
                sqlRd.Close();
                sqlConn.Close();
                invoiceToitem();
            }
        }

        public void invoiceToitem()
        {
            count = 0;
            foreach (var invoice in dicIncoiceList)
            {
                if (invoice.Value == 1)
                {
                    handleType1(invoice.Key);
                }
                if (invoice.Value == 2)
                {
                    handleType2(invoice.Key);
                }
                if (invoice.Value == 3)
                {
                    handleType3(invoice.Key);
                }
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (cbxinvoice.Checked == false)
            {
                page = 0;
                pagecount = 0;
                if (cbxStoreId.Text == "" || cbxStoreId.Text == "BOTH")
                {
                    updateDictionary(dtp1.Text, dtp2.Text, "", "more", "");
                }
                else if (cbxStoreId.Text == "HK01")
                {
                    updateDictionary(dtp1.Text, dtp2.Text, "HK01", "more", "");
                }
                else if (cbxStoreId.Text == "HK02")
                {
                    updateDictionary(dtp1.Text, dtp2.Text, "HK02", "more", "");
                }
                int itemsCount = 0;
                var count1 =
                   from invoice in invoiceitemslist
                   where invoice.invoicId == dicIncoiceList.ElementAt(pagecount).Key
                   group invoice by invoice.invoicId into invoicGroup
                   select new
                   {
                       itemCount = invoicGroup.Count(),
                   };
                foreach (var a in count1)
                {
                    itemsCount = a.itemCount;
                }
                printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("custom", 427, 560 + 60 * itemsCount);
                printPreviewDialog1.Document = printDocument1;
                printPreviewDialog1.ShowDialog();
            }
            else
            {
                page = 0;
                pagecount = 0;
                updateDictionary(dtp1.Text, dtp2.Text, "HK02", "one", txtInvoiceId.Text);
                int itemsCount = 0;
                var count1 =
                   from invoice in invoiceitemslist
                   where invoice.invoicId == dicIncoiceList.ElementAt(pagecount).Key
                   group invoice by invoice.invoicId into invoicGroup
                   select new
                   {
                       itemCount = invoicGroup.Count(),
                   };
                foreach (var a in count1)
                {
                    itemsCount = a.itemCount;
                }
                printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("custom", 427, 560 + 60 * itemsCount);
                printPreviewDialog1.Document = printDocument1;
                printPreviewDialog1.ShowDialog();
            }
        }


        public void handleType1(String invoice)
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
                invoiceitemslist.Add(new invoice
                {
                    invoicId = invoice,
                    itemId = sqlRd.GetString(0),
                    itemName = sqlRd.GetString(1),
                    quantity = sqlRd.GetInt32(2),
                    price = sqlRd.GetDouble(3),
                    discount = sqlRd.GetInt32(4),
                    total = sqlRd.GetString(5)
                });
            }
            count++;
            sqlRd.Close();
            sqlConn.Close();
        }

        public void handleType2(String invoice)
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
                    invoiceitemslist.Add(new invoice
                    {
                        invoicId = invoice,
                        itemId = sqlRd.GetString(0),
                        itemName = sqlRd.GetString(1),
                        quantity = sqlRd.GetInt32(2),
                        price = sqlRd.GetDouble(3),
                        discount = sqlRd.GetInt32(4),
                        total = "Deposit"
                    });
                }
                else
                {
                    invoiceitemslist.Add(new invoice
                    {
                        invoicId = invoice,
                        itemId = sqlRd.GetString(0),
                        itemName = sqlRd.GetString(1),
                        quantity = sqlRd.GetInt32(2),
                        price = sqlRd.GetDouble(3),
                        discount = sqlRd.GetInt32(4),
                        total = sqlRd.GetString(5)
                    });
                }
            }
            sqlRd.Close();
            sqlConn.Close();
        }

        public void handleType3(String invoice)
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
                invoiceitemslist.Add(new invoice
                {
                    invoicId = invoice,
                    itemId = sqlRd.GetString(0),
                    itemName = sqlRd.GetString(1),
                    quantity = sqlRd.GetInt32(2),
                    price = sqlRd.GetDouble(3),
                    discount = sqlRd.GetInt32(4),
                    total = sqlRd.GetString(5)
                });
            }
            sqlRd.Close();
            sqlConn.Close();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                e.HasMorePages = false;
                int height = 220;
                if (page == dicIncoiceList.Count - 1)
                {
                    if (dicIncoiceList.ElementAt(pagecount).Value == 1)
                    {
                        String address1 = "";
                        String address2 = "";
                        double total = 0;
                        double origanalTotal = 0;
                        var recepiteType1 =
                           from invoice in invoiceitemslist
                           where invoice.invoicId == dicIncoiceList.ElementAt(pagecount).Key
                           group invoice by invoice.invoicId into invoicGroup
                           select new
                           {
                               Total = invoicGroup.Sum(pc => Convert.ToDouble(pc.total)).ToString(),
                               OriganalTotal = invoicGroup.Sum(pc => Convert.ToInt32(pc.quantity) * Convert.ToDouble(pc.price)).ToString(),
                           };
                        foreach (var a in recepiteType1)
                        {
                            total = Convert.ToDouble(a.Total);
                            origanalTotal = Convert.ToDouble(a.OriganalTotal);
                        }
                        switch (dicShop.ElementAt(pagecount).Value)
                        {
                            case "HK01":
                                address1 = "KOWLOON";
                                address2 = "B102, LAM CHAK STREET";
                                break;
                            case "HK02":
                                address1 = "TSAUN WAN";
                                address2 = "LAM CHAK STREET";
                                break;
                        }
                        StringFormat sf = new StringFormat();
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Center;
                        StringFormat sf1 = new StringFormat();
                        sf1.LineAlignment = StringAlignment.Center;
                        // int stringlength;
                        //  stringlength = (int) Convert.ToDouble(e.Graphics.MeasureString("KOWLOON BAY STORE", new Font("Mircosoft Sans Serif", 20, FontStyle.Bold)).Width.ToString());
                        e.Graphics.DrawString(address1, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 40, sf);
                        e.Graphics.DrawString(address2, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 100, sf);
                        e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                            e.MarginBounds.Left + (e.MarginBounds.Width / 2), 140, sf);
                        e.Graphics.DrawString("STORE: " + dicShop.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 170, sf1);
                        e.Graphics.DrawString("STAFF: " + dicStaff.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, 170, sf);
                        e.Graphics.DrawString("INVOICE NO.: " + dicIncoiceList.ElementAt(pagecount).Key, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 200, sf1);
                        e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                            e.MarginBounds.Left + (e.MarginBounds.Width / 2), 220, sf);
                        var recepiteType1item =
                               from item in invoiceitemslist
                               where item.invoicId == dicIncoiceList.ElementAt(pagecount).Key.ToString()
                               select item;

                        foreach (var a in recepiteType1item)
                        {
                            height += 40;
                            e.Graphics.DrawString(a.itemName, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                            e.Graphics.DrawString(String.Format("{0:0.0}", a.total), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                            height += 20;
                            e.Graphics.DrawString(a.itemId + "   " +
                                a.quantity + "@ " +
                                String.Format("{0:0.0}", a.price), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
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
                        e.Graphics.DrawString(String.Format("{0:0.0}", origanalTotal).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("UNPAID", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString("0.0", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("DISCOUNT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", (origanalTotal - total)), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", (total)), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 30;
                        e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 20;
                        e.Graphics.DrawString("CASH TENDERED", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", dicTender.ElementAt(pagecount).Value), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("CHANGE", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", dicTender.ElementAt(pagecount).Value - total), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 30;
                        e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 20;
                        // e.Graphics.DrawString("Tran:155100", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(dicDate.ElementAt(pagecount).Value.ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 100, height); ;
                        height += 20;
                        e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 30;
                        e.Graphics.DrawString("Thank For Shopping", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        pagecount++;

                    }
                    else if (dicIncoiceList.ElementAt(pagecount).Value == 2)
                    {
                        String address1 = "";
                        String address2 = "";
                        switch (dicShop.ElementAt(pagecount).Value)
                        {
                            case "HK01":
                                address1 = "KOWLOON";
                                address2 = "B102, LAM CHAK STREET";
                                break;
                            case "HK02":
                                address1 = "TSAUN WAN";
                                address2 = "LAM CHAK STREET";
                                break;
                        }
                        StringFormat sf = new StringFormat();
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Center;
                        StringFormat sf1 = new StringFormat();
                        sf1.LineAlignment = StringAlignment.Center;
                        IEnumerable<invoice> recepiteType2 =
                            from invoice in invoiceitemslist
                            where invoice.invoicId == dicIncoiceList.ElementAt(pagecount).Key.ToString()
                            select invoice;
                        double total = 0;
                        double origanalTotal = 0;
                        double depositTotal = 0;
                        double depositOriganalTotal = 0;
                        foreach (var b in recepiteType2)
                        {
                            if (b.total != "Deposit")
                            {
                                total += ((Convert.ToDouble(b.price) * Convert.ToInt32(b.quantity)) *
                                (1 - Convert.ToDouble(b.discount) / 100));
                                origanalTotal += (Convert.ToDouble(b.price) * Convert.ToInt32(b.quantity));
                            }
                            else
                            {
                                depositTotal += ((Convert.ToDouble(b.price) * Convert.ToInt32(b.quantity)) *
                                (1 - Convert.ToDouble(b.discount) / 100));
                                depositOriganalTotal += (Convert.ToDouble(b.price) * Convert.ToInt32(b.quantity));
                            }
                        }
                        // int stringlength;
                        //  stringlength = (int) Convert.ToDouble(e.Graphics.MeasureString("KOWLOON BAY STORE", new Font("Mircosoft Sans Serif", 20, FontStyle.Bold)).Width.ToString());
                        e.Graphics.DrawString(address1, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 40, sf);
                        e.Graphics.DrawString(address2, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 100, sf);
                        e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                            e.MarginBounds.Left + (e.MarginBounds.Width / 2), 140, sf);
                        e.Graphics.DrawString("STORE: " + dicShop.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 170, sf1);
                        e.Graphics.DrawString("STAFF: " + dicStaff.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, 170, sf);
                        e.Graphics.DrawString("INVOICE NO.: " + dicIncoiceList.ElementAt(pagecount).Key, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 200, sf1);
                        e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                            e.MarginBounds.Left + (e.MarginBounds.Width / 2), 220, sf);
                        var recepiteType2item =
                           from item in invoiceitemslist
                           where item.invoicId == dicIncoiceList.ElementAt(pagecount).Key.ToString()
                           select item;
                        foreach (var a in recepiteType2item)
                        {
                            if (a.total != "Deposit")
                            {
                                height += 40;
                                e.Graphics.DrawString(a.itemId, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                                e.Graphics.DrawString(String.Format("{0:0.0}", a.total), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                                height += 20;
                                e.Graphics.DrawString(a.itemId + "   " +
                                    a.quantity + "@ " +
                                    String.Format("{0:0.0}", a.price), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                            }
                            else
                            {
                                height += 40;
                                e.Graphics.DrawString(a.itemId, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                                e.Graphics.DrawString("(UNPAID)", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 75, height);
                                e.Graphics.DrawString(String.Format("{0:0.0}", (a.quantity) *
                                    (a.price) *
                                    (1 - Convert.ToDouble(a.discount) / 100)),
                                    new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                                height += 20;
                                e.Graphics.DrawString(a.itemId + "   " +
                                    a.quantity + "@ " +
                                    String.Format("{0:0.0}", a.price), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                            }
                        }

                        height += 30;
                        e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 20;
                        e.Graphics.DrawString("PRE-ORDER", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString("No", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("PAID DEPOSIT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", depositTotal - dicDeposit.ElementAt(pagecount).Value).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("ORIGINAL TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", origanalTotal).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("UNPAID", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", dicDeposit.ElementAt(pagecount).Value), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("DISCOUNT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", origanalTotal - total + depositOriganalTotal - depositTotal), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", (total + depositTotal - dicDeposit.ElementAt(pagecount).Value)), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 30;
                        e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 20;
                        e.Graphics.DrawString("CASH TENDERED", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", dicTender.ElementAt(pagecount).Value), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("CHANGE", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", dicTender.ElementAt(pagecount).Value - (total + depositTotal - dicDeposit.ElementAt(pagecount).Value)), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 30;
                        e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 20;
                        // e.Graphics.DrawString("Tran:155100", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(dicDate.ElementAt(pagecount).Value.ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 100, height);
                        height += 20;
                        e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 30;
                        e.Graphics.DrawString("Thank For Shopping", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                    }
                    else if (dicIncoiceList.ElementAt(pagecount).Value == 3)
                    {
                        String address1 = "";
                        String address2 = "";
                        switch (dicShop.ElementAt(pagecount).Value)
                        {
                            case "HK01":
                                address1 = "KOWLOON";
                                address2 = "B102, LAM CHAK STREET";
                                break;
                            case "HK02":
                                address1 = "TSAUN WAN";
                                address2 = "LAM CHAK STREET";
                                break;
                        }
                        double total = 0;
                        double origanalTotal = 0;
                        var recepiteType3 =
                           from invoice in invoiceitemslist
                           where invoice.invoicId == dicIncoiceList.ElementAt(pagecount).Key.ToString()
                           group invoice by invoice.invoicId into invoicGroup
                           select new
                           {
                               Total = invoicGroup.Sum(pc => Convert.ToDouble(pc.total)).ToString(),
                               OriganalTotal = invoicGroup.Sum(pc => Convert.ToInt32(pc.quantity) * Convert.ToDouble(pc.price)).ToString(),
                           };
                        foreach (var a in recepiteType3)
                        {
                            total = Convert.ToDouble(a.Total);
                            origanalTotal = Convert.ToDouble(a.OriganalTotal);
                        }
                        StringFormat sf = new StringFormat();
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Center;
                        StringFormat sf1 = new StringFormat();
                        sf1.LineAlignment = StringAlignment.Center;
                        // int stringlength;
                        //  stringlength = (int) Convert.ToDouble(e.Graphics.MeasureString("KOWLOON BAY STORE", new Font("Mircosoft Sans Serif", 20, FontStyle.Bold)).Width.ToString());
                        e.Graphics.DrawString(address1, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 40, sf);
                        e.Graphics.DrawString(address2, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 100, sf);
                        e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                            e.MarginBounds.Left + (e.MarginBounds.Width / 2), 140, sf);
                        e.Graphics.DrawString("STORE: " + dicShop.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 170, sf1);
                        e.Graphics.DrawString("STAFF: " + dicStaff.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, 170, sf);
                        e.Graphics.DrawString("INVOICE NO.: " + dicIncoiceList.ElementAt(pagecount).Key, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 200, sf1);
                        e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                            e.MarginBounds.Left + (e.MarginBounds.Width / 2), 220, sf);

                        var recepiteType3item =
                           from invoice in invoiceitemslist
                           where invoice.invoicId == dicIncoiceList.ElementAt(pagecount).Key.ToString()
                           select invoice;
                        foreach (var a in recepiteType3item)
                        {
                            height += 40;
                            e.Graphics.DrawString(a.itemName, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                            e.Graphics.DrawString(String.Format("{0:0.0}", a.total), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                            height += 20;
                            e.Graphics.DrawString(a.itemId + "   " +
                                a.quantity + "@ " +
                                String.Format("{0:0.0}", a.price), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                        }

                        height += 30;
                        e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 20;
                        e.Graphics.DrawString("PRE-ORDER", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString("No", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("PAID DEPOSIT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", total - dicDeposit.ElementAt(pagecount).Value).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("ORIGINAL TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", origanalTotal).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("UNPAID", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", 0.0), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("DISCOUNT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", origanalTotal - total), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", dicDeposit.ElementAt(pagecount).Value), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 30;
                        e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 20;
                        e.Graphics.DrawString("CASH TENDERED", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", dicTender.ElementAt(pagecount).Value), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("CHANGE", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", dicTender.ElementAt(pagecount).Value - dicDeposit.ElementAt(pagecount).Value), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 30;
                        e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 20;
                        // e.Graphics.DrawString("Tran:155100", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(dicDate.ElementAt(pagecount).Value.ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 100, height);
                        height += 20;
                        e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 30;
                        e.Graphics.DrawString("Thank For Shopping", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        pagecount++;
                    }
                    e.HasMorePages = false;
                    pagecount = 0;
                }
                else
                {
                    if (dicIncoiceList.ElementAt(pagecount).Value == 1)
                    {
                        String address1 = "";
                        String address2 = "";
                        switch (dicShop.ElementAt(pagecount).Value)
                        {
                            case "HK01":
                                address1 = "KOWLOON";
                                address2 = "B102, LAM CHAK STREET";
                                break;
                            case "HK02":
                                address1 = "TSAUN WAN";
                                address2 = "LAM CHAK STREET";
                                break;
                        }
                        double total = 0;
                        double origanalTotal = 0;
                        IEnumerable<invoice> recepiteType2 =
                        from invoice in invoiceitemslist
                        where invoice.invoicId == dicIncoiceList.ElementAt(pagecount).Key
                        select invoice;

                        foreach (var b in recepiteType2)
                        {
                            if (b.total != "Deposit")
                            {
                                total += ((Convert.ToDouble(b.price) * Convert.ToInt32(b.quantity)) *
                                (1 - Convert.ToDouble(b.discount) / 100));
                                origanalTotal += (Convert.ToDouble(b.price) * Convert.ToInt32(b.quantity));
                            }
                        }
                        StringFormat sf = new StringFormat();
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Center;
                        StringFormat sf1 = new StringFormat();
                        sf1.LineAlignment = StringAlignment.Center;
                        // int stringlength;
                        //  stringlength = (int) Convert.ToDouble(e.Graphics.MeasureString("KOWLOON BAY STORE", new Font("Mircosoft Sans Serif", 20, FontStyle.Bold)).Width.ToString());
                        e.Graphics.DrawString(address1, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 40, sf);
                        e.Graphics.DrawString(address2, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 100, sf);
                        e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                            e.MarginBounds.Left + (e.MarginBounds.Width / 2), 140, sf);
                        e.Graphics.DrawString("STORE: " + dicShop.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 170, sf1);
                        e.Graphics.DrawString("STAFF: " + dicStaff.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, 170, sf);
                        e.Graphics.DrawString("INVOICE NO.: " + dicIncoiceList.ElementAt(pagecount).Key, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 200, sf1);
                        e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                            e.MarginBounds.Left + (e.MarginBounds.Width / 2), 220, sf);
                        var recepiteType1item =
                               from item in invoiceitemslist
                               where item.invoicId == dicIncoiceList.ElementAt(pagecount).Key.ToString()
                               select item;

                        foreach (var a in recepiteType1item)
                        {
                            height += 40;
                            e.Graphics.DrawString(a.itemName, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                            e.Graphics.DrawString(String.Format("{0:0.0}", a.total), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                            height += 20;
                            e.Graphics.DrawString(a.itemId + "   " +
                                a.quantity + "@ " +
                                String.Format("{0:0.0}", a.price), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
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
                        e.Graphics.DrawString(String.Format("{0:0.0}", origanalTotal).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("UNPAID", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString("0.0", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("DISCOUNT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", (origanalTotal - total)), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", (total)), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 30;
                        e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 20;
                        e.Graphics.DrawString("CASH TENDERED", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", dicTender.ElementAt(pagecount).Value), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("CHANGE", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", dicTender.ElementAt(pagecount).Value - total), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 30;
                        e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 20;
                        // e.Graphics.DrawString("Tran:155100", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(dicDate.ElementAt(pagecount).Value.ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 100, height);
                        height += 20;
                        e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 30;
                        e.Graphics.DrawString("Thank For Shopping", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        pagecount++;
                    }
                    else if (dicIncoiceList.ElementAt(pagecount).Value == 2)
                    {
                        String address1 = "";
                        String address2 = "";
                        switch (dicShop.ElementAt(pagecount).Value)
                        {
                            case "HK01":
                                address1 = "KOWLOON";
                                address2 = "B102, LAM CHAK STREET";
                                break;
                            case "HK02":
                                address1 = "TSAUN WAN";
                                address2 = "LAM CHAK STREET";
                                break;
                        }
                        double discount = 0;
                        StringFormat sf = new StringFormat();
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Center;
                        StringFormat sf1 = new StringFormat();
                        sf1.LineAlignment = StringAlignment.Center;
                        IEnumerable<invoice> recepiteType2 =
                            from invoice in invoiceitemslist
                            where invoice.invoicId == dicIncoiceList.ElementAt(pagecount).Key.ToString()
                            select invoice;
                        double total = 0;
                        double origanalTotal = 0;
                        double depositTotal = 0;
                        double depositOriganalTotal = 0;
                        foreach (var b in recepiteType2)
                        {
                            if (b.total != "Deposit")
                            {
                                total += ((Convert.ToDouble(b.price) * Convert.ToInt32(b.quantity)) *
                                (1 - Convert.ToDouble(b.discount) / 100));
                                origanalTotal += (Convert.ToDouble(b.price) * Convert.ToInt32(b.quantity));
                            }
                            else
                            {
                                depositTotal += ((Convert.ToDouble(b.price) * Convert.ToInt32(b.quantity)) *
                                (1 - Convert.ToDouble(b.discount) / 100));
                                depositOriganalTotal += (Convert.ToDouble(b.price) * Convert.ToInt32(b.quantity));
                            }
                        }
                        // int stringlength;
                        //  stringlength = (int) Convert.ToDouble(e.Graphics.MeasureString("KOWLOON BAY STORE", new Font("Mircosoft Sans Serif", 20, FontStyle.Bold)).Width.ToString());
                        e.Graphics.DrawString(address1, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 40, sf);
                        e.Graphics.DrawString(address2, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 100, sf);
                        e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                            e.MarginBounds.Left + (e.MarginBounds.Width / 2), 140, sf);
                        e.Graphics.DrawString("STORE: " + dicShop.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 170, sf1);
                        e.Graphics.DrawString("STAFF: " + dicStaff.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, 170, sf);
                        e.Graphics.DrawString("INVOICE NO.: " + dicIncoiceList.ElementAt(pagecount).Key, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 200, sf1);
                        e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                            e.MarginBounds.Left + (e.MarginBounds.Width / 2), 220, sf);
                        var recepiteType2item =
                           from item in invoiceitemslist
                           where item.invoicId == dicIncoiceList.ElementAt(pagecount).Key.ToString()
                           select item;
                        foreach (var a in recepiteType2item)
                        {
                            if (a.total != "Deposit")
                            {
                                height += 40;
                                e.Graphics.DrawString(a.itemId, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                                e.Graphics.DrawString(String.Format("{0:0.0}", a.total), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                                height += 20;
                                e.Graphics.DrawString(a.itemId + "   " +
                                    a.quantity + "@ " +
                                    String.Format("{0:0.0}", a.price), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                            }
                            else
                            {
                                height += 40;
                                e.Graphics.DrawString(a.itemId, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                                e.Graphics.DrawString("(UNPAID)", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 75, height);
                                e.Graphics.DrawString(String.Format("{0:0.0}", (a.quantity) *
                                    (a.price) *
                                    (1 - Convert.ToDouble(a.discount) / 100)),
                                    new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                                height += 20;
                                e.Graphics.DrawString(a.itemId + "   " +
                                    a.quantity + "@ " +
                                    String.Format("{0:0.0}", a.price), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                            }
                        }

                        height += 30;
                        e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 20;
                        e.Graphics.DrawString("PRE-ORDER", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString("No", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("PAID DEPOSIT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", depositTotal - dicDeposit.ElementAt(pagecount).Value).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("ORIGINAL TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", origanalTotal).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("UNPAID", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", dicDeposit.ElementAt(pagecount).Value), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("DISCOUNT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", origanalTotal - total + depositOriganalTotal - depositTotal), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", (total + depositTotal - dicDeposit.ElementAt(pagecount).Value)), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 30;
                        e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 20;
                        e.Graphics.DrawString("CASH TENDERED", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", dicTender.ElementAt(pagecount).Value), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("CHANGE", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", dicTender.ElementAt(pagecount).Value - (total + depositTotal - dicDeposit.ElementAt(pagecount).Value)), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 30;
                        e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 20;
                        // e.Graphics.DrawString("Tran:155100", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(dicDate.ElementAt(pagecount).Value.ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 100, height);
                        height += 20;
                        e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 30;
                        e.Graphics.DrawString("Thank For Shopping", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        pagecount++;
                    }
                    else if (dicIncoiceList.ElementAt(pagecount).Value == 3)
                    {
                        String address1 = "";
                        String address2 = "";
                        switch (dicShop.ElementAt(pagecount).Value)
                        {
                            case "HK01":
                                address1 = "KOWLOON";
                                address2 = "B102, LAM CHAK STREET";
                                break;
                            case "HK02":
                                address1 = "TSAUN WAN";
                                address2 = "LAM CHAK STREET";
                                break;
                        }
                        double total = 0;
                        double origanalTotal = 0;
                        var recepiteType3 =
                           from invoice in invoiceitemslist
                           where invoice.invoicId == dicIncoiceList.ElementAt(pagecount).Key.ToString()
                           group invoice by invoice.invoicId into invoicGroup
                           select new
                           {
                               Total = invoicGroup.Sum(pc => Convert.ToDouble(pc.total)).ToString(),
                               OriganalTotal = invoicGroup.Sum(pc => Convert.ToInt32(pc.quantity) * Convert.ToDouble(pc.price)).ToString(),
                           };
                        foreach (var a in recepiteType3)
                        {
                            total = Convert.ToDouble(a.Total);
                            origanalTotal = Convert.ToDouble(a.OriganalTotal);
                        }
                        StringFormat sf = new StringFormat();
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Center;
                        StringFormat sf1 = new StringFormat();
                        sf1.LineAlignment = StringAlignment.Center;
                        // int stringlength;
                        //  stringlength = (int) Convert.ToDouble(e.Graphics.MeasureString("KOWLOON BAY STORE", new Font("Mircosoft Sans Serif", 20, FontStyle.Bold)).Width.ToString());
                        e.Graphics.DrawString(address1, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 40, sf);
                        e.Graphics.DrawString(address2, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), 100, sf);
                        e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                            e.MarginBounds.Left + (e.MarginBounds.Width / 2), 140, sf);
                        e.Graphics.DrawString("STORE: " + dicShop.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 170, sf1);
                        e.Graphics.DrawString("STAFF: " + dicStaff.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, 170, sf);
                        e.Graphics.DrawString("INVOICE NO.: " + dicIncoiceList.ElementAt(pagecount).Key, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, 200, sf1);
                        e.Graphics.DrawString("--------------------------------------------", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black,
                            e.MarginBounds.Left + (e.MarginBounds.Width / 2), 220, sf);

                        var recepiteType3item =
                           from invoice in invoiceitemslist
                           where invoice.invoicId == dicIncoiceList.ElementAt(pagecount).Key.ToString()
                           select invoice;
                        foreach (var a in recepiteType3item)
                        {
                            height += 40;
                            e.Graphics.DrawString(a.itemName, new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                            e.Graphics.DrawString(String.Format("{0:0.0}", a.total), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                            height += 20;
                            e.Graphics.DrawString(a.itemId + "   " +
                                a.quantity + "@ " +
                                String.Format("{0:0.0}", a.price), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height, sf1);
                        }

                        height += 30;
                        e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 20;
                        e.Graphics.DrawString("PRE-ORDER", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString("Yes", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("PAID DEPOSIT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", total - dicDeposit.ElementAt(pagecount).Value).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("ORIGINAL TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", origanalTotal).ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("UNPAID", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", 0.0), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("DISCOUNT", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", origanalTotal - total), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("TOTAL", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", dicDeposit.ElementAt(pagecount).Value), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 30;
                        e.Graphics.DrawString("===============================================", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 20;
                        e.Graphics.DrawString("CASH TENDERED", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", dicTender.ElementAt(pagecount).Value), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 20;
                        e.Graphics.DrawString("CHANGE", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(String.Format("{0:0.0}", dicTender.ElementAt(pagecount).Value - dicDeposit.ElementAt(pagecount).Value), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right, height);
                        height += 30;
                        e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 20;
                        // e.Graphics.DrawString("Tran:155100", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 80, height);
                        e.Graphics.DrawString(dicDate.ElementAt(pagecount).Value.ToString(), new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 100, height);
                        height += 20;
                        e.Graphics.DrawString("---------------------------------------------------------------------------------", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        height += 30;
                        e.Graphics.DrawString("Thank For Shopping", new Font("Mircosoft Sans Serif", 10, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), height, sf);
                        pagecount++;
                    }
                    int itemsCount = 0;
                    var count1 =
                       from invoice in invoiceitemslist
                       where invoice.invoicId == dicIncoiceList.ElementAt(pagecount).Key
                       group invoice by invoice.invoicId into invoicGroup
                       select new
                       {
                           itemCount = invoicGroup.Count(),
                       };
                    foreach (var a in count1)
                    {
                        itemsCount = a.itemCount;
                    }
                    page++;
                    e.PageSettings.PaperSize = new System.Drawing.Printing.PaperSize("customer", 427, 560 + 60 * itemsCount);
                    e.HasMorePages = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("The date have no receipt");
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.AccountHomePage();
        }
    }
}
