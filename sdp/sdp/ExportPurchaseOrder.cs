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
    public partial class ExportPurchaseOrder : Form
    {
        private int count = 0;

        private int page = 0;
        private int pagecount;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;

        private List<order> orderitemslist = new List<order>();
        private Dictionary<String, String> supplier_id = new Dictionary<String, String>();
        private Dictionary<String, String> supplier_name = new Dictionary<String, String>();
        private Dictionary<String, String> supplier_email = new Dictionary<String, String>();
        private Dictionary<String, String> supplier_phone = new Dictionary<String, String>();
        private Dictionary<String, String> purchase_ttl_amount = new Dictionary<String, String>();
        private Dictionary<String, String> purchase_remarks = new Dictionary<String, String>();
        private Dictionary<String, String> supplier_address = new Dictionary<String, String>();
        private string language;

        public class order
        {
            public string orderid { get; set; }
            public string itemId { get; set; }
            public string description { get; set; }
            public double unit { get; set; }
            public int qty { get; set; }
            public double price { get; set; }
        }

        public ExportPurchaseOrder()
        {
            InitializeComponent();
        }

        private void ExportPurchaseOrder_Load(object sender, EventArgs e)
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
                cbxinvoice.Text = "一单输出";
                btnSubmit.Text = "输出";
                btnBack.Text = "返回";
            }
            else
            {
                label1.Text = "Perchase Order ID: ";
                label2.Text = "Date:";
                label4.Text = "To";
                btnSubmit.Text = "Export";
                btnBack.Text = "back";
                cbxinvoice.Text = "Export for one order";
            }
        }

        public void updateDictionary(String date1, String date2, String action, String orderid)
        {
            if (action == "more")
            {
                count = 0;
                orderitemslist.Clear();
                supplier_id.Clear();
                supplier_name.Clear();
                supplier_email.Clear();
                supplier_phone.Clear();
                purchase_ttl_amount.Clear();
                purchase_remarks.Clear();
                supplier_address.Clear();
                sqlConn.ConnectionString = ConnectString.ConnectionString;

                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select distinct purchase_order_id, inventory.supplier_id, supplier_name," +
                " supplier_email, supplier_phone , purchase_ttl_amount, purchase_remarks, supplier_address " +
                "from purchase_order " +
                "inner join purchase_request " +
                "on purchase_request.purchase_request_id = purchase_order.purchase_request_id " +
                "inner join purchase_item " +
                "on purchase_request.purchase_request_id = purchase_item.purchase_request_id " +
                "inner join inventory " +
                "on inventory.item_id = purchase_item.item_id " +
                "inner join supplier " +
                "on inventory.supplier_id = supplier.supplier_id " +
                "WHERE purchase_order.purchase_date >= '" + date1 + " 0:0:0' and purchase_order.purchase_date <= '" + date2 + " 23:59:59' " +
                "order by purchase_order.purchase_date";

                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    supplier_id.Add(sqlRd.GetString(0), sqlRd.GetString(1));
                    supplier_name.Add(sqlRd.GetString(0), sqlRd.GetString(2));
                    supplier_email.Add(sqlRd.GetString(0), sqlRd.GetString(3));
                    supplier_phone.Add(sqlRd.GetString(0), sqlRd.GetString(4));
                    purchase_ttl_amount.Add(sqlRd.GetString(0), sqlRd.GetString(5));
                    purchase_remarks.Add(sqlRd.GetString(0), sqlRd.GetString(6));
                    supplier_address.Add(sqlRd.GetString(0), sqlRd.GetString(7));
                }
                sqlRd.Close();
                sqlConn.Close();
                foreach (var order in supplier_id)
                {
                    handlePage(order.Key);
                }
            }
            else if (action == "one")
            {
                count = 0;
                orderitemslist.Clear();
                supplier_id.Clear();
                supplier_name.Clear();
                supplier_email.Clear();
                supplier_phone.Clear();
                purchase_ttl_amount.Clear();
                purchase_remarks.Clear();
                supplier_address.Clear();
                sqlConn.ConnectionString = ConnectString.ConnectionString;

                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select distinct purchase_order_id, inventory.supplier_id, supplier_name," +
                " supplier_email, supplier_phone , purchase_ttl_amount, purchase_remarks, supplier_address " +
                "from purchase_order " +
                "inner join purchase_request " +
                "on purchase_request.purchase_request_id = purchase_order.purchase_request_id " +
                "inner join purchase_item " +
                "on purchase_request.purchase_request_id = purchase_item.purchase_request_id " +
                "inner join inventory " +
                "on inventory.item_id = purchase_item.item_id " +
                "inner join supplier " +
                "on inventory.supplier_id = supplier.supplier_id " +
                "where purchase_order.purchase_order_id = '" + orderid + "'";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    supplier_id.Add(sqlRd.GetString(0), sqlRd.GetString(1));
                    supplier_name.Add(sqlRd.GetString(0), sqlRd.GetString(2));
                    supplier_email.Add(sqlRd.GetString(0), sqlRd.GetString(3));
                    supplier_phone.Add(sqlRd.GetString(0), sqlRd.GetString(4));
                    purchase_ttl_amount.Add(sqlRd.GetString(0), sqlRd.GetString(5));
                    purchase_remarks.Add(sqlRd.GetString(0), sqlRd.GetString(6));
                    supplier_address.Add(sqlRd.GetString(0), sqlRd.GetString(7));
                }
                sqlRd.Close();
                sqlConn.Close();
                foreach (var order in supplier_id)
                {
                    handlePage(order.Key);
                }
            }
        }


        public void handlePage(String orderid)
        {
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select purchase_order_id, purchase_item.item_id,item_name,p_unix_cost,p_item_qty,p_item_ttl_amount " +
                    "from purchase_order " +
                    "inner join purchase_item " +
                    "on purchase_item.purchase_request_id = purchase_order.purchase_request_id " +
                    "inner join inventory " +
                    "on inventory.item_id = purchase_item.item_id " +
                    "where purchase_order_id = " + orderid;
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                orderitemslist.Add(new order
                {
                    orderid = orderid,
                    itemId = sqlRd.GetString(1),
                    description = sqlRd.GetString(2),
                    unit = sqlRd.GetDouble(3),
                    qty = sqlRd.GetInt32(4),
                    price = sqlRd.GetDouble(5)
                }); ;
            }
            count++;
            sqlRd.Close();
            sqlConn.Close();

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (cbxinvoice.Checked == false)
            {
                pagecount = 0;
                updateDictionary(dtp1.Text, dtp2.Text, "more", "");
                printPreviewDialog1.Document = printDocument1;
                printPreviewDialog1.ShowDialog();
            }
            else
            {
                updateDictionary(dtp1.Text, dtp2.Text, "one", txtInvoiceId.Text);
                printPreviewDialog1.Document = printDocument1;
                printPreviewDialog1.ShowDialog();
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                e.HasMorePages = false;
                if (pagecount == supplier_id.Count - 1)
                {
                    int itemsCount = 0;
                    int i = 1;
                    var count1 =
                       from order in orderitemslist
                       where order.orderid == supplier_id.ElementAt(pagecount).Key
                       group order by order.orderid into invoicGroup
                       select new
                       {
                           itemCount = invoicGroup.Count(),
                       };
                    foreach (var a in count1)
                    {
                        itemsCount = a.itemCount;
                    }
                    int totalItem = itemsCount;
                    SolidBrush solidBrush = new SolidBrush(
                    Color.DarkGray);
                    Pen blackPen = new Pen(Color.Black, 1);

                    e.Graphics.DrawRectangle(blackPen, 30, 320, 400, 140);

                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;
                    StringFormat sf1 = new StringFormat();
                    sf1.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString("Better Limited", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 70, 40);
                    e.Graphics.DrawString("Unit 1301,kowloon Bay pazzy 022", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 70);
                    e.Graphics.DrawString("address address address address", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 90);
                    e.Graphics.DrawString("Tel: +852 1234 5678    Fax: +852 1234 5678", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 110);
                    e.Graphics.DrawString("Purchase Order: " + supplier_id.ElementAt(pagecount).Key, new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Right - 250, 150);
                    e.Graphics.DrawString("______________________________________________", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 180);
                    e.Graphics.DrawString("__________", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Right - 30, 180);
                    e.Graphics.DrawString("Purchase Order", new Font("Mircosoft Sans Serif", 20, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 250, 180);
                    e.Graphics.DrawString("Supplier Information", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 295);
                    e.Graphics.DrawString(supplier_name.ElementAt(pagecount).Value + "           +852 " + supplier_phone.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 330);
                    e.Graphics.DrawString(supplier_address.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 370);
                    e.Graphics.DrawString("Email : " + supplier_email.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 425);

                    e.Graphics.DrawRectangle(blackPen, 30, 490, 757, 65);
                    e.Graphics.FillRectangle(solidBrush, 30, 490, 150, 65);
                    e.Graphics.DrawRectangle(blackPen, 30, 490, 150, 65);

                    e.Graphics.DrawString("Remarks", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 495);
                    e.Graphics.DrawString(purchase_remarks.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 190, 495);
                    //      e.Graphics.DrawString("Remarks Test", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 190, 525);

                    e.Graphics.FillRectangle(solidBrush, 30, 580, e.MarginBounds.Right + 30, 34);
                    e.Graphics.DrawRectangle(blackPen, 30, 580, e.MarginBounds.Right + 30, 34);
                    e.Graphics.DrawRectangle(blackPen, 30, 580, 180, 34);
                    e.Graphics.DrawRectangle(blackPen, 30, 580, 180, 40 + 34 * totalItem);
                    e.Graphics.DrawRectangle(blackPen, 680, 580, 107, 40 + 34 * totalItem);
                    e.Graphics.DrawRectangle(blackPen, 30, 580, 757, 40 + 34 * totalItem);
                    e.Graphics.DrawRectangle(blackPen, 30, 580, 470, 40 + 34 * totalItem);
                    e.Graphics.DrawRectangle(blackPen, 30, 580, 555, 40 + 34 * totalItem);

                    e.Graphics.DrawString("Item Id", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 585);
                    e.Graphics.DrawString("Description", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 212, 585);
                    e.Graphics.DrawString("Price (HKD)", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 682, 585);
                    e.Graphics.DrawString("Quantity", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 504, 585);
                    e.Graphics.DrawString("Unix Price", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 589, 585);

                    var items =
                       from item in orderitemslist
                       where item.orderid == supplier_id.ElementAt(pagecount).Key.ToString()
                       select item;
                    foreach (var a in items)
                    {
                        e.Graphics.DrawString(a.itemId, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 585 + i * 35);
                        e.Graphics.DrawString(a.description, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 212, 585 + i * 35);
                        e.Graphics.DrawString(a.price.ToString(), new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 682, 585 + i * 35);
                        e.Graphics.DrawString(a.qty.ToString(), new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 504, 585 + i * 35);
                        e.Graphics.DrawString(a.unit.ToString(), new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 589, 585 + i * 35);
                        i++;
                    }

                    e.Graphics.DrawRectangle(blackPen, 30, 585 + i * 35 + 25, 200, 40);
                    e.Graphics.DrawRectangle(blackPen, 30, 585 + i * 35 + 25, 757, 40);

                    double purchaseTotal = 0;
                    var total =
                       from invoice in orderitemslist
                       where invoice.orderid == supplier_id.ElementAt(pagecount).Key
                       group invoice by invoice.orderid into invoicGroup
                       select new
                       {
                           Total = invoicGroup.Sum(pc => Convert.ToDouble(pc.price)).ToString(),
                       };
                    foreach (var a in total)
                    {
                        purchaseTotal = Convert.ToDouble(a.Total.ToString());
                    }
                    e.Graphics.DrawString("Total Amount(HKD)", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 585 + i * 35 + 35);
                    e.Graphics.DrawString(purchaseTotal.ToString(), new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 234, 585 + i * 35 + 35);
                }
                else
                {
                    int itemsCount = 0;
                    int i = 1;
                    var count1 =
                       from order in orderitemslist
                       where order.orderid == supplier_id.ElementAt(pagecount).Key
                       group order by order.orderid into invoicGroup
                       select new
                       {
                           itemCount = invoicGroup.Count(),
                       };
                    foreach (var a in count1)
                    {
                        itemsCount = a.itemCount;
                    }
                    int totalItem = itemsCount;
                    SolidBrush solidBrush = new SolidBrush(
                    Color.DarkGray);
                    Pen blackPen = new Pen(Color.Black, 1);

                    e.Graphics.DrawRectangle(blackPen, 30, 320, 400, 140);

                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;
                    StringFormat sf1 = new StringFormat();
                    sf1.LineAlignment = StringAlignment.Center;
                    e.Graphics.DrawString("Better Limited", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 70, 40);
                    e.Graphics.DrawString("Unit 1301,kowloon Bay pazzy 022", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 70);
                    e.Graphics.DrawString("address address address address", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 90);
                    e.Graphics.DrawString("Tel: +852 1234 5678    Fax: +852 1234 5678", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 110);
                    e.Graphics.DrawString("Purchase Order: " + supplier_id.ElementAt(pagecount).Key, new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Right - 250, 150);
                    e.Graphics.DrawString("______________________________________________", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 180);
                    e.Graphics.DrawString("__________", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Right - 30, 180);
                    e.Graphics.DrawString("Purchase Order", new Font("Mircosoft Sans Serif", 20, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 250, 180);
                    e.Graphics.DrawString("Supplier Information", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 295);
                    e.Graphics.DrawString(supplier_name.ElementAt(pagecount).Value + "           +852 " + supplier_phone.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 330);
                    e.Graphics.DrawString(supplier_address.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 370);
                    e.Graphics.DrawString("Email : " + supplier_email.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 425);

                    e.Graphics.DrawRectangle(blackPen, 30, 490, 757, 65);
                    e.Graphics.FillRectangle(solidBrush, 30, 490, 150, 65);
                    e.Graphics.DrawRectangle(blackPen, 30, 490, 150, 65);

                    e.Graphics.DrawString("Remarks", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 495);
                    e.Graphics.DrawString(purchase_remarks.ElementAt(pagecount).Value, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 190, 495);
                    //      e.Graphics.DrawString("Remarks Test", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 190, 525);

                    e.Graphics.FillRectangle(solidBrush, 30, 580, e.MarginBounds.Right + 30, 34);
                    e.Graphics.DrawRectangle(blackPen, 30, 580, e.MarginBounds.Right + 30, 34);
                    e.Graphics.DrawRectangle(blackPen, 30, 580, 180, 34);
                    e.Graphics.DrawRectangle(blackPen, 30, 580, 180, 40 + 34 * totalItem);
                    e.Graphics.DrawRectangle(blackPen, 680, 580, 107, 40 + 34 * totalItem);
                    e.Graphics.DrawRectangle(blackPen, 30, 580, 757, 40 + 34 * totalItem);
                    e.Graphics.DrawRectangle(blackPen, 30, 580, 470, 40 + 34 * totalItem);
                    e.Graphics.DrawRectangle(blackPen, 30, 580, 555, 40 + 34 * totalItem);

                    e.Graphics.DrawString("Item Id", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 585);
                    e.Graphics.DrawString("Description", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 212, 585);
                    e.Graphics.DrawString("Price (HKD)", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 682, 585);
                    e.Graphics.DrawString("Quantity", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 504, 585);
                    e.Graphics.DrawString("Unix Price", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 589, 585);

                    var items =
                       from item in orderitemslist
                       where item.orderid == supplier_id.ElementAt(pagecount).Key.ToString()
                       select item;
                    foreach (var a in items)
                    {
                        e.Graphics.DrawString(a.itemId, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 585 + i * 35);
                        e.Graphics.DrawString(a.description, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 212, 585 + i * 35);
                        e.Graphics.DrawString(a.price.ToString(), new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 682, 585 + i * 35);
                        e.Graphics.DrawString(a.qty.ToString(), new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 504, 585 + i * 35);
                        e.Graphics.DrawString(a.unit.ToString(), new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 589, 585 + i * 35);
                        i++;
                    }

                    e.Graphics.DrawRectangle(blackPen, 30, 585 + i * 35 + 25, 200, 40);
                    e.Graphics.DrawRectangle(blackPen, 30, 585 + i * 35 + 25, 757, 40);

                    double purchaseTotal = 0;
                    var total =
                       from invoice in orderitemslist
                       where invoice.orderid == supplier_id.ElementAt(pagecount).Key
                       group invoice by invoice.orderid into invoicGroup
                       select new
                       {
                           Total = invoicGroup.Sum(pc => Convert.ToDouble(pc.price)).ToString(),
                       };
                    foreach (var a in total)
                    {
                        purchaseTotal = Convert.ToDouble(a.Total.ToString());
                    }
                    e.Graphics.DrawString("Total Amount(HKD)", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 585 + i * 35 + 35);
                    e.Graphics.DrawString(purchaseTotal.ToString(), new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 234, 585 + i * 35 + 35);
                    e.HasMorePages = true;
                    pagecount++;
                }
            }
            catch
            {
                MessageBox.Show("Selected date have no receipt");
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.AccountHomePage();
        }
    }
}
