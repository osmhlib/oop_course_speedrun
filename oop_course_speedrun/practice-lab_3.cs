using System;
using System.Collections.Generic;

namespace CoffeeShopDelegates
{
    // --- ОГОЛОШЕННЯ ДЕЛЕГАТА ---
    // тип даних, який посилається на метод.
    // Метод повинен приймати (MenuItem) і повертати void.
    public delegate void ItemActionDelegate(MenuItem item);

    // Класи моделей з ПЗ2/ЛБ2
    public abstract class MenuItem
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public MenuItem(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        // Абстрактний метод для виводу
        public abstract void DisplayInfo();
    }

    public class Coffee : MenuItem
    {
        public string BeanType { get; set; }

        public Coffee(string name, decimal price, string beanType)
            : base(name, price)
        {
            BeanType = beanType;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[Coffee] {Name} (${Price}) - Bean: {BeanType}");
        }
    }

    public class Pastry : MenuItem
    {
        public int Calories { get; set; }

        public Pastry(string name, decimal price, int calories)
            : base(name, price)
        {
            Calories = calories;
        }

        public override void DisplayInfo()
        {
            Console.WriteLine($"[Pastry] {Name} (${Price}) - {Calories} kcal");
        }
    }

    // ЧАСТИНА 2: Клас для роботи з Делегатами

    public class MenuManager
    {
        private List<MenuItem> _items = new List<MenuItem>();

        public void AddItem(MenuItem item)
        {
            _items.Add(item);
        }

        // Метод 1: Виконує дію (делегат) для кожного елемента
        public void ProcessAll(ItemActionDelegate action)
        {
            Console.WriteLine(">> Processing all items with delegate...");
            foreach (var item in _items)
            {
                // Виклик методу, на який вказує делегат
                action(item);
            }
            Console.WriteLine("--------------------------------------");
        }

        // Метод 2: Фільтрація за допомогою вбудованого делегата Predicate
        // Predicate<T> повертає true/false
        public void ShowFiltered(Predicate<MenuItem> condition)
        {
            Console.WriteLine(">> Filtered items:");
            foreach (var item in _items)
            {
                if (condition(item)) // Якщо умова виконується
                {
                    item.DisplayInfo();
                }
            }
            Console.WriteLine("--------------------------------------");
        }

        // Метод 3: Підрахунок за допомогою вбудованого делегата Func
        // Func<T, TResult> приймає T і повертає TResult
        public decimal CalculateSum(Func<MenuItem, decimal> valueSelector)
        {
            decimal sum = 0;
            foreach (var item in _items)
            {
                sum += valueSelector(item);
            }
            return sum;
        }
    }

    // ЧАСТИНА 3: Виконання (Main)

    class Program
    {
        // Метод, який підходить під сигнатуру ItemActionDelegate
        static void ApplyHolidayDiscount(MenuItem item)
        {
            if (item.Price > 4.00m)
            {
                Console.WriteLine($"DISCOUNT APPLIED: {item.Name} is now ${item.Price * 0.9m:F2}");
            }
            else
            {
                Console.WriteLine($"Standard price:   {item.Name}");
            }
        }

        // Ще один метод для делегата
        static void CheckAvailability(MenuItem item)
        {
            Console.WriteLine($"Checking stock for {item.Name}... OK.");
        }

        static void Main()
        {
            MenuManager manager = new MenuManager();

            // Заповнення даними
            manager.AddItem(new Coffee("Latte", 4.50m, "Arabica"));
            manager.AddItem(new Coffee("Espresso", 2.50m, "Robusta"));
            manager.AddItem(new Pastry("Croissant", 3.00m, 250));
            manager.AddItem(new Pastry("Cheesecake", 6.50m, 500));

            Console.WriteLine("--- ПРАКТИЧНА РОБОТА 3: ДЕЛЕГАТИ ---\n");

            // 1. Використання власного делегата (ItemActionDelegate)
            // Передаємо метод CheckAvailability
            manager.ProcessAll(CheckAvailability);

            // Передаємо інший метод ApplyHolidayDiscount
            manager.ProcessAll(ApplyHolidayDiscount);


            // 2. Використання анонімного методу (через лямбда-вираз)
            // Створюємо дію "на льоту", не оголошуючи окремий метод
            manager.ProcessAll(item =>
            {
                Console.WriteLine($"Custom Log: Item {item.Name} loaded.");
            });


            // 3. Використання Predicate для фільтрації
            // Умова: показати товари дорожчі за 4 долари
            manager.ShowFiltered(item => item.Price > 4.00m);

            // Умова: показати тільки випічку
            manager.ShowFiltered(item => item is Pastry);


            // 4. Використання Func для підрахунку
            // Рахуємо загальну вартість меню
            decimal totalPrice = manager.CalculateSum(item => item.Price);
            Console.WriteLine($"TOTAL MENU VALUE: ${totalPrice}");

            // Рахуємо загальну кількість калорій (тільки для випічки)
            decimal totalCalories = manager.CalculateSum(item =>
            {
                if (item is Pastry p) return p.Calories;
                return 0; // Для кави 0 калорій
            });
            Console.WriteLine($"TOTAL CALORIES (Pastry): {totalCalories}");

            Console.ReadLine();
        }
    }
}