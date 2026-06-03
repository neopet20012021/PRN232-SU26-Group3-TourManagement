using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using TourManagement.Web.Models; // Đảm bảo đúng namespace chứa TourViewModel của bạn

namespace TourManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _clientFactory; // <-- Khai báo biến này để hết lỗi dòng 18

        // TIÊM (INJECT) dịch vụ vào thông qua Hàm khởi tạo (Constructor)
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        // ACTION CHÍNH CỦA TRANG CHỦ CÓ TÌM KIẾM/LỌC
        public async Task<IActionResult> Index(string? keyword, decimal? maxPrice)
        {
            var client = _clientFactory.CreateClient();

            // THAY THẾ xxxx BẰNG SỐ PORT API THỰC TẾ (Ví dụ: 7233)
            string apiUrl = $"https://localhost:7055/api/tour/search?keyword={keyword}&maxPrice={maxPrice}";

            var tours = await client.GetFromJsonAsync<List<TourViewModel>>(apiUrl);

            // Truyền dữ liệu lọc ngược lại sang View để giữ trạng thái trên ô nhập liệu
            ViewBag.Keyword = keyword;
            ViewBag.MaxPrice = maxPrice;

            return View(tours);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}