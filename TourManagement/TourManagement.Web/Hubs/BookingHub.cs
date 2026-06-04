using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TourManagement.Web.Hubs
{
    public class BookingHub : Hub
    {
        public async Task SendNewBooking(object bookingData)
        {
            await Clients.All.SendAsync("ReceiveNewBooking", bookingData);
        }
    }
}
