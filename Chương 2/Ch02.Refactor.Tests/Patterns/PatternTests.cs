using EduHub.Ch02.Domain;
using EduHub.Ch02.Logging;
using EduHub.Ch02.Notifications;
using EduHub.Ch02.Patterns.Observers;
using EduHub.Ch02.Patterns.Strategies;
using EduHub.Ch02.Repositories;
using EduHub.Ch02.Services;
using Xunit;

namespace Ch02.Refactor.Tests.Patterns;

public class PatternTests
{
    // ==========================================
    // 1. Factory Pattern Tests
    // ==========================================

    [Fact]
    public void Factory_Create_WithEmailChannel_ReturnsEmailNotificationService()
    {
        // Arrange
        var factory = new NotificationServiceFactory();

        // Act
        var service = factory.Create(NotificationChannel.Email);

        // Assert
        Assert.NotNull(service);
        Assert.IsType<EmailNotificationService>(service);
    }

    [Fact]
    public void Factory_Create_WithSmsChannel_ReturnsSmsNotificationService()
    {
        // Arrange
        var factory = new NotificationServiceFactory();

        // Act
        var service = factory.Create(NotificationChannel.Sms);

        // Assert
        Assert.NotNull(service);
        Assert.IsType<SmsNotificationService>(service);
    }

    [Fact]
    public void Factory_Create_WithSlackChannel_ReturnsSlackNotificationService()
    {
        // Arrange
        var factory = new NotificationServiceFactory();

        // Act
        var service = factory.Create(NotificationChannel.Slack);

        // Assert
        Assert.NotNull(service);
        Assert.IsType<SlackNotificationService>(service);
    }

