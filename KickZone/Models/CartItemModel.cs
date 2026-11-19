namespace KickZone.Models
{
    public class CartItemModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ShoeId { get; set; }
        public int Quantity { get; set; }

        public string ShoeName { get; set; } = string.Empty;
        public decimal ShoePrice { get; set; }
        public string? ShoeImageUrl { get; set; }
        public int ShoeSize { get; set; }
        public string ShoeColor { get; set; } = string.Empty;
        public string ShoeBrand { get; set; } = string.Empty;
        public int Stock { get; set; }

        public string DisplayImageUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ShoeImageUrl))
                    return "/Images/images.jpg";
                var url = ShoeImageUrl.Trim().TrimStart('~');
                return url.StartsWith('/') ? url : "/" + url;
            }
        }
    }
}
