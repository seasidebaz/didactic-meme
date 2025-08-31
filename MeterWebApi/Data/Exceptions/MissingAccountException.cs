namespace Data.Exceptions;

public class MissingAccountException(int accountId) : Exception($"Account {accountId} does not exist");