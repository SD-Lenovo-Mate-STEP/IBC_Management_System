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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Data.Entity;
using System.Net.Mail;
using System.Net.Sockets;
using System.Net;
using System.Configuration;

namespace IBC_Management_System
{
    public partial class Admin : Window
    {
    public string chatUsername;
        static AsyncServer chatSever = new AsyncServer("127.0.0.1", 1024);

        #region Panel Funtions
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            LoadRoles();
            LoadBranchData();
            LoadCategory();
            LoadCategoryComboBox();
            LoadProducts();
            LoadStocks();
            chatSever.StartServer();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            Admin admin = new Admin(chatUsername);
            admin.WindowState = WindowState.Minimized;
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void home_btn_Click(object sender, RoutedEventArgs e)
        {
            label_tb.Text = "Admin Panel";
            home_grid.Visibility = Visibility.Visible;
            branch_grid.Visibility = Visibility.Hidden;
            user_grid.Visibility = Visibility.Hidden;
            role_grid.Visibility = Visibility.Hidden;
            userControl_grid.Visibility = Visibility.Hidden;
            modify_branch_grid.Visibility = Visibility.Hidden;
            create_category_grid.Visibility = Visibility.Hidden;
            add_product_grid.Visibility = Visibility.Hidden;
            modify_product_grid.Visibility = Visibility.Hidden;
            search_grid.Visibility = Visibility.Hidden;
            stock_grid.Visibility = Visibility.Hidden;
        }

        private void createUser_btn_Click(object sender, RoutedEventArgs e)
        {
            label_tb.Text = "Create User";
            user_grid.Visibility = Visibility.Visible;
            role_grid.Visibility = Visibility.Hidden;
            userControl_grid.Visibility = Visibility.Hidden;
            home_grid.Visibility = Visibility.Hidden;
            modify_branch_grid.Visibility = Visibility.Hidden;
            create_category_grid.Visibility = Visibility.Hidden;
            branch_grid.Visibility = Visibility.Hidden;
            add_product_grid.Visibility = Visibility.Hidden;
            modify_product_grid.Visibility = Visibility.Hidden;
            search_grid.Visibility = Visibility.Hidden;
            stock_grid.Visibility = Visibility.Hidden;
        }

        private void createRole_btn_Click(object sender, RoutedEventArgs e)
        {
            label_tb.Text = "Create Role";
            user_grid.Visibility = Visibility.Hidden;
            role_grid.Visibility = Visibility.Visible;
            userControl_grid.Visibility = Visibility.Hidden;
            home_grid.Visibility = Visibility.Hidden;
            modify_branch_grid.Visibility = Visibility.Hidden;
            create_category_grid.Visibility = Visibility.Hidden;
            branch_grid.Visibility = Visibility.Hidden;
            add_product_grid.Visibility = Visibility.Hidden;
            modify_product_grid.Visibility = Visibility.Hidden;
            search_grid.Visibility = Visibility.Hidden;
            stock_grid.Visibility = Visibility.Hidden;
        }

        private void userControl_btn_Click(object sender, RoutedEventArgs e)
        {
            label_tb.Text = "Modify User";
            user_grid.Visibility = Visibility.Hidden;
            role_grid.Visibility = Visibility.Hidden;
            userControl_grid.Visibility = Visibility.Visible;
            home_grid.Visibility = Visibility.Hidden;
            modify_branch_grid.Visibility = Visibility.Hidden;
            create_category_grid.Visibility = Visibility.Hidden;
            branch_grid.Visibility = Visibility.Hidden;
            add_product_grid.Visibility = Visibility.Hidden;
            modify_product_grid.Visibility = Visibility.Hidden;
            search_grid.Visibility = Visibility.Hidden;
            stock_grid.Visibility = Visibility.Hidden;
        }

        private void createBranch_btn_Click(object sender, RoutedEventArgs e)
        {
            label_tb.Text = "Create Branch";
            branch_grid.Visibility = Visibility.Visible;
            user_grid.Visibility = Visibility.Hidden;
            role_grid.Visibility = Visibility.Hidden;
            userControl_grid.Visibility = Visibility.Hidden;
            home_grid.Visibility = Visibility.Hidden;
            modify_branch_grid.Visibility = Visibility.Hidden;
            create_category_grid.Visibility = Visibility.Hidden;
            add_product_grid.Visibility = Visibility.Hidden;
            modify_product_grid.Visibility = Visibility.Hidden;
            search_grid.Visibility = Visibility.Hidden;
            stock_grid.Visibility = Visibility.Hidden;
        }

