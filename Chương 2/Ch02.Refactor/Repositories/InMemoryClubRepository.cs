using EduHub.Ch02.Domain;

namespace EduHub.Ch02.Repositories;

public class InMemoryClubRepository : IClubRepository
{
    private readonly List<Club> _clubs = new();
    private readonly object _lock = new();

    public void Add(Club club)
    {
        lock (_lock)
        {
            _clubs.Add(club);
        }
    }

    public Club? GetById(Guid id)
    {
        lock (_lock)
        {
            return _clubs.FirstOrDefault(c => c.Id == id);
        }
    }

    public IReadOnlyList<Club> GetAll()
    {
        lock (_lock)
        {
            return _clubs.ToList().AsReadOnly();
        }
    }
}
