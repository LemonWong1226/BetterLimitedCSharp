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
    public partial class EditInventory : Form
    {
        int quan;
        double price;
        double cost;
        int alarm;
        int count = 0;
        public String itemid;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;

        public EditInventory()
        {
            InitializeComponent();
        }

        private void EditInventory_Load(object sender, EventArgs e)
        {
            changeLanguage();
            sqlConn.ConnectionString = ConnectString.ConnectionString;
            try
            {
                sqlConn.Open();
                sqlQuery = "SELECT item_id, item_name, inventory_qty, item_sales_price, item_cost, item_alarm, " +
                "supplier.supplier_id, supplier.supplier_name, supplier.supplier_email, item_remarks " +
                "FROM inventory inner join supplier on inventory.supplier_id = supplier.supplier_id " +
                 //  "where item_id = '" + itemid + "'";
                 "where item_id = '"+itemid+"'";
                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    lblItem.Text = sqlRd.GetString(0);
                    txtName.Text = sqlRd.GetString(1);
                    txtQuantity.Text = sqlRd.GetString(2);
                    txtPrice.Text = sqlRd.GetString(3);
                    txtCost.Text = sqlRd.GetString(4);
                    txtAlarm.Text = sqlRd.GetString(5);
                    txtSupplier.Text = sqlRd.GetString(6);
                    txtSupplierName.Text = sqlRd.GetString(7);
                    txtEmail.Text = sqlRd.GetString(8);
                    txtDetail.Text = sqlRd.GetString(9);
                }
                sqlConn.Close();
                /*
                if(Convert.ToInt32(txtQuantity.Text) <= Convert.ToInt32(txtAlarm.Text))
                {
                    sqlConn.Open();
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandText = "insert into notification values(default, default,'Alarm Message', '" + lblItem.Text + " stock level is low', '" + Main.instance.Staff + "')";
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                }

                */
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
                label3.Text = "仓库数量:";
                label4.Text = "售价(HKD):";
                label5.Text = "成品价(HKD):";
                label8.Text = "货品警报:";
                btnBack.Text = "返回";
                label6.Text = "供应商号 :";
                label9.Text = "供应商名称 :";
                label10.Text = "供应商电邮 :";
                label7.Text = "货品资料 :";
                btnDelete.Text = "删除";
                btnConfirm.Text = "确认";
            }
            else
            {
                label1.Text = "Item Id:";
                label2.Text = "Item Name:";
                label3.Text = "Inventory Quantity:";
                label4.Text = "Sale Price(HKD):";
                label5.Text = "Unit Cost(HKD):";
                label8.Text = "Inventory Alarm:";
                btnBack.Text = "BACK";
                label6.Text = "Supplier No. :";
                label9.Text = "Supplier Name :";
                label10.Text = "Supplier Email :";
                label7.Text = "Item Detail :";
                btnDelete.Text = "Delete";
                btnConfirm.Text = "Confirm";
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

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            try
            {

                sqlConn.Open();
                sqlQuery = "DELETE FROM inventory WHERE item_id = '" + itemid + "'";
                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                sqlCmd.ExecuteNonQuery();
                MessageBox.Show("Delete successful");
                Main.instance.Inventory();
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

        private void btnConfirm_Click_1(object sender, EventArgs e)
        {
            try
            {
                count = 0;
                quan = Convert.ToInt32(txtQuantity.Text);
                count++;
                price = Convert.ToDouble(txtPrice.Text);
                count++;
                cost = Convert.ToDouble(txtCost.Text);
                count++;
                alarm = Convert.ToInt32(txtAlarm.Text);
                count++;
                if (quan < 0)
                {
                    MessageBox.Show("Please input positive integer on quantity");
                }
                else if (price < 0)
                {
                    MessageBox.Show("Please input positive integer on price");
                }
                else if (cost < 0)
                {
                    MessageBox.Show("Please input positive integer on cost");
                }
                else if (alarm < 0)
                {
                    MessageBox.Show("Please input positive integer on alarm");
                }
                else
                {
                    sqlConn.Open();
                    sqlQuery = "update inventory set item_name = '" + txtName.Text + "', inventory_qty = " + txtQuantity.Text + ", item_sales_price = " + txtPrice.Text + ", item_cost = " + txtCost.Text +
                        ", item_alarm = " + txtAlarm.Text + ", item_remarks = '" + txtDetail.Text + "'" +
                        "  where item_id = '" + itemid + "'";
                    sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    MessageBox.Show("Update successful");
                    sqlConn.Close();
                }
                if (Convert.ToInt32(txtQuantity.Text) == 0)
                {
                    sqlConn.Open();
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandText = "insert into notification values(default, default,'Alarm Message', '" + lblItem.Text + " is out of stock already, please purchase the item', '" + Main.instance.Staff + "')";
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    Main.instance.increaseNotification();
                    Main.instance.setButtonRedDot();
                }
                else if (Convert.ToInt32(txtQuantity.Text) <= Convert.ToInt32(txtAlarm.Text))
                {
                    sqlConn.Open();
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandText = "insert into notification values(default, default,'Alarm Message', '" + lblItem.Text + " stock level is low', '" + Main.instance.Staff + "')";
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    Main.instance.increaseNotification();
                    Main.instance.setButtonRedDot();
                }
            }
            catch (FormatException ex)
            {
                switch (count)
                {
                    case 0:
                        MessageBox.Show("Please input correct integer on quantity");
                        break;
                    case 1:
                        MessageBox.Show("Please input correct integer on price");
                        break;
                    case 2:
                        MessageBox.Show("Please input correct integer on Cost");
                        break;
                    case 3:
                        MessageBox.Show("Please input correct integer on Alarm");
                        break;
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
