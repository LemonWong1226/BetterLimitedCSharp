using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using Newtonsoft.Json;

namespace sdp
{
    public partial class Main : Form
    {
        MySqlConnection sqlConn = new MySqlConnection();
        MySqlCommand sqlCmd = new MySqlCommand();
        DataTable sqlDt = new DataTable();
        String sqlQuery;
        MySqlDataAdapter Dta = new MySqlDataAdapter();
        DataSet DS = new DataSet();


        MySqlDataReader sqlRd;
        private String JSONlist;
        public String language;
        public String user;
        public String name;
        public int dept;
        public int position;
        public String Staff;
        public static Main instance;
        public Label title;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public int notification = 0;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public Main()
        {
            InitializeComponent();
            instance = this;
            btnHome.Image = null;
        }

        public void loadForm(object Form)
        {
            /*       btnHome.Image = null;
                   setButtonRedDot();*/
            if (this.mainpanel.Controls.Count > 0)
                this.mainpanel.Controls.RemoveAt(0);
            Form f = Form as Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            this.mainpanel.Controls.Add(f);
            this.mainpanel.Tag = f;
            f.Show();
        }

        public void setButtonRedDot()
        {
            btnHome.Image = Properties.Resources.redDot;
        }

        private void btnSale_Click(object sender, EventArgs e)
        {
            saleManagement();
        }

        public void saleManagement()
        {
            if (language == "Chinese")
                lblTitle.Text = "销售管理";
            else
                lblTitle.Text = "Sale Management";
            loadForm(new StoreHomePage());
        }


        public void checkout()
        {
            checkout check = new checkout();
            if (language == "Chinese")
                lblTitle.Text = "结账";
            else
                lblTitle.Text = "Check Out";
            check.setStore(sdp.Shop1.instance.shop);
            loadForm(check);
        }

        public void shop1(String store)
        {
            Shop1 shop = new Shop1();
            shop.setShop(store);
            if (store == "HK01")     
                lblTitle.Text = "HK-SHOP 1";
            else
                lblTitle.Text = "HK-SHOP 2";
            loadForm(shop);
         /*   lblTitle.Text = "Shop1";
            loadForm(new Shop1());*/
        }

        public void DeliveryHome()
        {
            if (language == "Chinese")
                lblTitle.Text = "送货主页";
            else
                lblTitle.Text = "Delivery";
            loadForm(new DeliveryHome());
        }

        public void DeliveryTable()
        {
            if (language == "Chinese")
                lblTitle.Text = "送货时间表";
            else
                lblTitle.Text = "Delivery Table";
            loadForm(new DeliveryOrder());
        }

        public void SaleRecord(String shop)
        {
            saleRecord record = new saleRecord();
            record.setShop(shop);
            if (language == "Chinese")
                lblTitle.Text = "销售记录";
            else
                lblTitle.Text = "Sales Records";
            loadForm(record);
        }

        public void Edititem(String storeId, String selectRow)
        {
            edititem edit = new edititem();
            edit.setSelectRow(selectRow);
            edit.setStoreId(storeId);
            if (language == "Chinese")
                lblTitle.Text = "编辑货品";
            else
                lblTitle.Text = "Edit Item";
            loadForm(edit);
        }

        public void Additem()
        {
            additem add = new additem();
            if (language == "Chinese")
                lblTitle.Text = "新增货品";
            else
                lblTitle.Text = "Add Item";
            add.setStore(Shop1.instance.shop);
            loadForm(add);
        }

        public void ItemDetail(String storeId, String selectRow)
        {
            itemDetail detail = new itemDetail();
            detail.setSelectRow(selectRow);
            detail.setStoreId(storeId);
            if (language == "Chinese")
                lblTitle.Text = "货品详情";
            else
                lblTitle.Text = "Item Detail";
            loadForm(detail);
        }

        public void ConfirmDelivery()
        {
            if (language == "Chinese")
                lblTitle.Text = "确认送货订单";
            else
                lblTitle.Text = "Delivery";
            loadForm(new ConfirmDelivery());
        }

