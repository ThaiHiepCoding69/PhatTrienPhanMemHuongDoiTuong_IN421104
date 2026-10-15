namespace EduHub.Ch02.Domain;

/// <summary>Sự kiện của CLB, kèm danh sách đăng ký và check-in.</summary>
public class Event
{
    public Guid Id { get; set; }
    public Guid ClubId { get; set; }
    public required string Title { get; set; }
    public DateTime Date { get; set; }

    public HashSet<Guid> RegisteredMemberIds { get; } = new();
    public HashSet<Guid> CheckedInMemberIds { get; } = new();
    public double AttendanceRate => RegisteredMemberIds.Count == 0 ? 0 : (double)CheckedInMemberIds.Count / RegisteredMemberIds.Count;

    public bool Register(Guid memberId)
    {
        return RegisteredMemberIds.Add(memberId);
    }

    public void CheckIn(Guid memberId)
    {
        if (!RegisteredMemberIds.Contains(memberId))
        {
            throw new InvalidOperationException("Member has not registered");
        }

        CheckedInMemberIds.Add(memberId);
    }
}
