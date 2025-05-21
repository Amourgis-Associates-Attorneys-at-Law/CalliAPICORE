using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalliAPI.UI
{


    public class DatePickerDialog : Form
    {
        public DateTime SelectedDate => dateTimePicker.Value;

        private DateTimePicker dateTimePicker;

        public DatePickerDialog(int MaxLookbackDays)
        {
            dateTimePicker = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                MinDate = DateTime.Today.AddDays(-MaxLookbackDays),
                MaxDate = DateTime.Today,
                Value = DateTime.Today.AddDays(-MaxLookbackDays)
            };

            var okButton = new Button { Text = "OK", DialogResult = DialogResult.OK };
            var cancelButton = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel };

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Controls = { okButton, cancelButton }
            };

            Controls.Add(dateTimePicker);
            Controls.Add(buttonPanel);

            AcceptButton = okButton;
            CancelButton = cancelButton;
        }
    }


}
