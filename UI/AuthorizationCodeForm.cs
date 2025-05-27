using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalliAPI.UI
{
    public partial class AuthorizationCodeForm : Form
    {
        public string AuthorizationCode => textBoxCode.Text;


        public AuthorizationCodeForm()
        {
            InitializeComponent();
            this.AcceptButton = btnOK;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
