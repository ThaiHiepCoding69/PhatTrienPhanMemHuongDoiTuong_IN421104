using System.Text;

namespace EduHub.Ch02.Patterns.Strategies;

public class CsvReportStrategy : IReportStrategy
{
    public string Name => "CSV";

    public string Render(IReadOnlyList<ClubStat> stats)
    {
        var sb = new StringBuilder();
        sb.AppendLine("ClubName,Members,Events,Registered,CheckedIn");

        foreach (var stat in stats)
        {
            sb.AppendLine($"{stat.ClubName},{stat.Members},{stat.Events},{stat.Registered},{stat.CheckedIn}");
        }

        return sb.ToString().TrimEnd();
    }
}
