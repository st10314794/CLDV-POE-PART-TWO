namespace CLDV_POE_PART_TWO.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public List<string>? RecentActivities { get; set; }
    }
}
