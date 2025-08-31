using Data.Model;

namespace Data.Interfaces;

public interface IRepository
{
    Task RecordMeterReadAsync(MeterRead meterRead);

    Task<UserAccount> GetUserAccountAsync(int accountId);
    Task<MeterRead> GetMeterReadAsync(int accountId, DateTime readingDate);

    Task<DateTime> GetLastReadingDateAsync(int accountId);
}