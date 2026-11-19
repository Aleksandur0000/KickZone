namespace KickZone.Models
{
    public class ShoesModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public int Size { get; set; }

        public string Color { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public int Stock { get; set; } = 10;

        public string DisplayImageUrl
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ImageUrl))
                    return "/Images/images.jpg";
                var url = ImageUrl.Trim().TrimStart('~');
                return url.StartsWith('/') ? url : "/" + url;
            }
        }
    }
}
