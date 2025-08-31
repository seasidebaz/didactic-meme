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
        Assert.ThrowsAsync<MissingAccountException>(async () => await readingInterface.AddReadingAsync(read));
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
        Assert.ThrowsAsync<ReadExistsException>(async () => await readingInterface.AddReadingAsync(read));
    }
    
    [Test]
    public void I_should_not_be_able_to_add_an_earlier_meter_reading()
    {
        var read = new MeterRead { AccountId = 1, MeterReadValue = 100, MeterReadingDateTime = DateTime.Now.AddDays(-1) };
        var repositoryMock = new Mock<IRepository>();
        repositoryMock.Setup(x => x.GetUserAccountAsync(read.AccountId))
            .ReturnsAsync(new UserAccount { AccountId = read.AccountId });
        repositoryMock.Setup(x => x.GetLastReadingDateAsync(read.AccountId)).ReturnsAsync(DateTime.Now);
        var readingInterface = new ReadingInterface(repositoryMock.Object);
        Assert.ThrowsAsync<NotLatestReadException>(async () => await readingInterface.AddReadingAsync(read));
    }
    
    [Test]
    public void I_should_be_able_to_add_a_meter_reading()
    {
        var read = new MeterRead { AccountId = 1, MeterReadValue = 100, MeterReadingDateTime = DateTime.Now };
        var repositoryMock = new Mock<IRepository>();
        repositoryMock.Setup(x => x.GetUserAccountAsync(read.AccountId))
            .ReturnsAsync(new UserAccount { AccountId = read.AccountId });
        var readingInterface = new ReadingInterface(repositoryMock.Object);
        Assert.DoesNotThrowAsync(async () => await readingInterface.AddReadingAsync(read));
    }
    
    [Test]
    public void I_should_not_be_able_to_add_a_meter_reading_with_a_value_over_NNNNN()
    {
        var read = new MeterRead { AccountId = 1, MeterReadValue = 100000, MeterReadingDateTime = DateTime.Now };
        var repositoryMock = new Mock<IRepository>();
        repositoryMock.Setup(x => x.GetUserAccountAsync(read.AccountId))
            .ReturnsAsync(new UserAccount { AccountId = read.AccountId });
        var readingInterface = new ReadingInterface(repositoryMock.Object);
        Assert.ThrowsAsync<InvalidValueFormatException>(async () => await readingInterface.AddReadingAsync(read));
    }
    
    [Test]
    public void I_should_not_be_able_to_add_a_meter_reading_with_a_negative_value()
    {
        var read = new MeterRead { AccountId = 1, MeterReadValue = -200, MeterReadingDateTime = DateTime.Now };
        var repositoryMock = new Mock<IRepository>();
        repositoryMock.Setup(x => x.GetUserAccountAsync(read.AccountId))
            .ReturnsAsync(new UserAccount { AccountId = read.AccountId });
        var readingInterface = new ReadingInterface(repositoryMock.Object);
        Assert.ThrowsAsync<InvalidValueFormatException>(async () => await readingInterface.AddReadingAsync(read));
    }
}