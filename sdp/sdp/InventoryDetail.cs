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
    public partial class InventoryDetail : Form
    {
        public static InventoryDetail instance;
        public String itemid;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;

        public InventoryDetail()
        {
            InitializeComponent();
            instance = this;
        }

        private void InventoryDetail_Load(object sender, EventArgs e)
        {
            changeLanguage();
            sqlConn.ConnectionString = ConnectString.ConnectionString;
            try
            {
                sqlConn.Open();
                sqlQuery = "SELECT item_id, item_name, inventory_qty, item_sales_price, item_cost, last_update, item_alarm, " +
                "supplier.supplier_id, supplier.supplier_name, supplier.supplier_email, item_remarks " +
                "FROM inventory " +
                "inner join supplier " +
                "on inventory.supplier_id = supplier.supplier_id " +
                "where item_id = '"+itemid+"'";
                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    lblItem.Text = sqlRd.GetString(0);
                    lblName.Text = sqlRd.GetString(1);
                    lblQuantity.Text = sqlRd.GetString(2);
                    lblPrice.Text = sqlRd.GetString(3);
                    lblCost.Text = sqlRd.GetString(4);
                    lblTime.Text = sqlRd.GetString(5);
                    lblAlarm.Text = sqlRd.GetString(6);
                    lblSupplierNo.Text = sqlRd.GetString(7);
                    lblSupplierName.Text = sqlRd.GetString(8);
                    lblSupplierEmail.Text = sqlRd.GetString(9);
                    lblItemDetail.Text = sqlRd.GetString(10);
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
                label1.Text = "货品号:";
                label2.Text = "货品名称:";
                label3.Text = "数量:";
                label4.Text = "售价(HKD):";
                label5.Text = "成品价(HKD):";
                label8.Text = "最后更新时间:";
                label16.Text = "货品警报:";
                btnBack.Text = "返回";
                label6.Text = "供应商号 :";
                label9.Text = "供应商名称 :";
                label10.Text = "供应商电邮 :";
                label7.Text = "货品资料:";
            }
            else
            {
                label1.Text = "Item Id:";
                label2.Text = "Item Name:";
                label3.Text = "Inventory Quantity:";
                label4.Text = "Sale Price(HKD):";
                label5.Text = "Unit Cost(HKD):";
                label8.Text = "Item Last Update Time:";
                label16.Text = "Inventory Alarm:";
                btnBack.Text = "BACK";
                label6.Text = "Supplier No. :";
                label9.Text = "Supplier Name :";
                label10.Text = "Supplier Email :";
                label7.Text = "Item Detail:";
            }
        }

        public void setitemid(String itemid)
        {
            this.itemid = itemid;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.Inventory();
        }
    }
}
