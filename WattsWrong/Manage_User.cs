using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WattsWrong.EquipmentMonitor;
using System.Text.RegularExpressions;

namespace WattsWrong
{
    public partial class Manage_User : Form
    {
        private EquipmentMonitor userMonitor = new EquipmentMonitor();
        public Manage_User()
        {
            InitializeComponent();
            List<UserReading> userReadings = userMonitor.GetReportingMembers();

            foreach (var reading in userReadings)
            {
                listBox1.Items.Add(reading.Email);
            }
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
            if(textBox1.Text == "")
            {
                MessageBox.Show("Please Enter a Name!");
            }
            if (!verifyEmail( textBox2.Text))
            {
                MessageBox.Show("Please Enter a valid Email address!");
            }
            else
            {
                try
                {
                    userMonitor.AddReportingMember(textBox1.Text, textBox2.Text);
                    listBox1.Items.Add(textBox2.Text);
                    MessageBox.Show("Member Added!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to Add member, Try again");
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Please Select a Valid Member!");
            }
            else
            {
                userMonitor.DeleteReportingMember(listBox1.SelectedItem.ToString());
                listBox1.Items.Remove(listBox1.SelectedItem);
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
