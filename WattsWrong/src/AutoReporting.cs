using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using static WattsWrong.EquipmentMonitor;

namespace WattsWrong
{
    public class AutoReporting
    {
       // private static readonly ReportGenerator reportGenerator = new ReportGenerator();
       // private static readonly UserService userService = new UserService();
      //  private static readonly EmailService emailService = new EmailService();
        private static EquipmentMonitor equipmentMonitor = new EquipmentMonitor();

        private static void getPDF(List<EquipmentReading> voltage_reading, List<EquipmentReading> current_reading, List<EquipmentReading> power_reading, string outputPath)

        {

            // Define the output path for the PDF
            //string outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "WattsWrongReport.pdf");

            string save_outputPath = Path.Combine(outputPath, "WattsWrong_Report.pdf");
            // Create an instance of the WattsWrongPDF class
            WattsWrongPDF pdfGenerator = new WattsWrongPDF(voltage_reading, current_reading, power_reading);

            // Call the method to generate the PDF
            pdfGenerator.GeneratePDF(save_outputPath);

            // Inform the user that the PDF was created
            // MessageBox.Show($"PDF generated successfully at: {outputPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Console.WriteLine($"Next report scheduled for: {"today generated PDF"}");

            // Handle any errors that occur during PDF generation
            //   MessageBox.Show($"An error occurred while generating the PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);



        }


        private static void sendEmail(string recipientEmail)
        {
            // Set up email configura tion
            string smtpServer = "smtp.gmail.com";
            int port = 587;
            string senderEmail = "wattswrong2024@gmail.com";
            string senderPassword = "xxxxxxxxxxx";


            // Initialize the EmailSender class
            EmailSender emailSender = new EmailSender(smtpServer, port, senderEmail, senderPassword);

            // Set up email details
            //string recipientEmail = "xxxxx@gmail.com";
            string subject = "Watt's Wrong Weekly Equipment Readings Report";
            string body = "Dear Recipient,<br><br>Please find the attached Watt's Wrong weekly report for equipment readings.<br><br>Best regards,<br>Watts Wrong";
            string pdfFilePath = "xxxxxxxxx";

            // Send the email
            emailSender.SendEmail(recipientEmail, subject, body, pdfFilePath ,false);
            //MessageBox.Show("Email Sent","Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Console.WriteLine($"Next report scheduled for: {"today sent an email"}");

        }


        public static void StartScheduler()
        {
            while (true)
            {
                DateTime now = DateTime.Now;
                DateTime nextRun = now.Date.AddDays(7 - (int)now.DayOfWeek).AddHours(8); // Runs every Sunday at 8 AM
               ;


                if (now.DayOfWeek == DayOfWeek.Sunday && now < now.Date.AddHours(8))
                {
                    // If it's Sunday but before 8 AM, today's 8 AM is the next run
                    nextRun = now.Date.AddHours(8);
                }
                else
                {
                    // Otherwise, schedule for the upcoming Sunday at 8 AM
                    nextRun = now.Date.AddDays(7 - (int)now.DayOfWeek).AddHours(8);
                }


                TimeSpan waitTime = nextRun - now;
                // TimeSpan timenow = now;
               
                if (waitTime.TotalMilliseconds > 0)
               // if (Math.Abs((now - nextRun).TotalSeconds) >= 20)
                {
                    Console.WriteLine($"Next report scheduled for: {nextRun}");
                    Console.WriteLine("---------------------------------- ");
                    Console.WriteLine($"Cureently it is: {now}");
                     Thread.Sleep(waitTime); // Wait until the next scheduled run
                }
                // else { 

                DateTime today = DateTime.Today;

                // Get the current week's Monday (start of week)
                DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

                // Get the current week's Sunday (end of week)
                DateTime endOfWeek = startOfWeek.AddDays(6);

                // Format dates to match your SQL query format (e.g., "yyyy-MM-dd")
                string startDate = startOfWeek.ToString("yyyy-MM-dd");
                string endDate = endOfWeek.ToString("yyyy-MM-dd");

                             

                // Generate and send report

               /* List<EquipmentReading> voltage_data = equipmentMonitor.RetrieveWeeklyReadings();
                List<EquipmentReading> current_data = equipmentMonitor.RetrieveWeeklyReadings();
                List<EquipmentReading> power_data = equipmentMonitor.RetrieveWeeklyReadings(); */

                List<EquipmentReading> voltage_data = equipmentMonitor.RetrievePowerReadings(startDate, endDate);
                List<EquipmentReading> current_data = equipmentMonitor.RetrieveCurrentReadings(startDate, endDate);
                List<EquipmentReading> power_data = equipmentMonitor.RetrievePowerReadings(startDate, endDate);

                Console.WriteLine($"Next report scheduled for: {"today"}");

                List<UserReading> userReadings = equipmentMonitor.GetReportingMembers();

                string save_path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                getPDF(voltage_data, current_data, power_data, save_path);

                foreach (var reading in userReadings)
                {
                    // listBox1.Items.Add(reading.Email);
                    sendEmail(reading.Email);
                }
                // string reportPath = reportGenerator.GenerateWeeklyReport();
                //List<string> userEmails = userService.GetUserEmails();
                //emailService.SendEmailReport(userEmails, reportPath);
            }
           // }
        }
    }
}
