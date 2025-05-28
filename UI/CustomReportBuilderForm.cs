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
using CalliAPI.Interfaces;
using CalliAPI.Models;
using CalliAPI.UI.Controls;
using CalliAPI.UI.Views;
using CalliAPI.Utilities;
using Task = System.Threading.Tasks.Task;

namespace CalliAPI.UI
{
    public partial class CustomReportBuilderForm : Form
    {
        private readonly ClioService _clioService;
        public CustomReportBuilderForm(ClioService clioService)
        {
            _clioService = clioService ?? throw new ArgumentNullException(nameof(clioService));
            InitializeComponent();
            // Initialize the content panel with controls
            setView();

        }

        private void setView()
        {
            // Get the selected item from the combo box
            string selectedItem = comboBox1.SelectedItem?.ToString();

            // Prepare the appropriate View (in UI/Views) as well as the clioService if needed
            UserControl view = selectedItem switch
            {
                "matters" => new MatterQueryControl(_clioService),
                _ => null
            };

            if (view != null)
            {
                panelContent.Controls.Clear(); // Remove previous view
                panelContent.Controls.Add(view); // Add new view
                view.Dock = DockStyle.Fill; // Make it fill the panel
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            setView();
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            var activeControl = panelContent.Controls.OfType<Control>().FirstOrDefault();

            if (activeControl is IQueryBuilder<Matter> matterBuilder)
            {
                //string fields = matterBuilder.SelectedFields;
                //var filters = matterBuilder.SelectedFilters;

                //IAsyncEnumerable<Matter> results = _clioService.GetAllMattersAsync(fields);

                //foreach (var filter in filters)
                //{
                //    results = filter(results);
                //}

                //await ReportLauncher.ShowAsync(results);
                await QueryExecutor.ExecuteAsync(matterBuilder, _clioService);
            }
            else if (activeControl is IQueryBuilder<CalliAPI.Models.Task> taskBuilder)
            {
                await QueryExecutor.ExecuteAsync(taskBuilder, _clioService);
            }


        }
    }
}
