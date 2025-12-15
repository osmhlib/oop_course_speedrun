using System;
using System.Collections.Generic;

namespace CoffeeShopEvents
{
    // --- ДЕЛЕГАТИ (З ПЗ3) ---
    public delegate void ItemActionDelegate(MenuItem item);

    // --- НОВЕ: КЛАС АРГУМЕНТІВ ПОДІЇ ---
    // цей клас служить "конвертом", у якому ми передаємо дані про подію
    public class MenuEventArgs : EventArgs
    {
        public MenuItem Item { get; }
        public string Message { get; }

        public MenuEventArgs(MenuItem item, string message)
        {
            Item = item;
            Message = message;
        }
    }

    // --- МОДЕЛІ (З ПЗ2/3) ---
    public abstract class MenuItem
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public MenuItem(string name, decimal price)
        {
            Name = name;
            Price = price;
        }
        public abstract void DisplayInfo();
    }

    public class Coffee : MenuItem
    {
        public string BeanType { get; set; }
        public Coffee(string name, decimal price, string beanType) : base(name, price)
        {
            BeanType = beanType;
        }
        public override void DisplayInfo() => Console.WriteLine($"[Coffee] {Name} (${Price})");
    }

    public class Pastry : MenuItem
    {
        public int Calories { get; set; }
        public Pastry(string name, decimal price, int calories) : base(name, price)
        {
            Calories = calories;
        }
        public override void DisplayInfo() => Console.WriteLine($"[Pastry] {Name} (${Price})");
    }

    // --- МЕНЕДЖЕР (ВИДАВЕЦЬ ПОДІЙ / PUBLISHER) ---

    public class MenuManager
    {
        private List<MenuItem> _items = new List<MenuItem>();

        // 1. Оголошення подій
        // eventhandler - це стандартний делегат для подій у c#
        public event EventHandler<MenuEventArgs> OnItemAdded;
        public event EventHandler<MenuEventArgs> OnItemRemoved;

        // подія для небезпечної операції (для тесту помилок)
        public event EventHandler<MenuEventArgs> OnCriticalChange;

        public void AddItem(MenuItem item)
        {
            _items.Add(item);

            // 2. Генерація події (тригер)
            // перевіряємо, чи є підписники (?.), і викликаємо invoke
            OnItemAdded?.Invoke(this, new MenuEventArgs(item, "New item arrived!"));
        }

        public void RemoveItem(string itemName)
        {
            // використовуємо делегат predicate для пошуку
            var itemToRemove = _items.Find(x => x.Name == itemName);

            if (itemToRemove != null)
            {
                _items.Remove(itemToRemove);

                // генерація події видалення
                OnItemRemoved?.Invoke(this, new MenuEventArgs(itemToRemove, "Item removed from menu."));
            }
        }

        // метод, що викликає подію з обробкою виключень
        public void TriggerEmergencyUpdate()
        {
            Console.WriteLine("\n[SYSTEM] Triggering critical update...");

            // перевіряємо, чи є підписники
            if (OnCriticalChange != null)
            {
                // отримуємо список усіх підписників (методів)
                var subscribers = OnCriticalChange.GetInvocationList();

                foreach (var sub in subscribers)
                {
                    try
                    {
                        // намагаємось виконати кожен метод підписника окремо
                        sub.DynamicInvoke(this, new MenuEventArgs(null, "Critical System Update"));
                    }
                    catch (Exception ex)
                    {
                        // якщо один підписник "впав", ми ловимо помилку і працюємо далі
                        // важливо: innerException, бо dynamicinvoke загортає помилку
                        Console.WriteLine($"[ERROR HANDLED] One subscriber failed: {ex.InnerException?.Message}");
                    }
                }
            }
        }

        // --- Методи з ПЗ3 (Делегати) ---
        public void ProcessAll(ItemActionDelegate action)
        {
            foreach (var item in _items) action(item);
        }
    }

    // --- НОВЕ: ПІДПИСНИК (LISTENER) ---
    // клас, який реагує на події
    public class KitchenMonitor
    {
        public void OnNewOrder(object sender, MenuEventArgs e)
        {
            Console.WriteLine($"[KITCHEN DISPLAY] + ALERT: {e.Message} -> {e.Item.Name} (${e.Item.Price})");
        }
    }

    // --- ГОЛОВНА ПРОГРАМА ---

    class Program
    {
        static void Main()
        {
            MenuManager manager = new MenuManager();
            KitchenMonitor monitor = new KitchenMonitor();

            // 1. ПІДПИСКА НА ПОДІЇ (SUBSCRIPTION)
            // ми кажемо: коли в manager станеться onitemadded, виклич метод у monitor
            manager.OnItemAdded += monitor.OnNewOrder;

            // підписуємось анонімним методом (через лямбду) на видалення
            manager.OnItemRemoved += (sender, args) =>
            {
                Console.WriteLine($"[ADMIN LOG] - WARNING: {args.Message} ({args.Item.Name})");
            };

            // 2. ДЕМОНСТРАЦІЯ РОБОТИ
            Console.WriteLine("--- EVENT DEMO: Adding Items ---");
            // при додаванні спрацює подія onitemadded -> kitchen monitor
            manager.AddItem(new Coffee("Latte", 4.50m, "Arabica"));
            manager.AddItem(new Pastry("Croissant", 3.00m, 250));

            Console.WriteLine("\n--- EVENT DEMO: Removing Items ---");
            // при видаленні спрацює подія onitemremoved -> admin log
            manager.RemoveItem("Latte");

            // 3. ОБРОБКА ВИКЛЮЧЕНЬ У ПОДІЯХ
            Console.WriteLine("\n--- EXCEPTION HANDLING DEMO ---");

            // додаємо хорошого підписника
            manager.OnCriticalChange += (sender, e) => Console.WriteLine("[Good Subscriber] System update received.");

            // додаємо "поганого" підписника, який кидає помилку
            manager.OnCriticalChange += (sender, e) =>
            {
                throw new InvalidOperationException("I am a buggy plugin!");
            };

            // додаємо ще одного хорошого (він має спрацювати, незважаючи на помилку попереднього)
            manager.OnCriticalChange += (sender, e) => Console.WriteLine("[Good Subscriber 2] I am still working!");

            // запускаємо
            manager.TriggerEmergencyUpdate();

            Console.WriteLine("\n--- PROGRAM FINISHED STABLE ---");
            Console.ReadLine();
        }
    }
}