using System;
using System.Drawing;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.IO.Font;
using iText.IO.Font.Constants;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.SkiaSharp;
using System.IO;
using static WattsWrong.EquipmentMonitor;
using System.Collections.Generic;
using OxyPlot.Legends;
using System.Linq;
using System.Globalization; // Import this namespace

namespace WattsWrong
{
   
    internal class WattsWrongPDF
    {
        List<EquipmentReading> readings;
        List<EquipmentReading> current_readings;
        List<EquipmentReading> voltage_readings;
        List<EquipmentReading> power_readings;

        public WattsWrongPDF(List<EquipmentReading> data1, List<EquipmentReading> data2, List<EquipmentReading> data3)
        {
            readings = data1;
            voltage_readings = data1;
            current_readings = data2;
            power_readings = data3;
        }
        public string GenerateGraph(string title, string yAxisTitle, string outputPath , List<EquipmentReading> graph_data)
        {
            // Create a new plot model
            var plotModel = new PlotModel { Title = title };

            // Add a legend
            plotModel.Legends.Add(new Legend
            {
                LegendPosition = LegendPosition.TopRight, // Position of the legend
                LegendPlacement = LegendPlacement.Inside,
                LegendBackground = OxyColors.White,
                LegendBorder = OxyColors.Black,
                LegendFontSize = 12
            });

            // Add X-axis (Time)
            plotModel.Axes.Add(new DateTimeAxis
            {
                /*Position = AxisPosition.Bottom,
                //StringFormat = "yyyy-MM-dd HH:mm",  // Customize as needed
                Title = "Time (s)",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
                */
                Position = AxisPosition.Bottom,
                StringFormat = "yyyy-MM-dd HH:mm",  // Customize as needed (e.g. "yyyy-MM-dd HH:mm")
                Title = "Time",
                IntervalType = DateTimeIntervalType.Hours, // Change to Seconds, Minutes, or Days as needed
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                IsZoomEnabled = true,
                IsPanEnabled = true
            });

            // Add Y-axis
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = yAxisTitle,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot
            });

            // Add Data Series for Phase 1, Phase 2, and Phase 3
            var phase1Series = new LineSeries { Title = "Phase 1", Color = OxyColors.Blue };
            var phase2Series = new LineSeries { Title = "Phase 2", Color = OxyColors.Red };
            var phase3Series = new LineSeries { Title = "Phase 3", Color = OxyColors.Green };

            // Example data
            /*
            for (int i = 1060; i <= 1080; i++)
            {
                phase1Series.Points.Add(new DataPoint(i, Math.Sin(i * 0.1) * 6)); // Replace with real data
                phase2Series.Points.Add(new DataPoint(i, Math.Cos(i * 0.1) * 5)); // Replace with real data
                phase3Series.Points.Add(new DataPoint(i, Math.Sin(i * 0.1 + 0.5) * 4)); // Replace with real data
            }
          */
            foreach (var reading in graph_data)
            {
                string dateTimeString = $"{reading.Timestamp}";
                DateTimeOffset dateTime = DateTimeOffset.Parse(dateTimeString);
               // double timestampDouble = dateTime.ToUnixTimeSeconds();// + dateTime.Offset.TotalSeconds;
                double timestampDouble = DateTimeAxis.ToDouble(dateTime.DateTime); // Converts DateTime to OxyPlot format

                phase1Series.Points.Add(new DataPoint(timestampDouble, reading.Phase1)); // Replace with real data
                phase2Series.Points.Add(new DataPoint(timestampDouble, reading.Phase2)); // Replace with real data
                phase3Series.Points.Add(new DataPoint(timestampDouble, reading.Phase3)); // Replace with real data
            }
           
            // Add the series to the model
            plotModel.Series.Add(phase1Series);
            plotModel.Series.Add(phase2Series);
            plotModel.Series.Add(phase3Series);

            // Export the plot as an image
            // var pngExporter = new OxyPlot.SkiaSharp.PngExporter();// { Width = 600, Height = 400 };
            //pngExporter.ExportToFile(plotModel, outputPath);
            using (var stream = File.Create(outputPath))
            {
                var pngExporter = new PngExporter { Width = 600, Height = 400 };
                pngExporter.Export(plotModel, stream);
            }

