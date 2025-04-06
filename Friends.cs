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
    public partial class Friends : MetroFramework.Forms.MetroForm
    {
        public Friends()
        {
            InitializeComponent();
        }

        string username = File.ReadAllText("pass.txt").Split(':')[0];

        private void Friends_Load(object sender, EventArgs e)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential("SERVER", "ID");
                    string[] friends = client.DownloadString($"SERVER/FRIENDS/{username}.txt").Split(':');
                    string[] pendingFriends = client.DownloadString($"SERVER/FRIENDS_PENDING/{username}.txt").Split('\n');

                    if (pendingFriends.Take(pendingFriends.Length - 1).ToArray().Length > 0)
                    {
                        metroButton4.Text = $"Requests ({pendingFriends.Take(pendingFriends.Length - 1).ToArray().Length})";
                    }

                    Console.WriteLine(pendingFriends.Take(pendingFriends.Length - 1).Count());

                    foreach (string friend in friends.Take(friends.Length - 1))
                    {
                        MetroFramework.Controls.MetroLabel newLabel = new MetroFramework.Controls.MetroLabel();
                        newLabel.Text = friend;
                        newLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
                        flowLayoutPanel1.Controls.Add(newLabel);
                    }

                    foreach (string pendingFriend in pendingFriends.Take(pendingFriends.Length - 1))
                    {
                        MetroFramework.Controls.MetroLabel newLabel = new MetroFramework.Controls.MetroLabel();
                        newLabel.Text = pendingFriend;
                        newLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
                        newLabel.Click += new EventHandler(newLabel_Click);

                        void newLabel_Click(object sender2, EventArgs e2)
                        {
                            string newFriends = client.DownloadString($"SERVER/FRIENDS/{username}.txt") + $"{pendingFriend}:";
                            File.WriteAllText("TEMPADDFRIEND.txt", newFriends);
                            client.UploadFile($"SERVER/FRIENDS/{username}.txt", WebRequestMethods.Ftp.UploadFile, "TEMPADDFRIEND.txt");
                            File.Delete("TEMPADDFRIEND.txt");
                            string newFriendsOther = client.DownloadString($"SERVER/FRIENDS/{pendingFriend}.txt") + $"{username}:";
                            File.WriteAllText("TEMPADDOTHERFRIEND.txt", newFriendsOther);
                            client.UploadFile($"SERVER/FRIENDS/{pendingFriend}.txt", WebRequestMethods.Ftp.UploadFile, "TEMPADDOTHERFRIEND.txt");
                            File.Delete("TEMPADDOTHERFRIEND.txt");
                            string newPendingFriends = client.DownloadString($"SERVER/FRIENDS_PENDING/{username}.txt").Replace($"{pendingFriend}\n", "");
                            File.WriteAllText("TEMPREMOVEPENDINGFRIEND.txt", newPendingFriends);
                            client.UploadFile($"SERVER/FRIENDS_PENDING/{username}.txt", WebRequestMethods.Ftp.UploadFile, "TEMPREMOVEPENDINGFRIEND.txt");
                            File.Delete("TEMPREMOVEPENDINGFRIEND.txt");
                            string newPendingFriendsOther = client.DownloadString($"SERVER/FRIENDS_PENDING/{pendingFriend}.txt").Replace($"{username}\n", "");
                            File.WriteAllText("TEMPREMOVEOTHERPENDINGFRIEND.txt", newPendingFriendsOther);
                            client.UploadFile($"SERVER/FRIENDS_PENDING/{pendingFriend}.txt", WebRequestMethods.Ftp.UploadFile, "TEMPREMOVEOTHERPENDINGFRIEND.txt");
                            File.Delete("TEMPREMOVEOTHERPENDINGFRIEND.txt");

                            flowLayoutPanel2.Controls.Remove(newLabel);
                        }

                        flowLayoutPanel2.Controls.Add(newLabel);
                    }
                }
            } catch (WebException) { }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            Visible = false;
            new Form1().ShowDialog();
            Close();
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            new AddFriend().ShowDialog();
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            metroButton2.Enabled = true;
            metroButton4.Enabled = false;
            flowLayoutPanel2.Visible = true;
            flowLayoutPanel1.Visible = false;
            metroLabel1.Visible = true;
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            metroButton2.Enabled = false;
            metroButton4.Enabled = true;
            flowLayoutPanel2.Visible = false;
            flowLayoutPanel1.Visible = true;
            metroLabel1.Visible = false;
        }
    }
}