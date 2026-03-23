
using System.ComponentModel.DataAnnotations;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;

namespace TradingApp.ViewModels.Input_ProductReport
{
    public class Created_ProductReportModel
    {
        [Required]
        public Guid ReportedProductId { get; set; }

        [Required]
        [MinLength(EntityValidation.ProductReport.TitleMinLength)]
        [MaxLength(EntityValidation.ProductReport.TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MinLength(EntityValidation.ProductReport.MessageMinLength)]
        [MaxLength(EntityValidation.ProductReport.MessageMaxLength)]
        public string Message { get; set; } = null!;

        [Required]
        public ProductReportType Type { get; set; }
    }
}
