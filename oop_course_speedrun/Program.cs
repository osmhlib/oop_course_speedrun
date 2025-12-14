using System;
using System.Collections.Generic; // додано для використання списків

namespace LogisticsApp
{
    // частина 1: моделі даних
    // базовий клас
    public class Parcel
    {
        public string Id { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public double WeightKg { get; set; }
    }

    // тип 1: крихкий вантаж
    public class FragileParcel : Parcel
    {
        public string Material { get; set; }
        public bool HasInsurance { get; set; }
    }

    // тип 2: термінова посилка
    public class ExpressParcel : Parcel
    {
        public int DeliveryTimeHours { get; set; }
        public bool IsWeekendDelivery { get; set; }
    }

    // тип 3: міжнародна посилка
    public class InternationalParcel : Parcel
    {
        public string DestinationCountry { get; set; }
        public decimal CustomsFee { get; set; }
    }

    // частина 2: сервіси
    public class PricingService
    {
        private readonly decimal _baseRatePerKg = 10.0m;

        // коефіцієнти для різних типів
        private readonly decimal _fragileCoefficient = 1.5m;
        private readonly decimal _insuranceCost = 50.0m;
        private readonly decimal _expressMultiplier = 2.0m;
        private readonly decimal _internationalFlatFee = 40.0m;

        public decimal CalculatePrice(Parcel parcel)
        {
            decimal cost = (decimal)parcel.WeightKg * _baseRatePerKg;

            switch (parcel)
            {
                case FragileParcel fragile:
                    cost *= _fragileCoefficient;
                    if (fragile.HasInsurance) cost += _insuranceCost;
                    break;

                case ExpressParcel express:
                    cost *= _expressMultiplier;
                    if (express.IsWeekendDelivery) cost += 20.0m;
                    break;

                case InternationalParcel international:
                    cost += _internationalFlatFee;
                    cost += international.CustomsFee;
                    break;

                // звичайна посилка
                case Parcel _:
                    break;
            }

            return cost;
        }
    }

    public class LogisticsService
    {
        private readonly PricingService _pricingService;

        public LogisticsService()
        {
            _pricingService = new PricingService();
        }

        // метод тепер може приймати список посилок для зручності
        public void ProcessBatch(List<Parcel> parcels)
        {
            foreach (var p in parcels)
            {
                ProcessShipment(p);
            }
        }

        private void ProcessShipment(Parcel parcel)
        {
            decimal price = _pricingService.CalculatePrice(parcel);
            PrintLabel(parcel, price);
        }

        private void PrintLabel(Parcel parcel, decimal price)
        {
            Console.WriteLine("--- LABEL ---");
            Console.WriteLine($"ID: {parcel.Id} | Type: {parcel.GetType().Name}");
            Console.WriteLine($"Route: {parcel.Sender} -> {parcel.Receiver}");

            // специфічний вивід для кожного типу
            switch (parcel)
            {
                case FragileParcel f:
                    Console.WriteLine($"[!] HANDLE WITH CARE: {f.Material.ToUpper()}");
                    break;
                case ExpressParcel e:
                    Console.WriteLine($"[>>] EXPRESS: {e.DeliveryTimeHours}h");
                    break;
                case InternationalParcel i:
                    Console.WriteLine($"[?] CUSTOMS: {i.DestinationCountry}");
                    break;
            }

            Console.WriteLine($"Price: ${price}");
            Console.WriteLine("-------------\n");
        }
    }

    // частина 3: main
    class Program
    {
        static void Main()
        {
            LogisticsService logistics = new LogisticsService();

            // створюємо список різних посилок
            // ми можемо покласти різні класи в один список типу List<Parcel>
            var batch = new List<Parcel>
            {
                new Parcel
                {
                    Id = "STD-1", Sender = "Kyiv", Receiver = "Poltava", WeightKg = 5
                },
                new FragileParcel
                {
                    Id = "FRG-2", Sender = "Lviv", Receiver = "Kyiv", WeightKg = 2,
                    Material = "Glass", HasInsurance = true
                },
                new ExpressParcel
                {
                    Id = "EXP-3", Sender = "Dnipro", Receiver = "Odesa", WeightKg = 1,
                    DeliveryTimeHours = 12, IsWeekendDelivery = false
                },
                new InternationalParcel
                {
                    Id = "INT-4", Sender = "Uzhhorod", Receiver = "Prague", WeightKg = 10,
                    DestinationCountry = "Czechia", CustomsFee = 12.5m
                }
            };

            Console.WriteLine("Starting batch processing...\n");

            // відправляємо весь список на обробку
            logistics.ProcessBatch(batch);
        }
    }
}