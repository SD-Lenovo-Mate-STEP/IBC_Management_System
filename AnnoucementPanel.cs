using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Office2010.Excel;
using IBC_Management_System.Annoucement;
using System.Threading;
using System.Configuration;

namespace IBC_Management_System
{
    public partial class AnnoucementForm : Form
    {
        public static string[] quotes = new string[20];
        //AsyncServer server = new AsyncServer("127.0.0.1", 1024);
        public static bool isEditing = false;
        public static int AnnoucementID = 0;
        public static bool isAdmin;
        public AnnoucementForm(bool Admin)
        {
            isAdmin = Admin;
            InitializeComponent();
            ReInitializeAnnoucements(isAdmin);
            Initialize();
        }
        public void SetIsAdmin(bool isAdmin_)
        {
            isAdmin = isAdmin_;
        }

        public void Initialize()
        {
            if (isAdmin == false)
            {
                MakeANCBtn.Visible = false;
                publishBtn.Visible = false;
                EditButton.Visible = false;
            }
            else
            {
                MakeANCBtn.Visible = true;
                publishBtn.Visible = true;
                EditButton.Visible = true;

            }
            HomeBtn.FlatStyle = FlatStyle.Flat;
            HomeBtn.FlatAppearance.BorderSize = 0;
            MakeANCBtn.FlatStyle = FlatStyle.Flat;
            MakeANCBtn.FlatAppearance.BorderSize = 0;
            publishBtn.FlatStyle = FlatStyle.Flat;
            publishBtn.FlatAppearance.BorderSize = 0;
            EditButton.FlatStyle = FlatStyle.Flat;
            EditButton.FlatAppearance.BorderSize = 0;
            panel3.Anchor = AnchorStyles.None;
            panel4.Visible = false;
            panel5.Visible = false;
            EditButton.Visible = false;
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            panel4.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            panel5.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
            foreach (Control item in panel4.Controls)
            {
                item.Anchor = AnchorStyles.None;
            }
            foreach (Control item in panel5.Controls)
            {
                if (item is PictureBox)
                {
                    item.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                }
                else
                {
                    item.Anchor = AnchorStyles.None;

                }

            }
        }
        public static IPAddress GetIP()
        {
            if (File.Exists("config.txt"))
            {
                //get current debug folder path
                try
                {

                    string currentPath = Directory.GetCurrentDirectory() + "\\config.txt";

                    string[] lines = File.ReadAllLines("config.txt");

                    IPAddress ip = IPAddress.Parse(lines[1]);
                    return ip;
                }
                catch (Exception)
                {
                    MessageBox.Show("Please open current path and put your own connection string in the file name \"config.txt\"");
                    return null;
                }
            }
            else
            {
                File.Create("config.txt").Close();
                MessageBox.Show("Please open current path and put your own connection string and IP for chat server in the file name \"config.txt\"");
                return null;

            }
        }
        private void HomeBtn_Click(object sender, EventArgs e)
        {
            if (panel4.Visible == true || panel5.Visible == true)
            {
                panel4.Visible = false;
                panel5.Visible = false;
                panel1.Visible = true;
            }
            else
            {
                panel1.Visible = true;
            }
            ReInitializeAnnoucements(isAdmin);
        }
        private void EditAnnoucement(int ID)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to edit this announcement?", "Edit", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }
            using (LibraryContext db = new LibraryContext())
            {
                Announcement announcement = db.Announcements.Find(ID);
                if (announcement != null)
                {
                    announcement.Subject = SubjectPrevLabel.Text;
                    announcement.From = FromPrevLabel.Text;
                    announcement.Content = ContentPrevTXTBOX.Text;
                    db.SaveChanges();
                    MessageBox.Show("Edited successfully");
                    isEditing = false;
                    EditButton.Visible = false;
                    AnnoucementID = 0;

                }
            }

        }

        private void ReInitializeAnnoucements(bool isAdmin)
        {
            panel1.Controls.Clear();
            panel1.AutoScroll = false;
            using (LibraryContext db = new LibraryContext())
            {
                var anncm = db.Announcements.OrderByDescending(x => x.Date).ToList();

                for (int i = 0; i <= anncm.Count - 1; i++)
                {
                    AnnoucementItem announcement = new AnnoucementItem(anncm[i]);
                    announcement.SetIsAdmin(isAdmin);
                    panel1.Controls.Add(announcement);
                    announcement.Location = new Point(0, i * 128);
                    announcement.Width = panel1.Width;
                }
            }
            panel1.AutoScroll = true;
        }

