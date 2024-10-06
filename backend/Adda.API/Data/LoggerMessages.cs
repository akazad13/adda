namespace Adda.API.Data;

public static partial class LoggerMessages
{
    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Error,
        Message = "An error occurred while initialising the database."
    )]
    public static partial void DatabaseInitializationError(this ILogger logger, Exception ex);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Error,
        Message = "An error occurred while seeding the database."
    )]
    public static partial void DatabaseSeedingError(this ILogger logger, Exception ex);
}
