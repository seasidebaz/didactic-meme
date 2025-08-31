using System.Globalization;
using CsvHelper;
using Data.Interfaces;
using Data.Model;
using Microsoft.AspNetCore.Mvc;

namespace MeterWebApi;

[Route("api/meter-reading-uploads")]
[ApiController]
public class MeterApi(IReadingInterface readingInterface, ILogger<MeterApi> logger) : ControllerBase
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

        var success = 0;
        foreach (var reading in readings)
        {
            try
            {
                await readingInterface.AddReadingAsync(reading);
                success++;
            }
            catch (Exception e)
            {
                errorMessages.Add($"Error: {e.Message}");
            }
        }

        var processingMessage = $"Successful: {success}; Failed: {errorMessages.Count}";
        if (errorMessages.Count == 0) return Ok(processingMessage);
        var errorMessage = errorMessages.Aggregate((x, y) => x + "\n" + y);
        logger.LogError("{ErrorMessage}", errorMessage);
        return BadRequest(processingMessage);
    }
}