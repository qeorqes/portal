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
using System.IO;

namespace Portal
{
    public partial class Login : MetroFramework.Forms.MetroForm
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {

            string user = metroTextBox1.Text;
            string pass = metroTextBox2.Text;

            using (var client = new WebClient())
            {
                client.Credentials = new NetworkCredential("SERVER", "ID");
                try
                {
                    string passFromWeb = client.DownloadString($"SERVER/LOGINS/{user}.txt").Split(':')[1];

                    if (pass == passFromWeb)
                    {
                        File.WriteAllText("pass.txt", $"{user}:{pass}");

                        Visible = false;
                        Application.Run(new Form1());
                    } else
                    {
                        metroButton1.Highlight = true;
                    }
                } catch (WebException) {
                    metroButton1.Highlight = true;
                }
            }


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (metroTextBox2.PasswordChar == '•')
            {
                metroTextBox2.PasswordChar = '\0';
            } else
            {
                metroTextBox2.PasswordChar = '•';
            }
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Visible = false;
            Register register = new Register();
            register.ShowDialog();
            Close();
        }
    }
}