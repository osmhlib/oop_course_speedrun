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
            // валідація: ціна не може бути від'ємною
            // це приклад захисту даних на етапі створення
            if (price < 0)
            {
                throw new ArgumentException($"price cannot be negative for '{name}'");
            }
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

        // --- нові методи для лабораторної 2 ---

        // віртуальний метод подачі
        public virtual void Serve()
        {
            Console.WriteLine($"[waiter] serving {Name} to the table.");
        }

        // віртуальний метод знижки з валідацією
        public virtual void ApplyDiscount(decimal percentage)
        {
            // перевірка на коректність відсотка
            if (percentage < 0 || percentage > 50)
            {
                throw new ArgumentOutOfRangeException(nameof(percentage), "discount must be between 0% and 50%");
            }

            decimal discountAmount = Price * (percentage / 100);
            Price -= discountAmount;
            Console.WriteLine($"[promo] discount {percentage}% applied. new price: ${Price:F2}");
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

        // перевизначення методу подачі (нове)
        public override void Serve()
        {
            string container = IsIced ? "transparent plastic cup" : "ceramic mug";
            Console.WriteLine($"[waiter] placing {Name} in a {container}.");
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
            // спочатку виконується код з базового класу menuitem
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

        // перевизначення знижки (нове)
        // для випічки дозволяємо більшу знижку, ніж стандартні 50%
        public override void ApplyDiscount(decimal percentage)
        {
            if (percentage < 0 || percentage > 90)
            {
                throw new ArgumentOutOfRangeException("pastry discount can be up to 90%");
            }

            // логіка розрахунку така сама, тому копіюємо формулу
            decimal discountAmount = Price * (percentage / 100);
            Price -= discountAmount;
            Console.WriteLine($"[promo] pastry sale! {percentage}% off. new price: ${Price:F2}");
        }
    }

    // --- новий клас для лабораторної 2 ---

    // клас-нащадок 3: смузі
    public class Smoothie : MenuItem
    {
        public List<string> Ingredients { get; set; }

        public Smoothie(string name, decimal price, List<string> ingredients)
            : base(name, price)
        {
            Ingredients = ingredients;
        }

        public override void DisplayInfo()
        {
            string list = Ingredients != null ? string.Join(", ", Ingredients) : "None";
            Console.WriteLine($"ITEM: {Name} (Smoothie)");
            Console.WriteLine($"Price: ${Price} | Mix: {list}");
        }

        public override void Prepare()
        {
            // перевірка: не можна зробити смузі без інгредієнтів
            if (Ingredients == null || Ingredients.Count == 0)
            {
                throw new InvalidOperationException("cannot blend smoothie without ingredients");
            }
            Console.WriteLine($"[blender] mixing ingredients: {string.Join(" + ", Ingredients)}...");
        }

        public override void Serve()
        {
            Console.WriteLine($"[waiter] serving {Name} with a thick straw.");
        }
    }

    class Program
    {
        static void Main()
        {
            List<MenuItem> order = new List<MenuItem>();

            Console.WriteLine("--- CREATING ORDER ---\n");

            // блок try-catch для безпечного додавання товарів
            try
            {
                order.Add(new Coffee("Latte", 4.50m, "Arabica", false));
                order.Add(new Pastry("Croissant", 3.00m, 250, true));

                // додавання нового типу товару
                order.Add(new Smoothie("Berry Mix", 6.00m, new List<string> { "Strawberry", "Banana", "Milk" }));

                // спроба створити товар з помилкою (від'ємна ціна)
                // розкоментуй рядок нижче, щоб перевірити роботу захисту
                // order.Add(new Coffee("Error Coffee", -5.00m, "None", false));

                // додавання товару, який викличе помилку пізніше (при приготуванні)
                order.Add(new Smoothie("Empty Glass", 5.00m, new List<string>()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] failed to create item: {ex.Message}");
            }

            Console.WriteLine("\n--- PROCESSING ORDER ---\n");

            // перебір колекції
            foreach (var item in order)
            {
                Console.WriteLine($"> Processing item: {item.Name}");

                // окремий блок try-catch для кожного замовлення
                // щоб помилка в одному товарі не зупиняла всю чергу
                try
                {
                    // 1. виводимо інфо
                    item.DisplayInfo();

                    // 2. готуємо (тут може виникнути помилка для Empty Glass)
                    item.Prepare();

                    // 3. застосовуємо знижку (нове)
                    // спробуємо дати 60% знижки
                    // для кави це викличе помилку (ліміт 50%)
                    // для випічки це спрацює (ліміт 90%)
                    if (item is Pastry)
                    {
                        item.ApplyDiscount(60);
                    }
                    else
                    {
                        // даємо безпечну знижку для інших
                        item.ApplyDiscount(10);
                    }

                    // 4. подаємо (нове)
                    item.Serve();

                    Console.WriteLine("-> Status: Completed");
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    // помилка зі знижкою
                    Console.WriteLine($"[!] DISCOUNT ERROR: {ex.Message}");
                }
                catch (InvalidOperationException ex)
                {
                    // помилка при приготуванні (логічна)
                    Console.WriteLine($"[!] KITCHEN ERROR: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // будь-які інші помилки
                    Console.WriteLine($"[!] UNKNOWN ERROR: {ex.Message}");
                }
                finally
                {
                    Console.WriteLine("------------------------");
                }
            }
        }
    }
}