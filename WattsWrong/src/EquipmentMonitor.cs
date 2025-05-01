using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;


namespace WattsWrong
{

    public class EquipmentMonitor
    {
        private readonly string _connectionString = "Data Source=equipment_readings.db;Version=3;";
        private const double VoltageThresholdHigh = 125.0; 
        private const double VoltageThresholdLow = 110.0; 
        private const double CurrentThresholdHigh = 200.0;
        private const double CurrentThresholdLow = 0.0;
        private DateTime lastSaveTime;
        private DateTime firstTime;
        private DateTime lastAlertUpdate;
        private int row_save_count;

        public EquipmentMonitor()
        {
            InitializeDatabase();
            lastSaveTime = DateTime.Now;
            firstTime = lastSaveTime;
            lastAlertUpdate = lastSaveTime.AddMinutes(-5);
        }

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS EquipmentReadings (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Phase TEXT,
                        Voltage REAL,
                        Current REAL,
                        Power REAL,
                        Timestamp DATETIME DEFAULT (datetime('now', '-05:00'))
                    )";
                // Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP

                string createReportingMembersTableQuery = @"
                    CREATE TABLE IF NOT EXISTS ReportingMembers (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Email TEXT UNIQUE NOT NULL
                    )";


                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SQLiteCommand(createReportingMembersTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveReading(string phase, double voltage, double current)
        {
            double power = voltage * current;
            DateTime currentTime = DateTime.Now;

            // Only save if 5 minutes have passed
            if ((currentTime - lastSaveTime).TotalMinutes >= 5)
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO EquipmentReadings (Phase, Voltage, Current, Power) VALUES (@phase, @voltage, @current, @power)";

                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@phase", phase);
                        command.Parameters.AddWithValue("@voltage", voltage);
                        command.Parameters.AddWithValue("@current", current);
                        command.Parameters.AddWithValue("@power", power);
                        command.ExecuteNonQuery();
                    }
                }
                row_save_count++;
                if (row_save_count >= 3)
                {
                    lastSaveTime = currentTime; // Update last save time
                    row_save_count = 0;
                }
                
            }