        public void DeliveryRecord()
        {
            if (language == "Chinese")
                lblTitle.Text = "送货订单记录";
            else
                lblTitle.Text = "Delivery Record";
            loadForm(new DeliveryRecord());
        }

        public void ConfirmDeliveryDetail()
        {
            ConfirmDeliveryDetail confirmDeliveryDetail = new ConfirmDeliveryDetail();
            confirmDeliveryDetail.setOrder(sdp.ConfirmDelivery.instance.order);
            if (language == "Chinese")
                lblTitle.Text = "确认送货详情";
            else
                lblTitle.Text = "Confirm Delivery Detail";
            loadForm(confirmDeliveryDetail);
        }

        public void DeliveryDetail(String action)
        {
            if (action == "record")
            {
                DeliveryDetail deliveryDetail = new DeliveryDetail();
                deliveryDetail.SetOrder(sdp.DeliveryRecord.instance.order);
                deliveryDetail.setAction("record");
                if (language == "Chinese")
                    lblTitle.Text = "确认送货详情";
                else
                    lblTitle.Text = "Delivery";
                loadForm(deliveryDetail);
            }
            else if(action == "table")
            {
                DeliveryDetail deliveryDetail = new DeliveryDetail();
                deliveryDetail.SetOrder(sdp.DeliveryOrder.instance.selectOrder.Substring(10,10));
                deliveryDetail.setAction("table");
                if (language == "Chinese")
                    lblTitle.Text = "确认送货详情";
                else
                    lblTitle.Text = "Delivery";
                loadForm(deliveryDetail);
            }
        }

        public void SaleRecordDetail(String action, String store)
        {
            if (action == "Sale")
            {
                saleRecordDetail detail = new saleRecordDetail();
                detail.setAction("Sale");
                detail.setInvoice(saleRecord.instance.selectRow);
                detail.setShop(store);
                if (language == "Chinese")
                    lblTitle.Text = "销售记录详情";
                else
                    lblTitle.Text = "Sale Record Detail";
                loadForm(detail);
            }
            else if (action == "confirmDelivery") 
            {
                saleRecordDetail detail = new saleRecordDetail();
                detail.setInvoice(sdp.ConfirmDeliveryDetail.instance.invoice);
                detail.setAction("confirmDelivery");
                if (language == "Chinese")
                    lblTitle.Text = "销售记录详情";
                else
                    lblTitle.Text = "Sale Record Detail";
                loadForm(detail);
            }
            else if (action == "deliveryRecord")
            {
                saleRecordDetail detail = new saleRecordDetail();
                detail.setInvoice(sdp.DeliveryDetail.instance.invoice);
                detail.setAction("deliveryRecord");
                if (language == "Chinese")
                    lblTitle.Text = "销售记录详情";
                else
                    lblTitle.Text = "Sale Record Detail";
                loadForm(detail);
            }
            else if (action == "table")
            {
                saleRecordDetail detail = new saleRecordDetail();
                detail.setInvoice(sdp.DeliveryDetail.instance.invoice);
                detail.setAction("table");
                if (language == "Chinese")
                    lblTitle.Text = "销售记录详情";
                else
                    lblTitle.Text = "Sale Record Detail";
                loadForm(detail);
            }
            else if (action == "confirmTechnical")
            {
                saleRecordDetail detail = new saleRecordDetail();
                detail.setInvoice(sdp.ConfirmTechncalDetail.instance.getInovice());
                detail.setAction("confirmTechnical");
                if (language == "Chinese")
                    lblTitle.Text = "销售记录详情";
                else
                    lblTitle.Text = "Sale Record Detail";
                loadForm(detail);
            }
        }

        public void InventoryDetail()
        {
            InventoryDetail detail = new InventoryDetail();
            detail.setitemid(sdp.Inventory.instance.itemid);
            if (language == "Chinese")
                lblTitle.Text = "库存详情";
            else
                lblTitle.Text = "Inventory Detail";
            loadForm(detail);
        }

