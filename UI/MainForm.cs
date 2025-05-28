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
using Microsoft.Win32;
using DocumentFormat.OpenXml.Bibliography;
using System.Net;

namespace CalliAPI
{
    public partial class MainForm : Form, IReportContext
    {
        private readonly ClioService _clioService; // for calling the API
        private readonly AMO_Logger _logger;
        private static readonly int MaxLookbackDays = 300; // for picking FastFetch start date
        private DateTime _lastUpdate = DateTime.MinValue; // for updating the progress bar only if a certain amount of time has passed
        private int _lastPercent = -1; // for updating the progress bar only if it's actually changed

        public void SetReportName(string name) { lblReportName.Text = name; }

        internal MainForm(ClioService clioService, AMO_Logger logger)
        {

            _clioService = clioService;
            _logger = logger;
            InitializeComponent();

            this.Text = VersionHelper.GetDisplayVersion();
            lblVersion.Text = VersionHelper.GetFormattedVersion();



            clioService.ProgressUpdated += (current, total) =>
            {
                SetProgress(current, total);
            };

        }

        public void SetProgress(int current, int total)
        {
            if (total <= 0)
            {
                progressBarPagesRetrieved.Value = 0;
                UpdateReportLabel("No pages to retrieve.");
                return;
            }



            int percent = Math.Min(100, (int)((current / (double)total) * 100));
            progressBarPagesRetrieved.Value = percent;

            progressBarPagesRetrieved.Maximum = 100;
            UpdateReportLabel($"Page {current} of {total} obtained from Clio.");
        }


        private void UpdateReportLabel(string text)
        {
            lblReportPageRetrieved.Text = text;
            lblReportPageRetrieved.AutoSize = true;



            // Clamp the left side
            int newLeft = progressBarPagesRetrieved.Right - lblReportPageRetrieved.Width;

            // Reposition the label so its right edge aligns with the progress bar's right edge
            lblReportPageRetrieved.Left = Math.Max(progressBarPagesRetrieved.Left, newLeft);
            lblReportPageRetrieved.Top = progressBarPagesRetrieved.Top + 2; // Adjust as needed
        }

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
                HttpResponseMessage status = await _clioService.VerifyAPI();
                lblClioAPIStatus.Text = status.IsSuccessStatusCode
                    ? "Clio API Status: Connected"
                    : $"Clio API Status: Error {status.StatusCode}";
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


        private async void allMattersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logger.Info("Report Start: All Matters");
            lblReportName.Text = "Report: All Matters";
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                await _clioService.GetAllMattersAsync(); // or whatever method you're calling
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
        private async Task<bool> StartOAuthProcess()
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
                        return false;
                    }

                    authorizationCode = _clioService.ValidateAuthorizationCode(form.AuthorizationCode);

                    if (!string.IsNullOrEmpty(authorizationCode))
                        break;

                    MessageBox.Show("Invalid authorization code. Please try again.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }


            await _clioService.GetAccessTokenAsync(authorizationCode);
            MessageBox.Show("Access Token successfully obtained!");
            return true;

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
                    "FastFetchCreatedSince" => new FastFetchCreatedSinceView(_clioService, this),
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

        private async void toolStripBtnConnectToClio_Click(object sender, EventArgs e)
        {
            try
            {
                int attempt = 0;
                int maxAttempts = 5; // Set the maximum number of attempts
                lblClioAPIStatus.Text = "Attempting to connect to Clio API...";
                Cursor.Current = Cursors.WaitCursor;
                // Start the OAuth process and get the access token
                bool userAuthorized = await StartOAuthProcess();

                if (userAuthorized)
                {

                    HttpResponseMessage httpResponse = await _clioService.VerifyAPI();

                    while (!httpResponse.IsSuccessStatusCode)
                    {
                        attempt++;
                        Thread.Sleep(3000);
                        httpResponse = await _clioService.VerifyAPI();

                        _logger.Info($"Attempt {attempt} of {maxAttempts} to connect to Clio API. Status: {httpResponse.StatusCode}. {await httpResponse.Content.ReadAsStringAsync()}");
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            lblClioAPIStatus.Text = "Clio API Status: Connected";
                            MessageBox.Show("Successfully connected to Clio API!", "Connection Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }
                        lblClioAPIStatus.Text = $"Attempt {attempt} of {maxAttempts} to connect to Clio API failed: {httpResponse.StatusCode}.";
                        if (attempt >= maxAttempts) throw new Exception(httpResponse.ReasonPhrase);
                    }

                    UpdateClioAPIStatus();
                }
                else
                {
                    lblClioAPIStatus.Text = "Authorization cancelled or failed.";
                    MessageBox.Show("Authorization was cancelled or failed. Please try again.", "Authorization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                lblClioAPIStatus.Text = "Error connecting to Clio API.";
                MessageBox.Show($"An error occurred while connecting to the Clio API: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private async void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Checking for updates... This may take a moment.");
                Cursor.Current = Cursors.WaitCursor;
                await VersionHelper.UpdateCalliAPI();
                MessageBox.Show("Update check complete. If an update was found, it has been applied. Please restart the application to see the changes.", "Update Check Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while checking for updates: {ex.Message}", "Update Check Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private async void Debug100Percent(object sender, EventArgs e)
        {
            for (int x = 1; x < 100; x++)
            {
                await Task.Delay(100); // non-blocking delay
                SetProgress(x, 100);
            }
        }

        private void resetRegistryKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the Clio Client Secret from the registry? This will require you to reauthorize the application.", "Confirm Clear Client Secret", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                if (MessageBox.Show("This will clear the Clio Client Secret from the registry. Are you sure?", "Confirm Clear Client Secret", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    ClearClientSecretFromRegistry();
        }


        public static void ClearClientSecretFromRegistry()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\CalliAPI", writable: true);
            key?.DeleteValue("ClioClientSecret", false);
        }

        private void customReportBuilderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomReportBuilderForm customReportBuilderForm = new CustomReportBuilderForm(_clioService);
            customReportBuilderForm.Show();
        }
    }
}
