using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;

namespace sdp
{
    public partial class ForgetPassword : Form
    {
        string randomCode;
        public static string sender;
        public ForgetPassword()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
                  string from, pass, messageBody;
                 Random rand = new Random();
                 randomCode = (rand.Next(999999)).ToString();
                 MailMessage message = new MailMessage();
                 sender = (txtEmail.Text);
                 from = "";
                 pass = "/";
                 messageBody = "Your reset code is " + randomCode;
                 message.To.Add(sender.ToString());
                 message.From = new MailAddress(from);
                 message.Body = messageBody;
                 message.Subject = "password reseting code";
                 SmtpClient smtp = new SmtpClient();
                 smtp.Host = "smtp.gmail.com";
                 smtp.EnableSsl = true;
                 smtp.Port = 2525;
                 smtp.UseDefaultCredentials = false;
                 smtp.Credentials = new NetworkCredential(from, pass);
                 smtp.Send(message); 
            /*    try
                {
                    smtp.Send(message);
                    MessageBox.Show("code send successfully");
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }*/
        }

        private void btnVerify_Click(object sender, EventArgs e)
        {
            if(randomCode == txtVerify.Text)
            {
                sender = txtEmail.Text;
                MessageBox.Show("OK");
            }
            else
            {
                MessageBox.Show("Wrong code");
            }
        }
    }
}