        private void modifyBranch_btn_Click(object sender, RoutedEventArgs e)
        {
            label_tb.Text = "Modify Branch";
            branch_grid.Visibility = Visibility.Hidden;
            user_grid.Visibility = Visibility.Hidden;
            role_grid.Visibility = Visibility.Hidden;
            userControl_grid.Visibility = Visibility.Hidden;
            home_grid.Visibility = Visibility.Hidden;
            modify_branch_grid.Visibility = Visibility.Visible;
            create_category_grid.Visibility = Visibility.Hidden;
            add_product_grid.Visibility = Visibility.Hidden;
            modify_product_grid.Visibility = Visibility.Hidden;
            search_grid.Visibility = Visibility.Hidden;
            stock_grid.Visibility = Visibility.Hidden;

            LoadBranchData();
        }

        private void createCategory_Click(object sender, RoutedEventArgs e)
        {
            label_tb.Text = "Create Category";
            branch_grid.Visibility = Visibility.Hidden;
            user_grid.Visibility = Visibility.Hidden;
            role_grid.Visibility = Visibility.Hidden;
            userControl_grid.Visibility = Visibility.Hidden;
            home_grid.Visibility = Visibility.Hidden;
            modify_branch_grid.Visibility = Visibility.Hidden;
            create_category_grid.Visibility = Visibility.Visible;
            add_product_grid.Visibility = Visibility.Hidden;
            modify_product_grid.Visibility = Visibility.Hidden;
            search_grid.Visibility = Visibility.Hidden;
            stock_grid.Visibility = Visibility.Hidden;
        }

        private void createProduct_Click(object sender, RoutedEventArgs e)
        {
            label_tb.Text = "Create Product";
            branch_grid.Visibility = Visibility.Hidden;
            user_grid.Visibility = Visibility.Hidden;
            role_grid.Visibility = Visibility.Hidden;
            userControl_grid.Visibility = Visibility.Hidden;
            home_grid.Visibility = Visibility.Hidden;
            modify_branch_grid.Visibility = Visibility.Hidden;
            create_category_grid.Visibility = Visibility.Hidden;
            add_product_grid.Visibility = Visibility.Visible;
            modify_product_grid.Visibility = Visibility.Hidden;
            search_grid.Visibility = Visibility.Hidden;
            stock_grid.Visibility = Visibility.Hidden;
        }

        private void modifyProduct_Click(object sender, RoutedEventArgs e)
        {
            label_tb.Text = "Modify Product";
            branch_grid.Visibility = Visibility.Hidden;
            user_grid.Visibility = Visibility.Hidden;
            role_grid.Visibility = Visibility.Hidden;
            userControl_grid.Visibility = Visibility.Hidden;
            home_grid.Visibility = Visibility.Hidden;
            modify_branch_grid.Visibility = Visibility.Hidden;
            create_category_grid.Visibility = Visibility.Hidden;
            add_product_grid.Visibility = Visibility.Hidden;
            modify_product_grid.Visibility = Visibility.Visible;
            search_grid.Visibility = Visibility.Hidden;
            stock_grid.Visibility = Visibility.Hidden;
        }

        private void search_btn_Click(object sender, RoutedEventArgs e)
        {
            Search searchPanel = new Search();
            searchPanel.Show();
        }

        private void chat_Click(object sender, RoutedEventArgs e)
        {
            home_grid.Visibility = Visibility.Hidden;
            branch_grid.Visibility = Visibility.Hidden;
            user_grid.Visibility = Visibility.Hidden;
            role_grid.Visibility = Visibility.Hidden;
            userControl_grid.Visibility = Visibility.Hidden;
            modify_branch_grid.Visibility = Visibility.Hidden;
            create_category_grid.Visibility = Visibility.Hidden;
            add_product_grid.Visibility = Visibility.Hidden;
            modify_product_grid.Visibility = Visibility.Hidden;
            stock_grid.Visibility = Visibility.Hidden;
            //search_grid.Visibility = Visibility.Visible;
            Chat chat = new Chat(chatUsername);
            chat.Show();
            this.Hide();
        }

        private void productStockLevel_Click(object sender, RoutedEventArgs e)
        {
            label_tb.Text = "Stock Panel";
            home_grid.Visibility = Visibility.Hidden;
            branch_grid.Visibility = Visibility.Hidden;
            user_grid.Visibility = Visibility.Hidden;
            role_grid.Visibility = Visibility.Hidden;
            userControl_grid.Visibility = Visibility.Hidden;
            modify_branch_grid.Visibility = Visibility.Hidden;
            create_category_grid.Visibility = Visibility.Hidden;
            add_product_grid.Visibility = Visibility.Hidden;
            modify_product_grid.Visibility = Visibility.Hidden;
            stock_grid.Visibility = Visibility.Visible;
        }
        #endregion

