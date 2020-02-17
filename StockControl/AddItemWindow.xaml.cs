using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;

namespace StockControl
{
    /// <summary>
    /// Interaction logic for AddItemWindow.xaml
    /// </summary>
    public partial class AddItemWindow : Window
    {
        readonly string[] Colourlist;
        public AddItemWindow()
        {
            InitializeComponent();
            Colourlist = new string[] { "Transparent", "Black", "White", "Grey", "Red", "Orange", "Yellow", "Green", "Blue", "Purple", "Brown", "Pink" };
            foreach (string c in Colourlist)
                cboxColours.Items.Add(c);
            cboxColours.SelectedIndex = 0;
        }
        public void OpenWithItemSelected(Item selection)
        {
            btnAdd.Content = "Finish Editing";
            txtName.Text = selection.Name;
            txtQuantity.Text = selection.Quantity.ToString();
            txtSupplier.Text = selection.Supplier;
            txtManufacturer.Text = selection.Manufacturer;
            for (int i = 0; i < Colourlist.Length; i++)
            {
                if (selection.Colour.Name == Colourlist[i])
                {
                    cboxColours.SelectedIndex = i;
                    break;
                }
            }
            txtVolume.Text = selection.Volume;
            txtCapacity.Text = selection.Capacity.ToString();
            string cats = selection.Categories.GetCategoryString();
            if (!string.IsNullOrEmpty(cats))
            {
                chkSanitary.IsChecked = cats.Contains("Sanitary");
                chkCleaning.IsChecked = cats.Contains("Cleaning");
                chkLaundry.IsChecked = cats.Contains("Laundry");
                chkConsumable.IsChecked = cats.Contains("Consumable");
                chkMedical.IsChecked = cats.Contains("Medical");
            }
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Check if this is an edit or an addition
            bool isEdit = false;
            if (btnAdd.Content.ToString() == "Finish Editing")
                isEdit = true;
            Categories categories = new Categories((bool)chkSanitary.IsChecked, (bool)chkCleaning.IsChecked,
                                                   (bool)chkLaundry.IsChecked, (bool)chkMedical.IsChecked, (bool)chkConsumable.IsChecked);
            int.TryParse(txtQuantity.Text, out int quan);
            int.TryParse(txtCapacity.Text, out int cap);
            Item thisItem = new Item(txtName.Text, Color.FromName(cboxColours.SelectedItem.ToString()), cap, quan,
                                     categories, txtSupplier.Text, txtManufacturer.Text, txtVolume.Text);
            MainWindow main = Application.Current.MainWindow as MainWindow;
            if (isEdit)
                main.ItemsList[main.lstItems.SelectedIndex] = thisItem;
            else main.ItemsList.Add(thisItem);
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TxtQuantity_GotFocus(object sender, RoutedEventArgs e)
        {
            txtQuantity.SelectAll();
        }

        private void TxtCapacity_GotFocus(object sender, RoutedEventArgs e)
        {
            txtCapacity.SelectAll();
        }

        private void TxtQuantity_GotFocus(object sender, MouseEventArgs e)
        {

        }


    }
}
