using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Paragraph = iTextSharp.text.Paragraph;
using DocumentFormat.OpenXml.Wordprocessing;
using Document = iTextSharp.text.Document;
using PageSize = iTextSharp.text.PageSize;
using Path = System.IO.Path;
using DocumentFormat.OpenXml.Bibliography;
using iTextSharp.xmp.impl;
using Font = iTextSharp.text.Font;
using ClosedXML.Excel;
using System.Collections.ObjectModel;
using Rectangle = iTextSharp.text.Rectangle;
using ClosedXML;


namespace IBC_Management_System
{

    public partial class Report : Window
    {
        IBC_STOREIIEntities db = new IBC_STOREIIEntities();
        string chatUsername;
        public Report()
        {
            InitializeComponent();
        }
        public Report(string username)
        {
            InitializeComponent();
            this.chatUsername = username;
        }

        private void btnLoadInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rbSearchDuration.IsChecked == true || rbStartEnd.IsChecked == true)
                {
                    if (rbSearchDuration.IsChecked == false && rbStartEnd.IsChecked == false)
                    {
                        MessageBox.Show("Please select options for searching!");
                    }
                    if (rbSearchDuration.IsChecked == true)
                    {
                        datagridViewInvoice.ItemsSource = ConvertInvoiceName(SearchInvoiceByDateDuration(DurationPicker.SelectedIndex)).DefaultView;
                    }
                    if (rbStartEnd.IsChecked == true)
                    {
                        if (StartDatePicker.SelectedDate.Value > EndDatePicker.SelectedDate.Value)
                        {
                            MessageBox.Show("Start date must be smaller than end date!");
                        }
                        else
                        {
                            datagridViewInvoice.ItemsSource = ConvertInvoiceName(SearchInvoiceByDate(StartDatePicker.SelectedDate.Value, EndDatePicker.SelectedDate.Value)).DefaultView;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select option to load informations!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message :" + ex.Message);
            }

        }
        private void txtSearchPurchase_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
            {
                if (txtSearchPurchase.Text != "")
                {
                    var purchase = db.Purchases.ToList();
                    var purchase1 = ConvertPurchaseTable(purchase);
                    datagridViewPurchase.ItemsSource = SearchUserByNamePurchase(purchase1, txtSearchPurchase.Text).DefaultView;
                }
            }
        }

