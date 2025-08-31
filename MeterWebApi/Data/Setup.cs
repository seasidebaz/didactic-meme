using Data.Context;
using Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Data;

public static class Setup
{
    public static IServiceCollection AddDb(this IServiceCollection services, string? connectionString)
    {
        if(string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));

        services.AddDbContext<MeterContext>(options =>
            options.UseSqlite(connectionString));

        return services;
    }

    /// <summary>
    /// Ensures the database is created and seeds UserAccounts from a CSV file,
    /// inserting only accounts that do not already exist.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <param name="relativeCsvPath">Relative path from content root to the CSV file. Defaults to "ReferenceFiles/Test_Accounts.csv".</param>
    public static async Task EnsureCreatedAndSeedAsync(this IServiceProvider services, string relativeCsvPath = "ReferenceFiles/Test_Accounts.csv")
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MeterContext>();

        // Ensure DB exists
        await context.Database.EnsureCreatedAsync();

        // Resolve content root path
        var env = scope.ServiceProvider.GetService<IHostEnvironment>();
        var basePath = env?.ContentRootPath ?? AppContext.BaseDirectory;
        var csvPath = Path.Combine(basePath, relativeCsvPath);

        if (!File.Exists(csvPath))
        {
            return;
        }

        // Read and parse CSV (AccountId,FirstName,LastName)
        var lines = await File.ReadAllLinesAsync(csvPath);
        if (lines.Length <= 1)
        {
            return;
        }

        var accounts = lines
            .Skip(1) // skip header
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => l.Split(',', StringSplitOptions.TrimEntries))
            .Where(parts => parts.Length >= 3 && int.TryParse(parts[0], out _))
            .Select(parts => new UserAccount
            {
                AccountId = int.Parse(parts[0]),
                FirstName = parts[1],
                LastName = parts[2]
            })
            .ToList();

        if (accounts.Count == 0)
        {
            return;
        }

        var existingIds = await context.UserAccounts
            .AsNoTracking()
            .Select(u => u.AccountId)
            .ToListAsync();

        var existingSet = existingIds.Count > 0 ? new HashSet<int>(existingIds) : new HashSet<int>();
        var toAdd = accounts.Where(u => !existingSet.Contains(u.AccountId)).ToList();

        if (toAdd.Count > 0)
        {
            await context.UserAccounts.AddRangeAsync(toAdd);
            await context.SaveChangesAsync();
        }
    }
}