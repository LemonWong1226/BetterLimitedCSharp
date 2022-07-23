using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace sdp
{
    public partial class PreOrderCheckout : Form
    {
        public static PreOrderCheckout instance;
        private String cashAction;
        private String staffName;
        private int pageHeight;
        private String invoiceId;
        private Double grandTotal = 0;
        private Double discount;
        private Double paidDeposit;
        private Double total;
        private String shop;
        private String remark;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;

        public PreOrderCheckout()
        {
            InitializeComponent();
            instance = this;
        }

        private void PreOrderCheckout_Load(object sender, EventArgs e)
        {
            cbxCash.Checked = true;
            DataGridViewTextBoxColumn Col1 = new DataGridViewTextBoxColumn();
            Col1.HeaderText = "item_id";
            Col1.ReadOnly = true;
            DataGridViewTextBoxColumn Col2 = new DataGridViewTextBoxColumn();
            Col2.HeaderText = "item_name";
            Col2.Width = 150;
            Col2.ReadOnly = true;
            DataGridViewTextBoxColumn Col3 = new DataGridViewTextBoxColumn();
            Col3.HeaderText = "Quantity";
            Col3.Width = 10;
            Col3.ValueType = typeof(int);
            DataGridViewTextBoxColumn Col4 = new DataGridViewTextBoxColumn();
            Col4.HeaderText = "Price";
            Col4.ValueType = typeof(double);
            DataGridViewTextBoxColumn Col5 = new DataGridViewTextBoxColumn();
            Col5.HeaderText = "Discount(%)";
            Col5.Width = 100;
            Col5.ValueType = typeof(double);
            DataGridViewTextBoxColumn Col6 = new DataGridViewTextBoxColumn();
            Col6.HeaderText = "Total";
            Col6.ReadOnly = true;
            Col6.ValueType = typeof(String);
            dataGridView1.Columns.AddRange(new[] { Col1, Col2, Col3, Col4, Col5, Col6 });
            generateDate();
            getPaidDeposit();
            updateLable();
        }

        public void generateDate()
        {
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            String[] order = new string[6];
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select order_item.item_id, item_name, order_item_qty, sale_price," +
                " cast(((order_item_final_amount - sale_price * order_item_qty) * -100 / sale_price / order_item_qty) as DECIMAL) as 'Discount'" +
                ", order_item_final_amount " +
                "from order_item " +
                "inner join inventory" +
                " on inventory.item_id = order_item.item_id" +
                " inner join sales_receipt " +
                "on sales_receipt.order_id = order_item.order_id" +
                " where sales_receipt.invoice_id = '" + invoiceId + "' and is_deposit = 'y'";
            Console.WriteLine(sqlCmd.CommandText);
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                for (int i = 0; i < 6; i++)
                {
                    order[i] = sqlRd.GetString(i);

                }
                grandTotal += Convert.ToDouble(order[2]) * Convert.ToDouble(order[3]);
                discount += Convert.ToDouble(order[3]) * Convert.ToInt32(order[2]) - Convert.ToDouble(order[5]);
                dataGridView1.Rows.Add(order[0], order[1], order[2], order[3], order[4], order[5]);
            }
            sqlRd.Close();
            sqlConn.Close();
        }

        public void getPaidDeposit()
        {
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            String[] order = new string[6];
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select order_deposit , store_id, order_remarks " +
                "from sales_order " +
                "natural join sales_receipt " +
                "where invoice_id = '" + invoiceId + "'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                paidDeposit = grandTotal - discount - sqlRd.GetDouble(0);
                shop = sqlRd.GetString(1);
                remark = sqlRd.GetString(2);
            }
            sqlRd.Close();
            sqlCmd.CommandText = "select first_name, last_name from user inner join staff " +
                "on staff.user_id = user.user_id where staff_id = '"+Main.instance.Staff+"'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                staffName = sqlRd.GetString(0) + " " + sqlRd.GetString(1);
            }
            sqlRd.Close();
            sqlConn.Close();
            total = grandTotal - paidDeposit - discount;
        }

        public void updateLable()
        {
            lblGrandTotal.Text = String.Format("{0:0.#}", grandTotal);
            lblDiscount.Text = String.Format("{0:0.#}", discount);
            lblPaidDeposit.Text = String.Format("{0:0.#}", paidDeposit);
            lblTotal.Text = String.Format("{0:0.#}", total);
            lblShop.Text = shop;
            lblInvoiceId.Text = invoiceId;
            txtRemark.Text = remark;
        }

        public void setInvoice(String invoiceId)
        {
            this.invoiceId = invoiceId;
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            pageHeight = 560 + 60 * dataGridView1.Rows.Count;
            String[,] order = new String[dataGridView1.Rows.Count, 6];
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    order[i, j] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                }
            }
            Cash cash = new Cash();
            cash.setStaffName(staffName);
            cash.setDiscount(discount);
            cash.setOriginalTotal(grandTotal);
            cash.setPaidDeposti(paidDeposit);
            cash.setTotal(total);
            cash.setOrder(order);
            cash.setStoreId(lblShop.Text);
            cash.setPageHeight(pageHeight);
            cash.setInvoiceId(Convert.ToInt32(invoiceId));
            cash.setPrice(Convert.ToDouble(lblTotal.Text));
            cash.setAction("preorder");
            cash.ShowDialog();
            Console.WriteLine(cashAction);
            if (cashAction == "cancell" || cashAction == "")
            {
                //
            }
            else if(cashAction == "confirm")
            {
                Main.instance.checkout();
            }
        }

        public void setInvoiceId(String invoiceId)
        {
            this.invoiceId = invoiceId;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.checkout();
        }

        public void setCashAction(String cashAction)
        {
            this.cashAction = cashAction;
        }
    }
}