        public void AddInventory()
        {
            if (language == "Chinese")
                lblTitle.Text = "添加库存";
            else
                lblTitle.Text = "Add Inventory";
            loadForm(new AddInventory());
        }

        public void Home()
        {
            Home home = new Home();
            if (language == "Chinese")
                lblTitle.Text = "主页";
            else
                lblTitle.Text = "Home";
            if(notification == 1)
            {
                home.viewImage1();
            }
            else if(notification == 2)
            {
                home.viewImage1();
                home.viewImage2();
            }
            else if(notification >= 2)
            {
                home.viewImage1();
                home.viewImage2();
                home.viewImage3();
            }
            home.setstaffID(Staff);
            loadForm(home);
            notification = 0;
        }

        public void increaseNotification()
        {
            notification++;
        }

        public void EditInventory()
        {
            EditInventory edit = new EditInventory();
            edit.setitemid(sdp.Inventory.instance.itemid);
            if (language == "Chinese")
                lblTitle.Text = "编辑库存";
            else
                lblTitle.Text = "Edit Inventory";
            loadForm(edit);
        }

        public void TechnicalRecord()
        {
            if (language == "Chinese")
                lblTitle.Text = "安装订单记录";
            else
                lblTitle.Text = "Installation Order Record";
            loadForm(new InstallationRecord());
        }

        public void Inventory()
        {
            if (language == "Chinese")
                lblTitle.Text = "库存";
            else
                lblTitle.Text = "Inventory";
            loadForm(new Inventory());
        }

        public void NotificationDetail()
        {
            NotificationDetail notificationDetail = new NotificationDetail();
            notificationDetail.setNotificationID(sdp.Notification.instance.notificationId);
            if (language == "Chinese")
                lblTitle.Text = "通知详情";
            else
                lblTitle.Text = "Notification Detail";
            loadForm(notificationDetail);
        }
        public void addDeliveryOrder()
        {
            if (language == "Chinese")
                lblTitle.Text = "添加交货单";
            else
                lblTitle.Text = "New Delivery Order";
            loadForm(new AddDeliveryOrder());
        }

        public void supplier()
        {
            if (language == "Chinese")
                lblTitle.Text = "供应商";
            else
                lblTitle.Text = "Supplier";
            loadForm(new Supplier());
        }

        public void Notification()
        {
            if (language == "Chinese")
                lblTitle.Text = "通知";
            else
                lblTitle.Text = "Notification";
            loadForm(new Notification());
        }

        public void addSupplier()
        {
            if (language == "Chinese")
                lblTitle.Text = "添加供应商";
            else
                lblTitle.Text = "Add Supplier";
            loadForm(new AddSupplier());
        }

        public void TechnicalTable()
        {
            if (language == "Chinese")
                lblTitle.Text = "技术支援表";
            else
                lblTitle.Text = "Technical Table";
            loadForm(new TechnicalTable());
        }

        public void Setting()
        {
            if (language == "Chinese")
                lblTitle.Text = "设定";
            else
                lblTitle.Text = "Setting";
            loadForm(new Setting());
        }

        public void setLable(String lable)
        {
            lblTitle.Text = lable;
        }

        public void EditSupplier()
        {
            EditSupplier edit = new EditSupplier();
            edit.setSelectRow(sdp.Supplier.instancel.itemid);
            if (language == "Chinese")
                lblTitle.Text = "编辑供应商";
            else
                lblTitle.Text = "Edit Supplier";
            loadForm(edit);
        }

        public void SupplierDetail()
        {
            SupplierDetail detail = new SupplierDetail();
            detail.setSelectRow(sdp.Supplier.instancel.itemid);
            if (language == "Chinese")
                lblTitle.Text = "供应商详情";
            else
                lblTitle.Text = "Supplier Detail";
            loadForm(detail);
        }

        public void NewTechnical()
        {
            if (language == "Chinese")
                lblTitle.Text = "添加技术支持";
            else
                lblTitle.Text = "New Technical Support";
            loadForm(new NewTechnical());
        }

