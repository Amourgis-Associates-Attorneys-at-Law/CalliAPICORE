using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CalliAPI.Models;

namespace CalliAPI.UI
{
    public partial class ReportForm : Form
    {
        public ReportForm()
        {
            InitializeComponent();
        }

        public void SetData(DataTable dataTable)
        {
            // Check if the DataTable is null or empty
            if (dataTable == null || dataTable.Columns.Count == 0)
            {
                MessageBox.Show("No data available to display.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            dataGridView1.DataSource = dataTable;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.ColumnHeadersVisible = true; // Ensure headers are visible

            // Breakpoint
            Thread.Sleep(10);
        }

        private void saveToExcel_Click(object sender, EventArgs e)
        {

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel Files (*.xlsx)|*.xlsx";
                sfd.Title = "Save Report As Excel File";
                sfd.FileName = "MattersReport.xlsx";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ExportDataGridViewToExcel(dataGridView1, sfd.FileName);
                    MessageBox.Show("Export complete!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }


        private void ExportDataGridViewToExcel(DataGridView dgv, string filePath)
        {
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Matters");

            // Add headers
            for (int col = 0; col < dgv.Columns.Count; col++)
            {
                worksheet.Cell(1, col + 1).Value = dgv.Columns[col].HeaderText;
            }

            // Add rows
            for (int row = 0; row < dgv.Rows.Count; row++)
            {
                for (int col = 0; col < dgv.Columns.Count; col++)
                {
                    worksheet.Cell(row + 2, col + 1).Value = dgv.Rows[row].Cells[col].Value?.ToString();
                }
            }

            workbook.SaveAs(filePath);
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            var table = dataGridView1.DataSource as DataTable;
            if (table == null) return;

            string filterText = txtFilter.Text.Replace("'", "''"); // Escape single quotes

            var filterConditions = table.Columns
         .Cast<DataColumn>()
         .Select(col => $"CONVERT([{col.ColumnName}], System.String) LIKE '%{filterText}%'");

            table.DefaultView.RowFilter = string.Join(" OR ", filterConditions);

        }

    }
}
