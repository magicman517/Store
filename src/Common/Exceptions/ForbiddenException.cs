namespace Common.Exceptions;

public class ForbiddenException(string message, int statusCode = 403, string errorCode = "FORBIDDEN")
    : ApiException(message, statusCode, errorCode);