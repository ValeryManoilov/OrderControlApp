using OrderControlApp.Models;
using OrderControlApp.Dto;
using LanguageExt;

namespace OrderControlApp.Interfaces
{
    public interface IProductManager
    {
        public Task<string> AddProductAsync(AddProductDto addProductData);
        public Task<Either<string, List<GetProductDto>>> GetProductByDateAsync(DateFrameDto getProductData);
    }
}
