namespace TourManagement.Web.Models
{
    public class TourViewModel
    {
        public int Id { get; set; }
        public string TourName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }
}