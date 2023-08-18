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

namespace IBC_Management_System.Annoucement
{
    public partial class AnnoucementItem : UserControl
    {
        public static Announcement an;
        public static int ID;
        public static bool isRead = false;
        public static bool IsAdmin = true;
        public AnnoucementItem(Announcement Annoucement_)
        {
            an = Annoucement_;
            InitializeComponent();
            this.Anchor = (AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top);
            string[] dateText = Annoucement_.Date.ToString().Split(' ');
            ID = Annoucement_.Id;
            DateLabel.Text = dateText[0].ToString();
            TimeLabel.Text = dateText[1].ToString();
            AnnoucementHeaderLabel.Text = Annoucement_.Subject.ToString();
            AnnoucementContentText.Text = Annoucement_.Content.ToString();
            AnnoucementID.Text = Annoucement_.Id.ToString();
            FromLabel.Text = Annoucement_.From.ToString();
            if (File.Exists("ReadAnnoucement.txt") == false)
            {
                File.Create("ReadAnnoucement.txt").Close();
            }

            try
            {
                string readAnnoucement = File.ReadAllText("ReadAnnoucement.txt");
                string[] readAnnoucementToArray = readAnnoucement.Split(' ');
                if (readAnnoucementToArray.Length > 0)
                {
                    foreach (string s in readAnnoucementToArray)
                    {
                        if (s == Annoucement_.Id.ToString())
                        {
                            isRead = true;
                        }
                    }

                }


                if (isRead == true)
                {
                    pictureBox1.Visible = false;
                    isRead = false;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
            this.BorderStyle = BorderStyle.FixedSingle;
            this.BorderColor = Color.LightBlue;


        }
        private Color _borderColor = Color.Black;

        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }
        public void SetIsAdmin(bool isAdmin)
        {
            IsAdmin = isAdmin;
            if (IsAdmin == false)
            {
                DeleteIcon.Visible = false;
                EditIcon.Visible = false;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw the border using the BorderColor property
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, _borderColor, ButtonBorderStyle.Solid);
        }



        private void AnnoucementItem_DoubleClick(object sender, EventArgs e)
        {
            var mainForm = this.ParentForm as AnnoucementForm;
            mainForm.AnnouncementPanel_DoubleClick(this.AnnoucementContentText.Text, this.AnnoucementHeaderLabel.Text, this.DateLabel.Text, this.FromLabel.Text, this.AnnoucementID.Text);
        }
        internal static Announcement GetValue()
        {
            return an;
        }

        private void EditIcon_Click(object sender, EventArgs e)
        {
            var mainForm = this.ParentForm as AnnoucementForm;
            int AnnoucementID = Convert.ToInt32(this.AnnoucementID.Text);
            mainForm.ShowEditPanel(AnnoucementID);
        }

        private void DeleteIcon_Click(object sender, EventArgs e)
        {
            var mainForm = this.ParentForm as AnnoucementForm;
            int AnnoucementID = Convert.ToInt32(this.AnnoucementID.Text);
            mainForm.DeleteAnnoucement(AnnoucementID);
        }
    }
}
