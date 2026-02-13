using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using TradingApp.GCommon;
namespace TradingApp.Data.Models
{
    public class Balance
    {
        [Key]
        [ForeignKey(nameof(User))]
        public string Id { get; set; } = null!;
       
        [Column(TypeName = EntityValidation.Balance.AmountDbType)]
        public decimal Amount { get; set; }



        public virtual User User { get; set; } = null!;
    }
}
