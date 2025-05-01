using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace WattsWrong
{
    public partial class Report_Gen : Form
    {
        public string email;
        public Boolean validEmail;
        public Boolean save_to_drive;
        public string save_file_loc;
        public string start_date , end_date;
        public Report_Gen()
        {
            InitializeComponent();
            this.email = "";
            this.validEmail = false;
            this.save_to_drive = false;
            this.save_file_loc = Environment.SpecialFolder.Desktop.ToString();
        }

        private Boolean verifyEmail(string st)
        {
            if (string.IsNullOrWhiteSpace(st))
                return false;  // Reject empty or whitespace emails

            // Regular expression pattern for valid email format
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            return Regex.IsMatch(st, pattern);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Report_Gen.DialogResult.OK;
            if (!select_email.Checked && !select_to_drive.Checked) {
                MessageBox.Show( "Please Select Receiving Mode!" );
                
            }
            else
            {
                this.email = email_address.Text;
                this.validEmail = verifyEmail(email);
                this.save_to_drive = select_to_drive.Checked ? true : false;
                this.start_date = dateTimePicker1.Value.Date.ToString("yyyy-MM-dd");
                this.end_date = dateTimePicker2.Value.Date.ToString("yyyy-MM-dd");
                // this.start_date = dateTimePicker1.Value.;
                // this.end_date = dateTimePicker2.Text;

                this.DialogResult = DialogResult.OK;
            }
        }

        private void Report_Gen_Load(object sender, EventArgs e)
        {

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            FolderBrowserDialog browse_loc = new FolderBrowserDialog();
            browse_loc.Description = "Please Select a Drive";
            browse_loc.RootFolder = Environment.SpecialFolder.MyComputer;
            if(browse_loc.ShowDialog() == DialogResult.OK)
            {
                save_file_loc = browse_loc.SelectedPath;
            }
        }

       
    }
}
