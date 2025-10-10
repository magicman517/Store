namespace Common.Exceptions;

public class NotFoundException(string message, int statusCode = 404, string errorCode = "NOT_FOUND")
    : ApiException(message, statusCode, errorCode);