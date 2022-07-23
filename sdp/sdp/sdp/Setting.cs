using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace sdp
{
    public partial class Setting : Form
    {
        String language;
        String action;
        String password;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string JSONlist;

        public Setting()
        {
            InitializeComponent();
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
                btnBack.Text = "返回";
                btnChangeName.Text = "更改名称";
                btnChange.Text = "更改密码";
                btnCreate.Text = "建立新用户";
                btnLanguage.Text = "语言";
                btnSubmit.Text = "更改";
                btnChangePassword.Text = "更改";
                lblName.Text = "用户名称:";
                lblNew.Text = "新密码:";
                lblold.Text = "旧密码:";
                lblConfirm.Text = "确认新密码:";
            }
            else
            {
                btnBack.Text = "BACK";
                btnChangeName.Text = "Change Name";
                btnChange.Text = "Change Password";
                btnCreate.Text = "Create Account";
                btnLanguage.Text = "Language";
                btnSubmit.Text = "SUBMIT";
                btnChangePassword.Text = "CHANGE";
                lblName.Text = "USER NAME:";
                lblNew.Text = "NEW PASSWORD:";
                lblold.Text = "OLD PASSWORD:";
                lblConfirm.Text = "CONFIRM NEW PASSWORD:";
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
                    case 67:
                        btnChange.Visible = true;
                        break;
                    case 64:
                        btnChangeName.Visible = true;
                        break;
                    case 66:
                        btnLanguage.Visible = true;
                        break;
                    case 65:
                        btnCreate.Visible = true;
                        break;
                }
            }
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (language == "Chinese")
                Main.instance.setLable("更改密码");
            else
                Main.instance.setLable("Change Password");
            action = "changePassword";
            txtConfirm.Visible = true;
            txtNew.Visible = true;
            txtOld.Visible = true;
            lblValue.Visible = true;
            lblValue.Text = "invalid(you must match below all condition)\n" +
    "The password must have letter\n" +
    "The password must have number\n" +
    "The password at least 6 length";
            lblConfirm.Visible = true;
            lblNew.Visible = true;
            lblold.Visible = true;
            btnChangeName.Visible = false;
            btnChange.Visible = false;
            btnCreate.Visible = false;
            btnLanguage.Visible = false;
            btnChangePassword.Visible = true;
            btnBack.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)  //create account
        {
            Main.instance.CreateAccount();
        }

        private void button2_Click(object sender, EventArgs e) //language
        {
            if (language == "Chinese")
                Main.instance.setLable("语言");
            else
                Main.instance.setLable("Language");
            action = "language";
            btnChangeName.Visible = false;
            btnChange.Visible = false;
            btnCreate.Visible = false;
            btnLanguage.Visible = false;
            btnEnglish.Visible = true;
            btnChinese.Visible = true;
            btnBack.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            try
            {
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "SELECT user_password FROM user where user_id = '" + Main.instance.user + "'";
                sqlRd = sqlCmd.ExecuteReader();
                while (sqlRd.Read())
                {
                    password = sqlRd.GetString(0);
                }
                sqlRd.Close();
                sqlConn.Close();
                if (txtOld.Text == password)
                {
                    if (txtNew.Text == txtConfirm.Text)
                    {
                        if (lblValue.Text == "valid")
                        {
                            sqlConn.Open();
                            sqlCmd.Connection = sqlConn;
                            sqlCmd.CommandText = "update user set user_password = '" + txtNew.Text + "' where user_id = '" + Main.instance.user + "'";
                            sqlCmd.ExecuteNonQuery();
                            sqlConn.Close();
                            MessageBox.Show("Successful to change password");
                        }
                        else
                        {
                            MessageBox.Show("The password is invalid");
                        }
                    }
                    else
                    {
                        MessageBox.Show("The password is not match");
                    }
                }
                else
                {
                    MessageBox.Show("The old password is incorrect");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConn.Close();
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();
            try
            {
                if (txtOld.Text == "")
                {
                    MessageBox.Show("The user name is not null");
                }
                else if(Main.instance.user == txtOld.Text)
                {
                    MessageBox.Show("The user is not same");
                }
                else
                {
                    sqlConn.Open();
                    sqlCmd.Connection = sqlConn;
                    sqlCmd.CommandText = "update user set user_id = '" + txtOld.Text + "' where user_id = '" + Main.instance.user + "'";
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();
                    MessageBox.Show("Successful to change the name");
                    Main.instance.setUser(txtOld.Text);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("PRIMARY") != -1)
                {
                    MessageBox.Show("The user already exist");
                }
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }
            finally
            {
                sqlConn.Close();
            }
        }

        private void btnChangeName_Click(object sender, EventArgs e)
        {
            if (language == "Chinese")
                Main.instance.setLable("更改名称");
            else
                Main.instance.setLable("Change Name");
            action = "changename";
            txtOld.UseSystemPasswordChar = false;
            btnChangeName.Visible = false;
            btnChange.Visible = false;
            btnCreate.Visible = false;
            btnSubmit.Visible = true;
            btnLanguage.Visible = false;
            lblName.Visible = true;
            txtOld.Visible = true;
            btnBack.Visible = true;
        }

        private void button1_Click_1(object sender, EventArgs e) //english
        {
            try
            {
                if (File.Exists(@"Language.txt"))
                {
                    File.Delete(@"Language.txt");
                }

                using (FileStream fs = File.Create(@"Language.txt"))
                {
                    byte[] language = new UTF8Encoding(true).GetBytes("English");
                    fs.Write(language, 0, language.Length);
                    fs.Close();
                }
                changeLanguage();
                Main.instance.changeLanguage();
                Main.instance.setLable("Setting");
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

        private void button2_Click_1(object sender, EventArgs e) //simplifed chinese
        {
            try
            {
                if (File.Exists(@"Language.txt"))
                {
                    File.Delete(@"Language.txt");
                }

                using (FileStream fs = File.Create(@"Language.txt"))
                {
                    byte[] language = new UTF8Encoding(true).GetBytes("Chinese");
                    fs.Write(language, 0, language.Length);
                    fs.Close();
                }
                changeLanguage();
                Main.instance.setLable("设定");
                Main.instance.changeLanguage();
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (language == "Chinese")
                Main.instance.setLable("设定");
            else
                Main.instance.setLable("Setting");
            switch (action)
            {
                case "changePassword":
                    txtConfirm.Visible = false;
                    txtNew.Visible = false;
                    txtOld.Visible = false;
                    lblConfirm.Visible = false;
                    lblNew.Visible = false;
                    lblold.Visible = false;
                    setPermission(Main.instance.dept, Main.instance.position);
                    btnChangePassword.Visible = false;
                    btnBack.Visible = false;
                    lblValue.Visible = false;
                    lblValue.Text = "invalid(you must match below all condition)\n" +
                        "The password must have letter\n" +
                        "The password must have number\n" +
                        "The password at least 6 length";
                    break;
                case "language":
                    setPermission(Main.instance.dept, Main.instance.position);
                    btnEnglish.Visible = false;
                    btnChinese.Visible = false;
                    btnBack.Visible = false;
                    break;
                case "changename":
                    setPermission(Main.instance.dept, Main.instance.position);
                    btnSubmit.Visible = false;
                    lblName.Visible = false;
                    lblold.Visible = false;
                    txtOld.Visible = false;
                    btnBack.Visible = false;
                    txtOld.UseSystemPasswordChar = true;
                    break;

            }
            txtOld.Clear();
            txtNew.Clear();
            txtConfirm.Clear();
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            setPermission(Main.instance.dept, Main.instance.position);
            changeLanguage();
        }

        private void txtNew_TextChanged(object sender, EventArgs e)
        {
            Regex nubmer = new Regex(@"[0-9]");
            Regex literal = new Regex(@"[a-z]|[A-Z]");
            if (nubmer.IsMatch(txtNew.Text) && literal.IsMatch(txtNew.Text) && txtNew.Text.Length >= 6)
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
