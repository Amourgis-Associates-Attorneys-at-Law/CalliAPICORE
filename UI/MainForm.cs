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
using CalliAPI_Mailer.UI;

namespace CalliAPI
{
    public partial class MainForm : Form
    {
        private readonly IAuthService _authService; // for authorizing OAuth
        private readonly IClioService _clioService; // for calling the API

        internal MainForm(IClioService clioService, IAuthService authService)
        {
            _authService = authService;
            _clioService = clioService;
            InitializeComponent();
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
            string status = await _clioService.VerifyAPI();
            lblClioAPIStatus.Text = status;
        }

        private async void unworkedMattersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get the data we need
            DataTable mattersTable = await _clioService.GetMattersNotCurrentlyBeingWorkedAsDataTable();

            // Show it in a new Report Form
            ReportForm reportForm = new ReportForm();
            reportForm.SetData(mattersTable);
            reportForm.Show();


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
            DataTable mattersTable = await _clioService.GetAllMattersAsDataTable();

            // Show it in a new Report Form
            ReportForm reportForm = new ReportForm();
            reportForm.SetData(mattersTable);
            reportForm.Show();
        }

        /// <summary>
        /// This method is called when the "Connect to Clio" button is clicked. It starts the OAuth process to allow for authorized API calls to Clio. The user will be prompted to enter the authorization code after authorizing the app in the browser,
        /// and the resulting access token will be stored in the authService.
        /// </summary>
        private async void StartOAuthProcess()
        {
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
    }
}
