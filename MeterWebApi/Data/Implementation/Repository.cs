using Data.Context;
using Data.Interfaces;
using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Implementation;

public class Repository(MeterContext context) : IRepository
{
    public async Task RecordMeterReadAsync(MeterRead meterRead)
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

    public async Task<DateTime> GetLastReadingDateAsync(int accountId)
    {
        return await context.MeterReads.Where(x => x.AccountId == accountId)
            .OrderByDescending(x => x.MeterReadingDateTime)
            .Select(x => x.MeterReadingDateTime)
            .FirstOrDefaultAsync();
    }
}