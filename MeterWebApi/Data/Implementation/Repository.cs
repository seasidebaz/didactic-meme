using Data.Context;
using Data.Interfaces;
using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Implementation;

public class Repository(MeterContext context) : IRepository
{
    public void Add(MeterRead meterRead)
    {
        Task.Run(async () => await AddAsync(meterRead));
    }

    public async Task AddAsync(MeterRead meterRead)
    {
        await context.AddAsync(meterRead);
        await context.SaveChangesAsync();
    }

    public async Task<UserAccount> GetUserAccountAsync(int accountId)
    {
        return (await context.UserAccounts.FirstOrDefaultAsync(x => x.AccountId == accountId))!;
    }

    public async Task<MeterRead> GetMeterReadAsync(int accountId, DateTime readingDate)
    {
        return (await context.MeterReads.FirstOrDefaultAsync(x =>
            x.AccountId == accountId && x.MeterReadingDateTime == readingDate))!;
    }
}