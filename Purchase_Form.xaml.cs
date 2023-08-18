using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
using Spire.Xls;
using System.Xml.Linq;
using Paragraph = iTextSharp.text.Paragraph;
using Path = System.IO.Path;
using Rectangle = iTextSharp.text.Rectangle;
using System.Diagnostics.PerformanceData;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Controls.Primitives;
using Microsoft.Office.Interop.Excel;
using Font = iTextSharp.text.Font;
using Window = System.Windows.Window;
using Workbook = Spire.Xls.Workbook;
using Worksheet = Spire.Xls.Worksheet;

namespace IBC_Management_System
{
    /// <summary>
    /// Interaction logic for Purchase_Form.xaml
    /// </summary>
    public partial class Purchase_Form : Window
    {
        DataSet dataSet = null;
        SqlDataAdapter dataAdapter = null;
        SqlConnection sqlConnection = null;
        SqlCommandBuilder cmdBuilder = null;
        IBC_STOREIIEntities db = new IBC_STOREIIEntities();
        
        public ObservableCollection<PurchaseItem> PurchaseItems { get; set; }

       
        

        private bool supplierSelected = false;
        int countId = 0;
        int purchaseId = 0;
        int countData = 0;
        public int UserId { get; set; }
        public string UserName { get; set; }
        public class PurchaseItem //: INotifyPropertyChanged
        {
            //public event PropertyChangedEventHandler PropertyChanged;

            public string ProductName { get; set; }
            public string SupplierName { get; set; }
            public int Id { get; set; }
            public int QTY { get; set; }
            public double Cost { get; set; }
            public double Amount => QTY * Cost;
            // Calculated property
        }
        public Purchase_Form()
        {
            InitializeComponent();
            PurchaseItems = new ObservableCollection<PurchaseItem>();
            LoadData();
            CallLoadFunction();
            Total_Purchase.Text = "0";
            DataContext = this;
            datagridviews.ItemsSource = PurchaseItems;
            int maxPurchaseId = db.Purchases.Select(p => p.Id).DefaultIfEmpty().Max();
            purchaseId = maxPurchaseId + 1;
            datedisplay.Text = DateTime.Now.ToShortDateString();

           
        }
        public void LoadData()
        {
            var supplier = db.Suppliers.ToList();
            foreach (var item in supplier)
            {
                suppierCombobox.Items.Add(item.Name);
            }
            var product = db.Products.ToList();
            foreach (var item in product)
            {
                productCombobox.Items.Add(item.Name);
            }

        }
        private bool canClose = false; // Initialize to true initially

