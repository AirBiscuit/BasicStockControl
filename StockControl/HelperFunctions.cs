using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Windows;

namespace StockControl
{
    public struct Backup
    {
        public List<Item> Items;
        public List<GridEntry> DayUpdates;

        public Backup(List<Item> products, List<GridEntry> days)
        {
            Items = products;
            DayUpdates = days;
        }
    }
    public enum ListType
    {
        ItemList,
        GridEntryList
    }
    public static class HelperFunctions
    {
        /// <summary>
        /// Backs up Items.json and every day update json file into a single .stockout file, which is a JSON file 
        /// containing a list of items and day updates. The 2 lists are seperated with the Registered trademark character (®)
        /// </summary>
        /// <param name="MainDirectory"></param>
        /// <param name="itemsFilePath"></param>
        /// <param name="outputPath"></param>
        public static void Backup(string MainDirectory, string itemsFilePath, string outputPath)
        {
            List<Item> items = new List<Item>();
            List<GridEntry> dayUpdates;
            string plainText;
            if (File.Exists(itemsFilePath))
            {
                plainText = File.ReadAllText(itemsFilePath);
                items = JsonConvert.DeserializeObject<List<Item>>(plainText);
            }
            string[] days = Directory.GetFiles(MainDirectory, "*.json", SearchOption.AllDirectories);
            dayUpdates = new List<GridEntry>();
            foreach (string day in days)
            {
                //Ignore the Items.json file, it has been deserialised already
                if (File.Exists(day) && !day.Contains("Items.json"))
                {
                    plainText = File.ReadAllText(day);
                    var current = JsonConvert.DeserializeObject<List<GridEntry>>(plainText);
                    dayUpdates.AddRange(current);
                }
            }
            Backup backup = new Backup(items, dayUpdates);
            string outputItems = JsonConvert.SerializeObject(backup.Items, Formatting.Indented);
            string outputDays = JsonConvert.SerializeObject(backup.DayUpdates, Formatting.Indented);
            string output = string.Format("{0}\n®\n{1}", outputItems, outputDays);
            if (!string.IsNullOrEmpty(outputPath))
            {
                if (!string.IsNullOrEmpty(output))
                    File.WriteAllText(outputPath, output);
                else throw new Exception("The backup file was empty, backup failed.");

            }
            else throw new Exception("No output path has been selected for backup file");
        }

