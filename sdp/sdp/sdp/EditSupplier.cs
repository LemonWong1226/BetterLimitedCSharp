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
    public partial class EditSupplier : Form
    {
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;

        private String selectRow;
        private string language;

        public EditSupplier()
        {
            InitializeComponent();
        }

        private void EditSupplier_Load(object sender, EventArgs e)
        {
            changeLanguage();
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            sqlConn.Open();
            sqlQuery = "select * from supplier where supplier_id = '" + selectRow + "'";
            sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                txtName.Text = sqlRd.GetString(1);
                txtEmail.Text = sqlRd.GetString(2);
                txtAddress.Text = sqlRd.GetString(4);
                txtPhone.Text = sqlRd.GetString(3);
                txtRemark.Text = sqlRd.GetString(5);
            }
            sqlRd.Close();
            sqlConn.Close();
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
                btxBack.Text = "返回";
                butDelete.Text = "删除";
                btnSubmit.Text = "提交";
            }
            else
            {
                label1.Text = "Supplier Name :";
                label2.Text = "Email :";
                label3.Text = "Address :";
                label4.Text = "Phone Number :";
                label5.Text = "Remarks :";
                btxBack.Text = "BACK";
                butDelete.Text = "DELETE";
                btnSubmit.Text = "SUBMIT";
            }
        }

        public void setSelectRow(String selectRow)
        {
            this.selectRow = selectRow;
        }

        private void butDelete_Click(object sender, EventArgs e)
        {
            string message = "are you want to delete the supplier?";
            string title = "delete item";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.Yes)
            {
                try
                {
                    sqlConn.Open();
                    sqlQuery = "DELETE FROM supplier WHERE supplier_id = '" + selectRow + "'";
                    sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    MessageBox.Show("Delete successfully");
                    Main.instance.supplier();
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
        }

        private void btxBack_Click(object sender, EventArgs e)
        {
            Main.instance.supplier();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtEmail.Text.IndexOf("@") != -1 && txtEmail.Text.IndexOf(".") != -1)
                {
                    if (txtPhone.Text.Length == 8)
                    {
                        if (checkphone())
                        {
                            if (txtAddress.Text != "")
                            {
                                if (txtName.Text != "")
                                {
                                    sqlConn.Open();

                                    sqlQuery = "update supplier " +
                                               "set supplier_name = '" + txtName.Text + "' , supplier_email = '" + txtEmail.Text + "' , supplier_address ='" +
                                               txtAddress.Text + "', supplier_phone = '" + txtPhone.Text + "', supplier_remarks = '" + txtRemark.Text + "' " +
                                               "where supplier_id = '" + selectRow + "'";
                                    Console.WriteLine(sqlQuery);
                                    sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                                    sqlCmd.ExecuteNonQuery();
                                    MessageBox.Show("Update successfully");
                                }
                                else
                                {
                                    MessageBox.Show("You cannot input null value on name");
                                }
                            }
                            else
                            {
                                MessageBox.Show("You cannot input null value on address");
                            }
                        }
                        else
                        {
                            MessageBox.Show("The phone number is incorrect format");
                        }
                    }
                    else
                    {
                        MessageBox.Show("The phone number must be 8 digital number");
                    }
                }
                else
                {
                    MessageBox.Show("Please input creect email address");
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("The phone number must be 8 digital number");
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
        public bool checkphone()
        {
            try
            {
                foreach (var a in txtPhone.Text)
                {
                    Convert.ToInt32(a.ToString());
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
