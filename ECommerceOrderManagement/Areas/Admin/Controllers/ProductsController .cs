using EOMS.DataAccess.Repository;
using EOMS.DataAccess.Repository.IRepository;
using EOMS.Models;
using EOMS.Models.ViewModels;
using EOMS.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class ProductsController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductsController(IProductRepository db, ICategoryRepository categoryRepository, IWebHostEnvironment webHostEnvironment)
    {
        _productRepository = db;
        _categoryRepository = categoryRepository;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        List<Product> objProductsList = _productRepository.GetAll(includeProperties:"Category").ToList();
        return View(objProductsList);
    }

    public IActionResult Upsert(int? id)
    {
        ProductVM productVM = new()
        {
            CategoryList = _categoryRepository.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            }),
            Product = new Product()
        };

        if (id == null || id == 0)
        {
            return View(productVM);
        }
        else
        {
            productVM.Product = _productRepository.Get(u => u.ProductId == id);
            return View(productVM);
        }
    }

    [HttpPost]
    public IActionResult Upsert(ProductVM productVM, IFormFile? file)
    {
        if (ModelState.IsValid)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string productPath = Path.Combine(wwwRootPath, @"images\product");

                if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                {
                    // delete the old image
                    var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var filestream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                {
                    file.CopyTo(filestream);
                }
                productVM.Product.ImageUrl = @"\images\product\" + fileName;
            }

            if (productVM.Product.ProductId == 0)
            {
                _productRepository.Add(productVM.Product);
            }
            else
            {
                _productRepository.Update(productVM.Product);
            }

            _productRepository.Save();
            TempData["success"] = "Product created successfully";
            return RedirectToAction("Index");
        }

        productVM.CategoryList = _categoryRepository.GetAll().Select(u => new SelectListItem
        {
            Text = u.Name,
            Value = u.Id.ToString()
        });

        return View(productVM);
    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        Product productFromDb = _productRepository.Get(u => u.ProductId == id);
        if (productFromDb == null)
        {
            return NotFound();
        }

        return View(productFromDb);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePOST(int? id)
    {
        Product? obj = _productRepository.Get(u => u.ProductId == id);
        if (obj == null)
        {
            return NotFound();
        }

        _productRepository.Remove(obj);
        _productRepository.Save();
        TempData["success"] = "Product deleted successfully";
        return RedirectToAction("Index");
    }
}
