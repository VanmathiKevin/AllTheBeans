
namespace AllTheBeans.Application.Exceptions
{
    public class InvalidBeanSelectionException : Exception
    {
        public InvalidBeanSelectionException(string message) : base(message) { }

        public InvalidBeanSelectionException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
