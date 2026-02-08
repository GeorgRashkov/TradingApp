using System.ComponentModel.DataAnnotations;
using TradingApp.Common;
namespace TradingApp.InputModels
{
    public class CreatedProductModel:CreatedUpdatedProductModel
    {
        [Required]
        public IFormFile FrontImageFile { get; set; } = null!;

        [Required]
        public IFormFile File3DModel { get; set; } = null!;

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
