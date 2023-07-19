using Serilog;
using Serilog.Events;

namespace DataAccessLayerTests;

public class TestLogger : ILogger
{
    public TestLogger()
    {
        LogEvents = new List<LogEvent>();
    }

    public List<LogEvent> LogEvents { get; set; }

    public void Write(LogEvent logEvent)
    {
        LogEvents.Add(logEvent);
    }
}