using ListingService.Domain.Enums;

namespace ListingService.WebApi.Contracts.Truck;

public class TruckRequest
{
    // === ОСНОВНАЯ ИНФОРМАЦИЯ ===
    public string Title { get; set; } = string.Empty;          // Заголовок объявления (напр. "Фура 20т")
    public string? Description { get; set; }                   // Дополнительное описание

    // === МАРШРУТ / ЗОНА РАБОТЫ ===
    public string RouteFrom { get; set; } = string.Empty;      // Откуда (обязательное)
    public string RouteTo { get; set; } = string.Empty;        // Куда (обязательное)

    // === ПАРАМЕТРЫ ТРАНСПОРТА ===
    public double CapacityTons { get; set; }                   // Грузоподъёмность в тоннах (обязательное)
    public double VolumeM3 { get; set; }                       // Объём кузова в м³ (обязательное)
    public string BodyType { get; set; } = string.Empty;       // Тип кузова (обязательное)
    public LoadingType LoadingType { get; set; }               // Тип загрузки

    // === ДОСТУПНОСТЬ ===
    public DateTime AvailableFrom { get; set; }                // Дата готовности (обязательное)

    // === СТАВКА ===
    public decimal? Price { get; set; }                        // Ставка за перевозку (обязательное)

    // === ФИНАНСЫ ===
    public PaymentType PaymentType { get; set; }               // Тип оплаты: наличные / с НДС / без НДС
    public bool AllowBargaining { get; set; }                  // Возможность торга
    public decimal? PrepaymentPercent { get; set; }            // Требуемый процент предоплаты
}