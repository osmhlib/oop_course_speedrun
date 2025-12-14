using System;
using System.Collections.Generic;

namespace CoffeeShopSystem
{
    // абстрактний базовий клас
    public abstract class MenuItem
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        // конструктор для зручності
        public MenuItem(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        // абстрактний метод - не має тіла, нащадки мусять його реалізувати
        public abstract void DisplayInfo();

        // віртуальний метод - має базову реалізацію, але її можна змінити
        public virtual void Prepare()
        {
            Console.WriteLine($"[system] starting preparation for {Name}...");
        }
    }

    // клас-нащадок 1: кава
    public class Coffee : MenuItem
    {
        public string BeanType { get; set; } // арабіка, робуста
        public bool IsIced { get; set; }     // чи з льодом

        public Coffee(string name, decimal price, string beanType, bool isIced)
            : base(name, price)
        {
            BeanType = beanType;
            IsIced = isIced;
        }

        // перевизначення абстрактного методу
        public override void DisplayInfo()
        {
            string type = IsIced ? "Iced Coffee" : "Hot Coffee";
            Console.WriteLine($"ITEM: {Name} ({type})");
            Console.WriteLine($"Price: ${Price} | Bean: {BeanType}");
        }

        // перевизначення віртуального методу
        public override void Prepare()
        {
            // викликаємо базову логіку (якщо треба), але тут напишемо свою
            Console.WriteLine($"[barista] grinding {BeanType} beans...");
            Console.WriteLine($"[barista] brewing espresso...");

            if (IsIced)
            {
                Console.WriteLine($"[barista] adding ice cubes...");
            }

            Console.WriteLine($"[barista] {Name} is ready!");
        }
    }

    // клас-нащадок 2: випічка
    public class Pastry : MenuItem
    {
        public int Calories { get; set; }
        public bool NeedsWarming { get; set; }

        public Pastry(string name, decimal price, int calories, bool needsWarming)
            : base(name, price)
        {
            Calories = calories;
            NeedsWarming = needsWarming;
        }

        // перевизначення абстрактного методу
        public override void DisplayInfo()
        {
            Console.WriteLine($"ITEM: {Name} (Bakery)");
            Console.WriteLine($"Price: ${Price} | Calories: {Calories} kcal");
        }

        // перевизначення віртуального методу
        public override void Prepare()
        {
            // спочатку виконується код з базового класу MenuItem
            base.Prepare();

            if (NeedsWarming)
            {
                Console.WriteLine($"[kitchen] warming up the {Name} in the oven...");
            }
            else
            {
                Console.WriteLine($"[kitchen] putting {Name} on a plate...");
            }

            Console.WriteLine($"[kitchen] {Name} is served.");
        }
    }

    class Program
    {
        static void Main()
        {
            // створення колекції об'єктів базового типу
            List<MenuItem> order = new List<MenuItem>
            {
                new Coffee("Latte", 4.50m, "Arabica", false),
                new Coffee("Cold Brew", 5.00m, "Blend", true),
                new Pastry("Croissant", 3.00m, 250, true),
                new Pastry("Cheesecake", 6.50m, 400, false)
            };

            Console.WriteLine("--- PROCESSING ORDER ---\n");

            // перебір колекції
            foreach (var item in order)
            {
                // 1. виводимо інфо
                item.DisplayInfo();

                // 2. готуємо
                item.Prepare();

                Console.WriteLine("------------------------");
            }
        }
    }
}