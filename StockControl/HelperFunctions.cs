using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

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
        /// <param name="backupFilePath">The path of the .stockout file to read the backup from</param>
        /// <returns></returns>
        public static Backup Restore(string backupFilePath)
        {
            string plainText;
            List<Item> i = new List<Item>();
            List<GridEntry> d = new List<GridEntry>();
            Backup b = new Backup();
            if (File.Exists(backupFilePath))
            {
                plainText = File.ReadAllText(backupFilePath);
                string[] lists = plainText.Split('®');
                i = JsonConvert.DeserializeObject<List<Item>>(lists[0]);
                d = JsonConvert.DeserializeObject<List<GridEntry>>(lists[1]);
                b.Items = i;
                b.DayUpdates = d;
            }
            return b;
        }
        public static void SaveRestoredDayUpdates(List<GridEntry> entries, string rootDirectory)
        {
            //Save all the days into seperate files
            //Directory by year, month

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
    }
}
