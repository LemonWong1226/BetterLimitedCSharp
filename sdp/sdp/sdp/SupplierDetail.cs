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
    public partial class SupplierDetail : Form
    {
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;

        public String selectRow;
        private string language;

        public SupplierDetail()
        {
            InitializeComponent();
        }

        public void setSelectRow(String selectRow)
        {
            this.selectRow = selectRow;
        }

        private void SupplierDetail_Load(object sender, EventArgs e)
        {
            changeLanguage();
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            try
            {
                sqlConn.Open();
                sqlQuery = "select * from supplier where supplier_id = '" + selectRow + "'";
                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    lblId.Text = sqlRd.GetString(0);
                    lblName.Text = sqlRd.GetString(1);
                    lblEmail.Text = sqlRd.GetString(2);
                    lblAddress.Text = sqlRd.GetString(4);
                    lblPhone.Text = sqlRd.GetString(3);
                    lblRemark.Text = sqlRd.GetString(5);
                }
                sqlRd.Close();
                sqlConn.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                label1.Text = "供应商名称 :";
                label2.Text = "电邮 :";
                label3.Text = "地址 :";
                label4.Text = "电话号码 :";
                label5.Text = "备注 :";
                label6.Text = "供应商号 :";
                btxBack.Text = "返回";
            }
            else
            {
                label1.Text = "Supplier Name :";
                label2.Text = "Email :";
                label3.Text = "Address :";
                label4.Text = "Phone Number :";
                label5.Text = "Remarks :";
                label6.Text = "Supplier Id :";
                btxBack.Text = "BACK";
            }
        }

        private void btxBack_Click(object sender, EventArgs e)
        {
            Main.instance.supplier();
        }
    }
}
