using EduHub.Ch02.Domain;
using EduHub.Ch02.Patterns.Observers;
using EduHub.Ch02.Repositories;

namespace EduHub.Ch02.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IClubRepository _clubRepository;
    private readonly List<IEventObserver> _observers = new();
    private readonly object _lock = new();

    public EventService(IEventRepository eventRepository, IClubRepository clubRepository)
    {
        _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
        _clubRepository = clubRepository ?? throw new ArgumentNullException(nameof(clubRepository));
    }

    public void Subscribe(IEventObserver observer)
    {
        if (observer == null) throw new ArgumentNullException(nameof(observer));
        lock (_lock)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }
    }

    public void Unsubscribe(IEventObserver observer)
    {
        if (observer == null) throw new ArgumentNullException(nameof(observer));
        lock (_lock)
        {
            _observers.Remove(observer);
        }
    }

    public Event CreateEvent(Guid clubId, string name, DateTime scheduledAt)
    {
        var club = _clubRepository.GetById(clubId)
            ?? throw new InvalidOperationException("Club not found");

        var ev = new Event
        {
            Id = Guid.NewGuid(),
            ClubId = clubId,
            Title = name,
            Date = scheduledAt
        };

        _eventRepository.Add(ev);

        List<IEventObserver> observersCopy;
        lock (_lock)
        {
            observersCopy = _observers.ToList();
        }

        foreach (var observer in observersCopy)
        {
            observer.OnEventCreated(ev, club);
        }

        return ev;
    }

    public bool RegisterMember(Guid eventId, Guid memberId)
    {
        var ev = _eventRepository.GetById(eventId)
            ?? throw new InvalidOperationException("Event not found");

        return ev.Register(memberId);
    }

    public void CheckInMember(Guid eventId, Guid memberId)
    {
        var ev = _eventRepository.GetById(eventId)
            ?? throw new InvalidOperationException("Event not found");

        ev.CheckIn(memberId);
    }
}
