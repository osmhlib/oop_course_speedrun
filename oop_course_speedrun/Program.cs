// базовий клас - звичайний вантаж
public class Parcel
{
    public string SenderName { get; set; }
    public string RecipientAddress { get; set; }
    public double WeightKg { get; set; }
    public decimal BasePrice { get; set; }
}

// успадкований клас - крихкий вантаж
public class FragileParcel : Parcel
{
    public string MaterialType { get; set; }
    public bool HasInsurance { get; set; }
}

public class DeliveryCalculatorService
{
    // приватні змінні
    private readonly decimal _pricePerKg = 50.0m;
    private readonly decimal _fragileCoefficient = 1.5m; // +50% за крихкість
    private readonly decimal _insuranceCost = 200.0m;

    // метод, який працює з базовим класом
    public void PrintShippingLabel(Parcel parcel)
    {
        Console.WriteLine("--- Shipping Label ---");
        Console.WriteLine($"From: {parcel.SenderName}");
        Console.WriteLine($"To: {parcel.RecipientAddress}");
        Console.WriteLine($"Weight: {parcel.WeightKg} kg");

        // виклик іншого методу сервісу для розрахунку
        decimal cost = CalculateCost(parcel);
        Console.WriteLine($"Total Delivery Cost: {cost} UAH");
        Console.WriteLine("----------------------");
    }

    // логіка розрахунку ціни
    public decimal CalculateCost(Parcel parcel)
    {
        decimal total = parcel.BasePrice + (decimal)parcel.WeightKg * _pricePerKg;

        if (parcel is FragileParcel fragile)
        {
            total *= _fragileCoefficient;

            if (fragile.HasInsurance)
            {
                total += _insuranceCost;
            }
        }

        return total;
    }
}

class Program
{
    static void Main()
    {
        // створюємо сервіс
        DeliveryCalculatorService service = new DeliveryCalculatorService();

        // створюємо звичайну модель (батьківський клас)
        Parcel simpleBox = new Parcel
        {
            SenderName = "Glib",
            RecipientAddress = "Kyiv, Khreshchatyk str, 1",
            WeightKg = 2.0,
            BasePrice = 100
        };

        // створюємо модель з успадкуванням (дочірній клас)
        FragileParcel vase = new FragileParcel
        {
            SenderName = "City Museum",
            RecipientAddress = "Lviv, Rynok Square, 10",
            WeightKg = 5.0,
            BasePrice = 100,
            MaterialType = "Glass", // специфічне поле
            HasInsurance = true     // специфічне поле
        };

        // сервіс обробляє обидва об'єкти
        Console.WriteLine("Processing standard parcel:");
        service.PrintShippingLabel(simpleBox);

        Console.WriteLine("\nProcessing fragile parcel:");
        service.PrintShippingLabel(vase);
    }
}