        #region Sreyoun's Code
        IBC_STOREIIEntities db = new IBC_STOREIIEntities();

        private ObservableCollection<User> searchResult = new ObservableCollection<User>();

        public Admin()
        {
            InitializeComponent();
            DataContext = this;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        }
        public Admin(string username)
        {
            chatUsername = username;
            InitializeComponent();
            DataContext = this;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        }

        private void ClearEditInput()
        {
            edit_username_tb.Text = "";
            edit_password_tb.Text = "";
            edit_number_tb.Text = "";
            edit_email_tb.Text = "";
            edit_firstname_tb.Text = "";
            edit_lastname_tb.Text = "";
            edit_address_tb.Text = "";

            // Clear and reset the ComboBox
            comboBox_add_role.SelectedItem = null;
            comboBox_select_role.SelectedItem = null;
        }

        private void goBack_btn_Click(object sender, RoutedEventArgs e)
        {
                Window1 login = new Window1();
                login.Show();
                this.Close();
        }

        public void LoadData()
        {
            try
            {
                var _user = db.Users.ToList();
                datagrid_user.ItemsSource = _user;

                datagrid_user.SelectedItems.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        private bool IsValidPhoneNumber(string number)
        {
            string pattern = @"^\d{9,}$";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(number);
            return match.Success;
        }
        private void AddUserbtn_Click(object sender, RoutedEventArgs e)
        {
            string userName = add_username_tb.Text.Trim();
            string password = add_password_tb.Text.Trim();
            string firstName = add_firstname_tb.Text.Trim();
            string lastName = add_lastname_tb.Text.Trim();
            string number = add_number_tb.Text.Trim();
            string email = add_email_tb.Text.Trim();
            string address = add_address_tb.Text.Trim();
            int role = comboBox_add_role.SelectedIndex + 1;


            if (role == 0 || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(number) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Please fill out all required fields.");
                return;
            }
            if (db.Users.Any(u => u.Username == userName))
            {
                MessageBox.Show("Username already exists. Please choose a different username.");
                return;
            }
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.");
                return;
            }

            string Validnumber = number.Trim();
            if (!IsValidPhoneNumber(number))
            {
                MessageBox.Show("Please enter a valid 11-digit phone number.");
                return;
            }

            User newUser = new User
            {
                Username = userName,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = number,
                Address = address,
                RoleId = role,
            };

            db.Users.Add(newUser);
            db.SaveChanges();

            comboBox_add_role.SelectedItem = null;
            MessageBox.Show("User added successfully ");

            add_username_tb.Clear();
            add_password_tb.Clear();
            add_firstname_tb.Clear();
            add_lastname_tb.Clear();
            add_number_tb.Clear();
            add_email_tb.Clear();
            add_address_tb.Clear();
            comboBox_add_role.SelectedIndex = -1;

            LoadData();
        }

        public void LoadRoles()
        {
            var roles = db.Roles.ToList();

            comboBox_select_role.Items.Clear();
            comboBox_add_role.Items.Clear();

            foreach (var items in roles)
            {
                comboBox_add_role.Items.Add(items.Description);
                comboBox_select_role.Items.Add(items.Description);
            }
        }

        private void Save_edit_Click(object sender, RoutedEventArgs e)
        {

            User selectedUser = datagrid_user.SelectedItem as User;
            if (selectedUser == null)
            {
                MessageBox.Show("Select User First Before edit!!!");
                return;
            }
            selectedUser.Username = edit_username_tb.Text;
            selectedUser.Password = edit_password_tb.Text;
            selectedUser.FirstName = edit_firstname_tb.Text;
            selectedUser.LastName = edit_lastname_tb.Text;
            selectedUser.PhoneNumber = edit_number_tb.Text;
            selectedUser.Email = edit_email_tb.Text;
            selectedUser.Address = edit_address_tb.Text;
            selectedUser.RoleId = comboBox_select_role.SelectedIndex + 1;

            db.SaveChanges();
            MessageBox.Show("Change Has been made to User!!!");

            LoadData();

        }

        private void DeleteUser_btn_Click(object sender, RoutedEventArgs e)
        {
            User selectedUser = datagrid_user.SelectedItem as User;

            if (selectedUser == null)
            {
                MessageBox.Show("Please select a user to delete.");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this user?", "Delete User", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
                {
                    var userToDelete = db.Users.FirstOrDefault(u => u.Id == selectedUser.Id);
                    if (userToDelete != null)
                    {
                        db.Users.Remove(userToDelete);
                        db.SaveChanges();
                        ClearEditInput();

                        MessageBox.Show("User Deleted Successfully");
                        LoadData();

                    }
                    else
                    {
                        MessageBox.Show("User not found!");
                    }
                }
            }
        }

        private void DownloadUser_btn_Click(object sender, RoutedEventArgs e)
        {
            User selectedUser = datagrid_user.SelectedItem as User;

            if (selectedUser != null)
            {

                string tempFileName = System.IO.Path.GetTempFileName() + ".pdf";
                GeneratePdf(selectedUser, tempFileName);


                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.FileName = "UserInfo.pdf";
                saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.Copy(tempFileName, saveFileDialog.FileName, true);
                    MessageBox.Show("PDF downloaded successfully.");
                }
            }
            else
            {
                MessageBox.Show("Please select a user to download their information.");
            }
        }

