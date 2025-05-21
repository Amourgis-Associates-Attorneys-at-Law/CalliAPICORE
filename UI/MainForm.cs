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

namespace CalliAPI
{
    public partial class MainForm : Form
    {
        private readonly IAuthService _authService; // for authorizing OAuth
        private readonly ClioService _clioService; // for calling the API
        private static readonly AMO_Logger _logger = new AMO_Logger("CalliAPI");
        private static readonly int MaxLookbackDays = 300;

        internal MainForm(ClioService clioService, IAuthService authService)
        {
            _authService = authService;
            _clioService = clioService;
            InitializeComponent();
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                StartOAuthProcess();
                Thread.Sleep(2000);
                UpdateClioAPIStatus();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            clioService.ProgressUpdated += (current, total) =>
            {
                this.progressBarPagesRetrieved.Invoke(() =>
                {
                    progressBarPagesRetrieved.Maximum = total;
                    progressBarPagesRetrieved.Value = Math.Min(current, total);
                    lblReportPageRetrieved.Text = $"Page {current} of {total} obtained from Clio.";
                });
            };
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
            var authorizationUrl = _authService.GetAuthorizationUrl();
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
                string userInput = Microsoft.VisualBasic.Interaction.InputBox(
                "Please enter the authorization code from the URL:",
                "Authorization Code",
                ""
                );

                authorizationCode = _authService.ValidateAuthorizationCode(userInput);
                // will return null if it's not the proper code, so we can break out if that's the case
                if (!string.IsNullOrEmpty(authorizationCode))
                    break;

                MessageBox.Show("Invalid authorization code. Please try again.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            await _authService.GetAccessTokenAsync(authorizationCode);
            MessageBox.Show(_authService.AccessToken, "Access Token");

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
    }
}
