using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using TradingApp.GCommon;
namespace TradingApp.ViewModels.InputProduct
{
    public abstract class CreatedUpdatedProductModel
    {
        public IFormFile? BackImageFile { get; set; }

        public IFormFile? TopImageFile { get; set; }

        public IFormFile? BottomImageFile { get; set; }

        public IFormFile? LeftImageFile { get; set; }

        public IFormFile? RightImageFile { get; set; }


        [Required]
        [MinLength(EntityValidation.Product.NameMinLength)]
        [MaxLength(EntityValidation.Product.NameMaxLength)]
        [RegularExpression(EntityValidation.Product.NameRegex)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [MinLength(EntityValidation.Product.DescriptionMinLength)]
        [MaxLength(EntityValidation.Product.DescriptionMaxLength)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(EntityValidation.Product.PriceMinValue, EntityValidation.Product.PriceMaxValue)]
        public decimal Price { get; set; }


        public abstract Dictionary<string, IFormFile> GetDictOfImageFiles();
       
    }
}
