using Data.Exceptions;
using Data.Interfaces;
using Data.Model;

namespace Data.Implementation;

public class ReadingInterface(IRepository repository) : IReadingInterface
{
    public async Task AddReadingAsync(MeterRead meterRead)
    {
        // Make sure the account exists
        var account = await repository.GetUserAccountAsync(meterRead.AccountId);
        if (account == null)
        {
            throw new MissingAccountException(meterRead.AccountId);
        }

        // Check the the value is in the correct format
        if (meterRead.MeterReadValue is < 0 or > 99999)
        {
            throw new InvalidValueFormatException(meterRead.AccountId, meterRead.MeterReadingDateTime, meterRead.MeterReadValue);
        }
        
        // Check that the reading hasn't been added
        var read = await repository.GetMeterReadAsync(meterRead.AccountId, meterRead.MeterReadingDateTime);
        if (read != null)
        {
            throw new ReadExistsException(meterRead.AccountId, meterRead.MeterReadingDateTime);
        }

        // Check that the reading is not earlier than the last reading
        var lastReadingDate = await repository.GetLastReadingDateAsync(meterRead.AccountId);
        if (meterRead.MeterReadingDateTime < lastReadingDate)
        {
            throw new NotLatestReadException(meterRead.AccountId, meterRead.MeterReadingDateTime);
        }
        
        await repository.RecordMeterReadAsync(meterRead);
    }
}