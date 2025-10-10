namespace Common.Exceptions;

public class ConflictException(string message, int statusCode = 409, string errorCode = "CONFLICT")
    : ApiException(message, statusCode, errorCode);