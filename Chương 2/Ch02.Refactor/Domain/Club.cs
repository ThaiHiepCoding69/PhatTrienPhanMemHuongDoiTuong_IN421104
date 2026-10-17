namespace EduHub.Ch02.Domain;

/// <summary>Thực thể Câu lạc bộ (Domain Entity), chứa dữ liệu và quy tắc nghiệp vụ cốt lõi.</summary>
public class Club
{
    public const int MaxMembers = 50;
    public const int MaxNameLength = 100;

    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

    /// <summary>Khởi tạo CLB mới với kiểm tra tính hợp lệ của tên.</summary>
    public static Club Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Club name cannot be empty or whitespace.", nameof(name));

        if (name.Length > MaxNameLength)
            throw new ArgumentException($"Club name cannot exceed {MaxNameLength} characters.", nameof(name));

        return new Club
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description
        };
    }

    public bool HasRoom(int currentMemberCount)
    {
        return currentMemberCount < MaxMembers;
    }
}
