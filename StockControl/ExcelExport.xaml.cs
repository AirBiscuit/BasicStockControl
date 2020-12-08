using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Office.Interop;
using Microsoft.Win32;

namespace StockControl
{
    /// <summary>
    /// Interaction logic for ExcelExport.xaml
    /// </summary>
    /// 
    class RowFormat
    {
        string Name;
        int[] Day;
    }
    public partial class ExcelExport : System.Windows.Window
    {
        public ExcelExport()
        {
            InitializeComponent();
        }

        private void btnCalcMonth_Click(object sender, RoutedEventArgs e)
        {
            /*Work out the rows
            One row for every item.
            once rows are done, for each item go through every day
            total each item
            put it in the appropriate column*/
            DateTime selection = new DateTime(2020, cbxMonth.SelectedIndex + 1, 1);

            //Generate the rows for every item first
            var main = System.Windows.Application.Current.MainWindow as MainWindow;
            var itemNames = main.ItemsList;
            Backup newLists = new Backup();
            gridMonth.ItemsSource = itemNames;

            //Open all the files, every month, then every day
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
                newLists = HelperFunctions.RestoreMonth(dlg.FileName, selection);
            }
            int daysInMonth = 31;
            //Account for months with less than 31 days
            switch (selection.Month)
            {
                case 2:
                    daysInMonth = 28;
                    break;
                case 4:
                    daysInMonth = 30;
                    break;
                case 6:
                    daysInMonth = 30;
                    break;
                case 9:
                    daysInMonth = 30;
                    break;
                case 11:
                    daysInMonth = 30;
                    break;
                default:
                    break;
            }
            int itemIndex = -1;
            var ItemUpdates = newLists.DayUpdates;
            foreach (var name in itemNames)
            {
                //Counter for index of the rows
                itemIndex++;
                //Limit the list to one item at a time
                var thisItem = ItemUpdates.Where(x => x.ItemName == name.Name);
                //Go through colunms 1 to 31 and write in quantity changes
                for (int i = 1; i == daysInMonth; i++)
                {
                    foreach (var item in thisItem)
                    {
                        //Check if they match: If they do, add the quantity to this day's tally
                        //If nothing matches, the quantity will be 0
                        if (item.Date.Day == i)
                        {

                        }

                    }
                }
            }
        }
    }
}
