namespace ListingService.Domain.Models;

public class Cargo
{
    public Cargo(Guid id, string title, string description, 
        double? weightKg, double? volumeM3, double? lengthCm, 
        double? widthCm, double? heightCm, string cargoType, 
        string? routeFrom, string? routeTo, double? distanceKm, 
        DateTime loadDate, decimal? price, DateTime createdAt)
    {
        Id = id;
        Title = title;
        Description = description;
        WeightKg = weightKg;
        VolumeM3 = volumeM3;
        LengthCm = lengthCm;
        WidthCm = widthCm;
        HeightCm = heightCm;
        CargoType = cargoType;
        RouteFrom = routeFrom;
        RouteTo = routeTo;
        DistanceKm = distanceKm;
        LoadDate = loadDate;
        Price = price;
        CreatedAt = createdAt;
    }
    
    public Guid Id { get; set; }
    
    // Основная информация
    public string Title { get; set; }        // Название груза
    public string Description { get; set; }  // Описание
    
    // Параметры груза
    public double? WeightKg { get; set; }                     // Вес (кг)
    public double? VolumeM3 { get; set; }                     // Объем (м³)
    public double? LengthCm { get; set; }                     // Длина (см)
    public double? WidthCm { get; set; }                      // Ширина (см)
    public double? HeightCm { get; set; }                     // Высота (см)
    public string CargoType { get; set; }     // Тип груза (тент, реф и т.д.)
    
    // Маршрут
    public string RouteFrom { get; set; }     // Откуда
    public string RouteTo { get; set; }        // Куда
    public double? DistanceKm { get; set; }                   // Расстояние (км)
    
    // Даты
    public DateTime LoadDate { get; set; }                    // Дата загрузки
    
    // Стоимость
    public decimal? Price { get; set; }                       // Цена
    
    // Пользователь
   // public Guid UserId { get; set; }                          // ID владельца
    
    // Метаданные
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}