            if ((currentTime - firstTime).TotalSeconds >= 40 && (currentTime - lastAlertUpdate).TotalMinutes >= 5) { 
                CheckForAlerts(phase, voltage, current); 
            }
        }

        public void AddReportingMember(string name, string email)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string insertQuery = "INSERT INTO ReportingMembers (Name, Email) VALUES (@name, @email)";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@email", email);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message); // Handles duplicate email exception
                    }
                }
            }
        }


        public void DeleteReportingMember(string email)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM ReportingMembers WHERE Email = @email";

                using (var command = new SQLiteCommand(deleteQuery, connection))
                {
                    
                    command.Parameters.AddWithValue("@email", email);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message); // Handles duplicate email exception
                    }
                }
            }
        }


        /// <summary>
        /// Retrieves stored readings from the database.
        /// </summary>
        /// <returns>A list of EquipmentReading objects containing the stored data.</returns>
        public List<EquipmentReading> RetrieveReadings(string start_date , string end_date)
        {
            List<EquipmentReading> readings = new List<EquipmentReading>();
            //string selectQuery = "SELECT Id, Phase, Voltage, Current, Power, Timestamp FROM EquipmentReadings ORDER BY Timestamp DESC";
             string selectQuery = "SELECT Id, Phase, Voltage, Current, Power, Timestamp FROM EquipmentReadings WHERE  Timestamp BETWEEN" + " '"+ start_date+ " 00:00:00' " + "AND" + "  '" + end_date + " 23:59:59'" + " ORDER BY Timestamp DESC";
           

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    //command.Parameters.AddWithValue("@date_start", start_date);
                   // command.Parameters.AddWithValue("@date_end", end_date);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            readings.Add(new EquipmentReading
                            {
                                Id = reader.GetInt32(0),
                                Phase = reader.GetString(1),
                                Phase1 = reader.GetDouble(2),
                                Phase2 = reader.GetDouble(3),
                                Phase3 = reader.GetDouble(4),
                                Timestamp = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }
            return readings;
        }

       

        public List<EquipmentReading> RetrieveVoltageReadings(string start_date, string end_date)
        {
            List<EquipmentReading> readings = new List<EquipmentReading>();

                                string selectQuery = @"
                    SELECT 
                        r1.Timestamp,
                        r1.Voltage AS phase1_voltage,
                        r2.Voltage AS phase2_voltage,
                        r3.Voltage AS phase3_voltage
                    FROM 
                        EquipmentReadings r1
                    JOIN 
                        EquipmentReadings r2 ON r1.Timestamp = r2.Timestamp
                    JOIN 
                        EquipmentReadings r3 ON r1.Timestamp = r3.Timestamp
                    WHERE 
                        r1.Phase = 'phase1_voltage' AND
                        r2.Phase = 'phase2_voltage' AND
                        r3.Phase = 'phase3_voltage' AND
                        r1.Timestamp BETWEEN @start AND @end
                    ORDER BY 
                        r1.Timestamp DESC;
                    ";

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@start", start_date + " 00:00:00");
                    command.Parameters.AddWithValue("@end", end_date + " 23:59:59");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // access reader["phase1_voltage"], etc.
                            readings.Add(new EquipmentReading
                            {

                                Id = 0,
                                Phase = "1",
                                Phase1 = reader.GetDouble(1),
                                Phase2 = reader.GetDouble(2),
                                Phase3 = reader.GetDouble(3),
                                Timestamp = reader.GetDateTime(0)
                            });
                        }
                    }
                }
            }


            return readings;
        }


        public List<EquipmentReading> RetrieveCurrentReadings(string start_date, string end_date)
        {
            List<EquipmentReading> readings = new List<EquipmentReading>();

            string selectQuery = @"
                    SELECT 
                        r1.Timestamp,
                        r1.Current AS phase1_voltage,
                        r2.Current AS phase2_voltage,
                        r3.Current AS phase3_voltage
                    FROM 
                        EquipmentReadings r1
                    JOIN 
                        EquipmentReadings r2 ON r1.Timestamp = r2.Timestamp
                    JOIN 
                        EquipmentReadings r3 ON r1.Timestamp = r3.Timestamp
                    WHERE 
                        r1.Phase = 'phase1_voltage' AND
                        r2.Phase = 'phase2_voltage' AND
                        r3.Phase = 'phase3_voltage' AND
                        r1.Timestamp BETWEEN @start AND @end
                    ORDER BY 
                        r1.Timestamp DESC;
                    ";

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@start", start_date + " 00:00:00");
                    command.Parameters.AddWithValue("@end", end_date + " 23:59:59");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // access reader["phase1_voltage"], etc.
                            readings.Add(new EquipmentReading
                            {

                                Id = 0,
                                Phase = "1",
                                Phase1 = reader.GetDouble(1),
                                Phase2 = reader.GetDouble(2),
                                Phase3 = reader.GetDouble(3),
                                Timestamp = reader.GetDateTime(0)
                            });
                        }
                    }
                }
            }


            return readings;
        }


        public List<EquipmentReading> RetrievePowerReadings(string start_date, string end_date)
        {
            List<EquipmentReading> readings = new List<EquipmentReading>();

            string selectQuery = @"
                    SELECT 
                        r1.Timestamp,
                        r1.Power AS phase1_voltage,
                        r2.Power AS phase2_voltage,
                        r3.Power AS phase3_voltage
                    FROM 
                        EquipmentReadings r1
                    JOIN 
                        EquipmentReadings r2 ON r1.Timestamp = r2.Timestamp
                    JOIN 
                        EquipmentReadings r3 ON r1.Timestamp = r3.Timestamp
                    WHERE 
                        r1.Phase = 'phase1_voltage' AND
                        r2.Phase = 'phase2_voltage' AND
                        r3.Phase = 'phase3_voltage' AND
                        r1.Timestamp BETWEEN @start AND @end
                    ORDER BY 
                        r1.Timestamp DESC;
                    ";

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@start", start_date + " 00:00:00");
                    command.Parameters.AddWithValue("@end", end_date + " 23:59:59");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // access reader["phase1_voltage"], etc.
                            readings.Add(new EquipmentReading
                            {

                                Id = 0,
                                Phase = "1",
                                Phase1 = reader.GetDouble(1),
                                Phase2 = reader.GetDouble(2),
                                Phase3 = reader.GetDouble(3),
                                Timestamp = reader.GetDateTime(0)
                            });
                        }
                    }
                }
            }


            return readings;
        }


        public List<EquipmentReading> RetrieveWeeklyReadings()
        {
            List<EquipmentReading> readings = new List<EquipmentReading>();
            
            //string selectQuery = "SELECT Id, Phase, Voltage, Current, Power, Timestamp FROM EquipmentReadings WHERE Timestamp BETWEEN" + " '" + start_date + " 00:00:00' " + "AND" + "  '" + end_date + " 23:59:59'" + " ORDER BY Timestamp DESC";

            string query = "SELECT Id, Phase, Voltage, Current, Power, Timestamp FROM EquipmentReadings " +
                   "WHERE Timestamp >= datetime('now', '-7 days') ORDER BY Timestamp DESC";


            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SQLiteCommand(query, connection))
                {
                    //command.Parameters.AddWithValue("@date_start", start_date);
                    // command.Parameters.AddWithValue("@date_end", end_date);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            readings.Add(new EquipmentReading
                            {
                                Id = reader.GetInt32(0),
                                Phase = reader.GetString(1),
                                Phase1 = reader.GetDouble(2),
                                Phase2 = reader.GetDouble(3),
                                Phase3 = reader.GetDouble(4),
                                Timestamp = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }
            return readings;
        }

        public List<UserReading> GetReportingMembers()
        {
            List<UserReading> members = new List<UserReading>();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string selectQuery = "SELECT Name, Email FROM ReportingMembers";

                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // members.Add((reader.GetString(0), reader.GetString(1)));
                            members.Add(new UserReading
                            {
                                Name = reader.GetString(0),
                                Email = reader.GetString(1),
                                
                            });
                        }
                    }
                }
            }

            return members;
        }


        //Get Data to be exported to server for backup
        public List<EquipmentReading> GetSQLiteData()
        {
            List<EquipmentReading> readings = new List<EquipmentReading>();
            string query = "SELECT Id, Phase, Voltage, Current, Power, Timestamp FROM EquipmentReadings";

            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            readings.Add(new EquipmentReading
                            {
                                Id = reader.GetInt32(0),
                                Phase = reader.GetString(1),
                                Phase1 = reader.GetDouble(2),
                                Phase2 = reader.GetDouble(3),
                                Phase3 = reader.GetDouble(4),
                                Timestamp = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }
            return readings;
        }

        // Convert UTC to Central Time (CT) with Automatic DST Handling
        private DateTime ConvertUTCToCentralTime(string utcTimestamp)
        {
            DateTime utcTime = DateTime.ParseExact(utcTimestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            TimeZoneInfo ctZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            DateTime ctTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, ctZone);
            return ctTime;
        }

        private void CheckForAlerts(string phase, double voltage, double current)
        {
            if (voltage < VoltageThresholdLow || (voltage > VoltageThresholdHigh) )
            {
                //    ShowAlert($"Alert for {phase}: Voltage or Current below threshold!");
               
                string status = voltage < VoltageThresholdLow ? "under-voltage" : "over-voltage";
                List<UserReading> userReadings = GetReportingMembers();
                foreach (var reading in userReadings)
                {
                    sendAlertEmail(reading.Email, status , phase , voltage);
                }
                lastAlertUpdate = DateTime.Now;
            }
            if (current < CurrentThresholdLow || (current > CurrentThresholdHigh))
            {
                //    ShowAlert($"Alert for {phase}: Voltage or Current below threshold!");

                string status = current < CurrentThresholdLow ? "under-current" : "over-current";
                List<UserReading> userReadings = GetReportingMembers();
                foreach (var reading in userReadings)
                {
                    sendAlertEmail(reading.Email, status, phase, voltage);
                }
                lastAlertUpdate = DateTime.Now;
            }
        }
        private void sendAlertEmail(string recipientEmail,string status , string phase , double voltage)
        {
            // Set up email configura tion
            string smtpServer = "smtp.gmail.com";
            int port = 587;
            string senderEmail = "wattswrong2024@gmail.com";
            string senderPassword = "xxxxxxx";


            // Initialize the EmailSender class
            EmailSender emailSender = new EmailSender(smtpServer, port, senderEmail, senderPassword);

            // Set up email details
           
            string subject = $"Watt's Wrong Alert: {status} Detected in {phase} ({voltage}V)";
          
            string body = $"Dear Recipient,<br><br>"
            + $"An {status} condition has been detected in {phase} with a voltage of {voltage}V.<br><br>"
            + $"Best regards,<br>Watts Wrong";
           
            emailSender.SendEmail(recipientEmail, subject, body, "ALERT", false);
            //MessageBox.Show("Email Sent","Success", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }
        private void ShowAlert(string message)
        {
            System.Windows.Forms.MessageBox.Show(message, "Alert", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
        }

        public class EquipmentReading
        {
            public int Id { get; set; }
            public string Phase { get; set; }
            public double Phase1 { get; set; }
            public double Phase2 { get; set; }
            public double Phase3 { get; set; }
            public DateTime Timestamp
            
            
            { get; set; }
        }

        public class UserReading
        {
            
            public string Name { get; set; }
            public string Email { get; set; }
           
            
        }

    }

}
