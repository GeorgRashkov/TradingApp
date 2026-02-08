
namespace TradingApp.InputModels
{
    public class UpdatedProductModel: CreatedUpdatedProductModel
    {
        public IFormFile? FrontImageFile { get; set; }
        public IFormFile? File3DModel { get; set; }

        public Guid Id { get; set; }

        public override Dictionary<string, IFormFile> GetDictOfImageFiles()
        {
            return new Dictionary<string, IFormFile>()
            {
               { "front", FrontImageFile },
                {"back",BackImageFile },
                {"top",TopImageFile },
                {"bottom",BottomImageFile },
                {"left",LeftImageFile },
                {"right",RightImageFile }
            };
        }
    }
}
