﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
using System.IO;
using System.Globalization;

namespace StockControl
{
    /// <summary>
    /// Interaction logic for EditDayUpdate.xaml
    /// </summary>
    public partial class EditDayUpdate : Window
    {
        public ObservableCollection<GridEntry> Entries, oldEntries;
        readonly MainWindow main;
        DateTime selected;
        bool isValid = false;

        public bool IsValid { get => isValid; set => isValid = value; }

        public EditDayUpdate()
        {
            InitializeComponent();
            IsValid = true;
            main = Application.Current.MainWindow as MainWindow;
            Entries = new ObservableCollection<GridEntry>();
            grdList.DataContext = Entries;
            //Open today's edit if it exists
            cdrSelectedDay.SelectedDate = DateTime.Today;

            //Fill the combobox with existing day updates
            //Fill the datagrid with GridEntries when the combobox's selction changes


        }

        private void CdrSelectedDay_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            //Ignore events where the day is entirely unselected
            if (cdrSelectedDay.SelectedDate.HasValue)
            {
                selected = cdrSelectedDay.SelectedDate.GetValueOrDefault();
                var daysEntries = HelperFunctions.OpenDayUpdateJson(selected);
                if (daysEntries != null)
                {
                    txtDay.Foreground = new SolidColorBrush(Colors.Green);
                    txtDay.Text = string.Format("{0} selected", selected.ToShortDateString());
                    Entries = new ObservableCollection<GridEntry>(daysEntries);
                    oldEntries = Entries;
                    grdList.DataContext = Entries;
                    btnDelAll.IsEnabled = true;
                    btnAdd.IsEnabled = true;
                    grdList.CanUserAddRows = true;
                    grdList.CanUserDeleteRows = true;
                }
                else
                {
                    txtDay.Foreground = new SolidColorBrush(Colors.Red);
                    txtDay.Text = "No Entries this day";
                    Entries = new ObservableCollection<GridEntry>();
                    grdList.DataContext = Entries;
                    btnDelAll.IsEnabled = false;
                    btnAdd.IsEnabled = false;
                    grdList.CanUserAddRows = false;
                    grdList.CanUserDeleteRows = false;
                }

            }
            else
            {
                txtDay.Foreground = new SolidColorBrush(Colors.Black);
                txtDay.Text = "Select Day:";
            }

        }

        private void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            HelperFunctions.SaveDayUpdateJson(Entries.ToList<GridEntry>(), (DateTime)cdrSelectedDay.SelectedDate);
            //TODO: Figure out how much of a difference to apply to main.ItemsList
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Either work on manually adding a new item to the DataContext or Enable adding a new row directly in the datagrid.
            
            SetValid(true);
        }

        public void SetValid(bool isValid)
        {
            IsValid = isValid;
            btnFinish.IsEnabled = isValid;
        }

        private void BtnDelEntry_Click(object sender, RoutedEventArgs e)
        {
            btnFinish.IsEnabled = true;
            Entries.RemoveAt(grdList.SelectedIndex);
            grdList.SelectedIndex = -1;
            btnDelEntry.IsEnabled = false;
        }

        private void BtnDelAll_Click(object sender, RoutedEventArgs e)
        {
            //Delete the file altogether
            //Get the path
            string pathToFile = HelperFunctions.GetPathFromDay(selected);
            //Double check the user intended to do this
            var result = MessageBox.Show("Are you sure you want to permanently delete this day?", "Delete Day?",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            //If they did, proceed.     If they didn't, instead do nothing.
            if (result == MessageBoxResult.Yes)
            {
                if (File.Exists(pathToFile))
                {
                    File.Delete(pathToFile);
                    MessageBox.Show("Day Update Deleted Successfully", "File deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else MessageBox.Show("This day is already deleted, or doesn't exist", "File not found", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void GrdList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //TODO: Disable the button if the calendar loses selection.
            if (cdrSelectedDay.SelectedDate != null)
            {

            }
            btnDelEntry.IsEnabled = true;
        }

        private void GrdList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //Extra Validation is needed here: Quantity must be a number, and Name has to correspond to an item name
            //otherwise reject the edit
            //Update: only the quantity needs validation here, the text is checked seperately

            if (e.Column.Header.ToString() == "Quantity")
                SetValid(int.TryParse((e.EditingElement as TextBox).Text, out _));
        }

    }
    public class StockValidationRule : ValidationRule
    {
        MainWindow main;
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            main = Application.Current.MainWindow as MainWindow;
            EditDayUpdate window = Application.Current.Windows.OfType<EditDayUpdate>().FirstOrDefault();
            List<Item> validItemNames = main.ItemsList.ToList<Item>();
            string name = (value as BindingGroup).Items[0].ToString();
            Item result;
            try
            {
                result = validItemNames.First(x => x.Name == name);
            }
            catch (InvalidOperationException)
            {
                result = new Item();
            }

            if (result.Name == name)
            {
                //TODO: Put this into a SetValid method which also handles the Finish button
                window.SetValid(true);
                return ValidationResult.ValidResult;
            }
            else
            {
                window.SetValid(false);
                return new ValidationResult(false, "Item must already exist");
            }
        }
    }
}
