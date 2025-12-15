using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoffeeShopThreading
{
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

    public class CoffeeShop
    {
        private decimal _totalRevenue = 0;
        private int _cupsInStock = 10;
        private readonly object _lockObject = new object();

        public decimal GetRevenue() => _totalRevenue;

        public void ProcessOrder(MenuItem item, int baristaId)
        {
            Console.WriteLine($"[Barista {baristaId}] started making {item.Name}...");

            // --- ЗМІНА ТУТ ---
            // Збільшили час: від 3000 мс (3 сек) до 6000 мс (6 сек)
            Thread.Sleep(new Random().Next(3000, 6000));

            Console.WriteLine($"[Barista {baristaId}] finished {item.Name}. Walking to register...");

            lock (_lockObject)
            {
                if (_cupsInStock > 0)
                {
                    _cupsInStock--;
                    _totalRevenue += item.Price;
                    Console.WriteLine($"   --> [REGISTER] +${item.Price} (Stock: {_cupsInStock}). Total: ${_totalRevenue}");
                }
                else
                {
                    Console.WriteLine($"   --> [REGISTER] FAIL! No cups left for {item.Name}!");
                }
            }
        }
    }

    class Program
    {
        static void Main()
        {
            CoffeeShop shop = new CoffeeShop();
            List<Task> tasks = new List<Task>();

            Console.WriteLine("--- MORNING RUSH STARTED (Long wait demo) ---\n");

            for (int i = 1; i <= 15; i++)
            {
                int orderId = i;
                MenuItem item = new Coffee($"Latte #{orderId}", 5.00m);

                Task task = Task.Run(() =>
                {
                    int threadId = Thread.CurrentThread.ManagedThreadId;
                    shop.ProcessOrder(item, threadId);
                });

                tasks.Add(task);
            }

            Console.WriteLine("All orders accepted. Baristas are working (Wait 3-6 seconds)...\n");

            // Головний потік чекає тут
            Task.WaitAll(tasks.ToArray());

            Console.WriteLine("\n--------------------------------");
            Console.WriteLine("--- SHIFT FINISHED ---");
            Console.WriteLine($"Final Revenue: ${shop.GetRevenue()}");
            Console.WriteLine($"Expected Max Revenue: $50.00");

            Console.ReadLine();
        }
    }
}