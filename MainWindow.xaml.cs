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

namespace WpfApp2
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
                if (!System.IO.File.Exists(filePath))
                {
                    MessageBox.Show("Nem létezik ilyen nevű fájl.");
                    return;
                }

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
                        Qty = int.Parse(elem.Element("Qty").Value ?? "0")
                    };

                    LegoItems.Add(legoItem);
                    originalItems.Add(legoItem);
                }

                var distinctCategories = originalItems.Select(item => item.CategoryName).Distinct().ToList();
                categoryFilterComboBox.ItemsSource = distinctCategories;
            }
            catch (Exception ex)
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

        private void ApplyFilters(string categoryName, string itemName)
        {
            LegoItems.Clear();

            var filteredItems = originalItems.Where(item =>
            {
                bool categoryMatch = categoryName == "" || item.CategoryName.ToLower() == categoryName.ToLower();

                bool itemNameMatch = string.IsNullOrEmpty(itemName) || item.ItemName.ToLower().StartsWith(itemName.ToLower());

                return categoryMatch && itemNameMatch;
            }).ToList();

            foreach (var item in filteredItems)
            {
                LegoItems.Add(item);
            }
        }

        private void OnFilterChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedCategory = categoryFilterComboBox.SelectedItem?.ToString() ?? string.Empty;
            ApplyFilters(selectedCategory, itemFilterTextBox.Text);
        }

        private void OnFilterChanged(object sender, TextChangedEventArgs e)
        {
            string selectedCategory = categoryFilterComboBox.SelectedItem?.ToString() ?? string.Empty;
            ApplyFilters(selectedCategory, itemFilterTextBox.Text);
        }
    }


}