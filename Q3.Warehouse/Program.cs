using System;
using System.Collections.Generic;

namespace Q3.Warehouse
{
    // a. Marker interface
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // b. ElectronicItem
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id; Name = name; Quantity = quantity; Brand = brand; WarrantyMonths = warrantyMonths;
        }

        public override string ToString() => $"[Electronics] #{Id} {Name} x{Quantity} Brand:{Brand} Warranty:{WarrantyMonths}m";
    }

    // c. GroceryItem
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id; Name = name; Quantity = quantity; ExpiryDate = expiryDate;
        }

        public override string ToString() => $"[Grocery] #{Id} {Name} x{Quantity} Expires:{ExpiryDate:d}";
    }

    // e. Custom exceptions
    public class DuplicateItemException : Exception { public DuplicateItemException(string msg) : base(msg) { } }
    public class ItemNotFoundException : Exception { public ItemNotFoundException(string msg) : base(msg) { } }
    public class InvalidQuantityException : Exception { public InvalidQuantityException(string msg) : base(msg) { } }

    // d. Generic Inventory Repository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Item with ID {id} not found.");
        }

        public List<T> GetAllItems() => new List<T>(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");
            var item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    // f. WareHouseManager
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Phone", 25, "Samsung", 12));

            _groceries.AddItem(new GroceryItem(1, "Rice (5kg)", 50, DateTime.Today.AddMonths(12)));
            _groceries.AddItem(new GroceryItem(2, "Milk (1L)", 80, DateTime.Today.AddMonths(6)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
                Console.WriteLine(item);
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Updated quantity for ID {id} to {item.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IncreaseStock Error] {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Removed item with ID {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RemoveItem Error] {ex.Message}");
            }
        }

        public void Demo()
        {
            Console.WriteLine("-- Groceries --");
            PrintAllItems(_groceries);

            Console.WriteLine("\n-- Electronics --");
            PrintAllItems(_electronics);

            Console.WriteLine("\n-- Exception Demos --");
            try { _electronics.AddItem(new ElectronicItem(1, "Tablet", 5, "Apple", 18)); }
            catch (Exception ex) { Console.WriteLine($"Duplicate add: {ex.Message}"); }

            RemoveItemById(_groceries, 999); // non-existent

            try { _electronics.UpdateQuantity(2, -10); }
            catch (Exception ex) { Console.WriteLine($"Invalid qty: {ex.Message}"); }
        }
    }

    public static class Program
    {
        public static void Main()
        {
            var manager = new WareHouseManager();
            manager.SeedData();
            manager.Demo();
        }
    }
}