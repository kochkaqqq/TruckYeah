using Application.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Infrastructure;

public static class CountrySeed
{
    private static readonly (Guid Id, string Name)[] Countries =
    [
        (Guid.Parse("11111111-1111-1111-1111-111111111111"), "Russia"),
        (Guid.Parse("22222222-2222-2222-2222-222222222222"), "Belarus"),
        (Guid.Parse("33333333-3333-3333-3333-333333333333"), "Kazakhstan"),
        (Guid.Parse("44444444-4444-4444-4444-444444444444"), "Germany"),
        (Guid.Parse("55555555-5555-5555-5555-555555555555"), "Poland")
    ];

    public static async Task EnsureSeededAsync(
        IDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var existingIds = await dbContext.Countries
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var missing = Countries
            .Where(c => !existingIds.Contains(c.Id))
            .Select(c => new Country
            {
                Id = c.Id,
                Name = new CountryName(c.Name)
            })
            .ToList();

        if (missing.Count == 0)
        {
            return;
        }

        await dbContext.Countries.AddRangeAsync(missing, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
