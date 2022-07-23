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
    public partial class Statement : Form
    {
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string store;
        private string language;

        public Statement()
        {
            InitializeComponent();
        }

        private void Statement_Load(object sender, EventArgs e)
        {
            changeLanguage();
            dtp1.MinDate = new DateTime(1985, 6, 20);
            dtp1.CustomFormat = "yyyy-MM-dd";
            dtp1.Format = DateTimePickerFormat.Custom;
            dtp2.MinDate = new DateTime(1985, 6, 20);
            dtp2.CustomFormat = "yyyy-MM-dd";
            dtp2.Format = DateTimePickerFormat.Custom;
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
                label2.Text = "日期:";
                label4.Text = "至";
                btnSubmit.Text = "提交";
                label3.Text = "年度 :";
                btnBack.Text = "返回";
                button1.Text = "提交";
            }
            else
            {
                label2.Text = "Date:";
                label4.Text = "To";
                btnBack.Text = "BACK";
                btnSubmit.Text = "SUBMIT";
                label3.Text = "Year :";
                button1.Text = "SUBMIT";
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            chart2.Visible = false;
            chart1.Series["Series1"].Points.Clear();
            double shop1Total = 0;
            double shop2Total = 0;
            sqlConn.ConnectionString = ConnectString.ConnectionString;

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select store_id, order_ttl_amount, order_deposit,order_is_complete " +
                "from sales_order " +
                "WHERE last_update >= '" + dtp1.Text + " 0:0:0' and last_update <= '" + dtp2.Text + " 23:59:59'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                switch (sqlRd.GetString(0))
                {
                    case "HK01":
                        if (sqlRd.GetString(3) == "y")
                            shop1Total += sqlRd.GetDouble(1);
                        else
                            shop1Total += sqlRd.GetDouble(1) - sqlRd.GetDouble(2);
                        break;
                    case "HK02":
                        if (sqlRd.GetString(3) == "y")
                            shop2Total += sqlRd.GetDouble(1);
                        else
                            shop2Total += sqlRd.GetDouble(1) - sqlRd.GetDouble(2); break;
                }
            }
            sqlRd.Close();
            sqlConn.Close();
            chart1.Series["Series1"].Points.AddXY("HK01", shop1Total);
            chart1.Series["Series1"].Points.AddXY("HK02", shop2Total);
            chart1.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                chart1.Visible = false;
                chart2.Series["HK01"].Points.Clear();
                chart2.Series["HK02"].Points.Clear();
                chart2.Visible = true;
                String month;
                double[] yearShop1Salary = new double[12];
                double[] yearShop2Salary = new double[12];
                int year = 0;
                year = Convert.ToInt32(txtQuantity.Text);
                for (int i = 0; i < 12; i++)
                {
                    if (i < 10)
                        month = 0 + (i + 1).ToString();
                    else
                        month = (i + 1).ToString();
                    try
                    {
                        sqlConn.ConnectionString = ConnectString.ConnectionString;
                        sqlConn.Open();
                        sqlCmd.Connection = sqlConn;
                        sqlCmd.CommandText = "select store_id, order_ttl_amount, order_deposit,order_is_complete " +
                            "from sales_order " +
                            "WHERE last_update like '" + year + "-" + month + "%';";
                        sqlRd = sqlCmd.ExecuteReader();
                        while (sqlRd.Read())
                        {
                            switch (sqlRd.GetString(0))
                            {
                                case "HK01":
                                    if (sqlRd.GetString(3) == "y")
                                        yearShop1Salary[i] += sqlRd.GetDouble(1);
                                    else
                                        yearShop1Salary[i] += sqlRd.GetDouble(1) - sqlRd.GetDouble(2);
                                    break;
                                case "HK02":
                                    if (sqlRd.GetString(3) == "y")
                                        yearShop2Salary[i] += sqlRd.GetDouble(1);
                                    else
                                        yearShop2Salary[i] += sqlRd.GetDouble(1) - sqlRd.GetDouble(2);
                                    break;
                            }
                        }
                        chart2.Series["HK01"].Points.AddXY(i + 1, yearShop1Salary[i]);
                        chart2.Series["HK02"].Points.AddXY(i + 1, yearShop2Salary[i]);
                        sqlRd.Close();
                        sqlConn.Close();
                    }
                    catch (Exception ex)
                    {
                        chart2.Visible = false;
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        sqlRd.Close();
                        sqlConn.Close();
                    }
                }
            }
            catch (FormatException)
            {
                chart2.Visible = false;
                MessageBox.Show("Please input correct year");
            }
            catch (Exception ex)
            {
                chart2.Visible = false;
                MessageBox.Show(ex.Message);
            }
        }

        public void setStore(String store)
        {
            this.store = store;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.shop1(store);
        }
    }
}
