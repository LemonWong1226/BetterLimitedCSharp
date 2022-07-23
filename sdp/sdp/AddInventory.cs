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
    public partial class AddInventory : Form
    {
        int quan;
        double price;
        double cost;
        int alarm;
        int count = 0;

        int itemNumber = 100001001;
        String itemId = "SDP100001001";
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;

        public AddInventory()
        {
            InitializeComponent();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {

            try
            {
                if (txtName.Text != "")
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
                        if (txtSupplier.Text != "")
                        {
                            sqlConn.ConnectionString = ConnectString.ConnectionString;
                            sqlConn.Open();
                            sqlCmd.Connection = sqlConn;
                            sqlCmd.CommandText = "select item_id from inventory order by item_id";
                            sqlRd = sqlCmd.ExecuteReader();
                            while (sqlRd.Read())
                            {
                                if (sqlRd.GetString(0) == itemId)
                                {
                                    itemNumber++;
                                    itemId = "SDP" + itemNumber;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            sqlRd.Close();
                            sqlConn.Close();
                            Console.WriteLine(itemId);

                            sqlConn.Open();
                            sqlQuery = "insert into inventory values('" + itemId + "' , '" + txtName.Text + "', '" + txtPrice.Text +
                                       "', '" + txtCost.Text + "', '" + txtQuantity.Text + "',"+txtAlarm.Text+", '" + txtDetail.Text + "','" + txtSupplier.Text + "',DEFAULT)";
                            sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                            sqlRd = sqlCmd.ExecuteReader();

                            sqlConn.Close();
                            itemNumber++;
                            itemId = "SDP" + itemNumber;

                            if (Convert.ToInt32(txtQuantity.Text) <= Convert.ToInt32(txtAlarm.Text))
                            {
                                sqlConn.Open();
                                sqlCmd.Connection = sqlConn;
                                sqlCmd.CommandText = "insert into notification values(default, default,'Alarm Message', '" + txtItem.Text + " store stock level is low', '" + Main.instance.Staff + "')";
                                sqlCmd.ExecuteNonQuery();
                                sqlConn.Close();
                            }
                            MessageBox.Show("Sussessful to add the inventory item");
                        }
                        else
                        {
                            MessageBox.Show("Please input the supplier");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please input the name");
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
            catch (MySqlException)
            {
                MessageBox.Show("The supplier is not found");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConn.Close();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.Inventory();
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            Supplier supplier = new Supplier();
            supplier.setAction("addinventory");
            supplier.ShowDialog();
        }

        private void AddInventory_Load(object sender, EventArgs e)
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
                label2.Text = "货品名称:";
                label3.Text = "数量:";
                label4.Text = "售价(HKD):";
                label5.Text = "成品价(HKD):";
                label8.Text = "货品警报:";
                btnBack.Text = "返回";
                label6.Text = "供应商号 :";
                label7.Text = "货品资料:";
                label1.Text = "货品号:";
                btnSupplier.Text = "供应商";
                btnSubmit.Text = "提交";
            }
            else
            {
                label2.Text = "Item Name:";
                label3.Text = "Quantity:";
                label4.Text = "Sale Price(HKD):";
                label5.Text = "Unit Cost(HKD):";
                label8.Text = "Inventory Alarm:";
                btnBack.Text = "BACK";
                label6.Text = "Supplier No. :";
                label7.Text = "Item Detail:";
                label1.Text = "Item Id:";
                btnSupplier.Text = "Supplier";
                btnSubmit.Text = "SUBMIT";
            }
        }
    }
}
