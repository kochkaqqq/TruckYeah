namespace ListingService.Infrastructure.Entities;

public class TruckEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Основная информация
    public string Title { get; set; } = string.Empty;         // Название (напр. "Фура 20т")
    public string Description { get; set; } = string.Empty;   // Описание
    
    // Параметры транспорта
    public string BodyType { get; set; } = string.Empty;      // Тип кузова (тент, реф, борт и т.д.)
    public double? CapacityKg { get; set; }                   // Грузоподъемность (кг)
    public double? VolumeM3 { get; set; }                     // Объем кузова (м³)
    public double? LengthCm { get; set; }                     // Длина кузова (см)
    public double? WidthCm { get; set; }                      // Ширина кузова (см)
    public double? HeightCm { get; set; }                     // Высота кузова (см)
    
    // Местоположение
    public string CurrentLocation { get; set; } = string.Empty; // Где находится
    
    // Маршрут (где готова работать)
    public string RouteFrom { get; set; } = string.Empty;     // Откуда
    public string RouteTo { get; set; } = string.Empty;       // Куда
    public int? RadiusKm { get; set; }                        // Радиус работы (км)
    
    
    // Стоимость
    public decimal? PricePerKm { get; set; }                  // Цена за км
    public string Currency { get; set; } = "EUR";
    
    
    // Пользователь
    //public Guid UserId { get; set; }                          // ID владельца
    
    // Метаданные
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}