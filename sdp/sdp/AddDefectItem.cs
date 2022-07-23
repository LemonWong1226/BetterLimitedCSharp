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
    public partial class AddDefectItem : Form
    {
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;

        public AddDefectItem()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkId(txtItemId.Text))
                {
                    if (checkInvoiceId(txtInvoiceId.Text))
                    {
                        if (checkQty(txtQty.Text))
                        {
                            if (checkStaff(txtStaffId.Text))
                            {
                                if (checkName(txtCustomerName.Text))
                                {
                                    if (checkphone(txtCustomerPhone.Text))
                                    {
                                        ConnectString conn = new ConnectString("root", "123456");
                                        sqlConn.ConnectionString = conn.getString();
                                        sqlConn.Open();
                                        sqlQuery = "insert into good_return_note values(default, " +
                                            "'"+txtItemId.Text+"' , " +
                                            ""+txtQty.Text+", " +
                                            ""+txtInvoiceId.Text+", " +
                                            "default, " +
                                            "'"+txtRemarks.Text+"', " +
                                            "'"+txtStaffId.Text+"', " +
                                            "'"+txtCustomerName.Text+"', " +
                                            "'"+txtCustomerPhone.Text+"', " +
                                            "1)";
                                        sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                                        sqlCmd.ExecuteNonQuery();
                                        sqlConn.Close();
                                        MessageBox.Show("successful to add the defect item");
                                    }
                                }
                            }
                        }
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConn.Close();
            }

        }

        public bool checkId(String itemid)
        {
            try
            {
                sqlConn.ConnectionString = ConnectString.ConnectionString;
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select item_id from inventory";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    if (itemid == sqlRd.GetString(0))
                    {
                        return true;
                    }
                }
                sqlRd.Close();
                sqlConn.Close();
                MessageBox.Show("the item id was not found");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public bool checkInvoiceId(String invoiceId)
        {
            try
            {
                sqlConn.ConnectionString = ConnectString.ConnectionString;
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select invoice_id from sales_receipt";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    if (invoiceId == sqlRd.GetString(0))
                    {
                        return true;
                    }
                }
                sqlRd.Close();
                sqlConn.Close();
                MessageBox.Show("the invoice id was not found");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        public bool checkQty(String qty)
        {
            try
            {
                if (Convert.ToInt32(qty) < 0)
                {
                    MessageBox.Show("the quantity cannot input negative values");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("please input correct integer on the quantity");
                return false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool checkphone(String phone)
        {
            try
            {
                if (phone.Length == 8)
                {
                    foreach (var a in phone)
                    {
                        Convert.ToInt32(a.ToString());
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show("the phone number must be 8 digital");
                    return false;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("please input correct format on the phone number");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool checkName(String name)
        {
            if(name == "")
            {
                MessageBox.Show("the customer name cannot null");
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool checkStaff(String staff)
        {
            try
            {
                sqlConn.ConnectionString = ConnectString.ConnectionString;
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select staff_id from staff";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    if (staff == sqlRd.GetString(0))
                    {
                        return true;
                    }
                }
                sqlRd.Close();
                sqlConn.Close();
                MessageBox.Show("the staff id was not found");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            finally
            {
                sqlConn.Close();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.DefectItem();
        }

        private void AddDefectItem_Load(object sender, EventArgs e)
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
                label2.Text = "货品号 :";
                label3.Text = "发票号 :";
                label4.Text = "退回数量 :";
                label5.Text = "员工编号 :";
                label8.Text = "客户名称 :";
                label1.Text = "客户电话 :";
                btnBack.Text = "返回";
                label7.Text = "备注:";
                btnSubmit.Text = "提交";
            }
            else
            {
                label2.Text = "Item Id :";
                label3.Text = "Sale Invoice Id :";
                label4.Text = "Return Quantity :";
                label5.Text = "Staff Id :";
                label8.Text = "Customer Name :";
                label1.Text = "Customer Phone :";
                btnBack.Text = "BACK";
                label7.Text = "Remarks:";
                btnSubmit.Text = "SUBMIT";
            }
        }
    }
}
