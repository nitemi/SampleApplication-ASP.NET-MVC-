using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using MyFirstAspnetProject.Data;
using MyFirstAspnetProject.Entities;
using MyFirstAspnetProject.Models;
using PagedList;


namespace MyFirstAspnetProject.Controllers
{
    public class ProductsController : Controller
    {
        [HttpGet]
        public ActionResult Index(int? page)
        { 
            var pageSize = 5;
            var context = new MyDbContext(); //dbcontext is our connection to our db
            var productsList = context.Products.ToList().ToPagedList(page ?? 1, pageSize);
            ViewBag.Page = page ?? 1;
            ViewBag.PageSize = pageSize;

            var productModelList = new List<ProductModel>();
            foreach (var product in productsList)
            {
                productModelList.Add(new ProductModel
                {
                    Name = product.Name,
                    Color = product.Color,
                    Quantity = product.Quantity,
                    Description = product.Description,
                    UnitPrice = product.UnitPrice,
                    Id = product.Id
                });
            }


            return View(productModelList);
        }

        [HttpGet] // to view
        public ActionResult AddProduct()
        {
            var context = new MyDbContext();
            var categories = context.Categories.ToList();// Retrieve the list of categories from the db

            var productModel = new ProductModel
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                })
            };

            return View(productModel);
        }

        [HttpPost]//modify state
        [ValidateAntiForgeryToken]//  it is a security features, it protect your code from Csrl attacks
        public ActionResult AddProduct([Bind(Exclude = "Id", Include ="Name, Description, Color, Images, Quantity, UnitPrice, SelectedCategoryId")]
        ProductModel productModel)
        {
            if (!ModelState.IsValid)//validation check
            {
                return View(productModel);
            }
            //Create a list to store the file paths associated with the product
            List<string> imagePaths = new List<string>();

            var context = new MyDbContext();
            //Handles the uploaded images
            if (productModel.Images != null && productModel.Images.Count > 0)
            {
                foreach (var imageFile in productModel.Images)
                {
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        //Save the image to a location of ypur choice, e.g a folder on the server
                        // you can generate a unique file name to avoid overwriting existing images
                        var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(Server.MapPath("~/Images"), fileName);
                        imageFile.SaveAs(filePath);

                        //store the file path or other info in your db
                        //you can associate teh file with the product being added
                        //store the file path in the list
                        imagePaths.Add("Images/" + fileName);
                    }
                }
            }


            //save to db
            //creating instance of entity, Creating a new product entity and set its properties
            var productEntity = new Products()
            {
                Name = productModel.Name,
                Color = productModel.Color,
                Quantity = productModel.Quantity,
                Description = productModel.Description,
                UnitPrice = productModel.UnitPrice,
                Category = context.Categories.Find(productModel.SelectedCategoryId)//set teh category property based on the selected category ID

            };

            context.Products.Add(productEntity);
            context.SaveChanges(); //save or commit to db
                                   //associste the uploaded image file pqaths withtyhe prioduct
            if (imagePaths.Count > 0)
            {
                foreach (var imagePath in imagePaths)
                {
                    var imageEntity = new ProductImage
                    {
                        ProductId = productEntity.Id,
                        ImagePath = imagePath,
                    };

                    context.ProductImages.Add(imageEntity);
                }
                context.SaveChanges();
            }
            //redirect or navigate to the index page
            return RedirectToAction("Index");
            //return Json(Url.Action("Index", "Products"));
        }
        [HttpGet]
        public ActionResult Edit(int id)//read from the db
        {
            var context = new MyDbContext();
            var product = context.Products.Find(id);

            if (product == null)//use to check
            {
                return HttpNotFound();//404 not found response if product is not found
            }

            var categories = context.Categories.ToList();//retreive list of categories from the db




            var productModel = new ProductModel
            {
                Name = product.Name,
                Color = product.Color,
                Quantity = product.Quantity,
                Description = product.Description,
                UnitPrice = product.UnitPrice,
                Id = product.Id,
                SelectedCategoryId = product.Category == null ? 0 : product.Category.Id, // Set the selected category ID
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            return View(productModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductModel productModel)
        {
            if (!ModelState.IsValid)
            {
                return View(productModel);
            }

            var context = new MyDbContext();
            var productEntity = context.Products.Find(productModel.Id);//to check if the product is still there

            if (productEntity == null)
            {
                return HttpNotFound();
            }
            var categoryEntity = context.Categories.Find(productModel.SelectedCategoryId);
            if(categoryEntity == null)
            {
                //add a custom validation error to modelstate
                ModelState.AddModelError("SelectedCategoryId", "The Selected category is not valid. Please select a valid category.");

                return View(productModel);
            }
            //lets update it
            productEntity.Name = productModel.Name;
            productEntity.Color = productModel.Color;
            productEntity.Quantity = productModel.Quantity;
            productEntity.Description = productModel.Description;
            productEntity.UnitPrice = productModel.UnitPrice;

            context.SaveChanges(); //commit to db

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var context = new MyDbContext();
            var productEntity = context.Products.Find(id);
            if (productEntity == null)
            {
                return HttpNotFound();
            }

            //remove product entity from db
            context.Products.Remove(productEntity);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ProductDetails(int id)
        {
            var context = new MyDbContext();
            var product = context.Products.Find(id);

            if (product == null)
            {
                return HttpNotFound(); // Return a 404 Not Found response if the product is not found.
            }

            // Create a ProductModel to pass the product details to the view
            var productModel = new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Color = product.Color,
                Quantity = product.Quantity,
                UnitPrice = product.UnitPrice,
                SelectedCategoryId = product.Category.Id // Set the selected category ID
            };

            // You also need to retrieve the associated images and add them to the Images property
            // You can do this by querying the database for images associated with the product

            // Example: Retrieve image paths from the ProductImages table for the product
            var imagePaths = context.ProductImages.Where(pi => pi.ProductId == id).Select(pi => pi.ImagePath).ToList();

            // Set the image paths in the ProductModel
            productModel.ImagePaths = imagePaths;
       

            return View(productModel);
        }

    }
}

/*[HttpGet]
        public ActionResult ProductDetails(int id)
        {

        var context = new MyDbContext();
            var product = context.Products.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            //Create a productmodel to pass the product details to the view
            var productModel = new ProductModel
            {
        Name = product.Name,
        Color = product.Color,
        Quantity = product.Quantity,
        Description = product.Description,
        UnitPrice = product.UnitPrice,
        Id = product.Id,
        SelectedCategoryId = product.Category.Id,//set the selected category
    };
      //you also need to retrieve the associated images ad add them to the images property
      //you can do this by querying the db for images associated with the product

            //examples: retrieve image paths from the productimages table fpr the product
             var imagePaths = context.ProductImages.Where(p => p.ProductId == id).Select(p=> p.ImagePath).ToList();

            //set the image paths in the productModel
            productModel.ImagePaths = imagePaths;

            return View(productModel);
    }
}
}*/
