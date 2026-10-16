using EduHub.Ch02.Domain;
using EduHub.Ch02.Logging;
using EduHub.Ch02.Notifications;
using EduHub.Ch02.Repositories;

namespace EduHub.Ch02.Services;

public class ClubService : IClubService
{
    private readonly IClubRepository _clubRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly INotificationService _notificationService;
    private readonly IAppLogger _logger;

    public ClubService(
        IClubRepository clubRepository,
        IMemberRepository memberRepository,
        INotificationService notificationService,
        IAppLogger logger)
    {
        _clubRepository = clubRepository ?? throw new ArgumentNullException(nameof(clubRepository));
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Club CreateClub(string name, string description, string presidentEmail)
    {
        var club = Club.Create(name, description);
        _clubRepository.Add(club);
        _logger.Info($"Club created: {club.Name}");
        _notificationService.Send(presidentEmail, $"Club '{club.Name}' created successfully.");
        return club;
    }

    public Member AddMemberToClub(Guid clubId, string memberName, string email)
    {
        var club = _clubRepository.GetById(clubId)
            ?? throw new InvalidOperationException("Club not found");

        var currentMemberCount = _memberRepository.GetAll().Count(m => m.ClubId == clubId);
        if (!club.HasRoom(currentMemberCount))
        {
            throw new InvalidOperationException("Club is full (max 50 members)");
        }

        var member = new Member
        {
            Id = Guid.NewGuid(),
            Name = memberName,
            Email = email,
            ClubId = clubId
        };

        _memberRepository.Add(member);
        _logger.Info($"Member {memberName} joined {club.Name}");
        _notificationService.Send(email, $"Welcome {memberName} to {club.Name}!");
        return member;
    }
}
