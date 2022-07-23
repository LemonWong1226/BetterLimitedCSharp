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
    public partial class checkout : Form
    {
        public String cashAction;
        public static checkout instance;
        Boolean found;
        Boolean itemDeposit;
        public Double minDeposit;
        public Double maxDeposit;
        public String store;
        public String isCancel;
        public String ispreOrderl;
        public String ispreComplete;
        public String address;
        public String invoice;
        public String preOrder;
        public String paidDeposit;
        public Double originalTotal;
        public Double discount;
        public String remark;
        public int totalQuan;
        public int quanSubtract;
        public Double totalDiscount;
        public Double total;
        public int pageHeight;
        public String[,] order;
        Boolean first = true;
        int count = 0;
        int dispositCount = 0;
        Dictionary<string, string> dicquan = new Dictionary<string, string>();
        Dictionary<string, string> dicPrice = new Dictionary<string, string>();
        Dictionary<string, string> dicDiscount = new Dictionary<string, string>();
        Dictionary<string, string> dicOriginalQuan = new Dictionary<string, string>();
        Dictionary<string, int> dicDispositQty = new Dictionary<string, int>();
        Dictionary<string, Double> dicDispositPrice = new Dictionary<string, Double>();
        Dictionary<string, Double> dicDispositDiscount = new Dictionary<string, Double>();

        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;

        public checkout()
        {
            InitializeComponent();
            instance = this;
        }


        private void checkout_Load(object sender, EventArgs e)
        {
            changeLanguage();
            cbxCash.Checked = true;
            DataGridViewTextBoxColumn Col1 = new DataGridViewTextBoxColumn();
            Col1.HeaderText = "item_id";
            Col1.ReadOnly = true;
            DataGridViewTextBoxColumn Col2 = new DataGridViewTextBoxColumn();
            Col2.HeaderText = "item_name";
            Col2.Width = 150;
            Col2.ReadOnly = true;
            DataGridViewTextBoxColumn Col3 = new DataGridViewTextBoxColumn();
            Col3.HeaderText = "Quantity";
            Col3.Width = 10;
            Col3.ValueType = typeof(int);
            DataGridViewTextBoxColumn Col4 = new DataGridViewTextBoxColumn();
            Col4.HeaderText = "Price";
            Col4.ValueType = typeof(double);
            DataGridViewTextBoxColumn Col5 = new DataGridViewTextBoxColumn();
            Col5.HeaderText = "Discount(%)";
            Col5.Width = 100;
            Col5.ValueType = typeof(double);
            DataGridViewTextBoxColumn Col6 = new DataGridViewTextBoxColumn();
            Col6.HeaderText = "Total";
            Col6.ReadOnly = true;
            Col6.ValueType = typeof(String);
            DataGridViewButtonColumn Col7 = new DataGridViewButtonColumn();

            Col7.HeaderText = "";
            Col7.Text = "X";
            Col7.UseColumnTextForButtonValue = true;
            Col7.Width = 25;
            if (language == "Chinese")
            {
                Col1.HeaderText = "货品号";
                Col2.HeaderText = "货品名称";
                Col3.HeaderText = "数量";
                Col4.HeaderText = "价格";
                Col5.HeaderText = "折扣(%)";
                Col6.HeaderText = "总共";
            }
            else
            {
                Col1.HeaderText = "Item Id";
                Col2.HeaderText = "Item Name";
                Col3.HeaderText = "Quantity";
                Col4.HeaderText = "Price";
                Col5.HeaderText = "Discount(%)";
                Col6.HeaderText = "Total";
            }
            dataGridView1.Columns.AddRange(new[] { Col1, Col2, Col3, Col4, Col5, Col6 });
            dataGridView1.Columns.Add(Col7);

            // TODO: This line of code loads data into the 'sdpDataSet.inventory' table. You can move, or remove it, as needed.
            //      inventoryTableAdapter.FillBy(sdpDataSet.inventory);

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
                btnInvoice.Text = "提交";
                labelPreInvoice.Text = "预订发票号 :";
                label5.Text = "备注:";
                btnBack.Text = "返回";
                btnClean.Text = "清除输入";
                label1.Text = "最低订金 :";
                label2.Text = "订金:";
                cbxPre.Text = "预订";
                label3.Text = "订金:";
                label4.Text = "折扣(HKD):";
                label6.Text = "总共(HKD):";
                cbxCash.Text = "现金";
                btnDeposit.Text = "输入订金";
                btnCheckout.Text = "结账";
            }
            else
            {
                labelItemcode.Text = "Item Code：";
                btnSubmitItem.Text = "SUBMIT";
                btnInvoice.Text = "SUBMIT";
                labelPreInvoice.Text = "Pre-order invoice :";
                label5.Text = "Remark:";
                btnBack.Text = "BACK";
                btnClean.Text = "CLEAN INPUT";
                label1.Text = "Min Deposit :";
                label2.Text = "Deposit:";
                cbxPre.Text = "Pre-order";
                label3.Text = "Deposit:";
                label4.Text = "Discount(HKD):";
                label6.Text = "Total(HKD):";
                cbxCash.Text = "CASH";
                btnDeposit.Text = "Input Deposit";
                btnCheckout.Text = "CHECKOUT";
            }
        }

        private void btnSubmitItem_Click(object sender, EventArgs e)
        {
            try
            {
                ConnectString rootconn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = rootconn.getString();
                found = false;
                String[] row = new String[9];
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
                    for (int i = 0; i < 4; i++)
                    {
                        row[i] = sqlRd.GetString(i);
                    }
                }
                sqlRd.Close();
                if (row[0] != null)
                {
                    if (Convert.ToInt32(row[3]) > 0)
                    {
                        if (first)
                        {
                            dataGridView1.Rows.Add(row[0], row[1], 1, Convert.ToDouble(row[2]), Convert.ToDouble(0), row[2]);
                            dicOriginalQuan.Add(row[0], row[3]);
                            updateDictionary();
                            calculateTotal();
                            first = false;

                        }
                        else
                        {
                            count = 0;
                            foreach (DataGridViewRow item in dataGridView1.Rows)
                            {
                                if (row[0] == item.Cells[0].Value.ToString() && "Deposit" != item.Cells[5].Value.ToString())
                                {
                                    found = true;
                                    break;
                                }
                                count++;
                            }

                            if (found)
                            {
                                dataGridView1[2, count].Value = 1 + Convert.ToInt32(dicquan[row[0]]);
                            }
                            else
                            {
                                dataGridView1.Rows.Add(row[0], row[1], 1, Convert.ToDouble(row[2]), Convert.ToDouble(0), row[2]);
                                dicOriginalQuan.Add(row[0], row[3]);
                                updateDictionary();
                                calculateTotal();
                            }
                        }
                    }
                    else if (Convert.ToDouble(row[2]) >= 5000)
                    {
                        itemDeposit = false;
                        dispositCount = 0;
                        foreach (DataGridViewRow item in dataGridView1.Rows)
                        {
                            if (row[0] == item.Cells[0].Value.ToString() && item.Cells[5].Value.ToString() == "Deposit")
                            {
                                itemDeposit = true;
                                break;
                            }
                            dispositCount++;
                        }

                        if (!itemDeposit)
                        {
                            string message = "the item is out of stock, are you want to make the deposit?";
                            string title = "make deposit";
                            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                            DialogResult result = MessageBox.Show(message, title, buttons);
                            if (result == DialogResult.Yes)
                            {
                                dataGridView1.Rows.Add(row[0], row[1], 1, Convert.ToDouble(row[2]), Convert.ToDouble(0), "Deposit");
                                updateDictionary();
                                calculateTotal();
                                cbxPre.Checked = true;
                            }
                        }
                        else
                        {
                            MessageBox.Show("you cannot add the deposit item");
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow item in dataGridView1.Rows)
                        {
                            if (row[0] == item.Cells[0].Value.ToString() && item.Cells[5].Value.ToString() == "Deposit")
                            {
                                itemDeposit = true;
                                break;
                            }
                            dispositCount++;
                        }

                        if (!itemDeposit)
                        {
                            string message = "the item is out of stock, are you want to make the deposit?";
                            string title = "make deposit";
                            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                            DialogResult result = MessageBox.Show(message, title, buttons);
                            if (result == DialogResult.Yes)
                            {
                                dataGridView1.Rows.Add(row[0], row[1], 1, Convert.ToDouble(row[2]), Convert.ToDouble(0), "Deposit");
                                updateDictionary();
                                calculateTotal();
                                cbxPre.Checked = true;
                            }
                        }
                        else
                        {
                            MessageBox.Show("you cannot add the deposit item");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("the item does not exist");
                }
                sqlConn.Close();


                //this code is complicated and error
                /*if (first)
                {
                    sqlDt.Load(sqlRd);
                    first = false;
                }
                else
                {
                    while (sqlRd.Read())
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            row[i] = sqlRd.GetString(i);
                        }
                    }
                    sqlDt.Rows.Add(row[0], row[1], row[2], row[3], row[4], row[5], row[6], row[7], row[8]);
                }
                sqlRd.Close();
                sqlConn.Close();
                dataGridView1.DataSource = sqlDt;
                //     MessageBox.Show(dataGridView1.Rows[0].Cells["itemidDataGridViewTextBoxColumn"].Value.ToString());
                dataGridView1.Rows[count].Cells["Quanlity"].Value = "1";
                dataGridView1.Rows[count].Cells["Discount"].Value = "0";


                quan = Convert.ToInt32(dataGridView1[2, count].Value.ToString());
                    price = Convert.ToDouble(dataGridView1[3, count].Value.ToString());
                    total = quan * price;
                dataGridView1.Rows[count].Cells["Total"].Value = total.ToString();
                //     dataGridView1[5, count].Value = dataGridView1[3, count].Value.ToString();
                count += 1; */

            }
            catch (ConstraintException)
            {
                MessageBox.Show("the item already exist");
            }
            catch (NoNullAllowedException)
            {
                MessageBox.Show("the item do not exist");
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("the item do not exist");
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

        public Boolean isDeposit()
        {
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                if (item.Cells[5].Value.ToString() == "Deposit")
                {
                    return true;
                }
            }
            return false;
        }

        public Boolean checkItemDeposit(String itemId)
        {
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                if (item.Cells[5].Value.ToString() == "Deposit" && item.Cells[0].Value.ToString() == itemId)
                {
                    return true;
                }
            }
            return false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString() != "Deposit")
                {
                    dicDiscount.Remove(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                    dicOriginalQuan.Remove(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                    dicPrice.Remove(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                    dicquan.Remove(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());

                    dataGridView1.Rows.RemoveAt(e.RowIndex);
                    count -= 1;
                    UpdateValue();
                    updateDictionary();
                }
                else
                {
                    dicDispositPrice.Remove(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                    dicDispositQty.Remove(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());

                    dataGridView1.Rows.RemoveAt(e.RowIndex);
                    count -= 1;
                    UpdateValue();
                    updateDictionary();
                    if (isDeposit())
                    {
                        cbxPre.Checked = true;
                    }
                    else
                    {
                        cbxPre.Checked = false;
                        lblDeposit.Text = "0";
                    }
                }
            }
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
        }

        private void dataGridView1_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            clearinput();
        }

        public void clearinput()
        {
            lblTotal.Text = "";
            int rowCount = dataGridView1.Rows.Count;
            for (int i = 0; i < rowCount; i++)
            {
                dataGridView1.Rows.RemoveAt(0);
            }
            dicDiscount.Clear();
            dicOriginalQuan.Clear();
            dicPrice.Clear();
            dicquan.Clear();
            dicDispositDiscount.Clear();
            dicDispositPrice.Clear();
            dicDispositQty.Clear();
            lblDiscount.Text = "0";
            lblTotal.Text = "0";
            lblDeposit.Text = "0";
            lblMinDeposit.Text = "0";
            lblDiscount.Text = "0";
            cbxPre.Checked = false;
            count = 0;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isDeposit())
            {
                cbxPre.Checked = true;
            }
            else
            {
                cbxPre.Checked = false;
                lblDeposit.Text = "0";
            }
            UpdateValue();
            updateDictionary();
        }

        public void setStore(String store)
        {
            this.store = store;
        }

        private void UpdateValue()
        {
            double price;
            int quan = 0;
            double total = 0;
            double discount;
            try
            {
                for (int counter = 0; counter < (dataGridView1.Rows.Count);
                    counter++)
                {
                    if (dataGridView1[5, counter].Value.ToString() != "Deposit")
                    {
                        if (dataGridView1[2, counter].Value.ToString() == "")
                        {
                            MessageBox.Show("cannot input null value");
                            dataGridView1[2, counter].Value = Convert.ToInt32(dicquan[dataGridView1[0, counter].Value.ToString()]);
                            break;
                        }
                        else if (dataGridView1[3, counter].Value.ToString() == "")
                        {
                            MessageBox.Show("cannot input null value");
                            dataGridView1[3, counter].Value = Convert.ToDouble(dicPrice[dataGridView1[0, counter].Value.ToString()]);
                            break;
                        }
                        else if (dataGridView1[4, counter].Value.ToString() == "")
                        {
                            MessageBox.Show("cannot input null value");
                            dataGridView1[4, counter].Value = Convert.ToDouble(dicDiscount[dataGridView1[0, counter].Value.ToString()]);
                            break;
                        }
                        else if (Convert.ToInt32(dataGridView1[2, counter].Value) <= 0)
                        {
                            MessageBox.Show("cannot input negative or zero value");
                            dataGridView1[2, counter].Value = Convert.ToInt32(dicquan[dataGridView1[0, counter].Value.ToString()]);
                            break;
                        }
                        else if (Convert.ToDouble(dataGridView1[3, counter].Value) < 0)
                        {
                            MessageBox.Show("cannot input negative value");
                            dataGridView1[3, counter].Value = Convert.ToDouble(dicPrice[dataGridView1[0, counter].Value.ToString()]);
                            break;
                        }
                        else if (!(Convert.ToDouble(dataGridView1[4, counter].Value) >= 0 && Convert.ToDouble(dataGridView1[4, counter].Value) <= 100))
                        {
                            MessageBox.Show("the discount range is 0 - 100");
                            dataGridView1[4, counter].Value = Convert.ToDouble(dicDiscount[dataGridView1[0, counter].Value.ToString()]);
                            break;
                        }
                        else
                        {
                            quanSubtract = Convert.ToInt32(dicOriginalQuan[dataGridView1[0, counter].Value.ToString()]) - Convert.ToInt32(dataGridView1[2, counter].Value.ToString());
                            if (quanSubtract >= 0)
                            {
                                quan = Convert.ToInt32(dataGridView1[2, counter].Value.ToString());
                                price = Convert.ToDouble(dataGridView1[3, counter].Value.ToString());
                                discount = Convert.ToDouble(dataGridView1[4, counter].Value.ToString());
                                total = (quan * price) * (1 - discount / 100);
                                total = Convert.ToDouble(String.Format("{0:0.#}", total));
                                dataGridView1.Rows[counter].Cells[5].Value = total.ToString();
                            }
                            else
                            {
                                if (Convert.ToDouble(dicPrice[dataGridView1[0, counter].Value.ToString()]) >= 5000)
                                {
                                    if (!checkItemDeposit(dataGridView1[0, counter].Value.ToString()))
                                    {
                                        string message = "The quantity is more the store stock, are you want to make the deposit for after quantity?";
                                        string title = "make deposit";
                                        MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                                        DialogResult result = MessageBox.Show(message, title, buttons);
                                        if (result == DialogResult.Yes)
                                        {
                                            dataGridView1[2, counter].Value = Convert.ToInt32(dicquan[dataGridView1[0, counter].Value.ToString()]);
                                            dataGridView1.Rows.Add(dataGridView1[0, counter].Value.ToString(),
                                                dataGridView1[1, counter].Value.ToString(),
                                                1,
                                                Convert.ToDouble(dataGridView1[3, counter].Value.ToString()),
                                                0.0,
                                                "Deposit");
                                            updateDictionary();
                                            calculateTotal();
                                            cbxPre.Checked = true;
                                        }
                                        else
                                        {
                                            dataGridView1[2, counter].Value = Convert.ToInt32(dicquan[dataGridView1[0, counter].Value.ToString()]);
                                        }
                                    }
                                    else
                                    {
                                        dataGridView1[2, counter].Value = Convert.ToInt32(dicquan[dataGridView1[0, counter].Value.ToString()]);
                                        MessageBox.Show("you cannot add the deposit item");
                                    }
                                }
                                else
                                {
                                    dataGridView1[2, counter].Value = Convert.ToInt32(dicquan[dataGridView1[0, counter].Value.ToString()]);
                                    MessageBox.Show("The quantity is more the store stock");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dataGridView1[2, counter].Value.ToString() == "")
                        {
                            MessageBox.Show("cannot input null value");
                            dataGridView1[2, counter].Value = Convert.ToInt32(dicDispositQty[dataGridView1[0, counter].Value.ToString()]);
                            break;
                        }
                        else if (dataGridView1[3, counter].Value.ToString() == "")
                        {
                            MessageBox.Show("cannot input null value");
                            dataGridView1[3, counter].Value = Convert.ToDouble(dicDispositPrice[dataGridView1[0, counter].Value.ToString()]);
                            break;
                        }
                        else if (dataGridView1[4, counter].Value.ToString() == "")
                        {
                            MessageBox.Show("cannot input null value");
                            dataGridView1[4, counter].Value = Convert.ToDouble(dicDispositDiscount[dataGridView1[0, counter].Value.ToString()]);
                            break;
                        }
                        else if (Convert.ToInt32(dataGridView1[2, counter].Value) <= 0)
                        {
                            MessageBox.Show("cannot input negative or zero value");
                            dataGridView1[2, counter].Value = Convert.ToInt32(dicDispositQty[dataGridView1[0, counter].Value.ToString()]);
                            break;
                        }
                        else if (Convert.ToDouble(dataGridView1[3, counter].Value) < 0)
                        {
                            MessageBox.Show("cannot input negative value");
                            dataGridView1[3, counter].Value = Convert.ToDouble(dicDispositPrice[dataGridView1[0, counter].Value.ToString()]);
                            break;
                        }
                        else if (!(Convert.ToDouble(dataGridView1[4, counter].Value) >= 0 && Convert.ToDouble(dataGridView1[4, counter].Value) <= 100))
                        {
                            MessageBox.Show("the discount range is 0 - 100");
                            dataGridView1[4, counter].Value = Convert.ToDouble(dicDispositQty[dataGridView1[0, counter].Value.ToString()]);
                            break;
                        }
                    }
                }
                calculateTotal();
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

        public void calculateTotal()
        {
            totalQuan = 0;
            originalTotal = 0;
            total = 0;
            minDeposit = 0;
            maxDeposit = 0;
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                if (item.Cells[5].Value.ToString() != "Deposit")
                {
                    originalTotal += (Convert.ToDouble(item.Cells[2].Value.ToString()) * Convert.ToDouble(item.Cells[3].Value.ToString()));
                    total += Convert.ToDouble(item.Cells[5].Value.ToString());
                    totalQuan += Convert.ToInt32(item.Cells[2].Value.ToString());
                }
                else
                {

                    maxDeposit += (Convert.ToInt32(item.Cells[2].Value.ToString()) *
                        Convert.ToDouble(item.Cells[3].Value.ToString()) *
                        (1 - (Convert.ToDouble(item.Cells[4].Value.ToString()) / 100)));

                    if (maxDeposit >= 5000)
                    {
                        minDeposit += (Convert.ToInt32(item.Cells[2].Value.ToString()) *
                            Convert.ToDouble(item.Cells[3].Value.ToString()) *
                            (1 - (Convert.ToDouble(item.Cells[4].Value.ToString()) / 100)) *
                            0.2);
                    }

                }
            }
            lblMinDeposit.Text = Convert.ToDouble(String.Format("{0:0.#}", minDeposit)).ToString();
            lblTotal.Text = (total + Convert.ToDouble(lblDeposit.Text)).ToString();
            totalDiscount = Convert.ToDouble(String.Format("{0:0.#}", (originalTotal - total)));
            lblDiscount.Text = totalDiscount.ToString();
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            if (cbxPre.Checked)
            {
                if (Convert.ToDouble(lblDeposit.Text) >= Convert.ToDouble(lblMinDeposit.Text))
                {
                    if (dataGridView1.Rows.Count == 0)
                    {
                        MessageBox.Show("at least input one item");
                    }
                    else
                    {
                        remark = txtRemark.Text;
                        int OriginalQuan;
                        pageHeight = 560 + 60 * dataGridView1.Rows.Count;
                        String[,] order = new String[dataGridView1.Rows.Count, 7];
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {
                            for (int j = 0; j < 6; j++)
                            {
                                order[i, j] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                            }
                            if (order[i, 5] != "Deposit")
                            {
                                OriginalQuan = Convert.ToInt32(dicOriginalQuan[dataGridView1.Rows[i].Cells[0].Value.ToString()]);
                                order[i, 6] = (OriginalQuan - Convert.ToInt32(order[i, 2])).ToString();
                            }
                        }
                        Cash cash = new Cash();
                        cash.setPrice(Convert.ToDouble(lblTotal.Text));
                        cash.setOrder(order);
                        if (cbxPre.Checked)
                        {
                            cash.checkDeposit(true);
                            cash.setPaidDeposti(Convert.ToDouble(lblDeposit.Text));
                        }
                        else
                        {
                            cash.checkDeposit(false);
                        }
                        cash.ShowDialog();
                        if (cashAction == "cancell" || cashAction == "")
                        {
                            //
                        }
                        else if (cashAction == "confirm")
                        {
                            clearinput();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("the deposit must be more the minimal deposit");
                }
            }
            else
            {
                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("at least input one item");
                }
                else
                {
                    remark = txtRemark.Text;
                    int OriginalQuan;
                    pageHeight = 560 + 60 * dataGridView1.Rows.Count;
                    String[,] order = new String[dataGridView1.Rows.Count, 7];
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            order[i, j] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                        }
                        if (order[i, 5] != "Deposit")
                        {
                            OriginalQuan = Convert.ToInt32(dicOriginalQuan[dataGridView1.Rows[i].Cells[0].Value.ToString()]);
                            order[i, 6] = (OriginalQuan - Convert.ToInt32(order[i, 2])).ToString();
                        }
                    }
                    Cash cash = new Cash();
                    cash.setPrice(Convert.ToDouble(lblTotal.Text));
                    cash.setOrder(order);
                    if (cbxPre.Checked)
                    {
                        cash.checkDeposit(true);
                        cash.setPaidDeposti(Convert.ToDouble(lblDeposit.Text));
                    }
                    else
                    {
                        cash.checkDeposit(false);
                    }
                    cash.ShowDialog();
                    if (cashAction == "cancell" || cashAction == "")
                    {
                        //
                    }
                    else if (cashAction == "confirm")
                    {
                        clearinput();
                    }
                }
            }
        }

        public void updateDictionary()
        {
            quantityDictionary();
            priceDictionary();
            discountDictionary();

        }

        public void quantityDictionary()
        {
            dicquan.Clear();
            dicDispositQty.Clear();
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                if (item.Cells[5].Value.ToString() != "Deposit")
                {
                    dicquan.Add(item.Cells[0].Value.ToString(), item.Cells[2].Value.ToString());
                }
                else
                {
                    dicDispositQty.Add(item.Cells[0].Value.ToString(), Convert.ToInt32(item.Cells[2].Value.ToString()));
                }
            }
        }

        public void priceDictionary()
        {
            dicPrice.Clear();
            dicDispositPrice.Clear();
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                if (item.Cells[5].Value.ToString() != "Deposit")
                {
                    dicPrice.Add(item.Cells[0].Value.ToString(), item.Cells[3].Value.ToString());
                }
                else
                {
                    dicDispositPrice.Add(item.Cells[0].Value.ToString(), Convert.ToDouble(item.Cells[3].Value.ToString()));
                }
            }
        }

        public void discountDictionary()
        {
            dicDiscount.Clear();
            dicDispositDiscount.Clear();
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                if (item.Cells[5].Value.ToString() != "Deposit")
                {
                    dicDiscount.Add(item.Cells[0].Value.ToString(), item.Cells[4].Value.ToString());
                }
                else
                {
                    dicDispositDiscount.Add(item.Cells[0].Value.ToString(), Convert.ToDouble(item.Cells[4].Value.ToString()));
                }
            }
        }



        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.saleManagement();
            /*     StoreHomePage home = new StoreHomePage();
                 this.Hide();
                 home.ShowDialog();
                 this.Close(); */

        }

        private void btnDeposit_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_DataError_1(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("please input integer");
            dataGridView1.CancelEdit();
        }

        private void btnDeposit_Click_1(object sender, EventArgs e)
        {
            if (cbxPre.Checked)
            {
                Double mindeposit;
                Double deposit;
                Double maxdeposit = 0;
                try
                {
                    deposit = Convert.ToDouble(String.Format("{0:0.#}", Convert.ToDouble(txtDeposit.Text)));
                    mindeposit = Convert.ToDouble(lblMinDeposit.Text);
                    maxdeposit = maxDeposit;
                    if (mindeposit <= deposit)
                    {
                        if (maxdeposit >= deposit)
                        {
                            lblDeposit.Text = deposit.ToString();
                            calculateTotal();
                        }
                        else
                        {
                            MessageBox.Show("The deposit among cannot be more the total amoung on each deposit item");
                        }
                    }
                    else
                    {
                        MessageBox.Show("The deposit among must be more the minimal deposit");
                    }
                }
                catch
                {
                    MessageBox.Show("Please input correct number");
                }
            }
            else
            {
                MessageBox.Show("you cannot input the deposite the non-deposit order");
            }
        }

        private void lblMinDeposit_Click(object sender, EventArgs e)
        {

        }

        private void btnInvoice_Click(object sender, EventArgs e)
        {
            String order_id = "";
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            sqlConn.Open();
            sqlCmd.Connection = sqlConn; //get order id
            sqlCmd.CommandText = "select order_id from sales_receipt where invoice_id = '" + txtInvoice.Text + "'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                order_id = sqlRd.GetString(0);
            }
            sqlRd.Close();
            sqlConn.Close();

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select order_is_cancelled, order_is_preorder, order_is_complete from sales_order where order_id = '" + order_id + "'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                isCancel = sqlRd.GetString(0);
                ispreOrderl = sqlRd.GetString(1);
                ispreComplete = sqlRd.GetString(2);
            }
            sqlRd.Close();
            sqlConn.Close();

            bool hasStuck = true;
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select store_stock_qty - order_item_qty from order_item " +
                "inner join store_stock " +
                "on store_stock.item_id = order_item.item_id " +
                "where store_id = '" + store + "' and order_id = '" + order_id + "' and is_deposit ='y'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                if (sqlRd.GetInt32(0) < 0)
                {
                    hasStuck = false;
                    break;
                }
            }
            sqlRd.Close();
            sqlConn.Close();

            if (order_id != "")
            {
                if (isCancel != "y")
                {
                    if (ispreOrderl != "n")
                    {
                        if (ispreComplete != "y")
                        {
                            if (hasStuck)
                            {
                                Main.instance.preOrder(txtInvoice.Text);
                            }
                            else
                            {
                                MessageBox.Show("the stock still haven't enough quantity");
                            }
                        }
                        else
                        {
                            MessageBox.Show("The order already complete");
                        }
                    }
                    else
                    {
                        MessageBox.Show("the order is not a pre-order");
                    }
                }
                else
                {
                    MessageBox.Show("the order already cancel");
                }

            }
            else
            {
                MessageBox.Show("the invoice is is not found");
            }
        }

        public void setCashAction(String cashAction)
        {
            this.cashAction = cashAction;
        }

    }
}
