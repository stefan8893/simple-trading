using System.Globalization;

namespace SimpleTrading.TestInfrastructure;

public abstract class TestBase : IDisposable
{
    protected TestBase()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("de-AT");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
    }

    public void Dispose()
    {
    }
}