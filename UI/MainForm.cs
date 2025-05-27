/* MainForm.cs
 * Jacob Hayes
 * 2025 04 21
 */

using CalliAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks; // for async
using System.Windows.Forms;
using CalliAPI.Interfaces;
using CalliAPI.BusinessLogic;
using CalliAPI.UI;
using Task = System.Threading.Tasks.Task;
using AmourgisCOREServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using CalliAPI.UI.Views;
using CalliAPI.Utilities;
using CalliAPI.UI.Controls;

namespace CalliAPI
{
    public partial class MainForm : Form
    {
        private readonly ClioService _clioService; // for calling the API
        private readonly AMO_Logger _logger;
        private static readonly int MaxLookbackDays = 300; // for picking FastFetch start date
        private DateTime _lastUpdate = DateTime.MinValue; // for updating the progress bar only if a certain amount of time has passed

        private int _lastPercent = -1; // for updating the progress bar only if it's actually changed


        internal MainForm(ClioService clioService, AMO_Logger logger)
        {
            _clioService = clioService;
            _logger = logger;
            InitializeComponent();

            this.Text = VersionHelper.GetDisplayVersion();
            lblVersion.Text = VersionHelper.GetFormattedVersion();



            clioService.ProgressUpdated += (current, total) =>
            {
                this.progressBarPagesRetrieved.Invoke(() =>
                {
                    if (total == 0) return; // Avoid division by zero

                    var now = DateTime.Now;
                    if ((now - _lastUpdate).TotalMilliseconds < 500) return; // Only update every 0.5s
                    _lastUpdate = now;


                    int percent = (int)((current / (double)total) * 100);

                    if (percent == _lastPercent) return; // Only update if the percent has changed
                    _lastPercent = percent;
                    progressBarPagesRetrieved.Value = percent;
                    UpdateReportLabel($"Page {current} of {total} obtained from Clio.");
                });
            };
        }


        private void UpdateReportLabel(string text)
        {
            lblReportPageRetrieved.Text = text;
            lblReportPageRetrieved.AutoSize = true;

            // Reposition the label so its right edge aligns with the progress bar's right edge
            lblReportPageRetrieved.Left = progressBarPagesRetrieved.Right - lblReportPageRetrieved.Width;
            lblReportPageRetrieved.Top = progressBarPagesRetrieved.Top + 2; // Adjust as needed
        }



        #region logic

        private static bool IsValidLookbackDate(DateTime selectedDate)
        {
            var earliestAllowed = DateTime.UtcNow.Date.AddDays(-MaxLookbackDays);
            return selectedDate >= earliestAllowed;
        }

        #endregion

        /// <summary>
        /// This method is called when the "Open Mailer Form" button is clicked. Opens a new MailerForm.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenMailerForm_Click(object sender, EventArgs e)
        {
            openNewMailer();

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void openNewMailer()
        {
            var mailerForm = new MailerForm();

            mailerForm.Show();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openNewMailer();
        }


        private async void UpdateClioAPIStatus()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                string status = await _clioService.VerifyAPI();
                lblClioAPIStatus.Text = status;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void lblClioAPIStatus_Click(object sender, EventArgs e)
        {
            UpdateClioAPIStatus();
        }

        private void connectToClioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartOAuthProcess(); UpdateClioAPIStatus();
        }

        private async void allMattersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger.Info("Report Start: All Matters");
            lblReportName.Text = "Report: All Matters";
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                await _clioService.GetAllMatters(); // or whatever method you're calling
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

        }

        /// <summary>
        /// This method is called when the "Connect to Clio" button is clicked. It starts the OAuth process to allow for authorized API calls to Clio. The user will be prompted to enter the authorization code after authorizing the app in the browser,
        /// and the resulting access token will be stored in the authService.
        /// </summary>
        private async void StartOAuthProcess()
        {
            _logger.Info("Starting OAuth process...");
            // Open a browser and make the front-end OAuth call in the URL
            var authorizationUrl = _clioService.GetAuthorizationUrl();
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = authorizationUrl,
                UseShellExecute = true
            };

            MessageBox.Show("Please authorize the application in the browser that opens. After authorization, you will be redirected to a URL. Copy the authorization code from that URL and paste it into the input box that will appear.",
            "Authorization", MessageBoxButtons.OK, MessageBoxIcon.Information);

            System.Diagnostics.Process.Start(psi);


            // Wait for the user to enter the authorization code correctly

            string authorizationCode = string.Empty;

            while (true)
            {
                using (var form = new AuthorizationCodeForm())
                {
                    var result = form.ShowDialog();

                    if (result == DialogResult.Cancel)
                    {
                        MessageBox.Show("Authorization cancelled.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    authorizationCode = _clioService.ValidateAuthorizationCode(form.AuthorizationCode);

                    if (!string.IsNullOrEmpty(authorizationCode))
                        break;

                    MessageBox.Show("Invalid authorization code. Please try again.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }


            await _clioService.GetAccessTokenAsync(authorizationCode);
            MessageBox.Show("Access Token successfully obtained!");

        }

        private async void allMattersToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            await FastFetchAllMatters();

        }

        /// <summary>
        /// Grab a date from the last MaxLookbackDays days and then call a report to get all matters created_since that date
        /// </summary>
        /// <returns></returns>
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
                lblReportName.Text = $"Report: All Matters since {selectedDate.ToShortDateString()}";
                _logger.Info($"Report Start: All Matters since {selectedDate.ToShortDateString()}");
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

        private async void createdSinceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lblReportName.Text = "Report: All Matters since <date>";
            await FastFetchAllMatters();
        }

        private void unworkedMattersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature is not yet implemented.", "Feature Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void all713MattersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lblReportName.Text = "Report: All Open 7 & 13 Matters";
            await _clioService.GetAllOpen713Matters();
        }

        private async void allUnworked7And13MattersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lblReportName.Text = "Report: All Unworked 7 & 13 Matters";
            await _clioService.GetUnworked713Matters();
        }

        private void treeViewReports_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Grab the selected node and check if it has a tag
            if (e.Node.Tag is string viewName)
            {

                // Prepare the appropriate View (in UI/Views) as well as the clioService if needed
                UserControl view = viewName switch
                {
                    "Reports" => new ReportsView(),
                    "ReportsOpenMattersUnworkedMatters" => new ReportsOpenMattersUnworkedMatters(_clioService),
                    //"Open713MattersView" => new Open713MattersView(_clioService),
                    _ => null
                };

                if (view != null)
                {
                    panelContent.Controls.Clear(); // Remove previous view
                    panelContent.Controls.Add(view); // Add new view
                    view.Dock = DockStyle.Fill; // Make it fill the panel
                }
            }

        }

        private void toolStripBtnConnectToClio_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                StartOAuthProcess();
                Thread.Sleep(1000);
                UpdateClioAPIStatus();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private async void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await VersionHelper.UpdateCalliAPI();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Debug100Percent(object sender, EventArgs e)
        {
            progressBarPagesRetrieved.Value = 0;
            for (int x = 0; x < 100; x++)
            {
                Thread.Sleep(100);
                progressBarPagesRetrieved.Value += 1;
            }
        }
    }
}
