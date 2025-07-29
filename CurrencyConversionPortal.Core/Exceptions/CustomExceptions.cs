namespace CurrencyConversionPortal.Core.Exceptions
{
    public class CurrencyConversionException : Exception
    {
        public CurrencyConversionException(string message) : base(message) { }
        public CurrencyConversionException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ValidationException : CurrencyConversionException
    {
        public ValidationException(string message) : base(message) { }
    }

    public class ExternalServiceException : Exception
    {
        public ExternalServiceException(string message) : base(message) { }
        public ExternalServiceException(string message, Exception innerException) : base(message, innerException) { }
    }
}