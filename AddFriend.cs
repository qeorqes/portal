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
    public partial class AddFriend : MetroFramework.Forms.MetroForm
    {
        public AddFriend()
        {
            InitializeComponent();
        }

        string username = File.ReadAllText("pass.txt").Split(':')[0];

        private void AddFriend_Load(object sender, EventArgs e)
        {

        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            bool userIsValid = false;
            foreach (char i in metroTextBox1.Text)
            {
                if (!"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123456890".Contains(i))
                {
                    userIsValid = false;
                    break;
                } else
                {
                    userIsValid = true;
                }
            }
            if (userIsValid)
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        client.Credentials = new NetworkCredential("SERVER", "ID");
                        string friends = client.DownloadString($"SERVER/FRIENDS/{username}.txt");
                        if (friends.Contains($"{metroTextBox1.Text}:"))
                        {
                            metroLabel1.Text = $"You are already friends with {metroTextBox1.Text}.";
                            metroLabel1.Visible = true;
                        } else if (client.DownloadString($"SERVER/FRIENDS_PENDING/{username}.txt").Contains($"{metroTextBox1.Text}\n") && !friends.Contains($"{metroTextBox1.Text}:"))
                        {
                            string newFriends = friends + $"{metroTextBox1.Text}:";
                            File.WriteAllText("TEMPADDFRIEND.txt", newFriends);
                            client.UploadFile($"SERVER/FRIENDS/{username}.txt", WebRequestMethods.Ftp.UploadFile, "TEMPADDFRIEND.txt");
                            File.Delete("TEMPADDFRIEND.txt");
                            string newFriendsOther = client.DownloadString($"SERVER/FRIENDS/{metroTextBox1.Text}.txt") + $"{username}:";
                            File.WriteAllText("TEMPADDOTHERFRIEND.txt", newFriendsOther);
                            client.UploadFile($"SERVER/FRIENDS/{metroTextBox1.Text}.txt", WebRequestMethods.Ftp.UploadFile, "TEMPADDOTHERFRIEND.txt");
                            File.Delete("TEMPADDOTHERFRIEND.txt");
                            string newPendingFriends = client.DownloadString($"SERVER/FRIENDS_PENDING/{username}.txt").Replace($"{metroTextBox1.Text}\n", "");
                            File.WriteAllText("TEMPREMOVEPENDINGFRIEND.txt", newPendingFriends);
                            client.UploadFile($"SERVER/FRIENDS_PENDING/{username}.txt", WebRequestMethods.Ftp.UploadFile, "TEMPREMOVEPENDINGFRIEND.txt");
                            File.Delete("TEMPREMOVEPENDINGFRIEND.txt");
                            string newPendingFriendsOther = client.DownloadString($"SERVER/FRIENDS_PENDING/{metroTextBox1.Text}.txt").Replace($"{username}\n", "");
                            File.WriteAllText("TEMPREMOVEOTHERPENDINGFRIEND.txt", newPendingFriendsOther);
                            client.UploadFile($"SERVER/FRIENDS_PENDING/{metroTextBox1.Text}.txt", WebRequestMethods.Ftp.UploadFile, "TEMPREMOVEOTHERPENDINGFRIEND.txt");
                            File.Delete("TEMPREMOVEOTHERPENDINGFRIEND.txt");
                        }
                        else
                        {
                            if (metroTextBox1.Text != "")
                            {
                                client.Credentials = new NetworkCredential("SERVER", "ID");
                                string users = client.DownloadString("SERVER/USERS/TOTAL.txt");
                                if (users.Contains($"{metroTextBox1.Text}\n"))
                                {
                                    string pendingOld = client.DownloadString($"SERVER/FRIENDS_PENDING/{metroTextBox1.Text}.txt");
                                    File.WriteAllText("TEMPADDFRIENDPENDING.txt", pendingOld + $"{username}\n");
                                    client.UploadFile($"SERVER/FRIENDS_PENDING/{metroTextBox1.Text}.txt", WebRequestMethods.Ftp.UploadFile, "TEMPADDFRIENDPENDING.txt");
                                    File.Delete("TEMPADDFRIENDPENDING.txt");
                                    Close();
                                } else
                                {
                                    metroLabel1.Text = "User does not exist.";
                                    metroLabel1.Visible = true;
                                }
                            }
                        }
                    }
                } catch (WebException)
                {
                    
                }
            }
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            Visible = false;
            Close();
        }
    }
}