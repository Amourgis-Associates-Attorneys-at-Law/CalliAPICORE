/*
 * CalliAPI Mailer
 * 2025 Mar 28
 * Jacob Hayes
 * The goal of this program is as follows:
 *  * A Webhook listens for any Clio documents being commented on.
 *  * When a document is commented on, either "mail" or "cancel", it will update the database.
 *  * This program has a DataGridView that displays the current status of all documents in the database.
 *  * In Form1_Load(), we add a button column to the DataGridView that allows us to print the document.
 *  
 *  P.S. - Calliope is the head of the Greek Muses, of whom Clio is one. I figured it was the perfect name to handle all our API requests from Clio,
 *  especially since the name CalliAPI was right there.
 */

using MaterialSkin;
using MaterialSkin.Controls;

namespace CalliAPI_Mailer
{
    public partial class Form1 : MaterialForm
    {
        public Form1()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create a new button column
            DataGridViewButtonColumn printButtonColumn = new DataGridViewButtonColumn();
            printButtonColumn.Name = "PrintButton";
            printButtonColumn.HeaderText = "Print";
            printButtonColumn.Text = "Print";
            printButtonColumn.UseColumnTextForButtonValue = true;

            // Create a new ignore button column
            DataGridViewButtonColumn ignoreButtonColumn = new DataGridViewButtonColumn();
            ignoreButtonColumn.Name = "IgnoreButton";
            ignoreButtonColumn.HeaderText = "Ignore";
            ignoreButtonColumn.Text = "Ignore";
            ignoreButtonColumn.UseColumnTextForButtonValue = true;

            // Add the new columns to the DataGridView
            dataGridView1.Columns.Add(printButtonColumn);
            dataGridView1.Columns.Add(ignoreButtonColumn);


        }

        // Handle the CellClick event


        /// <summary>
        /// This function checks to see if the user has clicked on the Print or Ignore buttons in the database, and updates the items inside as needed.
        /// </summary>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is a button cell

            if (e.ColumnIndex == dataGridView1.Columns["PrintButton"].Index && e.RowIndex >= 0)
            {
                // Get the data from the selected row
                var selectedRow = dataGridView1.Rows[e.RowIndex];
                var documentId = selectedRow.Cells["DocumentID"].Value;
                var documentUrl = selectedRow.Cells["DocumentURL"].Value;

                // Call your print method
                PrintDocument(documentId, documentUrl);
            }
            else if (e.ColumnIndex == dataGridView1.Columns["IgnoreButton"].Index && e.RowIndex >= 0)
            {
                // Handle the ignore button click
                var selectedRow = dataGridView1.Rows[e.RowIndex];
                var documentId = selectedRow.Cells["DocumentID"].Value;
                var documentUrl = selectedRow.Cells["DocumentURL"].Value;
                // Implement your ignore logic here
                MessageBox.Show($"Ignoring document with ID: {documentId} and URL: {documentUrl}");
                // You can add logic to update the database or perform other actions
            }
        }

        private void PrintDocument(object documentId, object documentUrl)
        {
            // Implement your print logic here
            MessageBox.Show($"Printing document with ID: {documentId} and URL: {documentUrl}");
            //throw new NotImplementedException();
        }
    }
}
