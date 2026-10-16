using EduHub.Ch02.Domain;
using Xunit;

namespace Ch02.Refactor.Tests.Domain;

public class ClubTests
{
    [Fact]
    public void Create_WithValidNameAndDescription_ReturnsClub()
    {
        // Arrange
        var name = "Tech Club";
        var description = "Club description";

        // Act
        var club = Club.Create(name, description);

        // Assert
        Assert.NotNull(club);
        Assert.NotEqual(Guid.Empty, club.Id);
        Assert.Equal(name, club.Name);
        Assert.Equal(description, club.Description);
    }

    [Fact]
    public void Create_WithEmptyOrWhitespaceName_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Club.Create("", "Description"));
        Assert.Throws<ArgumentException>(() => Club.Create("   ", "Description"));
    }

    [Fact]
    public void Create_WithNameExceedingMaxNameLength_ThrowsArgumentException()
    {
        // Arrange
        var longName = new string('A', Club.MaxNameLength + 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Club.Create(longName, "Description"));
    }

    [Fact]
    public void HasRoom_AtBoundaryMemberCounts_ReturnsExpectedResult()
    {
        // Arrange
        var club = Club.Create("Tech Club", "Description");

        // Act & Assert
        Assert.True(club.HasRoom(0));
        Assert.True(club.HasRoom(49));
        Assert.False(club.HasRoom(50));
    }
}