    [Fact]
    public void Factory_Create_WithInvalidChannel_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var factory = new NotificationServiceFactory();
        var invalidChannel = (NotificationChannel)999;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => factory.Create(invalidChannel));
    }

    // ==========================================
    // 2. Decorator Pattern Tests
    // ==========================================

    private class FakeLogger : IAppLogger
    {
        public List<string> Messages { get; } = new();

        public void Info(string message)
        {
            Messages.Add(message);
        }
    }

    [Fact]
    public void Decorator_Add_LogsAndDelegatesToInnerRepository()
    {
        // Arrange
        var innerRepo = new InMemoryClubRepository();
        var logger = new FakeLogger();
        var decorator = new LoggingClubRepository(innerRepo, logger);
        var club = Club.Create("Coding Club", "A programming club");

        // Act
        decorator.Add(club);

        // Assert
        Assert.Single(logger.Messages);
        Assert.Contains("Coding Club", logger.Messages[0]);
        Assert.Equal(club, innerRepo.GetById(club.Id));
    }

    [Fact]
    public void Decorator_GetById_LogsAndDelegatesToInnerRepository()
    {
        // Arrange
        var innerRepo = new InMemoryClubRepository();
        var logger = new FakeLogger();
        var decorator = new LoggingClubRepository(innerRepo, logger);
        var club = Club.Create("Robotics Club", "Robotics and IoT");
        innerRepo.Add(club);
        logger.Messages.Clear();

        // Act
        var result = decorator.GetById(club.Id);

        // Assert
        Assert.Single(logger.Messages);
        Assert.Contains(club.Id.ToString(), logger.Messages[0]);
        Assert.Equal(club, result);
    }

    [Fact]
    public void Decorator_GetAll_LogsAndDelegatesToInnerRepository()
    {
        // Arrange
        var innerRepo = new InMemoryClubRepository();
        var logger = new FakeLogger();
        var decorator = new LoggingClubRepository(innerRepo, logger);
        var club1 = Club.Create("Club 1", "Desc 1");
        var club2 = Club.Create("Club 2", "Desc 2");
        innerRepo.Add(club1);
        innerRepo.Add(club2);
        logger.Messages.Clear();

        // Act
        var result = decorator.GetAll();

        // Assert
        Assert.Single(logger.Messages);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Id == club1.Id);
        Assert.Contains(result, c => c.Id == club2.Id);
    }

    // ==========================================
    // 3. Strategy Pattern Tests
    // ==========================================

    [Fact]
    public void Strategy_CsvReportStrategy_RendersValidCsvHeaderAndRows()
    {
        // Arrange
        var strategy = new CsvReportStrategy();
        var stats = new List<ClubStat>
        {
            new("Tech Club", 20, 2, 15, 10),
            new("Sports Club", 10, 1, 8, 7)
        };

        // Act
        var output = strategy.Render(stats);

        // Assert
        Assert.Contains("ClubName,Members,Events,Registered,CheckedIn", output);
        Assert.Contains("Tech Club,20,2,15,10", output);
        Assert.Contains("Sports Club,10,1,8,7", output);
    }

    [Fact]
    public void Strategy_ConsoleAndCsvStrategies_RenderDistinctFormattedOutputs()
    {
        // Arrange
        var consoleStrategy = new ConsoleReportStrategy();
        var csvStrategy = new CsvReportStrategy();
        var stats = new List<ClubStat>
        {
            new("Tech Club", 20, 2, 15, 10)
        };

        // Act
        var consoleOutput = consoleStrategy.Render(stats);
        var csvOutput = csvStrategy.Render(stats);

        // Assert
        Assert.NotEqual(consoleOutput, csvOutput);
        Assert.Contains("=== CLUB REPORT ===", consoleOutput);
        Assert.Contains("Tech Club", consoleOutput);
        Assert.Contains("ClubName,Members,Events,Registered,CheckedIn", csvOutput);
        Assert.Contains("Tech Club,20,2,15,10", csvOutput);
    }

    // ==========================================
    // 4. Observer Pattern Tests
    // ==========================================

    private class SpyObserver : IEventObserver
    {
        public List<(Event Event, Club Club)> Notifications { get; } = new();

        public void OnEventCreated(Event ev, Club club)
        {
            Notifications.Add((ev, club));
        }
    }

    [Fact]
    public void Observer_CreateEvent_NotifiesAllSubscribedObservers()
    {
        // Arrange
        var clubRepo = new InMemoryClubRepository();
        var eventRepo = new InMemoryEventRepository();
        var eventService = new EventService(eventRepo, clubRepo);
        var club = Club.Create("AI Club", "Artificial Intelligence");
        clubRepo.Add(club);

        var spy1 = new SpyObserver();
        var spy2 = new SpyObserver();
        eventService.Subscribe(spy1);
        eventService.Subscribe(spy2);

        // Act
        var ev = eventService.CreateEvent(club.Id, "Workshop Machine Learning", DateTime.UtcNow);

        // Assert
        Assert.Single(spy1.Notifications);
        Assert.Equal(ev.Id, spy1.Notifications[0].Event.Id);
        Assert.Equal(club.Id, spy1.Notifications[0].Club.Id);

        Assert.Single(spy2.Notifications);
        Assert.Equal(ev.Id, spy2.Notifications[0].Event.Id);
        Assert.Equal(club.Id, spy2.Notifications[0].Club.Id);
    }

    [Fact]
    public void Observer_Unsubscribe_StopsReceivingNotifications()
    {
        // Arrange
        var clubRepo = new InMemoryClubRepository();
        var eventRepo = new InMemoryEventRepository();
        var eventService = new EventService(eventRepo, clubRepo);
        var club = Club.Create("AI Club", "Artificial Intelligence");
        clubRepo.Add(club);

        var spy = new SpyObserver();
        eventService.Subscribe(spy);
        eventService.Unsubscribe(spy);

        // Act
        eventService.CreateEvent(club.Id, "Workshop Deep Learning", DateTime.UtcNow);

        // Assert
        Assert.Empty(spy.Notifications);
    }
}
