
using System.ComponentModel.DataAnnotations;
using TradingApp.GCommon;

namespace TradingApp.ViewModels.InputOrderRequest
{
    public class CreatedOrderRequestModel
    {
        [Required]
        [MinLength(EntityValidation.Order.TitleMinLength)]
        [MaxLength(EntityValidation.Order.TitleMaxLength)]
        public string Title { get; set; } = null!;


        [Required]
        [MinLength(EntityValidation.Order.DescriptionMinLength)]
        [MaxLength(EntityValidation.Order.DescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [Required]
        [Range(EntityValidation.Order.PriceMinValue, EntityValidation.Order.PriceMaxValue)]
        public decimal MaxPrice { get; set; }
    }
}
