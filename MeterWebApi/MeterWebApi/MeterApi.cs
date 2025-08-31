using System.Globalization;
using CsvHelper;
using Data.Exceptions;
using Data.Interfaces;
using Data.Model;
using Microsoft.AspNetCore.Mvc;

namespace MeterWebApi;

[Route("api/[controller]")]
[ApiController]
public class MeterApi(IReadingInterface readingInterface) : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Post(IFormFile file)
    {
        var errorMessages = new List<string>();
            
        if(file.Length == 0 || !file.FileName.EndsWith(".csv")) return BadRequest();
            
        await using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        using var csvReader = new CsvReader(reader, CultureInfo.CurrentCulture);

        var readings = csvReader.GetRecords<MeterRead>();

        foreach (var reading in readings)
        {
            try
            {
                await readingInterface.AddReadingAsync(reading);
            }
            catch (MissingAccountException)
            {
                errorMessages.Add(
                    $"Missing user account: {reading.AccountId}: Reading date: {reading.MeterReadingDateTime}");
            }
            catch (ReadExistsException)
            {
                errorMessages.Add(
                    $"Duplicate reading: {reading.AccountId}: Reading date: {reading.MeterReadingDateTime}");
            }
            catch (Exception e)
            {
                errorMessages.Add($"Generic error message: {e.Message}");
            }
        }

        if (errorMessages.Count > 0)
        {
            return BadRequest(errorMessages);
        }
        return Ok();
    }
}