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

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOrWhitespaceName_ThrowsArgumentException(string invalidName)
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => Club.Create(invalidName, "Description"));
    }

    [Fact]
    public void Create_WithNameExceedingMaxNameLength_ThrowsArgumentException()
    {
        // Arrange
        var longName = new string('A', Club.MaxNameLength + 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Club.Create(longName, "Description"));
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(49, true)]
    [InlineData(50, false)]
    public void HasRoom_AtBoundaryMemberCounts_ReturnsExpectedResult(int currentMemberCount, bool expectedResult)
    {
        // Arrange
        var club = Club.Create("Tech Club", "Description");

        // Act
        var result = club.HasRoom(currentMemberCount);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}
