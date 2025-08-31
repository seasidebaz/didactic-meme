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
            throw new MissingAccountException();
        }
        // Check that the reading hasn't been added
        var read = await repository.GetMeterReadAsync(meterRead.AccountId, meterRead.MeterReadingDateTime);
        if (read != null)
        {
            throw new ReadExistsException();
        }

        await repository.AddAsync(meterRead);
    }
}