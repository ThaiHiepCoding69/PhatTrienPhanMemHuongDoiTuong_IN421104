using EduHub.Ch02.Logging;
using EduHub.Ch02.Notifications;
using EduHub.Ch02.Patterns.Observers;
using EduHub.Ch02.Patterns.Strategies;
using EduHub.Ch02.Repositories;
using EduHub.Ch02.Services;

// ═══════════════════════════════════════════════════════════════
//  EduHub Ch02 — Refactoring với SOLID và Design Patterns
// ═══════════════════════════════════════════════════════════════

Console.WriteLine("=================================================================");
Console.WriteLine(" DEMO KIẾN TRÚC MỚI SAU KHI REFACTOR (SOLID & DESIGN PATTERNS)   ");
Console.WriteLine("=================================================================");

// 1. Logging & Repositories (Decorator Pattern)
IAppLogger logger = new ConsoleLogger();
IClubRepository rawClubRepo = new InMemoryClubRepository();
IClubRepository clubRepo = new LoggingClubRepository(rawClubRepo, logger);
IMemberRepository memberRepo = new InMemoryMemberRepository();
IEventRepository eventRepo = new InMemoryEventRepository();

// 2. Notifications (Factory Pattern)
INotificationServiceFactory notificationFactory = new NotificationServiceFactory();
INotificationService emailNotification = notificationFactory.Create(NotificationChannel.Email);

// 3. Business Services
IClubService clubService = new ClubService(clubRepo, memberRepo, emailNotification, logger);
IEventService eventService = new EventService(eventRepo, clubRepo);
IReportService reportService = new ReportService(clubRepo, memberRepo, eventRepo);

// 4. Observer Pattern
eventService.Subscribe(new EmailNotifier(memberRepo, emailNotification));
eventService.Subscribe(new LogNotifier(logger));

// Thực hiện các luồng nghiệp vụ
Console.WriteLine("\n[1] Tạo CLB mới:");
var techClub = clubService.CreateClub("Tech Club", "CLB Công nghệ & Lập trình", "president@gmail.com");

Console.WriteLine("\n[2] Thêm thành viên vào CLB:");
var memberA = clubService.AddMemberToClub(techClub.Id, "Nguyễn Văn A", "a@student.dthu.edu.vn");

Console.WriteLine("\n[3] Tạo sự kiện CLB (Kích hoạt Observer):");
var workshopEvent = eventService.CreateEvent(techClub.Id, "Workshop SOLID & Design Patterns", new DateTime(2026, 11, 20));

Console.WriteLine("\n[4] Đăng ký & Check-in thành viên:");
eventService.RegisterMember(workshopEvent.Id, memberA.Id);
eventService.CheckInMember(workshopEvent.Id, memberA.Id);

// 5. Strategy Pattern (Báo cáo)
Console.WriteLine("\n[5] Xuất báo cáo (Console Strategy):");
Console.WriteLine(reportService.GenerateReport(new ConsoleReportStrategy()));

Console.WriteLine("\n[6] Xuất báo cáo (CSV Strategy):");
Console.WriteLine(reportService.GenerateReport(new CsvReportStrategy()));

