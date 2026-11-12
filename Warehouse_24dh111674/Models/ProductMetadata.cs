using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse_24dh111674.Models
{
    public partial class Product
    {
        [NotMapped]
        public string StockStatus
        {
            get
            {
                if (Quantity <= 0)
                {
                    return "Hết hàng";
                }
                if (Quantity <= 4)
                {
                    return "Sắp hết hàng";
                }
                return "Còn hàng";
            }
        }

        [NotMapped]
        public string StockBadgeClass
        {
            get
            {
                if (Quantity <= 0)
                {
                    return "bg-danger";
                }
                if (Quantity <= 4)
                {
                    return "bg-warning text-dark";
                }
                return "bg-success";
            }
        }
    }

    public class ProductMetadata
    {
        [Display(Name = "Mã sản phẩm")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm tối đa 100 ký tự.")]
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Danh mục không được để trống.")]
        [StringLength(100, ErrorMessage = "Danh mục tối đa 100 ký tự.")]
        [Display(Name = "Danh mục")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Giá bán không được để trống.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn 0.")]
        [Display(Name = "Đơn giá")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải là số nguyên lớn hơn hoặc bằng 0.")]
        [Display(Name = "Số lượng tồn")]
        public int Quantity { get; set; }
    }
}
