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
    public partial class Inventory : Form
    {
        public String itemid;
        public static Inventory instance;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        Dictionary<string, string> alarm = new Dictionary<string, string>();
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;
        private string JSONlist;

        public Inventory()
        {
            InitializeComponent();
            instance = this;
        }

        private void Inventory_Load(object sender, EventArgs e)
        {
            setPermission(Main.instance.dept, Main.instance.position);
            changeLanguage();
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            if (language == "Chinese")
                sqlCmd.CommandText = "SELECT item_id as '货品号', item_name as '货品名称', inventory_qty as '仓库数量', item_remarks as '货品备注' FROM inventory";
            else
                sqlCmd.CommandText = "SELECT item_id as 'Item Id', item_name as 'Item Name', inventory_qty as 'Inventory Quantity', item_remarks as 'Item Remarks' FROM inventory";
            sqlRd = sqlCmd.ExecuteReader();
            sqlDt.Load(sqlRd);
            sqlRd.Close();
            sqlConn.Close();
            dataGridView1.DataSource = sqlDt;
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
                button1.Text = "新增货品";
                button2.Text = "编辑货品";
                button3.Text = "货品详情";
                button5.Text = "退货";
                button6.Text = "供应商";
            }
            else
            {
                button1.Text = "Add Item";
                button2.Text = "Edit Item";
                button3.Text = "Item Details";
                button5.Text = "Defect Items";
                button6.Text = "Supplier";
            }
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
                    case 28:
                        button1.Visible = true;
                        break;
                    case 27:
                        button2.Visible = true;
                        break;
                    case 26:
                        button3.Visible = true;
                        break;
                    case 25:
                        button5.Visible = true;
                        break;
                    case 20:
                        button6.Visible = true;
                        break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Main.instance.AddInventory();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            seleceRow();
            Main.instance.EditInventory();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            seleceRow();
            Main.instance.InventoryDetail();
        }

        public void seleceRow()
        {
            Int32 selectedRowCount =
                  dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                itemid = (dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Main.instance.supplier();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            seleceRow();
            Main.instance.InventoryDetail();
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            Main.instance.supplier();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Main.instance.DefectItem();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            alarm.Clear();
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select item_id, item_alarm from inventory";
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
                int qtyEntered = Convert.ToInt32(row.Cells[2].Value);
                if (Convert.ToInt32(alarm[row.Cells[0].Value.ToString()]) >= Convert.ToInt32(row.Cells[2].Value))
                {
                    if (qtyEntered == 0)
                    {
                        dataGridView1[2, count].Style.ForeColor = Color.Red;//to color the row
                    }
                    else
                    {
                        dataGridView1[2, count].Style.ForeColor = Color.Orange;//to color the row
                    }
                }
                count++;
            }
        }

        private void dataGridView1_Sorted(object sender, EventArgs e)
        {

        }
    }
}
