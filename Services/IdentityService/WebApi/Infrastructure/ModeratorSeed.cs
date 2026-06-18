using Application.Interfaces;
using Application.Shared.HelpFuntions;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Infrastructure;

public static class ModeratorSeed
{
    public static async Task EnsureSeededAsync(IDbContext dbContext, IConfiguration configuration)
    {
        var emailValue = configuration["Moderator:Email"];
        var password = configuration["Moderator:Password"];
        if (string.IsNullOrWhiteSpace(emailValue) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var email = new Email(emailValue);
        var existing = await dbContext.Users.FirstOrDefaultAsync(u => u.Email.Value == email.Value);
        if (existing is not null)
        {
            existing.Role = AccountRole.Moderator;
            existing.Status = AccountStatus.Active;
            await dbContext.SaveChangesAsync();
            return;
        }

        var country = await dbContext.Countries.OrderBy(c => c.Name.Value).FirstAsync();
        var phone = new Phone(configuration["Moderator:Phone"] ?? "+79990000000");
        var moderator = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Phone = phone,
            PasswordHash = PasswordHasher.Hash(password),
            Country = country,
            Postcode = new Postcode(configuration["Moderator:Postcode"] ?? "000000"),
            FullName = new FullName("Moderator", "System", null),
            UserType = UserType.Individual,
            Role = AccountRole.Moderator,
            Status = AccountStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        await dbContext.Users.AddAsync(moderator);
        await dbContext.SaveChangesAsync();
    }
}
