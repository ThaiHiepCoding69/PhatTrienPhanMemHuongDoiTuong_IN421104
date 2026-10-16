using EduHub.Ch02.Domain;
using EduHub.Ch02.Logging;

namespace EduHub.Ch02.Patterns.Observers;

public class LogNotifier : IEventObserver
{
    private readonly IAppLogger _logger;

    public LogNotifier(IAppLogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnEventCreated(Event ev, Club club)
    {
        _logger.Info($"Event '{ev.Title}' created for club '{club.Name}'");
    }
}
