USE TourManagementDB;
GO

-- 1. Nếu bảng Reviews chưa tồn tại -> Tạo mới với đầy đủ 4 tiêu chí
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reviews')
BEGIN
    CREATE TABLE [Reviews] (
        [ReviewId] INT IDENTITY(1,1) PRIMARY KEY,
        [TourId] INT NOT NULL,
        [UserId] INT NULL,
        [CustomerName] NVARCHAR(100) NOT NULL,
        [Rating] INT NOT NULL CHECK ([Rating] >= 1 AND [Rating] <= 5),
        [CleanlinessRating] INT NOT NULL DEFAULT 5 CHECK ([CleanlinessRating] >= 1 AND [CleanlinessRating] <= 5),
        [ComfortRating] INT NOT NULL DEFAULT 5 CHECK ([ComfortRating] >= 1 AND [ComfortRating] <= 5),
        [AmenitiesRating] INT NOT NULL DEFAULT 5 CHECK ([AmenitiesRating] >= 1 AND [AmenitiesRating] <= 5),
        [ValueRating] INT NOT NULL DEFAULT 5 CHECK ([ValueRating] >= 1 AND [ValueRating] <= 5),
        [Comment] NVARCHAR(1000) NOT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),

        CONSTRAINT FK_Reviews_Tours FOREIGN KEY ([TourId]) REFERENCES [Tours]([TourId]) ON DELETE CASCADE,
        CONSTRAINT FK_Reviews_Users FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE SET NULL
    );
END
ELSE
BEGIN
    -- Nếu bảng Reviews đã tạo từ bước trước -> Tự động thêm 4 cột tiêu chí mới vào bảng cũ
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reviews') AND name = 'CleanlinessRating')
    BEGIN
        ALTER TABLE [Reviews] ADD [CleanlinessRating] INT NOT NULL DEFAULT 5;
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reviews') AND name = 'ComfortRating')
    BEGIN
        ALTER TABLE [Reviews] ADD [ComfortRating] INT NOT NULL DEFAULT 5;
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reviews') AND name = 'AmenitiesRating')
    BEGIN
        ALTER TABLE [Reviews] ADD [AmenitiesRating] INT NOT NULL DEFAULT 5;
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reviews') AND name = 'ValueRating')
    BEGIN
        ALTER TABLE [Reviews] ADD [ValueRating] INT NOT NULL DEFAULT 5;
    END
END
GO

-- 2. Xóa dữ liệu cũ của bảng Reviews nếu có để chèn lại seed data chi tiết mới
DELETE FROM [Reviews];
GO

-- 3. Chèn lại dữ liệu mẫu đầy đủ các tiêu chí
INSERT INTO [Reviews] ([TourId], [CustomerName], [Rating], [CleanlinessRating], [ComfortRating], [AmenitiesRating], [ValueRating], [Comment], [CreatedDate])
VALUES
(1, N'Trần Minh Tuấn', 5, 5, 5, 5, 5, N'Tour Sa Pa rất tuyệt vời! Hướng dẫn viên nhiệt tình, phòng sạch sẽ, tiện nghi đầy đủ.', DATEADD(day, -5, GETDATE())),
(1, N'Nguyễn Thị Hoa', 4, 4, 4, 4, 4, N'Lịch trình vừa phải, khách sạn 3 sao sạch sẽ. Bữa ăn tạm ổn.', DATEADD(day, -3, GETDATE())),
(2, N'Lê Hoàng Nam', 5, 5, 5, 5, 5, N'Du thuyền 5 sao Vịnh Hạ Long đẳng cấp, tiện ích tuyệt vời và đồ ăn buffet hải sản rất phong phú.', DATEADD(day, -7, GETDATE())),
(2, N'Đặng Văn Hùng', 5, 5, 4, 5, 5, N'Trải nghiệm chèo thuyền Kayak ở Vịnh Lan Hạ vô cùng đáng nhớ. Phòng ốc thoải mái.', DATEADD(day, -2, GETDATE())),
(3, N'Phạm Thu Trang', 5, 5, 5, 5, 5, N'Bà Nà Hills đẹp mê hồn, cầu Vàng rực rỡ. Tour sắp xếp rất chu đáo!', DATEADD(day, -4, GETDATE()));
GO

SELECT * FROM [Reviews];
GO
