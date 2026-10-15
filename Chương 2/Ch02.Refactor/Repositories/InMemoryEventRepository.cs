using EduHub.Ch02.Domain;

namespace EduHub.Ch02.Repositories;

public class InMemoryEventRepository : IEventRepository
{
    private readonly List<Event> _events = new();
    private readonly object _lock = new();

    public void Add(Event ev)
    {
        lock (_lock)
        {
            _events.Add(ev);
        }
    }

    public Event? GetById(Guid id)
    {
        lock (_lock)
        {
            return _events.FirstOrDefault(e => e.Id == id);
        }
    }

    public IReadOnlyList<Event> GetAll()
    {
        lock (_lock)
        {
            return _events.ToList().AsReadOnly();
        }
    }

    public IReadOnlyList<Event> GetByClubId(Guid clubId)
    {
        lock (_lock)
        {
            return _events.Where(e => e.ClubId == clubId).ToList().AsReadOnly();
        }
    }
}
