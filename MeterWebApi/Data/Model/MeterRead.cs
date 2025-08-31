using System.ComponentModel.DataAnnotations;

namespace Data.Model;

public class MeterRead
{
    [Required] public int AccountId { get; set; }
    [Required] public DateTime MeterReadingDateTime { get; set; }
    [Required] public int MeterReadValue { get; set; }
}
