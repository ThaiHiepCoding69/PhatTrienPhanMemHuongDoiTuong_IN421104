namespace EduHub.Ch02.Notifications;

public interface INotificationService
{
    void Send(string recipient, string message);
}
