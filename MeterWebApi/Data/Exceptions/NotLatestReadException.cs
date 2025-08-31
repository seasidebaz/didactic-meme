namespace Data.Exceptions;

public class NotLatestReadException(int accountId, DateTime readingDate)
    : Exception($"Reading for {accountId} on {readingDate} is not the latest");