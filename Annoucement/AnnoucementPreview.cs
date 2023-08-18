using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IBC_Management_System.Annoucement
{
    public partial class AnnoucementPreview : UserControl
    {
        public AnnoucementPreview(Announcement announcement)

        {
            InitializeComponent();
                 this.Anchor = (AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top);
            SubjectPrevLabel.Text = announcement.Subject;
            DateLabel.Text = announcement.Date.ToString();
            ContentPrevTXTBOX.Text = announcement.Content;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.components.Dispose();
        }
    }
}
