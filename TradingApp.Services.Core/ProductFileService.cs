using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic.FileIO;

using TradingApp.Services.Core.Interfaces;
using TradingApp.ViewModels.InputProduct;


namespace TradingApp.Services.Core
{
    public class ProductFileService: IProductFileService
    {
        public async Task SaveProductInFolderAsync(CreatedProductModel product, string creatorName)
        {
            string creatorPath = Path.Combine("wwwroot", "Creators", creatorName);            
            Directory.CreateDirectory(creatorPath);

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

        public async Task UpdateProductInFolderAsync(UpdatedProductModel product, string creatorName, string oldProductName)
        {
            string creatorPath = Path.Combine("wwwroot", "Creators", creatorName);

            if (product.ProductName != oldProductName)
            {
                //rename the 3D model file
                FileSystem.RenameFile(
                    file: Path.Combine(creatorPath, oldProductName, oldProductName + ".jpg"),
                    newName: Path.Combine(product.ProductName + ".jpg")
                );

                //remane the product directory
                FileSystem.RenameDirectory(
                    directory: Path.Combine(creatorPath, oldProductName),
                    newName: Path.Combine(product.ProductName)
                );
            }

            //gets the path to the product directory (inclusive)
            string productPath = Path.Combine(creatorPath, product.ProductName);

            Dictionary<string, IFormFile> imageFiles = product.GetDictOfImageFiles();

            //this code replaces old images of the product with the new ones
            foreach ((string imageName, IFormFile imageFile) in imageFiles)
            {
                if (imageFile is null)
                { continue; }

                string imagePath = Path.Combine(productPath, imageName + ".jpg");
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
            }

            //this code replaces the 3D model file of the product with with the new one
            if (product.File3DModel is not null)
            {
                string product3DModelPath = Path.Combine(productPath, product.ProductName + ".jpg");
                using (var stream = new FileStream(product3DModelPath, FileMode.Create))
                { await product.File3DModel.CopyToAsync(stream); }
            }



        }


        public void DeleteProductFolder(string creatorName, string productName)
        {
            string productPath = GetProductPath(creatorName, productName);
            Directory.Delete(productPath, true);
        }


        public byte[] Get3dModelFileBytes(string creatorName, string productName)
        {
            string productPath = Path.Combine(GetProductPath(creatorName, productName), productName + ".jpg");
            byte[] bytes = File.ReadAllBytes(productPath);
            return bytes;
        }

        private string GetProductPath(string creatorName, string productName)
        {
            string productPath = Path.Combine("wwwroot", "Creators", creatorName, productName);
            if (Directory.Exists(productPath) == false)
            {
                throw new DirectoryNotFoundException(productPath);
            }
            return productPath;
        }
    }
}
