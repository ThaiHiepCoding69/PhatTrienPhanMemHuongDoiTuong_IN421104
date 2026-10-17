using System.Text;
using EduHub.Ch02.Domain;
using EduHub.Ch02.Logging;
using EduHub.Ch02.Notifications;
using EduHub.Ch02.Patterns.Observers;
using EduHub.Ch02.Patterns.Strategies;
using EduHub.Ch02.Repositories;
using EduHub.Ch02.Services;

// Cấu hình mã hóa UTF-8 để hiển thị tiếng Việt trên màn hình Console
Console.OutputEncoding = Encoding.UTF8;

// ═══════════════════════════════════════════════════════════════════════════════
// COMPOSITION ROOT — NƠI DUY NHẤT KHỞI TẠO VÀ THIẾT LẬP DEPENDENCY INJECTION
// ═══════════════════════════════════════════════════════════════════════════════

// 1. Logger
IAppLogger logger = new ConsoleLogger();

// 2. Repositories (Áp dụng Decorator Pattern cho Club Repository)
IClubRepository clubRepo = new LoggingClubRepository(new InMemoryClubRepository(), logger);
IMemberRepository memberRepo = new InMemoryMemberRepository();
IEventRepository eventRepo = new InMemoryEventRepository();

// 3. Notification & Factory (Áp dụng Factory Pattern)
INotificationServiceFactory notifFactory = new NotificationServiceFactory();
INotificationService emailService = notifFactory.Create(NotificationChannel.Email);

// 4. Business Services (Pure Dependency Injection)
IClubService clubService = new ClubService(clubRepo, memberRepo, emailService, logger);
IEventService eventService = new EventService(eventRepo, clubRepo);
IReportService reportService = new ReportService(clubRepo, memberRepo, eventRepo);

// 5. Đăng ký Observers (Áp dụng Observer Pattern)
eventService.Subscribe(new LogNotifier(logger));
eventService.Subscribe(new EmailNotifier(memberRepo, emailService));

// ═══════════════════════════════════════════════════════════════════════════════
// 5 KỊCH BẢN DEMO BẢO ĐẢM TÍNH HOÀN THIỆN CỦA HỆ THỐNG
// ═══════════════════════════════════════════════════════════════════════════════

Console.WriteLine("================================================================================");
Console.WriteLine("       EDUHUB CH02 — HỆ THỐNG QUẢN LÝ CÂU LẠC BỘ (SOLID & DESIGN PATTERNS)      ");
Console.WriteLine("================================================================================");

// ------------------------------------------------------------------------------
// DEMO 1: Tạo Câu lạc bộ & Thêm thành viên qua ClubService
// ------------------------------------------------------------------------------
Console.WriteLine("\n--------------------------------------------------------------------------------");
Console.WriteLine(" DEMO 1: TẠO CÂU LẠC BỘ & THÊM THÀNH VIÊN (ClubService & Decorator Pattern)     ");
Console.WriteLine("--------------------------------------------------------------------------------");
Console.WriteLine("-> Khởi tạo CLB 'CLB Công nghệ Thông tin (IT Club)'...");
var club = clubService.CreateClub(
    "CLB Công nghệ Thông tin",
    "Nơi chia sẻ kiến thức phần mềm, kiến trúc hệ thống và lập trình ứng dụng",
    "president.itclub@eduhub.vn"
);
Console.WriteLine($"[✓] CLB đã tạo thành công: {club.Name} (ID: {club.Id})");

Console.WriteLine("\n-> Thêm thành viên vào CLB qua ClubService:");
var member1 = clubService.AddMemberToClub(club.Id, "Nguyễn Văn An", "an.nguyen@eduhub.vn");
Console.WriteLine($"  [+] Thành viên 1: {member1.Name} ({member1.Email})");

var member2 = clubService.AddMemberToClub(club.Id, "Trần Thị Bình", "binh.tran@eduhub.vn");
Console.WriteLine($"  [+] Thành viên 2: {member2.Name} ({member2.Email})");

var member3 = clubService.AddMemberToClub(club.Id, "Lê Hoàng Cường", "cuong.le@eduhub.vn");
Console.WriteLine($"  [+] Thành viên 3: {member3.Name} ({member3.Email})");

// ------------------------------------------------------------------------------
// DEMO 2 (Observer): Tạo Sự kiện mới -> Observer tự động thông báo đồng thời qua Log và Email
// ------------------------------------------------------------------------------
Console.WriteLine("\n--------------------------------------------------------------------------------");
Console.WriteLine(" DEMO 2 (Observer): TẠO SỰ KIỆN MỚI — TỰ ĐỘNG THÔNG BÁO QUA LOG & EMAIL          ");
Console.WriteLine("--------------------------------------------------------------------------------");
Console.WriteLine("-> Khi gọi eventService.CreateEvent, tất cả Observers đã đăng ký (LogNotifier,");
Console.WriteLine("   EmailNotifier) sẽ đồng thời nhận thông báo và tự động xử lý:");

var workshopEvent = eventService.CreateEvent(
    club.Id,
    "Workshop: Clean Architecture & SOLID Patterns",
    new DateTime(2026, 11, 20, 9, 0, 0)
);
Console.WriteLine($"\n[✓] Sự kiện được khởi tạo: '{workshopEvent.Title}' (ID: {workshopEvent.Id})");

