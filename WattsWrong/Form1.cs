using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.IO;
using static WattsWrong.EquipmentMonitor;
using static WattsWrong.BackUpSql;
using System.Runtime.InteropServices;


namespace WattsWrong
{
    public partial class Form1 : Form
    {
        private SerialPort _serialPort;
        private const int RetryInterval = 5000; // 5 seconds between retries
        private bool isConnected = false;
        private EquipmentMonitor equipmentMonitor = new EquipmentMonitor();
        private BackUpSql new_BackUp = new BackUpSql();

       // [DllImport("kernel32.dll")]
       // private static extern uint SetThreadExecutionState(uint esFlags);
        const uint ES_CONTINUOUS = 0x80000000;
        const uint ES_SYSTEM_REQUIRED = 0x00000001;
        // Optional: prevents display from turning off
        private const uint ES_DISPLAY_REQUIRED = 0x00000002;

        public Form1()
        {
            try { 
            InitializeComponent();
            
            StartSerialConnection(); // Start the connection setup
            }
            catch (Exception ex)
            {
                File.WriteAllText("startup_error.txt", ex.ToString());
            }

        }

       
        private void sendEmail(string recipientEmail)
        {
            // Set up email configura tion
            string smtpServer = "smtp.gmail.com";
            int port = 587;
            string senderEmail = "wattswrong2024@gmail.com";
            string senderPassword = "xxxxx";


            // Initialize the EmailSender class
            EmailSender emailSender = new EmailSender(smtpServer, port, senderEmail, senderPassword);

            // Set up email details
            
            string subject = "Watt's Wrong Report";
            string body = "Dear Recipient,<br><br>Please find the attached Watt's Wrong Equipment report.<br><br>Best regards,<br>Watts Wrong";
            
            string pdfFilePath = "C:\\Users\\LattePanda\\Desktop\\WattsWrong_Report.pdf";

            // Send the email
            emailSender.SendEmail(recipientEmail, subject, body, pdfFilePath, true);
            //MessageBox.Show("Email Sent","Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        private void getPDF(List<EquipmentReading> voltage_reading, List<EquipmentReading> current_reading , List<EquipmentReading> power_reading, string outputPath )

        {
           
                // Define the output path for the PDF
                //string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "WattsWrongReport.pdf");

                string save_outputPath = Path.Combine(outputPath, "WattsWrong_Report.pdf");
                // Create an instance of the WattsWrongPDF class
                WattsWrongPDF pdfGenerator = new WattsWrongPDF(voltage_reading, current_reading, power_reading);

                // Call the method to generate the PDF
                pdfGenerator.GeneratePDF(save_outputPath);

                // Inform the user that the PDF was created
                MessageBox.Show($"PDF generated successfully at: {outputPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
          
                // Handle any errors that occur during PDF generation
              //   MessageBox.Show($"An error occurred while generating the PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            

        }

        private void StartSerialConnection()
        {
            Thread connectionThread = new Thread(() =>
            {
                while (!isConnected)
                {
                    try
                    {
                       // Thread.Sleep(9000);
                        SetupSerialPort();
                        isConnected = true;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        ShowMessage($"Access denied to COM port: {ex.Message}. Retrying...");
                    }
                    catch (IOException ex)
                    {
                        ShowMessage($"Error accessing port: {ex.Message}. Retrying...");
                    }
                    catch (Exception ex)
                    {
                        ShowMessage($"Unexpected error: {ex.Message}. Retrying...");
                    }

                    if (!isConnected)
                    {
                        Thread.Sleep(RetryInterval); // Wait before retrying
                    }
                }
            });
            
            connectionThread.IsBackground = true;
            connectionThread.Start();
        }

        private void ShowMessage(string message)
        {
            Invoke((MethodInvoker)delegate
            {
                MessageBox.Show(message);
            });
        }

        private void SetupSerialPort()
        {
            _serialPort = new SerialPort("COM7", 9600); // Set  COM port
            _serialPort.DtrEnable = true;
            _serialPort.RtsEnable = true;
            _serialPort.DataReceived += SerialPort_DataReceived;
            _serialPort.Open();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = _serialPort.ReadLine().Trim(); // Example: "A:120V,10A;B:121V,11A;C:122V,9A;"
                Invoke((MethodInvoker)delegate
                {
                    ParseAndDisplayData(data);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading data: {ex.Message}");
            }
        }

        private void ParseAndDisplayData(string data)
        {
            string[] phaseReadings = data.Split(';');

            double totalpower = 0;
 
            foreach (string phaseReading in phaseReadings)
            {
                if (phaseReading.StartsWith("A:"))
                {
                    UpdatePhaseLabels(phaseReading.Substring(2), phase1_voltage, phase1_current);
                    string[] values = phaseReading.Substring(2).Split(',');
                   // double.TryParse(values[0].Replace("V", ""), out double voltage);
                   // double.TryParse(values[1].Replace("A", ""), out double current);
                   // totalpower += current * voltage;
                    if (values.Length >= 2)
                    {
                        if (double.TryParse(values[0].Replace("V", ""), out double voltage) &&
                            double.TryParse(values[1].Replace("A", ""), out double current))
                        {
                            totalpower += current * voltage;
                        }
                        else
                        {
                            Console.WriteLine("Warning: Could not parse voltage or current from: " + phaseReading);
                            totalpower += 0;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Malformed data: " + phaseReading);
                        totalpower += 0;
                    }
                }
                else if (phaseReading.StartsWith("B:"))
                {
                    UpdatePhaseLabels(phaseReading.Substring(2), phase2_voltage, phase2_current);
                    
                    string[] values = phaseReading.Substring(2).Split(',');
                    /*
                    double.TryParse(values[0].Replace("V", ""), out double voltage);
                    double.TryParse(values[1].Replace("A", ""), out double current);
                    totalpower += current * voltage; 
                    */
                    if (values.Length >= 2)
                    {
                        if (double.TryParse(values[0].Replace("V", ""), out double voltage) &&
                            double.TryParse(values[1].Replace("A", ""), out double current))
                        {
                            totalpower += current * voltage;
                        }
                        else
                        {
                            Console.WriteLine("Warning: Could not parse voltage or current from: " + phaseReading);
                            totalpower += 0;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Malformed data: " + phaseReading);
                        totalpower += 0;
                    }

                }
                else if (phaseReading.StartsWith("C:"))
                {
                    UpdatePhaseLabels(phaseReading.Substring(2), phase3_voltage, phase3_current);
                   
                    string[] values = phaseReading.Substring(2).Split(',');
                    /*
                    double.TryParse(values[0].Replace("V", ""), out double voltage);
                    double.TryParse(values[1].Replace("A", ""), out double current);
                    totalpower += current * voltage; 
                    */
                    if (values.Length >= 2)
                    {
                        if (double.TryParse(values[0].Replace("V", ""), out double voltage) &&
                            double.TryParse(values[1].Replace("A", ""), out double current))
                        {
                            totalpower += current * voltage;
                        }
                        else
                        {
                            Console.WriteLine("Warning: Could not parse voltage or current from: " + phaseReading);
                            totalpower += 0;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Malformed data: " + phaseReading);
                        totalpower += 0;
                    }

                }
            }
            total_power.Text = totalpower +"W";
        }

        private void UpdatePhaseLabels(string reading, TextBox voltageLabel, TextBox currentLabel)
        {
           
            string[] values = reading.Split(',');

            if (values.Length == 2 &&
                double.TryParse(values[0].Replace("V", ""), out double voltage) &&
                double.TryParse(values[1].Replace("A", ""), out double current))
            {
                voltageLabel.Text = "Voltage: " + voltage + "V";
                currentLabel.Text = "Current: " + current + "A";

                equipmentMonitor.SaveReading(voltageLabel.Name, voltage, current);

            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
               // SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED);

                Task.Run(() => AutoReporting.StartScheduler());

                Task.Run(() => new_BackUp.CreateBackupData());

            }
            catch (Exception ex)
            {
                MessageBox.Show("Startup error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<EquipmentReading> readings = equipmentMonitor.RetrieveReadings("2023-02-15" , "2026-03-15");

            string displayText = "Stored Equipment Readings:\n";
            foreach (var reading in readings)
            {
                displayText += $"[{reading.Timestamp}] Phase: {reading.Phase}, Voltage: {reading.Phase1}V, Current: {reading.Phase2}A, Power: {reading.Phase3}W\n";
                //total_power.Text = displayText;
            }

            MessageBox.Show(displayText, "Equipment Readings");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            //getPDF(voltage_data , current_data, power_data);
            // sendEmail();

            Report_Gen newform = new Report_Gen();

            if (newform.ShowDialog() == DialogResult.OK)
            {
                

                List<EquipmentReading> voltage_data = equipmentMonitor.RetrieveVoltageReadings(newform.start_date, newform.end_date);
                List<EquipmentReading> current_data = equipmentMonitor.RetrieveCurrentReadings(newform.start_date, newform.end_date);
                List<EquipmentReading> power_data = equipmentMonitor.RetrievePowerReadings(newform.start_date, newform.end_date);

              /*
                if(voltage_data.Count == 0 && current_data.Count == 0 && power_data.Count ==0) {
                    BackUpSql GetServer = new BackUpSql();
                    voltage_data = GetServer.RetrieveVoltageFromMySQL(newform.start_date, newform.end_date);
                    current_data = GetServer.RetrieveCurrentFromMySQL(newform.start_date, newform.end_date);
                    power_data = GetServer.RetrievePowerFromMySQL(newform.start_date, newform.end_date);

                }
              */

                if (newform.validEmail && !newform.save_to_drive) {
                    string save_path;// = Environment.SpecialFolder.Desktop.ToString();
                    save_path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    getPDF(voltage_data, current_data, power_data, save_path);
                    
                    try
                    {
                        sendEmail(newform.email);
                       
                    }
                    catch (Exception ex) {
                        MessageBox.Show(ex.Message);
                    }
                }
                else if (newform.save_to_drive)
                {
                    getPDF(voltage_data, current_data, power_data,newform.save_file_loc); 
                }
                
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Manage_User newU = new Manage_User();
            newU.ShowDialog();
        }

        private void reportUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Manage_User newU = new Manage_User();
            newU.ShowDialog();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
           // new_BackUp.CreateBackupData();
        }
    }
}
