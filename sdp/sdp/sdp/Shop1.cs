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
    public partial class Shop1 : Form
    {
        public String shop;
        public static Shop1 instance;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        Dictionary<string, string> alarm = new Dictionary<string, string>();
        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;


        public String selectRow;
        private string language;
        private string JSONlist;

        public Shop1()
        {
            InitializeComponent();
            instance = this;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            setPermission(Main.instance.dept, Main.instance.position);
            changeLanguage();
            upLoadData();
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
                    case 9:
                        butAddItem.Visible = true;
                        break;
                    case 10:
                        btnCheckout.Visible = true;
                        break;
                    case 8:
                        butEdit.Visible = true;
                        break;
                    case 7:
                        butDetail.Visible = true;
                        break;
                    case 69:
                        butSaleRecord.Visible = true;
                        break;
                    case 6:
                        btnAddReorder.Visible = true;
                        break;
                    case 5:
                        button1.Visible = true;
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
                btnCheckout.Text = "结账";
                butAddItem.Text = "新增货品";
                btnBack.Text = "返回";
                butEdit.Text = "编辑货品";
                butDetail.Text = "货品详情";
                butSaleRecord.Text = "销售记录";
                btnAddReorder.Text = "补货";
                button1.Text = "结算表";
            }
            else
            {
                btnCheckout.Text = "Checkout";
                butAddItem.Text = "Add Item";
                btnBack.Text = "BACK";
                butEdit.Text = "Edit item";
                butDetail.Text = "Item Details";
                butSaleRecord.Text = "Sale Record";
                btnAddReorder.Text = "Add Reorder";
                button1.Text = "Statement";
            }
        }

        private void upLoadData()
        {
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            //           MessageBox.Show(rootconn.getString());
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            if (language == "Chinese")
            {
                sqlCmd.CommandText = "SELECT store_stock.store_stock_id as '店铺货品号' " +
                ", store_stock.item_id as '货品号' " +
                ", inventory.item_name as '货品名称' " +
                ", store_stock.store_stock_qty as '数量' " +
                ", inventory.item_sales_price as '金额(HKD)' " +
                "FROM store_stock " +
                "INNER JOIN inventory " +
                "on inventory.item_id = store_stock.item_id " +
                "where store_id = '" + shop + "'";
            }
            else
            {
                sqlCmd.CommandText = "SELECT store_stock.store_stock_id as 'Store item Id' " +
                ", store_stock.item_id as 'Item Id' " +
                ", inventory.item_name as 'Item name' " +
                ", store_stock.store_stock_qty as 'Quantity' " +
                ", inventory.item_sales_price as 'Price(HKD)' " +
                "FROM store_stock " +
                "INNER JOIN inventory " +
                "on inventory.item_id = store_stock.item_id " +
                "where store_id = '" + shop + "'";
            }
            /*   try
               {
                   MessageBox.Show(sqlCmd.ExecuteReader().ToString());
               }
               catch(Exception e)
               {
                   String message;
                   message = "SELECT command denied to user 'sammy'@'localhost' for table 'store_stock'";
                   if(message == e.Message)
                       MessageBox.Show("fdhdsfhg");
               }
               finally
               {
                   sqlConn.Close();
               } */
            sqlRd = sqlCmd.ExecuteReader();
            sqlDt.Load(sqlRd);
            sqlRd.Close();
            sqlConn.Close();
            dataGridView1.DataSource = sqlDt;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            Int32 selectedRowCount =
                dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                selectRow = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                MessageBox.Show(selectRow);
            }
        }

        private void butEdit_Click_1(object sender, EventArgs e)
        {
            Int32 selectedRowCount =
                dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                selectRow = (dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                Main.instance.Edititem(shop, selectRow);
            }
            /*
            edititem edit = new edititem();
            edit.setSelectRow(selectRow);
            this.Hide();
            edit.ShowDialog();
            this.Close();*/
            }
        /*
                public String getSelectRow()
                {
                    return selectRow;
                }

                public void setSelectRow(String selectRow)
                {
                    this.selectRow = selectRow;
                }
        */
        private void butAddItem_Click(object sender, EventArgs e)
        {
            Main.instance.Additem();
        }

        private void butDetail_Click(object sender, EventArgs e)
        {
            Int32 selectedRowCount =
                dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                selectRow = (dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                Main.instance.ItemDetail(shop, selectRow);
            }
            /*
            itemDetail detail = new itemDetail();
            detail.setSelectRow(selectRow);
            this.Hide();
            detail.ShowDialog();
            this.Close();*/
        }

        private void butSaleRecord_Click(object sender, EventArgs e)
        {
            Console.WriteLine(shop);
            Main.instance.SaleRecord(shop);/*
            saleRecord record = new saleRecord();
            this.Hide();
            record.ShowDialog();
            this.Close();*/
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.saleManagement();
        }

        public void setShop(String shop)
        {
            this.shop = shop;
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            Main.instance.checkout();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            alarm.Clear();
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select store_stock_id, stock_alarm from store_stock where store_id = '" + shop+"'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                alarm.Add(sqlRd.GetString(0), sqlRd.GetString(1));
            }
            sqlRd.Close();
            sqlConn.Close();
            int count = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                int qtyEntered = Convert.ToInt32(row.Cells[3].Value);
                if (Convert.ToInt32(alarm[row.Cells[0].Value.ToString()]) >= Convert.ToInt32(row.Cells[3].Value))
                {
                    if (qtyEntered == 0)
                    {
                        dataGridView1[3, count].Style.ForeColor = Color.Red;//to color the row
                    }
                    else
                    {
                        dataGridView1[3, count].Style.ForeColor = Color.Orange;//to color the row
                    }
                }
                count++;
            }
        }

        private void btnAddReorder_Click(object sender, EventArgs e)
        {
            Main.instance.AddReorder(shop);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Main.instance.Statement(shop);
        }
    }
}
