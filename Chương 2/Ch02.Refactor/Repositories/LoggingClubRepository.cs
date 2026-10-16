using EduHub.Ch02.Domain;
using EduHub.Ch02.Logging;

namespace EduHub.Ch02.Repositories;

public class LoggingClubRepository : IClubRepository
{
    private readonly IClubRepository _inner;
    private readonly IAppLogger _logger;

    public LoggingClubRepository(IClubRepository inner, IAppLogger logger)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Add(Club club)
    {
        _logger.Info($"[repo] Add club '{club.Name}'");
        _inner.Add(club);
    }

    public Club? GetById(Guid id)
    {
        _logger.Info($"[repo] Get club by id '{id}'");
        return _inner.GetById(id);
    }

    public IReadOnlyList<Club> GetAll()
    {
        _logger.Info("[repo] Get all clubs");
        return _inner.GetAll();
    }
}
