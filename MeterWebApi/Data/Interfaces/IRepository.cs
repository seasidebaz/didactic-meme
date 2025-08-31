using Data.Model;

namespace Data.Interfaces;

public interface IRepository
{
    void Add(MeterRead meterRead);
    Task AddAsync(MeterRead meterRead);

    Task<UserAccount> GetUserAccountAsync(int accountId);
    Task<MeterRead> GetMeterReadAsync(int accountId, DateTime readingDate);
}