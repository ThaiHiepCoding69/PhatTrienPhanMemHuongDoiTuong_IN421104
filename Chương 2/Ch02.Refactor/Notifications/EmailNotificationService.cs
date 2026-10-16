namespace EduHub.Ch02.Notifications;

public class EmailNotificationService : INotificationService
{
    public void Send(string recipient, string message)
    {
        Console.WriteLine($"[EMAIL to {recipient}]: {message}");
    }
}
