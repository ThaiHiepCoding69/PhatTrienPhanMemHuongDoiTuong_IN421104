using System.Text;

namespace EduHub.Ch02.Patterns.Strategies;

public class ConsoleReportStrategy : IReportStrategy
{
    public string Name => "Console";

    public string Render(IReadOnlyList<ClubStat> stats)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== CLUB REPORT ===");
        sb.AppendLine(string.Format("{0,-20} | {1,7} | {2,6} | {3,10} | {4,10}", "Club Name", "Members", "Events", "Registered", "Checked In"));
        sb.AppendLine(new string('-', 65));

        foreach (var stat in stats)
        {
            sb.AppendLine(string.Format("{0,-20} | {1,7} | {2,6} | {3,10} | {4,10}", stat.ClubName, stat.Members, stat.Events, stat.Registered, stat.CheckedIn));
        }

        return sb.ToString().TrimEnd();
    }
}
