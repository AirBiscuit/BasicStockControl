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
using Microsoft.Win32;

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
        #region My Methods
        private void SetPaths()
        {
            folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\StBrendansStock";
            Directory.CreateDirectory(folderPath);
            filePath = folderPath + "\\Items.json";
        }  

        /// <summary>
        /// Refresh the attributes pane with the selected Item's details
        /// </summary>
        void UpdateAttributes()
        {
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

        /// <summary>
        /// Load the items saved in Items.json into a C# List
        /// </summary>
        /// <returns></returns>
        List<Item> LoadItemList()
        {
            string plainText;
            List<Item> i = new List<Item>();
            if (File.Exists(filePath))
            {
                plainText = File.ReadAllText(filePath);
                i = JsonConvert.DeserializeObject<List<Item>>(plainText);
            }
            return i;
        }

        /// <summary>
        /// Save the current list of items into Items.json
        /// </summary>
        void SaveItemsList()
        {
            string outputJSON = JsonConvert.SerializeObject(ItemsList, Formatting.Indented);
            File.WriteAllText(filePath, outputJSON);
            MessageBox.Show("Save complete", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Add or subtract the specified amount from the item at a specific position in the list.
        /// </summary>
        /// <param name="Index">Position in the list</param>
        /// <param name="QuantityChange">Amount to add or remove the selected item's quantity</param>
        public void UpdateQuantity(int Index, int QuantityChange)
        {
            /*
             * Warning: If the DataGridView has been sorted, index would likely refer to the wrong place in the list.
             * Workaround: The combobox on DayUpdateWindow explicitly uses ItemsList as its datasource, so sorting on main 
             * won't affect ItemsList's order. The indices *should* be correct.
             */
            ItemsList[Index].Quantity += QuantityChange;
            RefreshList();
        }

        public void UpdateQuantity(GridEntry ChangedItem, int QuantityChange)
        {
            ItemsList.First(x => x.Name == ChangedItem.ItemName).Quantity += QuantityChange;
            RefreshList();
        }

        public void RefreshList()
        {
            lstItems.ItemsSource = null;
            lstItems.ItemsSource = ItemsList;
        }

        void SetEditButtons(bool enabled)
        {
            btnEditItem.IsEnabled = enabled;
            hdrEditItem.IsEnabled = enabled;
        }

        /// <summary>
        /// Allow the DataGrid to be directly edited to allow manual corrections
        /// </summary>
        /// <param name="isEditingEnabled">Is the DataGrid allowed to be manipulated?</param>
        void ToggleDataEditing(bool isEditingEnabled)
        {
            if (isEditingEnabled)
            {
                lstItems.Columns[3].IsReadOnly = false;
                btnCorrect.Visibility = Visibility.Visible;
                btnDayUpdate.Visibility = Visibility.Hidden;
            }
            else
            {
                lstItems.Columns[3].IsReadOnly = true;
                btnCorrect.Visibility = Visibility.Hidden;
                btnDayUpdate.Visibility = Visibility.Visible;
            }
        }
        #endregion
        #region Events
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
            if (lstItems.SelectedIndex != -1)
            {
                var ItemWindow = new AddItemWindow();
                ItemWindow.Show();
                ItemWindow.OpenWithItemSelected(currentItem);
            }
            else SetEditButtons(false);
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
            SetEditButtons(true);
        }
        private void MenuBackup_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog
            {
                FileName = string.Format("StBrendansBackup {0}-{1}-{2}", DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year),
                Filter = "Stockout Backup Files (.stockout)|*.stockout",
                DefaultExt = ".stockout"
            };
            bool? result = saveFile.ShowDialog();
            if (result == true)
                HelperFunctions.Backup(folderPath, filePath, saveFile.FileName);
        }
        private void MenuRestore_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Title = "Open .stockout backup file",
                Filter = "Stockout Backup Files (.stockout)|*.stockout",
                DefaultExt = ".stockout",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                var newLists = HelperFunctions.Restore(dlg.FileName, folderPath);
                ItemsList = new ObservableCollection<Item>(newLists.Items);
                if (ItemsList.Count > 1)
                    SaveItemsList();
            }
        }

        private void MenuEditDay_Click(object sender, RoutedEventArgs e)
        {
            //Show the Day update Edit window
            EditDayUpdate editDay = new EditDayUpdate();
            editDay.Show();
        }

        private void MenuCorrect_Click(object sender, RoutedEventArgs e)
        {
            ToggleDataEditing(true);
        }

        private void BtnCorrect_Click(object sender, RoutedEventArgs e)
        {
            ToggleDataEditing(false);
            //Save the List
            SaveItemsList();
        }

        private void MenuExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelExport excel = new ExcelExport();
            excel.Show();
        }

        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveItemsList();
        }
        #endregion
    }
}