        internal void DeleteAnnoucement(int ID)
        {
            //show a message box to confirm the deletion
            DialogResult result = MessageBox.Show("Are you sure you want to delete this announcement?", "Delete", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }

            using (LibraryContext db = new LibraryContext())
            {
                var announcements = db.Announcements.ToList();
                foreach (var annoucement in announcements)
                {
                    if (annoucement.Id == ID)
                    {
                        db.Announcements.Remove(annoucement);
                        db.SaveChanges();
                        MessageBox.Show("Deleted successfully");
                    }
                }
            }
            ReInitializeAnnoucements(isAdmin);
        }
        internal void AnnouncementPanel_DoubleClick(string Content, string Subject, string Date, string From, string ID)
        {
            if (panel1.Visible == true)
            {
                panel1.Visible = false;
                panel5.Visible = true;
            }
            else
            {
                panel5.Visible = true;
            }
            int id = Convert.ToInt32(ID);
            ContentPrevTXTBOX.Text = Content;
            SubjectPrevLabel.Text = Subject;
            FromPrevLabel.Text = From;
            DatePrevLabel.Text = Date.ToString();
            isEditing = false;
            EditButton.Visible = false;
            ContentPrevTXTBOX.ReadOnly = true;
            SubjectPrevLabel.Visible = true;
            FromPrevLabel.Visible = true;
            //append the id to the txt of ReadAnnoucement.txt
            string writeText = (id.ToString() + ' ');
            if (!File.Exists("ReadAnnoucement.txt"))
            {
                using (StreamWriter sw = File.CreateText("ReadAnnoucement.txt"))
                {
                    sw.Write(writeText);
                    sw.Close();
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText("ReadAnnoucement.txt"))
                {
                    sw.Write(writeText);
                    sw.Close();

                }
            }


        }

        internal void ShowEditPanel(int ID)
        {
            AnnoucementID = ID;
            if (panel1.Visible == true)
            {
                panel1.Visible = false;
                panel5.Visible = true;
                EditButton.Visible = true;
            }
            else
            {
                panel5.Visible = true;
            }

            using (LibraryContext db = new LibraryContext())
            {
                var annoucement = db.Announcements.ToList();
                foreach (var ann in annoucement)
                {
                    if (ann.Id == ID)
                    {
                        ContentPrevTXTBOX.Text = ann.Content;
                        SubjectPrevLabel.Text = ann.Subject;
                        FromPrevLabel.Text = ann.From;
                        DatePrevLabel.Text = ann.Date.ToString();
                        ContentPrevTXTBOX.ReadOnly = false;
                        SubjectPrevLabel.ReadOnly = false;
                        FromPrevLabel.ReadOnly = false;
                        isEditing = false;
                    }
                }

            };
        }




        public class LibraryContext : DbContext
        {
            public LibraryContext() : base(ConfigurationManager.ConnectionStrings["IBC_STOREIIEntities"].ConnectionString) //Connection string to database
            {

            }
            public DbSet<Stock> Stocks { get; set; }
            public DbSet<Invoice> Invoices { get; set; }
            public DbSet<Announcement> Announcements { get; set; }
            public DbSet<User> Users { get; set; }

            public static string GetConnectionString()
            {
                if (File.Exists("config.txt"))
                {
                    string[] lines = File.ReadAllLines("config.txt");
                    //remove all spaces from the connection string
                    string connectionString = lines[0];
                    for (int i = 99; i < connectionString.Length; i++)
                    {
                        if (connectionString[i] == ' ')
                        {
                            connectionString = connectionString.Remove(i, 1);
                        }

                    }
                    if (connectionString.Contains("Default"))
                    {
                        MessageBox.Show("Please open current path and put your own connection string in the file name \"config.txt\"");
                        Environment.Exit(0);
                        return null;
                    }
                    else
                    {
                        return connectionString;

                    }

                }
                else
                {
                    File.Create("config.txt").Close();
                    MessageBox.Show("Please open current path and put your own connection string in the file name \"config.txt\"");
                    return null;
                }
            }

        }
      
        private void EditButton_Click(object sender, EventArgs e)
        {
            EditAnnoucement(AnnoucementID);
            HomeBtn_Click(sender, e);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (isEditing == true)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to exit without saving?", "Exit", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    panel4.Visible = false;
                    panel5.Visible = false;
                    panel1.Visible = true;
                    isEditing = false;
                    ReInitializeAnnoucements(isAdmin);
                }
            }
            else
            {
                panel4.Visible = false;
                panel5.Visible = false;
                panel1.Visible = true;
                ReInitializeAnnoucements(isAdmin);

            }

        }

        private void publishBtn_Click(object sender, EventArgs e)
        {
            Admin admin = new Admin();

            admin.MakeAnnouncement(SubjectTXTBOX.Text, FromTXT.Text, ContentTxtBox.Text);


        }

        private void MakeANCBtn_Click(object sender, EventArgs e)
        {
            if (panel1.Visible == true || panel5.Visible == true)
            {
                panel1.Visible = false;
                panel5.Visible = false;
                panel4.Visible = true;
            }
            else
            {
                panel4.Visible = true;
            }
        }


    }
}
