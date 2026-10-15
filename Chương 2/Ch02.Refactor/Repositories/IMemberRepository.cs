using EduHub.Ch02.Domain;

namespace EduHub.Ch02.Repositories;

public interface IMemberRepository
{
    void Add(Member member);
    Member? GetById(Guid id);
    IReadOnlyList<Member> GetAll();
}
