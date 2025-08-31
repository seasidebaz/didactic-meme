namespace Data.Exceptions;

public class ReadExistsException(int accountId, DateTime readingDate)
    : Exception($"Reading for {accountId} on {readingDate} already exists");