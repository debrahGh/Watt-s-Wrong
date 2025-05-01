using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Threading.Tasks;

using static WattsWrong.EquipmentMonitor;

namespace WattsWrong
{
    internal class BackUpSql
    {
        private EquipmentMonitor SQLITE_Data = new EquipmentMonitor();
         private readonly string _sqliteConnectionString = "Data Source=equipment_readings.db;Version=3;";
         private readonly string _mysqlConnectionString = "server=your_mysql_server;database=your_database;user=your_user;password=your_password;";
        

        public void CreateBackupData()
        {
            while (true) { 

            DateTime now = DateTime.Now;
            DateTime nextRun = now.Date.AddDays(1); // Runs every day at 12 AM


            

            TimeSpan waitTime = nextRun - now;
            // TimeSpan timenow = now;

            if (waitTime.TotalMilliseconds > 0)
            // if (Math.Abs((now - nextRun).TotalSeconds) >= 20)
            {
                Console.WriteLine($"Next Backup scheduled for: {nextRun}");
                Console.WriteLine("---------------------------------- ");
                Console.WriteLine($"Cureently it is: {now}");
                Thread.Sleep(waitTime); // Wait until the next scheduled run
            }
            List<WattsWrong.EquipmentMonitor.EquipmentReading> SQLITE_readings = SQLITE_Data.GetSQLiteData();
            SaveToMySQL(SQLITE_readings);
            Console.WriteLine("Done successfully with the BackUp!!!!");
            }
        }

        

        private void SaveToMySQL(List<WattsWrong.EquipmentMonitor.EquipmentReading> readings)
        {
            using (var connection = new MySqlConnection(_mysqlConnectionString))
            {
                connection.Open();
                string createTableQuery = @"CREATE TABLE IF NOT EXISTS EquipmentReadings (
                    Id INT PRIMARY KEY,
                    Phase VARCHAR(50),
                    Voltage DOUBLE,
                    Current DOUBLE,
                    Power DOUBLE,
                    Timestamp DATETIME
                )";

                using (var command = new MySqlCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                foreach (var reading in readings)
                {
                    string insertQuery = @"INSERT INTO EquipmentReadings (Id, Phase, Voltage, Current, Power, Timestamp) 
                                          VALUES (@Id, @Phase, @Voltage, @Current, @Power, @Timestamp) 
                                          ON DUPLICATE KEY UPDATE Voltage=@Voltage, Current=@Current, Power=@Power, Timestamp=@Timestamp";
                    using (var command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", reading.Id);
                        command.Parameters.AddWithValue("@Phase", reading.Phase);
                        command.Parameters.AddWithValue("@Voltage", reading.Phase1);
                        command.Parameters.AddWithValue("@Current", reading.Phase2);
                        command.Parameters.AddWithValue("@Power", reading.Phase3);
                        command.Parameters.AddWithValue("@Timestamp", reading.Timestamp);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public List<EquipmentReading> RetrieveVoltageFromMySQL(string startDate, string endDate)
        {
            List<EquipmentReading> readings = new List<EquipmentReading>();

            using (var connection = new MySqlConnection(_mysqlConnectionString))
            {
                connection.Open();


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

                using (var command = new MySqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@start", startDate + " 00:00:00");
                    command.Parameters.AddWithValue("@end", endDate + " 23:59:59");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
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


        public List<EquipmentReading> RetrieveCurrentFromMySQL(string startDate, string endDate)
        {
            List<EquipmentReading> readings = new List<EquipmentReading>();

            using (var connection = new MySqlConnection(_mysqlConnectionString))
            {
                connection.Open();


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

                using (var command = new MySqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@start", startDate + " 00:00:00");
                    command.Parameters.AddWithValue("@end", endDate + " 23:59:59");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
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

        public List<EquipmentReading> RetrievePowerFromMySQL(string startDate, string endDate)
        {
            List<EquipmentReading> readings = new List<EquipmentReading>();

            using (var connection = new MySqlConnection(_mysqlConnectionString))
            {
                connection.Open();


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

                using (var command = new MySqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@start", startDate + " 00:00:00");
                    command.Parameters.AddWithValue("@end", endDate + " 23:59:59");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
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

        private class EquipmentReadings
        {
            public int Id { get; set; }
            public string Phase { get; set; }
            public double Voltage { get; set; }
            public double Current { get; set; }
            public double Power { get; set; }
            public DateTime Timestamp { get; set; }
        } 
    }
}