Console.WriteLine("\n-> Thành viên đăng ký tham gia sự kiện:");
eventService.RegisterMember(workshopEvent.Id, member1.Id);
Console.WriteLine($"  [+] Đăng ký thành công cho: {member1.Name}");
eventService.RegisterMember(workshopEvent.Id, member2.Id);
Console.WriteLine($"  [+] Đăng ký thành công cho: {member2.Name}");
eventService.RegisterMember(workshopEvent.Id, member3.Id);
Console.WriteLine($"  [+] Đăng ký thành công cho: {member3.Name}");

Console.WriteLine("\n-> Thực hiện điểm danh (Check-in) sự kiện:");
eventService.CheckInMember(workshopEvent.Id, member1.Id);
Console.WriteLine($"  [✓] Check-in thành công: {member1.Name}");
eventService.CheckInMember(workshopEvent.Id, member2.Id);
Console.WriteLine($"  [✓] Check-in thành công: {member2.Name}");
Console.WriteLine($"  [-] Thành viên {member3.Name} chưa check-in.");
Console.WriteLine($"  => Tỷ lệ tham dự (Attendance Rate): {workshopEvent.AttendanceRate:P0} ({workshopEvent.CheckedInMemberIds.Count}/{workshopEvent.RegisteredMemberIds.Count})");

// ------------------------------------------------------------------------------
// DEMO 3 (Strategy): Xuất báo cáo hoạt động ra cả 2 định dạng Console và CSV qua ReportService
// ------------------------------------------------------------------------------
Console.WriteLine("\n--------------------------------------------------------------------------------");
Console.WriteLine(" DEMO 3 (Strategy): XUẤT BÁO CÁO HOẠT ĐỘNG VỚI CONSOLE & CSV STRATEGY           ");
Console.WriteLine("--------------------------------------------------------------------------------");
Console.WriteLine("-> [1] Báo cáo dạng Console Table (ConsoleReportStrategy):");
var consoleReport = reportService.GenerateReport(new ConsoleReportStrategy());
Console.WriteLine(consoleReport);

Console.WriteLine("\n-> [2] Báo cáo dạng Raw CSV (CsvReportStrategy):");
var csvReport = reportService.GenerateReport(new CsvReportStrategy());
Console.WriteLine(csvReport);

// ------------------------------------------------------------------------------
// DEMO 4 (Factory): Sử dụng Factory duyệt qua các kênh Email, SMS, Slack để gửi thông báo mẫu
// ------------------------------------------------------------------------------
Console.WriteLine("\n--------------------------------------------------------------------------------");
Console.WriteLine(" DEMO 4 (Factory): SỬ DỤNG FACTORY DUYỆT CÁC KÊNH THÔNG BÁO (Email, SMS, Slack)  ");
Console.WriteLine("--------------------------------------------------------------------------------");
Console.WriteLine("-> Lần lượt khởi tạo dịch vụ thông báo thông qua NotificationServiceFactory:");
foreach (var channel in Enum.GetValues<NotificationChannel>())
{
    var service = notifFactory.Create(channel);
    Console.WriteLine($"  • Kênh: {channel,-6} => Concrete Class: {service.GetType().Name}");
    service.Send("admin@eduhub.vn", $"[Hệ thống] Thông báo định kỳ gửi qua kênh {channel}.");
}

// ------------------------------------------------------------------------------
// DEMO 5: Bẫy lỗi và kiểm tra tính hợp lệ của nghiệp vụ (Validation & Exception handling)
// ------------------------------------------------------------------------------
Console.WriteLine("\n--------------------------------------------------------------------------------");
Console.WriteLine(" DEMO 5: BẪY LỖI & KIỂM TRA TÍNH HỢP LỆ NGHIỆP VỤ (Validation & Exception Catch) ");
Console.WriteLine("--------------------------------------------------------------------------------");

// 5.1 Kiểm tra tên CLB rỗng
try
{
    Console.WriteLine("[Case 5.1] Thử tạo CLB với tên rỗng:");
    clubService.CreateClub("   ", "Mô tả không hợp lệ", "error@eduhub.vn");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"  [!] Đã bẫy ngoại lệ thành công (ArgumentException): {ex.Message}");
}

// 5.2 Kiểm tra tên CLB vượt quá MaxNameLength
try
{
    Console.WriteLine("\n[Case 5.2] Thử tạo CLB với tên dài hơn 100 ký tự:");
    var longName = new string('X', Club.MaxNameLength + 1);
    clubService.CreateClub(longName, "Mô tả vượt giới hạn", "error@eduhub.vn");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"  [!] Đã bẫy ngoại lệ thành công (ArgumentException): {ex.Message}");
}

// 5.3 Kiểm tra Check-in khi chưa đăng ký sự kiện
try
{
    Console.WriteLine("\n[Case 5.3] Thử Check-in thành viên chưa đăng ký sự kiện:");
    var strangerId = Guid.NewGuid();
    eventService.CheckInMember(workshopEvent.Id, strangerId);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"  [!] Đã bẫy ngoại lệ thành công (InvalidOperationException): {ex.Message}");
}

// 5.4 Kiểm tra thao tác với CLB không tồn tại
try
{
    Console.WriteLine("\n[Case 5.4] Thử thêm thành viên vào CLB với ID không tồn tại:");
    clubService.AddMemberToClub(Guid.NewGuid(), "Khách Vãng Lai", "guest@eduhub.vn");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"  [!] Đã bẫy ngoại lệ thành công (InvalidOperationException): {ex.Message}");
}

Console.WriteLine("\n================================================================================");
Console.WriteLine("                     HOÀN THÀNH TẤT CẢ 5 KỊCH BẢN DEMO!                         ");
Console.WriteLine("================================================================================");
