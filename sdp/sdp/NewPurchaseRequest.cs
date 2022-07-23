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
    public partial class NewPurchaseRequest : Form
    {
        private double total;
        private int totalQty;
        private String purchaseRequestId;
        Boolean first = true;
        Boolean supplierNoDulicate;
        Boolean found;
        int count = 0;
        Dictionary<string, string> dicquan = new Dictionary<string, string>();
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;

        public NewPurchaseRequest()
        {
            InitializeComponent();
        }

        private void NewPurchaseRequest_Load(object sender, EventArgs e)
        {
            changeLanguage();
            DataGridViewTextBoxColumn itemId = new DataGridViewTextBoxColumn();
            itemId.ReadOnly = true;
            DataGridViewTextBoxColumn itemName = new DataGridViewTextBoxColumn();
            itemName.ReadOnly = true;
            DataGridViewTextBoxColumn quantity = new DataGridViewTextBoxColumn();
            quantity.ValueType = typeof(int);
            DataGridViewTextBoxColumn itemCost = new DataGridViewTextBoxColumn();
            itemCost.ReadOnly = true;
            DataGridViewTextBoxColumn supplierId = new DataGridViewTextBoxColumn();
            supplierId.ReadOnly = true;
            DataGridViewButtonColumn cancel = new DataGridViewButtonColumn();
            cancel.HeaderText = "";
            cancel.Text = "X";
            cancel.UseColumnTextForButtonValue = true;
            if (language == "Chinese")
            {
                itemId.HeaderText = "货品号";
                itemName.HeaderText = "货品名称";
                quantity.HeaderText = "数量";
                itemCost.HeaderText = "成本价";
                supplierId.HeaderText = "供应商号";
            }
            else
            {
                itemId.HeaderText = "Item ID";
                itemName.HeaderText = "Item Name";
                quantity.HeaderText = "Quantity";
                itemCost.HeaderText = "Unix Cost";
                supplierId.HeaderText = "Supplier Id";
            }
            dataGridView1.Columns.AddRange(new[] { itemId, itemName, quantity, itemCost, supplierId });
            dataGridView1.Columns.Add(cancel);
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
                labelItemcode.Text = "货品号：";
                btnSubmitItem.Text = "提交";
                label5.Text = "备注:";
                btnBack.Text = "返回";
                label1.Text = "供应商号：";
                btnSupplier.Text = "提交";
                label6.Text = "总共金额(HKD):";
                btnSubmit.Text = "提交";
            }
            else
            {
                labelItemcode.Text = "Item Code：";
                btnSubmitItem.Text = "SUBMIT";
                label5.Text = "Remark:";
                btnBack.Text = "BACK";
                label1.Text = "Supplier Id：";
                btnSupplier.Text = "SUBMIT";
                label6.Text = "Total Cost(HKD):";
                btnSubmit.Text = "SUBMIT";
            }
        }

        private void btnSubmitItem_Click(object sender, EventArgs e)
        {
            try
            {
                ConnectString rootconn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = rootconn.getString();
                found = false;
                supplierNoDulicate = false;
                String[] row = new String[4];
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "SELECT item_id, item_name, supplier_id, item_cost " +
                    "FROM inventory " +
                    "where item_id = '" + txtItem.Text + "'";

                sqlRd = sqlCmd.ExecuteReader();


                while (sqlRd.Read())
                {
                    for (int i = 0; i < 4; i++)
                    {
                        row[i] = sqlRd.GetString(i);
                    }
               //     supplier = row[2];
                }
                sqlRd.Close();

                if (row[0] != null)
                {
                    if (first)
                    {
                        dataGridView1.Rows.Add(row[0], row[1], 0, row[3], row[2]);
                        quantityDictionary();
                        first = false;
                    }
                    else
                    {
                        count = 0;
                        foreach (DataGridViewRow item in dataGridView1.Rows)
                        {
                            if (row[0] == item.Cells[0].Value.ToString())
                            {
                                found = true;
                                break;
                            }
                            count++;
                        }
                        count = 0;
                        foreach (DataGridViewRow item in dataGridView1.Rows)
                        {
                            if (row[2] != item.Cells[4].Value.ToString())
                            {
                                supplierNoDulicate = true;
                                break;
                            }
                            else
                            {
                                supplierNoDulicate = false;
                            }
                            count++;
                        }
                        if (found)
                        {

                            MessageBox.Show("The item has already exist");
                        }
                        else if (supplierNoDulicate)
                        {
                            MessageBox.Show("The supplier has already exist");
                        }
                        else
                        {
                            dataGridView1.Rows.Add(row[0], row[1], 0, row[3], row[2]);
                            quantityDictionary();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("The item do not exist");
                }
                sqlConn.Close();
            }
            catch (NoNullAllowedException)
            {
                MessageBox.Show("The item do not exist");
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("The item do not exist");
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

        public void quantityDictionary()
        {
            dicquan.Clear();
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                dicquan.Add(item.Cells[0].Value.ToString(), item.Cells[2].Value.ToString());
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (Convert.ToInt32(row.Cells[2].Value) < 0)
                {
                    MessageBox.Show("you cannot input negative value on quantity item");
                    row.Cells[2].Value = Convert.ToInt32(dicquan[row.Cells[0].Value.ToString()]);
                    break;
                }
            }
            CalculateTotal();
            quantityDictionary();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                dicquan.Remove(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                dataGridView1.Rows.RemoveAt(e.RowIndex);
                CalculateTotal();
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Please input the integer on the quantity");
            dataGridView1.CancelEdit();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                CalculateTotal();
                ConnectString rootconn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = rootconn.getString();
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "insert into purchase_request values (default, "+totalQty+", '"+txtRemark.Text+"', 1, "+ total + ", default); " +
                    "SELECT LAST_INSERT_ID();";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    purchaseRequestId = sqlRd.GetString(0);
                }
                sqlRd.Close();
                sqlConn.Close();

                int count = 0;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    double itemTotal = Convert.ToDouble(row.Cells[2].Value) * Convert.ToDouble(row.Cells[3].Value);
                    sqlConn.Open();
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandText = "insert into purchase_item values(default, '"+ purchaseRequestId + 
                        "', '"+row.Cells[0].Value+"', "+ row.Cells[2].Value + ", "+ itemTotal + ","+ row.Cells[3].Value + ")";
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    count++;
                }
                MessageBox.Show("the purchase request has been sent");
            }
            else
            {
                MessageBox.Show("You must input a purchase request item");
            }
        }

        public void CalculateTotal()
        {
            totalQty = 0;
            total = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                totalQty += Convert.ToInt32(row.Cells[2].Value);
                total += Convert.ToDouble(row.Cells[3].Value) * Convert.ToInt32(row.Cells[2].Value);
            }
            lblTotal.Text = total.ToString();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.PurchaseRequestOrderRecord();
        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {

        }
    }
}
