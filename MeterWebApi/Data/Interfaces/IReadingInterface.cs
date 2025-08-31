using Data.Model;

namespace Data.Interfaces;

public interface IReadingInterface
{
    Task AddReadingAsync(MeterRead meterRead);
}