        public void ReorderRequest()
        {
            if (language == "Chinese")
                lblTitle.Text = "补货订购请求";
            else
                lblTitle.Text = "Re-order Request";
            loadForm(new ReorderRequest());
        }

        public void AddReorder(String store)
        {
            AddReorder add = new AddReorder();
            add.setStore(store);
            if (language == "Chinese")
                lblTitle.Text = "补货请求";
            else
                lblTitle.Text = "Add Re-order Request";
            loadForm(add);
        }

        public void CreateAccount()
        {
            loadForm(new CreateAccount());
        }

        public void TechnicalHome()
        {
            if (language == "Chinese")
                lblTitle.Text = "技术支援";
            else
                lblTitle.Text = "Technical Support";
            loadForm(new TechnicalHome());
        }

        public void TechniacalTable()
        {
            if (language == "Chinese")
                lblTitle.Text = "技术支援时间表";
            else
                lblTitle.Text = "Technical Support Time Table";
            loadForm(new TechnicalTable());
        }

        public void preOrder(String invoice)
        {
            if (language == "Chinese")
                lblTitle.Text = "预订表";
            else
                lblTitle.Text = "Pre-order Table";
            PreOrderCheckout pre = new PreOrderCheckout();
            pre.setInvoiceId(invoice);
            loadForm(pre);
        }

        public void InstallationDetail(String action)
        {
            if (action == "table")
            {
                InstallationDetail detail = new InstallationDetail();
                detail.setInstallationId(sdp.TechnicalTable.instance.getOrder());
                detail.setAction(action);
                if (language == "Chinese")
                    lblTitle.Text = "安装详情";
                else
                    lblTitle.Text = "Installation Detail";
                loadForm(detail);
            }
            else if(action == "record")
            {
                InstallationDetail detail = new InstallationDetail();
                detail.setInstallationId(sdp.InstallationRecord.instance.order);
                if (language == "Chinese")
                    lblTitle.Text = "安装详情";
                else
                    lblTitle.Text = "Installation Detail";
                detail.setAction(action);
                loadForm(detail);
            }
        }

        public void ConfirmInstallation()
        {
            if (language == "Chinese")
                lblTitle.Text = "确认安装";
            else
                lblTitle.Text = "Confirm Installation";
            loadForm(new ConfirmTechnical());
        }

        public void ConfirmInstallationDetail()
        {
            ConfirmTechncalDetail detail = new ConfirmTechncalDetail();
            detail.setInstallationId(sdp.ConfirmTechnical.instance.order);
            if (language == "Chinese")
                lblTitle.Text = "确认安装详情";
            else
                lblTitle.Text = "Confirm Installation Detail";
            loadForm(detail);
        }

        public void ReorderRequestDetail(String reorderId)
        {
            ReorderRequestDetail detail = new ReorderRequestDetail();
            detail.setReorderID(reorderId);
            if (language == "Chinese")
                lblTitle.Text = "补货请求详细信息";
            else
                lblTitle.Text = "Re-order Request Detail";
            loadForm(detail);
        }

        public void ReorderRecord()
        {
            if (language == "Chinese")
                lblTitle.Text = "补货记录";
            else
                lblTitle.Text = "Re-order Record";
            loadForm(new ReorderRecord());
        }

        public void ReorderRecordDetail(String reorderId)
        {
            ReorderDetail detail = new ReorderDetail();
            detail.setReorderID(reorderId);
            if (language == "Chinese")
                lblTitle.Text = "补货细节";
            else
                lblTitle.Text = "Re-order Order Detail";
            loadForm(detail);
        }

        public void PurchaseHome()
        {
            if (language == "Chinese")
                lblTitle.Text = "采购";
            else
                lblTitle.Text = "Purchase";
            loadForm(new PurchaseHome());
        }

        public void PurchaseRequestOrderRecord()
        {
            if (language == "Chinese")
                lblTitle.Text = "采购请求记录";
            else
                lblTitle.Text = "Purchase Request Order Record";
            loadForm(new PurchaseRequestOrderRecord());
        }

