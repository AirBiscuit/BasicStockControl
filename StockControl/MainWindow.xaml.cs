using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.IO;

namespace StockControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Item> ItemsList;
        Item currentItem;
        public string filePath, folderPath;
        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
            SetPaths();
            //Read all the items from the saved list
            ItemsList = new ObservableCollection<Item>(LoadItemList());
            //Put them in the listbox
            lstItems.DataContext = ItemsList;
            lstItems.Columns[0].SortDirection = System.ComponentModel.ListSortDirection.Ascending;
        }

        private void SetPaths()
        {
            folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\StBrendansStock";
            Directory.CreateDirectory(folderPath);
            filePath = folderPath + "\\Items.json";
        }
        void UpdateAttributes()
        {
            //Change the attributes pane to the currently selected item's details.
            lstAttributes.Items.Clear();
            int selection = lstItems.SelectedIndex;
            if (selection == -1)
                selection = 0;
            currentItem = (Item)lstItems.Items.GetItemAt(selection);

            lstAttributes.Items.Add("Name: " + currentItem.Name);
            lstAttributes.Items.Add("Quantity: " + currentItem.Quantity);
            lstAttributes.Items.Add("Supplier: " + currentItem.Supplier);
            lstAttributes.Items.Add("Manufacturer: " + currentItem.Manufacturer);
            string cats = currentItem.Categories.GetCategoryString();
            if (!string.IsNullOrEmpty(cats))
                lstAttributes.Items.Add("Categories: " + cats);
            else lstAttributes.Items.Add("Categories: None");
        }
        List<Item> LoadItemList()
        {
            //TODO: Delete this testing code
            //currentItem = new Item("Toilet Roll", Color.White, 1, 27, "Western Hygiene", "Tork");
            //ItemsList.Add(currentItem);
            //currentItem = new Item("J Cloths", Color.Blue, 10, 50, "Aldi", "Power Force");
            //ItemsList.Add(currentItem);
            //currentItem = new Item("Taski Sani", Color.Red, 1, 4, "MG Hygiene", "Diversy", "1.5L");
            //ItemsList.Add(currentItem);
            //currentItem = new Item("White bin bags", "Lidl", "Purio");
            //ItemsList.Add(currentItem);

            string plainText;
            List<Item> i = new List<Item>();
            if (File.Exists(filePath))
            {
                plainText = File.ReadAllText(filePath);
                i = JsonConvert.DeserializeObject<List<Item>>(plainText);
            }
            /* * * * * * * * * * * * * * * * * * * * * * * * 
             * Try the default destination
             * If not found, show a file selection dialog
             * Option A): Open streamreader, iterate and add each item to i
             * Option B): Use native JSON storage, open JSON file, add to i
             * * * * * * * * * * * * * * * * * * * * * * * */
            return i;
        }
        void SaveItemsList()
        {
            /*
            
            Show file selection dialog
            Create a streamwriter
            Iterate through the list
            Write to the stream
            Close the stream.

            */
            string outputJSON = JsonConvert.SerializeObject(ItemsList, Formatting.Indented);
            File.WriteAllText(filePath, outputJSON);
            MessageBox.Show("Save complete", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void BtnAddNewItem_Click(object sender, RoutedEventArgs e)
        {
            //Open the new item UI
            var ItemWindow = new AddItemWindow();
            ItemWindow.Show();
        }
        private void BtnEditItem_Click(object sender, RoutedEventArgs e)
        {
            //If there's an item selected, open the edit window with it as the context
            //Otherwise just open the edit window.
            var ItemWindow = new AddItemWindow();
            ItemWindow.Show();
            ItemWindow.OpenWithItemSelected(currentItem);
        }
        private void BtnDayUpdate_Click(object sender, RoutedEventArgs e)
        {
            //Open the new day window
            var Update = new DayUpdateWindow();
            Update.Show();
        }
        private void LstItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAttributes();
            btnEditItem.IsEnabled = true;
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveItemsList();
        }
    }
}

