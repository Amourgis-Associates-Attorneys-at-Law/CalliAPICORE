using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CalliAPI.BusinessLogic;
using CalliAPI.Utilities;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CalliAPI.UI.Views
{
    public partial class ReportsOpenMattersUnworkedMatters : UserControl
    {
        private KeywordTooltipManager _tooltipManager;
        private readonly ClioService _clioService;

        public ReportsOpenMattersUnworkedMatters(ClioService clioService)
        {
            _clioService = clioService;
            InitializeComponent();
            // Initialize the keyword tooltip manager
            _tooltipManager = new KeywordTooltipManager(richTextBox1, toolTip1, TooltipDefinitions.Keywords);
            _tooltipManager.HighlightKeywords();

        }

        private async void btnLaunch_Click(object sender, EventArgs e)
        {
            await _clioService.GetUnworked713Matters();
        }
    }
}
