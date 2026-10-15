using EduHub.Ch02.Domain;

namespace EduHub.Ch02.Repositories;

public interface IEventRepository
{
    void Add(Event ev);
    Event? GetById(Guid id);
    IReadOnlyList<Event> GetAll();
    IReadOnlyList<Event> GetByClubId(Guid clubId);
}
