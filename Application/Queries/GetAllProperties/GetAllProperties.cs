namespace Application.Queries.GetAllProperties
{
    public class PropertyFilters
    {
        public string? Search { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}