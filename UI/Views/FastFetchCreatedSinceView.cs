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
using CalliAPI.UI.Forms;

namespace CalliAPI.UI.Views
{
    public partial class FastFetchCreatedSinceView : UserControl
    {
        private ClioService _clioService;
        private readonly IReportContext _context;

        public FastFetchCreatedSinceView(ClioService clioService, IReportContext context)
        {
            _clioService = clioService;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            InitializeComponent();
        }

        private const int MaxLookbackDays = 365; // Maximum lookback period in days
        public string reportName;

        private async void btnLaunch_Click(object sender, EventArgs e)
        {
            await FastFetchAllMatters();
        }

        private async Task FastFetchAllMatters()
        {
            DateTime selectedDate;

            while (true)
            {
                using (var datePicker = new DatePickerDialog(MaxLookbackDays)) // Custom form or use DateTimePicker in a dialog
                {
                    if (datePicker.ShowDialog() != DialogResult.OK)
                        return; // User cancelled

                    selectedDate = datePicker.SelectedDate;

                    if (IsValidLookbackDate(selectedDate))
                        break;

                    MessageBox.Show($"Please select a date within the last {MaxLookbackDays} days.",
                    "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                reportName = $"Report: All Matters since {selectedDate.ToShortDateString()}";
                _context.SetReportName(reportName);
                await _clioService.FastFetchAllMatters(selectedDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private static bool IsValidLookbackDate(DateTime selectedDate)
        {
            var earliestAllowed = DateTime.UtcNow.Date.AddDays(-MaxLookbackDays);
            return selectedDate >= earliestAllowed;
        }

        

    }
}
