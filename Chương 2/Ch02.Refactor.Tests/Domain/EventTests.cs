using EduHub.Ch02.Domain;
using Xunit;

namespace Ch02.Refactor.Tests.Domain;

public class EventTests
{
    [Fact]
    public void Register_NewMember_ReturnsTrue()
    {
        // Arrange
        var ev = new Event { Id = Guid.NewGuid(), ClubId = Guid.NewGuid(), Title = "Workshop", Date = DateTime.UtcNow };
        var memberId = Guid.NewGuid();

        // Act
        var result = ev.Register(memberId);

        // Assert
        Assert.True(result);
        Assert.Contains(memberId, ev.RegisteredMemberIds);
    }

    [Fact]
    public void Register_SameMemberTwice_ReturnsFalse()
    {
        // Arrange
        var ev = new Event { Id = Guid.NewGuid(), ClubId = Guid.NewGuid(), Title = "Workshop", Date = DateTime.UtcNow };
        var memberId = Guid.NewGuid();
        ev.Register(memberId);

        // Act
        var result = ev.Register(memberId);

        // Assert
        Assert.False(result);
        Assert.Single(ev.RegisteredMemberIds);
    }

    [Fact]
    public void CheckIn_RegisteredMember_AddsToCheckedInMemberIds()
    {
        // Arrange
        var ev = new Event { Id = Guid.NewGuid(), ClubId = Guid.NewGuid(), Title = "Workshop", Date = DateTime.UtcNow };
        var memberId = Guid.NewGuid();
        ev.Register(memberId);

        // Act
        ev.CheckIn(memberId);

        // Assert
        Assert.Contains(memberId, ev.CheckedInMemberIds);
    }

    [Fact]
    public void CheckIn_UnregisteredMember_ThrowsInvalidOperationException()
    {
        // Arrange
        var ev = new Event { Id = Guid.NewGuid(), ClubId = Guid.NewGuid(), Title = "Workshop", Date = DateTime.UtcNow };
        var memberId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => ev.CheckIn(memberId));
        Assert.Equal("Member has not registered", exception.Message);
    }

    [Fact]
    public void AttendanceRate_WithNoRegisteredMembers_ReturnsZero()
    {
        // Arrange
        var ev = new Event { Id = Guid.NewGuid(), ClubId = Guid.NewGuid(), Title = "Workshop", Date = DateTime.UtcNow };

        // Act & Assert
        Assert.Equal(0, ev.AttendanceRate);
    }

    [Fact]
    public void AttendanceRate_WhenHalfMembersCheckedIn_ReturnsHalf()
    {
        // Arrange
        var ev = new Event { Id = Guid.NewGuid(), ClubId = Guid.NewGuid(), Title = "Workshop", Date = DateTime.UtcNow };
        var member1 = Guid.NewGuid();
        var member2 = Guid.NewGuid();
        ev.Register(member1);
        ev.Register(member2);

        // Act
        ev.CheckIn(member1);

        // Assert
        Assert.Equal(0.5, ev.AttendanceRate);
    }
}
