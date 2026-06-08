using Application.Interfaces;
using Application.Shared.Exceptions;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IDbContext _dbContext;

        public CompanyService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Company> AddCompany(string companyName)
        {
            var company = new Company
            {
                Name = new CompanyName(companyName)
            };

            await _dbContext.Companies.AddAsync(company);
            await _dbContext.SaveChangesAsync();

            return company;
        }

        public async Task DeleteCompany(Guid companyId)
        {
            var entity = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Id == companyId)
                ?? throw new EntityNotFoundException(nameof(Company), companyId.ToString());

            _dbContext.Companies.Remove(entity);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Company> GetCompany(Guid companyId)
        {
            var company = await _dbContext.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == companyId)
                ?? throw new EntityNotFoundException(nameof(Company), companyId.ToString());

            return company;
        }
    }
}
