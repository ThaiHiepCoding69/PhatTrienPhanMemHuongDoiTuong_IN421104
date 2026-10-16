using EduHub.Ch02.Domain;
using EduHub.Ch02.Notifications;
using EduHub.Ch02.Repositories;

namespace EduHub.Ch02.Patterns.Observers;

public class EmailNotifier : IEventObserver
{
    private readonly IMemberRepository _memberRepository;
    private readonly INotificationService _notificationService;

    public EmailNotifier(IMemberRepository memberRepository, INotificationService notificationService)
    {
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    public void OnEventCreated(Event ev, Club club)
    {
        var members = _memberRepository.GetAll();
        var clubMembers = members.Where(m => m.ClubId == club.Id).ToList();
        var targetMembers = clubMembers.Count > 0 ? clubMembers : members.ToList();

        foreach (var member in targetMembers)
        {
            _notificationService.Send(member.Email, $"New event '{ev.Title}' created for club '{club.Name}'");
        }
    }
}