        private void Closeapp_Click_1(object sender, RoutedEventArgs e)
        {
            if (countData != 0)
            {
                canClose = true; // Prevent the window from closing
                MessageBox.Show("Please click the 'Finish' button before closing the window.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                canClose = false; // Allow the window to close
                this.Close();
            }
        }

        private void Minimize_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Back_Click_1(object sender, RoutedEventArgs e)
        {
            Window1 login = new Window1();
            login.Show();
            this.Close();
        }

        private void Warehouse_Click(object sender, RoutedEventArgs e)
        {
            label_text.Text = "WareHouse";
            WarehouseUI.Visibility = Visibility.Visible;
            DisributionUI.Visibility = Visibility.Collapsed;
            PurchaseUI.Visibility = Visibility.Collapsed;
            SupplierUI.Visibility = Visibility.Collapsed;
            LoadWarehouseData();
        }

        private void Disribution_Click(object sender, RoutedEventArgs e)
        {
            label_text.Text = "Distribution";
            WarehouseUI.Visibility = Visibility.Collapsed;
            DisributionUI.Visibility = Visibility.Visible;
            PurchaseUI.Visibility = Visibility.Collapsed;
            SupplierUI.Visibility = Visibility.Collapsed;
        }

        private void Supplier_Click(object sender, RoutedEventArgs e)
        {
            label_text.Text = "Supplier";
            SupplierUI.Visibility = Visibility.Visible;
            WarehouseUI.Visibility = Visibility.Collapsed;
            DisributionUI.Visibility = Visibility.Collapsed;
            PurchaseUI.Visibility = Visibility.Collapsed;
        }

        private void Purchase_Click(object sender, RoutedEventArgs e)
        {

            label_text.Text = "Purchase";
            SupplierUI.Visibility = Visibility.Collapsed;
            WarehouseUI.Visibility = Visibility.Collapsed;
            DisributionUI.Visibility = Visibility.Collapsed;
            PurchaseUI.Visibility = Visibility.Visible;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (productCombobox.SelectedItem == null ||
                suppierCombobox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(qty.Text) ||
                string.IsNullOrWhiteSpace(cost.Text))
            {

                MessageBox.Show("Please fill in all required fields.");
                return;
            }
            countData++;
            countId += 1;
            double amount = int.Parse(qty.Text) * double.Parse(cost.Text);
            Total_Purchase.Text = (double.Parse(Total_Purchase.Text) + amount).ToString();

            var newItem = new PurchaseItem
            {
                ProductName = productCombobox.SelectedItem.ToString(),
                SupplierName = suppierCombobox.SelectedItem.ToString(),
                QTY = int.Parse(qty.Text),
                Cost = double.Parse(cost.Text),
                Id = countId
            };

            PurchaseItems.Add(newItem);
            // Clear input fields after adding
            productCombobox.SelectedIndex = -1;
            qty.Text = string.Empty;
            cost.Text = string.Empty;
        }

        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal totalPurchaseCost = 0;
                suppierCombobox.IsEnabled = true;
                Supplier selectedSupplier = null;

                // Calculate the total purchase cost and get the selected supplier
                foreach (var item in PurchaseItems)
                {
                    totalPurchaseCost += (decimal)item.Amount;

                    if (selectedSupplier == null)
                    {
                        Supplier supplier = db.Suppliers.FirstOrDefault(s => s.Name == item.SupplierName);
                        if (supplier != null)
                        {
                            selectedSupplier = supplier;
                        }
                    }
                }

                if (selectedSupplier == null)
                {
                    MessageBox.Show("Please select a supplier.");
                    return;
                }

                // Add the purchase record to the Purchase database table
                Purchase newPurchase = new Purchase
                {
                    Date = DateTime.Now,
                    TotalCost = totalPurchaseCost,
                    SupplierId = selectedSupplier.Id,
                    UserId = 1


                };
                db.Purchases.Add(newPurchase);

                db.SaveChanges();
                // Retrieve the generated PurchaseId
                int generatedPurchaseId = newPurchase.Id;

                // Add PurchaseDetails to the database for each item
                foreach (var purchaseItem in PurchaseItems)
                {
                    Product selectedProduct = db.Products.FirstOrDefault(product => product.Name == purchaseItem.ProductName);

                    if (selectedProduct == null)
                    {
                        MessageBox.Show($"Product '{purchaseItem.ProductName}' not found.");
                        return;
                    }
                    PurchaseDetail purchaseDetail = new PurchaseDetail
                    {
                        Quantity = purchaseItem.QTY,
                        Cost = (decimal)purchaseItem.Cost,
                        ProductId = selectedProduct.Id,
                        PurchaseId = generatedPurchaseId, // Use the generated PurchaseId
                        WarehouseId = 1

                    };

                    db.PurchaseDetails.Add(purchaseDetail);
                    db.SaveChanges();
                    int PdIds = purchaseDetail.Id;

                    // Update the warehouse or add a new entry
                    var warehouse = db.Warehouses.FirstOrDefault(w => w.ProductId == selectedProduct.Id);

                    if (warehouse != null)
                    {
                        warehouse.Quantity += purchaseItem.QTY;
                    }
                    else
                    {
                        warehouse = new Warehouse
                        {
                            ProductId = selectedProduct.Id,
                            Quantity = purchaseItem.QTY,
                            PdId = PdIds
                        };
                    }
                    db.Warehouses.Add(warehouse);

                }

                // Save changes to the database
                db.SaveChanges();
                PurchaseItems.Clear();
                datagridviews.Items.Refresh();
                Total_Purchase.Text = "0";
                productCombobox.SelectedIndex = -1;
                suppierCombobox.SelectedIndex = -1;
                qty.Text = string.Empty;
                cost.Text = string.Empty;
                countData = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.ToString()}");
            }
        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (PurchaseItems.Count == 0)
            {
                MessageBox.Show("No items to print.");
                return;
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = "Invoice " + purchaseId;
            saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf|Excel Files (*.xlsx)|*.xlsx";
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                string fileExtension = System.IO.Path.GetExtension(filePath).ToLower();

                if (fileExtension == ".pdf")
                {
                    GeneratePDF(filePath);
                }
                else if (fileExtension == ".xlsx")
                {
                    GenerateExcel(filePath);
                }

                MessageBox.Show("Invoice saved successfully.");
            }

        }
        private void GeneratePDF(string filePath)
        {
            Document doc = new Document();
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();
            string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "Logo.jpg");
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.ScalePercent(50f);
            logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
            doc.Add(logo);
            // Add title "INVOICE"
            Paragraph title = new Paragraph("INVOICE", new Font(Font.FontFamily.HELVETICA, 20, Font.BOLD));
            title.Alignment = Element.ALIGN_CENTER;
            doc.Add(title);

            // Add Date
            Paragraph date = new Paragraph($"Date: {DateTime.Now.ToShortDateString()} \n");
            doc.Add(date);

            // Add Invoice #
            Paragraph invoiceNumber = new Paragraph($"Invoice #: {purchaseId}\n");
            doc.Add(invoiceNumber);

            Paragraph supplierNameParagraph = new Paragraph($"Supplier Name: {suppierCombobox.SelectedItem.ToString()}\n");
            doc.Add(supplierNameParagraph);
            Paragraph paragraph = new Paragraph("\n\n\n");
            doc.Add(paragraph);
            // Create table with custom headers and specified column widths
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;

            // Set column widths
            float[] columnWidths = { 30f, 170f, 50f, 60f, 60f };
            table.SetWidths(columnWidths);

            // Set table border to 0 to remove the table border
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            float cellMinHeight = 50f; // Adjust this value as needed
            table.DefaultCell.MinimumHeight = cellMinHeight;
            // Add table headers
            table.AddCell(CreateCenteredCell("Id"));
            table.AddCell(CreateCenteredCell("Product"));
            table.AddCell(CreateCenteredCell("Quantity"));
            table.AddCell(CreateCenteredCell("Unit Price"));
            table.AddCell(CreateCenteredCell("Amount"));

            // Add data rows from PurchaseItems
            foreach (var item in PurchaseItems)
            {
                table.AddCell(CreateCenteredCell(item.Id.ToString()));
                table.AddCell(CreateCenteredCell(item.ProductName));
                table.AddCell(CreateCenteredCell(item.QTY.ToString()));
                table.AddCell(CreateCenteredCell(item.Cost.ToString()));
                table.AddCell(CreateCenteredCell(item.Amount.ToString()));
            }

            doc.Add(table);

            Paragraph totalPurchase = new Paragraph($"Total :$ {Total_Purchase.Text}");
            totalPurchase.Alignment = Element.ALIGN_RIGHT;
            doc.Add(totalPurchase);
            doc.Close();
        }
        private PdfPCell CreateCenteredCell(string content)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content, new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            // cell.Border = Rectangle.NO_BORDER;
            return cell;
        }
        private void GenerateExcel(string filePath)
        {
            Workbook workbook = new Workbook();
            Worksheet worksheet = workbook.Worksheets[0];

            // Add data rows from PurchaseItems
            int row = 1;

            // Add table headers
            worksheet.Range[row, 1].Text = "Id";
            worksheet.Range[row, 2].Text = "Product";
            worksheet.Range[row, 3].Text = "Quantity";
            worksheet.Range[row, 4].Text = "Unit Price";
            worksheet.Range[row, 5].Text = "Amount";

            row++;

            foreach (var item in PurchaseItems)
            {
                worksheet.Range[row, 1].NumberValue = item.Id;
                worksheet.Range[row, 2].Text = item.ProductName;
                worksheet.Range[row, 3].NumberValue = item.QTY;
                worksheet.Range[row, 4].NumberValue = item.Cost;
                worksheet.Range[row, 5].NumberValue = item.Amount;

                row++;
            }

            workbook.SaveToFile(filePath);
        }

        private void qty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only numeric input for qty field
            e.Handled = !IsNumericInput(e.Text);
        }

        private void cost_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Allow only decimal input for cost field
            e.Handled = !IsDecimalInput(e.Text);
        }

        private bool IsNumericInput(string text)
        {
            // Use a regular expression to check if the input is numeric
            return System.Text.RegularExpressions.Regex.IsMatch(text, "^[0-9]+$");
        }

        private bool IsDecimalInput(string text)
        {
            // Use a regular expression to check if the input is a decimal number
            return System.Text.RegularExpressions.Regex.IsMatch(text, "^[0-9]*\\.?[0-9]*$");
        }

        private void suppierCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!supplierSelected)
            {
                supplierSelected = true;
                suppierCombobox.IsEnabled = false; // Disable the supplier ComboBox
            }
        }
        private void LoadWarehouseData()
        {
            try
            {
                var query = from product in db.Products
                            join warehouse in db.Warehouses on product.Id equals warehouse.ProductId
                            select new
                            {
                                product.Id,
                                product.Name,
                                warehouse.Quantity
                            };

                WarehouseDataGrid.ItemsSource = query.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        private void LoadDistributeData()
        {

            var query = from Distribution in db.Distributions
                        join product in db.Products on Distribution.ProductId equals product.Id
                        join branch in db.Branches on Distribution.BranchId equals branch.Id
                        select new
                        {
                            Distribute_ID = Distribution.Id,
                            product.Name,
                            Distribution.Quantity,
                            branch.Address,
                            Distribution.Date
                        };

            Disributiondatagrid.ItemsSource = query.ToList();

        }

        private void LoadSupplierData()
        {
            var supplier = db.Suppliers.ToList();
            SupplierDataGrid.ItemsSource = supplier;
        }

        private void CallLoadFunction()
        {
            LoadDistributeData();
            LoadWarehouseData();
            LoadSupplierData();
            LoadProductDataToCB();
            LoadBranchDataToCB();
        }

        private void AddSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (NameTB.Text.Length > 0 && PhonenumberTB.Text.Length > 0 && AddressTB.Text.Length > 0 && EmailTB.Text.Length > 0)
            {
                using (IBC_STOREIIEntities IBC_Store = new IBC_STOREIIEntities())
                {
                    //Create a Supplier Info
                    Supplier supplier = new Supplier()
                    {
                        Name = NameTB.Text,
                        PhoneNumber = PhonenumberTB.Text,
                        Address = AddressTB.Text,
                        Email = EmailTB.Text,
                    };

                    IBC_Store.Suppliers.Add(supplier);
                    IBC_Store.SaveChanges();
                    LoadSupplierData();
                }
                NameTB.Text = string.Empty;
                PhonenumberTB.Text = string.Empty;
                AddressTB.Text = string.Empty;
                EmailTB.Text = string.Empty;

            }
            else
            {
                MessageBox.Show("Please Input the Suppliers Information.");
                return;
            }
        }

        private void EditSupplier_Click(object sender, RoutedEventArgs e)
        {
            using (IBC_STOREIIEntities IBC_Store = new IBC_STOREIIEntities())
            {
                Supplier selectedSupplier = SupplierDataGrid.SelectedItem as Supplier;

                if (selectedSupplier == null)
                {
                    MessageBox.Show("Please select a Supplier to edit.");
                    return;
                }

                selectedSupplier.Name = NameTB.Text;
                selectedSupplier.Address = AddressTB.Text;
                selectedSupplier.PhoneNumber = PhonenumberTB.Text;
                selectedSupplier.Email = EmailTB.Text;

                // Save changes to the database
                db.SaveChanges();
                MessageBox.Show("Supplier Successfully Modified!!");

                // Clear the input fields

                NameTB.Text = string.Empty;
                AddressTB.Text = string.Empty;
                PhonenumberTB.Text = string.Empty;
                EmailTB.Text = string.Empty;

                // Refresh the data in the modify_branch_datagrid
                LoadSupplierData();
            }
        }

        private void SupplierDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Supplier selectedSupplier = SupplierDataGrid.SelectedItem as Supplier;

            if (selectedSupplier != null)
            {
                NameTB.Text = selectedSupplier.Name;
                PhonenumberTB.Text = selectedSupplier.PhoneNumber;
                AddressTB.Text = selectedSupplier.Address;
                EmailTB.Text = selectedSupplier.Email;

            }
            LoadSupplierData();
        }

        private void AddDistribution_Click(object sender, RoutedEventArgs e)
        {
            if (ProductCB.Items.Count > 0 && QuantityTB.Text.Length > 0 && BranchCB.Items.Count > 0)
            {
                using (IBC_STOREIIEntities IBC_Store = new IBC_STOREIIEntities  ())
                {

                    //Create a Distribute Info
                    Product product = IBC_Store.Products.FirstOrDefault(p => p.Name == ProductCB.SelectedItem.ToString());
                    Branch branch = IBC_Store.Branches.FirstOrDefault(b => b.Address == BranchCB.SelectedItem.ToString());
                    Warehouse warehouse = IBC_Store.Warehouses.FirstOrDefault(w => w.Id == 1);
                    Stock stock = IBC_Store.Stocks.FirstOrDefault(s => s.BranchId == branch.Id && s.ProductId == product.Id);

                    Distribution distribution = new Distribution()
                    {
                        ProductId = product.Id,
                        BranchId = branch.Id,
                        Quantity = int.Parse(QuantityTB.Text),
                        Date = DateTime.Parse(DateTime.Now.ToShortDateString()),
                        WarehouseId = warehouse.Id
                    };
                    if (stock != null)
                    {
                        stock.Quantity += int.Parse(QuantityTB.Text);
                    }
                    else
                    {
                        stock = new Stock()
                        {
                            ProductId = product.Id,
                            BranchId = branch.Id,
                            Quantity = int.Parse(QuantityTB.Text),
                        };
                        IBC_Store.Stocks.Add(stock);

                    }

                    IBC_Store.Database.ExecuteSqlCommand($"UPDATE Warehouses SET Warehouses.Quantity = Warehouses.Quantity - {distribution.Quantity} WHERE (Warehouses.ProductId = {product.Id})");
                    IBC_Store.Distributions.Add(distribution);
                    IBC_Store.SaveChanges();
                    LoadDistributeData();

                }
                ProductCB.Text = string.Empty;
                BranchCB.Text = string.Empty;
                QuantityTB.Text = string.Empty;

            }
            else
            {
                MessageBox.Show("Please Input the Distribute Information.");
                return;
            }
        }

        private void LoadBranchDataToCB()
        {
            using (IBC_STOREIIEntities IBC_Store = new IBC_STOREIIEntities())
            {
                sqlConnection = new SqlConnection(IBC_Store.Database.Connection.ConnectionString);
                sqlConnection.Open();
                SqlCommand command = new SqlCommand($"SELECT ADDRESS FROM Branches", sqlConnection);
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        BranchCB.Items.Add(reader.GetString(0));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                reader.Close();
                sqlConnection.Close();
            }
        }
        private void LoadProductDataToCB()
        {
            using (IBC_STOREIIEntities IBC_Store = new IBC_STOREIIEntities())
            {
                var product = IBC_Store.Products.ToList();
                foreach (var item in product)
                {
                    ProductCB.Items.Add(item.Name);
                }
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            username_textblock.Text = "Username Login: " + UserName;
        }

        private void ChatButton_Click(object sender, RoutedEventArgs e)
        {
            Chat chat = new Chat(UserName);
            chat.Show();
        }
    }
}
