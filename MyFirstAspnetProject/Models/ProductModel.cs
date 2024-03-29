using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyFirstAspnetProject.Models
{
    //The product model is the one use to pass data or receive data from the UI
    public class ProductModel
    {
        public int Id { get; set; } = 0;
        [Required]
        public string Name { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Description must not be more than 100 words long")]
        public string Description { get; set; }
        public string Color { get; set; }
        [Range(0, 500)]
        public int Quantity { get; set; }
        [DisplayName("Unit Price")]
        public decimal UnitPrice { get; set; }
        [DisplayName("Category")]
        [Required(ErrorMessage = "Please select a category")]//Add Required Attribute
        [Range(1, int.MaxValue, ErrorMessage ="Please select a valid category")]//Use Range to ensure the value is greater than 0
       
        public int SelectedCategoryId {  get; set; }//Properties to hold the selected categories
        
        public IEnumerable<SelectListItem>Categories { get; set; }// List of availale categories, use to display category


        public List<string> ImagePaths { get; set; }
        [DisplayName("Image")]
        public List<HttpPostedFileBase> Images { get; set; }//Property for image upload
    }
}