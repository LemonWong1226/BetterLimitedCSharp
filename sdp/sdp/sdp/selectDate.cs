using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sdp
{
    public partial class selectDate : Form
    {
        public selectDate()
        {
            InitializeComponent();
        }

        private void selectDate_Load(object sender, EventArgs e)
        {
            dateTimePicker1.MinDate = new DateTime(1985, 6, 20);
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            this.Close();
        }

        public String getDate()
        {
            return dateTimePicker1.Text;
        }
    }
}
