using MinTwitterApp.Common;


namespace MinTwitterApp.Tests.Common;
public class DateTimeAccessorForUnitTest
        : IDateTimeAccessor
{
    public DateTime Now => new DateTime(2000, 2, 3, 4, 5, 6);
}