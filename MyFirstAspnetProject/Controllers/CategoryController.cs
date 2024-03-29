using MyFirstAspnetProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using MyFirstAspnetProject.Models;
using MyFirstAspnetProject.Entities;
namespace MyFirstAspnetProject.Controllers
{
    public class CategoryController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var context = new MyDbContext();
            var categoryList = context.Categories.ToList();

            var productModelList = new List<CategoryModel>();
            var productsModelList = new List<CategoryModel>();
            foreach (var product in categoryList)
            {
                productsModelList.Add(new CategoryModel
                {
                    Name = product.Name,
                    Description = product.Description,
                    Id = product.Id
                });
            }
            return View(productModelList);
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory([Bind(Exclude = "Id")] CategoryModel categoryModel)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryModel);
            }



            //Save product to database

            var context = new MyDbContext();


            // Create a new Category entity and set its properties
            var categoryEntity = new Category
            {
                Name = categoryModel.Name,
                Description = categoryModel.Description,
            };


            context.Categories.Add(categoryEntity);
            context.SaveChanges();



            //redirect or navigate to the index page
            return RedirectToAction("Index");

        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var context = new MyDbContext();
            var product = context.Categories.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var categoryModel = new CategoryModel
            {
                Name = product.Name,
                Id = product.Id,
                Description = product.Description,

            };

            return View(categoryModel);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategoryModel categoryModel)
        {
            
            if (!ModelState.IsValid)
            {
                return View(categoryModel);

            }

            var context = new MyDbContext();
            var categoryEntity = context.Categories.Find(categoryModel.Id);

            if (categoryEntity == null)
            {
                return HttpNotFound();
            }

            //Lets update it
            categoryEntity.Name = categoryModel.Name;
            categoryEntity.Description = categoryModel.Description;

            context.SaveChanges();

            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var context = new MyDbContext();
            var categoryEntity = context.Categories.Find(id);


            if (categoryEntity == null)
            {
                return HttpNotFound();
            }


            //Remove the product entity from the database
            context.Categories.Remove(categoryEntity);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}