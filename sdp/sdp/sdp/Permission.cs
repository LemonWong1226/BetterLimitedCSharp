using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.IO;

namespace sdp
{
    public partial class Permission : Form
    {
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;

        private String JSONlist;
        List<int> permission = new List<int>();
        private string language;

        public Permission()
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
                label8.Text = "部门 :";
                label1.Text = "岗位 :";
                label2.Text = "类别 :";
                btnReset.Text = "重设";
                btnSubmit.Text = "提交";
                comboBox1.Items.Add("销售");
                comboBox1.Items.Add("仓库");
                comboBox1.Items.Add("技术");
                comboBox1.Items.Add("会计");
                comboBox1.Items.Add("采购");
                comboBox1.Items.Add("管理员");
                comboBox2.Items.Add("文员/售货员/工人");
                comboBox2.Items.Add("经理");
                comboBox2.Items.Add("管理员");
                cobType.Items.Add("销售管理");
                cobType.Items.Add("仓库");
                cobType.Items.Add("送货");
                cobType.Items.Add("采购");
                cobType.Items.Add("技术支援");
                cobType.Items.Add("会计");
                cobType.Items.Add("通知");
                cobType.Items.Add("设定");
                cobType.Items.Add("权限");
            }
            else
            {
                label8.Text = "Department :";
                label1.Text = "Position :";
                label2.Text = "Type :";
                btnReset.Text = "RESET";
                btnSubmit.Text = "SUBMIT";
                comboBox1.Items.Add("Sales / Store");
                comboBox1.Items.Add("Inventory");
                comboBox1.Items.Add("Technical");
                comboBox1.Items.Add("Accounting");
                comboBox1.Items.Add("Purchase");
                comboBox1.Items.Add("Admin");
                comboBox2.Items.Add("Clerk / Representative / Worker");
                comboBox2.Items.Add("Manager");
                comboBox2.Items.Add("Admin");
                cobType.Items.Add("Sale Management");
                cobType.Items.Add("Inventory");
                cobType.Items.Add("Delivery");
                cobType.Items.Add("Purchase");
                cobType.Items.Add("Technical Support");
                cobType.Items.Add("Account");
                cobType.Items.Add("Notification");
                cobType.Items.Add("Setting");
                cobType.Items.Add("Permission");
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "" && comboBox2.Text != "")
            {
                checkedToList();
                ConnectString rootconn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = rootconn.getString();
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "update group_permission set permission = '" + JsonConvert.SerializeObject(permission)
                + "' where department = " + deptToNumeric(comboBox1.Text) +
                    " and position = " + positionToNumberic(comboBox2.Text);
                Console.WriteLine(sqlCmd.CommandText);
                sqlCmd.ExecuteNonQuery();
                sqlConn.Close();
                MessageBox.Show("successful to update the permission");
            }
            else
            {
                MessageBox.Show("please choose department and position combo box");
            }
        }

        public void resetpermission()
        {
            if (comboBox1.Text != "" && comboBox2.Text != "")
            {
                ConnectString rootconn = new ConnectString("root", "123456");
                sqlConn.ConnectionString = rootconn.getString();
                sqlConn.Open();
                sqlCmd.Connection = sqlConn;
                sqlCmd.CommandText = "select permission " +
                                    "from group_permission " +
                                    "where department = " + deptToNumeric(comboBox1.Text) +
                                    " and position = " + positionToNumberic(comboBox2.Text);
                sqlRd = sqlCmd.ExecuteReader();
                Console.WriteLine(sqlCmd.CommandText);
                while (sqlRd.Read())
                {
                    JSONlist = sqlRd.GetString(0);
                }
                sqlRd.Close();
                sqlConn.Close();
                Console.WriteLine(JSONlist);
                var permissionList = JsonConvert.DeserializeObject<List<int>>(JSONlist);
                foreach (var control in this.Controls)
                {
                    GroupBox gb = control as GroupBox;
                    if (null != gb)
                    {
                        foreach (var cbx in gb.Controls.OfType<CheckBox>())
                        {
                            cbx.Checked = false;
                            sqlConn.Open();
                            sqlCmd.Connection = sqlConn;
                            sqlCmd.CommandText = "select * from permission where permission_name = '" + cbx.Text + "'";
                            sqlRd = sqlCmd.ExecuteReader();
                            while (sqlRd.Read())
                            {
                                foreach (var permissionID in permissionList)
                                {
                                    if (permissionID == sqlRd.GetInt32("permission_id"))
                                    {
                                        cbx.Checked = true;
                                    }
                                }
                            }
                            sqlRd.Close();
                            sqlConn.Close();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("please choose department and position combo box");
            }
        }

        public void checkedToList()
        {
            permission.Clear();
            foreach (var control in this.Controls)
            {
                GroupBox gb = control as GroupBox;
                if (null != gb)
                {
                    foreach (var cbx in gb.Controls.OfType<CheckBox>())
                    {
                        if (cbx.Checked)
                        {
                            ConnectString rootconn = new ConnectString("root", "123456");
                            sqlConn.ConnectionString = rootconn.getString();
                            sqlConn.Open();
                            sqlCmd.Connection = sqlConn;
                            sqlCmd.CommandText = "select * from permission where permission_name = '" + cbx.Text + "'";
                            sqlRd = sqlCmd.ExecuteReader();
                            while (sqlRd.Read())
                            {
                                permission.Add(sqlRd.GetInt32("permission_id"));
                            }
                            sqlRd.Close();
                            sqlConn.Close();
                        }
                    }
                }
            }
        }

        public void updatePermissionDB() //reset the database on the permission table, don't use it method
        {
            foreach (var control in this.Controls)
            {
                GroupBox gb = control as GroupBox;
                if (null != gb)
                {
                    foreach (var cbx in gb.Controls.OfType<CheckBox>())
                    {
                        ConnectString rootconn = new ConnectString("root", "123456");  
                        sqlConn.ConnectionString = rootconn.getString();
                        sqlConn.Open();
                        sqlCmd.Connection = sqlConn;
                        sqlCmd.CommandText = "insert into permission values(default, '"+ cbx.Text + "')";
                        sqlCmd.ExecuteNonQuery();
                        sqlConn.Close();

                    }
                }
            }
        }

        public int deptToNumeric(String dept)
        {
            switch (dept)
            {
                case "Sales / Store":
                    return 1;
                case "Inventory":
                    return 2;
                case "Technical":
                    return 3;
                case "Accounting":
                    return 4;
                case "Purchase":
                    return 5;
                case "Admin":
                    return 99;
                case "销售":
                    return 1;
                case "仓库":
                    return 2;
                case "技术":
                    return 3;
                case "会计":
                    return 4;
                case "采购":
                    return 5;
                case "管理员":
                    return 99;
            }
            return 0;
        }

        public int positionToNumberic(String position)
        {
            switch (position)
            {
                case "Clerk / Representative / Worker":
                    return 1;
                case "Manager":
                    return 2;
                case "Admin":
                    return 99;
                case "文员/售货员/工人":
                    return 1;
                case "经理":
                    return 2;
                case "管理员":
                    return 99;
            }
            return 0;
        }


        private void Permission_Load(object sender, EventArgs e)
        {
            changeLanguage();
        }

        private void cobType_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach(var control in this.Controls)
            {
                GroupBox gb = control as GroupBox;
                if(null != gb)
                {
                    gb.Visible = false;
                }
            }
            switch (cobType.Text)
            {
                case "Sale Management":
                    gbSale.Visible = true;
                    gbSale.Location = new Point(161, 203);
                    break;
                case "Inventory":
                    gbInventory.Visible = true;
                    gbInventory.Location = new Point(161, 203);
                    break;
                case "Delivery":
                    gbDelivery.Visible = true;
                    gbDelivery.Location = new Point(161, 203);
                    break;
                case "Purchase":
                    gbPurchase.Visible = true;
                    gbPurchase.Location = new Point(161, 203);
                    break;
                case "Technical Support":
                    gbTechnical.Visible = true;
                    gbTechnical.Location = new Point(161, 203);
                    break;
                case "Account":
                    gbAccount.Visible = true;
                    gbAccount.Location = new Point(161, 203);
                    break;
                case "Notification":
                    gbNoti.Visible = true;
                    gbNoti.Location = new Point(161, 203);
                    break;
                case "Setting":
                    gbSetting.Visible = true;
                    gbSetting.Location = new Point(161, 203);
                    break;
                case "Permission":
                    gbPermission.Visible = true;
                    gbPermission.Location = new Point(161, 203);
                    break;
                case "销售管理":
                    gbSale.Visible = true;
                    gbSale.Location = new Point(161, 203);
                    break;
                case "仓库":
                    gbInventory.Visible = true;
                    gbInventory.Location = new Point(161, 203);
                    break;
                case "送货":
                    gbDelivery.Visible = true;
                    gbDelivery.Location = new Point(161, 203);
                    break;
                case "采购":
                    gbPurchase.Visible = true;
                    gbPurchase.Location = new Point(161, 203);
                    break;
                case "技术支援":
                    gbTechnical.Visible = true;
                    gbTechnical.Location = new Point(161, 203);
                    break;
                case "会计":
                    gbAccount.Visible = true;
                    gbAccount.Location = new Point(161, 203);
                    break;
                case "通知":
                    gbNoti.Visible = true;
                    gbNoti.Location = new Point(161, 203);
                    break;
                case "设定":
                    gbSetting.Visible = true;
                    gbSetting.Location = new Point(161, 203);
                    break;
                case "权限":
                    gbPermission.Visible = true;
                    gbPermission.Location = new Point(161, 203);
                    break;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            resetpermission();
        }

        private void gbTechnical_Enter(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.Text != "" && comboBox2.Text != "")
            {
                resetpermission();
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text != "" && comboBox2.Text != "")
            {
                resetpermission();
            }
        }
    }
}
