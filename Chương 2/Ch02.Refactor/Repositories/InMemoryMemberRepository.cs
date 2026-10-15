using EduHub.Ch02.Domain;

namespace EduHub.Ch02.Repositories;

public class InMemoryMemberRepository : IMemberRepository
{
    private readonly List<Member> _members = new();
    private readonly object _lock = new();

    public void Add(Member member)
    {
        lock (_lock)
        {
            _members.Add(member);
        }
    }

    public Member? GetById(Guid id)
    {
        lock (_lock)
        {
            return _members.FirstOrDefault(m => m.Id == id);
        }
    }

    public IReadOnlyList<Member> GetAll()
    {
        lock (_lock)
        {
            return _members.ToList().AsReadOnly();
        }
    }
}
