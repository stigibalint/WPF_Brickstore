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
    }
}
