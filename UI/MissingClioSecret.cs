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
    public partial class MissingClioSecret : Form
    {
        public string ClientSecret => txtSecret.Text;
        public MissingClioSecret()
        {
            InitializeComponent();
        }

    }
}
