-- 1. Create table PromoCodes and seed initial codes (including WELCOME) if empty
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PromoCodes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[PromoCodes] (
        [PromoCodeId] INT IDENTITY(1,1) NOT NULL,
        [Code] NVARCHAR(50) NOT NULL,
        [DiscountPercent] DECIMAL(18,2) NOT NULL,
        [MinBookingValue] DECIMAL(18,2) NOT NULL,
        [StartDate] DATETIME2 NOT NULL,
        [EndDate] DATETIME2 NOT NULL,
        [MaxUsage] INT NOT NULL,
        [UsageCount] INT NOT NULL,
        [IsActive] BIT NOT NULL,
        CONSTRAINT [PK_PromoCodes] PRIMARY KEY CLUSTERED ([PromoCodeId] ASC)
    );
END
GO

IF NOT EXISTS (SELECT * FROM [dbo].[PromoCodes])
BEGIN
    SET IDENTITY_INSERT [dbo].[PromoCodes] ON;
    INSERT INTO [dbo].[PromoCodes] ([PromoCodeId], [Code], [DiscountPercent], [MinBookingValue], [StartDate], [EndDate], [MaxUsage], [UsageCount], [IsActive])
    VALUES 
    (1, 'TOUR2025', 0.10, 0.00, DATEADD(day, -10, GETDATE()), DATEADD(day, 365, GETDATE()), 100, 0, 1),
    (2, 'VIP50', 0.50, 5000000.00, DATEADD(day, -10, GETDATE()), DATEADD(day, 365, GETDATE()), 10, 0, 1),
    (3, 'SUMMER100', 0.15, 2000000.00, DATEADD(day, -10, GETDATE()), DATEADD(day, 90, GETDATE()), 200, 0, 1),
    (4, 'WELCOME', 0.15, 0.00, DATEADD(day, -10, GETDATE()), DATEADD(day, 365, GETDATE()), 9999, 0, 1);
    SET IDENTITY_INSERT [dbo].[PromoCodes] OFF;
END
ELSE IF NOT EXISTS (SELECT * FROM [dbo].[PromoCodes] WHERE [Code] = 'WELCOME')
BEGIN
    INSERT INTO [dbo].[PromoCodes] ([Code], [DiscountPercent], [MinBookingValue], [StartDate], [EndDate], [MaxUsage], [UsageCount], [IsActive])
    VALUES ('WELCOME', 0.15, 0.00, DATEADD(day, -10, GETDATE()), DATEADD(day, 365, GETDATE()), 9999, 0, 1);
END
GO

-- 2. Expand column sizes in Tours table to support long descriptions and itineraries
ALTER TABLE Tours ALTER COLUMN Itinerary NVARCHAR(MAX);
ALTER TABLE Tours ALTER COLUMN IncludedServices NVARCHAR(MAX);
ALTER TABLE Tours ALTER COLUMN ExcludedServices NVARCHAR(MAX);
ALTER TABLE Tours ALTER COLUMN Description NVARCHAR(MAX);
GO

-- 3. Update existing Tours (1, 2, 3) with correct Vietnamese text and multi-line formatting
UPDATE Tours SET 
    Description = N'Khám phá vẻ đẹp huyền ảo của sương mù Sa Pa và đỉnh Fansipan hùng vĩ. Hành trình 3 ngày 2 đêm tuyệt vời khám phá miền núi Tây Bắc với bản làng Cát Cát, chợ phiên Sa Pa và cáp treo Fansipan - nóc nhà Đông Dương.',
    Itinerary = N'Ngày 1: Hà Nội - Sa Pa
Tập trung tại điểm đón, xe đưa đón đến bến xe. Xe khởi hành lúc 22h từ Hà Nội đi Sa Pa.

Ngày 2: Sa Pa - Bản Cát Cát
Sáng: Đến Sa Pa, nhận phòng khách sạn. Tham quan thị trấn Sa Pa, chợ phiên, nhà thờ đá.
Chiều: Khám phá bản Cát Cát - bản làng người H''Mông cổ kính, ngắm thác nước, tìm hiểu nghề dệt thủ công.
Tối: Thưởng thức ẩm thực Sa Pa, nghỉ đêm tại khách sạn.

