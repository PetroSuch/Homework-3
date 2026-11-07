using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductsWithRouting.Models;
using ProductsWithRouting.Services;

namespace ProductsWithRouting.Controllers
{
    public class ProductsController : Controller
    {
        private List<Product> myProducts;

        public ProductsController(Data data)
        {
            myProducts = data.Products;
        }

        [HttpGet("/products/index")]
        [HttpGet("/products")]
        [HttpGet("/items/index")]
        [HttpGet("/items")]
        public IActionResult Index(int? filterId, string filtername)
        {
            IEnumerable<Product> products = myProducts;

            if(filterId != null)
            {
                products = products.Where((item) => item.Id == filterId);
            }

            if(!string.IsNullOrWhiteSpace(filtername))
            {
                products = products.Where((item) => item.Name == filtername);
            }
            
            return View(products);
        }

        [HttpGet("/products/{id:int}")]
        public IActionResult View(int id)
        {
            Product product = GetProductById(id);

            if (product == null)
            {
                return RedirectToAction(nameof(ProductNotFound));
            }

            return View(product);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Product product = GetProductById(id);

            if (product == null)
            {
                return RedirectToAction(nameof(ProductNotFound));
            }

            return View(product);
        } 

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if(!ModelState.IsValid)
            {
                return View(product);
            }

            Product productItem = GetProductById(product.Id);

            if (productItem == null)
            {
                return RedirectToAction(nameof(ProductNotFound));
            }

            productItem.Name = product.Name;
            productItem.Description = product.Description;
            
            return RedirectToAction(nameof(Index));
        } 
        
        [HttpPost("/products/create")]
        [HttpPost("/products/new")]
        public IActionResult Create(Product product)
        {
            if(!ModelState.IsValid)
            {
                return View(product);
            }

            CreateProduct(product);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("/products/create")]
        [HttpGet("/products/new")]
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Delete(int id)
        {
            Product productItem = GetProductById(id);

            if (productItem == null)
            {
                return RedirectToAction(nameof(ProductNotFound));
            }

            myProducts.Remove(productItem);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult ProductNotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void CreateProduct(Product product)
        {
            int maxId = 0;

            if(myProducts.Count > 0)
            {
                maxId = myProducts.Max((item) => item.Id);
            }

            product.Id = maxId + 1;

            myProducts.Add(product);
        }

        private Product GetProductById(int id)
        {
            return myProducts.FirstOrDefault((item) => item.Id == id);
        }
    }
}