        private void txtSearchSeller_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
            {
                if (txtSearchSeller.Text != "")
                {
                    var invoice = db.Invoices.ToList();
                    var invoice1 = ConvertInvoiceName(invoice);
                    datagridViewInvoice.ItemsSource = SearchUserByNameInInvoice(invoice1, txtSearchSeller.Text).DefaultView;
                }
            }
        }
        private void SearchProductTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
            {
                if (SearchProductTextBox.Text != "")
                {
                    var stock = db.Stocks.ToList();
                    var stock1 = ConvertStockData(stock);
                    ProductDataGrid.ItemsSource = SearchProductByNameInStock(stock1, SearchProductTextBox.Text).DefaultView;

                }
                else
                {
                    var stock = db.Stocks.ToList();
                    ProductDataGrid.ItemsSource = ConvertStockData(stock).DefaultView;
                }
            }
        }

        private List<Invoice> SearchInvoiceByDateDuration(int indexSearch)
        {
            List<Invoice> result = new List<Invoice>();
            using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
            {

                var invoies = db.Invoices.ToList();
                switch (indexSearch)
                {
                    case 1:
                        result = invoies.Where(x => x.Date.Date == DateTime.Today).ToList();
                        break;
                    case 2:
                        DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                        DateTime endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

                        result = invoies.Where(x => x.Date >= startOfWeek && x.Date <= endOfWeek).ToList();
                        break;
                    case 3:
                        result = invoies.Where(x => x.Date >= DateTime.Now.AddMonths(-1)).ToList();
                        break;
                    case 4:
                        result = invoies.Where(x => x.Date >= DateTime.Now.AddYears(-1)).ToList();

                        break;
                }
            }
            return result;
        }

        private List<Invoice> SearchInvoiceByDate(DateTime value1, DateTime value2)
        {
            List<Invoice> result = new List<Invoice>();
            using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
            {

                var invoies = db.Invoices.ToList();
                result = invoies.Where(x => x.Date >= value1 && x.Date.Date <= value2).ToList();
            }
            return result;
        }
        private List<Purchase> SearchPurchaseByDateDuration(int indexSearch)
        {
            List<Purchase> result = new List<Purchase>();
            using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
            {

                var purchase = db.Purchases.ToList();
                switch (indexSearch)
                {
                    case 1:
                        result = purchase.Where(x => x.Date.Date == DateTime.Today).ToList();
                        break;
                    case 2:
                        DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                        DateTime endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

                        result = purchase.Where(x => x.Date >= startOfWeek && x.Date <= endOfWeek).ToList();
                        break;
                    case 3:
                        result = purchase.Where(x => x.Date >= DateTime.Now.AddMonths(-1)).ToList();
                        break;
                    case 4:
                        result = purchase.Where(x => x.Date >= DateTime.Now.AddYears(-1)).ToList();

                        break;
                }
            }
            return result;
        }

        private List<Purchase> SearchPurchaseByDate(DateTime value1, DateTime value2)
        {
            List<Purchase> result = new List<Purchase>();
            using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
            {

                var purchase = db.Purchases.ToList();
                result = purchase.Where(x => x.Date >= value1 && x.Date.Date <= value2).ToList();
            }
            return result;
        }

        private List<Distribution> SearchDistributionByDateDuration(int indexSearch)
        {
            List<Distribution> result = new List<Distribution>();
            using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
            {
                var distribution = db.Distributions.ToList();
                switch (indexSearch)
                {
                    case 1:
                        result = distribution.Where(x => x.Date.Date == DateTime.Today).ToList();
                        break;
                    case 2:
                        DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                        DateTime endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

                        result = distribution.Where(x => x.Date >= startOfWeek && x.Date <= endOfWeek).ToList();
                        break;
                    case 3:
                        result = distribution.Where(x => x.Date >= DateTime.Now.AddMonths(-1)).ToList();
                        break;
                    case 4:
                        result = distribution.Where(x => x.Date >= DateTime.Now.AddYears(-1)).ToList();

                        break;
                }
            }
            return result;
        }

        private List<Distribution> SearchDistributionByDate(DateTime value1, DateTime value2)
        {
            List<Distribution> result = new List<Distribution>();
            using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
            {

                var distribution = db.Distributions.ToList();
                result = distribution.Where(x => x.Date >= value1 && x.Date.Date <= value2).ToList();
            }
            return result;
        }

        private void rbSearchDuration_Checked(object sender, RoutedEventArgs e)
        {
            if (rbSearchDuration.IsChecked == true)
            {
                StartDatePicker.IsEnabled = false;
                EndDatePicker.IsEnabled = false;
            }
            if (rbStartEnd.IsChecked == false)
            {
                DurationPicker.IsEnabled = true;
            }
        }

        private void rbStartEnd_Checked(object sender, RoutedEventArgs e)
        {
            if (rbStartEnd.IsChecked == true)
            {
                DurationPicker.IsEnabled = false;
            }
            if (rbSearchDuration.IsChecked == false)
            {
                StartDatePicker.IsEnabled = true;
                EndDatePicker.IsEnabled = true;
            }

        }
        private void btnSaveInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (datagridViewInvoice.ItemsSource == null)
            {
                MessageBox.Show("No items to save.");
                return;
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf|Excel Files (*.xlsx)|*.xlsx";

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                string fileExtension = System.IO.Path.GetExtension(filePath).ToLower();

                if (saveFileDialog.FilterIndex == 1) // Save as PDF
                {
                    GeneratePdfInvoice(filePath);
                    MessageBox.Show("PDF saved successfully.");
                }
                else if (saveFileDialog.FilterIndex == 2) // Save as Excel
                {
                    GenerateExcelInvoice(filePath);
                    MessageBox.Show("Excel saved successfully.");
                }
            }
        }

    private void btnSave_Purchase_Click(object sender, RoutedEventArgs e)
        {
            if (datagridViewPurchase.ItemsSource == null)
            {
                MessageBox.Show("No items to save.");
                return;
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf|Excel Files (*.xlsx)|*.xlsx";

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                string fileExtension = System.IO.Path.GetExtension(filePath).ToLower();

                if (saveFileDialog.FilterIndex == 1) // Save as PDF
                {
                    GeneratePdfPurchase(filePath);
                    MessageBox.Show("PDF saved successfully.");
                }
                else if (saveFileDialog.FilterIndex == 2) // Save as Excel
                {
                    GenerateExcelPurchase(filePath);
                    MessageBox.Show("Excel saved successfully.");
                }
            }
        }
        private void btnSave_Distribution_Click(object sender, RoutedEventArgs e)
        {
            if (datagridviewDistribution.ItemsSource == null)
            {
                MessageBox.Show("No items to save.");
                return;
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf|Excel Files (*.xlsx)|*.xlsx";

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                string fileExtension = System.IO.Path.GetExtension(filePath).ToLower();

                if (saveFileDialog.FilterIndex == 1) // Save as PDF
                {
                    GeneratePdfDistribution(filePath);
                    MessageBox.Show("PDF saved successfully.");
                }
                else if (saveFileDialog.FilterIndex == 2) // Save as Excel
                {
                    GenerateExcelDistribution(filePath);
                    MessageBox.Show("Excel saved successfully.");
                }
            }
        }

        private void btnSaveSale_Click(object sender, RoutedEventArgs e)
        {
            if (datagridViewSale.ItemsSource == null)
            {
                MessageBox.Show("No items to save.");
                return;
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf|Excel Files (*.xlsx)|*.xlsx";

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                string fileExtension = System.IO.Path.GetExtension(filePath).ToLower();

                if (saveFileDialog.FilterIndex == 1) // Save as PDF
                {
                    GeneratePdfSale(filePath);
                    MessageBox.Show("PDF saved successfully.");
                }
                else if (saveFileDialog.FilterIndex == 2) // Save as Excel
                {
                    GenerateExcelSale(filePath);
                    MessageBox.Show("Excel saved successfully.");
                }
            }
        }
        private PdfPCell CreateCenteredCell(string content)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content, new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //cell.Border = Rectangle.NO_BORDER;
            return cell;
        }

        private void GeneratePdfInvoice(string filePath)
        {
            Document doc = new Document();
            FileStream combinedFs = new FileStream(filePath, FileMode.Create);

            PdfWriter combinedWriter = PdfWriter.GetInstance(doc, combinedFs);
            doc.Open();
            Font font = FontFactory.GetFont("Century Gothic", 8f, Font.BOLD,
            iTextSharp.text.BaseColor.BLACK);//Initializing Information font

            string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "logo.png");
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.ScalePercent(50f);
            logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
            doc.Add(logo);

            // Add the logo to the document and Informations
            Paragraph paragraph = new Paragraph("Internation Book Centre\nEmail Address : IBCStore@Gmail.com\nPhone Number : 012 852 645\n", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD));
            paragraph.Alignment = Element.ALIGN_RIGHT;
            doc.Add(paragraph);

            // Add Title
            Paragraph title = new Paragraph("INVOICE", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);

            // Add some space between the logo and the text
            doc.Add(new Paragraph("\n\n"));

            // Create a table with 7 columns
            var table = new PdfPTable(6);
            table.WidthPercentage = 100;

            // Set the minimum height of the cells
            table.DefaultCell.MinimumHeight = 20f;

            // Add the headers to the table
            table.AddCell(new Phrase("Id"));
            table.AddCell(new Phrase("Date"));
            table.AddCell(new Phrase("Total"));
            table.AddCell(new Phrase("Customer"));
            table.AddCell(new Phrase("User"));
            table.AddCell(new Phrase("Branch"));

            var invoice1 = db.Invoices.ToList();
            //Add the data from the invoices to the table
            if (invoice1 != null)
            {
                foreach (var invoice in invoice1)
                {
                    table.AddCell(CreateCenteredCell(invoice.Id.ToString()));
                    table.AddCell(CreateCenteredCell(invoice.Date.ToString()));
                    table.AddCell(CreateCenteredCell(invoice.Total.ToString()));
                    table.AddCell(CreateCenteredCell(invoice.Customer.Name));
                    table.AddCell(CreateCenteredCell($"{invoice.User.FirstName} {invoice.User.LastName}"));
                    table.AddCell(CreateCenteredCell(invoice.Branch.Address));
                }
            }

            Paragraph totalInvoice = new Paragraph("Total Invoice : " + table.Rows.Count.ToString() + "\nDate Created : " + DateTime.Now.Date.ToString() + "\n\n ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD));
            totalInvoice.Alignment = Element.ALIGN_LEFT;
            doc.Add(totalInvoice);

            doc.Add(table);

            doc.Close(); // Close the document after adding content
        }
        private void GeneratePdfPurchase(string filePath)
        {
            Document doc = new Document();
            PdfWriter wirter = PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));

            doc.Open();

            string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "logo.png");
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.ScalePercent(50f);
            logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
            doc.Add(logo);

            Paragraph paragraph = new Paragraph("Internation Book Centre\nEmail Address : IBCStore@Gmail.com\nPhone Number : 012 852 645\n", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD));
            paragraph.Alignment = Element.ALIGN_RIGHT;
            doc.Add(paragraph);

            // Add Title
            Paragraph title = new Paragraph("PURCHASE INVOICE", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);

            // Add some space between the logo and the text
            doc.Add(new Paragraph("\n\n"));
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;

            // Set column widths
            float[] columnWidths = { 30f, 170f, 50f, 60f, 60f };
            table.SetWidths(columnWidths);

            // Set the minimum height of the cells
            table.DefaultCell.MinimumHeight = 20f;

            // Add the headers to the table
            table.AddCell(new Phrase("Id"));
            table.AddCell(new Phrase("Date"));
            table.AddCell(new Phrase("Total"));
            table.AddCell(new Phrase("Supplier"));
            table.AddCell(new Phrase("User"));

            var purchase1 = db.Purchases.ToList();
            //Add the data from the invoices to the table
            if (purchase1 != null)
            {
                foreach (var purchase in purchase1)
                {
                    table.AddCell(CreateCenteredCell(purchase.Id.ToString()));
                    table.AddCell(CreateCenteredCell(purchase.Date.ToString()));
                    table.AddCell(CreateCenteredCell(purchase.TotalCost.ToString()));
                    table.AddCell(CreateCenteredCell(purchase.Supplier.Name));
                    table.AddCell(CreateCenteredCell($"{purchase.User.FirstName} {purchase.User.LastName}"));
                }
            }

            Paragraph totalPurchase = new Paragraph("Total Purchase : " + table.Rows.Count.ToString() + "\nDate Created : " + DateTime.Now.Date.ToString() + "\n\n ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD));
            totalPurchase.Alignment = Element.ALIGN_LEFT;
            doc.Add(totalPurchase);

            doc.Add(table);

            doc.Close(); // Close the document after adding content
        }
        private void GeneratePdfDistribution(string filePath)
        {
            Document doc = new Document();
            FileStream combinedFs = new FileStream(filePath, FileMode.Create);

            PdfWriter combinedWriter = PdfWriter.GetInstance(doc, combinedFs);
            doc.Open();
            Font font = FontFactory.GetFont("Century Gothic", 8f, Font.BOLD,
            iTextSharp.text.BaseColor.BLACK);//Initializing Information font

            string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "logo.png");
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.ScalePercent(50f);
            logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
            doc.Add(logo);

            // Add the logo to the document and Informations
            Paragraph paragraph = new Paragraph("Internation Book Centre\nEmail Address : IBCStore@Gmail.com\nPhone Number : 012 852 645\n", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD));
            paragraph.Alignment = Element.ALIGN_RIGHT;
            doc.Add(paragraph);

            // Add Title
            Paragraph title = new Paragraph("DISTRIBUTION INVOICE", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);

            // Add some space between the logo and the text
            doc.Add(new Paragraph("\n\n"));

            // Create a table with 7 columns
            var table = new PdfPTable(6);
            table.WidthPercentage = 100;

            // Set the minimum height of the cells
            table.DefaultCell.MinimumHeight = 20f;

            // Add the headers to the table
            table.AddCell(new Phrase("Id"));
            table.AddCell(new Phrase("Product"));
            table.AddCell(new Phrase("Branch"));
            table.AddCell(new Phrase("Quanity"));
            table.AddCell(new Phrase("Date"));
            table.AddCell(new Phrase("Warehouse"));

            var distribution1 = db.Distributions.ToList();
            //Add the data from the invoices to the table
            if (distribution1 != null)
            {
                foreach (var distribution in distribution1)
                {
                    table.AddCell(CreateCenteredCell(distribution.Id.ToString()));
                    table.AddCell(CreateCenteredCell(getName(distribution.ProductId)));
                    table.AddCell(CreateCenteredCell(distribution.Branch.Address));
                    table.AddCell(CreateCenteredCell(distribution.Quantity.ToString()));
                    table.AddCell(CreateCenteredCell(distribution.Date.ToString()));
                    table.AddCell(CreateCenteredCell(distribution.WarehouseId.ToString()));
                }
            }

            Paragraph totalDistribution = new Paragraph("Total Distribution : " + table.Rows.Count.ToString() + "\nDate Created : " + DateTime.Now.Date.ToString() + "\n\n ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD));
            totalDistribution.Alignment = Element.ALIGN_LEFT;
            doc.Add(totalDistribution);

            doc.Add(table);

            doc.Close(); // Close the document after adding content
        }
        private void GeneratePdfSale(string filePath)
        {
            Document doc = new Document();
            FileStream combinedFs = new FileStream(filePath, FileMode.Create);

            PdfWriter combinedWriter = PdfWriter.GetInstance(doc, combinedFs);
            doc.Open();
            Font font = FontFactory.GetFont("Century Gothic", 8f, Font.BOLD,
            iTextSharp.text.BaseColor.BLACK);//Initializing Information font

            string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "logo.png");
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.ScalePercent(50f);
            logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
            doc.Add(logo);

            // Add the logo to the document and Informations
            Paragraph paragraph = new Paragraph("Internation Book Centre\nEmail Address : IBCStore@Gmail.com\nPhone Number : 012 852 645\n", new Font(Font.FontFamily.HELVETICA, 8, Font.BOLD));
            paragraph.Alignment = Element.ALIGN_RIGHT;
            doc.Add(paragraph);

            // Add Title
            Paragraph title = new Paragraph("SALE INVOICE", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);

            // Add some space between the logo and the text
            doc.Add(new Paragraph("\n\n"));

            // Create a table with 7 columns
            var table = new PdfPTable(4);
            table.WidthPercentage = 100;

            // Set the minimum height of the cells
            table.DefaultCell.MinimumHeight = 20f;

            // Add the headers to the table
            table.AddCell(new Phrase("Id"));
            table.AddCell(new Phrase("Product"));
            table.AddCell(new Phrase("Invoice"));
            table.AddCell(new Phrase("Quanity"));

            var sale1 = db.Sales.ToList();
            //Add the data from the invoices to the table
            if (sale1 != null)
            {
                foreach (var sale in sale1)
                {
                    table.AddCell(CreateCenteredCell(sale.Id.ToString()));
                    table.AddCell(CreateCenteredCell(sale.Product.Name));
                    table.AddCell(CreateCenteredCell(sale.InvoiceId.ToString()));
                    table.AddCell(CreateCenteredCell(sale.Quantity.ToString()));
                }
            }

            Paragraph totalSale = new Paragraph("Total Sale : " + table.Rows.Count.ToString() + "\nDate Created : " + DateTime.Now.Date.ToString() + "\n\n ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD));
            totalSale.Alignment = Element.ALIGN_LEFT;
            doc.Add(totalSale);

            doc.Add(table);

            doc.Close(); // Close the document after adding content
        }
        private void GenerateExcelInvoice(string fileName)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Invoice");

                // Add the column headers
                worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 2).Value = "Date";
                worksheet.Cell(1, 3).Value = "Total";
                worksheet.Cell(1, 4).Value = "Customer";
                worksheet.Cell(1, 5).Value = "User";
                worksheet.Cell(1, 6).Value = "Branch";

                var invoices = db.Invoices.ToList(); // Retrieve invoices from the database
                int row = 2;

                foreach (var invoice in invoices)
                {
                    worksheet.Cell(row, 1).Value = invoice.Id.ToString();
                    worksheet.Cell(row, 2).Value = invoice.Date.ToString();
                    worksheet.Cell(row, 3).Value = invoice.Total.ToString();
                    worksheet.Cell(row, 4).Value = invoice.Customer.Name;
                    worksheet.Cell(row, 5).Value = $"{invoice.User.FirstName} {invoice.User.LastName}";
                    worksheet.Cell(row, 6).Value = invoice.Branch.Address;

                    row++;
                }

                workbook.SaveAs(fileName);
            }
        }

        private void GenerateExcelPurchase(string fileName)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Purchase Invoice");

                // Add the column headers
                worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 2).Value = "Date";
                worksheet.Cell(1, 3).Value = "Total Cost";
                worksheet.Cell(1, 4).Value = "Supplier";
                worksheet.Cell(1, 5).Value = "Customer";

                var purchases = db.Purchases.ToList(); // Retrieve invoices from the database
                int row = 2;

                foreach (var purchase in purchases)
                {
                    worksheet.Cell(row, 1).Value = purchase.Id.ToString();
                    worksheet.Cell(row, 2).Value = purchase.Date.ToString();
                    worksheet.Cell(row, 3).Value = purchase.TotalCost.ToString();
                    worksheet.Cell(row, 4).Value = purchase.Supplier.Name;
                    worksheet.Cell(row, 5).Value = $"{purchase.User.FirstName} {purchase.User.LastName}";
                    row++;
                }

                workbook.SaveAs(fileName);
            }
        }
        private void GenerateExcelDistribution(string fileName)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Distribution Invoice");

                // Add the column headers
                worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 2).Value = "Product";
                worksheet.Cell(1, 3).Value = "Branch";
                worksheet.Cell(1, 4).Value = "Quanity";
                worksheet.Cell(1, 5).Value = "Date";
                worksheet.Cell(1, 6).Value = "Warehouse";

                var distribtuions = db.Distributions.ToList(); // Retrieve invoices from the database
                int row = 2;

                foreach (var distribtuion in distribtuions)
                {
                    worksheet.Cell(row, 1).Value = distribtuion.Id.ToString();
                    worksheet.Cell(row, 2).Value = getName(distribtuion.ProductId);
                    worksheet.Cell(row, 3).Value = distribtuion.Branch.Address;
                    worksheet.Cell(row, 4).Value = distribtuion.Quantity.ToString();
                    worksheet.Cell(row, 5).Value = distribtuion.Date.ToString();
                    worksheet.Cell(row, 6).Value = distribtuion.WarehouseId.ToString();
                    row++;
                }

                workbook.SaveAs(fileName);
            }
        }
        private string getName(int id)
        {
            var temp = db.Products.Where(s => s.Id == id).FirstOrDefault();
            if (temp != null)
            {
                return temp.Name;
            }
            return "";
        }
        private void GenerateExcelSale(string fileName)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sale Invoice");

                // Add the column headers
                worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 2).Value = "Product";
                worksheet.Cell(1, 4).Value = "Quanity";
                worksheet.Cell(1, 5).Value = "Date";
                worksheet.Cell(1, 6).Value = "Invoice";

                var sales = db.Sales.ToList(); // Retrieve invoices from the database
                int row = 2;

                foreach (var sale in sales)
                {
                    worksheet.Cell(row, 1).Value = sale.Id.ToString();
                    worksheet.Cell(row, 2).Value = sale.Product.Name;
                    worksheet.Cell(row, 3).Value = sale.Quantity.ToString();
                    worksheet.Cell(row, 4).Value = sale.InvoiceId.ToString();
                    row++;
                }

                workbook.SaveAs(fileName);
            }
        }
        private void StockCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckStockLevelCheckBox.IsChecked == true)
            {

                using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
                {
                    if (SearchProductTextBox.Text != "")
                    {
                        var stock = db.Stocks.ToList();
                        var stock1 = ConvertStockData(stock);
                        ProductDataGrid.ItemsSource = FilterStockLevel(SearchProductByNameInStock(stock1, SearchProductTextBox.Text)).DefaultView;
                    }
                    else
                    {
                        var stock = db.Stocks.ToList();
                        ProductDataGrid.ItemsSource = FilterStockLevel(ConvertStockData(stock)).DefaultView;
                    }
                }

            }
        }
        private void StockCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Button_Click(sender, e);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
            {
                if (SearchProductTextBox.Text != "")
                {
                    var stock = db.Stocks.ToList();
                    var stock1 = ConvertStockData(stock);
                    ProductDataGrid.ItemsSource = SearchProductByNameInStock(stock1, SearchProductTextBox.Text).DefaultView;
                }
                else
                {
                    var stock = db.Stocks.ToList();
                    ProductDataGrid.ItemsSource = ConvertStockData(stock).DefaultView;
                }
            }
        }
        private DataTable SearchUserByNameInInvoice(DataTable invoices, string name)
        {
            using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
            {
                DataTable dataTable = new DataTable();
                DataTable result = invoices;
                foreach (DataRow row in result.Rows.Cast<DataRow>().ToList())
                {
                    if (!row[5].ToString().ToLower().Contains(name.ToLower()))
                    {
                        result.Rows.Remove(row);
                    }
                }
                return result;
            }
        }

        private DataTable SearchUserByNamePurchase(DataTable purchase, string name)
        {
            using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
            {
                DataTable dataTable = new DataTable();
                DataTable result = purchase;
                foreach (DataRow row in result.Rows.Cast<DataRow>().ToList())
                {
                    if (!row[4].ToString().ToLower().Contains(name.ToLower()))
                    {
                        result.Rows.Remove(row);
                    }
                }
                return result;
            }
        }
        private DataTable SearchProductByNameInStock(DataTable stocks, string name)
        {
            using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
            {
                DataTable dataTable = new DataTable();
                DataTable result = stocks;
                foreach (DataRow row in result.Rows.Cast<DataRow>().ToList())
                {
                    if (!row[1].ToString().ToLower().Contains(name.ToLower()))
                    {
                        result.Rows.Remove(row);
                    }
                }
                return result;
            }
        }
        private DataTable ConvertStockData(List<Stock> Stocks)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Quantity", typeof(int));
            table.Columns.Add("Price", typeof(decimal));
            table.Columns.Add("Category", typeof(string));
            table.Columns.Add("StockLevel", typeof(int));
            table.Columns.Add("Branch", typeof(string));
            for (int i = 0; i <= Stocks.Count - 1; i++)
            {
                DataRow row = table.NewRow();
                table.Rows.Add(Stocks[i].Id, Stocks[i].Product.Name, Stocks[i].Quantity, Stocks[i].Product.Price, Stocks[i].Product.Category.Name, Stocks[i].Level, Stocks[i].Branch.Address);
            }
            return table;
        }
        private DataTable ConvertDistributionData(List<Distribution> Distributions)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Product", typeof(string));
            table.Columns.Add("Branch", typeof(string));
            table.Columns.Add("Quanity", typeof(int));
            table.Columns.Add("Date", typeof(DateTime));
            table.Columns.Add("Warehouse", typeof(string));
            for (int i = 0; i < Distributions.Count; i++)
            {
                DataRow row = table.NewRow();
                row["Id"] = Distributions[i].Id;
                row["Quanity"] = Distributions[i].Quantity;
                row["Date"] = Distributions[i].Date;
                using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
                {
                    var products = db.Products.ToList();
                    var branches = db.Branches.ToList();
                    var warehouses = db.Warehouses.ToList();
                    foreach (var product in products)
                    {
                        if (product.Id == Distributions[i].ProductId)
                        {
                            row["Product"] = product.Name;
                        }
                    }
                    foreach (var branch in branches)
                    {
                        if (branch.Id == Distributions[i].BranchId)
                        {
                            row["Branch"] = branch.Address;
                        }
                    }
                    foreach (var warehouse in warehouses)
                    {
                        if (warehouse.Id == Distributions[i].WarehouseId)
                        {
                            row["Warehouse"] = warehouse.Id;
                        }
                    }

                }
                table.Rows.Add(row);
            }
            return table;
        }
        private DataTable ConvertPurchaseTable(List<Purchase> Purchases)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Date", typeof(DateTime));
            table.Columns.Add("Total Cost", typeof(Decimal));
            table.Columns.Add("Supplier", typeof(string));
            table.Columns.Add("User", typeof(string));
            for (int i = 0; i < Purchases.Count; i++)
            {
                DataRow row = table.NewRow();
                row["Id"] = Purchases[i].Id;
                row["Date"] = Purchases[i].Date;
                row["Total Cost"] = Purchases[i].TotalCost;
                using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
                {
                    var suppliers = db.Suppliers.ToList();
                    var users = db.Users.ToList();

                    foreach (var supplier in suppliers)
                    {
                        if (supplier.Id == Purchases[i].SupplierId)
                        {
                            row["Supplier"] = supplier.Name;
                        }
                    }
                    foreach (var user in users)
                    {
                        if (user.Id == Purchases[i].UserId)
                        {
                            row["User"] = user.FirstName + " " + user.LastName;
                        }
                    }
                }
                table.Rows.Add(row);
            }
            return table;
        }
        private DataTable ConvertInvoiceName(List<Invoice> Invoices)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Date", typeof(DateTime));
            dt.Columns.Add("Total", typeof(decimal));
            dt.Columns.Add("Branch", typeof(string));
            dt.Columns.Add("Customer", typeof(string));
            dt.Columns.Add("User", typeof(string));

            for (int i = 0; i <= Invoices.Count - 1; i++)
            {
                DataRow row = dt.NewRow();
                row["Id"] = Invoices[i].Id;
                row["Date"] = Invoices[i].Date;
                row["Total"] = Invoices[i].Total;
                using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
                {
                    var customer = db.Customers.ToList();
                    var user = db.Users.ToList();
                    var branch = db.Branches.ToList();

                    foreach (var customers in customer)
                    {
                        if (customers.Id == Invoices[i].CustomerId)
                        {
                            row["Customer"] = customers.Name;
                        }
                    }
                    foreach (var users in user)
                    {
                        if (users.Id == Invoices[i].UserId)
                        {
                            row["User"] = users.FirstName + " " + users.LastName;
                        }
                    }
                    foreach (var branches in branch)
                    {
                        if (branches.Id == Invoices[i].BranchId)
                        {
                            row["Branch"] = branches.Address;
                        }
                    }

                }
                dt.Rows.Add(row);

            }
            return dt;
        }
        private DataTable FilterStockLevel(DataTable table)
        {
            var filter = new DataTable();
            var result = table;
            foreach (DataRow row in result.Rows.Cast<DataRow>().ToList())
            {
                int quantity = int.Parse(row[2].ToString());
                int level = int.Parse((row[5].ToString()));
                if (quantity > level)
                {
                    result.Rows.Remove(row);
                }
            }
            return result;
        }
        private void rbSearchDuration_Purchase_Checked(object sender, RoutedEventArgs e)
        {
            if (rbSearchDuration_Purchase.IsChecked == true)
            {
                StartDatePicker_Purchase.IsEnabled = false;
                EndDatePicker_Purchase.IsEnabled = false;
            }
            if (rbStartEnd_Purchase.IsChecked == false)
            {
                DurationPicker_Purchase.IsEnabled = true;
            }
        }

        private void rbStartEnd_Purchase_Checked(object sender, RoutedEventArgs e)
        {
            if (rbStartEnd_Purchase.IsChecked == true)
            {
                DurationPicker_Purchase.IsEnabled = false;
            }
            if (rbSearchDuration_Purchase.IsChecked == false)
            {
                StartDatePicker_Purchase.IsEnabled = true;
                EndDatePicker_Purchase.IsEnabled = true;
            }
        }

        private void btnLoad_Purchase_Click(object sender, RoutedEventArgs e)
        {
            if (rbSearchDuration_Purchase.IsChecked == true || rbStartEnd_Purchase.IsChecked == true)
            {
                try
                {
                    if (rbSearchDuration_Purchase.IsChecked == false && rbStartEnd.IsChecked == false)
                    {
                        MessageBox.Show("Please select options for searching!");
                    }
                    if (rbSearchDuration_Purchase.IsChecked == true)
                    {
                        datagridViewPurchase.ItemsSource = ConvertPurchaseTable(SearchPurchaseByDateDuration(DurationPicker_Purchase.SelectedIndex)).DefaultView;
                    }
                    if (rbStartEnd_Purchase.IsChecked == true)
                    {
                        if (StartDatePicker_Purchase.SelectedDate.Value > EndDatePicker_Purchase.SelectedDate.Value)
                        {
                            MessageBox.Show("Start date must be smaller than end date!");
                        }
                        else
                        {
                            datagridViewPurchase.ItemsSource = ConvertPurchaseTable(SearchPurchaseByDate(StartDatePicker_Purchase.SelectedDate.Value, EndDatePicker_Purchase.SelectedDate.Value)).DefaultView;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Message :" + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select options for loading informations!");
            }
        }
        private void ProductDataGrid_Initialized(object sender, EventArgs e)
        {
            using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
            {
                var stocks = db.Stocks.ToList();
                ProductDataGrid.ItemsSource = ConvertStockData(stocks).DefaultView;
            }
        }

        private void datagridViewInvoice_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

            if (e.PropertyName == "BranchId" || e.PropertyName == "SalesId" || e.PropertyName == "UserId" || e.PropertyName == "CustomerId")
            {
                e.Cancel = true;
            }
        }

        private void rbSearchDuration_Distribution_Checked(object sender, RoutedEventArgs e)
        {
            if (rbSearchDuration_Purchase.IsChecked == true)
            {
                StartDatePicker_Distribution.IsEnabled = false;
                EndDatePicker_Distribution.IsEnabled = false;
            }
            if (rbStartEnd_Distribution.IsChecked == false)
            {
                DurationPicker_Distribution.IsEnabled = true;
            }
        }

        private void rbStartEnd_Distribution_Checked(object sender, RoutedEventArgs e)
        {
            if (rbStartEnd_Distribution.IsChecked == true)
            {
                DurationPicker_Distribution.IsEnabled = false;
            }
            if (rbSearchDuration_Distribution.IsChecked == false)
            {
                StartDatePicker_Distribution.IsEnabled = true;
                EndDatePicker_Distribution.IsEnabled = true;
            }
        }

        private void btnLoad_Distribution_Click(object sender, RoutedEventArgs e)
        {
            if (rbSearchDuration_Distribution.IsChecked == true || rbStartEnd_Distribution.IsChecked == true)
            {
                try
                {
                    if (rbSearchDuration_Distribution.IsChecked == false && rbStartEnd_Distribution.IsChecked == false)
                    {
                        MessageBox.Show("Please select options for searching!");
                    }
                    if (rbSearchDuration_Distribution.IsChecked == true)
                    {
                        datagridviewDistribution.ItemsSource = ConvertDistributionData(SearchDistributionByDateDuration(DurationPicker_Distribution.SelectedIndex)).DefaultView;
                    }
                    if (rbStartEnd_Distribution.IsChecked == true)
                    {
                        if (StartDatePicker_Distribution.SelectedDate.Value > EndDatePicker_Distribution.SelectedDate.Value)
                        {
                            MessageBox.Show("Start date must be smaller than end date!");
                        }
                        else
                        {
                            datagridviewDistribution.ItemsSource = ConvertDistributionData(SearchDistributionByDate(StartDatePicker_Distribution.SelectedDate.Value, EndDatePicker_Distribution.SelectedDate.Value)).DefaultView;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Message :" + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select options for loading informations!");
            }
        }

        private void datagridviewDistribution_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "ProductId" || e.PropertyName == "BranchId" || e.PropertyName == "WarehouseId")
            {
                e.Cancel = true;
            }
        }

        private void ProductDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Picture" || e.PropertyName == "CategoryId" || e.PropertyName == "PurchaseDetails" || e.PropertyName == "Sales")
            {
                e.Cancel = true;
            }
        }

        private void datagridViewInvoice_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (datagridViewInvoice.SelectedItem != null)
                {
                    DataRowView dataRow = (DataRowView)datagridViewInvoice.SelectedItem;
                    string id = dataRow.Row.ItemArray[0].ToString();//Get id of invoice
                    int invoiceID = Convert.ToInt32(id);//Convert id to int
                    using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
                    {
                        var sales = db.Sales.Where(x => x.InvoiceId == invoiceID).ToList();//Get all sales of invoice by invoice id
                        InvoiceDetail invoiceDetail = new InvoiceDetail(ConvertDataSaleOfInvoice(sales));//Convert data to datatable and send to InvoiceDetail form (Function Converting is on line 105)
                        invoiceDetail.ShowDialog();//Show InvoiceDetail form
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message :" + ex.Message);
            }
        }

        private DataTable ConvertDataSaleOfInvoice(List<Sale> sales)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Product");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("Price");
            dt.Columns.Add("InvoiceID");
            foreach (var item in sales)
            {
                dt.Rows.Add(item.Id, item.Product.Name, item.Quantity, item.Product.Price, item.InvoiceId);//Add data to rows of datatable 
            }
            return dt;//Return datatable
        }


        private void btnLoadSale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (rbSearchDuration_Sale.IsChecked == true || rbStartEnd_Sale.IsChecked == true)
                {
                    if (rbSearchDuration_Sale.IsChecked == false && rbStartEnd_Sale.IsChecked == false)
                    {
                        MessageBox.Show("Please select options for searching!");
                    }
                    if (rbSearchDuration_Sale.IsChecked == true)
                    {
                        datagridViewSale.ItemsSource = ConvertSaleName(SearchSaleByDateDuration(DurationPicker_Sale.SelectedIndex)).DefaultView;
                    }
                    if (rbStartEnd_Sale.IsChecked == true)
                    {
                        if (StartDatePicker_Sale.SelectedDate.Value > EndDatePicker_Sale.SelectedDate.Value)
                        {
                            MessageBox.Show("Start date must be smaller than end date!");
                        }
                        else
                        {
                            datagridViewSale.ItemsSource = ConvertSaleName(SearchSaleByDate(StartDatePicker_Sale.SelectedDate.Value, EndDatePicker_Sale.SelectedDate.Value)).DefaultView;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select option to load informations!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message :" + ex.Message);
            }
        }

        private List<Sale> SearchSaleByDate(DateTime value1, DateTime value2)
        {
            List<Sale> result = new List<Sale>();

            using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
            {
                var sales = db.Sales.Include(x => x.Invoice).Include(x => x.Product).ToList();
                //write query to find sales within value1 and value2 and include their product and invoice
                result = sales.Where(x => x.Invoice.Date >= value1 && x.Invoice.Date.Date <= value2).ToList();

                //result = db.Sales.Where(x => x.Invoice.Date >= value1 && x.Invoice.Date <= value2).Include(x=>x.Invoice).Include(x=>x.Product).ToList();
            }
            return result;
        }

        private DataTable ConvertSaleName(List<Sale> sales)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Product");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("Price");
            dt.Columns.Add("InvoiceID");
            dt.Columns.Add("Date");
            foreach (var item in sales)
            {
                DataRow row = dt.NewRow();
                row["ID"] = item.Id;
                // row["Product"] = item.Product.Name;
                row["Quantity"] = item.Quantity;
                // row["Price"] = item.Product.Price;
                row["InvoiceID"] = item.InvoiceId;
                // row["Date"] = item.Invoice.Date;
                using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
                {
                    var products = db.Products.ToList();
                    var invoices = db.Invoices.ToList();
                    foreach (var product in products)
                    {
                        if (product.Id == item.ProductId)
                        {
                            row["Product"] = product.Name;
                            row["Price"] = product.Price;
                        }
                    }
                    foreach (var invoice in invoices)
                    {
                        if (invoice.Id == item.InvoiceId)
                        {
                            row["Date"] = invoice.Date;
                        }
                    }
                }
                dt.Rows.Add(row);

            }
            return dt;
        }

        private List<Sale> SearchSaleByDateDuration(int v)
        {
            List<Sale> result = new List<Sale>();
            switch (v)
            {
                case 1:
                    using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
                    {
                        var sales = db.Sales.Include(x => x.Invoice).ToList();
                        result = sales.Where(x => x.Invoice.Date.Date == DateTime.Today).ToList();

                    }
                    break;
                case 2:
                    using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
                    {
                        DateTime startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                        DateTime endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

                        result = db.Sales.Where(x => x.Invoice.Date >= startOfWeek && x.Invoice.Date <= endOfWeek).ToList();

                    }
                    break;
                case 3:
                    using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
                    {

                        var sales = db.Sales.Where(x => x.Invoice.Date.Month == DateTime.Now.Month).ToList();
                        result = sales;
                    }
                    break;
                case 4:
                    using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
                    {
                        var sales = db.Sales.Where(x => x.Invoice.Date.Year == DateTime.Now.Year).ToList();
                        result = sales;
                    }
                    break;

            }
            return result;
        }

        private void rbSearchDuration_Sale_Checked(object sender, RoutedEventArgs e)
        {
            DurationPicker_Sale.IsEnabled = true;
        }

        private void rbStartEnd_Sale_Checked(object sender, RoutedEventArgs e)
        {
            EndDatePicker_Sale.IsEnabled = true;
            StartDatePicker_Sale.IsEnabled = true;
            DurationPicker_Sale.IsEnabled = false;

        }

        private void closeApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ChatButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(chatUsername);
            Chat chat = new Chat(chatUsername);
            chat.Show();
        }
    }    

}




