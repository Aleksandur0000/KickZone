namespace KickZone.Models.ViewModels
{
    public class CatalogViewModel
    {
        public List<ShoesModel> Shoes { get; set; } = new();
        public List<CategoryModel> Categories { get; set; } = new();
        public List<string> Brands { get; set; } = new();
        public int? CategoryId { get; set; }
        public string? Brand { get; set; }
        public string? Search { get; set; }
        public string? Error { get; set; }
    }
}
