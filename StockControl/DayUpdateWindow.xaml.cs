﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace StockControl
{
    /// <summary>
    /// Interaction logic for DayUpdateWindow.xaml
    /// </summary>
    public partial class DayUpdateWindow : Window
    {
        public ObservableCollection<GridEntry> Entries;
        MainWindow main;
        public DayUpdateWindow()
        {
            InitializeComponent();
            main = Application.Current.MainWindow as MainWindow;
            Entries = new ObservableCollection<GridEntry>();

            //Set up all the UI elements to lessen chance of exception
            calendar.SelectedDate = DateTime.Today;
            //Add list of items to the top combobox
            cboxItemName.ItemsSource = main.ItemsList;
            cboxItemName.SelectedIndex = 0;
            txtQuantity.Text = "1";
            grdList.DataContext = Entries;


            //TEST DATA
            Entries.Add(new GridEntry("Toilet Roll", DateTime.Today, 5));
            Entries.Add(new GridEntry("Window Cleaner", DateTime.Today, 5));
            Entries.Add(new GridEntry("Blue Roll", DateTime.Today, 5));
        }

        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            //Add to the datagrid
            int.TryParse(txtQuantity.Text, out int quan);
            if (cboxIncoming.SelectedIndex == 0)
            {
                quan *= -1;
            }
            DateTime d = (DateTime)calendar.SelectedDate;
            Entries.Add(new GridEntry(cboxItemName.Text, d, quan));
            grdList.DataContext = Entries;
            //Update the quantity of the item on the main page
        }

        private void TxtQuantity_GotFocus(object sender, RoutedEventArgs e)
        {
            txtQuantity.SelectAll();
        }

        private void txtQuantity_GotMouseCapture(object sender, MouseEventArgs e)
        {
            txtQuantity.SelectAll();
        }

        private void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            //Save all the days into seperate files
            //Directory by year, month
            List<DateTime> dates = new List<DateTime>();
            foreach (GridEntry item in Entries)
            {
                if (!dates.Contains(item.Date))
                    dates.Add(item.Date);
            }
            List<GridEntry> dayUpdate;
            foreach (DateTime date in dates)
            {
                string directory = string.Format("{0}\\{1}\\{2}", main.folderPath, date.Year, date.Month);
                string file = string.Format("{0}\\{1}{2}", directory, date.Day, ".json");
                Directory.CreateDirectory(directory);
                dayUpdate = new List<GridEntry>();
                for (int i = 0; i < Entries.Count; i++)
                {
                    if (Entries[i].Date == date)
                    {
                        dayUpdate.Add(Entries[i]);
                    }
                }
                string outputJSON = JsonConvert.SerializeObject(dayUpdate, Formatting.Indented);
                if (!string.IsNullOrEmpty(outputJSON))
                    File.WriteAllText(file, outputJSON);
            }
            MessageBox.Show("Save complete", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
