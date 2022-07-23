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
    public partial class saleRecord : Form
    {
        String selectaction = "all";
        String search;
        String preOrder;
        public static saleRecord instance;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;

        private String shop;
        public String saleDate;
        public String selectRow;
        public String saleSelect;
        public String storeid;
        public String date;
        public String preorder;
        public String action;

        public Dictionary<String, int> receiptType = new Dictionary<String, int>();
        private string language;

        public saleRecord()
        {
            InitializeComponent();
            instance = this;
        }

        private void upLoadData(String search)
        {
            sqlConn.ConnectionString = ConnectString.ConnectionString;
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "SELECT distinct sales_receipt.invoice_id as 'Sale Invoice Id' " +
            ", sales_order.store_id as 'Store Id' " +
            ", sales_order.last_update as 'Date' " +
            ", sales_order.order_ttl_amount as 'Total Price(HKD)' " +
            ", IF(sales_order.order_is_preorder = 'n', 'No', 'Yes') as 'Pre-order' " +
            "FROM sales_order " +
            "INNER JOIN sales_receipt " +
            "on sales_order.order_id = sales_receipt.order_id " +
            "where sales_receipt.invoice_id like '" + search + "%'" +
            "and staff_id = '" + Main.instance.Staff + "'";
            sqlRd = sqlCmd.ExecuteReader();
            sqlDt.Clear();
            /*      while (sqlRd.Read())
                  {
                      if(sqlRd.GetString(4) == "n")
                      {
                          preOrder = "No";
                      }
                      else
                      {
                          preOrder = "Yes";
                      }
                      sqlDt.Rows.Add(sqlRd.GetString(0), sqlRd.GetString(1), sqlRd.GetString(2), sqlRd.GetString(3), preOrder);
                  }*/
            sqlDt.Load(sqlRd);
            sqlRd.Close();
            sqlConn.Close();
            dataGridView1.DataSource = sqlDt;
        }


        private void saleRecord_Load(object sender, EventArgs e)
        {
            changeLanguage();
            DataGridViewTextBoxColumn Col1 = new DataGridViewTextBoxColumn();
            Col1.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Col1.Width = 160;
            DataGridViewTextBoxColumn Col2 = new DataGridViewTextBoxColumn();
            Col2.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Col2.Width = 160;
            DataGridViewTextBoxColumn Col3 = new DataGridViewTextBoxColumn();
            Col3.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Col3.Width = 260;
            DataGridViewTextBoxColumn Col4 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn Col5 = new DataGridViewTextBoxColumn();
            if (language == "Chinese")
            {
                Col1.HeaderText = "发票号";
                Col2.HeaderText = "店铺号";
                Col3.HeaderText = "销售日期";
                Col4.HeaderText = "总共金额";
                Col5.HeaderText = "发票类型";
            }
            else
            {
                Col1.HeaderText = "Invoice Id";
                Col2.HeaderText = "Store Id";
                Col3.HeaderText = "Sale Date";
                Col4.HeaderText = "Sale Total Price";
                Col5.HeaderText = "Invoice Type";
            }
            dataGridView1.Columns.AddRange(new[] { Col1, Col2, Col3, Col4, Col5});
            /* "select invoice_id, store_id, sales_datetime, sum(order_item_final_amount) - order_deposit +  ( " +
                 "select sum(order_item_final_amount) " +
                 "from sales_receipt " +
                 "inner join sales_order " +
                 "on sales_order.order_id = sales_receipt.order_id " +
                 "inner join order_item " +
                 "on sales_order.order_id = order_item.order_id " +
                 "where invoice_id = '" + invoice + "' " +
                 "group by is_deposit " +
                 "having is_deposit = 'n') as 'Total Price',  receipt_type " +
                 "from sales_receipt " +
                 "inner join sales_order " +
                 "on sales_order.order_id = sales_receipt.order_id " +
                 "inner join order_item " +
                 "on sales_order.order_id = order_item.order_id " +
                 "where invoice_id = '" + invoice + "' " +
                 "group by is_deposit " +
                 "having is_deposit = 'y'";*/
            //  upLoadData("");
            selectaction = "all";
            receiptDictioncary("","","");
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
                label4.Text = "搜寻发票号 :";
                butBack.Text = "BACK";
                btnAll.Text = "查看所有记录";
                butStore.Text = "选择商店";
                butDate.Text = "选择日期";
                butDetail.Text = "详情";
            }
            else
            {
                label4.Text = "Search Invoice Id :";
                butBack.Text = "BACK";
                btnAll.Text = "View All Order";
                butStore.Text = "Select Store";
                butDate.Text = "Select Date";
                butDetail.Text = "Details";
            }
        }

        public void receiptDictioncary(String date, String store, String search)
        {
            receiptType.Clear();
            dataGridView1.Rows.Clear();
            sqlConn.ConnectionString = ConnectString.ConnectionString;
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            if (Main.instance.position == 2 || Main.instance.position == 99 || Main.instance.dept == 99)
            {
                sqlCmd.CommandText = "select invoice_id, receipt_type " +
                "from sales_receipt " +
                "inner join sales_order " +
                "on sales_order.order_id = sales_receipt.order_id " +
                "where sales_datetime like '%" + date + "%' " +
                "and store_id like '%" + store + "%' " +
                "and invoice_id like '" + search + "%' " +
                "and order_is_cancelled = 'n'";
            }
            else
            {
                sqlCmd.CommandText = "select invoice_id, receipt_type " +
                "from sales_receipt " +
                "inner join sales_order " +
                "on sales_order.order_id = sales_receipt.order_id " +
                "where sales_datetime like '%" + date + "%' " +
                "and store_id like '%" + store + "%' " +
                "and invoice_id like '" + search + "%' " +
                "and sales_receipt.staff_id = '" + Main.instance.Staff + "' " +
                "and order_is_cancelled = 'n'";
            }
            Console.WriteLine(sqlCmd.CommandText);
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                receiptType.Add(sqlRd.GetString(0), sqlRd.GetInt32(1));
            }
            sqlRd.Close();
            sqlConn.Close();
            foreach(var a in receiptType)
            {
                if(a.Value == 1)
                {
                    insertType1(a.Key);
                }
                else if(a.Value == 2)
                {
                    insertType2(a.Key);
                }
                else if(a.Value == 3)
                {
                    insertType3(a.Key);
                }
            }
        }

        public void insertType1(String invoice)
        {
            sqlConn.ConnectionString = ConnectString.ConnectionString;
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select invoice_id, store_id, DATE_FORMAT(sales_datetime,'%Y-%m-%d %H:%i:%S'), order_ttl_amount " +
                "from sales_receipt " +
                "inner join sales_order " +
                "on sales_order.order_id = sales_receipt.order_id " +
                "where invoice_id = '"+invoice+"'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                if (language == "Chinese")
                {
                    dataGridView1.Rows.Add(sqlRd.GetString(0), sqlRd.GetString(1), sqlRd.GetString(2), sqlRd.GetString(3), "正常收据");

                }
                else
                {
                    dataGridView1.Rows.Add(sqlRd.GetString(0), sqlRd.GetString(1), sqlRd.GetString(2), sqlRd.GetString(3), "Non-Deposit Receipt");

                }
            }
            sqlRd.Close();
            sqlConn.Close();
        }

        public void insertType2(String invoice)
        {
            sqlConn.ConnectionString = ConnectString.ConnectionString;
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select invoice_id, store_id, DATE_FORMAT(sales_datetime,'%Y-%m-%d %H:%i:%S'), sum(order_item_final_amount) - order_deposit +  COALESCE(( " +
                 "select sum(order_item_final_amount) " +
                 "from sales_receipt " +
                 "inner join sales_order " +
                 "on sales_order.order_id = sales_receipt.order_id " +
                 "inner join order_item " +
                 "on sales_order.order_id = order_item.order_id " +
                 "where invoice_id = '" + invoice + "' " +
                 "group by is_deposit " +
                 "having is_deposit = 'n' ), 0) as 'Total Price',  receipt_type " +
                 "from sales_receipt " +
                 "inner join sales_order " +
                 "on sales_order.order_id = sales_receipt.order_id " +
                 "inner join order_item " +
                 "on sales_order.order_id = order_item.order_id " +
                 "where invoice_id = '" + invoice + "' " +
                 "group by is_deposit " +
                 "having is_deposit = 'y'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                if (language == "Chinese")
                {
                    dataGridView1.Rows.Add(sqlRd.GetString(0), sqlRd.GetString(1), sqlRd.GetString(2), sqlRd.GetString(3), "非完成订单");

                }
                else
                {
                    dataGridView1.Rows.Add(sqlRd.GetString(0), sqlRd.GetString(1), sqlRd.GetString(2), sqlRd.GetString(3), "Deposit Receipt");

                }
            }
            sqlRd.Close();
            sqlConn.Close();
        }

        public void insertType3(String invoice)
        {
            sqlConn.ConnectionString = ConnectString.ConnectionString;
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select invoice_id, store_id, DATE_FORMAT(sales_datetime,'%Y-%m-%d %H:%i:%S'), order_deposit " +
                "from sales_receipt " +
                "inner join sales_order " +
                "on sales_order.order_id = sales_receipt.order_id " +
                "where invoice_id = '" + invoice + "'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                if (language == "Chinese")
                {
                    dataGridView1.Rows.Add(sqlRd.GetString(0), sqlRd.GetString(1), sqlRd.GetString(2), sqlRd.GetString(3), "已完成订单");

                }
                else
                {
                    dataGridView1.Rows.Add(sqlRd.GetString(0), sqlRd.GetString(1), sqlRd.GetString(2), sqlRd.GetString(3), "Completed Deposit Receipt");

                }
            }
            sqlRd.Close();
            sqlConn.Close();
        }

        private void butDate_Click(object sender, EventArgs e)
        {
            selectDate date = new selectDate();
            date.ShowDialog();
            saleDate = date.getDate();
            txtInvoiceId.Clear();
            receiptDictioncary(saleDate, "","");
           // updateDate("", saleDate);
        }

        public void updateDate(String search, String Date)
        {
            selectaction = "date";
            sqlConn.ConnectionString = ConnectString.ConnectionString;
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "SELECT distinct sales_receipt.invoice_id as 'Sale Invoice Id' " +
            ", sales_order.store_id as 'Store Id' " +
            ", sales_order.last_update as 'Date' " +
            ", sales_order.order_ttl_amount as 'Total Price(HKD)' " +
            ", IF(sales_order.order_is_preorder = 'n', 'No', 'Yes') as 'Pre-order' " +
            "FROM sales_order " +
            "INNER JOIN sales_receipt " +
            "on sales_order.order_id = sales_receipt.order_id " +
            "WHERE last_update >= '" + Date + " 0:0:0' and last_update <= '" + Date + " 23:59:59' and " +
            "sales_receipt.invoice_id like '" + search + "%'";
            sqlDt.Clear();
            sqlRd = sqlCmd.ExecuteReader();
            sqlDt.Load(sqlRd);
            sqlRd.Close();
            sqlConn.Close();
            dataGridView1.DataSource = sqlDt;
        }

        public void setShop(String shop)
        {
            this.shop = shop;
        }

        private void butStore_Click(object sender, EventArgs e)
        {
            selectaction = "store";
            selectShop Store = new selectShop();
            Store.ShowDialog();
            txtInvoiceId.Clear();
            saleSelect = Store.getSelect();
            receiptDictioncary("", saleSelect,"");
            // updateStore("", saleSelect);
        }

        public void updateStore(String search, String Store)
        {
            sqlConn.ConnectionString = ConnectString.ConnectionString;
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "SELECT distinct sales_receipt.invoice_id as 'Sale Invoice Id' " +
            ", sales_order.store_id as 'Store Id' " +
            ", sales_order.last_update as 'Date' " +
            ", sales_order.order_ttl_amount as 'Total Price(HKD)' " +
            ", IF(sales_order.order_is_preorder = 'n', 'No', 'Yes') as 'Pre-order' " +
            "FROM sales_order " +
            "INNER JOIN sales_receipt " +
            "on sales_order.order_id = sales_receipt.order_id " +
            "WHERE sales_order.store_id = '" + Store + "' and sales_receipt.invoice_id like '" + search + "%'";
            sqlDt.Clear();
            sqlRd = sqlCmd.ExecuteReader();
            sqlDt.Load(sqlRd);
            sqlRd.Close();
            sqlConn.Close();
            dataGridView1.DataSource = sqlDt;
        }


        private void butBack_Click(object sender, EventArgs e)
        {
            Main.instance.shop1(shop);/*
            Shop1 back = new Shop1();
            this.Hide();
            back.ShowDialog();
            this.Close();*/
        }

        private void butDetail_Click(object sender, EventArgs e)
        {
            Int32 selectedRowCount =
            dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                selectRow = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                storeid = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                date = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                preorder = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
                Main.instance.SaleRecordDetail("Sale", shop);
            }
            /*          saleRecordDetail detail = new saleRecordDetail();
                      detail.setOrderid(selectRow);
                      detail.setStoreid(storeid);
                      detail.setDate(date);
                      detail.setPreorder(preorder);
                      this.Hide();
                      detail.ShowDialog();
                      this.Close();*/
        }

        public String getSelectRow()
        {
            return selectRow;
        }

        public void setSelectRow(String selectRow)
        {
            this.selectRow = selectRow;
        }

        private void txtItemID_TextChanged(object sender, EventArgs e)
        {
            switch(selectaction){
                case "all":
                    receiptDictioncary("", "", txtInvoiceId.Text);
                    break;
                case "store":
                    receiptDictioncary("", saleSelect, txtInvoiceId.Text);
                    break;
                case "date":
                    receiptDictioncary(saleDate,"",txtInvoiceId.Text);
                    break;
            }
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            selectaction = "all";
            txtInvoiceId.Clear();
         //     upLoadData("");
            receiptDictioncary("", "","");
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
        }
    }
}
