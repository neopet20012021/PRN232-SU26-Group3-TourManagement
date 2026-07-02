-- 1. KHỞI TẠO DATABASE
USE master;
GO

IF EXISTS (SELECT * FROM sys.databases WHERE name = N'PRN232_TourManagement')
BEGIN
    ALTER DATABASE PRN232_TourManagement SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE PRN232_TourManagement;
END
GO

CREATE DATABASE PRN232_TourManagement;
GO

USE PRN232_TourManagement;
GO

-- 2. TẠO BẢNG TÀI KHOẢN (Phục vụ chức năng Phân quyền Authentication/Role-based)
CREATE TABLE [Users] (
    [UserId] INT IDENTITY(1,1) PRIMARY KEY,
    [Username] NVARCHAR(50) NOT NULL UNIQUE,
    [Password] NVARCHAR(100) NOT NULL, -- Trong thực tế nên hash, làm ASSM có thể để text thuần để test
    [FullName] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(100) NULL,
    [Role] NVARCHAR(20) NOT NULL CHECK ([Role] IN ('Admin', 'Staff')), -- Chỉ nhận 2 role theo đề bài
    [IsActive] BIT NOT NULL DEFAULT 1
);
GO

-- 3. TẠO BẢNG DANH MỤC TOUR (Ví dụ: Tour Biển, Tour Núi, Tour Nước Ngoài...)
CREATE TABLE [Categories] (
    [CategoryId] INT IDENTITY(1,1) PRIMARY KEY,
    [CategoryName] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL
);
GO

-- 4. TẠO BẢNG TOUR DU LỊCH (Thực thể chính để làm CRUD, Tìm kiếm nâng cao, OData)
CREATE TABLE [Tours] (
    [TourId] INT IDENTITY(1,1) PRIMARY KEY,
    [TourCode] NVARCHAR(20) NOT NULL UNIQUE, -- Mã định danh tour (ví dụ: T001)
    [TourName] NVARCHAR(200) NOT NULL,
    [CategoryId] INT NOT NULL,
    [Price] DECIMAL(18, 2) NOT NULL CHECK ([Price] >= 0), -- Khoảng giá để làm bộ lọc tìm kiếm
    [StartDate] DATETIME NOT NULL,                     -- Ngày khởi hành để lọc nâng cao
    [EndDate] DATETIME NOT NULL,
    [MaxParticipants] INT NOT NULL CHECK ([MaxParticipants] > 0),
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Available', -- Trạng thái để lọc (Available, Cancelled, Completed)
    [Description] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT FK_Tours_Categories FOREIGN KEY ([CategoryId]) REFERENCES [Categories]([CategoryId]) ON DELETE CASCADE,
    CONSTRAINT CK_TourDate CHECK ([EndDate] >= [StartDate])
);
GO

-- 5. CHÈN DỮ LIỆU MẪU (Seed Data để chạy kiểm thử được ngay)

-- Chèn tài khoản hệ thống (Admin có toàn quyền, Staff bị hạn chế xóa/sửa)
INSERT INTO [Users] ([Username], [Password], [FullName], [Email], [Role], [IsActive]) VALUES
(N'admin', N'admin123', N'Nguyễn Quản Trị', N'admin@travel.com', N'Admin', 1),
(N'staff1', N'staff123', N'Trần Nhân Viên', N'staff1@travel.com', N'Staff', 1),
(N'staff2', N'staff123', N'Lê Thị Nhân Viên', N'staff2@travel.com', N'Staff', 1);

-- Chèn danh mục tour
INSERT INTO [Categories] ([CategoryName], [Description]) VALUES
(N'Tour Du Thuyền & Biển Đảo', N'Các chuyến tham quan bãi biển và nghỉ dưỡng du thuyền 5 sao'),
(N'Tour Khám Phá Núi Rừng', N'Chương trình leo núi, trekking và khám phá văn hóa vùng cao'),
(N'Tour Nghỉ Dưỡng Đô Thị', N'Tham quan các thành phố lớn và mua sắm');

-- Chèn danh sách Tour du lịch với đầy đủ khoảng giá, ngày tháng, trạng thái để test chức năng tìm kiếm nâng cao
INSERT INTO [Tours] ([TourCode], [TourName], [CategoryId], [Price], [StartDate], [EndDate], [MaxParticipants], [Status], [Description]) VALUES
(N'TOUR-HL01', N'Khám phá Vịnh Hạ Long 3 ngày 2 đêm', 1, 3500000, '2026-06-15', '2026-06-18', 20, N'Available', N'Trải nghiệm ngủ đêm trên du thuyền xa hoa'),
(N'TOUR-PQ02', N'Nghỉ dưỡng Phú Quốc thiên đường nắng vàng', 1, 5200000, '2026-07-01', '2026-07-05', 25, N'Available', N'Vé máy bay khứ hồi kèm khách sạn trung tâm'),
(N'TOUR-SP03', N'Chinh phục đỉnh Fansipan - Sapa mù sương', 2, 2800000, '2026-06-20', '2026-06-23', 15, N'Available', N'Trekking ngắm thung lũng Mường Hoa'),
(N'TOUR-HG04', N'Hà Giang - Mùa hoa tam giác mạch', 2, 3200000, '2026-10-10', '2026-10-14', 12, N'Available', N'Khám phá cột cờ Lũng Cú, đèo Mã Pí Lèng'),
(N'TOUR-HN05', N'City Tour Hà Nội - Ngàn năm văn hiến', 3, 990000, '2026-06-05', '2026-06-06', 30, N'Available', N'Tham quan Lăng Bác, Hồ Gươm và Phố Cổ'),
(N'TOUR-DN06', N'Đà Nẵng - Hội An - Bà Nà Hills', 1, 4500000, '2026-05-10', '2026-05-14', 20, N'Completed', N'Tour đã hoàn thành trong tháng 5');
GO

-- 6. KIỂM TRA DỮ LIỆU (Chạy lệnh này để xác nhận dữ liệu đã lên đủ)
SELECT * FROM [Users];
SELECT * FROM [Categories];
SELECT * FROM [Tours];
GO