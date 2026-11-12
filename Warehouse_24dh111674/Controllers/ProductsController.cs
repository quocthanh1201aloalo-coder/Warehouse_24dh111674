using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Warehouse_24dh111674.Models;

namespace Warehouse_24dh111674.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Warehouse_24dh111674Entities db = new Warehouse_24dh111674Entities();

        // Câu 2a: Hiển thị danh sách sản phẩm và trạng thái tồn kho
        public ActionResult Index()
        {
            var products = db.Products.OrderBy(p => p.ProductID).ToList();
            return View(products);
        }

        // Câu 2b: Xác nhận xóa sản phẩm (chỉ cho phép khi Quantity = 0)
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Mã sản phẩm không hợp lệ.";
                return RedirectToAction("Index");
            }

            var product = db.Products.Find(id.Value);
            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm có ProductID = " + id.Value + ".";
                return RedirectToAction("Index");
            }

            if (product.Quantity > 0)
            {
                TempData["Error"] = string.Format(
                    "Từ chối xóa: Sản phẩm '{0}' hiện còn {1} đơn vị (Quantity > 0). Chỉ cho phép xóa khi Quantity = 0; bản ghi được giữ nguyên.",
                    product.ProductName, product.Quantity);
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // Câu 2b: Thực hiện xóa sau khi xác nhận
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm có ProductID = " + id + " để xóa.";
                return RedirectToAction("Index");
            }

            if (product.Quantity > 0)
            {
                TempData["Error"] = string.Format(
                    "Từ chối xóa: Sản phẩm '{0}' hiện còn {1} đơn vị (Quantity > 0). Bản ghi được giữ nguyên.",
                    product.ProductName, product.Quantity);
                return RedirectToAction("Index");
            }

            db.Products.Remove(product);
            db.SaveChanges();
            TempData["Success"] = string.Format("Đã xóa thành công sản phẩm hết hàng '{0}' (ProductID = {1}).", product.ProductName, product.ProductID);
            return RedirectToAction("Index");
        }

        // Câu 3: Lọc theo danh mục, khoảng tồn kho và sắp xếp theo Quantity
        public ActionResult Filter(string category, int? minQuantity, int? maxQuantity, string sortOrder = "asc")
        {
            var categories = db.Products
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            var viewModel = new ProductFilterViewModel
            {
                Category = category,
                MinQuantity = minQuantity,
                MaxQuantity = maxQuantity,
                SortOrder = sortOrder,
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c,
                    Text = c,
                    Selected = string.Equals(c, category, StringComparison.Ordinal)
                }).ToList(),
                Products = new List<Product>()
            };

            if (minQuantity.HasValue && minQuantity.Value < 0)
            {
                ModelState.AddModelError("MinQuantity", "Giá trị 'Từ số lượng' không được là số âm.");
            }

            if (maxQuantity.HasValue && maxQuantity.Value < 0)
            {
                ModelState.AddModelError("MaxQuantity", "Giá trị 'Đến số lượng' không được là số âm.");
            }

            if (minQuantity.HasValue && maxQuantity.HasValue && minQuantity.Value > maxQuantity.Value)
            {
                ModelState.AddModelError("", "Giá trị 'Từ số lượng' không được lớn hơn 'Đến số lượng'.");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            IQueryable<Product> query = db.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category == category);
            }

            if (minQuantity.HasValue)
            {
                query = query.Where(p => p.Quantity >= minQuantity.Value);
            }

            if (maxQuantity.HasValue)
            {
                query = query.Where(p => p.Quantity <= maxQuantity.Value);
            }

            if (string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderByDescending(p => p.Quantity).ThenBy(p => p.ProductID);
            }
            else
            {
                query = query.OrderBy(p => p.Quantity).ThenBy(p => p.ProductID);
            }

            viewModel.Products = query.ToList();
            return View(viewModel);
        }

        // Câu 4a: Nhập kho - GET
        public ActionResult StockIn()
        {
            var model = new StockAdjustViewModel();
            PopulateProductOptions(model);
            return View(model);
        }

        // Câu 4a: Nhập kho - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StockIn(StockAdjustViewModel model)
        {
            if (!model.Amount.HasValue || model.Amount.Value <= 0)
            {
                ModelState.AddModelError("Amount", "Số lượng nhập kho phải là số nguyên dương (> 0).");
            }

            Product product = null;
            if (model.ProductID.HasValue)
            {
                product = db.Products.Find(model.ProductID.Value);
                if (product == null)
                {
                    ModelState.AddModelError("ProductID", "Sản phẩm được chọn không tồn tại.");
                }
            }

            if (!ModelState.IsValid)
            {
                PopulateProductOptions(model);
                return View(model);
            }

            int oldQty = product.Quantity;
            product.Quantity += model.Amount.Value;
            db.SaveChanges();

            TempData["Success"] = string.Format(
                "Nhập kho thành công: '{0}' tăng từ {1} lên {2} (+{3}). Trạng thái: {4}.",
                product.ProductName, oldQty, product.Quantity, model.Amount.Value, product.StockStatus);
            return RedirectToAction("Index");
        }

        // Câu 4b: Ghi nhận hàng hỏng - GET
        public ActionResult Damaged()
        {
            var model = new StockAdjustViewModel();
            PopulateProductOptions(model);
            return View(model);
        }

        // Câu 4b: Ghi nhận hàng hỏng - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Damaged(StockAdjustViewModel model)
        {
            if (!model.Amount.HasValue || model.Amount.Value <= 0)
            {
                ModelState.AddModelError("Amount", "Số lượng hàng hỏng phải là số nguyên dương (> 0).");
            }

            Product product = null;
            if (model.ProductID.HasValue)
            {
                product = db.Products.Find(model.ProductID.Value);
                if (product == null)
                {
                    ModelState.AddModelError("ProductID", "Sản phẩm được chọn không tồn tại.");
                }
                else if (model.Amount.HasValue && model.Amount.Value > product.Quantity)
                {
                    ModelState.AddModelError("Amount", string.Format(
                        "Số lượng hỏng ({0}) lớn hơn số lượng hiện có ({1}). Không cập nhật CSDL.",
                        model.Amount.Value, product.Quantity));
                }
            }

            if (!ModelState.IsValid)
            {
                PopulateProductOptions(model);
                return View(model);
            }

            int oldQty = product.Quantity;
            product.Quantity -= model.Amount.Value;
            db.SaveChanges();

            TempData["Success"] = string.Format(
                "Đã ghi nhận hàng hỏng: '{0}' giảm từ {1} xuống {2} (-{3}). Trạng thái: {4}.",
                product.ProductName, oldQty, product.Quantity, model.Amount.Value, product.StockStatus);
            return RedirectToAction("Index");
        }

        // Câu 5: Điều chuyển hàng giữa hai sản phẩm cùng danh mục trong một transaction - GET
        public ActionResult Transfer()
        {
            var model = new TransferViewModel();
            PopulateTransferOptions(model);
            return View(model);
        }

        // Câu 5: Điều chuyển hàng - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transfer(TransferViewModel model)
        {
            if (!model.Quantity.HasValue || model.Quantity.Value <= 0)
            {
                ModelState.AddModelError("Quantity", "Số lượng điều chuyển (q) phải là số nguyên dương (> 0).");
            }

            if (model.SourceProductID.HasValue && model.DestinationProductID.HasValue
                && model.SourceProductID.Value == model.DestinationProductID.Value)
            {
                ModelState.AddModelError("", "Hai sản phẩm nguồn (A) và đích (B) không được trùng ID. Cả hai tồn kho giữ nguyên.");
            }

            if (!ModelState.IsValid)
            {
                PopulateTransferOptions(model);
                return View(model);
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var source = db.Products.Find(model.SourceProductID.Value);
                    var dest = db.Products.Find(model.DestinationProductID.Value);

                    if (source == null || dest == null)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "Không tìm thấy sản phẩm nguồn hoặc sản phẩm đích.");
                        PopulateTransferOptions(model);
                        return View(model);
                    }

                    if (!string.Equals(source.Category, dest.Category, StringComparison.Ordinal))
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", string.Format(
                            "Không thể điều chuyển vì khác danh mục: '{0}' ({1}) và '{2}' ({3}). Cả hai tồn kho giữ nguyên.",
                            source.ProductName, source.Category, dest.ProductName, dest.Category));
                        PopulateTransferOptions(model);
                        return View(model);
                    }

                    if (source.Quantity < model.Quantity.Value)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("Quantity", string.Format(
                            "Sản phẩm nguồn '{0}' chỉ có {1} đơn vị, không đủ để chuyển {2} đơn vị. Cả hai tồn kho giữ nguyên.",
                            source.ProductName, source.Quantity, model.Quantity.Value));
                        PopulateTransferOptions(model);
                        return View(model);
                    }

                    source.Quantity -= model.Quantity.Value;
                    dest.Quantity += model.Quantity.Value;
                    db.SaveChanges();
                    transaction.Commit();

                    TempData["Success"] = string.Format(
                        "Điều chuyển thành công {0} đơn vị trong danh mục '{1}': '{2}' còn {3}, '{4}' tăng lên {5}.",
                        model.Quantity.Value, source.Category,
                        source.ProductName, source.Quantity,
                        dest.ProductName, dest.Quantity);

                    return RedirectToAction("Transfer");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Lỗi trong quá trình giao dịch điều chuyển: " + ex.Message);
                    PopulateTransferOptions(model);
                    return View(model);
                }
            }
        }

        // Câu 6: Thêm sản phẩm mới - GET
        public ActionResult Create()
        {
            return View(new Product { Quantity = 0 });
        }

        // Câu 6: Thêm sản phẩm mới - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductName,Category,Price,Quantity")] Product product)
        {
            if (product.ProductName != null)
            {
                product.ProductName = product.ProductName.Trim();
            }

            if (product.Category != null)
            {
                product.Category = product.Category.Trim();
            }

            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                ModelState.AddModelError("ProductName", "Tên sản phẩm không được để trống.");
            }

            if (string.IsNullOrWhiteSpace(product.Category))
            {
                ModelState.AddModelError("Category", "Danh mục không được để trống.");
            }

            if (product.Price <= 0)
            {
                ModelState.AddModelError("Price", "Đơn giá (Price) phải lớn hơn 0.");
            }

            if (product.Quantity < 0)
            {
                ModelState.AddModelError("Quantity", "Số lượng (Quantity) phải là số nguyên lớn hơn hoặc bằng 0.");
            }

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            db.Products.Add(product);
            db.SaveChanges();
            TempData["Success"] = string.Format(
                "Đã thêm mới sản phẩm '{0}' (ProductID = {1}, Số lượng = {2}).",
                product.ProductName, product.ProductID, product.Quantity);
            return RedirectToAction("Index");
        }

        // Câu 7: Thống kê tồn kho bằng LINQ và Entity Framework
        public ActionResult Statistics()
        {
            var model = new InventoryStatisticsViewModel
            {
                TotalProducts = db.Products.Count(),
                TotalUnitsInStock = db.Products.Sum(p => (int?)p.Quantity) ?? 0,
                LowStockProductsCount = db.Products.Count(p => p.Quantity >= 1 && p.Quantity <= 4),
                OutOfStockProductsCount = db.Products.Count(p => p.Quantity == 0),
                InStockProductsCount = db.Products.Count(p => p.Quantity >= 5),
                CategoryBreakdown = db.Products
                    .GroupBy(p => p.Category)
                    .Select(g => new CategoryStatItem
                    {
                        Category = g.Key,
                        ProductCount = g.Count(),
                        TotalUnits = g.Sum(x => (int?)x.Quantity) ?? 0,
                        LowStockCount = g.Count(x => x.Quantity >= 1 && x.Quantity <= 4),
                        OutOfStockCount = g.Count(x => x.Quantity == 0)
                    })
                    .OrderBy(c => c.Category)
                    .ToList()
            };

            return View(model);
        }

        private void PopulateProductOptions(StockAdjustViewModel model)
        {
            var products = db.Products.OrderBy(p => p.ProductID).ToList();
            model.Products = products;
            model.ProductOptions = products.Select(p => new SelectListItem
            {
                Value = p.ProductID.ToString(),
                Text = string.Format("[ID: {0}] {1} | {2} (Tồn: {3})", p.ProductID, p.ProductName, p.Category, p.Quantity),
                Selected = model.ProductID.HasValue && model.ProductID.Value == p.ProductID
            }).ToList();
        }

        private void PopulateTransferOptions(TransferViewModel model)
        {
            var products = db.Products.OrderBy(p => p.Category).ThenBy(p => p.ProductID).ToList();
            model.Products = products;
            model.ProductOptions = products.Select(p => new SelectListItem
            {
                Value = p.ProductID.ToString(),
                Text = string.Format("[ID: {0}] {1} | Danh mục: {2} (Tồn: {3})", p.ProductID, p.ProductName, p.Category, p.Quantity)
            }).ToList();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
