using Application.Interfaces;
using Application.Shared.Exceptions;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly IDbContext _dbContext;

        public CountryService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Country> AddCountry(string countryName)
        {
            var country = new Country
            {
                Name = new CountryName(countryName)
            };

            await _dbContext.Countries.AddAsync(country);
            await _dbContext.SaveChangesAsync();

            return country;
        }

        public async Task DeleteCiuntry(Guid countryId)
        {
            var entity = await _dbContext.Countries.FirstOrDefaultAsync(c => c.Id == countryId)
                ?? throw new EntityNotFoundException(nameof(Country), countryId.ToString());

            _dbContext.Countries.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ICollection<Country>> GetAllCountries()
        {
            return await _dbContext.Countries.AsNoTracking().ToListAsync();
        }

        public async Task<Country> GetCountry(Guid countryId)
        {
           return await _dbContext.Countries.AsNoTracking().FirstOrDefaultAsync(c => c.Id == countryId)
                ?? throw new EntityNotFoundException(nameof(Country), countryId.ToString());
        }
    }
}
