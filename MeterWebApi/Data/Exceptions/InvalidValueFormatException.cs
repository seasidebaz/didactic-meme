namespace Data.Exceptions;

public class InvalidValueFormatException(int accountId, DateTime readingDate, int value)
    : Exception($"Value is invalid for {accountId} on {readingDate}: {value}");
