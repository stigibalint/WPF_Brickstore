using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Final_WPF_Brickstore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<LegoItem> LegoItems { get; set; } = new ObservableCollection<LegoItem>();
        private List<LegoItem> originalItems = new List<LegoItem>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            dataGrid.ItemsSource = LegoItems;
        }

        private void LoadData(string filePath)
        {
            try
            {
                XDocument xaml = XDocument.Load(filePath);
                LegoItems.Clear();
                originalItems.Clear();

                foreach (var elem in xaml.Descendants("Item"))
                {
                    var legoItem = new LegoItem
                    {
                        ItemID = elem.Element("ItemID").Value,
                        ItemName = elem.Element("ItemName").Value,
                        CategoryName = elem.Element("CategoryName").Value,
                        ColorName = elem.Element("ColorName").Value,
                        Qty = int.Parse(elem.Element("Qty").Value)
                    };

                    LegoItems.Add(legoItem);
                    originalItems.Add(legoItem);
                }

                UpdateCategoryComboBox();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}");
            }
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "BSX files (*.bsx)|*.bsx|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                LoadData(openFileDialog.FileName);
            }
        }

        private void ApplyFilters()
        {
            string categoryText = categoryFilterTextBox.Text.ToLower();
            string selectedCategory = categoryFilterComboBox.SelectedItem != null && categoryFilterComboBox.SelectedItem.ToString() != "All Categories"
                ? categoryFilterComboBox.SelectedItem.ToString()
                : string.Empty;

            string itemName = itemFilterTextBox.Text.ToLower();

            LegoItems.Clear();

            var filteredItems = originalItems.Where(item =>
            {
                bool categoryTextMatch = string.IsNullOrEmpty(categoryText) || item.CategoryName.ToLower().Contains(categoryText);
                bool categoryComboMatch = string.IsNullOrEmpty(selectedCategory) || item.CategoryName.ToLower() == selectedCategory.ToLower();
                bool itemNameMatch = string.IsNullOrEmpty(itemName) || item.ItemName.ToLower().Contains(itemName);

                return categoryTextMatch && categoryComboMatch && itemNameMatch;
            }).ToList();

            foreach (var item in filteredItems)
            {
                LegoItems.Add(item);
            }

            UpdateCategoryComboBox();
        }

        private void UpdateCategoryComboBox()
        {
            categoryFilterComboBox.SelectionChanged -= OnFilterChanged;


            var filteredCategories = LegoItems.Select(item => item.CategoryName).Distinct().OrderBy(category => category).ToList();

            categoryFilterComboBox.Items.Clear();
            categoryFilterComboBox.Items.Add("All Categories");

            foreach (var category in filteredCategories)
            {
                categoryFilterComboBox.Items.Add(category);
            }

            categoryFilterComboBox.SelectedIndex = 0;
            categoryFilterComboBox.SelectionChanged += OnFilterChanged;
        }

        private void OnFilterChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void OnFilterChanged(object sender, SelectionChangedEventArgs e)
        {

            if (categoryFilterComboBox.SelectedItem != null && categoryFilterComboBox.SelectedItem.ToString() == "All Categories")
            {

                categoryFilterTextBox.Text = string.Empty;
                itemFilterTextBox.Text = string.Empty;


                LegoItems.Clear();
                foreach (var item in originalItems)
                {
                    LegoItems.Add(item);
                }

                UpdateCategoryComboBox();
            }
            else
            {
                ApplyFilters();
            }
        }
    }

}