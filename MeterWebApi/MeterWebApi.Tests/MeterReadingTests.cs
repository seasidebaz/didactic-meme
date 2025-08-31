using Data.Exceptions;
using Data.Implementation;
using Data.Interfaces;
using Data.Model;
using Moq;

namespace MeterWebApi.Tests;

public class MeterReadingTests
{
    [Test]
    public void I_should_not_be_able_to_submit_a_meter_reading_for_a_missing_account()
    {
        var read = new MeterRead { AccountId = 1, MeterReadValue = 100, MeterReadingDateTime = DateTime.Now };
        var repositoryMock = new Mock<IRepository>();
        var readingInterface = new ReadingInterface(repositoryMock.Object);
        Assert.ThrowsAsync<MissingAccountException>(() => readingInterface.AddReadingAsync(read));
    }
    
    [Test]
    public void I_should_not_be_able_to_add_a_duplicate_meter_reading()
    {
        var read = new MeterRead { AccountId = 1, MeterReadValue = 100, MeterReadingDateTime = DateTime.Now };
        var repositoryMock = new Mock<IRepository>();
        repositoryMock.Setup(x => x.GetMeterReadAsync(read.AccountId, read.MeterReadingDateTime)).ReturnsAsync(read);
        repositoryMock.Setup(x => x.GetUserAccountAsync(read.AccountId))
            .ReturnsAsync(new UserAccount { AccountId = read.AccountId });
        var readingInterface = new ReadingInterface(repositoryMock.Object);
        Assert.ThrowsAsync<ReadExistsException>(() => readingInterface.AddReadingAsync(read));
    }
}