using System;
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

namespace StockControl
{
    /// <summary>
    /// Interaction logic for EditDayUpdate.xaml
    /// </summary>
    public partial class EditDayUpdate : Window
    {
        public ObservableCollection<GridEntry> Entries, oldEntries;
        MainWindow main;
        public EditDayUpdate()
        {
            InitializeComponent();
            main = Application.Current.MainWindow as MainWindow;
            Entries = new ObservableCollection<GridEntry>();
            grdList.DataContext = Entries;
            //Go through the folder of the calendar's current month, find all .json files
            cdrSelectedDay.SelectedDate = DateTime.Today;

            //Fill the combobox with existing day updates
            //Fill the datagrid with GridEntries when the combobox's selction changes


        }

        private void CdrSelectedDay_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            //Ignore events where the day is entirely unselected
            if (cdrSelectedDay.SelectedDate.HasValue)
            {
                DateTime selected;
                selected = cdrSelectedDay.SelectedDate.GetValueOrDefault();
                var daysEntries = HelperFunctions.OpenDayUpdateJson(selected);
                if (daysEntries != null)
                {
                    Entries = new ObservableCollection<GridEntry>(daysEntries);
                    oldEntries = Entries;
                    grdList.DataContext = Entries;
                    btnDelEntry.IsEnabled = true;
                }
                else
                {
                    Entries = new ObservableCollection<GridEntry>();
                    grdList.DataContext = Entries;
                    btnDelEntry.IsEnabled = false;
                }

            }

        }

        private void BtnFinish_Click(object sender, RoutedEventArgs e)
        {

            //foreach (DataGridRow row in grdList.)
            //{

            //}
            HelperFunctions.SaveDayUpdateJson(Entries.ToList<GridEntry>(), (DateTime)cdrSelectedDay.SelectedDate);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            btnFinish.IsEnabled = true;
        }

        private void BtnDelEntry_Click(object sender, RoutedEventArgs e)
        {
            btnFinish.IsEnabled = true;
        }

        private void BtnDelAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GrdList_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            btnDelEntry.IsEnabled = true;
        }

        private void GrdList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            btnFinish.IsEnabled = true;
        }
    }
}
