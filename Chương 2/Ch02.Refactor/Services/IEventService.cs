using EduHub.Ch02.Domain;
using EduHub.Ch02.Patterns.Observers;

namespace EduHub.Ch02.Services;

public interface IEventService
{
    void Subscribe(IEventObserver observer);
    void Unsubscribe(IEventObserver observer);
    Event CreateEvent(Guid clubId, string name, DateTime scheduledAt);
    bool RegisterMember(Guid eventId, Guid memberId);
    void CheckInMember(Guid eventId, Guid memberId);
}