Ngày 3: Fansipan - Hà Nội
Sáng: Tham quan đỉnh Fansipan (3.143m) bằng cáp treo hiện đại nhất Đông Nam Á.
Chiều: Tham quan vườn lan Sa Pa, check-out khách sạn. Xe đưa về Hà Nội.',
    IncludedServices = N'Xe khách khứ hồi Hà Nội - Sa Pa|Khách sạn 3-4 sao (2 đêm)|Ăn sáng hàng ngày|Hướng dẫn viên chuyên nghiệp|Vé tham quan bản Cát Cát|Vé cáp treo Fansipan khứ hồi',
    ExcludedServices = N'Chi phí cá nhân (đồ uống, giặt ủi)|Bảo hiểm du lịch|Ăn trưa và tối|Tip cho hướng dẫn viên'
WHERE TourId = 1;

UPDATE Tours SET 
    Description = N'Trải nghiệm đẳng cấp 5 sao trên du thuyền khám phá Vịnh Hạ Long và Vịnh Lan Hạ - kỳ quan thiên nhiên thế giới. Ngắm hoàng hôn trên biển, chèo kayak qua hang động, tắm biển tại bãi cát trắng tinh.',
    Itinerary = N'Ngày 1: Hà Nội - Hạ Long - Lên Du Thuyền
Sáng: Xe đón tại khách sạn Hà Nội, khởi hành đến Hạ Long (khoảng 3.5 giờ).
Trưa: Lên du thuyền, ăn trưa trên thuyền. Tham quan hang Đầu Gỗ hoặc hang Thiên Cung.
Chiều: Chèo kayak qua các hang động, tắm biển tại bãi cát. Hoàng hôn trên boong tàu.
Tối: Tiệc tối cao cấp, câu mực đêm. Ngủ đêm trên du thuyền.

Ngày 2: Vịnh Lan Hạ - Hà Nội
Sáng: Chào bình minh trên boong. Ăn sáng và yoga trên thuyền.
Tham quan Vịnh Lan Hạ, bơi lội tại bãi Ba Trái Đào.
Trưa: Buffet hải sản tươi. Check-out phòng cabin.
Chiều: Xe đưa về Hà Nội, kết thúc chuyến đi.',
    IncludedServices = N'Xe khách khứ hồi Hà Nội - Hạ Long|Phòng cabin đôi/đơn trên du thuyền 5 sao|Đầy đủ bữa ăn (2 bữa sáng, 2 bữa trưa, 1 bữa tối)|Chèo kayak và tắm biển|Vé tham quan các hang động|Hướng dẫn viên song ngữ',
    ExcludedServices = N'Đồ uống có cồn (bia, rượu, cocktail)|Chi phí cá nhân|Tip cho thủy thủ đoàn và hướng dẫn viên|Bảo hiểm du lịch'
WHERE TourId = 2;

UPDATE Tours SET 
    Description = N'Tour miền Trung di sản: Đà Nẵng, Hội An, Bà Nà rực rỡ sắc màu. Khám phá Cầu Vàng nổi tiếng thế giới, phố cổ Hội An lung linh đèn lồng, bãi biển Mỹ Khê trong xanh tuyệt đẹp.',
    Itinerary = N'Ngày 1: Đà Nẵng - Bà Nà Hills - Cầu Vàng
Sáng: Đón tại sân bay Đà Nẵng, nhận phòng khách sạn.
Chiều: Tham quan Bà Nà Hills - Cầu Vàng huyền thoại, Làng Pháp, Vườn hoa Le Jardin D''Amour.
Tối: Về Đà Nẵng, dạo cầu Rồng ngắm phun lửa và phun nước.

Ngày 2: Hội An Phố Cổ
Sáng: Di chuyển Đà Nẵng - Hội An (30 phút).
Khám phá Phố cổ Hội An: Chùa Cầu, Hội quán Phúc Kiến, làng gốm Thanh Hà.
Chiều: Làm đèn lồng thủ công, thả đèn hoa đăng trên sông Hoài.
Tối: Ăn tối với cao lầu, mì Quảng đặc sản Hội An.

