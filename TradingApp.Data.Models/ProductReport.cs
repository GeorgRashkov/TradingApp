
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradingApp.GCommon;
using TradingApp.GCommon.Enums;

namespace TradingApp.Data.Models
{
    public class ProductReport
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [ForeignKey(nameof(Reporter))]
        public string ReporterId { get; set; } = null!;

        [Required]
        //[ForeignKey(nameof(Product))]
        public Guid ReportedProductId {  get; set; }

        [Required]
        [MaxLength(EntityValidation.ProductReport.TitleMaxLength)]
        public string Title { get; set; }

        [Required]
        [MaxLength(EntityValidation.ProductReport.MessageMaxLength)]
        public string Message { get; set; }

        [Required]
        [Column(TypeName = EntityValidation.ProductReport.DateType)]
        public DateTime CreatedAt { get; set; }

        [Required]
        public ProductReportType Type { get; set; }

        [Required]
        public ProductReportStatus Status { get; set; }



        public virtual User Reporter { get; set; } = null!;        
        public virtual Product Product { get; set; } = null!;


    }
}
