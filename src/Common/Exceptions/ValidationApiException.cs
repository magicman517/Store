namespace Common.Exceptions;

public class ValidationApiException(string message, int statusCode = 400, string errorCode = "VALIDATION_ERROR")
    : ApiException(message, statusCode, errorCode);