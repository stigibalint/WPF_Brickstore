using LEGO;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;

namespace LEGO
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<LegoItem> LegoItems { get; set; } = new ObservableCollection<LegoItem>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadData("brickstore_parts_7288-1-mobile-police-unit.bsx");
            dataGrid.ItemsSource = CollectionViewSource.GetDefaultView(LegoItems);
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
                        ItemID = elem.Element("ItemID")?.Value,
                        ItemName = elem.Element("ItemName")?.Value,
                        CategoryName = elem.Element("CategoryName")?.Value,
                        ColorName = elem.Element("ColorName")?.Value,
                        Qty = int.Parse(elem.Element("Qty")?.Value ?? "0")
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}");
            }
        }

        private void ApplyFilters(string categoryName)
        {
            var view = CollectionViewSource.GetDefaultView(LegoItems);
            view.Filter = item =>
            {
                var legoItem = item as LegoItem;
                bool categoryMatch = string.IsNullOrEmpty(categoryName) || legoItem.CategoryName == categoryName;
              
                return categoryMatch;
            };
        }

        private void OnFilterChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters(categoryFilterTextBox.Text);
        }
    }
}