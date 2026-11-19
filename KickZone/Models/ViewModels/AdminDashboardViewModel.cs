namespace KickZone.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int UserCount { get; set; }
        public int ShoeCount { get; set; }
        public int OrderCount { get; set; }
        public List<OrderModel> RecentOrders { get; set; } = new();
        public List<DailyStatModel> RevenueByDay { get; set; } = new();
    }
}