        private void GeneratePdf(User user, string fileName)
        {
            Document doc = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(fileName, FileMode.Create));

            string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "logo.png");
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.ScalePercent(50f);
            logo.SetAbsolutePosition(doc.Left, doc.Top - logo.ScaledHeight);

            doc.Open();


            doc.Add(logo);


            doc.Add(new Paragraph("\n\n\n\n\n\n"));

            doc.Add(new Paragraph($"\t\t\tInformation for {user.FirstName} {user.LastName}:"));
            doc.Add(new Paragraph($"\t\t\tUsername: {user.Username}"));
            doc.Add(new Paragraph($"\t\t\tPassword: {user.Password}"));
            doc.Add(new Paragraph($"\t\t\tRole: {user.Role.Description}"));
            doc.Add(new Paragraph($"\t\t\tEmail: {user.Email}"));
            doc.Add(new Paragraph($"\t\t\tPhone Number: {user.PhoneNumber}"));
            doc.Add(new Paragraph($"\t\t\tAddress:  {user.Address}"));

            doc.Close();
        }

        private void user_search_click(object sender, RoutedEventArgs e)
        {
            string searchUsername = searchUser_tb.Text.Trim();
            if (!string.IsNullOrEmpty(searchUsername))
            {
                var searchResult = db.Users.Where(user => user.Username.Contains(searchUsername)).ToList();
                datagrid_user.ItemsSource = searchResult;

            }
            else
            {
                LoadData();
            }
        }

        private void searchUser_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchUsername = searchUser_tb.Text.Trim();

            if (string.IsNullOrEmpty(searchUsername))
            {
                LoadData();
            }
            else
            {
                var searchResult = db.Users
                    .Where(user => user.Username.Contains(searchUsername) ||
                                   user.FirstName.Contains(searchUsername) ||
                                   user.LastName.Contains(searchUsername))
                    .ToList();

                if (searchResult.Count == 0)
                {
                    MessageBox.Show("User not found.");
                }
                datagrid_user.ItemsSource = searchResult;
            }
        }

        private void Datagrid_user_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            User selectedUser = datagrid_user.SelectedItem as User;

            if (selectedUser != null)
            {
                edit_username_tb.Text = selectedUser.Username;
                edit_password_tb.Text = selectedUser.Password;
                edit_firstname_tb.Text = selectedUser.FirstName;
                edit_lastname_tb.Text = selectedUser.LastName;
                edit_number_tb.Text = selectedUser.PhoneNumber;
                edit_email_tb.Text = selectedUser.Email;
                edit_address_tb.Text = selectedUser.Address;
                comboBox_select_role.SelectedIndex = selectedUser.RoleId - 1;
            }
        }

        public void LoadBranchData()
        {
            var branches = db.Branches.ToList();
            branch_datagrid.ItemsSource = branches;
            modify_branch_datagrid.ItemsSource = branches;

        }

        private void Edit_branch_bt_Click(object sender, RoutedEventArgs e)
        {
            Branch selectedBranch = modify_branch_datagrid.SelectedItem as Branch;

            if (selectedBranch == null)
            {
                MessageBox.Show("Please select a branch to edit.");
                return;
            }


            selectedBranch.Address = edit_branch_address_tb.Text;
            selectedBranch.PhoneNumber = edit_branch_number_tb.Text;

            // Save changes to the database
            db.SaveChanges();
            ClearEditInput();
            MessageBox.Show("Branch Successfully Modified!!");

            // Clear the input fields
            edit_branch_address_tb.Text = string.Empty;
            edit_branch_number_tb.Text = string.Empty;

            // Refresh the data in the modify_branch_datagrid
            LoadBranchData();
        }

        private void Modify_branch_datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Branch selectedBranch = modify_branch_datagrid.SelectedItem as Branch;

            if (selectedBranch != null)
            {
                edit_branch_address_tb.Text = selectedBranch.Address;
                edit_branch_number_tb.Text = selectedBranch.PhoneNumber;

            }
        }

        private void Delete_branch_bt_Click(object sender, RoutedEventArgs e)
        {
            Branch selectedBranch = modify_branch_datagrid.SelectedItem as Branch;

            if (selectedBranch == null)
            {
                MessageBox.Show("Please select a branch to delete.");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this branch?", "Delete Branch", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
                {
                    var branchToDelete = db.Branches.FirstOrDefault(b => b.Id == selectedBranch.Id);
                    if (branchToDelete != null)
                    {
                        db.Branches.Remove(branchToDelete);
                        db.SaveChanges();
                        MessageBox.Show("Branch Deleted Successfully");
                        ClearEditInput();
                        LoadBranchData();

                        edit_branch_address_tb.Text = "";
                        edit_branch_number_tb.Text = "";

                    }
                    else
                    {
                        MessageBox.Show("Branch not found!");
                    }
                }
            }
        }

        private void Add_branch_bt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string address = branch_address_tb.Text;
                string phoneNumber = branch_number_tb.Text;

                if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(phoneNumber))
                {
                    MessageBox.Show("Please enter both address and phone number.");
                    return;
                }
                if (db.Branches.Any(b => b.Address == address || b.PhoneNumber == phoneNumber))
                {
                    MessageBox.Show("A branch with the same address or phone number already exists. Please enter different details.");
                    return;
                }

                Branch newBranch = new Branch
                {
                    Address = address,
                    PhoneNumber = phoneNumber
                };

                db.Branches.Add(newBranch);
                db.SaveChanges();

                MessageBox.Show("Branch added successfully.");

                // Clear the input fields
                branch_address_tb.Text = string.Empty;
                branch_number_tb.Text = string.Empty;

                LoadBranchData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding branch: {ex.Message}");
            }
        }

        private void AddRole_btn_Click(object sender, RoutedEventArgs e)
        {
            string roleDescription = descirptionTextBox.Text;
            bool isReporter = report_cb.IsChecked ?? false;
            bool isSeller = seller_cb.IsChecked ?? false;
            bool isPurchase = purchase_cb.IsChecked ?? false;
            bool isCrud = crud_cb.IsChecked ?? false;

            if (string.IsNullOrEmpty(roleDescription))
            {
                MessageBox.Show("Please enter a role description.");
                return;
            }
            if (!isReporter && !isSeller && !isPurchase && !isCrud)
            {
                MessageBox.Show("Please select at least one permission.");
                return;
            }

            Role newRole = new Role
            {
                Description = roleDescription,
                Reporter = isReporter,
                Seller = isSeller,
                Purchase = isPurchase,
                Crud = isCrud,
            };


            db.Roles.Add(newRole);
            db.SaveChanges();

            LoadRoles();

            MessageBox.Show("New Role Added Successfully");

            // Clear input fields
            descirptionTextBox.Text = "";
            report_cb.IsChecked = false;
            seller_cb.IsChecked = false;
            purchase_cb.IsChecked = false;
            crud_cb.IsChecked = false;
        }

        private void Add_category_bt_Click(object sender, RoutedEventArgs e)
        {
            string category = category_name_tb.Text.Trim();

            if (string.IsNullOrWhiteSpace(category))
            {
                MessageBox.Show("Category name cannot be empty.");
                return;
            }

            if (db.Categories.Any(c => c.Name == category))
            {
                MessageBox.Show("Category with the same name already exists. Please enter a different category name.");
                return;
            }

            Category newCategory = new Category
            {
                Name = category,
            };

            db.Categories.Add(newCategory);
            db.SaveChanges();

            LoadCategory();

            LoadCategoryComboBox();

            ClearEditInput();

            MessageBox.Show("New Category Added Successfully");

            category_name_tb.Text = string.Empty;


        }

        public void LoadCategory()
        {
            var cat = db.Categories.ToList();
            category_datagrid.ItemsSource = cat;
        }

        private void Edit_category_bt_Click(object sender, RoutedEventArgs e)
        {
            Category selectCategory = category_datagrid.SelectedItem as Category;

            if (selectCategory == null)
            {
                MessageBox.Show("Select catagory First before editing!!!");
            }

            selectCategory.Name = category_name_tb.Text;
            db.SaveChanges();
            ClearEditInput();

            LoadCategory();
            LoadCategoryComboBox();
        }

        private void Category_datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Category selectedCategory = category_datagrid.SelectedItem as Category;
            if (selectedCategory != null)
            {
                category_name_tb.Text = selectedCategory.Name;
            }
        }

        private void Delete_category_bt_Click(object sender, RoutedEventArgs e)
        {

            Category selectCatagory = category_datagrid.SelectedItem as Category;
            if (selectCatagory == null)
            {
                MessageBox.Show("Please select Catagory Before Editing!!");
                return;
            }
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this category?", "Delete Category", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                using (IBC_STOREIIEntities db = new IBC_STOREIIEntities())
                {
                    var categoryToDelete = db.Categories.FirstOrDefault(b => b.Id == selectCatagory.Id);
                    if (categoryToDelete != null)
                    {
                        db.Categories.Remove(categoryToDelete);
                        db.SaveChanges();
                        MessageBox.Show("Category Deleted Successfully");
                        ClearEditInput();
                        LoadCategory();

                        LoadCategoryComboBox();
                        category_name_tb.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Category not found!");
                    }
                }
            }
        }

        private bool exporting = false;
        private void search_all_bt_Click(object sender, RoutedEventArgs e)
        {
            exporting = true;
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

        private Dictionary<int, string> userPasswords = new Dictionary<int, string>();

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

        private void Print_search_bt_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();

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
                            Paragraph title = new Paragraph("Data Information", titleFont);
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

        #endregion

        #region Rin Section

        private void upload_pic_Click(object sender, RoutedEventArgs e)
        {
            BrowseImage();
        }

        public void BrowseImage()
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.png";
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == true)
            {
                product_img.Tag = File.ReadAllBytes(openFileDialog.FileName);
                product_img.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void save_product_Click(object sender, RoutedEventArgs e)
        {
            byte[] imageBytes = product_img.Tag as byte[];

            if (string.IsNullOrWhiteSpace(prod_name_tb.Text) ||
                string.IsNullOrWhiteSpace(prod_price_tb.Text) ||
                prod_category_cb.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all the required fields.");
                return;
            }

            if (imageBytes == null)
            {
                MessageBox.Show("Please upload image.");
                return;
            }

            //Need to compare the cost to price first

            decimal price = decimal.Parse(prod_price_tb.Text);
            string name = prod_name_tb.Text;
            string barcode = prod_barcode_tb.Text;
            int categoryId = prod_category_cb.SelectedIndex + 1;

            Product newProduct = new Product
            {
                Name = name,
                Price = price,
                BarCode = barcode,
                Picture = imageBytes,
                CategoryId = categoryId
            };
            db.Products.Add(newProduct);
            db.SaveChanges();

            LoadProducts();
            LoadCategoryComboBox();

            MessageBox.Show("Product saved successfully.");

            prod_price_tb.Text = null;
            prod_name_tb.Text = null;
            prod_category_cb.SelectedIndex = -1;
            product_img.Source = null;
            prod_barcode_tb.Text = null;

        }

        private void product_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Product selectedProd = product_list.SelectedItem as Product;

            if (selectedProd != null)
            {
                edit_prodName_tb.Text = selectedProd.Name;
                edit_prodPrice_tb.Text = selectedProd.Price.ToString();
                edit_prodCategory_cb.SelectedIndex = selectedProd.CategoryId - 1;
                edit_prodBarcode_tb.Text = selectedProd.BarCode;

                // Display the product image
                if (selectedProd.Picture != null)
                {
                    BitmapImage productBitmap = ConvertBytesToBitmapImage(selectedProd.Picture);
                    product_img_old.Source = productBitmap;
                }
                else
                    product_img_old.Source = null;
            }
        }

        private BitmapImage ConvertBytesToBitmapImage(byte[] imageData)
        {
            if (imageData == null)
                return null;

            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream memoryStream = new MemoryStream(imageData))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        private bool isImageModified = false;

        private void edit_product_Click(object sender, RoutedEventArgs e)
        {
            byte[] imageBytes = isImageModified ? product_img_old.Tag as byte[] : null;

            Product selectedProduct = product_list.SelectedItem as Product;

            if (selectedProduct == null)
            {
                MessageBox.Show("Select Product First Before edit!!!");
                return;
            }

            selectedProduct.Price = decimal.Parse(edit_prodPrice_tb.Text);
            selectedProduct.Name = edit_prodName_tb.Text;
            selectedProduct.CategoryId = edit_prodCategory_cb.SelectedIndex + 1;
            selectedProduct.BarCode = edit_prodBarcode_tb.Text;
            selectedProduct.Picture = isImageModified ? imageBytes : selectedProduct.Picture;

            db.SaveChanges();

            MessageBox.Show("Change Has been Made!!!");

            LoadProducts();

            edit_prodPrice_tb.Text = null;
            edit_prodName_tb.Text = null;
            edit_prodCategory_cb.SelectedIndex = -1;
            product_img_old.Source = null;
            edit_prodBarcode_tb.Text = null;

            isImageModified = false;
        }

        private void delete_product_Click(object sender, RoutedEventArgs e)
        {
            Product selectProd = product_list.SelectedItem as Product;
            if (selectProd == null)
            {
                MessageBox.Show("Please select Catagory Before Editing!!");
                return;
            }
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this Product?", "Delete Product", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var prodToDelete = db.Products.FirstOrDefault(b => b.Id == selectProd.Id);
                if (prodToDelete != null)
                {
                    db.Products.Remove(prodToDelete);
                    db.SaveChanges();
                    MessageBox.Show("Product Deleted Successfully");

                    edit_prodPrice_tb.Text = null;
                    edit_prodName_tb.Text = null;
                    edit_prodCategory_cb.SelectedIndex = -1;
                    product_img_old.Source = null;
                    edit_prodBarcode_tb.Text = null;

                    LoadProducts();
                }
                else
                {
                    MessageBox.Show("Product not found!");
                }
            }
        }

        public void BrowseNewImage()
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.png";
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == true)
            {
                product_img_old.Tag = File.ReadAllBytes(openFileDialog.FileName);
                product_img_old.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                isImageModified = true;
            }
        }

        private void change_prod_pic_Click(object sender, RoutedEventArgs e)
        {
            BrowseNewImage();

        }

        private void LoadProducts()
        {
            var products = db.Products.ToList();
            product_list.ItemsSource = products;
        }

        private void LoadStocks()
        {
            var stocks = db.Stocks.ToList();
            list_stock.ItemsSource = stocks;
        }

        private void LoadCategoryComboBox()
        {
            var cat = db.Categories.ToList();
            prod_category_cb.Items.Clear();
            edit_prodCategory_cb.Items.Clear();
            foreach (var item in cat)
            {
                prod_category_cb.Items.Add(item.Name);
                edit_prodCategory_cb.Items.Add(item.Name);
            }

        }

        private void save_level_stock_Click(object sender, RoutedEventArgs e)
        {
            Stock selectedStock = list_stock.SelectedItem as Stock; 
            if (selectedStock == null)
            {
                MessageBox.Show("Select Stock First Before Set Level!!!");
                return;
            }

            selectedStock.Level = Int32.Parse(stock_level.Text);
            db.SaveChanges();

            MessageBox.Show("Change Has been Made!!!");

            LoadStocks();

            stock_level.Text = null;
        }

        internal void MakeAnnouncement(string text1, string text2, string text3)
        {
            chatSever.MakeAnnouncement(text1, text2, text3);
        }


        #endregion

        #region Chat Server
        public class LibraryContext : DbContext
        {
            public LibraryContext() : base(ConfigurationManager.ConnectionStrings["IBC_STOREIIEntities"].ConnectionString) //Connection string to database
            {

            }
            public DbSet<Stock> Stocks { get; set; }
            public DbSet<Invoice> Invoices { get; set; }
            public DbSet<Announcement> Announcements { get; set; }
            public DbSet<User> Users { get; set; }

            public static string GetConnectionString()
            {
                if (File.Exists("config.txt"))
                {
                    string[] lines = File.ReadAllLines("config.txt");
                    //remove all spaces from the connection string
                    string connectionString = lines[0];
                    for (int i = 99; i < connectionString.Length; i++)
                    {
                        if (connectionString[i] == ' ')
                        {
                            connectionString = connectionString.Remove(i, 1);
                        }

                    }
                    if (connectionString.Contains("Default"))
                    {
                        MessageBox.Show("Please open current path and put your own connection string in the file name \"config.txt\"");
                        Environment.Exit(0);
                        return null;
                    }
                    else
                    {
                        return connectionString;

                    }

                }
                else
                {
                    File.Create("config.txt").Close();
                    MessageBox.Show("Please open current path and put your own connection string in the file name \"config.txt\"");
                    return null;
                }
            }
            public void StartServer()
            {

            }

        }
        class AsyncServer
        {
            public static List<Socket> listofClients = new List<Socket>();
            public static List<string> userList = new List<string>();

            string[] quotes = new string[11];
            IPEndPoint endP;
            Socket socket;
            bool broke = false;
            public List<string> GetUsers()
            {
                return userList;
            }
            public AsyncServer(string strAddr, int port)
            {
                endP = new IPEndPoint(IPAddress.Parse(strAddr), port);
            }
            public void StartServer()
            {
                Console.WriteLine("\nWaitting for client....");

                if (socket != null) return;
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                socket.Bind(endP);
                socket.Listen(10);
                socket.BeginAccept(new AsyncCallback(MyAcceptCallbackFunction), socket);

            }
            private void MyAcceptCallbackFunction(IAsyncResult ia)
            {
                broke = false;
                //get a link to the listening socket
                Socket socket = (Socket)ia.AsyncState;
                //get a socket to exchange data with the client
                Socket ns = socket.EndAccept(ia);
                //output the connection information to the console
                string now = DateTime.Now.ToString();
                bool justConnected = true;
                string NameOfUser = "";
                if (justConnected)
                {


                    listofClients.Add(ns);
                    byte[] sendAnswer1 = new byte[1024];
                    int bytesReceived1 = ns.Receive(sendAnswer1);
                    string username = System.Text.Encoding.ASCII.GetString(sendAnswer1, 0, bytesReceived1);
                    if (!userList.Contains(username))
                    {
                        userList.Add(username);
                    }
                    Console.WriteLine(username + "Connected");
                    foreach (var client in listofClients)
                    {
                        //send sendAnswer1 to all clients
                        foreach (var item in userList)
                        {
                            string addUser = string.Format("?gm ds AddUser({0});", item);
                            sendAnswer1 = System.Text.Encoding.ASCII.GetBytes(addUser);
                            client.BeginSend(sendAnswer1, 0, sendAnswer1.Length, SocketFlags.None, new AsyncCallback(MySendCallbackFunction), client);

                        }
                    }
                    NameOfUser = username;
                    justConnected = false;
                }

                Console.WriteLine(string.Format("Client {0} connected on {1}", ns.RemoteEndPoint.ToString(), now));

                socket.BeginAccept(new AsyncCallback(MyAcceptCallbackFunction), socket);
                try
                {
                    do
                    {


                        broke = false;
                        byte[] sendAnswer = new byte[1024];
                        int bytesReceived = ns.Receive(sendAnswer);
                        string str = System.Text.Encoding.ASCII.GetString(sendAnswer, 0, bytesReceived);
                        sendAnswer = System.Text.Encoding.ASCII.GetBytes(NameOfUser + ": " + str);

                        foreach (var client in listofClients)
                        {

                            try
                            {

                                client.BeginSend(sendAnswer, 0, sendAnswer.Length, SocketFlags.None, new AsyncCallback(MySendCallbackFunction), client);
                            }
                            catch
                            { }

                        }


                    } while (!broke);
                }
                catch (Exception e)
                {
                    listofClients.Remove(ns);
                    userList.Remove(NameOfUser);
                    byte[] sendAnswer1 = new byte[1024];
                    byte[] sendAnswer2 = new byte[1024];
                    //int bytesReceived1 = ns.Receive(sendAnswer1);
                    foreach (var client in listofClients)
                    {
                        try
                        {


                            //send sendAnswer1 to all clients
                            string addUser = string.Format("?gm ds RemoveUser({0});", NameOfUser);
                            sendAnswer1 = System.Text.Encoding.ASCII.GetBytes(addUser);
                            client.BeginSend(sendAnswer1, 0, sendAnswer1.Length, SocketFlags.None, new AsyncCallback(MySendCallbackFunction), client);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    string disconnectedTime = DateTime.Now.ToString();
                    Console.WriteLine(string.Format("Client {0} Disconnected on {1} ", ns.RemoteEndPoint.ToString(), now));

                    Console.WriteLine(e.Message);
                }
            }
            void MySendCallbackFunction(IAsyncResult ia)
            {
                if (broke)
                {
                    Socket ns = (Socket)ia.AsyncState;
                    ns.Shutdown(SocketShutdown.Receive);
                    ns.Close();
                }

            }

            public void MakeAnnouncement(string Subject, string from, string content)
            {
                byte[] sendAnswer = new byte[1024];
                sendAnswer = System.Text.Encoding.ASCII.GetBytes("?gm ds MakeAnnouncement");
                foreach (var client in listofClients)
                {
                    client.BeginSend(sendAnswer, 0, sendAnswer.Length, SocketFlags.None, new AsyncCallback(MySendCallbackFunction), client);
                }
                Announcement announcement = new Announcement();
                announcement.Subject = Subject;
                announcement.From = from;
                announcement.Content = content;
                announcement.Date = DateTime.Now;
                using (var db = new LibraryContext())
                {
                    db.Announcements.Add(announcement);
                    db.SaveChanges();
                    var users = db.Users.ToList();
                    var smtpClient = new SmtpClient("smtp-mail.outlook.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("anikjanas168@outlook.com", "Aptest123"),
                        EnableSsl = true,
                    };
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress("anikjanas168@outlook.com", "IBC BOOK store");
                    mailMessage.Subject = announcement.Subject;
                    mailMessage.Body = announcement.Content;
                    foreach (var user in users)
                    {
                        mailMessage.To.Add(user.Email);
                    }
                    smtpClient.Send(mailMessage);

                }
            }
        }
        #endregion
    }
}
