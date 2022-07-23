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
    public partial class edititem : Form
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

        public edititem()
        {
            InitializeComponent();
        }

        private void edititem_Load(object sender, EventArgs e)
        {
            changeLanguage();
            update();
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
                label2.Text = "货品号:";
                label3.Text = "货品名称:";
                label4.Text = "金额(HKD):";
                label5.Text = "存货数量:";
                label9.Text = "货品警报:";
                butback.Text = "返回";
                label6.Text = "供应商名称:";
                label7.Text = "供应商电邮:";
                label8.Text = "供应商备注:";
                label10.Text = "补货数量:";
                label11.Text = "*当收到从仓库运来的货品时，请在此输入数量";
                butDelete.Text = "删除";
                butConfirm.Text = "确认";
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
                label10.Text = "Add Quantity :";
                label11.Text = "*When received the item from inventory, please enter the quantity";
                butDelete.Text = "Delete";
                butConfirm.Text = "Confirm";
            }
        }

        public void update()
        {
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
                txtQuantity.Text = quantity;
                txtAlarm.Text = alarm;
                labSupplierName.Text = supplierName;
                labSupplierEmail.Text = supplierEmail;
                txtRemark.Text = remark;

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

        public void setSelectRow(String selectRow)
        {
            this.selectRow = selectRow;
        }

        public String getSelectRow()
        {
            return selectRow;
        }

        public void setStoreId(String storeId)
        {
            this.storeId = storeId;
        }

        private void butback_Click(object sender, EventArgs e)
        {
     //       Shop1 back = new Shop1();
      //      this.Hide();
      //      back.ShowDialog();
       //     this.Close();

            Main.instance.shop1(storeId);
        }

        private void butDelete_Click(object sender, EventArgs e)
        {
            string message = "are you want to delete the item?";
            string title = "delete item";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.Yes)
            {
                try
                {
                    sqlConn.Open();
                    sqlQuery = "DELETE FROM store_stock WHERE store_stock_id = '" + selectRow + "'";
                    sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                    sqlRd = sqlCmd.ExecuteReader();
                    sqlRd.Close();
                    sqlConn.Close();
                    MessageBox.Show("Delete successful");
                    Main.instance.shop1(selectRow);

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


        private void butConfirm_Click(object sender, EventArgs e)
        {
            try
            {    
                if (Convert.ToInt32(txtQuantity.Text) >= 0 && Convert.ToInt32(txtAlarm.Text) >= 0 && Convert.ToInt32(txtAddQuan.Text) >= 0)
                {
                    int inventoryQty = 0;
                    sqlConn.ConnectionString = ConnectString.ConnectionString;
                    sqlConn.Open();
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandText = "select inventory_qty from inventory where item_id = '"+ labItemid.Text+ "'";
                    sqlRd = sqlCmd.ExecuteReader();
                    while (sqlRd.Read())
                    {
                        inventoryQty = sqlRd.GetInt32(0);
                    }
                    sqlRd.Close();
                    sqlConn.Close();

                    if (inventoryQty - Convert.ToInt32(txtAddQuan.Text) >= 0)
                    {
                        sqlConn.Open();
                        sqlQuery = "update store_stock " +
                                   "set store_stock_qty = " + (Convert.ToInt32(txtQuantity.Text) + Convert.ToInt32(txtAddQuan.Text)) + " , store_stock_remarks = '" + txtRemark.Text + "' , stock_alarm =" + txtAlarm.Text + " " +
                                   "where store_stock_id = '" + selectRow + "'";
                        sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                        sqlRd = sqlCmd.ExecuteReader();
                        sqlRd.Close();
                        sqlConn.Close();

                        sqlConn.Open();
                        sqlQuery = "update inventory set inventory_qty = inventory_qty - " + txtAddQuan.Text + " where item_id = '" + itemId + "'";
                        sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                        sqlRd = sqlCmd.ExecuteReader();
                        sqlRd.Close();
                        sqlConn.Close();
                        MessageBox.Show("Update successfully");
                        update();
                    }
                    else
                    {
                        MessageBox.Show("the warehouse have no enough quantity");
                    }
                }
                else
                {
                    MessageBox.Show("Cannot input negative number");
                }

                if (Convert.ToInt32(txtQuantity.Text) <= Convert.ToInt32(txtAlarm.Text))
                {
                    sqlConn.Open();
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandText = "insert into notification values(default, default,'Alarm Message', '" + labStoreItemId.Text + " stock level is low', '" + Main.instance.Staff + "')";
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    Main.instance.increaseNotification();
                    Main.instance.setButtonRedDot();
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please input correct integer on corresponder textbox");
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

        private void txtAddQuan_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
