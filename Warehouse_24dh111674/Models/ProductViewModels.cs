using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Warehouse_24dh111674.Models
{
    public class ProductFilterViewModel
    {
        public string Category { get; set; }
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
        public string SortOrder { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public List<Product> Products { get; set; }
    }

    public class StockAdjustViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn sản phẩm.")]
        [Display(Name = "Sản phẩm")]
        public int? ProductID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng.")]
        [Display(Name = "Số lượng")]
        public int? Amount { get; set; }

        public IEnumerable<SelectListItem> ProductOptions { get; set; }
        public List<Product> Products { get; set; }
    }

    public class TransferViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn sản phẩm nguồn (A).")]
        [Display(Name = "Sản phẩm nguồn (A)")]
        public int? SourceProductID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn sản phẩm đích (B).")]
        [Display(Name = "Sản phẩm đích (B)")]
        public int? DestinationProductID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng điều chuyển.")]
        [Display(Name = "Số lượng điều chuyển (q)")]
        public int? Quantity { get; set; }

        public IEnumerable<SelectListItem> ProductOptions { get; set; }
        public List<Product> Products { get; set; }
    }

    public class CategoryStatItem
    {
        public string Category { get; set; }
        public int ProductCount { get; set; }
        public int TotalUnits { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
    }

    public class InventoryStatisticsViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalUnitsInStock { get; set; }
        public int LowStockProductsCount { get; set; }
        public int OutOfStockProductsCount { get; set; }
        public int InStockProductsCount { get; set; }
        public List<CategoryStatItem> CategoryBreakdown { get; set; }
    }
}
