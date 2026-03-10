using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TI_Devops2026_DemoAspCrud.Contexts;
using TI_Devops2026_DemoAspCrud.Entities;
using TI_Devops2026_DemoAspCrud.Mappers;
using TI_Devops2026_DemoAspCrud.Models;

namespace TI_Devops2026_DemoAspCrud.Controllers
{
    public class ProductController : Controller
    {

        private readonly DemoAspCrudContext _demoAspCrudContext;

        public ProductController(DemoAspCrudContext demoAspCrudContext)
        {
            _demoAspCrudContext = demoAspCrudContext;
        }

        public IActionResult Index()
        {
            //List<Product> products = _demoAspCrudContext.Products
            //    .Include(p => p.Category)
            //    .ToList();

            //List<ProductIndexResponse> dtos = products
            //    .Select(p => p.ToProductIndexResponse())
            //    .ToList();

            List<ProductIndexResponse> dtos = _demoAspCrudContext.Products
                .Include(p => p.Category)
                .Select(p => p.ToProductIndexResponse())
                .ToList();

            return View(dtos);
        }
    }
}