        public void NewPurchaseRequest()
        {
            if (language == "Chinese")
                lblTitle.Text = "新增采购请求";
            else
                lblTitle.Text = "New Purchase Request";
            loadForm(new NewPurchaseRequest());
        }

        public void PurchaseRequestOrderDetail(String requestId, String status)
        {
            PurchaseRequestOrderDetail detail = new PurchaseRequestOrderDetail();
            detail.setRequestId(requestId);
            detail.setStatus(status);
            if (language == "Chinese")
                lblTitle.Text = "采购请求详情";
            else
                lblTitle.Text = "Purchase Request Order Detail";
            loadForm(detail);
        }

        public void PurchaseOrderRecord()
        {
            if (language == "Chinese")
                lblTitle.Text = "采购订单记录";
            else
                lblTitle.Text = "Purchase Order Record";
            loadForm(new PurchaseOrderRecord());
        }

        public void PurchaseOrderDetail(String orderId, String status)
        {
            PurchaseOrderDetail detail = new PurchaseOrderDetail();
            detail.setOrderId(orderId);
            detail.setStatus(status);
            if (language == "Chinese")
                lblTitle.Text = "采购订单详情";
            else
                lblTitle.Text = "Purchase Order Detail";
            loadForm(detail);
        }

        public void DefectItemDetails(String orderId, String status)
        {
            DefectItemDetails detail = new DefectItemDetails();
            detail.setGoodReturnItemId(orderId);
            detail.setSelectStatus(status);
            if (language == "Chinese")
                lblTitle.Text = "退货详情";
            else
                lblTitle.Text = "Defect Item Details";
            loadForm(detail);
        }

        public void DefectItem()
        {
            if (language == "Chinese")
                lblTitle.Text = "退货";
            else
                lblTitle.Text = "Defect Item";
            loadForm(new DefectItem());
        }

        public void AddDefectItem()
        {
            if (language == "Chinese")
                lblTitle.Text = "新增退货";
            else
                lblTitle.Text = "Add Defect Item";
            loadForm(new AddDefectItem());
        }

        public void AccountHomePage()
        {
            if (language == "Chinese")
                lblTitle.Text = "会计";
            else
                lblTitle.Text = "Account";
            loadForm(new AccountHomePage());
        }

        public void ExportPaymentReceipt()
        {
            if (language == "Chinese")
                lblTitle.Text = "输出付款收据";
            else
                lblTitle.Text = "Export Payment Receipt";
            loadForm(new ExportPaymentReceipt());
        }

        public void GoodReceivedNote() {
            if (language == "Chinese")
                lblTitle.Text = "收货单";
            else
                lblTitle.Text = "Good Received Note";
            loadForm(new GoodReceivedNote());
        }

        public void ExportPurchaseOrder()
        {
            if (language == "Chinese")
                lblTitle.Text = "输出采购订单";
            else
                lblTitle.Text = "Export Purchase Order";
            loadForm(new ExportPurchaseOrder());
        }

