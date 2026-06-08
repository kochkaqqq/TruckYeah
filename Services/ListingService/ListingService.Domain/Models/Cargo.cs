using System.ComponentModel.DataAnnotations;
using ListingService.Domain.Enums;

namespace ListingService.Domain.Models;

public class Cargo
{
    public Guid Id { get; set; }
    //public Guid UserId { get; set; } // ID владельца (грузовладельца)

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

    // === ПУБЛИКАЦИЯ И ПРОДВИЖЕНИЕ ===
    public ListingStatus Status { get; set; }                  // Статус объявления
    public DateTime CreatedAt { get; set; }                    // Дата создания
    public DateTime? PublishedAt { get; set; }                 // Дата публикации на бирже
    public bool BoostToTop { get; set; }                       // Поднять в ТОП (платно)
    public DateTime? BoostedUntil { get; set; }                // До когда действует поднятие в ТОП

    // === ШАБЛОНЫ И АРХИВ ===
    public bool IsTemplate { get; set; }                       // Сохранено как шаблон
    public string? TemplateName { get; set; }                  // Название шаблона
    public Guid? SourceListingId { get; set; }                 // ID исходного объявления (при копировании из архива)

    // Примечание (общее поле для текста)
    public string? Notes { get; set; }
}