using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Newtonsoft.Json;

namespace StockControl
{
    public class Item
    {
        string name, supplier, manufacturer, volume, ean;
        int capacity, quantity;
        Color colour;
        Categories categories;

        public string Name { get => name; set => name = value; }
        public string Supplier { get => supplier; set => supplier = value; }
        public string Manufacturer { get => manufacturer; set => manufacturer = value; }
        public string Volume { get => volume; set => volume = value; }
        public string Ean { get => ean; set => ean = value; }
        public int Capacity { get => capacity; set => capacity = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public Color Colour { get => colour; set => colour = value; }
        public Categories Categories { get => categories; set => categories = value; }

        /// <summary>
        /// An item with the following attributes
        /// </summary>
        /// <param name="Name">The name of the item</param>
        /// <param name="Supplier">Where the item is ordere/purchased from</param>
        /// <param name="Manufacturer">Who makes the item</param>
        /// <param name="Volume">The volume of liquid or weight of the item is applicable</param>
        /// <param name="EAN">The barcode of the item</param>
        public Item(string Name, string Supplier = null, string Manufacturer = null, string Volume = null, string EAN = null)
        {
            this.Name = Name;
            this.Supplier = Supplier;
            this.Manufacturer = Manufacturer;
            this.Volume = Volume;
            Ean = EAN;
            Colour = Color.Transparent;
            Quantity = 0;
            this.Categories = new Categories();
        }
        /// <summary>
        /// An item with the following attributes
        /// </summary>
        /// <param name="Name">The name of the item</param>
        /// <param name="Colour">The colour of the item</param>
        /// <param name="Supplier">Where the item is ordere/purchased from</param>
        /// <param name="Manufacturer">Who makes the item</param>
        /// <param name="Volume">The volume of liquid or weight of the item is applicable</param>
        /// <param name="EAN">The barcode of the item</param>
        /// <param name="Colour"></param>

        public Item(string Name, Color Colour, int Capacity, int Quantity, string Supplier = null, string Manufacturer = null, string Volume = null, string EAN = null)
        {
            this.Name = Name;
            this.Supplier = Supplier;
            this.Manufacturer = Manufacturer;
            this.Volume = Volume;
            Ean = EAN;
            this.Colour = Colour;
            this.Capacity = Capacity;
            this.Quantity = Quantity;
            this.Categories = new Categories();
        }
        public Item(string Name, Color Colour, int Capacity, int Quantity, Categories Categories, string Supplier = null, string Manufacturer = null, string Volume = null, string EAN = null)
        {
            this.Name = Name;
            this.Supplier = Supplier;
            this.Manufacturer = Manufacturer;
            this.Volume = Volume;
            Ean = EAN;
            this.Colour = Colour;
            this.Capacity = Capacity;
            this.Quantity = Quantity;
            this.Categories = Categories;
        }

        public Item(string Name, string Supplier, string Manufacturer, string Volume, string Ean, int Capacity, int Quantity, Color Colour)
        {
            this.Name = Name;
            this.Supplier = Supplier;
            this.Manufacturer = Manufacturer;
            this.Volume = Volume;
            this.Ean = Ean;
            this.Capacity = Capacity;
            this.Quantity = Quantity;
            this.Colour = Colour;
        }
        public Item()
        {
            Name = "";
            Supplier = "";
            Manufacturer = "";
            Volume = "";
            Ean = "";
            Capacity = 0;
            Quantity = 0;
            Colour = Color.Transparent;
            Categories = new Categories(false);
        }
        
        public override string ToString()
        {
            return Name;
        }
    }
}
