using System;
using System.Collections.Generic;

namespace CoffeeShopInterfaces
{
    // ---------------------------------------------------------
    // частина 1: класи з інтерфейсами
    // ---------------------------------------------------------

    // додаємо icomparable для сортування та icloneable для копіювання
    public abstract class MenuItem : IComparable<MenuItem>, ICloneable
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public MenuItem(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public abstract void DisplayInfo();

        // реалізація icomparable
        // цей метод визначає "хто важливіший" при сортуванні
        public int CompareTo(MenuItem other)
        {
            if (other == null) return 1;

            // логіка: спочатку порівнюємо за ціною
            int priceComparison = this.Price.CompareTo(other.Price);

            // якщо ціни різні - повертаємо результат порівняння цін
            if (priceComparison != 0)
            {
                return priceComparison;
            }

            // якщо ціни однакові - сортуємо за алфавітом (по назві)
            return this.Name.CompareTo(other.Name);
        }

        // реалізація icloneable
        // створює "неглибоку" копію об'єкта
        public object Clone()
        {
            // memberwiseclone автоматично копіює всі поля (значення)
            // для складних типів (списків) треба писати складнішу логіку, 
            // але для string, decimal, int, bool цього достатньо
            return this.MemberwiseClone();
        }

        // перевизначимо tostring для зручного виводу в консоль
        public override string ToString()
        {
            return $"{Name} - ${Price}";
        }
    }

    public class Coffee : MenuItem
    {
        public string BeanType { get; set; }
        public bool IsIced { get; set; }

        public Coffee(string name, decimal price, string beanType, bool isIced)
            : base(name, price)
        {
            BeanType = beanType;
            IsIced = isIced;
        }

        public override void DisplayInfo() => Console.WriteLine($"[Coffee] {Name} (${Price})");
    }

    public class Pastry : MenuItem
    {
        public int Calories { get; set; }

        public Pastry(string name, decimal price, int calories)
            : base(name, price)
        {
            Calories = calories;
        }

        public override void DisplayInfo() => Console.WriteLine($"[Pastry] {Name} (${Price})");
    }

    // ---------------------------------------------------------
    // частина 2: демонстрація
    // ---------------------------------------------------------

    class Program
    {
        static void Main()
        {
            // 1. створюємо список (меню)
            List<MenuItem> menu = new List<MenuItem>
            {
                new Coffee("Latte", 4.50m, "Arabica", false),
                new Pastry("Donut", 2.00m, 300), // найдешевший
                new Coffee("Espresso", 2.50m, "Robusta", false),
                new Pastry("Cake", 5.00m, 500),  // найдорожчий
                new Coffee("Latte", 4.50m, "Robusta", true) // така ж ціна як у першого, але інша назва/тип
            };

            Console.WriteLine("--- BEFORE SORTING (Original Order) ---");
            PrintList(menu);

            // 2. сортування (icomparable)
            // метод sort() автоматично викликає наш compareto для кожного елемента
            menu.Sort();

            Console.WriteLine("\n--- AFTER SORTING (By Price, then Name) ---");
            PrintList(menu);

            // 3. клонування (icloneable)
            Console.WriteLine("\n--- CLONING DEMO ---");

            // беремо перший елемент зі списку (це має бути donut, бо він найдешевший)
            MenuItem original = menu[0];

            // створюємо клон
            // важливо: метод clone повертає object, тому треба робити приведення типів
            MenuItem clonedCopy = (MenuItem)original.Clone();

            Console.WriteLine($"Original: {original}");
            Console.WriteLine($"Clone:    {clonedCopy}");

            // змінюємо клон, щоб довести, що це окремий об'єкт
            Console.WriteLine("\n>> Modifying the clone (Price += 10, Name change)...");
            clonedCopy.Price += 10.00m;
            clonedCopy.Name = "Golden Donut";

            Console.WriteLine($"Original: {original} (Unchanged)");
            Console.WriteLine($"Clone:    {clonedCopy} (Changed)");

            // перевірка посилань
            // referenceequals поверне false, якщо це два різні об'єкти в пам'яті
            bool areSame = ReferenceEquals(original, clonedCopy);
            Console.WriteLine($"\nAre they the same object in memory? -> {areSame}");

            Console.ReadLine();
        }

        // допоміжний метод для друку списку
        static void PrintList(List<MenuItem> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}