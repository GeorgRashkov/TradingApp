
using System.ComponentModel.DataAnnotations;

namespace TradingApp.GCommon.Filters
{
    public class ProductFilter
    {        
        [MaxLength(EntityValidation.Product.NameMaxLength)]
        [RegularExpression(EntityValidation.Product.NameRegex)]
        public string? ProductName { get; set; } = null!;
        public string? CreatorName { get; set; } = null!;

        [Range(EntityValidation.Product.PriceMinValue, EntityValidation.Product.PriceMaxValue)]
        public double MinPrice { get; set; } = EntityValidation.Product.PriceMinValue;

        [Range(EntityValidation.Product.PriceMinValue, EntityValidation.Product.PriceMaxValue)]
        public double MaxPrice { get; set; } = EntityValidation.Product.PriceMaxValue;
    }
}
