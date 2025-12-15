using System;
using System.Collections.Generic;
using System.Threading;      // CancellationToken
using System.Threading.Tasks; // Task, async, await

namespace CoffeeShopAsync
{
    // --- МОДЕЛІ ---
    public abstract class MenuItem
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public MenuItem(string name, decimal price) { Name = name; Price = price; }
    }

    public class Coffee : MenuItem
    {
        public Coffee(string name, decimal price) : base(name, price) { }
    }

    // --- АСИНХРОННА КАВ'ЯРНЯ ---
    public class CoffeeShop
    {
        private decimal _totalRevenue = 0;
        private readonly object _lockObject = new object();

        // Асинхронний метод
        // 1. Позначений словом async
        // 2. Повертає Task
        // 3. Приймає CancellationToken
        public async Task ProcessOrderAsync(MenuItem item, CancellationToken token)
        {
            // Перевірка на старті: чи не скасували замовлення ще до початку?
            if (token.IsCancellationRequested)
            {
                Console.WriteLine($"[System] Order {item.Name} was cancelled before start.");
                return;
            }

            Console.WriteLine($"[Barista] Started making {item.Name}...");

            try
            {
                // Імітація роботи (1-3 секунди)
                // await Task.Delay НЕ блокує потік, він звільняє його для інших задач
                // Ми передаємо token всередину Delay. Якщо token спрацює, Delay викине помилку миттєво.
                await Task.Delay(new Random().Next(1000, 3000), token);

                // Якщо ми дійшли сюди, значить Delay завершився успішно і скасування не було.
                // Заходимо в критичну секцію (синхронізація все ще потрібна для спільних змінних!)
                lock (_lockObject)
                {
                    _totalRevenue += item.Price;
                    Console.WriteLine($"   --> [DONE] {item.Name} served! (+${item.Price})");
                }
            }
            catch (TaskCanceledException) // або OperationCanceledException
            {
                // Цей блок спрацює, якщо ми скасували завдання під час очікування (Delay)
                Console.WriteLine($"[X] CANCELLATION: Shop closed! {item.Name} thrown away.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }

        public decimal GetRevenue() => _totalRevenue;
    }

    class Program
    {
        // Main тепер теж async Task, щоб ми могли використовувати await всередині
        static async Task Main()
        {
            CoffeeShop shop = new CoffeeShop();

            // CancellationTokenSource - це "пульт", на якому є червона кнопка скасування
            CancellationTokenSource cts = new CancellationTokenSource();

            Console.WriteLine("--- COFFEE SHOP OPEN (Async Mode) ---");
            Console.WriteLine("--- NOTE: The shop will close automatically in 2 seconds! ---\n");

            List<Task> tasks = new List<Task>();

            // Створюємо 10 замовлень
            for (int i = 1; i <= 10; i++)
            {
                MenuItem item = new Coffee($"Latte #{i}", 5.00m);

                // Запускаємо асинхронну задачу
                // Передаємо cts.Token - це "провід", що йде від пульта до баристи
                Task orderTask = shop.ProcessOrderAsync(item, cts.Token);

                tasks.Add(orderTask);
            }

            try
            {
                // Встановлюємо таймер на пульті: натиснути "Скасувати" через 2000 мс (2 сек)
                cts.CancelAfter(2000);

                // Чекаємо завершення всіх задач (і успішних, і скасованих)
                // Task.WhenAll не блокує інтерфейс, він асинхронно чекає
                await Task.WhenAll(tasks);
            }
            catch (Exception)
            {
                // Task.WhenAll може викинути виключення, якщо одна з задач впала,
                // але ми вже обробили помилки всередині ProcessOrderAsync, тому тут просто глушимо.
            }

            Console.WriteLine("\n--------------------------------");
            Console.WriteLine("--- SHOP CLOSED ---");
            Console.WriteLine($"Final Revenue: ${shop.GetRevenue()}");

            // Статистика завдань
            int completed = 0;
            int cancelled = 0;
            foreach (var t in tasks)
            {
                if (t.Status == TaskStatus.RanToCompletion) completed++;
                else if (t.Status == TaskStatus.Canceled || t.Status == TaskStatus.Faulted) cancelled++;
                // Примітка: через try-catch всередині методу, статус може бути RanToCompletion навіть при скасуванні,
                // залежить від того, чи ми кидаємо помилку далі. 
                // У нашому коді ми "ковтаємо" помилку (catch TaskCanceledException), тому Tasks технічно завершуються успішно.
                // Але ми бачимо результат в консолі.
            }

            Console.WriteLine($"Detailed check: All {tasks.Count} tasks processed (some served, some interrupted).");
            Console.ReadLine();
        }
    }
}