using TradingApp.InputModels;

namespace TradingApp.Services
{
    public class CrudFile
    {
        public async Task SaveProductInFolder(CreatedProductModel product, string creatorName, bool createUser)
        {
            string creatorPath = Path.Combine("wwwroot", "Creators", creatorName);
            if (createUser == true)
            { Directory.CreateDirectory(creatorPath); }

            string productPath = Path.Combine(creatorPath, product.ProductName);
            Directory.CreateDirectory(productPath);

            Dictionary<string, IFormFile> imageFiles = product.GetDictOfImageFiles();

            foreach ((string imageName, IFormFile imageFile) in imageFiles)
            {
                if (imageFile == null)
                { continue; }

                string imagePath = Path.Combine(productPath, imageName + ".jpg");
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
            }

            string product3DModelPath = Path.Combine(productPath, product.ProductName + ".jpg");
            using (var stream = new FileStream(product3DModelPath, FileMode.Create))
            { await product.File3DModel.CopyToAsync(stream); }
        }
    }
}
