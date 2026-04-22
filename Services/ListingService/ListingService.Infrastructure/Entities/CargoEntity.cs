namespace ListingService.Infrastructure.Entities;

public class CargoEntity
{
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