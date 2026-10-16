using EduHub.Ch02.Patterns.Strategies;
using EduHub.Ch02.Repositories;

namespace EduHub.Ch02.Services;

public class ReportService : IReportService
{
    private readonly IClubRepository _clubRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IEventRepository _eventRepository;

    public ReportService(
        IClubRepository clubRepository,
        IMemberRepository memberRepository,
        IEventRepository eventRepository)
    {
        _clubRepository = clubRepository ?? throw new ArgumentNullException(nameof(clubRepository));
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
    }

    public string GenerateReport(IReportStrategy strategy)
    {
        if (strategy == null) throw new ArgumentNullException(nameof(strategy));

        var clubs = _clubRepository.GetAll();
        var allMembers = _memberRepository.GetAll();
        var allEvents = _eventRepository.GetAll();

        var stats = new List<ClubStat>();

        foreach (var club in clubs)
        {
            var memberCount = allMembers.Count(m => m.ClubId == club.Id);
            var clubEvents = allEvents.Where(e => e.ClubId == club.Id).ToList();
            var eventCount = clubEvents.Count;
            var registeredCount = clubEvents.Sum(e => e.RegisteredMemberIds.Count);
            var checkedInCount = clubEvents.Sum(e => e.CheckedInMemberIds.Count);

            stats.Add(new ClubStat(club.Name, memberCount, eventCount, registeredCount, checkedInCount));
        }

        return strategy.Render(stats);
    }
}
