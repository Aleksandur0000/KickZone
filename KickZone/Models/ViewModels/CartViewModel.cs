namespace KickZone.Models.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemModel> Items { get; set; } = new();
        public decimal Total { get; set; }
        public string? Error { get; set; }
    }
}