        public void Statement(String store)
        {
            Statement statement = new Statement();
            statement.setStore(store);
            if (language == "Chinese")
                lblTitle.Text = "结算表";
            else
                lblTitle.Text = "Statement";
            loadForm(statement);
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



        private void panelheader_MouseMove(object sender, MouseEventArgs e)
        {
            Move_Form(Handle, e);
        }

        public void Move_Form(IntPtr Handle, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void btnDelivey_Click(object sender, EventArgs e)
        {
            DeliveryHome();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            btnHome.Image = null;
            Home();
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            Inventory();
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {

        }

        private void mainpanel_Paint(object sender, PaintEventArgs e)
        {

        }

        public void changeLanguage()
        {
            StreamReader text = new StreamReader(@"Language.txt");
            language = text.ReadLine();
            text.Close();
            if (language == "Chinese")
            {
                btnHome.Text = "主頁";
                btnSale.Text = "销售管理";
                btnInventory.Text = "仓库";
                btnDelivey.Text = "送货";
                btnPurchase.Text = "采购";
                btnTechnical.Text = "技术支援";
                btnAccount.Text = "会计";
                btnNotification.Text = "通知";
                btnSetting.Text = "设定";
                btnPermission.Text = "权限";
                btnLogout.Text = "登出";
            }
            else
            {
                btnHome.Text = "HOME";
                btnSale.Text = "SALE MANAGEMENT";
                btnInventory.Text = "INVENTORY";
                btnDelivey.Text = "DELIVERY";
                btnPurchase.Text = "PURCHASE";
                btnTechnical.Text = "TECHNICAL SUPPORT";
                btnAccount.Text = "ACCOUNT";
                btnNotification.Text = "NOTIFICATION";
                btnSetting.Text = "SETTING";
                btnPermission.Text = "PERMISSION";
                btnLogout.Text = "LOGOUT";
            }
            switch (dept)
            {
                case 1:
                    if (language == "Chinese")
                        lblDept.Text = "销售";
                    else
                        lblDept.Text = "Sales";
                    break;
                case 2:
                    if (language == "Chinese")
                        lblDept.Text = "仓库";
                    else
                        lblDept.Text = "Inventory";
                    break;
                case 3:
                    if (language == "Chinese")
                        lblDept.Text = "技术";
                    else
                        lblDept.Text = "Technical";
                    break;
                case 4:
                    if (language == "Chinese")
                        lblDept.Text = "会计";
                    else
                        lblDept.Text = "Accounting";
                    break;
                case 5:
                    if (language == "Chinese")
                        lblDept.Text = "采购";
                    else
                        lblDept.Text = "Purchase";
                    break;
                case 99:
                    if (language == "Chinese")
                        lblDept.Text = "管理员";
                    else
                        lblDept.Text = "Admin";
                    break;
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Console.WriteLine(user);
            try
            {
                StreamReader text = new StreamReader(@"Language.txt");
                language = text.ReadLine();
                text.Close();
            }
            catch
            {

            }
            btnSale.Visible = false;
            btnInventory.Visible = false;
            btnDelivey.Visible = false;
            btnPurchase.Visible = false;
            btnTechnical.Visible = false;
            btnAccount.Visible = false;
            btnNotification.Visible = false;
            btnSetting.Visible = false;
            btnPermission.Visible = false;

            changeLanguage();

            lblName.Text = name;
            setPermission(dept, position);
            Home();
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
                    case 15:
                        btnSale.Visible = true;
                        break;
                    case 29:
                        btnInventory.Visible = true;
                        break;
                    case 39:
                        btnDelivey.Visible = true;
                        break;
                    case 50:
                        btnPurchase.Visible = true;
                        break;
                    case 60:
                        btnTechnical.Visible = true;
                        break;
                    case 63:
                        btnAccount.Visible = true;
                        break;
                    case 2:
                        btnNotification.Visible = true;
                        break;
                    case 68:
                        btnSetting.Visible = true;
                        break;
                    case 16:
                        btnPermission.Visible = true;
                        break;
                }
            }
        }


        private void btnTechnical_Click(object sender, EventArgs e)
        {
            TechnicalHome();
        }

        public void setStaff(String Staff)
        {
            this.Staff = Staff;
        }

        public void setName(String name)
        {
            this.name = name;
        }

        public void setUser(String user)
        {
            this.user = user;
        }

        public void setDept(int dept)
        {
            this.dept = dept;
        }

        public void setPosition(int position)
        {
            this.position = position;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            string message = "Do you want to log out?";
            string title = "log out";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.Yes)
            {
                login log = new login();
                this.Close();
                log.ShowDialog();
            }
        }

        private void btnNotification_Click(object sender, EventArgs e)
        {
            Notification();
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            Setting();
        }

        private void btnPurchase_Click(object sender, EventArgs e)
        {
            PurchaseHome();
        }

        private void btnAccount_Click(object sender, EventArgs e)
        {
            AccountHomePage();
        }

        private void btnPermission_Click(object sender, EventArgs e)
        {
            if (language == "Chinese")
                lblDept.Text = "权限";
            else
                lblTitle.Text = "Permission";
            loadForm(new Permission());
        }

        private void paneltop_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
