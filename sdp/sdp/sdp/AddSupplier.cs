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
    public partial class AddSupplier : Form
    {
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        String sqlQuery;
        private string language;

        public AddSupplier()
        {
            InitializeComponent();
        }

        private void AddSupplier_Load(object sender, EventArgs e)
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
                label1.Text = "供应商名称 :";
                label2.Text = "电邮 :";
                label3.Text = "地址 :";
                label4.Text = "电话号码 :";
                label5.Text = "备注 :";
                btxBack.Text = "返回";
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
                btnSubmit.Text = "SUBMIT";
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            sqlConn.ConnectionString = ConnectString.ConnectionString;

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

                                sqlQuery = "insert into supplier values(Default, '" + txtName.Text + "','" + txtEmail.Text + "', '" + 
                                    txtPhone.Text + "','" + txtAddress.Text + "', '" + txtRemark.Text + "')";
                                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                                sqlCmd.ExecuteNonQuery();
                                MessageBox.Show("Successfully to add the supplier");
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
                    MessageBox.Show("Please input correct email address");
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

        private void btxBack_Click(object sender, EventArgs e)
        {
            Main.instance.supplier();
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
