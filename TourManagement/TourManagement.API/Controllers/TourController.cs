using Microsoft.AspNetCore.Mvc;

namespace TourManagement.API.Controllers
{
    using global::TourManagement.Data;
    using Microsoft.AspNetCore.Mvc;
   

    namespace TourManagement.API.Controllers
    {
        [ApiController]
        [Route("api/[controller]")] // Đường dẫn gọi API sẽ là: https://localhost:xxxx/api/tour
        public class TourController : ControllerBase
        {
            private readonly TourManagementDbContext _context;

            // Inject DbContext vào thông qua Hàm khởi tạo (Constructor)
            public TourController(TourManagementDbContext context)
            {
                _context = context;
            }

            // ĐOẠN CODE TÌM KIẾM CỦA BẠN ĐẶT TẠI ĐÂY:
            [HttpGet("search")] // Đường dẫn đầy đủ: https://localhost:xxxx/api/tour/search
            public IActionResult SearchTours([FromQuery] string? keyword, [FromQuery] decimal? maxPrice)
            {
                // Dùng LINQ để filter trong DB
                var query = _context.Tours.AsQueryable();

                if (!string.IsNullOrEmpty(keyword))
                {
                    // Tìm theo tên hoặc mô tả của Tour
                    query = query.Where(t => t.TourName.Contains(keyword) || t.Description.Contains(keyword));
                }

                if (maxPrice.HasValue)
                {
                    // Lọc theo giá nhỏ hơn hoặc bằng mức giá người dùng nhập
                    query = query.Where(t => t.Price <= maxPrice.Value);
                }

                return Ok(query.ToList());
            }
        }
    }
}
