using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TourManagement.Data.Models;

namespace TourManagement.Business.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(Booking booking, HttpContext context, string vnp_ReturnUrl);
        bool PaymentExecute(IQueryCollection collections);
    }
}
