using System.Linq;
using System.Threading.Tasks;
using Electro.Data;
using Microsoft.AspNetCore.Mvc;

namespace Electro.Views.Shared.Component
{
    [ViewComponent(Name = "ProductView")]
    public class ProductViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public ProductViewComponent(ApplicationDbContext context)
        {
            _context = context;

        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.topselleing =  _context.Products.OrderByDescending(x=>x.Id).Take(10).Skip(5);
            ViewBag.bestdeals =  _context.Products.OrderByDescending(x=>x.Id).Take(13).Skip(10);
            ViewBag.bestdeals2 =  _context.Products.OrderByDescending(x=>x.Id).Take(16).Skip(13);

            return View();
        }
    }
}