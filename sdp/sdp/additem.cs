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
    public partial class additem : Form
    {
        int inventoryQty = 0;
        int inventoryAlarm = 0;
        int islow;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;

        String store;
        private string language;

        public additem()
        {
            InitializeComponent();
        }

        public String setStoreAlpha(String store)
        {
            if(store == "HK01")
            {
                return "A";
            }
            else
            {
                return "B";
            }
        }

        private void button1_Click(object sender, EventArgs e)
    {
            int number = 1;
            sqlConn.ConnectionString = ConnectString.ConnectionString;

            try
            {
                if(Convert.ToInt32(txtQuantity.Text) > Convert.ToInt32(txtAlarm.Text))
                {
                    islow = 0;
                }
                else
                {
                    islow = 1;
                }
                string value = String.Format(setStoreAlpha(store) + "{0:D5}", number);
                sqlConn.Open();
                sqlQuery = "SELECT * FROM store_stock where store_id = '"+store+"' order by store_stock_id";
                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    if(sqlRd.GetString(0) == value)
                    {
                        number++;
                        value = String.Format(setStoreAlpha(store) + "{0:D5}", number);
                    }
                    else
                    {
                        break; ;
                    }
                }
                sqlRd.Close();
                sqlConn.Close();
                number += 1;
                if (Convert.ToInt32(txtAlarm.Text) < 0)
                {
                    MessageBox.Show("Please input positive integer on on Alarm Level");
                }
                else if(Convert.ToInt32(txtQuantity.Text) < 0)
                {
                    MessageBox.Show("Please input positive integer on on Quantity");
                }
                else
                {

                    sqlConn.Open();
                    sqlQuery = "select inventory_qty, item_alarm from inventory where item_id = '" + txtItemID.Text + "'";
                    sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                    sqlRd = sqlCmd.ExecuteReader();
                    while (sqlRd.Read())
                    {
                        inventoryQty = sqlRd.GetInt32(0);
                        inventoryAlarm = sqlRd.GetInt32(1);
                    }
                    sqlRd.Close();
                    sqlConn.Close();

                    if (inventoryQty >= Convert.ToInt32(txtQuantity.Text))
                    {
                        sqlConn.Open();
                        sqlQuery = "insert into store_stock values ('" + value + "', '" + store + "', '" + txtItemID.Text +
                "', '" + txtQuantity.Text + "', " + Convert.ToInt32(txtAlarm.Text) + ", '" + txtRemark.Text + "', '" + islow + "')";
                        sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                        sqlRd = sqlCmd.ExecuteReader();
                        MessageBox.Show("Sussessful to add the item");
                        sqlConn.Close();

                        sqlConn.Open();
                        sqlCmd.Connection = sqlConn;
                        sqlCmd.CommandText = "update inventory set inventory_qty = (inventory_qty - " + txtQuantity.Text + ") where item_id = '" + txtItemID.Text + "'";
                        sqlCmd.ExecuteNonQuery();
                        sqlConn.Close();

                        if (Convert.ToInt32(txtQuantity.Text) <= Convert.ToInt32(txtAlarm.Text))
                        {
                            sqlConn.Open();
                            sqlCmd.Connection = sqlConn;
                            sqlCmd.CommandText = "insert into notification values(default, default,'Alarm Message', '" + txtItemID.Text + " stock level is low', '" + Main.instance.Staff + "')";
                            sqlCmd.ExecuteNonQuery();
                            sqlConn.Close();
                            Main.instance.increaseNotification();
                            Main.instance.setButtonRedDot();
                        }

                        if ((inventoryQty - Convert.ToInt32(txtQuantity.Text)) <= inventoryAlarm && inventoryQty.ToString() != txtQuantity.Text)
                        {
                            sqlConn.Open();
                            sqlCmd.Connection = sqlConn;
                            sqlCmd.CommandText = "insert into notification values(default, default,'Alarm Message', '" + txtItemID.Text + " stock level is low', '" + Main.instance.Staff + "')";
                            sqlCmd.ExecuteNonQuery();
                            sqlConn.Close();
                            Main.instance.increaseNotification();
                            Main.instance.setButtonRedDot();
                        }

                        if(inventoryQty.ToString() == txtQuantity.Text)
                        {
                            sqlConn.Open();
                            sqlCmd.Connection = sqlConn;
                            sqlCmd.CommandText = "insert into notification values(default, default,'Alarm Message', '" + txtItemID.Text + " is out of stock on inventory already, please purchase the item', '" + Main.instance.Staff + "')";
                            sqlCmd.ExecuteNonQuery();
                            sqlConn.Close();
                            Main.instance.increaseNotification();
                            Main.instance.setButtonRedDot();
                        }
                    }
                    else
                    {
                        MessageBox.Show("The item have not enough quantity");
                    }

                }
            }
            catch (Exception ex)
            {
        //        MessageBox.Show(ex.Message);
                if(ex.Message.IndexOf("FOREIGN KEY") != -1)
                {
                    MessageBox.Show("The item ID do not exist");
                }
                else if (ex.Message.IndexOf("Duplicate entry") != -1)
                {
                    MessageBox.Show("The store item already exist on the store");
                }
                else if (ex.Message.IndexOf("store_stock_qty") != -1)
                {
                    MessageBox.Show("Please input integer on Quantity");
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }
            finally
            {
                sqlConn.Close();
            }
        }

        private void butBack_Click(object sender, EventArgs e)
        {
            Main.instance.shop1(store);
        }

        private void additem_Load(object sender, EventArgs e)
        {
            changeLanguage();
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
                label2.Text = "数量:";
                label5.Text = "警报水平:";
                label4.Text = "备注:";
                butBack.Text = "返回";
                button1.Text = "提交";
            }
            else
            {
                label1.Text = "Item Id:";
                label2.Text = "Quantity:";
                label5.Text = "Alarm Level:";
                label4.Text = "Remarks:";
                butBack.Text = "BACK";
                button1.Text = "SUBMIT";
            }
        }

        public void setStore(String store)
        {
            this.store = store;
        }
    }
}
