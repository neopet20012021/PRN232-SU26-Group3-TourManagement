using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TourManagement.Data.Models;

namespace TourManagement.Business.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(Booking booking, HttpContext context, string vnp_ReturnUrl)
        {
            var vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (booking.FinalPrice * 100).ToString("0")); // Amount in VND * 100
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang: " + booking.BookingCode);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl); // We pass it dynamically from Controller
            vnpay.AddRequestData("vnp_TxnRef", booking.BookingCode + "_" + DateTime.Now.Ticks.ToString()); // Unique transaction reference

            string paymentUrl = vnpay.CreateRequestUrl(_configuration["VnPay:BaseUrl"], _configuration["VnPay:HashSecret"]);

            return paymentUrl;
        }

        public bool PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();

            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = vnpay.GetResponseData("vnp_TxnRef");
            var vnp_TransactionId = vnpay.GetResponseData("vnp_TransactionNo");
            var vnp_SecureHash = collections["vnp_SecureHash"].ToString();
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _configuration["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return false;
            }

            if (vnp_ResponseCode == "00") // 00 = success
            {
                return true;
            }

            return false;
        }
    }
}
