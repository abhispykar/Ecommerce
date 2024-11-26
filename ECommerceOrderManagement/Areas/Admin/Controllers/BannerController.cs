using EOMS.DataAccess.Repository.IRepository;
using EOMS.Utility;
using EOMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace ECommerceOrderManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class BannerController : Controller
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BannerController(IBannerRepository bannerRepository, IWebHostEnvironment webHostEnvironment)
        {
            _bannerRepository = bannerRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Banner> objBannerList = _bannerRepository.GetAll().ToList();
            return View(objBannerList);
        }

        public IActionResult Upsert(int? id)
        {
            Banner banner = new Banner();
            if (id == null)
            {
                return View(banner);
            }
            else
            {
                banner = _bannerRepository.GetT(b => b.Id == id);
                if (banner == null)
                {
                    return NotFound();
                }
            }
            return View(banner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Banner banner, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string bannerPath = Path.Combine(wwwRootPath, @"images/banner");

                    if (!string.IsNullOrEmpty(banner.ImageUrl))
                    {
                        //delete the old image

                        var oldBannerPath = Path.Combine(wwwRootPath, banner.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldBannerPath))
                        {
                            System.IO.File.Delete(oldBannerPath);
                        }

                    }

                    using (var filestream = new FileStream(Path.Combine(bannerPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    banner.ImageUrl = @"/images/banner/" + fileName;
                }

                if (banner.Id == 0)
                {
                    banner.CreatedDate = DateTime.Now;
                    _bannerRepository.Add(banner);
                    TempData["success"] = "Banner saved successfully";
                }
                else
                {
                    _bannerRepository.Update(banner);
                    TempData["success"] = "Banner updated successfully";
                }
                _bannerRepository.Save();
                return RedirectToAction("Index");

            }
            TempData["error"] = "Failed to save banner. Please check the form and try again.";
            return View(banner);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult Delete(int id)
        {
            var banner = _bannerRepository.GetT(b => b.Id == id);
            if (banner == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, banner.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _bannerRepository.Remove(banner);
            _bannerRepository.Save();
            return Json(new { success = true, message = "Delete successful" });
        }
    }
}