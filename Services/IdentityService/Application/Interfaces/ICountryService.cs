using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICountryService
    {
        Task<Country> GetCountry(Guid countryId);
        Task<ICollection<Country>> GetAllCountries();
        Task<Country> AddCountry(string countryName);
        Task DeleteCiuntry(Guid countryId);
    }
}
