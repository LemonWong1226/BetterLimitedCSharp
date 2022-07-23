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
    public partial class Supplier : Form
    {
        public static Supplier instancel;
        public String action;
        public String itemid;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;
        private string JSONlist;

        public Supplier()
        {
            InitializeComponent();
            instancel = this;
        }

        private void Supplier_Load(object sender, EventArgs e)
        {
            setPermission(Main.instance.dept, Main.instance.position);
            changeLanguage();
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            if (language == "Chinese")
            {
                sqlCmd.CommandText = "select supplier_id as '供应商号', supplier_name as '供应商名称', supplier_email as '供应商电邮' from supplier";
            }
            else
            {
                sqlCmd.CommandText = "select supplier_id as 'Supplier Id', supplier_name as 'Supplier Name', supplier_email as 'Supplier Email' from supplier";
            }
            sqlRd = sqlCmd.ExecuteReader();
            sqlDt.Load(sqlRd);
            sqlRd.Close();
            sqlConn.Close();
            dataGridView1.DataSource = sqlDt;

            if(action == "addinventory")
            {
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                btnAdd.Visible = false;
                btnEdit.Visible = false;
                btnDetails.Visible = false;
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
                    case 19:
                        btnAdd.Visible = true;
                        break;
                    case 18:
                        btnEdit.Visible = true;
                        break;
                    case 17:
                        btnDetails.Visible = true;
                        break;
                }
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
                btnback.Text = "返回";
                btnAdd.Text = "新增";
                btnEdit.Text = "编辑";
                btnDetails.Text = "详情";
            }
            else
            {
                btnback.Text = "BACK";
                btnAdd.Text = "Add New";
                btnEdit.Text = "Edit Item";
                btnDetails.Text = "More Details";
            }
        }

        private void btnback_Click(object sender, EventArgs e)
        {
            if(action == "addinventory")
            {
                this.Close();
            }
            else
            {
                Main.instance.Inventory();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Main.instance.addSupplier();
        }

        public void setAction(String action)
        {
            this.action = action;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Int32 selectedRowCount =
                dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                itemid = (dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
            }
            Main.instance.EditSupplier();
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            Int32 selectedRowCount =
    dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (selectedRowCount > 0)
            {
                itemid = (dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
            }
            Main.instance.SupplierDetail();
        }
    }
}
