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
using System.Text.RegularExpressions;

namespace sdp
{
    public partial class CreateAccount : Form
    {
        public String store;
        public String position;
        public String dept;
        public String deptNumber;
        public String language;

        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;

        public CreateAccount()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.Setting();
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
                btnBack.Text = "返回 :";
                button1.Text = "提交 :";
                label1.Text = "用户名称 :";
                label2.Text = "密码 :";
                label8.Text = "确认密码 :";
                label4.Text = "名字 :";
                label3.Text = "姓氏 :";
                label9.Text = "员工电邮 :";
                label5.Text = "部门 :";
                label6.Text = "岗位 :";
                label7.Text = "店铺";
                cbxDept.Items.Add("销售");
                cbxDept.Items.Add("仓库");
                cbxDept.Items.Add("技术");
                cbxDept.Items.Add("会计");
                cbxDept.Items.Add("采购");
                cbxDept.Items.Add("管理员");
                cbxPosition.Items.Add("文员/售货员/工人");
                cbxPosition.Items.Add("经理");
                cbxPosition.Items.Add("管理员");
            }
            else
            {
                btnBack.Text = "BACK";
                button1.Text = "SUBMIT";
                label1.Text = "User Name:";
                label2.Text = "Password :";
                label8.Text = "Confirm Password :";
                label4.Text = "First Name :";
                label3.Text = "Last Name :";
                label9.Text = "Staff Email :";
                label5.Text = "Department :";
                label6.Text = "Position :";
                label7.Text = "Store :";
                cbxDept.Items.Add("Sale");
                cbxDept.Items.Add("Inventory");
                cbxDept.Items.Add("Technical");
                cbxDept.Items.Add("Accounting");
                cbxDept.Items.Add("Purchase");
                cbxDept.Items.Add("Admin");
                cbxPosition.Items.Add("Clerk / Representative / Worker");
                cbxPosition.Items.Add("Manager");
                cbxPosition.Items.Add("Admin");

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkUser())
            {
                if(checkPassword(txtPassword.Text, txtConfirm.Text))
                {
                    if(checkFullName(txtFirst.Text, txtLast.Text))
                    {
                        if (checkEmail(txtEmail.Text))
                        {
                            if (DeptToAplha())
                            {
                                if (PositionToAplha())
                                {
                                    if (checkSelectStore())
                                    {
                                        if (lblValue.Text == "valid")
                                        {
                                            try
                                            {
                                                ConnectString rootconn = new ConnectString("root", "123456");
                                                sqlConn.ConnectionString = rootconn.getString();

                                                int number = 1;
                                                string deptId = String.Format(dept + "{0:D4}", number);
                                                sqlConn.Open();
                                                sqlQuery = "SELECT * FROM staff where staff_id like '" + dept + "%' order by staff_id";
                                                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                                                sqlRd = sqlCmd.ExecuteReader();
                                                while (sqlRd.Read())
                                                {
                                                    if (sqlRd.GetString(0) == deptId)
                                                    {
                                                        number++;
                                                        deptId = String.Format(dept + "{0:D4}", number);
                                                    }
                                                    else
                                                    {
                                                        break; ;
                                                    }
                                                }
                                                sqlRd.Close();
                                                sqlConn.Close();

                                                sqlConn.Open();
                                                sqlCmd.Connection = sqlConn;
                                                sqlCmd.CommandText = "insert into user values('" + txtName.Text + "','" + txtPassword.Text + "','" +
                                                    txtFirst.Text + "','" + txtLast.Text + "',default)";
                                                Console.WriteLine(sqlCmd.CommandText);
                                                sqlCmd.ExecuteNonQuery();
                                                sqlConn.Close();

                                                sqlConn.Open();
                                                sqlCmd.Connection = sqlConn;
                                                sqlCmd.CommandText = "insert into staff values('" + deptId + "','" + deptNumber + "','" +
                                                    position + "','y','" + txtName.Text + "','" + store + "',default,'" + txtEmail.Text + "')";
                                                sqlCmd.ExecuteNonQuery();

                                                MessageBox.Show("successful to create the user");
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
                                        else
                                        {
                                            MessageBox.Show("The password is invalid");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }          
        }

        public Boolean checkSelectStore()
        {
            if (rbnHK01.Checked || rbnHK02.Checked)
            {
                if (rbnHK01.Checked)
                {
                    store = "HK01";
                    return true;
                }
                else
                {
                    store = "HK02";
                    return true;
                }
            }
            else
            {
                MessageBox.Show("please select a store");
                return false;
            }
        }

        public Boolean DeptToAplha()
        {
            switch (cbxDept.Text)
            {
                case "Sale":
                    dept = "S";
                    deptNumber = "1";
                    return true;
                case "Inventory":
                    dept = "I";
                    deptNumber = "2";
                    return true;
                case "Technical":
                    dept = "T";
                    deptNumber = "3";
                    return true;
                case "Accounting":
                    dept = "A";
                    deptNumber = "4";
                    return true;
                case "Purchase":
                    dept = "P";
                    deptNumber = "5";
                    return true;
                case "Admin":
                    dept = "Z";
                    deptNumber = "99";
                    return true;
                case "销售":
                    dept = "S";
                    deptNumber = "1";
                    return true;
                case "仓库":
                    dept = "I";
                    deptNumber = "2";
                    return true;
                case "技术":
                    dept = "T";
                    deptNumber = "3";
                    return true;
                case "会计":
                    dept = "A";
                    deptNumber = "4";
                    return true;
                case "采购":
                    dept = "P";
                    deptNumber = "5";
                    return true;
                case "管理员":
                    dept = "Z";
                    deptNumber = "99";
                    return true;
                default :
                    MessageBox.Show("please select an department");
                    return false;
            }
        }

        public Boolean PositionToAplha()
        {
            switch (cbxPosition.Text)
            {
                case "Clerk/Representative/Worker":
                    position = "1";
                    return true;
                case "Manager":
                    position = "2";
                    return true;
                case "Admin":
                    position = "99";
                    return true;
                case "文员/售货员/工人":
                    position = "1";
                    return true;
                case "经理":
                    position = "2";
                    return true;
                case "管理员":
                    position = "99";
                    return true;
                default:
                    MessageBox.Show("please select an position");
                    return false;
            }
        }

        public Boolean checkUser()
        {
            if (txtName.Text != "")
            {
                Boolean exist = false;
                ConnectString rootconn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = rootconn.getString();
                sqlConn.Open();
                sqlQuery = "select user_id from user where user_id = '"+txtName.Text+"';";
                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    exist = true;
                }
                sqlRd.Close();
                sqlConn.Close();

                if (exist)
                {
                    MessageBox.Show("The user id already exist");
                    return false;
                }
                {
                    return true;
                }
            }
            else
            {
                MessageBox.Show("The user id cannot null");
                return false;
            }
        }

        public Boolean checkPassword(String password, String Confirm)
        {
            if(password != "")
            {
                if(Confirm != "")
                {
                    if(password == Confirm)
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("The password and confirm password is not match, try again");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("The confirm password cannot null");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("The password cannot null");
                return false;
            }
        }

        public Boolean checkFullName(String first, String last)
        {
            if(first != "")
            {
                if(last != "")
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("The last name cannot null");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("The first name cannot null");
                return false;
            }
        }

        public Boolean checkEmail(String Email)
        {
            if(Email != "")
            {
                if(Email.IndexOf("@") != -1 && Email.IndexOf(".") != -1 && Email.IndexOf(".") > Email.IndexOf("@"))
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Please input correct format on your email");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("The Email cannot null");
                return false;
            }
        }

        private void CreateAccount_Load(object sender, EventArgs e)
        {
            changeLanguage();
            lblValue.Text = "invalid(you must match below all condition)\n" +
    "The password must have letter\n" +
    "The password must have number\n" +
    "The password at least 6 length";
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            Regex nubmer = new Regex(@"[0-9]");
            Regex literal = new Regex(@"[a-z]|[A-Z]");
            if (nubmer.IsMatch(txtPassword.Text) && literal.IsMatch(txtPassword.Text) && txtPassword.Text.Length >= 6)
                lblValue.Text = "valid";
            else
            {
                lblValue.Text = "invalid(you must match below all condition)\n" +
                    "The password must have letter\n" +
                    "The password must have number\n" +
                    "The password at least 6 length";
            }
        }
    }
}
