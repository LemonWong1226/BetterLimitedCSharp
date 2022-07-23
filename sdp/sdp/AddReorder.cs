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
    public partial class AddReorder : Form
    {
        private int totalQty;
        private String reorderID;
        private String store;
        Boolean first = true;
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

        public AddReorder()
        {
            InitializeComponent();
        }

        private void AddReorder_Load(object sender, EventArgs e)
        {
            changeLanguage();
            DataGridViewTextBoxColumn itemId = new DataGridViewTextBoxColumn();
            itemId.ReadOnly = true;
            DataGridViewTextBoxColumn itemName = new DataGridViewTextBoxColumn();
            itemName.Width = 150;
            itemName.ReadOnly = true;
            DataGridViewTextBoxColumn quantity = new DataGridViewTextBoxColumn();
            quantity.Width = 10;
            quantity.ValueType = typeof(int);
            DataGridViewButtonColumn cancel = new DataGridViewButtonColumn();
            cancel.Text = "X";
            cancel.UseColumnTextForButtonValue = true;
            cancel.Width = 25;

            if (language == "Chinese")
            {
                itemId.HeaderText = "货品号";
                itemName.HeaderText = "货品名称";
                quantity.HeaderText = "数量";
                cancel.HeaderText = "";
            }
            else
            {
                itemId.HeaderText = "Item ID";
                itemName.HeaderText = "Item Name";
                quantity.HeaderText = "Quantity";
                cancel.HeaderText = "";
            }
            dataGridView1.Columns.AddRange(new[] { itemId , itemName , quantity });
            dataGridView1.Columns.Add(cancel);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.shop1(store);
        }

        private void btnSubmitItem_Click(object sender, EventArgs e)
        {
            try
            {
                ConnectString rootconn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = rootconn.getString();
                found = false;
                String[] row = new String[2];
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "SELECT store_stock.item_id, item_name, item_sales_price, store_stock_qty " +
                    "FROM store_stock " +
                    "inner join inventory " +
                    "on store_stock.item_id = inventory.item_id " +
                    "where store_stock.item_id = '" + txtItem.Text + "' and store_id = '" + store + "'";

                sqlRd = sqlCmd.ExecuteReader();


                while (sqlRd.Read())
                {
                    for (int i = 0; i < 2; i++)
                    {
                        row[i] = sqlRd.GetString(i);
                    }
                }
                sqlRd.Close();

                if (row[0] != null)
                {
                    if (first)
                    {
                        dataGridView1.Rows.Add(row[0],row[1],0);
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
                        if (found)
                        {

                            MessageBox.Show("The item has already exist");
                        }
                        else
                        {
                            dataGridView1.Rows.Add(row[0], row[1], 0);
                            quantityDictionary();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("The item does not exist");
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
                label5.Text = "备注:";
                btnBack.Text = "返回";
                btnSubmit.Text = "提交";
            }
            else
            {
                labelItemcode.Text = "Item Code：";
                label5.Text = "Remark:";
                btnBack.Text = "BACK";
                btnSubmit.Text = "SUBMIT";
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

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Please input the integer on the quantity");
            dataGridView1.CancelEdit();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    totalQty += Convert.ToInt32(row.Cells[2].Value);
                }
                MessageBox.Show("the re-order request has been sent");
                ConnectString rootconn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = rootconn.getString();
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "insert into reorder_request values(default, " + totalQty + ", default, '" + Main.instance.Staff + "', '" + store + "', 1, '" + txtRemark.Text + "'); " +
                    "SELECT LAST_INSERT_ID();";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    reorderID = sqlRd.GetString(0);
                }
                sqlRd.Close();
                sqlConn.Close();

                int count = 0;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    sqlConn.Open();
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandText = "insert into reorder_item values (default, '" + reorderID + "', '" + row.Cells[0].Value + "', '" + row.Cells[2].Value + "')";
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    count++;
                }
            }
            else
            {
                MessageBox.Show("You must input a re-order item");
            }
        }

        private void cbxStore_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if(Convert.ToInt32(row.Cells[2].Value) < 0)
                {
                    MessageBox.Show("you cannot input negative value on quantity item");
                    row.Cells[2].Value = Convert.ToInt32(dicquan[row.Cells[0].Value.ToString()]);
                    break;
                }
            }
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
            }
        }

        public void setStore(String store)
        {
            this.store = store;
        }
    }
}
