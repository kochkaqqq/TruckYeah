using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICompanyService
    {
        Task<Company> GetCompany(Guid companyId);
        Task<Company> AddCompany(string companyName);
        Task DeleteCompany(Guid companyId);
    }
}
