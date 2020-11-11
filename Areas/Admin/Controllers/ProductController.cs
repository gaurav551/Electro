using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Electro.Models;
using System.IO;
using System.Drawing;
using Electro.Data;
using NToastNotify;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Authorization;

namespace Electro.Areas.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public ProductController(ILogger<ProductController> logger,
        ApplicationDbContext context,
         IToastNotification toastNotification)
        {
            _logger = logger;
            _context = context;
            _toastNotification = toastNotification;
        }


        public async Task<IActionResult> Index(string search, int page=1)
        {
            TempData["isactive1"] = "active";
            ViewData["search"] = search;
            if(string.IsNullOrEmpty(search))
            {
            var a =  _context.Products.OrderByDescending(x => x.Id);
            var model = await PagingList.CreateAsync(a, 15, page);
            model.Action = "Index";
             return View(model);
            }
            else
            {
            var a =  _context.Products.Where(x=>x.Name.Contains(search)).OrderByDescending(x => x.Id);
            var model = await PagingList.CreateAsync(a, 15, page);
            model.Action = "Index";
             return View(model);
            }



        }

         [HttpGet]
        public IActionResult New()
        {
             TempData["isactive3"] = "active";
             var pr = new Product();
            return View(pr);
        }
         [HttpPost]
        public async Task<IActionResult> New(Product p)
        {

            var files = HttpContext.Request.Form.Files;
            if(files.Count<3)
            {
                ViewData["noimage"] = "Please select at least two or more images";
                return View();
            }
            foreach (var Photo in files)
            {
                if (Photo != null && Photo.Length > 0)
                {
                    var file = Photo;
                    var filename = RandomString(3) + file.FileName;
                   if (file.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            Bitmap original = (Bitmap)Image.FromStream(memoryStream);
                            //For Resize
                            Bitmap processed = new Bitmap(original, new Size(700,700));
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/products", filename);
                            processed.Save(path);
                            memoryStream.Flush();
                            p.Images = p.Images + ";" + filename;
                        }
                    }
                }
            }
            p.AddedBy = "Admin";

            _context.Products.Add(p);
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage(p.Name + " is Added");
            // Thread.Sleep(10000);
            //         return Json(new { success = true });
            return RedirectToAction(nameof(Index));
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
         public IActionResult Category()
        {
            TempData["isactive2"] = "active";
            var a =  _context.Categories.ToList();

            return View(a);
        }

         public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x=>x.Id==id);
            var images = product.Images.Split(';').ToList();
            foreach(var img in images)
            {
              var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/products", img);
              if(System.IO.File.Exists(path))
              {
              System.IO.File.Delete(path);
              }


            }
            _context.Remove(product);
           await  _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
         public IActionResult Edit(int id)
        {
            var product = _context.Products.FirstOrDefault(x=>x.Id==id);

            return View(product);
        }
        [HttpPost]
         public IActionResult Edit(Product p)
        {
            var oldproduct = _context.Products.AsNoTracking().FirstOrDefault(x=>x.Id==p.Id);
             p.AddedBy = oldproduct.AddedBy;
             p.Images = oldproduct.Images;
             p.AddedOn = oldproduct.AddedOn;
             p.Rating = oldproduct.Rating;
            _context.Products.Update(p);
            _context.SaveChanges();
            _toastNotification.AddSuccessToastMessage(p.Name + " Updated");
            // Thread.Sleep(10000);
            //         return Json(new { success = true });
            return RedirectToAction(nameof(Index));

        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
