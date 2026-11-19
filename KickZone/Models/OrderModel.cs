namespace KickZone.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "pending";
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; } = string.Empty;
    }

    public class OrderItemModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ShoeId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ShoeName { get; set; } = string.Empty;
    }
}