Ngày 3: Biển Mỹ Khê - Sơn Trà - Bay về
Sáng: Tự do tắm biển Mỹ Khê - top 6 bãi biển đẹp nhất hành tinh.
Trưa: Tham quan bán đảo Sơn Trà, chùa Linh Ứng.
Chiều: Ra sân bay Đà Nẵng, bay về Hà Nội.',
    IncludedServices = N'Vé máy bay khứ hồi Hà Nội - Đà Nẵng|Khách sạn 4 sao (2 đêm tại Đà Nẵng)|Ăn sáng tại khách sạn|Vé cáp treo Bà Nà Hills khứ hồi|Xe đưa đón theo lịch trình|Hướng dẫn viên chuyên nghiệp',
    ExcludedServices = N'Bữa ăn trưa và tối (tự túc)|Đồ uống|Mua sắm và chi phí cá nhân|Bảo hiểm du lịch'
WHERE TourId = 3;
GO

-- 4. Seed Tour 4 (Phú Quốc) and its corresponding schedule if not exists
IF NOT EXISTS (SELECT * FROM Tours WHERE TourId = 4)
BEGIN
    SET IDENTITY_INSERT Tours ON;
    INSERT INTO Tours (TourId, TourName, TourCode, Description, Days, Nights, PricePerAdult, ChildPrice, Category, Destination, MaxCapacity, IsActive, Itinerary, IncludedServices, ExcludedServices, Image, CreatedDate, UpdatedDate)
    VALUES (
        4,
        N'Combo Trọn Gói Phú Quốc - Tặng Vé VinWonders',
        'PQ4100',
        N'Khám phá thiên đường đảo ngọc Phú Quốc. Trải nghiệm vui chơi cực đỉnh tại VinWonders, tham quan vườn thú Vinpearl Safari, check-in Grand World thành phố không ngủ, ngắm hoàng hôn tuyệt đẹp trên biển Sunset Sanato.',
        3,
        2,
        4100000.00,
        3000000.00,
        N'Nghỉ dưỡng',
        N'Phú Quốc',
        30,
        1,
        N'Ngày 1: Hà Nội - Phú Quốc - Grand World
Sáng: Bay từ Hà Nội đến Phú Quốc. Xe đón về resort nhận phòng.
Chiều: Tham quan Grand World - Venice thu nhỏ, xem show diễn Tinh Hoa Việt Nam.
Tối: Ăn hải sản tại chợ đêm Dương Đông.

Ngày 2: VinWonders & Safari Phú Quốc
Sáng: Khám phá Vinpearl Safari - vườn thú bán hoang dã đầu tiên tại Việt Nam.
Chiều: Vui chơi giải trí tại VinWonders với hàng trăm trò chơi trong nhà và ngoài trời cảm giác mạnh.
Tối: Thưởng thức show diễn nhạc nước nhạc nước rực rỡ.

Ngày 3: Hoàng Hôn Sunset Sanato - Bay về
Sáng: Tự do tắm biển, mua sắm đặc sản ngọc trai, nước mắm Phú Quốc.
Trưa: Check-out khách sạn. Check-in Sunset Sanato chụp ảnh hoàng hôn.
Chiều: Xe tiễn ra sân bay Phú Quốc, bay về Hà Nội.',
        N'Vé máy bay khứ hồi Hà Nội - Phú Quốc|Resort 4 sao sát biển (2 đêm)|Vé VinWonders & Safari Phú Quốc|Xe đưa đón sân bay và theo lịch trình|Ăn sáng buffet tại resort',
        N'Chi phí ăn trưa và tối (tự túc)|Chi phí cá nhân ngoài chương trình|Tip cho hướng dẫn viên và tài xế|Bảo hiểm du lịch',
        'https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?auto=format&fit=crop&q=80&w=2105',
        GETDATE(),
        GETDATE()
    );
    SET IDENTITY_INSERT Tours OFF;
END

IF NOT EXISTS (SELECT * FROM TourSchedules WHERE ScheduleId = 4)
BEGIN
    SET IDENTITY_INSERT TourSchedules ON;
    INSERT INTO TourSchedules (ScheduleId, TourId, StartDate, EndDate, MaxParticipants, AvailableSeats, ActualAdultPrice, ActualChildPrice, Status, GuideName, CreatedDate)
    VALUES (
        4,
        4,
        DATEADD(day, 15, GETDATE()),
        DATEADD(day, 18, GETDATE()),
        30,
        25,
        4100000.00,
        3000000.00,
        'Active',
        N'Nguyễn Hoàng Nam',
        GETDATE()
    );
    SET IDENTITY_INSERT TourSchedules OFF;
END
GO
