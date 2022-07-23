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
    public partial class PurchaseOrderDetail : Form
    {
        private String status;
        private String orderId;
        private String supplierId;
        private String supplierName;
        private String supplierEmail;
        private String supplierPhone;
        private String supplierAddress;
        private String total;
        private String remark;
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();

        DataSet DS = new DataSet();

        MySqlDataReader sqlRd;
        private string language;
        private string JSONlist;

        public PurchaseOrderDetail()
        {
            InitializeComponent();
        }

        private void PurchaseOrderDetail_Load(object sender, EventArgs e)
        {
            setPermission(Main.instance.dept, Main.instance.position);
            changeLanguage();
            if (status == "Request Confirmed" || status == "Request Rejected" || status == "Request Cancelled" ||
                status == "请求已确认" || status == "请求被拒绝" || status == "请求已取消")
            {
                btnConfirm.Visible = false;
                btnReject.Visible = false;
            } 
            ConnectString rootconn = new ConnectString("root", "123456");
            sqlConn.ConnectionString = rootconn.getString();

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            sqlCmd.CommandText = "select distinct inventory.supplier_id, supplier_name," +
                " supplier_email, supplier_phone , purchase_ttl_amount, purchase_remarks, supplier_address " +
                "from purchase_order " +
                "inner join purchase_request " +
                "on purchase_request.purchase_request_id = purchase_order.purchase_request_id " +
                "inner join purchase_item " +
                "on purchase_request.purchase_request_id = purchase_item.purchase_request_id " +
                "inner join inventory " +
                "on inventory.item_id = purchase_item.item_id " +
                "inner join supplier " +
                "on inventory.supplier_id = supplier.supplier_id " +
                "where purchase_order_id = '"+ orderId + "'";
            sqlRd = sqlCmd.ExecuteReader();
            while (sqlRd.Read())
            {
                supplierId = sqlRd.GetString(0);
                supplierName = sqlRd.GetString(1);
                supplierEmail = sqlRd.GetString(2);
                supplierPhone = sqlRd.GetString(3);
                total = sqlRd.GetString(4);
                remark = sqlRd.GetString(5);
                supplierAddress = sqlRd.GetString(6);
            }
            sqlRd.Close();
            sqlConn.Close();
            lblSupplierId.Text = supplierId;
            lblSupplierName.Text = supplierName;
            lblSupplierEmail.Text = supplierEmail;
            lblSupplierPhone.Text = supplierPhone;
            lblTotal.Text = total;
            txtRemark.Text = remark;

            sqlConn.Open();
            sqlCmd.Connection = sqlConn;
            if (language == "Chinese")
            {
                sqlCmd.CommandText = "select purchase_item.item_id as '货品号', " +
                "item_name as '货品名称', " +
                "p_item_qty as '数量'," +
                "p_unix_cost as '成本价', " +
                "p_item_ttl_amount as '金额 (HKD)' " +
                "from purchase_item " +
                "inner join inventory " +
                "on inventory.item_id = purchase_item.item_id " +
                "inner join purchase_order " +
                "on purchase_order.purchase_request_id = purchase_item.purchase_request_id " +
                "where purchase_order_id = '" + orderId + "'";
            }
            else
            {
                sqlCmd.CommandText = "select purchase_item.item_id as 'Item Id', " +
                "item_name as 'Item Name', " +
                "p_item_qty as 'Quantity'," +
                "p_unix_cost as 'Cost', " +
                "p_item_ttl_amount as 'Total (HKD)' " +
                "from purchase_item " +
                "inner join inventory " +
                "on inventory.item_id = purchase_item.item_id " +
                "inner join purchase_order " +
                "on purchase_order.purchase_request_id = purchase_item.purchase_request_id " +
                "where purchase_order_id = '" + orderId + "'";
            }
            sqlRd = sqlCmd.ExecuteReader();
            sqlDt.Load(sqlRd);
            sqlRd.Close();
            sqlConn.Close();
            dataGridView1.DataSource = sqlDt;
            dataGridView1.Columns[4].Width = 100;
            if (language == "Chinese")
                dataGridView1.Columns[3].Width = 100;
            else
                dataGridView1.Columns[3].Width = 50;
            dataGridView1.Columns[2].Width = 80;
            dataGridView1.Columns[0].Width = 150;
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
                    case 41:
                        btnReject.Visible = true;
                        btnConfirm.Visible = true;
                        break;
                    case 40:
                        btnGenarate.Visible = true;
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
                label5.Text = "备注:";
                btnBack.Text = "返回";
                btnReject.Text = "拒绝";
                btnConfirm.Text = "确认订单";
                btnGenarate.Text = "生成收货单";
                label2.Text = "供应商号 :";
                label3.Text = "供应商名称 :";
                label4.Text = "供应商电邮 :";
                label7.Text = "供应商电话 :";
                label6.Text = "总共金额(HKD):";
            }
            else
            {
                label5.Text = "Remark:";
                btnBack.Text = "BACK";
                btnReject.Text = "Reject";
                btnConfirm.Text = "Confirm Order";
                btnGenarate.Text = "Genarate Order";
                label2.Text = "Supplier Id :";
                label3.Text = "Supplier Name :";
                label4.Text = "Supplier Email :";
                label7.Text = "Supplier Phone :";
                label6.Text = "Total Cost(HKD):";
            }
        }

        public void setOrderId(String orderId)
        {
            this.orderId = orderId;
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
          /*  dataGridView1.Columns[4].Width = 100;
            if (language == "Chinese")
                dataGridView1.Columns[3].Width = 100;
            else
                dataGridView1.Columns[3].Width = 50;
            dataGridView1.Columns[2].Width = 80;
            dataGridView1.Columns[0].Width = 150;*/
        }

        private void btnGenarate_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int i=1;
            int totalItem = dataGridView1.Rows.Count;  //limoit = 12
            SolidBrush solidBrush = new SolidBrush(
            Color.DarkGray);
            Pen blackPen = new Pen(Color.Black, 1);

            e.Graphics.DrawRectangle(blackPen, 30, 320, 400, 140);

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            StringFormat sf1 = new StringFormat();
            sf1.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString("Better Limited", new Font("Mircosoft Sans Serif", 18, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 70, 40);
            e.Graphics.DrawString("Unit 1301,kowloon Bay pazzy 022", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 70);
            e.Graphics.DrawString("address address address address", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 90);
            e.Graphics.DrawString("Tel: +852 1234 5678    Fax: +852 1234 5678", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 110);
            e.Graphics.DrawString("Purchase Order: "+ orderId , new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Right - 250, 150);
            e.Graphics.DrawString("______________________________________________", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 180);
            e.Graphics.DrawString("__________", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Right - 30, 180);
            e.Graphics.DrawString("Purchase Order", new Font("Mircosoft Sans Serif", 20, FontStyle.Bold), Brushes.Black, e.MarginBounds.Right - 250, 180);
            e.Graphics.DrawString("Supplier Information", new Font("Mircosoft Sans Serif", 12), Brushes.Black, e.MarginBounds.Left - 70, 295);
            e.Graphics.DrawString(supplierName + "           +852 "+supplierPhone, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 330);
            e.Graphics.DrawString(supplierAddress, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 370);
            e.Graphics.DrawString("address test", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 390);
            e.Graphics.DrawString("Email : "+ supplierEmail, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 425);

            e.Graphics.DrawRectangle(blackPen, 30, 490, 757, 65);
            e.Graphics.FillRectangle(solidBrush, 30, 490, 150, 65);
            e.Graphics.DrawRectangle(blackPen, 30, 490, 150, 65);

            e.Graphics.DrawString("Remarks", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 60, 495);
            e.Graphics.DrawString(remark, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 190, 495);
      //      e.Graphics.DrawString("Remarks Test", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 190, 525);

            e.Graphics.FillRectangle(solidBrush, 30, 580, e.MarginBounds.Right + 30, 34);
            e.Graphics.DrawRectangle(blackPen, 30, 580, e.MarginBounds.Right + 30, 34);
            e.Graphics.DrawRectangle(blackPen, 30, 580, 180, 34);
            e.Graphics.DrawRectangle(blackPen, 30, 580, 180, 40 + 34 * totalItem);
            e.Graphics.DrawRectangle(blackPen, 680, 580, 107, 40 + 34 * totalItem);
            e.Graphics.DrawRectangle(blackPen, 30, 580, 757, 40 + 34 * totalItem);
            e.Graphics.DrawRectangle(blackPen, 30, 580, 470, 40 + 34 * totalItem);
            e.Graphics.DrawRectangle(blackPen, 30, 580, 555, 40 + 34 * totalItem);

            e.Graphics.DrawString("Item Id", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 585);
            e.Graphics.DrawString("Description", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 212, 585);
            e.Graphics.DrawString("Price (HKD)", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 682, 585);
            e.Graphics.DrawString("Quantity", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 504, 585);
            e.Graphics.DrawString("Unix Price", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 589, 585);

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                e.Graphics.DrawString(row.Cells[0].Value.ToString(), new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 585 + i * 35);
                e.Graphics.DrawString(row.Cells[1].Value.ToString(), new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 212, 585 + i * 35);
                e.Graphics.DrawString(row.Cells[4].Value.ToString(), new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 682, 585 + i * 35);
                e.Graphics.DrawString(row.Cells[2].Value.ToString(), new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 504, 585 + i * 35);
                e.Graphics.DrawString(row.Cells[3].Value.ToString(), new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 589, 585 + i * 35);
                i++;
            }

            e.Graphics.DrawRectangle(blackPen, 30, 585 + i * 35 + 25, 200, 40);
            e.Graphics.DrawRectangle(blackPen, 30, 585 + i * 35 + 25, 757, 40);

            e.Graphics.DrawString("Total Amount(HKD)", new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, e.MarginBounds.Left - 62, 585 + i * 35 + 35);
            e.Graphics.DrawString(total, new Font("Mircosoft Sans Serif", 12, FontStyle.Bold), Brushes.Black, 234, 585 + i * 35 + 35);
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            UpdateStatus("confirm", 2);
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            UpdateStatus("reject", 3);
        }

        public void UpdateStatus(String action, int state)
        {
            string message = "Are you want to " + action + " the order?";
            string title = action + " order";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.Yes)
            {
                try
                {
                    ConnectString rootconn = new ConnectString("root", "123456");
                    sqlConn.ConnectionString = rootconn.getString();
                    switch (state)
                    {
                        case 2:
                            sqlConn.Open();
                            sqlCmd.Connection = sqlConn;
                            sqlCmd.CommandText = "update purchase_order set purchase_order_status = " + state + " where purchase_order_id = '" +
                                orderId + "'";
                            sqlCmd.ExecuteNonQuery();
                            sqlConn.Close();

                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                sqlConn.Open();
                                sqlCmd.Connection = sqlConn;
                                sqlCmd.CommandText = "update inventory set inventory_qty = inventory_qty + "+row.Cells[2].Value+
                                    " where item_id = '"+row.Cells[0].Value+"'";
                                sqlCmd.ExecuteNonQuery();
                                sqlConn.Close();
                            }
                            MessageBox.Show("Successful to " + action + " this order");
                            Main.instance.PurchaseOrderRecord();
                            break;
                        case 3:
                            sqlConn.Open();
                            sqlCmd.Connection = sqlConn;
                            sqlCmd.CommandText = "update purchase_order set purchase_order_status = " + state + " where purchase_order_id = '" +
                                orderId + "'";
                            sqlCmd.ExecuteNonQuery();
                            sqlConn.Close();
                            MessageBox.Show("Successful to " + action + " this order");
                            Main.instance.PurchaseOrderRecord();
                            break;
                    }
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

        private void btnBack_Click(object sender, EventArgs e)
        {
            Main.instance.PurchaseOrderRecord();
        }

        public void setStatus(String status)
        {
            this.status = status;
        }
    }
}
