using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Paragraph = iTextSharp.text.Paragraph;
using Path = System.IO.Path;
using System.Collections.ObjectModel;
using System.Data;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using OfficeOpenXml;

namespace IBC_Management_System
{
    /// <summary>
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : Window
    {
        IBC_STOREIIEntities db = new IBC_STOREIIEntities();
        private Dictionary<int, string> userPasswords = new Dictionary<int, string>();
        private bool exporting = false;

        public Search()
        {
            InitializeComponent();
        }

        private void available_search_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            TestTextbox.Text = "";
            datagrid_all.ItemsSource = null;
            if(available_search_cb.SelectedIndex == 0)
            {
                TestTextbox.Tag = "Search by Firstname,Lastname,Username";
            }
            else if(available_search_cb.SelectedIndex == 1)
            {
                TestTextbox.Tag = "Search by Product Name,Category";
            }
            else if(available_search_cb.SelectedIndex == 2)
            {
                TestTextbox.Tag = "Search by Address";
            }

        }

        private void SearchData(string selectCat, string searchText)
        {
            searchText = searchText.ToLower();

            switch (selectCat)
            {
                case "User":
                    SearchAndDisplay<User>(db.Users, u =>
                        u.Username.ToLower().Contains(searchText) ||
                        u.FirstName.ToLower().Contains(searchText) ||
                        u.LastName.ToLower().Contains(searchText),
                        ConvertUserData);
                    break;
                case "Product":
                    SearchAndDisplay<Product>(db.Products, p =>
                        p.Name.ToLower().Contains(searchText) ||
                        p.Category.Name.ToLower().Contains(searchText),
                        ConvertProductData);
                    break;
                case "Branch":
                    SearchAndDisplay<Branch>(db.Branches, b =>
                        b.Address.ToLower().Contains(searchText),
                        ConvertBranchData);
                    break;
                default:
                    MessageBox.Show("Not found Category that you want to find!");
                    break;
            }
        }
        private DataTable ConvertBranchData(List<Branch> branches)
        {
            var table = new DataTable();
            table.Columns.AddRange(new[]
            {
                new DataColumn("ID"),
                new DataColumn("Address"),
                new DataColumn("PhoneNumber")
             });

            foreach (var branch in branches)
            {
                var dr = table.NewRow();
                dr["ID"] = branch.Id;
                dr["Address"] = branch.Address;
                dr["PhoneNumber"] = branch.PhoneNumber;
                table.Rows.Add(dr);
            }

            return table;
        }
        private DataTable ConvertProductData(List<Product> products)
        {
            var categories = db.Categories.ToList();

            var table = new DataTable();
            table.Columns.AddRange(new[]
            {
                new DataColumn("ID"),
                new DataColumn("ProductName"),
                new DataColumn("ProductPrice"),
                new DataColumn("ProductBarcode"),
                new DataColumn("Category")
             });

            foreach (var product in products)
            {
                var dr = table.NewRow();
                dr["ID"] = product.Id;
                dr["ProductName"] = product.Name;
                dr["ProductPrice"] = product.Price;
                dr["ProductBarcode"] = product.BarCode;

                var productCategory = categories.FirstOrDefault(cat => cat == product.Category);
                if (productCategory != null)
                {
                    dr["Category"] = productCategory.Name;
                }

                table.Rows.Add(dr);
            }

            return table;
        }


        private void SearchAndDisplay<T>(IQueryable<T> query, Func<T, bool> filterFunc, Func<List<T>, DataTable> convertFunc)
        {
            try
            {
                var temp = query.Where(filterFunc).ToList();

                if (temp.Count == 0)
                {
                    MessageBox.Show($"{typeof(T).Name} not found.");
                    datagrid_all.ItemsSource = null;
                    return;
                }

                DataTable table = convertFunc(temp);
                datagrid_all.ItemsSource = table.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching for {typeof(T).Name}s: {ex.Message}");
            }
        }
        private DataTable ConvertUserData(List<User> users)
        {
            var roles = db.Roles.ToList();

            var table = new DataTable();
            table.Columns.AddRange(new[]
            {
                new DataColumn("ID"),
                new DataColumn("FirstName"),
                new DataColumn("LastName"),
                new DataColumn("Email"),
                new DataColumn("PhoneNumber"),
                new DataColumn("Address"),
                new DataColumn("Username"),
                new DataColumn("Password"),
                new DataColumn("Role")
             });

            foreach (var user in users)
            {
                var dr = table.NewRow();
                dr["ID"] = user.Id;
                dr["FirstName"] = user.FirstName;
                dr["LastName"] = user.LastName;
                dr["Email"] = user.Email;
                dr["Username"] = user.Username;
                dr["Password"] = new string('*', user.Password.Length);
                dr["Address"] = user.Address;
                dr["PhoneNumber"] = user.PhoneNumber;

                var userRole = roles.FirstOrDefault(role => role == user.Role);
                if (userRole != null)
                {
                    dr["Role"] = userRole.Description;
                    userPasswords[user.Id] = user.Password;
                }

                table.Rows.Add(dr);
            }

            return table;
        }

