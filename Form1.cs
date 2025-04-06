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

// Project gemaakt door Tiamo, Georges en Valenci.
// Let op dat de server niet echt is ingevuld in velden zoals lines 60 en 61.
// Dit SERVER en ID zijn placeholders voor als er echt een server was.

namespace Portal
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        public Form1()
        {
            InitializeComponent();
            flowLayoutPanel1.DragEnter += new DragEventHandler(Form1_DragEnter);
            flowLayoutPanel1.DragDrop += new DragEventHandler(Form1_DragDrop);
        }

        string username = File.ReadAllText("pass.txt").Split(':')[0];

        public List<string> filesList = new List<string>();

        private void Form1_Load(object sender, EventArgs e)
        {
            
            metroLabel1.Text = $"Logged in as: {username}";

            reload();
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (metroComboBox1.SelectedItem != null)
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files) filesList.Add(file);

                for (int i = 0; i < filesList.Count; i++)
                {

                    var startOfName = filesList[i].LastIndexOf("\\");
                    var fileName = filesList[i].Substring(startOfName + 1, filesList[i].Length - (startOfName + 1));


                    using (var client = new WebClient())
                    {
                        client.Credentials = new NetworkCredential("SERVER", "ID");
                        client.UploadFile($"SERVER/FILES/{metroComboBox1.SelectedItem}/{fileName}", WebRequestMethods.Ftp.UploadFile, filesList[i]);
                    }

                    MetroFramework.Controls.MetroButton newButton = new MetroFramework.Controls.MetroButton();
                    newButton.Text = fileName;
                    newButton.Theme = MetroFramework.MetroThemeStyle.Dark;
                    flowLayoutPanel1.Controls.Add(newButton);
                }
            }

            filesList.Clear();
        }

        private void metroButton1_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Visible = true;
            flowLayoutPanel2.Visible = false;
            metroButton3.Enabled = false;
            metroButton4.Enabled = true;
            metroButton2.Enabled = true;
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            flowLayoutPanel1.Visible = false;
            flowLayoutPanel2.Visible = true;
            metroButton3.Enabled = true;
            metroButton4.Enabled = false;
            metroButton2.Enabled = false;
        }

        private void metroLabel1_Click(object sender, EventArgs e)
        {

        }

        private void metroButton5_Click(object sender, EventArgs e)
        {
            TopMost = false;
            Visible = false;
            Friends friends = new Friends();
            friends.ShowDialog();
            Close();
        }

        private void metroButton6_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to log out?", "Log out", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                File.Delete("pass.txt");
                Visible = false;
                Login login = new Login();
                login.ShowDialog();
                Close();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            reload();
        }

        void reload()
        {
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel2.Controls.Clear();
            metroComboBox1.Items.Clear();
            try
            {
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential("SERVER", "ID");
                    string pendingFriends = client.DownloadString($"SERVER/FRIENDS_PENDING/{username}.txt");
                    int amountOfPendingFriends = pendingFriends.Split('\n').Take(pendingFriends.Split('\n').Length - 1).ToArray().Length;
                    Console.WriteLine(pendingFriends.Split('\n').Take(pendingFriends.Split('\n').Length - 1).ToArray().Length);
                    if (amountOfPendingFriends > 0 && pendingFriends != "")
                    {
                        metroButton5.Text = $"Friends ({amountOfPendingFriends})";
                    }

                    List<string> strList = new List<string>();
                    FtpWebRequest fwr = (FtpWebRequest)FtpWebRequest.Create(new Uri($"SERVER/FILES/{username}"));
                    fwr.Credentials = new NetworkCredential("SERVER", "ID");
                    fwr.Method = WebRequestMethods.Ftp.ListDirectory;

                    StreamReader sr = new StreamReader(fwr.GetResponse().GetResponseStream());
                    string str = sr.ReadLine();
                    while (str != null)
                    {
                        strList.Add(str);
                        str = sr.ReadLine();
                    }
                    strList = strList.Skip(2).ToList();
                    if (strList.Count > 0)
                    {
                        metroButton4.Text = $"Received ({strList.Count})";

                        foreach (string i in strList)
                        {
                            MetroFramework.Controls.MetroButton newButton = new MetroFramework.Controls.MetroButton();
                            newButton.Text = i;
                            newButton.Theme = MetroFramework.MetroThemeStyle.Dark;
                            flowLayoutPanel2.Controls.Add(newButton);
                            newButton.Click += new EventHandler(newButton_Click);

                            void newButton_Click(object sender, EventArgs e)
                            {
                                File.WriteAllText($"received\\{i}", client.DownloadString($"SERVER/FILES/{username}/{i}"));
                                System.Diagnostics.Process.Start("explorer.exe", "/select, \"" + $"received\\{i}" + "\"");
                            }
                        }
                    }
                }
            }
            catch (WebException)
            {
                
            }

            string[] friends = { };
            try
            {
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential("SERVER", "ID");
                    friends = client.DownloadString($"SERVER/FRIENDS/{username}.txt").Split(':');
                    friends = friends.Take(friends.Length - 1).ToArray();
                    metroComboBox1.Items.AddRange(friends);
                }
            }
            catch (WebException)
            {

            }
        }
    }
}