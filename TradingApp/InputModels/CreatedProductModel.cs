using System.ComponentModel.DataAnnotations;
using TradingApp.Common;
namespace TradingApp.InputModels
{
    public class CreatedProductModel
    {
        [Required]
        public IFormFile FrontImageFile { get; set; } = null!;

       
        public IFormFile? BackImageFile { get; set; } = null!;
                
        public IFormFile? TopImageFile { get; set; } = null!;
               
        public IFormFile? BottomImageFile { get; set; } = null!;
                
        public IFormFile? LeftImageFile { get; set; } = null!;
               
        public IFormFile? RightImageFile { get; set; } = null!;


        [Required]
        public IFormFile File3DModel { get; set; } = null!;


        [Required]
        [MinLength(EntityValidation.Product.NameMinLength)]
        [MaxLength(EntityValidation.Product.NameMaxLength)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [MinLength(EntityValidation.Product.DescriptionMinLength)]
        [MaxLength(EntityValidation.Product.DescriptionMaxLength)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(EntityValidation.Product.PriceMinValue, EntityValidation.Product.PriceMaxValue)]
        public decimal Price { get; set; }   
        

        public Dictionary<string, IFormFile> GetDictOfImageFiles()
        {
            return new Dictionary<string, IFormFile>()
            {
               { "front", FrontImageFile },
                {"back",BackImageFile },
                {"top",TopImageFile },
                {"bottm",BottomImageFile },
                {"left",LeftImageFile },
                {"right",RightImageFile }
            };
        }
    }
}
