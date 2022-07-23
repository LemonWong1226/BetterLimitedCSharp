using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace sdp
{
    public partial class login : Form
    {
        String staff;
        String name;
        int position;
        int dept;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;


        public login()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            bool login = false;
            sqlConn.ConnectionString = ConnectString.ConnectionString;

            try
            {
                sqlConn.Open();
                sqlQuery = "SELECT user.user_id, user_password, staff_id, first_name, last_name, department, position " +
                    "FROM user inner join staff "+
                    "on staff.user_id = user.user_id";
                sqlCmd = new MySqlCommand(sqlQuery, sqlConn);
                sqlRd = sqlCmd.ExecuteReader();


                while (sqlRd.Read())
                {
                    if (textBox1.Text == sqlRd.GetString(0) && textBox2.Text == sqlRd.GetString(1))
                    {
                        name = sqlRd.GetString(3) + " " + sqlRd.GetString(4);
                        staff = sqlRd.GetString(2);
                        dept = sqlRd.GetInt32(5);
                        position = sqlRd.GetInt32(6);
                        login = true;
                        break;
                    }
                }

                if (login)
                {
                    MessageBox.Show("Login Success! ");
                    Main m = new Main();
                    m.setUser(textBox1.Text);
                    m.setStaff(staff);
                    m.setName(name);
                    m.setDept(dept);
                    m.setPosition(position);
                    m.Show();
                    this.Hide();
                }
                else if (textBox1.Text == "" || textBox2.Text == "")
                {
                    MessageBox.Show("The usernme or password cannot be null " +
                        "\n                      Please try again");
                }
                else
                {
                    MessageBox.Show("The user account or password is invalid " +
                            "\n                      Please try again");
                }

                sqlConn.Close();
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ForgetPassword forget = new ForgetPassword();
            this.Hide();
            forget.ShowDialog();
        }
    }
}
