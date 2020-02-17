using System;
using System.Collections.Generic;
using System.Text;

namespace StockControl
{
    public class Categories
    {
        bool isSanitary, isCleaning, isLaundry, isMedical, isConsumable;

        public bool IsSanitary { get => isSanitary; set => isSanitary = value; }
        public bool IsCleaning { get => isCleaning; set => isCleaning = value; }
        public bool IsLaundry { get => isLaundry; set => isLaundry = value; }
        public bool IsMedical { get => isMedical; set => isMedical = value; }
        public bool IsConsumable { get => isConsumable; set => isConsumable = value; }

        public Categories()
        {
            IsSanitary = false;
            IsCleaning = false;
            IsLaundry = false;
            IsMedical = false;
            IsConsumable = false;
        }
        public Categories(bool AllCategories)
        {
            IsSanitary = AllCategories;
            IsCleaning = AllCategories;
            IsLaundry = AllCategories;
            IsMedical = AllCategories;
            IsConsumable = AllCategories;
        }
        public Categories(bool isSanitary, bool isCleaning, bool isLaundry, bool isMedical, bool isConsumable)
        {
            IsSanitary = isSanitary;
            IsCleaning = isCleaning;
            IsLaundry = isLaundry;
            IsMedical = isMedical;
            IsConsumable = isConsumable;
        }

        public string GetCategoryString()
        {
            string cats = null;
            if (IsSanitary)
                cats += "Sanitary,";
            if (IsCleaning)
                cats += "Cleaning,";
            if (IsLaundry)
                cats += "Laundry,";
            if (IsConsumable)
                cats += "Consumable,";
            if (IsMedical)
                cats += "Medical";

            if (!string.IsNullOrEmpty(cats) && cats.EndsWith(","))
                cats.Remove(cats.Length - 2);
            return cats;
        }
    }
}
