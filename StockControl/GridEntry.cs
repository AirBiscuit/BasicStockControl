using System;
using System.Collections.Generic;
using System.Text;

namespace StockControl
{
    public class GridEntry
    {
        string itemName;
        DateTime date;
        int quantity;
        public string ItemName { get => itemName; set => itemName = value; }
        public DateTime Date { get => date; set => date = value; }
        public int Quantity { get => quantity; set => quantity = value; }

        public GridEntry(string name, DateTime date, int amount)
        {
            ItemName = name;
            Date = date;
            Quantity = amount;
        }
    }
}
