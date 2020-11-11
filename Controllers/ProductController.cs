using System.Linq;
using System.Threading.Tasks;
using Electro.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NToastNotify;
using ReflectionIT.Mvc.Paging;

namespace Electro.Controllers
{
    public class ProductController : Controller
    {

        private readonly ILogger<ProductController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        private readonly IDataProtector _protector;

        public ProductController(ILogger<ProductController> logger,
        ApplicationDbContext context,
         IToastNotification toastNotification,
        IDataProtectionProvider dataProtectionProvider)
        {
            _logger = logger;
            _context = context;
            _toastNotification = toastNotification;

        }


        public IActionResult Index()
        {
            TempData["isactivehome"] = "active";
            ViewBag.newproducts = _context.Products.OrderByDescending(x=>x.Id).Take(5);

            // var a =  _context.Products.ToList();

            return View();
        }
         [HttpGet]
         public IActionResult Details(int id)
         {
             var a = _context.Products.FirstOrDefault(x=>x.Id==id);
             ViewBag.related = _context.Products.Where(x=>x.Category.Equals(a.Category) && x.Id!=a.Id).Take(4);
             return View(
                a
             );

         }
         public IActionResult Category(string id)
         {
              TempData[id]="active";

              ViewBag.category = _context.Products.Where(x=>x.Category.Equals(id)).ToList();
              ViewBag.thiscat = id;
              return View();
         }
         public IActionResult Search(string category, string keyword)
         {
             if((category==null) && (keyword==null))
             {
                 return NoContent();
             }
             TempData["k"] = keyword;
             TempData["c"] = category;


             return View(
                 _context.Products.Where((
                 x=>(
                    (string.IsNullOrEmpty(category)) || x.Category.Contains(category)) &&
                    (string.IsNullOrEmpty(keyword)) || x.Name.Contains(keyword)))
                    .Take(5).ToList()
             );
         }
         public async Task<IActionResult> Store(string search, int page=1)
        {
            TempData["isactivestore"] = "active";
            ViewData["search"] = search;
            if(string.IsNullOrEmpty(search))
            {
            var a =  _context.Products.OrderByDescending(x => x.Id);
            var model = await PagingList.CreateAsync(a, 9, page);
            model.Action = "Store";
             return View(model);
            }
            else
            {
            var a =  _context.Products.Where(x=>x.Name.Contains(search)).OrderByDescending(x => x.Id);
            var model = await PagingList.CreateAsync(a, 9, page);
            model.Action = "Store";
             return View(model);
            }
         }



    }
}