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
    public partial class itemDetail : Form
    {
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;

        private String storeId;
        public String storeItemId;
        public String itemId;
        public String itemName;
        public String price;
        public String quantity;
        public String alarm;
        public String supplierName;
        public String supplierEmail;
        public String remark;

        private String selectRow;
        private string language;

        public itemDetail()
        {
            InitializeComponent();
        }

        private void ItemDetail_Load(object sender, EventArgs e)
        {
            changeLanguage();
            sqlConn.ConnectionString = ConnectString.ConnectionString;
            try
            {
                sqlConn.Open();
                sqlQuery = "SELECT store_stock.store_stock_id" +
                ", store_stock.item_id " +
                ", inventory.item_name " +
                ", inventory.item_sales_price " +
                ", store_stock.store_stock_qty " +
                ", store_stock.stock_alarm " +
                ", supplier.supplier_name " +
                ", supplier.supplier_email " +
                ", store_stock.store_stock_remarks " +
                "FROM store_stock " +
                "INNER JOIN inventory " +
                "on inventory.item_id = store_stock.item_id " +
                "INNER JOIN supplier " +
                "on inventory.supplier_id = supplier.supplier_id " +
                "where store_stock_id = '" + selectRow + "' ";
                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    storeItemId = sqlRd.GetString(0);
                    itemId = sqlRd.GetString(1);
                    itemName = sqlRd.GetString(2);
                    price = sqlRd.GetString(3);
                    quantity = sqlRd.GetString(4);
                    alarm = sqlRd.GetString(5);
                    supplierName = sqlRd.GetString(6);
                    supplierEmail = sqlRd.GetString(7);
                    remark = sqlRd.GetString(8);
                }
                sqlConn.Close();
                labStoreItemId.Text = storeItemId;
                labItemid.Text = itemId;
                labItemName.Text = itemName;
                labPrice.Text = price;
                labQuantity.Text = quantity;
                labAlarm.Text = alarm;
                labSupplierName.Text = supplierName;
                labSupplierEmail.Text = supplierEmail;
                labRemark.Text = remark;
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
                label1.Text = "店铺货品号:";
                label2.Text = "货品序号:";
                label3.Text = "货品名称:";
                label4.Text = "金额(HKD):";
                label5.Text = "存货数量:";
                label9.Text = "货品警报:";
                butback.Text = "返回";
                label6.Text = "供应商名称:";
                label7.Text = "供应商电邮:";
                label8.Text = "供应商备注:";
            }
            else
            {
                label1.Text = "Store Item Id:";
                label2.Text = "Item Id:";
                label3.Text = "Item Name:";
                label4.Text = "Price(HKD):";
                label5.Text = "Store Quantity:";
                label9.Text = "Inventory Alarm:";
                butback.Text = "BACK";
                label6.Text = "Supplier Name:";
                label7.Text = "Supplier Email:";
                label8.Text = "Store Remarks:";
            }
        }

        public void setSelectRow(String selectRow)
        {
            this.selectRow = selectRow;
        }

        public void setStoreId(String storeId)
        {
            this.storeId = storeId;
        }

        public String getSelectRow()
        {
            return selectRow;
        }

        private void butback_Click(object sender, EventArgs e)
        {
            Main.instance.shop1(storeId);
        }
    }
}
