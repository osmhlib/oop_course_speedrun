using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // обов'язково для LINQ!

namespace CoffeeShopLinq
{
    // ---------------------------------------------------------
    // ЧАСТИНА 1: Моделі (Ті самі, що в ПЗ4)
    // ---------------------------------------------------------

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

        // для сортування (icomparable)
        public int CompareTo(MenuItem other)
        {
            if (other == null) return 1;
            int priceComp = Price.CompareTo(other.Price);
            if (priceComp != 0) return priceComp;
            return Name.CompareTo(other.Name);
        }

        // для клонування (icloneable)
        public object Clone() => this.MemberwiseClone();

        public override string ToString() => $"{Name} - ${Price:F2}";
    }

    public class Coffee : MenuItem
    {
        public string BeanType { get; set; }
        public Coffee(string name, decimal price, string beanType) : base(name, price)
        {
            BeanType = beanType;
        }
        public override void DisplayInfo() => Console.WriteLine($"Coffee: {Name}");
    }

    public class Pastry : MenuItem
    {
        public int Calories { get; set; }
        public Pastry(string name, decimal price, int calories) : base(name, price)
        {
            Calories = calories;
        }
        public override void DisplayInfo() => Console.WriteLine($"Pastry: {Name}");
    }

    // ---------------------------------------------------------
    // ЧАСТИНА 2: Колекція з ітераторами (yield)
    // ---------------------------------------------------------

    // реалізуємо ienumerable, щоб по цьому класу можна було бігати foreach
    public class MenuCollection : IEnumerable<MenuItem>
    {
        private List<MenuItem> _items = new List<MenuItem>();

        public void Add(MenuItem item) => _items.Add(item);

        // 1. стандартний ітератор (реалізація інтерфейсу)
        public IEnumerator<MenuItem> GetEnumerator()
        {
            foreach (var item in _items)
            {
                // yield return повертає елемент і "заморожує" метод до наступного кроку циклу
                yield return item;
            }
        }

        // обов'язкова заглушка для старого інтерфейсу (можна ігнорувати)
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // 2. власний ітератор з умовою (повертає тільки дешеві товари)
        // це дозволяє перебрати не все меню, а тільки частину, не створюючи новий list
        public IEnumerable<MenuItem> GetCheapItems(decimal limit)
        {
            foreach (var item in _items)
            {
                if (item.Price < limit)
                {
                    yield return item;
                }
            }
        }

        // 3. ітератор, який повертає тільки випічку
        public IEnumerable<MenuItem> GetOnlyFood()
        {
            foreach (var item in _items)
            {
                if (item is Pastry)
                {
                    yield return item;
                }
            }
        }
    }

    // ---------------------------------------------------------
    // ЧАСТИНА 3: Виконання (LINQ та Iterators)
    // ---------------------------------------------------------

    class Program
    {
        static void Main()
        {
            // ініціалізація нашої колекції
            MenuCollection menu = new MenuCollection();
            menu.Add(new Coffee("Latte", 4.50m, "Arabica"));
            menu.Add(new Coffee("Espresso", 2.50m, "Robusta"));
            menu.Add(new Pastry("Croissant", 3.00m, 250));
            menu.Add(new Pastry("Cake", 7.00m, 500));
            menu.Add(new Pastry("Donut", 1.50m, 300));
            menu.Add(new Coffee("Cappuccino", 4.00m, "Arabica"));

            Console.WriteLine("--- 1. CUSTOM ITERATOR (yield) ---");
            Console.WriteLine("Items cheaper than $4.00:");

            // тут працює метод GetCheapItems з yield
            // він не віддає весь список одразу, а по одному елементу
            foreach (var item in menu.GetCheapItems(4.00m))
            {
                Console.WriteLine($" -> {item}");
            }


            Console.WriteLine("\n--- 2. LINQ: FILTERING (Where) ---");
            // запит: знайти всі товари, де ціна > 3 AND це кава
            var expensiveCoffee = menu
                .Where(x => x.Price > 3.00m && x is Coffee);

            foreach (var item in expensiveCoffee)
            {
                Console.WriteLine($" -> {item}");
            }


            Console.WriteLine("\n--- 3. LINQ: PROJECTION (Select) ---");
            // запит: нам не потрібні об'єкти, нам потрібні тільки їх назви (string)
            // ми перетворюємо (проектуємо) list<menuitem> у list<string>
            var namesOnly = menu.Select(x => x.Name.ToUpper());

            Console.WriteLine(string.Join(", ", namesOnly));


            Console.WriteLine("\n--- 4. LINQ: ORDERING (OrderBy) ---");
            // запит: відсортувати за спаданням ціни
            var sortedMenu = menu.OrderByDescending(x => x.Price);

            foreach (var item in sortedMenu)
            {
                Console.WriteLine($" -> {item}");
            }


            Console.WriteLine("\n--- 5. LINQ: AGGREGATION (Count, Sum, Average) ---");

            // кількість елементів
            int count = menu.Count();

            // сума всіх цін
            decimal totalValue = menu.Sum(x => x.Price);

            // середня ціна
            decimal averagePrice = menu.Average(x => x.Price);

            // найдорожчий товар (знайти максимум)
            decimal maxPrice = menu.Max(x => x.Price);

            Console.WriteLine($"Total items:   {count}");
            Console.WriteLine($"Total value:   ${totalValue}");
            Console.WriteLine($"Average price: ${averagePrice:F2}");
            Console.WriteLine($"Max price:     ${maxPrice}");

            Console.ReadLine();
        }
    }
}