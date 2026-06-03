using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TourManagement.Web.Controllers
{
    [Authorize(Roles = "Admin")] // Chỉ Admin mới được vào controller này
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View(); // Tạo file Dashboard.cshtml tương ứng trong Views/Admin/
        }
    }
}
