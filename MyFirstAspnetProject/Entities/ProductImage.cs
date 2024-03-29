using System.ComponentModel.DataAnnotations;

namespace MyFirstAspnetProject.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(250)]//Adjust the maximum length as needed
        public string ImagePath { get; set; }
    }
}