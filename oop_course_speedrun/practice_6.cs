using System;
using System.Threading.Tasks;

namespace CoffeeShopSimpleState
{
    // --- 1. СТАНИ ---
    public enum OrderState
    {
        New,        // Щойно замовили
        Cooking,    // Бариста готує
        Ready       // Можна забирати
    }

    // --- 2. КЛАС ЗАМОВЛЕННЯ (Він же Машина Станів) ---
    public class Order
    {
        public string ProductName { get; set; }

        // Поточний стан (змінювати може тільки сам клас)
        public OrderState CurrentState { get; private set; }

        public Order(string productName)
        {
            ProductName = productName;
            CurrentState = OrderState.New; // Початковий стан
        }

        // АСИНХРОННИЙ ПЕРЕХІД: New -> Cooking -> Ready
        public async Task PrepareAsync()
        {
            // Перевірка: чи можна починати готувати?
            if (CurrentState != OrderState.New)
            {
                Console.WriteLine($"[Помилка] Не можна готувати {ProductName}, бо статус: {CurrentState}");
                return;
            }

            // Перехід у стан "Готується"
            CurrentState = OrderState.Cooking;
            Console.WriteLine($"[Статус] {ProductName}: Готується... (чекайте 2 сек)");

            // Імітація роботи (асинхронно)
            await Task.Delay(2000);

            // Перехід у стан "Готово"
            CurrentState = OrderState.Ready;
            Console.WriteLine($"[Статус] {ProductName}: ГОТОВО! Можна забирати.");
        }

        // ПЕРЕХІД: Видача замовлення
        public void Serve()
        {
            if (CurrentState == OrderState.Ready)
            {
                Console.WriteLine($"[Фінал] Клієнт забрав {ProductName}. Смачного!");
            }
            else
            {
                Console.WriteLine($"[Помилка] Клієнт хоче забрати {ProductName}, але воно ще не готове!");
            }
        }
    }

    // --- 3. ЗАПУСК ---
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("--- ПРОСТА МАШИНА СТАНІВ ---");

            // 1. Створюємо замовлення
            Order myCoffee = new Order("Лате");
            Console.WriteLine($"Поточний стан: {myCoffee.CurrentState}"); // New

            // 2. Спроба забрати одразу (Помилка логіки)
            myCoffee.Serve();

            // 3. Запускаємо процес приготування
            // await означає, що ми чекаємо завершення переходу станів
            await myCoffee.PrepareAsync();

            // 4. Тепер статус змінився, пробуємо забрати знову
            Console.WriteLine($"Поточний стан: {myCoffee.CurrentState}"); // Ready
            myCoffee.Serve();

            Console.ReadLine();
        }
    }
}