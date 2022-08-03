using System;
namespace WebShop.ModelViews
{
    public class SearchProductVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int? Price { get; set; }
        public string Thumb { get; set; }
        public string Alias { get; set; }
    }
}