        private void search_all_bt_Click_1(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedItems = available_search_cb.SelectedItem as ComboBoxItem;

            if (selectedItems == null)
            {
                MessageBox.Show("Please select a category.");
                return;
            }
            string selectCat = selectedItems.Content.ToString();
            string searchText = TestTextbox.Text.Trim();
            try
            {
                SearchData(selectCat, searchText);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void print_search_bt_Click(object sender, RoutedEventArgs e)
        {
            //private void Print_search_bt_Click(object sender, RoutedEventArgs e)
            //{
            Microsoft.Win32.SaveFileDialog saveDialog = new SaveFileDialog();

            saveDialog.FileName = "DataExport";

            saveDialog.Filter = "PDF Files|*.pdf|Excel Files|*.xlsx";

            if (saveDialog.ShowDialog() == true)
            {
                string filePath = saveDialog.FileName;
                string fileExtension = Path.GetExtension(filePath).ToLower();

                exporting = true; // Set the flag for exporting

                if (fileExtension == ".pdf")
                {
                    using (Document combinedDoc = new Document(PageSize.A4.Rotate()))
                    {
                        using (FileStream combinedFs = new FileStream(filePath, FileMode.Create))
                        {
                            PdfWriter combinedWriter = PdfWriter.GetInstance(combinedDoc, combinedFs);
                            combinedDoc.Open();

                            string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "logo.png");
                            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                            logo.ScalePercent(50f);
                            logo.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                            combinedDoc.Add(logo);

                            Font titleFont = new Font(Font.FontFamily.HELVETICA, 24, Font.BOLD);
                            Paragraph title = new iTextSharp.text.Paragraph("Data Information", titleFont);
                            title.Alignment = Element.ALIGN_LEFT;
                            combinedDoc.Add(title);

                            combinedDoc.Add(new Paragraph("\n\n"));

                            PdfPTable pdfTable = new PdfPTable(datagrid_all.Columns.Count);
                            pdfTable.WidthPercentage = 100;

                            foreach (DataGridColumn column in datagrid_all.Columns)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(column.Header.ToString()));
                                cell.FixedHeight = 40f;

                                cell.MinimumHeight = 40f;

                                cell.BackgroundColor = new iTextSharp.text.BaseColor(192, 192, 192);
                                pdfTable.AddCell(cell);
                            }

                            foreach (var item in datagrid_all.Items)
                            {
                                if (item is DataRowView rowView)
                                {
                                    foreach (DataGridColumn column in datagrid_all.Columns)
                                    {
                                        var cellContent = column.GetCellContent(item);
                                        if (cellContent is TextBlock textBlock)
                                        {
                                            string cellValue = textBlock.Text;

                                            if (column.Header.ToString() == "Password")
                                            {
                                                // Check if exporting and if cellValue contains only asterisks
                                                if (exporting && column.Header.ToString() == "Password")
                                                {
                                                    // Reveal the actual password for exporting
                                                    if (userPasswords.TryGetValue(Convert.ToInt32(rowView["ID"]), out string password))
                                                    {
                                                        cellValue = password;
                                                    }
                                                }
                                            }
                                            PdfPCell cell = new PdfPCell(new Phrase(cellValue));
                                            cell.FixedHeight = 40f;

                                            cell.MinimumHeight = 40f;

                                            cell.BackgroundColor = new iTextSharp.text.BaseColor(173, 216, 230);
                                            cell.HorizontalAlignment = Element.ALIGN_CENTER; // Set the alignment
                                            pdfTable.AddCell(cell);
                                        }
                                    }
                                }
                            }
                            combinedDoc.Add(pdfTable);
                            combinedDoc.Close();
                        }
                    }

                    MessageBox.Show("PDF generated successfully!");
                }
                else if (fileExtension == ".xlsx")
                {
                    try
                    {
                        using (var package = new ExcelPackage())
                        {
                            var worksheet = package.Workbook.Worksheets.Add("DataExport");

                            int rowIndex = 1;

                            // Add headers to the Excel sheet
                            for (int columnIndex = 1; columnIndex <= datagrid_all.Columns.Count; columnIndex++)
                            {
                                worksheet.Cells[rowIndex, columnIndex].Value = datagrid_all.Columns[columnIndex - 1].Header;
                            }

                            rowIndex++;

                            // Add data rows to the Excel sheet
                            foreach (var item in datagrid_all.Items)
                            {
                                if (item is DataRowView rowView)
                                {
                                    foreach (DataGridColumn column in datagrid_all.Columns)
                                    {
                                        var cellContent = column.GetCellContent(item);
                                        if (cellContent is TextBlock textBlock)
                                        {
                                            string cellValue = textBlock.Text;

                                            if (column.Header.ToString() == "Password")
                                            {
                                                if (exporting && column.Header.ToString() == "Password")
                                                {
                                                    if (exporting && column.Header.ToString() == "Password")
                                                    {
                                                        if (userPasswords.TryGetValue(Convert.ToInt32(rowView["ID"]), out string password))
                                                        {
                                                            cellValue = password;
                                                        }
                                                    }
                                                }
                                            }

                                            worksheet.Cells[rowIndex, column.DisplayIndex + 1].Value = cellValue;
                                        }
                                    }
                                    rowIndex++;
                                }
                            }

                            // Auto-fit columns for better visibility
                            worksheet.Cells.AutoFitColumns();

                            // Save the Excel package to the specified file
                            File.WriteAllBytes(filePath, package.GetAsByteArray());

                            MessageBox.Show("Excel file generated successfully!");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while generating the Excel file: " + ex.Message);
                    }
                }

                exporting = false;


            }
        }
        private void TestTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ComboBoxItem selectedItems = available_search_cb.SelectedItem as ComboBoxItem;

            if (selectedItems == null)
            {
                MessageBox.Show("Please select a category.");
                return;
            }

            string selectCat = selectedItems.Content.ToString();
            string searchText = TestTextbox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(searchText))
            {

                try
                {
                    SearchData(selectCat, "");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            else if (searchText.Length >= 1)
            {
                try
                {
                    SearchData(selectCat, searchText);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void TestTextbox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }
    }
}