        /// <summary>
        /// Provides a struct with a list of Items and Dayupdates, read from backupFilePath
        /// </summary>
        /// <param name="BackupFilePath">The path to the .stockout file</param>
        /// <param name="RootDirectory">The path to the root directory into which the backed up files will be restored</param>
        /// <returns>The struct with the lists of data</returns>
        public static Backup Restore(string BackupFilePath, string RootDirectory)
        {
            string plainText;
            List<Item> i;
            List<GridEntry> d;
            Backup b = new Backup();
            if (File.Exists(BackupFilePath))
            {
                plainText = File.ReadAllText(BackupFilePath);
                string[] lists = plainText.Split('®');
                i = JsonConvert.DeserializeObject<List<Item>>(lists[0]);
                d = JsonConvert.DeserializeObject<List<GridEntry>>(lists[1]);
                b.Items = i;
                b.DayUpdates = d;
                SaveRestoredDayUpdates(d, RootDirectory);
                string itemsJSON = JsonConvert.SerializeObject(b.Items, Formatting.Indented);
                File.WriteAllText(RootDirectory + "Items.json", itemsJSON);
            }
            return b;
        }
        public static void SaveRestoredDayUpdates(List<GridEntry> entries, string rootDirectory)
        {
            //Save all the days into seperate files
            //Directory by year, month
            Directory.Delete(rootDirectory, true);

            List<DateTime> dates = new List<DateTime>();
            foreach (GridEntry item in entries)
            {
                if (!dates.Contains(item.Date))
                    dates.Add(item.Date);
            }
            List<GridEntry> dayUpdate;
            foreach (DateTime date in dates)
            {
                string directory = string.Format("{0}\\{1}\\{2}", rootDirectory, date.Year, date.Month);
                string file = string.Format("{0}\\{1}{2}", directory, date.Day, ".json");
                Directory.CreateDirectory(directory);
                dayUpdate = new List<GridEntry>();
                for (int i = 0; i < entries.Count; i++)
                {
                    if (entries[i].Date == date)
                        dayUpdate.Add(entries[i]);
                }
                string outputJSON = JsonConvert.SerializeObject(dayUpdate, Formatting.Indented);
                if (!string.IsNullOrEmpty(outputJSON))
                    File.WriteAllText(file, outputJSON);
            }
        }
        public static List<Item> OpenItemsJson()
        {
            string path = string.Format("{0}\\StBrendansStock\\Items.json", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            return JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(path));
        }
        public static List<Item> OpenItemsJson(string FilePath)
        {
            return JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(FilePath));
        }
        public static List<GridEntry> OpenDayUpdateJson(string FilePath)
        {
            return JsonConvert.DeserializeObject<List<GridEntry>>(File.ReadAllText(FilePath));
        }
        public static List<GridEntry> OpenDayUpdateJson(DateTime Day)
        {
            string path = GetPathFromDay(Day);
            if (File.Exists(path))
                return JsonConvert.DeserializeObject<List<GridEntry>>(File.ReadAllText(path));
            else return null;
        }
        public static string GetPathFromDay(DateTime Day)
        {
            string path = string.Format("{0}\\StBrendansStock\\{1}\\{2}\\{3}.json",
                                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Day.Year, Day.Month, Day.Day);
            return path;
        }
        public static void SaveDayUpdateJson(List<GridEntry> newDayUpdate, DateTime Day)
        {
            var main = Application.Current.MainWindow as MainWindow;
            var oldDayUpdate = OpenDayUpdateJson(Day);
            //Save all the days into seperate files
            //Directory by year, month

            List<DateTime> dates = new List<DateTime>();

            var changes = newDayUpdate.Except(oldDayUpdate);
            foreach (GridEntry item in changes)
            {
                for (int i = 0; i < oldDayUpdate.Count; i++)
                {
                    if (item.ItemName == oldDayUpdate[i].ItemName)
                    {
                        oldDayUpdate[i] = item;
                    }
                }
            }
            List<GridEntry> outputUpdate = oldDayUpdate;
            foreach (GridEntry item in outputUpdate)
            {
                if (!dates.Contains(item.Date))
                    dates.Add(item.Date);
                main.UpdateQuantity(item, item.Quantity);
            }
            foreach (DateTime date in dates)
            {
                /* If Dates has more than one entry, that means the Date of a GridEntry has been changed while editing.
                 * When this happens, multiple DayUpdate files now need to be read, changes compred, then all overwritten including
                 * the old data that wasn't changed this time.
                 * 
                 * Example: There are 2 Dayupdates: 16th and 17th of Feb.
                 * DU16: Toilet Roll|-6
                 * DU17: Toilet Roll|-9, Air Freshener|-1, Gloves Large|-4
                 * 
                 * The DayUpdate for 17th Feb is Changed, the Toilet Roll GridEntry has its date changed to the 16th.
                 * The 17th is completely overwritten with all the Items in newDayUpdate that have a matching date because it
                 * was opened at the start of this function. DU17 is now as follows:
                 * DU17: Air Freshener|-1, Gloves Large|-4
                 * This is serialised to JSON and written to a file, all fine.
                 * 
                 * Next, we need to 
                 * 1. find any entries with a new date
                 * 2. Open the Dayupdate file for that date
                 * 3. 
                 */
                for (int i = 0; i < newDayUpdate.Count; i++)
                {
                    if (newDayUpdate[i].Date == date)
                        outputUpdate.Add(newDayUpdate[i]);
                }
                string outputJSON = JsonConvert.SerializeObject(outputUpdate, Formatting.Indented);
                if (!string.IsNullOrEmpty(outputJSON))
                    File.WriteAllText(GetPathFromDay(Day), outputJSON);
            }
        }
        public static void DeleteDayUpdate(DateTime Day)
        {
            File.Delete(GetPathFromDay(Day));
        }
    }
}
