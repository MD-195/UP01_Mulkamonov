using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;

namespace ISIP523_Mulkamonov
{
    public partial class ReportPage : Page
    {
        private Chart ChartIncome;

        public ReportPage()
        {
            InitializeComponent();
            LoadOwners();
            SetupChart();
        }

        private void SetupChart()
        {
            ChartIncome = new Chart();

            ChartIncome.BackColor = System.Drawing.Color.WhiteSmoke;

            ChartArea chartArea = new ChartArea("MainArea");
            chartArea.BackColor = System.Drawing.Color.WhiteSmoke;
            ChartIncome.ChartAreas.Add(chartArea);

            var series = new Series("Доход")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true,
                Color = System.Drawing.Color.FromArgb(1, 152, 175) 
            };
            ChartIncome.Series.Add(series);

            HostPanel.Child = ChartIncome;
        }

        private void LoadOwners()
        {
            using (var db = new DB_Mulkamonov_LeaseEntities())
            {
                ComboOwner.ItemsSource = db.User.Where(u => u.Role.Name == "User").ToList();
            }
        }

        private void ComboOwner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboOwner.SelectedItem is User selectedOwner)
            {
                using (var db = new DB_Mulkamonov_LeaseEntities())
                {
                    ChartIncome.Series["Доход"].Points.Clear();

                    var realEstates = db.RealEstate.Where(r => r.OwnerID == selectedOwner.ID).ToList();

                    foreach (var re in realEstates)
                    {
                        ChartIncome.Series["Доход"].Points.AddXY(re.Title, re.PricePerMonth);
                    }
                }
            }
        }
        private void BtnExportWord_Click(object sender, RoutedEventArgs e)
        {
            if (ComboOwner.SelectedItem is User selectedOwner)
            {
                try
                {
                    var application = new Word.Application();
                    Word.Document document = application.Documents.Add();

                    Word.Paragraph titleParagraph = document.Paragraphs.Add();
                    Word.Range titleRange = titleParagraph.Range;
                    titleRange.Text = $"Отчет по объектам: {selectedOwner.FIO}";
                    titleRange.Font.Size = 16;
                    titleRange.Font.Bold = 1;
                    titleRange.InsertParagraphAfter();

                    using (var db = new DB_Mulkamonov_LeaseEntities())
                    {
                        var realEstates = db.RealEstate.Include("Category").Where(r => r.OwnerID == selectedOwner.ID).ToList();

                        Word.Paragraph tableParagraph = document.Paragraphs.Add();
                        Word.Range tableRange = tableParagraph.Range;
                        Word.Table table = document.Tables.Add(tableRange, realEstates.Count + 1, 3);

                        table.Borders.Enable = 1;

                        table.Cell(1, 1).Range.Text = "Название";
                        table.Cell(1, 2).Range.Text = "Категория";
                        table.Cell(1, 3).Range.Text = "Доход (руб/мес)";

                        for (int i = 0; i < realEstates.Count; i++)
                        {
                            table.Cell(i + 2, 1).Range.Text = realEstates[i].Title;
                            table.Cell(i + 2, 2).Range.Text = realEstates[i].Category.Name;
                            table.Cell(i + 2, 3).Range.Text = realEstates[i].PricePerMonth.ToString("N2");
                        }
                    }

                    application.Visible = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при экспорте в Word: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Сначала выберите владельца из списка!");
            }
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if (ComboOwner.SelectedItem is User selectedOwner)
            {
                try
                {
                    var application = new Excel.Application();
                    application.Visible = true; 

                    Excel.Workbook workbook = application.Workbooks.Add();
                    Excel.Worksheet worksheet = workbook.Worksheets[1];
                    worksheet.Name = "Отчет по доходам"; 

                    worksheet.Cells[1, 1] = "Название объекта";
                    worksheet.Cells[1, 2] = "Категория";
                    worksheet.Cells[1, 3] = "Цена (руб/мес)";

                    Excel.Range headerRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, 3]];
                    headerRange.Font.Bold = true;
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    using (var db = new DB_Mulkamonov_LeaseEntities())
                    {
                        var realEstates = db.RealEstate.Include("Category").Where(r => r.OwnerID == selectedOwner.ID).ToList();

                        int startRowIndex = 2;

                        foreach (var re in realEstates)
                        {
                            worksheet.Cells[startRowIndex, 1] = re.Title;
                            worksheet.Cells[startRowIndex, 2] = re.Category.Name;
                            worksheet.Cells[startRowIndex, 3] = re.PricePerMonth;

                            startRowIndex++;
                        }

                        Excel.Range sumNameRange = worksheet.Range[worksheet.Cells[startRowIndex, 1], worksheet.Cells[startRowIndex, 2]];
                        sumNameRange.Merge();
                        sumNameRange.Value = "ОБЩИЙ ИТОГ:";
                        sumNameRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                        sumNameRange.Font.Bold = true;

                        worksheet.Cells[startRowIndex, 3].Formula = $"=SUM(C2:C{startRowIndex - 1})";
                        worksheet.Cells[startRowIndex, 3].Font.Bold = true;

                        Excel.Range grandTotalRange = worksheet.Range[worksheet.Cells[startRowIndex, 1], worksheet.Cells[startRowIndex, 3]];
                        grandTotalRange.Font.Color = Excel.XlRgbColor.rgbRed;
                    }

                    worksheet.Columns.AutoFit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при экспорте в Excel: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите владельца для отчета!");
            }
        }
    }
}