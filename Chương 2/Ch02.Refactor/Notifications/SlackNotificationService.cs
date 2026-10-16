namespace EduHub.Ch02.Notifications;

public class SlackNotificationService : INotificationService
{
    public void Send(string recipient, string message)
    {
        Console.WriteLine($"[SLACK to {recipient}]: {message}");
    }
}
