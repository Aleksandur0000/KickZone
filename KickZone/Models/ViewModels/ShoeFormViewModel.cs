namespace KickZone.Models.ViewModels
{
    public class ShoeFormViewModel
    {
        public ShoesModel Shoe { get; set; } = new();
        public List<CategoryModel> Categories { get; set; } = new();
        public string? Error { get; set; }
    }
}