            return outputPath;
        }

        // Add a row to the summary table
        private void AddSummaryRow(Table table, string metric, double phaseA, double phaseB, double phaseC, double min, double max)
        {
            table.AddCell(metric);
            table.AddCell($"Avg: {phaseA}, Min: {min}, Max: {max}");
            table.AddCell($"Avg: {phaseB}, Min: {min}, Max: {max}");
            table.AddCell($"Avg: {phaseC}, Min: {min}, Max: {max}");
        }

        // Add a section for a graph
        private void AddGraphSection(Document document, string title, string imagePath, Func<string, string> graphGenerator)
        {
            document.Add(new Paragraph($"\n{title}")
                .SetFontSize(16)
                .SetFont(PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD))
                .SetUnderline());

            // Generate and add graph
            string generatedGraphPath = graphGenerator(imagePath);
            iText.Layout.Element.Image graphImage = new iText.Layout.Element.Image(ImageDataFactory.Create(generatedGraphPath))
                .SetWidth(400)
                .SetHeight(300)
                .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);
            document.Add(graphImage);
        }

        // Method to Generate a Graph Image
        private void GenerateGraphImages()
        {
            // Create a PlotModel for Voltage Graph
            var voltagePlotModel = new PlotModel { Title = "Voltage Data" };
            voltagePlotModel.Background = OxyColors.White; // Set background color to white

            // Add Axes
            voltagePlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Time (s)" });
            voltagePlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Voltage (V)" });

            // Add Series for Voltage Data
            var voltageSeries1 = new LineSeries { Title = "Phase 1 Voltage", Color = OxyColors.Blue };
            voltageSeries1.Points.Add(new DataPoint(1, 220));
            voltageSeries1.Points.Add(new DataPoint(2, 230));
            voltagePlotModel.Series.Add(voltageSeries1);

            // Export Voltage Graph to PNG
            using (var stream = File.Create("VoltageGraph.png"))
            {
                var exporter = new PngExporter { Width = 800, Height = 600 };
                exporter.Export(voltagePlotModel, stream);
            }

            // Repeat for Current and Power Graphs with similar logic
        }

        public void GeneratePDF(string outputFilePath)
        {
            // Create a PDF writer instance
            PdfWriter writer = new PdfWriter(outputFilePath);

            // Create a PDF document instance
            PdfDocument pdf = new PdfDocument(writer);

            // Create a Document layout for the PDF
            Document document = new Document(pdf);

            // Add a Title
            PdfFont boldFont = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
            Paragraph title = new Paragraph("Watt's Wrong Report")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20)
                .SetFont(boldFont);
            document.Add(title);

            // Add Subtitle with Italic Styling
            Text subtitleText = new Text("Power Quality Monitoring Device")
                .SetFont(PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_OBLIQUE));
            Paragraph subtitle = new Paragraph(subtitleText)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(14);
            document.Add(subtitle);

            if (voltage_readings != null && voltage_readings.Count > 0)
            {
                // Get the earliest and latest timestamps

                var firstReading = voltage_readings.FirstOrDefault();
                var lastReading = voltage_readings.LastOrDefault();

                var startTime = firstReading.Timestamp;
                var endTime = lastReading.Timestamp;

                Text subtitleDate = new Text($"Data Report from {endTime} to {startTime}.")
               .SetFont(PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_OBLIQUE));
                Paragraph subtitle_date = new Paragraph(subtitleDate)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(12);
                document.Add(subtitle_date);

                
            }
            /*
            // Add a Section Header
            document.Add(new Paragraph("\nProject Overview")
                .SetFontSize(16)
                .SetFont(boldFont)
                .SetUnderline());

            // Add Content
            document.Add(new Paragraph(
                "The Watt's Wrong project aims to design a non-invasive device that monitors power quality in industrial plants. "
                + "This device collects and analyzes data to ensure operational efficiency while avoiding machine shutdowns."
            ).SetFontSize(12));

            // *******************************
            // Recording Duration Section
            // *******************************
            // (Here we assume voltage_readings contains the overall timestamps. Adjust as needed if you want to use a different data set.)

           

            if (voltage_readings != null && voltage_readings.Count > 0)
            {
                // Get the earliest and latest timestamps

               // var startTime = voltage_readings.Select(r => DateTimeOffset.Parse(readings.Timestamp).Min());
                //var endTime = voltage_readings.Select(r => DateTimeOffset.Parse(r.Timestamp)).Max();

                var firstReading = voltage_readings.FirstOrDefault();
                var lastReading = voltage_readings.LastOrDefault();

                var startTime = firstReading.Timestamp;
                var endTime = lastReading.Timestamp;


               document.Add(new Paragraph("\nRecording Duration")
                    .SetFontSize(16)
                    .SetFont(boldFont)
                    .SetUnderline());
                document.Add(new Paragraph($"Data Report from {endTime} to {startTime}.")
                    .SetFontSize(12));
            }
                    */



            // *******************************
            // Statistical Overview Section
            // *******************************
            document.Add(new Paragraph("\nStatistical Overview")
                .SetFontSize(16)
                .SetFont(boldFont)
                .SetUnderline());

            // -- Voltage Statistics --
            if (voltage_readings != null && voltage_readings.Count > 0)
            {
                double vPhase1Avg = voltage_readings.Average(r => r.Phase1);
                double vPhase1Min = voltage_readings.Min(r => r.Phase1);
                double vPhase1Max = voltage_readings.Max(r => r.Phase1);

                double vPhase2Avg = voltage_readings.Average(r => r.Phase2);
                double vPhase2Min = voltage_readings.Min(r => r.Phase2);
                double vPhase2Max = voltage_readings.Max(r => r.Phase2);

                double vPhase3Avg = voltage_readings.Average(r => r.Phase3);
                double vPhase3Min = voltage_readings.Min(r => r.Phase3);
                double vPhase3Max = voltage_readings.Max(r => r.Phase3);

                document.Add(new Paragraph("Voltage (V):").SetFontSize(14).SetFont(boldFont));
                document.Add(new Paragraph($"Phase 1 - Avg: {vPhase1Avg:F2}, Min: {vPhase1Min:F2}, Max: {vPhase1Max:F2}")
                    .SetFontSize(12));
                document.Add(new Paragraph($"Phase 2 - Avg: {vPhase2Avg:F2}, Min: {vPhase2Min:F2}, Max: {vPhase2Max:F2}")
                    .SetFontSize(12));
                document.Add(new Paragraph($"Phase 3 - Avg: {vPhase3Avg:F2}, Min: {vPhase3Min:F2}, Max: {vPhase3Max:F2}")
                    .SetFontSize(12));
            }

            // -- Current Statistics --
            if (current_readings != null && current_readings.Count > 0)
            {
                double cPhase1Avg = current_readings.Average(r => r.Phase1);
                double cPhase1Min = current_readings.Min(r => r.Phase1);
                double cPhase1Max = current_readings.Max(r => r.Phase1);

                double cPhase2Avg = current_readings.Average(r => r.Phase2);
                double cPhase2Min = current_readings.Min(r => r.Phase2);
                double cPhase2Max = current_readings.Max(r => r.Phase2);

                double cPhase3Avg = current_readings.Average(r => r.Phase3);
                double cPhase3Min = current_readings.Min(r => r.Phase3);
                double cPhase3Max = current_readings.Max(r => r.Phase3);

                document.Add(new Paragraph("\nCurrent (A):").SetFontSize(14).SetFont(boldFont));
                document.Add(new Paragraph($"Phase 1 - Avg: {cPhase1Avg:F2}, Min: {cPhase1Min:F2}, Max: {cPhase1Max:F2}")
                    .SetFontSize(12));
                document.Add(new Paragraph($"Phase 2 - Avg: {cPhase2Avg:F2}, Min: {cPhase2Min:F2}, Max: {cPhase2Max:F2}")
                    .SetFontSize(12));
                document.Add(new Paragraph($"Phase 3 - Avg: {cPhase3Avg:F2}, Min: {cPhase3Min:F2}, Max: {cPhase3Max:F2}")
                    .SetFontSize(12));
            }

            // -- Power Statistics --
            if (power_readings != null && power_readings.Count > 0)
            {
                double pPhase1Avg = power_readings.Average(r => r.Phase1);
                double pPhase1Min = power_readings.Min(r => r.Phase1);
                double pPhase1Max = power_readings.Max(r => r.Phase1);

                double pPhase2Avg = power_readings.Average(r => r.Phase2);
                double pPhase2Min = power_readings.Min(r => r.Phase2);
                double pPhase2Max = power_readings.Max(r => r.Phase2);

                double pPhase3Avg = power_readings.Average(r => r.Phase3);
                double pPhase3Min = power_readings.Min(r => r.Phase3);
                double pPhase3Max = power_readings.Max(r => r.Phase3);

                document.Add(new Paragraph("\nPower (W):").SetFontSize(14).SetFont(boldFont));
                document.Add(new Paragraph($"Phase 1 - Avg: {pPhase1Avg:F2}, Min: {pPhase1Min:F2}, Max: {pPhase1Max:F2}")
                    .SetFontSize(12));
                document.Add(new Paragraph($"Phase 2 - Avg: {pPhase2Avg:F2}, Min: {pPhase2Min:F2}, Max: {pPhase2Max:F2}")
                    .SetFontSize(12));
                document.Add(new Paragraph($"Phase 3 - Avg: {pPhase3Avg:F2}, Min: {pPhase3Min:F2}, Max: {pPhase3Max:F2}")
                    .SetFontSize(12));
            }


            // Add a Placeholder for Graph
            document.Add(new Paragraph("\nGraphs & Visualizations")
                .SetFontSize(16)
                .SetFont(boldFont)
                .SetUnderline());

            // Generate Graph Images
            var graphGenerator = new WattsWrongPDF(voltage_readings, current_readings, power_readings);

            readings = voltage_readings;
            string voltageGraphPath = graphGenerator.GenerateGraph("Voltage vs Time", "Voltage (V)", "voltageGraph.png",voltage_readings);
            readings = current_readings;
            string currentGraphPath = graphGenerator.GenerateGraph("Current vs Time", "Current (A)", "currentGraph.png" , current_readings);
            readings = power_readings;
            string powerGraphPath = graphGenerator.GenerateGraph("Power vs Time", "Power (W)", "powerGraph.png", power_readings);

            // Add the Graphs to the PDF
            document.Add(new iText.Layout.Element.Image(ImageDataFactory.Create(voltageGraphPath))
                .SetWidth(400)
                .SetHeight(300)
                .SetHorizontalAlignment((iText.Layout.Properties.HorizontalAlignment?)System.Windows.Forms.HorizontalAlignment.Center));
            document.Add(new iText.Layout.Element.Image(ImageDataFactory.Create(currentGraphPath))
                .SetWidth(400)
                .SetHeight(300)
                .SetHorizontalAlignment((iText.Layout.Properties.HorizontalAlignment?)System.Windows.Forms.HorizontalAlignment.Center));
            document.Add(new iText.Layout.Element.Image(ImageDataFactory.Create(powerGraphPath))
                .SetWidth(400)
                .SetHeight(300)
                .SetHorizontalAlignment((iText.Layout.Properties.HorizontalAlignment?)System.Windows.Forms.HorizontalAlignment.Center));
           
            // Add Another Section
            /*  document.Add(new Paragraph("\nConclusion")
                  .SetFontSize(16)
                  .SetFont(boldFont)
                  .SetUnderline());
              document.Add(new Paragraph(
                  "The device has proven its ability to provide real-time power quality data, ensuring seamless operation of industrial equipment. "
                  + "Further development will focus on enhancing software capabilities for more detailed analytics."
              ).SetFontSize(12));
            */
            // Close the document
            document.Close();

            Console.WriteLine("PDF created successfully: " + outputFilePath);
        }

    }
}
