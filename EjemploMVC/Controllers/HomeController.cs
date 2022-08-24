using EjemploMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AdventureWorksNS.Data;

namespace EjemploMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AdventureWorksDB db;

        public HomeController(ILogger<HomeController> logger, 
                            AdventureWorksDB injectedContext)
        {
            _logger = logger;
            db = injectedContext;
        }

        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any)]
        public IActionResult Index()
        {
            _logger.LogError("Esto es un error");
            _logger.LogWarning("Esto es un warning");
            _logger.LogInformation("Esto solo es un mensaje");
            HomeIndexViewModel model = new();
            model.ContadorVisitas = (new Random()).Next(1, 1001);
            model.Productos = db.Products.ToList();
            model.Categorias = db.ProductCategories.ToList();

            return View(model);
        }

        [Route("privo")]
        [Authorize(Roles="Administrators")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult DetalleProducto(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("Se debe colocar un ID del producto en la ruta, por ejemplo /Home/DetalleProducto/12");
            }
            Product? model = db.Products.SingleOrDefault(p => p.ProductId == id);
            if (model == null)
            {
                return NotFound($"El producto con el id {id} no se pudo encontrar");
            }
            return View(model);
        }

        public IActionResult ProductosConPrecioMayorA(decimal? precio)
        {
            if (!precio.HasValue)
            {
                return BadRequest("Se debe colocar un precio en la ruta, por ejemplo /home/ProductosConPrecioMayorA/500");
            }
            IEnumerable<Product> model = db.Products
                    .Where(p => p.ListPrice >= precio);
            if (!model.Any())
            {
                return NotFound($"No hay productos que cuesten mas que {precio:C}");
            }
            ViewData["PrecioMaximo"] = precio.Value.ToString("C");
            return View(model);
        }

        public IActionResult DetalleCategoria(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("Se debe colocar un ID de categoria en la ruta, por ejemplo /Home/DetalleCategoria/12");
            }

            try
            {
                IEnumerable<Product> model = db.Products.Where(c => c.ProductCategoryId == id);

                ProductCategory? prueba = db.ProductCategories.Where(c => c.ProductCategoryId == id).First();

                if (prueba != null)
                {
                    ViewBag.nombre = prueba.Name;
                    ViewBag.ID = id;
                }
                if (model == null)
                {
                    return NotFound($"la cateogoria con el id {id} no se pudo encontrar");
                }
                return View(model);


            }
            catch (Exception e)
            {
                return BadRequest("Se debe colocar un ID de categoria en la ruta, por ejemplo /Home/DetalleCategoria/12");
            }

            
        }
    }
}