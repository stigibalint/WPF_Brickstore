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

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            dataGrid.ItemsSource = LegoItems; // Itt cseréltem le a CollectionViewSource-t
        }

        private void LoadData(string filePath)
        {
            try
            {
                if (!System.IO.File.Exists(filePath))
                {
                    MessageBox.Show("The selected file does not exist.");
                    return;
                }

                XDocument xaml = XDocument.Load(filePath);
                foreach (var elem in xaml.Descendants("Item"))
                {
                    LegoItems.Add(new LegoItem
                    {
                        ItemID = elem.Element("ItemID").Value,
                        ItemName = elem.Element("ItemName").Value,
                        CategoryName = elem.Element("CategoryName").Value,
                        ColorName = elem.Element("ColorName").Value,
                        Qty = int.Parse(elem.Element("Qty").Value ?? "0")
                    });
                }
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
            var view = CollectionViewSource.GetDefaultView(LegoItems);
            view.Filter = item =>
            {
                var legoItem = item as LegoItem;
                bool categoryMatch = string.IsNullOrEmpty(categoryName) || legoItem.CategoryName.ToLower().Contains(categoryName.ToLower());
                bool itemNameMatch = string.IsNullOrEmpty(itemName) || legoItem.ItemName.ToLower().Contains(itemName.ToLower());


                return categoryMatch && itemNameMatch;
            };
        }
        private void OnFilterChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters(categoryFilterTextBox.Text, itemFilterTextBox.Text);
        }
    }
}