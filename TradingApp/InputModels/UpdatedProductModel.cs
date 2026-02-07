
namespace TradingApp.InputModels
{
    public class UpdatedProductModel: CreatedProductModel
    {
        public override IFormFile FrontImageFile { get; set; } = null!;
        public override IFormFile File3DModel { get; set; } = null!;
    }
}
