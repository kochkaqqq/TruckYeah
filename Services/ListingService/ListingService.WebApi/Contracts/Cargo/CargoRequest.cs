using ListingService.Domain.Enums;
using ListingService.Domain.Models;

namespace ListingService.WebApi.Contracts.Cargo;

public class CargoRequest
{

    // === ОСНОВНАЯ ИНФОРМАЦИЯ ===
    public string Title { get; set; } = string.Empty;          // Заголовок объявления (для списка)
    public string CargoName { get; set; } = string.Empty;      // Наименование груза (обязательное)

    // === МАРШРУТ ===
    public string RouteFrom { get; set; } = string.Empty;      // Откуда (обязательное)
    public string RouteTo { get; set; } = string.Empty;        // Куда (обязательное)
    public virtual ICollection<RoutePoint> RoutePoints { get; set; } = new List<RoutePoint>(); // Промежуточные точки

    // === ДАТЫ ===
    public DateTime LoadDateTime { get; set; }                 // Дата и время загрузки (обязательное)
    public DateTime UnloadDateTime { get; set; }              // Дата и время разгрузки

    // === ПАРАМЕТРЫ ГРУЗА ===
    public double WeightTons { get; set; }                     // Вес в тоннах (обязательное)
    public double VolumeM3 { get; set; }                       // Объём в м³ (обязательное)
    public string BodyTypeRequired { get; set; } = string.Empty; // Тип кузова (обязательное)
    public LoadingType LoadingType { get; set; }               // Тип загрузки (обязательное)

    // === ГАБАРИТЫ И УПАКОВКА ===
    public double? LengthCm { get; set; }                      // Длина (см)
    public double? WidthCm { get; set; }                       // Ширина (см)
    public double? HeightCm { get; set; }                      // Высота (см)
    public int? PalletsCount { get; set; }                     // Количество палет
    public string? PackagingType { get; set; }                 // Тип упаковки

    // === ДОКУМЕНТЫ И ТРЕБОВАНИЯ ===
    public bool RequiresCMR { get; set; }                      // Требуется CMR
    public bool RequiresTIR { get; set; }                      // Требуется TIR
    public bool IsADR { get; set; }                            // Опасный груз (ADR)

    // === ФИНАНСЫ ===
    public PaymentType PaymentType { get; set; }               // Тип оплаты: наличные / с НДС / без НДС
    public bool AllowBargaining { get; set; }                  // Возможность торга
    public decimal? PrepaymentPercent { get; set; }            // Процент предоплаты (0-100)
    
    public string? Notes { get; set; }
}